using Microsoft.AspNetCore.DataProtection;

namespace Max.Core.DataProtection
{
    public class DataProtectionExtention : IDataProtectorExtention
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private const string key = "57d95f48-c262-4e22-ae97-01d9d445f3f7";
        private IDataProtector protector = null;
        public DataProtectionExtention(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
            this.protector = _dataProtectionProvider.CreateProtector(key);
        }

        public string Decrypt(string data)
        {
            try
            {
                return protector.Unprotect(data);
            }
            catch
            {
                return data;
            }
        }

        public string Encrypt(string data)
        {
            return this.protector.Protect(data);
        }
    }
}
public interface IDataProtectorExtention
{
    public string Encrypt(string data);
    public string Decrypt(string data);
}