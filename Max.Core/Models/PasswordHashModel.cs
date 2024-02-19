using System;
using System.Collections.Generic;
using System.Text;
using Max.Core;
using Max.Core.Helpers;

namespace Max.Core.Models
{
    public class PasswordHash
    {
        public string Salt { get; set; }
        public string Password { get; set; }

        public PasswordHash()
        {
            Salt = string.Empty;
            Password = string.Empty;
        }

        public PasswordHash(string password)
        {
            Byte[] _salt;
            Byte[] _hash;

            _salt = SecurePassword.GetRandomSalt(Constants.CDefaultSaltLength);
            _hash = SecurePassword.PasswordToHash(_salt, password, Constants.iterations);
            this.Password = SecurePassword.HashBytesToHexString(_hash);
            this.Salt = SecurePassword.HashBytesToHexString(_salt);
        }

        public bool IsValidPassword(string salt,  string  passwordHash, string password)
        {
            Byte[] _salt;
            Byte[] _hash;
            _salt = SecurePassword.HashHexStringToBytes(salt);
            _hash = SecurePassword.HashHexStringToBytes(passwordHash);
            return SecurePassword.ComparePasswordToHash(_salt, password, _hash, Constants.iterations);

        }

    }
}
