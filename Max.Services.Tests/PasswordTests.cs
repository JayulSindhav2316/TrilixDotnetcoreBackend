using Xunit;
using Max.Core.Models;
using Max.Core.Helpers;
using Max.Core;

namespace Max.Services.Tests
{
    public class PasswordTests
    {
        [Fact]
        public void Password_Validate_ValidHash()
        {
            string password = "TestPassword@1234.,";

            PasswordHash hash = new PasswordHash(password);

            Assert.True(hash.IsValidPassword(hash.Salt,hash.Password, password), "User Password matches.");

        }

        [Fact]
        public void Password_Validate_InValidPassword()
        {
            string password = "TestPassword@1234.,";
            string invalidPassword = "TestPassword@1234";

            PasswordHash hash = new PasswordHash(password);

            Assert.False(hash.IsValidPassword(hash.Salt, hash.Password, invalidPassword), "User Password does not matches.");

        }

        [Fact]
        public void Password_Validate_InValidSalt()
        {
            string password = "TestPassword@1234.,";

            PasswordHash hash = new PasswordHash(password);

            //Get another salt value

            string salt = SecurePassword.GetRandomSaltHexString(Constants.CDefaultSaltLength);
            hash.Salt = salt;

            Assert.False(hash.IsValidPassword(hash.Salt, hash.Password, password), "User Password does not  matches.");

        }
    }
}
