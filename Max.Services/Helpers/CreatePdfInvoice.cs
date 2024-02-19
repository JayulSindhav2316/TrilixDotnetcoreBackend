using System;
using System.Collections.Generic;
using System.Text;
using Max.Core;
using Max.Core.Models;
using Max.Core.Helpers;
using Max.Core.Helpers.Pdf;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Max.Data.DataModel;
using System.IO;
using Document = iTextSharp.text.Document;
using Cell = Max.Core.Helpers.Pdf.Cell;
using System.Runtime.InteropServices.ComTypes;
using System.Reflection.Metadata;
using System.Globalization;
using MySqlConnector;
using System.Data;
using System.Collections;
using System.Linq;
using Item = Max.Core.Helpers.Pdf.Item;

namespace Max.Services.Helpers
{
    public class PdfInvoice : MaxPdfFactory
    {
        private int invoiceId { get; set; }
        public string fileName { get; set; }
        private InvoiceModel invoice { get; set; }
        private BillingAddressModel billingAddress { get; set; }
        private float pageHeaderHeight { get; set; }
        private float addressTableHeight { get; set; }

        private string documentRootPath { get; set; }

        MaxPdfDocument invoicePdf;
        Document PdfDocument;
        //EntityService entityService;
        private MaxPdfPageEvent pageEvent { get; set; }
        int fontSize = 10;
        public PdfInvoice(string documentPath, InvoiceModel invoice)
        {
            this.invoice = invoice;
            //this.GetBillableAddress().ConfigureAwait(false);
            

            documentRootPath = documentPath;

            invoicePdf = new MaxPdfDocument($"Invoice-{invoice.InvoiceId}");

            fileName = $"Invoice-{invoice.InvoiceId}.pdf";
            invoicePdf.LayOut = PageLayoutTypes.Portrait;

            invoicePdf.LeftMargin = 20;
            invoicePdf.TopMargin = 20;
            invoicePdf.BottomMargin = 20;
            invoicePdf.RightMargin = 20;
            invoicePdf.FontSize = fontSize;

            PdfDocument = CreatePdfDocument(fileName, invoicePdf, fontSize);
        }

        public byte[] GetPdf()
        {
            using (MemoryStream pdfMemoryStream = new MemoryStream())
            {
                string logoImagePath = documentRootPath + this.invoice.Organization.HeaderImage;
                this.pageEvent = new MaxPdfPageEvent(logoImagePath);

                PdfWriter pdfWriter = PdfWriter.GetInstance(PdfDocument, pdfMemoryStream);

                pdfWriter.AddViewerPreference(PdfName.Picktraybypdfsize, new PdfBoolean(false));
                pdfWriter.AddViewerPreference(PdfName.Printarea, PdfName.None);

                pageEvent.PrintPageHeader = false;
                pdfWriter.PageEvent = pageEvent;

                //Get Invoice Details

                PdfDocument.Open();

                PdfPTable PageHeaderTable = GetPageHeaderTable(logoImagePath);
                PdfDocument.Add(PageHeaderTable);

                PdfDocument.Add(PrintBlankRows(1));

                PdfPTable organizationHeaderTable = GetOrganizationAddressHeader();
                PdfDocument.Add(organizationHeaderTable);

                PdfDocument.Add(PrintBlankRows(1));

                PdfPTable billToTable = GetBillToTable();
                PdfDocument.Add(billToTable);

                PdfDocument.Add(PrintBlankRows(1));

                //PageHeaderTable.WriteSelectedRows(0, -1, 10, PdfDocument.PageSize.Top - 5, pdfWriter.DirectContent);
                //PdfPTable AddressTable = GetAddressTable();
                //AddressTable.WriteSelectedRows(0, -1, 50, PdfDocument.PageSize.Top - 100, pdfWriter.DirectContent);

                PdfDocument.Add(PrintBlankRows(1));

                PdfDocument.Add(PrintMembership());

                PdfDocument.Close();
                return pdfMemoryStream.ToArray();
            }
        }

        private PdfPTable GetPageHeaderTable(string imagePath)
        {

            float pageWidth = PdfDocument.PageSize.Width;

            float[] headerwidths = { pageWidth * 50.0F / 100.0F,
                                     pageWidth * 50.0F / 100.0F
                                   };

            PdfPTable dtPageHeader = new PdfPTable(2);

            dtPageHeader.SetTotalWidth(headerwidths);
            dtPageHeader.WidthPercentage = 90;

            //Organization Logo
            Cell cell = new Cell();
            Item item = new Item();

            cell = new Cell();
            item = new Item();
            item.ItemType = "iTextSharp.text.Image";
            item.Value = imagePath;
            item.Style.FontColor = "GRAY";
            item.Width = 188;
            item.Height = 66;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));


            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "INVOICE";
            item.Style.Font = "VERDANA";
            item.Style.FontSize = 25;
            item.Style.FontStyle = "BOLD";
            item.Style.FontColor = "GRAY";
            item.Style.HorzontalAlignment = 2;
            item.Style.VerticalAlignment = 3;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            return dtPageHeader;
        }

        private PdfPTable GetOrganizationAddressHeader()
        {

            float pageWidth = PdfDocument.PageSize.Width;

            float[] headerwidths = { pageWidth * 100.0F / 100.0F };

            PdfPTable dtPageHeader = new PdfPTable(1);

            dtPageHeader.SetTotalWidth(headerwidths);
            dtPageHeader.WidthPercentage = 90;

            //Organization Logo
            Cell cell = new Cell();
            Item item = new Item();

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = this.invoice.Organization.Title;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.ColumnSpan = 2;
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = this.invoice.Organization.Address1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.ColumnSpan = 2;
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = this.invoice.Organization.City + ", " + this.invoice.Organization.State + ", " + this.invoice.Organization.Zip;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.ColumnSpan = 2;
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = this.invoice.Organization.Website;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.ColumnSpan = 2;
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            return dtPageHeader;
        }

        private PdfPTable GetBillToTable()
        {
            float pageWidth = PdfDocument.PageSize.Width;

            float[] headerwidths = {    pageWidth * 70.0F / 100.0F,
                                        pageWidth * 15.0F / 100.0F,
                                        pageWidth * 15.0F / 100.0F
                                    };

            PdfPTable dt = new PdfPTable(3);

            dt.SetTotalWidth(headerwidths);
            dt.WidthPercentage = 90;

            Item item = new Item();
            Cell cell = new Cell();

            //Start Row
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Style.Font = "VERDANA";
            item.Style.FontSize = 10;
            item.Style.FontColor = "BLACK";
            item.Style.FontStyle = "BOLD";
            item.Style.HorzontalAlignment = 0;
            item.Value = "Bill To";
            item.ColumnSpan = 3;
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));
            

            //Start Row
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Style.Font = "VERDANA";
            item.Style.FontSize = 10;
            item.Style.FontColor = "BLACK";
            item.Style.FontStyle = "NORMAL";
            item.Style.HorzontalAlignment = 0;
            item.Value = this.invoice.BillingAddress.BillToName;
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Invoice Id : ";
            item.Style.HorzontalAlignment = 2;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = this.invoice.InvoiceId.ToString();
            item.Style.HorzontalAlignment = 0;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));

            //Start Row
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Style.Font = "VERDANA";
            item.Style.FontSize = 10;
            item.Style.FontColor = "BLACK";
            item.Style.FontStyle = "NORMAL";
            item.Style.HorzontalAlignment = 0;
            item.Value = this.invoice.BillingAddress.StreetAddress;
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));

            DateTime invoiceDate = invoice.Date;
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Invoice Date: ";
            item.Style.HorzontalAlignment = 2;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));

            
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = invoiceDate.ToString("d");
            item.Style.HorzontalAlignment = 0;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));

            //Start Row
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Style.Font = "VERDANA";
            item.Style.FontSize = 10;
            item.Style.FontColor = "BLACK";
            item.Style.FontStyle = "NORMAL";
            item.Style.HorzontalAlignment = 0;
            item.Value = this.invoice.BillingAddress.City + ", " + this.invoice.BillingAddress.State + ", " + this.invoice.BillingAddress.Zip + ", ";
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Created By: ";
            item.Style.Font = "VERDANA";
            item.Style.FontSize = 10;
            item.Style.FontColor = "BLACK";
            item.Style.FontStyle = "NORMAL";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));

            
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = this.invoice.UserName;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = 10;
            item.Style.FontColor = "BLACK";
            item.Style.FontStyle = "NORMAL";
            item.Style.HorzontalAlignment = 0;
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));

            //Start Row
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Style.Font = "VERDANA";
            item.Style.FontSize = 10;
            item.Style.FontColor = "BLACK";
            item.Style.FontStyle = "NORMAL";
            item.Style.HorzontalAlignment = 0;
            item.Value = "  ";
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));

            DateTime dueDate = invoice.DueDate;
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Due Date: ";
            item.Style.Font = "VERDANA";
            item.Style.FontSize = 10;
            item.Style.FontColor = "BLACK";
            item.Style.FontStyle = "NORMAL";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));

            //Start Row
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = dueDate.ToString("d");
            item.Style.Font = "VERDANA";
            item.Style.FontSize = 10;
            item.Style.FontColor = "BLACK";
            item.Style.FontStyle = "NORMAL";
            item.Style.HorzontalAlignment = 0;
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));            

            return dt;

        }
        private PdfPTable PrintBlankRows(int numberOfRows)
        {
            float pageWidth = PdfDocument.PageSize.Width;

            PdfPTable dt = new PdfPTable(1);

            float[] tblWidth = { pageWidth };


            dt.SetTotalWidth(tblWidth);
            dt.WidthPercentage = 90;

           Item item = new Item();
            Cell cell = new Cell();

            //Print Billing Address

            for (int i = 0; i < numberOfRows; i++)
            {

                cell = new Cell();
                item = new Item();
                item.ItemType = "Label";
                item.Style.Font = "VERDANA";
                item.Style.FontSize = 12;
                item.Style.FontColor = "BLACK";
                item.Style.FontStyle = "BOLD";
                item.Style.HorzontalAlignment = 0;
                item.Value = "     ";


                cell.AddItem(item);
                dt.AddCell(RenderCell(cell));
            }
            return dt;

        }

        private PdfPTable PrintMembership()
        {
           
            PdfPTable pdfPTable = new PdfPTable(1);

            float pageWidth = PdfDocument.PageSize.Width;
            float[] headerwidths = { pageWidth };


            pdfPTable.SetTotalWidth(headerwidths);
            pdfPTable.WidthPercentage = 90;

            //string sectionName = "New Membership";
            //PdfPTable dtHeader = PrintSectionHeader(sectionName);
            //PdfPCell sectionHeader = new PdfPCell(dtHeader);

            //sectionHeader.BorderWidthBottom = 0;
            //sectionHeader.BorderWidthTop = 0;
            //sectionHeader.BorderWidthLeft = 0;
            //sectionHeader.BorderWidthRight = 0;
            //sectionHeader.Colspan = 1;
            //sectionHeader.FixedHeight = 20f;
            //pdfPTable.AddCell(sectionHeader);            

            //PdfPTable itemHeader = PrintItemDetailHeader();
            //pdfPTable.AddCell(itemHeader);

            //PdfPTable dtNewMember = PrintNewMemberDetails();
            //PdfPCell newMemberHeader = new PdfPCell(dtNewMember);

            //newMemberHeader.BorderWidthBottom = 0;
            //newMemberHeader.BorderWidthTop = 0;
            //newMemberHeader.BorderWidthLeft = 0;
            //newMemberHeader.BorderWidthRight = 0;
            //newMemberHeader.Colspan = 1;
            //newMemberHeader.FixedHeight = 25f;
            //newMemberHeader.VerticalAlignment = 5;

            //pdfPTable.AddCell(newMemberHeader);

            PdfPTable itemHeader = PrintItemDetailHeader();
            pdfPTable.AddCell(itemHeader);

            if(invoice.BillingType == "Membership")
            {
                PdfPTable membershipSummary = PrintMembershipSummary();
                pdfPTable.AddCell(membershipSummary);
            }
            
            foreach ( var lineItem in invoice.InvoiceDetails)
            {
                PdfPTable dt = PrintLineItem(lineItem);
                pdfPTable.AddCell(dt);
            }

            PdfPTable totals = PrintTotals();
            pdfPTable.AddCell(totals);

            return pdfPTable;
        }
        private PdfPTable PrintSectionHeader(string sectionName)
        {
            float pageWidth = PdfDocument.PageSize.Width;

            float[] headerwidths = { pageWidth * 10.0F / 100.0F,
                                     pageWidth * 60.0F / 100.0F,
                                     pageWidth * 10.0F / 100.0F,
                                     pageWidth * 10.0F / 100.0F,
                                     pageWidth * 10.0F / 100.0F,
                                   };

            PdfPTable pdfPTable = new PdfPTable(5);

            pdfPTable.SetTotalWidth(headerwidths);
            pdfPTable.WidthPercentage = 90;


            Item item = new Item();
            Cell cell = new Cell();

            item = new Item();
            item.ItemType = "Label";
            item.Value = sectionName;
            item.ColumnSpan = 5;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.HorzontalAlignment = 0;
            item.Style.VerticalAlignment = 5;
            item.Style.FontColor = "BLACK";
            item.Style.Background = "LIGHT_GRAY";

            cell = new Cell();
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            return (pdfPTable);
        }

        private PdfPTable PrintItemDetailHeader()
        {
            float pageWidth = PdfDocument.PageSize.Width;

            float[] headerwidths = { pageWidth * 10.0F / 100.0F,
                                     pageWidth * 60.0F / 100.0F,
                                     pageWidth * 15.0F / 100.0F,
                                     pageWidth * 15.0F / 100.0F
                                   };

            PdfPTable pdfPTable = new PdfPTable(4);

            pdfPTable.SetTotalWidth(headerwidths);
            pdfPTable.WidthPercentage = 90;


            Item item = new Item();
            Cell cell = new Cell();

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Qty.";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.HorzontalAlignment = 1;
            item.Style.FontColor = "BLACK";
            item.Style.Background = "LIGHT_GRAY";
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Description";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.HorzontalAlignment = 0;
            item.Style.FontColor = "BLACK";
            item.Style.Background = "LIGHT_GRAY";
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            item = new Item();
            cell = new Cell();
            item.ItemType = "Label";
            item.Value = "Rate";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.HorzontalAlignment = 1;
            item.Style.FontColor = "BLACK";
            item.Style.Background = "LIGHT_GRAY";
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            item = new Item();
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Amount";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.HorzontalAlignment = 1;
            item.Style.FontColor = "BLACK";
            item.Style.Background = "LIGHT_GRAY";
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            return (pdfPTable);

        }

        private PdfPTable PrintLineItem(InvoiceDetailModel lineItem)
        {

            float pageWidth = PdfDocument.PageSize.Width;

            float[] headerwidths = { pageWidth * 10.0F / 100.0F,
                                     pageWidth * 60.0F / 100.0F,
                                     pageWidth * 15.0F / 100.0F,
                                     pageWidth * 15.0F / 100.0F
                                   };

            PdfPTable pdfPTable = new PdfPTable(4);

            pdfPTable.SetTotalWidth(headerwidths);
            pdfPTable.WidthPercentage = 90;


            Item item = new Item();
            Cell cell = new Cell();


            item = new Item();
            cell = new Cell();
            item.ItemType = "Label";
            item.Value = lineItem.Quantity.ToString();
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 1;            
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = lineItem.Description;
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 0;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = lineItem.Price.ToString("c2");
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 1;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            item = new Item();
            cell = new Cell();
            item.ItemType = "Label";
            item.Value = lineItem.Amount.ToString("c2");
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 1;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            return (pdfPTable);
        }

        private PdfPTable PrintNewMemberDetails()
        {
            float pageWidth = PdfDocument.PageSize.Width;

            float[] headerwidths = { pageWidth * 10.0F / 100.0F,
                                     pageWidth * 30.0F / 100.0F,
                                     pageWidth * 20.0F / 100.0F,
                                     pageWidth * 20.0F / 100.0F,
                                     pageWidth * 20.0F / 100.0F,
                                   };

            PdfPTable pdfPTable = new PdfPTable(5);

            pdfPTable.SetTotalWidth(headerwidths);
            pdfPTable.WidthPercentage = 90;


            Item item = new Item();
            Cell cell = new Cell();

            item = new Item();
            item.ItemType = "Label";
            item.Value = "";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
           
            item.Value = $"{invoice.BillingAddress.BillToName}";
            
            item.ColumnSpan =1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 0;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            DateTime startDate = invoice.Membership.StartDate;
            DateTime endDate = invoice.Membership.EndDate;

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = $"{invoice.Membership.MembershipType.Name}";
            item.ColumnSpan =1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = $"Start Date:{startDate.ToString("d")}";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";

            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = $"End Date:{endDate.ToString("d")}";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";

            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            return (pdfPTable);
        }

        private PdfPTable PrintTotals()
        {
            float pageWidth = PdfDocument.PageSize.Width;

            float[] headerwidths = { pageWidth * 70.0F / 100.0F,
                                     pageWidth * 20.0F / 100.0F,
                                     pageWidth * 10.0F / 100.0F,
                                   };

            PdfPTable pdfPTable = new PdfPTable(3);

            pdfPTable.SetTotalWidth(headerwidths);
            pdfPTable.WidthPercentage = 90;


            Item item = new Item();
            Cell cell = new Cell();

            item = new Item();
            item.ItemType = "Label";
            item.Value = "";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";

            item.Style.FontColor = "BLACK";

           
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Sub Total";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            decimal subTotal = invoice.InvoiceDetails.Sum(x => x.Amount);
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = $"{subTotal.ToString("c2")}";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.FontColor = "BLACK";
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Tax Rate";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;

            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "0%";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;

            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Tax Amount";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;

            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            decimal taxAmount = 0;
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = taxAmount.ToString("c2");
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;

            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Total Due";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;

            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = subTotal.ToString("c2");
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;

            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            return (pdfPTable);
        }

        private PdfPTable PrintMembershipSummary()
        {
            float pageWidth = PdfDocument.PageSize.Width;

            float[] headerwidths = { pageWidth * 10.0F / 100.0F,
                                     pageWidth * 90.0F / 100.0F
                                   };

            PdfPTable pdfPTable = new PdfPTable(2);

            pdfPTable.SetTotalWidth(headerwidths);
            pdfPTable.WidthPercentage = 90;


            Item item = new Item();
            Cell cell = new Cell();

            item = new Item();
            item.ItemType = "Label";
            item.Value = "";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = invoice.Membership.MembershipType.CategoryNavigation.Name + ", " + this.invoice.Membership.MembershipType.Name;
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 0;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            //DateTime startDate = this.invoice.Membership.StartDate;
            //DateTime endDate = this.invoice.Membership.EndDate;

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 0;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = $"Member: {this.invoice.Entity.Name}";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 0;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 0;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = $"Next Bill Date: {this.invoice.Membership.NextBillDate.ToString("d")}";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 0;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 0;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            //cell = new Cell();
            //item = new Item();
            //item.ItemType = "Label";
            //item.Value = $"Period: {startDate.ToString("d")} through: {endDate.ToString("d")}";
            //item.ColumnSpan = 1;
            //item.Style.Font = "VERDANA";
            //item.Style.FontSize = fontSize;
            //item.Style.FontStyle = "NORMAL";
            //item.Style.FontColor = "BLACK";
            //item.Style.HorzontalAlignment = 0;
            //cell.AddItem(item);
            //pdfPTable.AddCell(RenderCell(cell));

            return (pdfPTable);
        }
    }
}
