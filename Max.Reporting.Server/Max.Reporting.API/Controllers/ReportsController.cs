using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using System.Net;
using Telerik.Reporting.Services;
using Telerik.Reporting.Services.AspNetCore;
using System;

namespace Max.Reporting.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ReportsControllerBase
    {
        
        private readonly IConfiguration _configuration;
        public ReportsController(IReportServiceConfiguration reportServiceConfiguration, IConfiguration configuration)
            : base(reportServiceConfiguration)
        {
             this._configuration= configuration;
        }

        protected override HttpStatusCode SendMailMessage(System.Net.Mail.MailMessage mailMessage)
        {
            using var smtp = new SmtpClient();
            smtp.Connect(this._configuration["AppSettings:EmailServer"], Convert.ToInt32(this._configuration["AppSettings:PortNumber"]), SecureSocketOptions.SslOnConnect);
            smtp.Authenticate(this._configuration["AppSettings:UserName"], this._configuration["AppSettings:Password"]);
            smtp.Send((MimeKit.MimeMessage)mailMessage);
            smtp.Disconnect(true);            
            return HttpStatusCode.OK;
        }
    }
}
