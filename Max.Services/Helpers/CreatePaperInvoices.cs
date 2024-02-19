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
    public class CreatePaperInvoice : MaxPdfFactory
    {
        private int cycleId { get; set; }
        public string fileName { get; set; }
        private List<InvoiceModel> paperInvoices { get; set; }
        private BillingAddressModel billingAddress { get; set; }
        private float pageHeaderHeight { get; set; }
        private float addressTableHeight { get; set; }

        private string documentRootPath { get; set; }
        private string organizationHeader { get; set; }
        private Organization organization { get; set; }
        MaxPdfDocument paperInvoicePdf;

        Document PdfDocument;
        //EntityService entityService;
        private MaxPdfPageEvent pageEvent { get; set; }
        int fontSize = 10;
        public CreatePaperInvoice(string documentPath, List<InvoiceModel> paperInvoices, Organization organization)
        {
            this.paperInvoices = paperInvoices;
            this.organization = organization;
            this.organizationHeader = organization.HeaderImage;

            documentRootPath = documentPath;

            paperInvoicePdf = new MaxPdfDocument($"PaperInvoice-Cycle-{cycleId}");

            fileName = $"PaperInvoice-Cycle-{cycleId}.pdf";
            paperInvoicePdf.LayOut = PageLayoutTypes.Portrait;

            paperInvoicePdf.LeftMargin = 18;
            paperInvoicePdf.TopMargin = 18;
            paperInvoicePdf.BottomMargin = 18;
            paperInvoicePdf.RightMargin = 18;
            paperInvoicePdf.FontSize = fontSize;

            PdfDocument = CreatePdfDocument(fileName, paperInvoicePdf, fontSize);
        }

        public byte[] GetPdf()
        {
            using (MemoryStream pdfMemoryStream = new MemoryStream())
            {
                string logoImagePath = documentRootPath + organizationHeader;
                if (File.Exists(logoImagePath))
                {
                    this.pageEvent = new MaxPdfPageEvent(logoImagePath);
                }
                PdfWriter pdfWriter = PdfWriter.GetInstance(PdfDocument, pdfMemoryStream);

                pdfWriter.AddViewerPreference(PdfName.Picktraybypdfsize, new PdfBoolean(false));
                pdfWriter.AddViewerPreference(PdfName.Printarea, PdfName.None);

                if (File.Exists(logoImagePath))
                {
                    pageEvent.PrintPageHeader = false;
                    pdfWriter.PageEvent = pageEvent;
                }
                //Get Invoice Details

                PdfDocument.Open();
                foreach (var invoice in paperInvoices)
                {
                    PdfDocument.Add(PrintBlankRows(2));
                    PdfPTable PageHeaderTable = GetPageHeaderTable(logoImagePath);
                    PdfDocument.Add(PageHeaderTable);

                    PdfDocument.Add(PrintBlankRows(1));

                    PdfPTable organizationHeaderTable = GetInvoiceHeader(invoice);
                    PdfDocument.Add(organizationHeaderTable);

                    PdfDocument.Add(PrintBlankRows(1));

                    PdfPTable billToTable = GetBillToTable(invoice);
                    PdfDocument.Add(billToTable);

                    PdfDocument.Add(PrintBlankRows(2));

                    PdfPTable detachmentTable = GetDetachmentTable();
                    PdfDocument.Add(detachmentTable);
                    PdfPTable memberHeaderTable = GetMemberInvoiceHeader(logoImagePath, invoice);
                    PdfDocument.Add(memberHeaderTable);

                    PdfDocument.Add(PrintBlankRows(1));
                    PdfDocument.Add(PrintBlankRows(1));

                    PdfDocument.Add(PrintInvoice(invoice));
                    PdfDocument.NewPage();
                }



                PdfDocument.Close();
                return pdfMemoryStream.ToArray();
            }
        }


        public byte[] GetEventPdf()
        {
            using (MemoryStream pdfMemoryStream = new MemoryStream())
            {
                string logoImagePath = documentRootPath + organizationHeader;
                this.pageEvent = new MaxPdfPageEvent(logoImagePath);

                PdfWriter pdfWriter = PdfWriter.GetInstance(PdfDocument, pdfMemoryStream);

                pdfWriter.AddViewerPreference(PdfName.Picktraybypdfsize, new PdfBoolean(false));
                pdfWriter.AddViewerPreference(PdfName.Printarea, PdfName.None);

                pageEvent.PrintPageHeader = false;
                pdfWriter.PageEvent = pageEvent;

                //Get Invoice Details

                PdfDocument.Open();
                foreach (var invoice in paperInvoices)
                {
                    PdfDocument.Add(PrintBlankRows(2));
                    PdfPTable PageHeaderTable = GetPageHeaderTable(logoImagePath);
                    PdfDocument.Add(PageHeaderTable);

                    PdfDocument.Add(PrintBlankRows(1));

                    PdfPTable organizationHeaderTable = GetInvoiceHeader(invoice);
                    PdfDocument.Add(organizationHeaderTable);

                    PdfDocument.Add(PrintBlankRows(1));

                    PdfPTable billToTable = GetBillToTable(invoice);
                    PdfDocument.Add(billToTable);

                    PdfDocument.Add(PrintBlankRows(2));

                    PdfPTable detachmentTable = GetDetachmentTable();
                    PdfDocument.Add(detachmentTable);
                    PdfPTable memberHeaderTable = GetMemberInvoiceHeader(logoImagePath, invoice);
                    PdfDocument.Add(memberHeaderTable);

                    PdfDocument.Add(PrintBlankRows(1));
                    PdfDocument.Add(PrintBlankRows(1));

                    PdfDocument.Add(PrintEventInvoice(invoice));
                    PdfDocument.NewPage();
                }



                PdfDocument.Close();
                return pdfMemoryStream.ToArray();
            }
        }


        private PdfPTable GetPageHeaderTable(string imagePath)
        {

            float pageWidth = PdfDocument.PageSize.Width;

            float[] headerwidths = { pageWidth * 70.0F / 100.0F,
                                     pageWidth * 30.0F / 100.0F
                                   };

            PdfPTable dtPageHeader = new PdfPTable(2);

            dtPageHeader.SetTotalWidth(headerwidths);
            dtPageHeader.WidthPercentage = 90;

            //Organization Logo
            Cell cell = new Cell();
            Item item = new Item();

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "INVOICE";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 22;
            item.Style.FontStyle = "BOLD";
            item.Style.FontColor = "GRAY";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            item.Style.VerticalAlignment = PdfCell.ALIGN_MIDDLE;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "iTextSharp.text.Image";
            item.Value = imagePath;
            item.Style.FontColor = "GRAY";
            item.Width = 140;
            item.Height = 50;
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            item.Style.VerticalAlignment = PdfCell.ALIGN_MIDDLE;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            return dtPageHeader;
        }

        private PdfPTable GetInvoiceHeader(InvoiceModel invoice)
        {

            float pageWidth = PdfDocument.PageSize.Width;

            float[] headerwidths = { pageWidth * 60.0F / 100.0F,
                                     pageWidth * 20.0F / 100.0F,
                                     pageWidth * 20.0F / 100.0F,
                                   };

            PdfPTable dtPageHeader = new PdfPTable(3);

            dtPageHeader.SetTotalWidth(headerwidths);
            dtPageHeader.WidthPercentage = 90;

            Cell cell = new Cell();
            Item item = new Item();
            //Organization Name

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = this.organization.Title;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 12;
            item.Style.FontStyle = "Bold";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            //Member Id

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Member #:";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            //Member Id

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = invoice.BillableEntityId.ToString();
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));


            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = $"{this.organization.Address1} {this.organization.Address2} {this.organization.Address3}";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Invoice #:";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = invoice.InvoiceId.ToString();
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));


            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = this.organization.City + ", " + this.organization.State + " " + this.organization.Zip;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Billing Date:";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = invoice.Date.ToString("MM/dd/yyyy");
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));


            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = this.organization.Website != null ? this.organization.Website.Replace("https://", "").Replace("/", "") : "";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Due Date:";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = invoice.DueDate.ToString("MM/dd/yyyy");
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Amount Due:";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Bold";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = invoice.InvoiceDetails.Sum(x => x.Amount).ToString("c2");
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Bold";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Enclosed Amount:";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "$__________";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));
            return dtPageHeader;
        }

        private PdfPTable GetMemberInvoiceHeader(string imagePath, InvoiceModel invoice)
        {

            float pageWidth = PdfDocument.PageSize.Width;

            float[] headerwidths = { pageWidth * 60.0F / 100.0F,
                                     pageWidth * 20.0F / 100.0F,
                                     pageWidth * 20.0F / 100.0F
                                   };

            PdfPTable dtPageHeader = new PdfPTable(3);

            dtPageHeader.SetTotalWidth(headerwidths);
            dtPageHeader.WidthPercentage = 90;

            Cell cell = new Cell();
            Item item = new Item();
            //Organization Name

            cell = new Cell();
            item = new Item();
            item.ItemType = "iTextSharp.text.Image";
            item.Value = imagePath;
            item.Style.FontColor = "GRAY";
            item.Width = 140;
            item.Height = 50;
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            item.Style.VerticalAlignment = PdfCell.ALIGN_MIDDLE;
            item.RowSpan = 5;
            cell.AddItem(item);

            var rowCell = RenderCell(cell);
            rowCell.Rowspan = 5;
            dtPageHeader.AddCell(rowCell);

            //Member Id

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Member #:";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            //Member Id

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = invoice.BillableEntityId.ToString();
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Invoice #:";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = invoice.InvoiceId.ToString();
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Billing Date:";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = invoice.Date.ToString("MM/dd/yyyy");
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));


            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Due Date:";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = invoice.DueDate.ToString("MM/dd/yyyy");
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Normal";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));


            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Created By:";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Bold";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = invoice.UserName;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "Bold";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            cell.AddItem(item);
            dtPageHeader.AddCell(RenderCell(cell));

            return dtPageHeader;
        }
        private PdfPTable GetDetachmentTable()
        {
            float pageWidth = PdfDocument.PageSize.Width;

            float[] headerwidths = { pageWidth * 100.0F / 100.0F };

            PdfPTable dt = new PdfPTable(1);

            dt.SetTotalWidth(headerwidths);
            dt.WidthPercentage = 90;

            Item item = new Item();
            Cell cell = new Cell();

            //Start Row
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 12;
            item.Style.FontColor = "BLACK";
            item.Style.FontStyle = "BOLD";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            item.Value = "----------------------------------------------------------------------------------------------------------------------------";
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));
            //Start Row
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 8;
            item.Style.FontColor = "BLACK";
            item.Style.FontStyle = "Normal";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_MIDDLE;
            item.Value = "Please detach and return above portion with your payment";
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));
            return dt;
        }

        private PdfPTable GetBillToTable(InvoiceModel invoice)
        {
            float pageWidth = PdfDocument.PageSize.Width;

            float[] headerwidths = {    pageWidth * 60.0F / 100.0F,
                                        pageWidth * 20.0F / 100.0F,
                                        pageWidth * 20.0F / 100.0F
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
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 12;
            item.Style.FontColor = "BLACK";
            item.Style.FontStyle = "BOLD";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            item.Value = "Bill To";
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));

            //Start Row
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 12;
            item.Style.FontColor = "BLACK";
            item.Style.FontStyle = "BOLD";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            item.Value = "Remit Payment To:";
            item.ColumnSpan = 2;
            cell.AddItem(item);

            dt.AddCell(RenderCell(cell));

            //Start Row
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 12;
            item.Style.FontColor = "BLACK";
            item.Style.FontStyle = "BOLD";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            item.Value = "";
            item.ColumnSpan = 3;
            cell.AddItem(item);

            dt.AddCell(RenderCell(cell));

            //Start Row
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontColor = "BLACK";
            item.Style.FontStyle = "NORMAL";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            item.Value = invoice.BillingAddress.BillToName;
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = this.organization.Title;
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.ColumnSpan = 2;
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));

            //Start Row
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontColor = "BLACK";
            item.Style.FontStyle = "NORMAL";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            item.Value = invoice.BillingAddress.StreetAddress;
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = $"{organization.Address1} {organization.Address2} {organization.Address3}";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.ColumnSpan = 2;
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));


            //Start Row
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontColor = "BLACK";
            item.Style.FontStyle = "NORMAL";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            item.Value = invoice.BillingAddress.City + ", " + invoice.BillingAddress.State + " " + invoice.BillingAddress.Zip;
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = $"{organization.City}, {organization.State}, {organization.Zip}";
            item.Style.HorzontalAlignment = PdfCell.ALIGN_LEFT;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = 10;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.ColumnSpan = 2;
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
                item.Style.Font = "ARIAL";
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

        private PdfPTable PrintInvoice(InvoiceModel invoice)
        {

            PdfPTable pdfPTable = new PdfPTable(1);

            float pageWidth = PdfDocument.PageSize.Width;
            float[] headerwidths = { pageWidth };


            pdfPTable.SetTotalWidth(headerwidths);
            pdfPTable.WidthPercentage = 90;

            PdfPTable itemHeader = PrintItemDetailHeader();
            pdfPTable.AddCell(itemHeader);

            bool membershipHeaderPrinted = false;
            foreach (var lineItem in invoice.InvoiceDetails)
            {
                if (lineItem.ItemType == (int)InvoiceItemType.Membership && !membershipHeaderPrinted)
                {
                    PdfPTable dt = PrintMembershipLineItem(invoice, lineItem);
                    PdfPCell wrapper = new PdfPCell(dt);
                    wrapper.BorderWidthBottom = 0;
                    wrapper.BorderWidthTop = 0;
                    pdfPTable.AddCell(wrapper);
                    membershipHeaderPrinted = true;


                    dt = PrintLineItem(lineItem);
                    pdfPTable.AddCell(dt);
                }
                else
                {
                    PdfPTable dt = PrintLineItem(lineItem);
                    pdfPTable.AddCell(dt);
                }
            }

            var height = pdfPTable.TotalHeight;

            while (height < 230)
            {
                PdfPTable dt = PrintBlankLineItem();
                PdfPCell wrapper = new PdfPCell(dt);
                wrapper.BorderWidthBottom = 0;
                wrapper.BorderWidthTop = 0;
                pdfPTable.AddCell(wrapper);
                height = pdfPTable.TotalHeight;
            }

            //PdfPTable notes = PrintNotes(invoice);
            //pdfPTable.AddCell(notes);

            PdfPTable totals = PrintTotals(invoice);
            pdfPTable.AddCell(totals);
            height = pdfPTable.TotalHeight;
            return pdfPTable;
        }

        private PdfPTable PrintEventInvoice(InvoiceModel invoice)
        {

            PdfPTable pdfPTable = new PdfPTable(1);

            float pageWidth = PdfDocument.PageSize.Width;
            float[] headerwidths = { pageWidth };


            pdfPTable.SetTotalWidth(headerwidths);
            pdfPTable.WidthPercentage = 90;

            PdfPTable itemHeader = PrintEventItemDetailHeader();
            pdfPTable.AddCell(itemHeader);

            PdfPTable dtDesc = PrintEventDescription(invoice);
            pdfPTable.AddCell(dtDesc);
            int counter = 1;
            foreach (var lineItem in invoice.InvoiceDetails)
            {
                PdfPTable dt = PrintEventLineItem(lineItem, counter);
                pdfPTable.AddCell(dt);
                counter++;
            }

            var height = pdfPTable.TotalHeight;

            while (height < 250)
            {
                PdfPTable dt = PrintBlankLineItem();
                PdfPCell wrapper = new PdfPCell(dt);
                wrapper.BorderWidthBottom = 0;
                wrapper.BorderWidthTop = 0;
                pdfPTable.AddCell(wrapper);
                height = pdfPTable.TotalHeight;
            }

            PdfPTable totals = PrintTotals(invoice);
            pdfPTable.AddCell(totals);
            height = pdfPTable.TotalHeight;
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
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.HorzontalAlignment = 1;
            item.Style.FontColor = "BLACK";
            item.Style.Background = "LIGHT_GRAY";
            cell.AddItem(item);
            PdfPCell wrapperCell = RenderCell(cell);
            wrapperCell.BorderWidthBottom = 0;
            wrapperCell.BorderWidthLeft = 0;
            wrapperCell.BorderWidthTop = 0;
            wrapperCell.BorderWidthRight = 0;
            pdfPTable.AddCell(wrapperCell);

            return (pdfPTable);

        }

        private PdfPTable PrintEventItemDetailHeader()
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
            item.Value = "Sr.no";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.HorzontalAlignment = 1;
            item.Style.FontColor = "BLACK";
            item.Style.Background = "LIGHT_GRAY";
            cell.AddItem(item);
            PdfPCell wrapperCell = RenderCell(cell);
            wrapperCell.BorderWidthBottom = 0;
            wrapperCell.BorderWidthLeft = 0;
            wrapperCell.BorderWidthTop = 0;
            wrapperCell.BorderWidthRight = 0;
            pdfPTable.AddCell(wrapperCell);

            return (pdfPTable);

        }

        private PdfPTable PrintBlankLineItem()
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
            item.Value = " ";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 1;
            item.Style.BorderBottom = 0;
            item.Style.BorderTop = 0;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = " ";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 0;
            item.Style.BorderBottom = 0;
            item.Style.BorderTop = 0;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = " ";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            item.Style.BorderBottom = 0;
            item.Style.BorderTop = 0;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            item = new Item();
            cell = new Cell();
            item.ItemType = "Label";
            item.Value = " ";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            item.Style.BorderBottom = 0;
            item.Style.BorderTop = 0;
            cell.AddItem(item);

            PdfPCell wrapperCell = RenderCell(cell);
            wrapperCell.BorderWidthBottom = 0;
            wrapperCell.BorderWidthLeft = 0;
            wrapperCell.BorderWidthTop = 0;
            wrapperCell.BorderWidthRight = 0;
            pdfPTable.AddCell(wrapperCell);

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
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            item = new Item();
            cell = new Cell();
            item.ItemType = "Label";
            item.Value = lineItem.Amount.ToString("c2");
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            return (pdfPTable);
        }

        private PdfPTable PrintEventDescription(InvoiceModel invoice)
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
            item.Value = "";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 1;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = invoice.Event.Name + " - " + (invoice.Event.EventTypeId == 1 ? "In-Person" : invoice.Event.EventTypeId == 2 ? "Virtual" : "Pre Recorded");
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            item = new Item();
            cell = new Cell();
            item.ItemType = "Label";
            item.Value = "";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            return (pdfPTable);
        }

        private PdfPTable PrintEventLineItem(InvoiceDetailModel lineItem, int counter)
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
            item.Value = counter.ToString();
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            item = new Item();
            cell = new Cell();
            item.ItemType = "Label";
            item.Value = lineItem.Amount.ToString("c2");
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            return (pdfPTable);
        }

        private PdfPTable PrintTotals(InvoiceModel invoice)
        {
            float pageWidth = PdfDocument.PageSize.Width;

            //float[] headerwidths = { pageWidth * 15.0F / 100.0F,
            //                         pageWidth * 20.0F / 100.0F,
            //                         pageWidth * 15.0F / 100.0F,
            //                         pageWidth * 10.0F / 100.0F,
            //                         pageWidth * 10.0F / 100.0F,
            //                         pageWidth * 10.0F / 100.0F,
            //                         pageWidth * 10.0F / 100.0F,
            //                         pageWidth * 10.0F / 100.0F
            //                       };

            //PdfPTable pdfPTable = new PdfPTable(8);

            PdfPTable leftTable = new PdfPTable(1);
            PdfPTable rightTable = new PdfPTable(2);

            leftTable.DefaultCell.Border = PdfPCell.NO_BORDER;
            rightTable.DefaultCell.Border = PdfPCell.NO_BORDER;

            PdfPCell pdfCell = new PdfPCell();

            float[] leftHeaderwidths = { pageWidth * 90.0F / 100.0F
                    
                                         //, pageWidth * 60.0F / 100.0F
                                      };
            leftTable.SetTotalWidth(leftHeaderwidths);
            leftTable.WidthPercentage = 90;


            float[] rightHeaderwidths = { pageWidth * 30.0F / 100.0F,
                                          pageWidth * 30.0F / 100.0F };
            rightTable.SetTotalWidth(rightHeaderwidths);
            rightTable.WidthPercentage = 90;


            //pdfPTable.SetTotalWidth(headerwidths);
            //pdfPTable.WidthPercentage = 90;


            Item item = new Item();
            Cell cell = new Cell();

            item = new Item();
            item.ItemType = "Label";
            item.Value = "Important Announcement: ";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";

            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 0;
            cell.AddItem(item);
            //pdfPTable.AddCell(RenderCell(cell));

            leftTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();

            item = new Item();
            item.ItemType = "Label";
            item.Value = invoice.Notes;
            item.ColumnSpan = 3;
            item.RowSpan = 3;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";

            item.Style.FontColor = "BLACK";

            item.Style.HorzontalAlignment = 0;

            cell.AddItem(item);
            //pdfPTable.AddCell(RenderCell(cell));

            leftTable.AddCell(RenderCell(cell));


            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Sub Total";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            //pdfPTable.AddCell(RenderCell(cell));

            rightTable.AddCell(RenderCell(cell));

            decimal subTotal = invoice.InvoiceDetails.Sum(x => x.Amount);
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = $"{subTotal.ToString("c2")}";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            //pdfPTable.AddCell(RenderCell(cell));

            rightTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.FontColor = "BLACK";
            cell.AddItem(item);
            //pdfPTable.AddCell(RenderCell(cell));

            rightTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.FontColor = "BLACK";
            cell.AddItem(item);
            //pdfPTable.AddCell(RenderCell(cell));

            rightTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Tax (0%)";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;

            cell.AddItem(item);
            //pdfPTable.AddCell(RenderCell(cell));

            rightTable.AddCell(RenderCell(cell));

            decimal taxAmount = 0;
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = taxAmount.ToString("c2");
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;

            cell.AddItem(item);
            //pdfPTable.AddCell(RenderCell(cell));

            rightTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            //pdfPTable.AddCell(RenderCell(cell));

            rightTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            //pdfPTable.AddCell(RenderCell(cell));

            rightTable.AddCell(RenderCell(cell));


            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Total Due";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;

            cell.AddItem(item);
            //pdfPTable.AddCell(RenderCell(cell));

            rightTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = subTotal.ToString("c2");
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;

            cell.AddItem(item);
            //pdfPTable.AddCell(RenderCell(cell)); ;

            rightTable.AddCell(RenderCell(cell));

            PdfPTable mainTable = new PdfPTable(2);
            mainTable.SetTotalWidth(new float[] {70, 30 });
            mainTable.WidthPercentage = 100;
            mainTable.DefaultCell.Border = PdfPCell.NO_BORDER;
            mainTable.AddCell(leftTable);
            mainTable.AddCell(rightTable);

            //return (pdfPTable);
            return (mainTable);
        }

        private PdfPTable PrintMembershipLineItem(InvoiceModel invoice, InvoiceDetailModel invoiceDetail)
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
            item.Value = "";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 1;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            if (invoice.Membership != null)
            {
                item.Value = $"{invoice.Membership?.MembershipType?.CategoryNavigation?.Name}, {invoice.Membership?.MembershipType?.Name}";
            }
            else
            {
                item.Value = "";
            }
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 0;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            //DateTime startDate = invoice.Membership.StartDate;
            //DateTime endDate = invoice.Membership.EndDate;
            DateTime? nextBillDate = invoice.Membership?.NextBillDate;

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 0;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = $"Member: {invoice.Entity.Name}";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            item = new Item();
            cell = new Cell();
            item.ItemType = "Label";
            item.Value = "";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 0;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));


            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = $"Next Bill Date: {nextBillDate?.ToString("MM/dd/yyyy")}";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 0;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));


            //cell = new Cell();
            //item = new Item();
            //item.ItemType = "Label";
            //item.Value = $"Period: {startDate.ToString("MM/dd/yyyy")} through: {endDate.ToString("MM/dd/yyyy")}";
            //item.ColumnSpan = 1;
            //item.Style.Font = "ARIAL";
            //item.Style.FontSize = fontSize;
            //item.Style.FontStyle = "NORMAL";
            //item.Style.FontColor = "BLACK";
            //item.Style.HorzontalAlignment = 0;
            //cell.AddItem(item);
            //pdfPTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "";
            item.ColumnSpan = 1;
            item.Style.Font = "ARIAL";
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
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 0;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));


            return (pdfPTable);
        }
    }
}
