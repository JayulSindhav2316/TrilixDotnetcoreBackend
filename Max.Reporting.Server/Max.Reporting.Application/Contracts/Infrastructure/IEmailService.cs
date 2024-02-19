using Max.Reporting.Application.Models;

namespace Max.Reporting.Application.Contracts.Infrastructure
{
    public interface IEmailService
    {
        Task<bool> SendEmail(Email email);
    }
}
