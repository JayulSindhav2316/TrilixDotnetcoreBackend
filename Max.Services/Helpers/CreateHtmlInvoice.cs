using Max.Core.Models;
using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Services.Helpers
{
    public class HtmlInvoice
    {
        private InvoiceModel _invoice;

        public HtmlInvoice(InvoiceModel invoice)
        {
            _invoice = invoice;
        }

        public String Create()
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.AppendLine(GetHtmlHeader());
            sbHtml.AppendLine(GetTitle());
            sbHtml.AppendLine(GetStyle());
            sbHtml.AppendLine("<body>");
            sbHtml.AppendLine("<div class='invoice-box'><table>");
            sbHtml.AppendLine(GetOrganizationHeaderRow());
            sbHtml.AppendLine(GetInvoiceHeaderRow());
            sbHtml.AppendLine(GetMembershipInfoRow());
            sbHtml.AppendLine(GetInvoiceDetailHeader());
            sbHtml.AppendLine(GetInvoiceDetails());
            sbHtml.AppendLine(GetInvoiceTotal());
            sbHtml.AppendLine("</div></table></body>");
            sbHtml.AppendLine("</html>");
            return sbHtml.ToString();
        }

        public String GetHtmlHeader()
        {
            string header =  "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Strict//EN' 'https://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd'>";
            return header;
        }

        private string GetTitle()
        {
            return "<head><title>Your Membership Invoice.</title></head>";
        }

        private string GetStyle()
        {
            StringBuilder sbStyle = new StringBuilder();

            sbStyle.AppendLine("<style type='text/css' media='screen'>");
            sbStyle.AppendLine("body {padding: 0!important; margin: 0!important; display: block!important; min-width:100% !important; width: 100% !important; background:#ffffff; -webkit-text-size-adjust:none }");
            sbStyle.AppendLine(".invoice-box {");
            sbStyle.AppendLine("\tmax-width: 800px;");
            sbStyle.AppendLine("\tmargin: auto;");
            sbStyle.AppendLine("\tpadding: 30px;");
            sbStyle.AppendLine("\tborder: 1px solid #eee;");
            sbStyle.AppendLine("\tbox-shadow: 0 0 10px rgba(0, 0, 0, 0.15);");
            sbStyle.AppendLine("\tfont-size: 14px;");
            sbStyle.AppendLine("\tline-height: 24px;");
            sbStyle.AppendLine("\tfont-family: 'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;");
            sbStyle.AppendLine("\tcolor: #555;");
            sbStyle.AppendLine("\tbackground-color: #FFF;");
            sbStyle.AppendLine("\t}");
            sbStyle.AppendLine(".invoice-box table {");
            sbStyle.AppendLine("\twidth: 100%;");
            sbStyle.AppendLine("\tline-height: inherit;");
            sbStyle.AppendLine("\ttext-align: left;");
            sbStyle.AppendLine("\tborder-collapse: collapse;");
            sbStyle.AppendLine("\t}");
            sbStyle.AppendLine(".invoice-box table td {");
            sbStyle.AppendLine("\tpadding: 5px;");
            sbStyle.AppendLine("\tvertical-align: top;");
            sbStyle.AppendLine("\t}");
            sbStyle.AppendLine(".invoice-box table tr td:nth-child(2) {");
            sbStyle.AppendLine("\ttext-align: right;");
            sbStyle.AppendLine("\t}");
            sbStyle.AppendLine(".invoice-box table tr.top table td {");
            sbStyle.AppendLine("\tpadding-bottom: 20px;");
            sbStyle.AppendLine("\t}");
            sbStyle.AppendLine(".invoice-box table tr.top table td.title {");
            sbStyle.AppendLine("\tfont-size: 45px;");
            sbStyle.AppendLine("\tline-height: 45px;");
            sbStyle.AppendLine("\tcolor: #333;");
            sbStyle.AppendLine("\t}");
            sbStyle.AppendLine(".invoice-box table tr.information table td {");
            sbStyle.AppendLine("\tpadding-bottom: 40px;");
            sbStyle.AppendLine("\t}");
            sbStyle.AppendLine(".invoice-box table tr.heading td {");
            sbStyle.AppendLine("\tbackground: #eee;");
            sbStyle.AppendLine("\tborder-bottom: 1px solid #ddd;");
            sbStyle.AppendLine("\tfont-weight: bold;");
            sbStyle.AppendLine("\t}");
            sbStyle.AppendLine(".invoice-box table tr.details td {");
            sbStyle.AppendLine("\tpadding-bottom: 20px;");
            sbStyle.AppendLine("\t}");
            sbStyle.AppendLine(".invoice-box table tr.item td {");
            sbStyle.AppendLine("\tborder-bottom: 1px solid #eee;");
            sbStyle.AppendLine("\t}");
            sbStyle.AppendLine(".invoice-box table tr.item.last td {");
            sbStyle.AppendLine("\tborder-bottom: none;");
            sbStyle.AppendLine("\t}");
            sbStyle.AppendLine(".invoice-box table tr.total td:nth-child(3) {");
            sbStyle.AppendLine("\tborder-top: 2px solid #eee;font-weight: bold;");
            sbStyle.AppendLine("\t}");
            sbStyle.AppendLine("@media only screen and (max-width: 600px) {");
            sbStyle.AppendLine("\t.invoice-box table tr.top table td {");
            sbStyle.AppendLine("\twidth: 100%;");
            sbStyle.AppendLine("\tdisplay: block;");
            sbStyle.AppendLine("\ttext-align: center;");
            sbStyle.AppendLine("\t}");
            sbStyle.AppendLine(".invoice-box table tr.information table td {");
            sbStyle.AppendLine("\twidth: 100%;");
            sbStyle.AppendLine("\tdisplay: block;");
            sbStyle.AppendLine("\ttext-align: center;");
            sbStyle.AppendLine("\t}");
            sbStyle.AppendLine("\t}");
            sbStyle.AppendLine("</style>");
            return sbStyle.ToString();

        }

        private string GetOrganizationHeaderRow()
        {
            StringBuilder sbHeader = new StringBuilder();

            sbHeader.AppendLine("<tr class='top'>");
            sbHeader.AppendLine("<td colspan='2'>");
            sbHeader.AppendLine("<table><tr>");
            sbHeader.AppendLine("<td class='title'><img src='https://maxapi.membermax.com/app-images/header.jpg' style='width: 100%; max-width: 300px'/></td>");
            sbHeader.AppendLine("<td style='text-align:right;'>");
            sbHeader.AppendLine($"{_invoice.Organization.Address1}<BR/>");
            sbHeader.AppendLine($"{_invoice.Organization.Address2}<BR/>");
            if (_invoice.Organization.Address2.Length > 0)
            {
                sbHeader.AppendLine($"{_invoice.Organization.Address3}<BR/>");
            }
            sbHeader.AppendLine($"{_invoice.Organization.City}, {_invoice.Organization.State}, {_invoice.Organization.Zip}<BR/>");
            sbHeader.AppendLine("</td></tr></table>");
            sbHeader.AppendLine("</td></tr>");

            return sbHeader.ToString();
        }

        private string GetInvoiceHeaderRow()
        {
            StringBuilder sbInvoiceHeader = new StringBuilder();
            sbInvoiceHeader.AppendLine("<tr class='information'><td colspan = '2'>");
            sbInvoiceHeader.AppendLine("<table><tr>");
            sbInvoiceHeader.AppendLine("<td>");
            sbInvoiceHeader.AppendLine("<B>Bill to:</B><br/>");
            sbInvoiceHeader.AppendLine($"{_invoice.BillingAddress.BillToName}<br/>");
            sbInvoiceHeader.AppendLine($"{_invoice.BillingAddress.StreetAddress}</br>");
            sbInvoiceHeader.AppendLine($"{_invoice.BillingAddress.City}, {_invoice.BillingAddress.State}, {_invoice.BillingAddress.Zip}<BR/>");
            sbInvoiceHeader.AppendLine("</td>");
            sbInvoiceHeader.AppendLine("<td style='text-align:right;'>");
            sbInvoiceHeader.AppendLine($"Invoice #: {_invoice.InvoiceId}<br/>");
            sbInvoiceHeader.AppendLine($"Date: {_invoice.Date.ToString("MM/dd/yyyy")}<br/>");
            sbInvoiceHeader.AppendLine($"Due Date: {_invoice.DueDate.ToString("MM/dd/yyyy")}<br/>");
            sbInvoiceHeader.AppendLine("</td>");
            sbInvoiceHeader.AppendLine("</tr></table>");
            sbInvoiceHeader.AppendLine("</td></tr>");
            return sbInvoiceHeader.ToString();
        }

        private string GetMembershipInfoRow()
        {
            StringBuilder sbMembership = new StringBuilder();
            sbMembership.AppendLine("<tr class='heading'><td>Membership</td><td></td></tr>");
            sbMembership.AppendLine("<tr class='details'><td colspan = '2'><table><tr>");
            sbMembership.AppendLine($"<td>{_invoice.Membership.MembershipType.Code}:{_invoice.Membership.MembershipType.Name}</td>");
            sbMembership.AppendLine("</tr></table></td></tr>");
            return sbMembership.ToString();
        }

        private string GetInvoiceDetailHeader()
        {
            StringBuilder sbInvoiceHeader = new StringBuilder();
            sbInvoiceHeader.AppendLine("<tr class='heading'><td colspan = '2'>");
            sbInvoiceHeader.AppendLine("<table><tr><td style='text-align:center;'>Item #</td><td style='text-align:center;'>Item Description</td><td style='text-align:center;'>Price</td></tr></table>");
            sbInvoiceHeader.AppendLine("</td></tr>");
            return sbInvoiceHeader.ToString();
        }
        private string GetInvoiceDetails()
        {
            StringBuilder sbInvoiceDetail = new StringBuilder();
            sbInvoiceDetail.AppendLine("<tr class='item'>");
            sbInvoiceDetail.AppendLine("<td colspan='2'><table>");
            foreach (var item in _invoice.InvoiceDetails)
            {
                sbInvoiceDetail.AppendLine($"<tr><td style='text-align:center;border:1px;'>{item.InvoiceDetailId}</td>");
                sbInvoiceDetail.AppendLine($"<td style='text-align:left;'>{item.Description}</td>");
                sbInvoiceDetail.AppendLine($"<td style='text-align:right;'>{String.Format("{0:C2}", item.Price)}</td></tr>");
            }
            sbInvoiceDetail.AppendLine("</table></td></tr>");
            return sbInvoiceDetail.ToString();
        }
        private string GetInvoiceTotal()
        {
            StringBuilder sbInvoiceTotal = new StringBuilder();
            sbInvoiceTotal.AppendLine("<tr>");
            sbInvoiceTotal.AppendLine($"<td style='text-align:right' colspan='2'><B>Total: {String.Format("{0:C2}", _invoice.Amount)}</B></td>");
            sbInvoiceTotal.AppendLine("</tr>");
            return sbInvoiceTotal.ToString();
        }
    }
}
