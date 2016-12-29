using System;
using System.Collections.Generic;
using CryptoSQLite.Extensions;

namespace CryptoSQLite.CryptoProviders
{
    internal class AesCryptoProvider : ICryptoProvider
    {
        private byte[] _key;
        private readonly byte[] _solt;
        private readonly Aes _aes;

        public AesCryptoProvider(Aes.AesKeyType keyType)
        {
            _aes = new Aes(keyType);
            _solt = new byte[16];
        }

        private byte[] GetGamma(int count)
        {
            if (count < 1)
                throw new ArgumentException();

            var takts = count / 16;
            if (count % 16 > 0)
                takts += 1;

            var gamma = new byte[takts * 16];

            for (var t = 0; t < takts; t++)
            {
                var tmp = _aes.ElectronicCodeBook(_solt);    // get gamma from gost

                _solt.Xor(tmp, 16);
                
                gamma.MemCpy(tmp, 16, 16*t);

                tmp.ZeroMemory();
            }

            return gamma;
        }

        public void XorGamma(byte[] data, int dataLen = 0)
        {
            if (_key == null)
                throw new NullReferenceException("Encryption key has not been installed.");
            if (_solt == null)
                throw new NullReferenceException("Solt has not been installed.");

            var len = dataLen == 0 ? data.Length : dataLen;

            var gamma = GetGamma(len);

            data.Xor(gamma, len);

            gamma.ZeroMemory();     // clean gamma
        }

        public void SetKey(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _key = key;

            _aes.SetKey(key, Aes.AesKeyType.Aes256);
        }

        public void SetSolt(byte[] solt)
        {
            if (solt == null)
                throw new ArgumentNullException(nameof(solt));
            if (solt.Length < 8)
                throw new ArgumentException("Solt must contain at least 8 baytes");

            _solt.MemCpy(solt, solt.Length);
            _solt.MemCpy(solt, solt.Length, 8);
            _solt.Xor(0xFF, 8);
        }

        public void Dispose()
        {
            _key = null;
            _aes?.Dispose();
        }
    }

    #region AES Implementation

    internal class Aes : IDisposable
    {

        public enum AesKeyType
        {
            Aes128 = 4,
            Aes192 = 6,
            Aes256 = 8
        }

        #region Class members

        private AesKeyType _aesKeyType;
        private int _nRounds;
        private uint[] _key;

        #endregion // Class members


        #region Constructors

        public Aes()
        {
            _aesKeyType = AesKeyType.Aes256;
            _nRounds = 14;
            _key = null;
        }

        public Aes(AesKeyType aesKeyType)
        {
            _key = null;
            _aesKeyType = aesKeyType;
            switch (aesKeyType)
            {
                case AesKeyType.Aes128:
                    _nRounds = 10;
                    break;
                case AesKeyType.Aes192:
                    _nRounds = 12;
                    break;
                case AesKeyType.Aes256:
                    _nRounds = 14;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(aesKeyType), aesKeyType, null);
            }
        }
        #endregion //Constructors


        #region Class Interface


        public bool SetKey(byte[] key, AesKeyType keyType)
        {
            var tmp = new uint[8];

            for (var i = 0; i < tmp.Length; i++)
                tmp[i] = BitConverter.ToUInt32(key, 4 * i);

            _aesKeyType = keyType;
            switch (keyType)
            {
                case AesKeyType.Aes128:
                    if (key.Length < 4*(int)AesKeyType.Aes128) return false;
                    _nRounds = 10;
                    break;
                case AesKeyType.Aes192:
                    if (key.Length < 4*(int)AesKeyType.Aes192) return false;
                    _nRounds = 12;
                    break;
                case AesKeyType.Aes256:
                    if (key.Length < 4*(int)AesKeyType.Aes256) return false;
                    _nRounds = 14;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(keyType), keyType, null);
            }

            _key?.ZeroMemory();      // Zero memory with previous key

            _key = KeyExpantion(tmp);

            tmp.ZeroMemory();

            return true;
        }


        public byte[] ElectronicCodeBook(byte[] data)
        {
            if (data.Length % 16 != 0) return null;
            var toRet = new byte[data.Length];
            var tmp = new byte[16];

            for (var i = 0; i < data.Length; i += 16)
            {
                for (var j = 0; j < 16; j++)
                    tmp[j] = data[i + j];
                var res = Encrypt(tmp, _key);
                for (var j = 0; j < 16; j++)
                    toRet[i + j] = res[j];
            }

            return toRet;
        }
        #endregion //Class Interface


        #region Encrypt function

        // Encrypt
        private byte[] Encrypt(byte[] input, uint[] expandedKey)
        {
            if ((_aesKeyType == AesKeyType.Aes128 && expandedKey.Length != 44) ||
                (_aesKeyType == AesKeyType.Aes192 && expandedKey.Length != 52) ||
                (_aesKeyType == AesKeyType.Aes256 && expandedKey.Length != 60)) return null;
            if (input.Length != 16) return null;
            uint[] roundKey = { expandedKey[0], expandedKey[1], expandedKey[2], expandedKey[3] };

            var state = GetMatrixState(input);
            state = AddRoundKey(state, roundKey);
            for (var round = 1; round < _nRounds; round++)
            {
                state = SubBytes(state);
                state = ShiftRows(state);
                state = MixedColumns(state);

                for (var i = 0; i < 4; i++)
                    roundKey[i] = expandedKey[round * 4 + i];

                state = AddRoundKey(state, roundKey);
            }
            state = SubBytes(state);
            state = ShiftRows(state);

            for (var i = 0; i < 4; i++)
                roundKey[i] = expandedKey[_nRounds * 4 + i];

            state = AddRoundKey(state, roundKey);

            var toRet = new byte[input.Length];
            for (var i = 0; i < 4; i++)
                for (var j = 0; j < 4; j++)
                    toRet[4 * i + j] = state[i, j];

            return toRet;
        }

        private byte[,] GetMatrixState(byte[] data)
        {
            var toRet = new byte[4, 4];
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    toRet[i, j] = data[4 * i + j];
                }
            }
            return toRet;
        }
        #endregion


        #region Add Key and state

        private byte[,] AddRoundKey(byte[,] state, uint[] key)
        {
            var toRet = new byte[4, 4];
            for (var i = 0; i < 4; i++)
            {
                toRet[i, 0] = (byte)(state[i, 0] ^ (byte)((key[i] >> 24) & 0xFF));
                toRet[i, 1] = (byte)(state[i, 1] ^ (byte)((key[i] >> 16) & 0xFF));
                toRet[i, 2] = (byte)(state[i, 2] ^ (byte)((key[i] >> 8) & 0xFF));
                toRet[i, 3] = (byte)(state[i, 3] ^ (byte)(key[i] & 0xFF));
            }
            return toRet;
        }
        #endregion


        #region Constants
        // SBOX AES
        private readonly byte[] _sBox =
        {
            0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5, 0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76,
            0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0, 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0,
            0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15,
            0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a, 0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75,
            0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84,
            0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b, 0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf,
            0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8,
            0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2,
            0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73,
            0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88, 0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb,
            0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c, 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79,
            0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08,
            0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6, 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a,
            0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e,
            0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94, 0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf,
            0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68, 0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16
        };

        private readonly uint[] _rCon =
        {
            0x00000000,
            0x01000000,
            0x02000000,
            0x04000000,
            0x08000000,
            0x10000000,
            0x20000000,
            0x40000000,
            0x80000000,
            0x1b000000,
            0x36000000
        };

        private readonly byte[,] _matrixForMixedColumns =
        {
            {2, 3, 1, 1},
            {1, 2, 3, 1},
            {1, 1, 2, 3},
            {3, 1, 1, 2}
        };

        private readonly byte[,] _matrixForInverseMixedColumns =
        {
            {0x0e, 0x0b, 0x0d, 0x09},
            {0x09, 0x0e, 0x0b, 0x0d},
            {0x0d, 0x09, 0x0e, 0x0b},
            {0x0b, 0x0d, 0x09, 0x0e}
        };
        #endregion


        #region Substitutions of state matrix

        private byte[,] SubBytes(byte[,] state)
        {
            var toRet = new byte[4, 4];
            for (var i = 0; i < 4; i++)
                for (var j = 0; j < 4; j++)
                    toRet[i, j] = _sBox[state[i, j]];
            return toRet;
        }

        #endregion 


        #region Functions of cycle row rotation in state matrix
        // Functions of cycle row rotation (left) in state matrix
        private byte[,] ShiftRows(byte[,] state)
        {
            var toRet = new byte[4, 4];
            // first row doesn't rotate
            toRet[0, 0] = state[0, 0];
            toRet[0, 1] = state[0, 1];
            toRet[0, 2] = state[0, 2];
            toRet[0, 3] = state[0, 3];
            // second row rotate on one position
            toRet[1, 0] = state[1, 1];
            toRet[1, 1] = state[1, 2];
            toRet[1, 2] = state[1, 3];
            toRet[1, 3] = state[1, 0];
            // third row rotate on two positions
            toRet[2, 0] = state[2, 2];
            toRet[2, 1] = state[2, 3];
            toRet[2, 2] = state[2, 0];
            toRet[2, 3] = state[2, 1];
            // fourth row rotate on three positions
            toRet[3, 3] = state[3, 2];
            toRet[3, 2] = state[3, 1];
            toRet[3, 1] = state[3, 0];
            toRet[3, 0] = state[3, 3];
            return toRet;
        }

        #endregion 


        #region Columns permutation in state matrix
        private byte[,] MixedColumns(byte[,] state)
        {
            var toRet = new byte[4, 4];
            for (var row = 0; row < 4; row++)
            {
                for (var col = 0; col < 4; col++)
                {
                    for (var i = 0; i < 4; i++)
                        toRet[row, col] ^= PolinomialMultilay(_matrixForMixedColumns[row, i], state[i, col]);
                }
            }
            return toRet;
        }

        #endregion //Columns permutation in state matrix


        #region Galoe Field
        // Multiplication functions in GF(2^8)
        private byte PolinomialMultilay(int matrixKoff, byte stateByte)
        {
            var st1 = stateByte;
            var st2 = PolinomialMultiplyByTwo(st1);
            var st4 = PolinomialMultiplyByTwo(st2);
            var st8 = PolinomialMultiplyByTwo(st4);
            switch (matrixKoff)
            {
                case 1:     // multiplication on 1 (1) in GF(2^8)
                    return stateByte;
                case 2:     // multiplication on 2 (х) in GF(2^8)
                    return PolinomialMultiplyByTwo(stateByte);
                case 3:     // multiplication on 3 (х+1) in GF(2^8)
                    return (byte)(st1 ^ st2);
                case 9:     // multiplication on 9 (x^3+1)
                    return (byte)(st1 ^ st8);
                case 11:    // 0x0b x^3+x+1
                    return (byte)(st1 ^ st2 ^ st8);
                case 13:    // 0x0d x^3+x^2+1
                    return (byte)(st1 ^ st4 ^ st8);
                case 14:    // 0x0e x^3+x^2+x
                    return (byte)(st2 ^ st4 ^ st8);
                default:
                    return stateByte;
            }
        }

        // Multiply on 2 in Galoe field
        private byte PolinomialMultiplyByTwo(byte number)
        {
            uint tmp = number;
            tmp <<= 1;
            if (tmp > 0xFF)
                tmp ^= 0x1B;
            return (byte)(tmp & 0xFF);
        }
        #endregion //Galoe field


        #region Key Extension Functions

        private uint SubWord(uint word)
        {
            uint toRet = 0;
            for (var i = 0; i < 4; i++)
            {
                var tmp = (word >> (8 * i)) & 0xFF;
                var val = _sBox[tmp];
                toRet |= val;
                toRet <<= 8;
            }
            return toRet;
        }

        private uint RotateWord(uint word)
        {
            var tmp = word >> 24;
            var toRet = word << 8;
            toRet |= tmp;
            return toRet;
        }

        private uint[] KeyExpantion(IList<uint> key)
        {
            var nk = (int)_aesKeyType;
            const int nb = 4;
            var expandedKey = new uint[nb * (_nRounds + 1)];    // развернутый ключ
            for (var i = 0; i < nk; i++)
                expandedKey[i] = key[i];

            for (var i = nk; i < nb * (_nRounds + 1); i++)
            {
                var tmp = expandedKey[i - 1];
                if (i % nk == 0)
                {
                    tmp = SubWord(RotateWord(tmp)) ^ _rCon[i / nk];
                }
                else if (nk > 6 && i % nk == 4)
                {
                    tmp = SubWord(tmp);
                }
                expandedKey[i] = expandedKey[i - nk] ^ tmp;
            }
            return expandedKey;
        }

        #endregion

        public void Dispose()
        {
            _key?.ZeroMemory();
        }
    }

    #endregion

}
