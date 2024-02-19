using MailKit.Net.Smtp;
using MailKit.Security;
using Max.Core;
using Max.Core.Helpers;
using Max.Core.Models;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services
{
    public class EmailService : IEmailService
    {
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly AppSettings _appSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDocumentService _documentService;
        private readonly ITenantService _tenantService;
        private readonly ILogger<EmailService> _logger;
        public EmailService(IOptions<AppSettings> appSettings,
                            IUnitOfWork unitOfWork,
                            IHostEnvironment hostEnvironment,
                            IDocumentService documentService,
                            ITenantService tenantService,
                            ILogger<EmailService> logger
                            )
        {
            _appSettings = appSettings.Value;
            this._unitOfWork = unitOfWork;
            this._documentService = documentService;
            this._hostingEnvironment = hostEnvironment;
            this._tenantService = tenantService;
            this._logger = logger;
        }

        public void Send()
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("support@membermax.com"));//TODO AKS - Should come from Config
            email.To.Add(MailboxAddress.Parse("ashoks@membermax.com"));//TODO AKS - Should come from Config
            email.Subject = "Your membership Invoice";
            email.Body = new TextPart(TextFormat.Html) { Text = "Test Message" };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(_appSettings.EmailServer, _appSettings.PortNumber, SecureSocketOptions.SslOnConnect);
            smtp.Authenticate(_appSettings.UserName, _appSettings.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        public async Task<bool> SendHtmlInvoice(BatchEmailNotificationModel model)
        {
            string emailBody = BuildManualBillingNotificationEmailBody(model);
            var builder = new BodyBuilder { HtmlBody = emailBody };

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("support@membermax.com"));//TODO AKS - Should come from Config
            email.To.Add(MailboxAddress.Parse(model.EmailAddress));
            email.Subject = model.Subject != null ? model.Subject : "Your Invoice";
            email.Body = builder.ToMessageBody();

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(_appSettings.EmailServer, _appSettings.PortNumber, SecureSocketOptions.SslOnConnect);
            smtp.Authenticate(_appSettings.UserName, _appSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);

            return (true);

        }
        public async Task<bool> SendHtmlReceipt(EmailMessageModel model)
        {

            var emailConfig = await _tenantService.GetTenantByOrganizationName(model.OrganizationName);

            var builder = new BodyBuilder { HtmlBody = model.MessageBody };
            builder.Attachments.Add("Receipt-" + model.ReceiptId.ToString() + ".pdf", model.PdfData);
            // create message
            if (emailConfig.SenderEmailAddress != null)
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(emailConfig.SenderEmailAddress));
                email.Subject = model.Subject;
                email.To.Add(MailboxAddress.Parse(model.ReceipientAddress));
                email.Body = builder.ToMessageBody();
                // send email
                using var smtp = new SmtpClient();
                if (emailConfig.EmailServerNeedsAuthentication == (int)Status.Active)
                {
                    smtp.Connect(emailConfig.EmailServer, int.Parse(emailConfig.EmailServerPort), SecureSocketOptions.SslOnConnect);
                    smtp.Authenticate(emailConfig.EmailSenderUserName, emailConfig.EmailSenderPassword);
                }
                else
                {
                    smtp.Connect(emailConfig.EmailServer, int.Parse(emailConfig.EmailServerPort), SecureSocketOptions.None);
                }
                await smtp.SendAsync(email);
                smtp.Disconnect(true);

                return (true);
            }
            else
            {
                throw new Exception("Could not send the email.Please review email configuration.");
            }
        }
        public async Task<bool> SendAutoBillingNotification(AutoBillingEmailNotificationModel model)
        {
            var emailConfig = await _tenantService.GetTenantByOrganizationId(model.OrganizationId);
            string emailBody = BuildAutoBillingNotificationEmailBody(model);
            var builder = new BodyBuilder { HtmlBody = emailBody };
            // create message
            if (emailConfig.SenderEmailAddress != null)
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(emailConfig.SenderEmailAddress));
                email.To.Add(MailboxAddress.Parse(model.EmailAddresses[0]));
                if (model.EmailAddresses.Length > 1)
                {
                    for (int i = 1; i < model.EmailAddresses.Length; i++)
                    {
                        email.Cc.Add(MailboxAddress.Parse(model.EmailAddresses[i]));
                    }
                }
                else
                {
                    email.To.Add(MailboxAddress.Parse(model.EmailAddresses[0]));
                }
                email.Subject = model.Subject;
                email.Body = builder.ToMessageBody();
                // send email
                using var smtp = new SmtpClient();
                if (emailConfig.EmailServerNeedsAuthentication == (int)Status.Active)
                {
                    smtp.Connect(emailConfig.EmailServer, int.Parse(emailConfig.EmailServerPort), SecureSocketOptions.SslOnConnect);
                    smtp.Authenticate(emailConfig.EmailSenderUserName, emailConfig.EmailSenderPassword);
                }
                else
                {
                    smtp.Connect(emailConfig.EmailServer, int.Parse(emailConfig.EmailServerPort), SecureSocketOptions.None);
                }
                await smtp.SendAsync(email);
                smtp.Disconnect(true);

                return (true);
            }
            else
            {
                throw new Exception("Could not send the email.Please review email configuration.");
            }
        }

        public async Task<bool> SendMultiFactorNotification(string organizationName, string emailAddress, string code)
        {
            _logger.LogInformation("Entering Email Service -> SendMultiFactorNotification -> Email Address -> " + emailAddress);
            var emailConfig = await _tenantService.GetTenantByOrganizationName(organizationName);
            string emailBody = BuildVerificationEmailBody(code, organizationName);
            var builder = new BodyBuilder { HtmlBody = emailBody };
            if (emailConfig.SenderEmailAddress != null)
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(emailConfig.SenderEmailAddress));
                email.To.Add(MailboxAddress.Parse(emailAddress));
                email.Subject = "Your verification code";
                email.Body = builder.ToMessageBody();

                // send email
                using var smtp = new SmtpClient();
                if (emailConfig.EmailServerNeedsAuthentication == (int)Status.Active)
                {
                    smtp.Connect(emailConfig.EmailServer, int.Parse(emailConfig.EmailServerPort), SecureSocketOptions.SslOnConnect);
                    smtp.Authenticate(emailConfig.EmailSenderUserName, emailConfig.EmailSenderPassword);
                }
                else
                {
                    smtp.Connect(emailConfig.EmailServer, int.Parse(emailConfig.EmailServerPort), SecureSocketOptions.None);
                }
                await smtp.SendAsync(email);
                smtp.Disconnect(true);

                _logger.LogInformation("Exiting Email Service -> SendMultiFactorNotification -> Email Address -> " + emailAddress );
                return (true);
            }
            else
            {
                throw new Exception("Could not send the email.Please review email configuration.");                
            }
        }

        public async Task<bool> SendBatchInvoiceNotification(BatchEmailNotificationModel model)
        {
            var emailConfig = await _tenantService.GetTenantByOrganizationName(model.Organization.Name);
            string emailBody = BuildManualBillingNotificationEmailBody(model);
            var builder = new BodyBuilder { HtmlBody = emailBody };

            if (emailConfig.SenderEmailAddress != null)
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(emailConfig.SenderEmailAddress));
                email.To.Add(MailboxAddress.Parse(model.EmailAddress));
                email.Subject = model.Subject != null ? model.Subject : "Your Invoice"; ;
                email.Body = builder.ToMessageBody();

                // send email
                using var smtp = new SmtpClient();
                if (emailConfig.EmailServerNeedsAuthentication == (int)Status.Active)
                {
                    smtp.Connect(emailConfig.EmailServer, int.Parse(emailConfig.EmailServerPort), SecureSocketOptions.SslOnConnect);
                    smtp.Authenticate(emailConfig.EmailSenderUserName, emailConfig.EmailSenderPassword);
                }
                else
                {
                    smtp.Connect(emailConfig.EmailServer, int.Parse(emailConfig.EmailServerPort), SecureSocketOptions.None);
                }
                await smtp.SendAsync(email);
                smtp.Disconnect(true);

                return (true);
            }
            else
            {
                throw new Exception("Could not send the email.Please review email configuration.");
            }
        }

        public async Task<bool> SendPasswordResetNotification(ResetPasswordModel model)
        {
            var emailConfig = await _tenantService.GetTenantByOrganizationName(model.OrganizationName);
            string emailBody = BuildResetPasswordNotificationEmailBody(model);
            var builder = new BodyBuilder { HtmlBody = emailBody };

            if (emailConfig.SenderEmailAddress != null)
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(emailConfig.SenderEmailAddress));
                email.To.Add(MailboxAddress.Parse(model.Email));
                email.Subject = "Your Password Reset Request";
                email.Body = builder.ToMessageBody();
                // send email
                using var smtp = new SmtpClient();
                if (emailConfig.EmailServerNeedsAuthentication == (int)Status.Active)
                {
                    smtp.Connect(emailConfig.EmailServer, int.Parse(emailConfig.EmailServerPort), SecureSocketOptions.SslOnConnect);
                    smtp.Authenticate(emailConfig.EmailSenderUserName, emailConfig.EmailSenderPassword);
                }
                else
                {
                    smtp.Connect(emailConfig.EmailServer, int.Parse(emailConfig.EmailServerPort), SecureSocketOptions.None);
                }
                await smtp.SendAsync(email);
                smtp.Disconnect(true);

                return (true);
            }
            else
            {
                throw new Exception("Could not send the email.Please review email configuration.");
            }
        }

        public async Task<bool> SendAccountVerificationEmailBody(MemberAccountEmailModel model)
        {
            var emailConfig = await _tenantService.GetTenantByOrganizationName(model.OrganizationName);
            string emailBody = BuildMemberAccountVerificationEmailBody(model.Token,model.SiteUrl);
            var builder = new BodyBuilder { HtmlBody = emailBody };

            if (emailConfig.SenderEmailAddress != null)
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(emailConfig.SenderEmailAddress));
                email.To.Add(MailboxAddress.Parse(model.EmailAddress));
                email.Subject = "Your verifcation code for new account";
                email.Body = builder.ToMessageBody();
                // send email
                using var smtp = new SmtpClient();
                if (emailConfig.EmailServerNeedsAuthentication == (int)Status.Active)
                {
                    smtp.Connect(emailConfig.EmailServer, int.Parse(emailConfig.EmailServerPort), SecureSocketOptions.SslOnConnect);
                    smtp.Authenticate(emailConfig.EmailSenderUserName, emailConfig.EmailSenderPassword);
                }
                else
                {
                    smtp.Connect(emailConfig.EmailServer, int.Parse(emailConfig.EmailServerPort), SecureSocketOptions.None);
                }
                await smtp.SendAsync(email);
                smtp.Disconnect(true);

                return (true);
            }
            else
            {
                throw new Exception("Could not send the email.Please review email configuration.");
            }
        }
        private string BuildResetPasswordNotificationEmailBody(ResetPasswordModel model)
        {
            var messageBody = "";

            try
            {
                var strTemplateFilePath = Path.Combine(_hostingEnvironment.ContentRootPath + "/Documents/EmailTemplates/ResetPasswordNotification.html");
                var reader = new StreamReader(strTemplateFilePath);
                messageBody = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to read Email Template.");
            }
            messageBody = messageBody.Replace("[[[name]]]", model.Name);
            messageBody = messageBody.Replace("[[[url]]]", model.ResetPasswordUrl);
            return messageBody;
        }
        private string BuildAutoBillingNotificationEmailBody(AutoBillingEmailNotificationModel model)
        {

            var messageBody = "";

            try
            {
                var strTemplateFilePath = Path.Combine(_hostingEnvironment.ContentRootPath + "/Documents/EmailTemplates/RecurringBillingStatusNotification.html");//TODO AKS - Should it be based on organization choice. 
                var reader = new StreamReader(strTemplateFilePath);
                messageBody = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to read Email Template.");
            }
            messageBody = messageBody.Replace("[[[Title]]]", model.Title);
            messageBody = messageBody.Replace("[[[billingtype]]]", model.BillingType);
            messageBody = messageBody.Replace("[[[processdate]]]", model.ProcessDate);
            messageBody = messageBody.Replace("[[[throughdate]]]", model.ThroughDate);
            messageBody = messageBody.Replace("[[[totalDue]]]", model.TotalDue.ToString("C2"));
            messageBody = messageBody.Replace("[[[approved]]]", model.Approved.ToString("C2"));
            messageBody = messageBody.Replace("[[[declined]]]", model.Declined.ToString("C2"));
            return messageBody;
        }

        private string BuildVerificationEmailBody(string code, string organizationName = "")
        {
            var messageBody = "";
            var strTemplateFilePath = "";

            try
            {
                strTemplateFilePath = Path.Combine(_hostingEnvironment.ContentRootPath + "/Documents/EmailTemplates/TwoFactorVerificationNotification_" + organizationName  + ".html");                
                var reader = new StreamReader(strTemplateFilePath);
                messageBody = reader.ReadToEnd();
                reader.Close();
            }
            catch (FileNotFoundException ex)
            {
                var strUniversaleTemplateFilePath = Path.Combine(_hostingEnvironment.ContentRootPath + "/Documents/EmailTemplates/TwoFactorVerificationNotification.html");
                var reader = new StreamReader(strUniversaleTemplateFilePath);
                messageBody = reader.ReadToEnd();
                reader.Close();
                _logger.LogError("EmaiService.cs => BuildVerificationEmailBody() => File not found in path " + strTemplateFilePath);

            }
            catch (Exception ex)
            {
                throw new Exception("Unable to read Email Template.");
            }
            messageBody = messageBody.Replace("[[[code]]]", code);
            return messageBody;
        }
        private string BuildMemberAccountVerificationEmailBody(string code, string siteUrl = "")
        {
            var messageBody = "";
            var strTemplateFilePath = "";

            try
            {
                strTemplateFilePath = Path.Combine(_hostingEnvironment.ContentRootPath + "/Documents/EmailTemplates/NewMemberAccountEmail.html");
                var reader = new StreamReader(strTemplateFilePath);
                messageBody = reader.ReadToEnd();
                reader.Close();
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError("EmaiService.cs => BuildVerificationEmailBody() => File not found in path " + strTemplateFilePath);

            }
            catch (Exception ex)
            {
                throw new Exception("Unable to read Email Template.");
            }
            messageBody = messageBody.Replace("[[[code]]]", code);
            messageBody = messageBody.Replace("[[[site]]]", siteUrl);
            return messageBody;
        }
        public string GetInvoiceNotificationEmailBody(BatchEmailNotificationModel model)
        {
            return BuildManualBillingNotificationEmailBody(model);
        }
        private string BuildManualBillingNotificationEmailBody(BatchEmailNotificationModel model)
        {
            var messageBody = "";

            try
            {
                var strTemplateFilePath = Path.Combine(_hostingEnvironment.ContentRootPath + "/Documents/EmailTemplates/InvoiceEmailTemplate.html");//TODO AKS - Should it be based on organization choice. 
                var reader = new StreamReader(strTemplateFilePath);
                messageBody = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to read Email Template.");
            }
            var cityStateZip = $"{model.Organization.City},{model.Organization.State},{model.Organization.Zip.FormatZip()}";
            messageBody = messageBody.Replace("[[[INVOICE-ID]]]", model.InvoiceId.ToString());
            messageBody = messageBody.Replace("[[[ORGANIZATION]]]", model.Organization.Name);
            messageBody = messageBody.Replace("[[[ORGANIZATION-NAME]]]", model.Organization.Title);
            messageBody = messageBody.Replace("[[[DUEDATE]]]", model.Invoice.DueDate.ToShortDateString());
            messageBody = messageBody.Replace("[[[INVOICE-DETAIL]]]", GetInvoiceDetails(model.Invoice));
            messageBody = messageBody.Replace("[[[DUE-AMOUNT]]]", model.Invoice.IsBalanceAmount? model.Invoice.BalanceAmount.ToString("C2") : model.Invoice.Amount.ToString("C2"));
            messageBody = messageBody.Replace("[[[BILLABLE-PERSON-NAME]]]", model.Name);
            messageBody = messageBody.Replace("[[[ORGANIZATION-ADDRESS]]]", model.Organization.Address1);
            messageBody = messageBody.Replace("[[[ORGANIZATION-CITY-STATE-ZIP]]]", cityStateZip);
            messageBody = messageBody.Replace("[[[ORGANIZATION-URL]]]", model.Organization.Website);
            messageBody = messageBody.Replace("[[[PAYMENT-LINK]]]", $"{model.BaseUrl}/#/memberpayment?{model.PaymentUrl}");
            if (model.Invoice.Amount == 0)
            {
                messageBody = messageBody.Replace("[[[ReviewAndPayDisplay]]]", "none");
            }

            return messageBody;
        }
        private string GetInvoiceDetails(InvoiceModel invoice)
        {
            StringBuilder sbInvoiceDetail = new StringBuilder();
            sbInvoiceDetail.AppendLine("<table class='width100 line-item-container' style='font-family:ArialMT, Arial, Helvetica, Times New Roman; color:#393a3d; width:100%; padding:0px 0px 0px 0px'>");
            sbInvoiceDetail.AppendLine("<tbody>");
            sbInvoiceDetail.AppendLine("<tr>");
            sbInvoiceDetail.AppendLine("<td>");
            sbInvoiceDetail.AppendLine("<table class='title-and-amount width100' style='font-family:ArialMT, Arial, Helvetica, Times New Roman; color:#393a3d; width:100'>");
            sbInvoiceDetail.AppendLine("<tbody style=''>");
            sbInvoiceDetail.AppendLine("</table>");
            foreach (var item in invoice.InvoiceDetails)
            {
                sbInvoiceDetail.AppendLine("<table class='width100 itemDescWrapper' style='font-family:ArialMT, Arial, Helvetica, Times New Roman; color:#393a3d; width:100%'>");
                sbInvoiceDetail.AppendLine("<tbody>");
                sbInvoiceDetail.AppendLine($"<td style='text-align:left;'>{item.Description}</td>");
                sbInvoiceDetail.AppendLine($"<td style='text-align:right; padding: 0px 20px 0px 0px'>{String.Format("{0:C2}", item.Price)}</td>");
                sbInvoiceDetail.AppendLine("</tr>");
                sbInvoiceDetail.AppendLine("</tbody>");
                sbInvoiceDetail.AppendLine("</table>");
            }
            sbInvoiceDetail.AppendLine("<table class='width100 itemDescWrapper' style='font-family:ArialMT, Arial, Helvetica, Times New Roman; color:#393a3d; border-top:dotted 1px #babec5; width:100%'>");
            sbInvoiceDetail.AppendLine("<tbody>");
            sbInvoiceDetail.AppendLine("<td style='text-align:right; width:80%'><B>Balance Due</B></td>");
            sbInvoiceDetail.AppendLine($"<td style='text-align:right; padding: 0px 20px 0px 0px; width:20%'><B>{String.Format("{0:C2}", invoice.IsBalanceAmount ? invoice.BalanceAmount : invoice.Amount)}</B></td>");
            sbInvoiceDetail.AppendLine("</tr>");
            sbInvoiceDetail.AppendLine("</tbody>");
            sbInvoiceDetail.AppendLine("</table>");
            sbInvoiceDetail.AppendLine("</table></td></tr>");
            return sbInvoiceDetail.ToString();
        }
        
    }


}
