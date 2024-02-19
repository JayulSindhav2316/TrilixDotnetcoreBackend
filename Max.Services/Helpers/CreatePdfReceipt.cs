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
using System.Threading.Tasks;


namespace Max.Services.Helpers
{
    public class CreatePdfReceipt : MaxPdfFactory
    {
        private int receiptId { get; set; }
        public string fileName { get; set; }
        private ReceiptModel receipt { get; set; }

        MaxPdfDocument receiptPdf;
        Document PdfDocument;
        private string documentRootPath { get; set; }
        private MaxPdfPageEvent pageEvent { get; set; }
        int fontSize = 10;
        private decimal receiptTotal = 0;

        public CreatePdfReceipt(string documentPath, ReceiptModel receipt)
        {
            this.receipt = receipt;
            documentRootPath = documentPath;


            receiptPdf = new MaxPdfDocument($"Receipt-{receipt.Receiptid}");

            fileName = $"Receipt-{receipt.Receiptid}.pdf";
            receiptPdf.LayOut = PageLayoutTypes.Portrait;

            receiptPdf.LeftMargin = 20;
            receiptPdf.TopMargin = 20;
            receiptPdf.BottomMargin = 20;
            receiptPdf.RightMargin = 20;
            receiptPdf.FontSize = fontSize;

            PdfDocument = CreatePdfDocument(fileName, receiptPdf, fontSize);
        }
        public byte[] GetPdf()
        {
            using (MemoryStream pdfMemoryStream = new MemoryStream())
            {
                string logoImagePath = documentRootPath + this.receipt.Organization.HeaderImage;
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
            item.Value = "RECEIPT";
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
            item.Value = this.receipt.Organization.Title;
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
            item.Value = this.receipt.Organization.Address1;
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
            item.Value = this.receipt.Organization.City + ", " + this.receipt.Organization.State + ", " + this.receipt.Organization.Zip;
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
            item.Value = this.receipt.Organization.Website;
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
            item.Value = this.receipt.BillingAddress.BillToName;
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Receipt Id : ";
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
            item.Value = this.receipt.Receiptid.ToString();
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
            item.Value = this.receipt.BillingAddress.StreetAddress;
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));

            DateTime receiptDate = this.receipt.Date;
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Receipt Date: ";
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
            item.Value = receiptDate.ToString("d");
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
            item.Value = this.receipt.BillingAddress.City + ", " + this.receipt.BillingAddress.State + ", " + this.receipt.BillingAddress.Zip;
            cell.AddItem(item);
            dt.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Paid From: ";
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
            item.Value = "Member Portal";
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

            PdfPTable itemHeader = PrintItemDetailHeader();
            pdfPTable.AddCell(itemHeader);

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

            foreach (var lineItem in this.receipt.LineItems)
            {
                if (lineItem.MembershipCategory.Length > 0)
                {
                    PdfPTable dt = PrintMembershipSummary(lineItem);
                    pdfPTable.AddCell(dt);
                }
                else if (!string.IsNullOrEmpty(lineItem.EventName) && !string.IsNullOrEmpty(lineItem.EventType))
                {
                    PdfPTable dt = PrintEventSummary(lineItem);
                    pdfPTable.AddCell(dt);
                }
                else
                {
                    PdfPTable dt = PrintLineItem(lineItem);
                    pdfPTable.AddCell(dt);
                }

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
        private PdfPTable PrintLineItem(ReceiptLineItem lineItem)
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
            item.Value = lineItem.Quantity;
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            item = new Item();
            cell = new Cell();
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
            item.Value = lineItem.Rate;
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
            item.Value = lineItem.Amount;
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

        private PdfPTable PrintMembershipSummary(ReceiptLineItem lineItem)
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
            item.Value = lineItem.MembershipCategory + ", " + lineItem.MembershipName;
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 0;
            cell.AddItem(item);
            pdfPTable.AddCell(RenderCell(cell));

            //DateTime startDate = ""Membership.StartDate;
            //DateTime endDate = invoice.Membership.EndDate;

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
            //item.Value = lineItem.MembershipPeriod;
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

        private PdfPTable PrintEventSummary(ReceiptLineItem lineItem)
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
            item.Value = lineItem.EventName + " - " + lineItem.EventType;
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

            return (pdfPTable);
        }


        private PdfPTable PrintTotals()
        {
            float pageWidth = PdfDocument.PageSize.Width;

            float[] headerwidths = { pageWidth * 30.0F / 100.0F,
                                     pageWidth * 30.0F / 100.0F,
                                     pageWidth * 30.0F / 100.0F,
                                   };

            float[] leftHeaderwidths = { pageWidth * 90.0F / 100.0F };

            PdfPTable pdfpLeftTable = new PdfPTable(1);
            pdfpLeftTable.SetTotalWidth(leftHeaderwidths);
            pdfpLeftTable.WidthPercentage = 90;


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

            pdfpLeftTable.AddCell(RenderCell(cell));

            var notes = this.receipt.Notes;
            cell = new Cell();

            item = new Item();
            item.ItemType = "Label";
            item.Value = notes;
            item.ColumnSpan = 3;
            item.RowSpan = 3;
            item.Style.Font = "ARIAL";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";

            item.Style.FontColor = "BLACK";

            item.Style.HorzontalAlignment = 0;

            cell.AddItem(item);

            pdfpLeftTable.AddCell(RenderCell(cell));




            PdfPTable pdfPRightTable = new PdfPTable(3);

            pdfPRightTable.SetTotalWidth(headerwidths);
            pdfPRightTable.WidthPercentage = 90;


            item = new Item();
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
            pdfPRightTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Total";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            pdfPRightTable.AddCell(RenderCell(cell));

            decimal subTotal = this.receipt.TotalAmount;
            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = subTotal.ToString("C2");
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            pdfPRightTable.AddCell(RenderCell(cell));

            if (this.receipt.TotalDiscount > 0)
            {
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
                pdfPRightTable.AddCell(RenderCell(cell));

                cell = new Cell();
                item = new Item();
                item.ItemType = "Label";
                item.Value = "Discount";
                item.ColumnSpan = 1;
                item.Style.Font = "VERDANA";
                item.Style.FontSize = fontSize;
                item.Style.FontStyle = "BOLD";
                item.Style.FontColor = "BLACK";
                item.Style.HorzontalAlignment = 2;
                cell.AddItem(item);
                pdfPRightTable.AddCell(RenderCell(cell));

                cell = new Cell();
                item = new Item();
                item.ItemType = "Label";
                item.Value = "$" + this.receipt.TotalDiscount.ToString("N2");
                item.ColumnSpan = 1;
                item.Style.Font = "VERDANA";
                item.Style.FontSize = fontSize;
                item.Style.FontStyle = "NORMAL";
                item.Style.FontColor = "BLACK";
                item.Style.HorzontalAlignment = 2;
                cell.AddItem(item);
                pdfPRightTable.AddCell(RenderCell(cell));
            }

            if (this.receipt.CreditUsed > 0)
            {
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
                pdfPRightTable.AddCell(RenderCell(cell));

                cell = new Cell();
                item = new Item();
                item.ItemType = "Label";
                item.Value = "Member Credit Used";
                item.ColumnSpan = 1;
                item.Style.Font = "VERDANA";
                item.Style.FontSize = fontSize;
                item.Style.FontStyle = "BOLD";
                item.Style.FontColor = "BLACK";
                item.Style.HorzontalAlignment = 2;
                cell.AddItem(item);
                pdfPRightTable.AddCell(RenderCell(cell));

                cell = new Cell();
                item = new Item();
                item.ItemType = "Label";
                item.Value = "$" + this.receipt.CreditUsed.ToString("N2");
                item.ColumnSpan = 1;
                item.Style.Font = "VERDANA";
                item.Style.FontSize = fontSize;
                item.Style.FontStyle = "NORMAL";
                item.Style.FontColor = "BLACK";
                item.Style.HorzontalAlignment = 2;
                cell.AddItem(item);
                pdfPRightTable.AddCell(RenderCell(cell));
            }


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
            pdfPRightTable.AddCell(RenderCell(cell));

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = "Paid by";
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "BOLD";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            pdfPRightTable.AddCell(RenderCell(cell));

            string paymentType = this.receipt.PaymentTransactions[0].PaymentType == "CreditCard" ?
                                this.receipt.PaymentTransactions[0].CardType : this.receipt.PaymentTransactions[0].PaymentType;

            cell = new Cell();
            item = new Item();
            item.ItemType = "Label";
            item.Value = paymentType;
            item.ColumnSpan = 1;
            item.Style.Font = "VERDANA";
            item.Style.FontSize = fontSize;
            item.Style.FontStyle = "NORMAL";
            item.Style.FontColor = "BLACK";
            item.Style.HorzontalAlignment = 2;
            cell.AddItem(item);
            pdfPRightTable.AddCell(RenderCell(cell));

            PdfPTable mainTable = new PdfPTable(2);
            mainTable.SetTotalWidth(new float[] { 60, 40 });
            mainTable.WidthPercentage = 100;
            mainTable.DefaultCell.Border = PdfPCell.NO_BORDER;
            mainTable.AddCell(pdfpLeftTable);
            mainTable.AddCell(pdfPRightTable);

            return (mainTable);
        }


        //private PdfPTable PrintTotals()
        //{
        //    float pageWidth = PdfDocument.PageSize.Width;

        //    float[] headerwidths = { pageWidth * 70.0F / 100.0F,
        //                             pageWidth * 20.0F / 100.0F,
        //                             pageWidth * 10.0F / 100.0F,
        //                           };

        //    PdfPTable pdfPTable = new PdfPTable(3);

        //    pdfPTable.SetTotalWidth(headerwidths);
        //    pdfPTable.WidthPercentage = 90;


        //    Item item = new Item();
        //    Cell cell = new Cell();

        //    item = new Item();
        //    item.ItemType = "Label";
        //    item.Value = "";
        //    item.ColumnSpan = 1;
        //    item.Style.Font = "VERDANA";
        //    item.Style.FontSize = fontSize;
        //    item.Style.FontStyle = "BOLD";

        //    item.Style.FontColor = "BLACK";


        //    cell.AddItem(item);
        //    pdfPTable.AddCell(RenderCell(cell));

        //    cell = new Cell();
        //    item = new Item();
        //    item.ItemType = "Label";
        //    item.Value = "Total";
        //    item.ColumnSpan = 1;
        //    item.Style.Font = "VERDANA";
        //    item.Style.FontSize = fontSize;
        //    item.Style.FontStyle = "BOLD";
        //    item.Style.FontColor = "BLACK";
        //    item.Style.HorzontalAlignment = 2;
        //    cell.AddItem(item);
        //    pdfPTable.AddCell(RenderCell(cell));

        //    decimal subTotal = this.receipt.TotalAmount;
        //    cell = new Cell();
        //    item = new Item();
        //    item.ItemType = "Label";
        //    item.Value = subTotal.ToString("C2");
        //    item.ColumnSpan = 1;
        //    item.Style.Font = "VERDANA";
        //    item.Style.FontSize = fontSize;
        //    item.Style.FontStyle = "NORMAL";
        //    item.Style.FontColor = "BLACK";
        //    item.Style.HorzontalAlignment = 0;
        //    cell.AddItem(item);
        //    pdfPTable.AddCell(RenderCell(cell));

        //    if (this.receipt.TotalDiscount > 0)
        //    {
        //        cell = new Cell();
        //        item = new Item();
        //        item.ItemType = "Label";
        //        item.Value = "";
        //        item.ColumnSpan = 1;
        //        item.Style.Font = "VERDANA";
        //        item.Style.FontSize = fontSize;
        //        item.Style.FontStyle = "BOLD";
        //        item.Style.FontColor = "BLACK";
        //        cell.AddItem(item);
        //        pdfPTable.AddCell(RenderCell(cell));

        //        cell = new Cell();
        //        item = new Item();
        //        item.ItemType = "Label";
        //        item.Value = "Discount";
        //        item.ColumnSpan = 1;
        //        item.Style.Font = "VERDANA";
        //        item.Style.FontSize = fontSize;
        //        item.Style.FontStyle = "BOLD";
        //        item.Style.FontColor = "BLACK";
        //        item.Style.HorzontalAlignment = 2;
        //        cell.AddItem(item);
        //        pdfPTable.AddCell(RenderCell(cell));

        //        cell = new Cell();
        //        item = new Item();
        //        item.ItemType = "Label";
        //        item.Value = "$" + this.receipt.TotalDiscount.ToString("N2");
        //        item.ColumnSpan = 1;
        //        item.Style.Font = "VERDANA";
        //        item.Style.FontSize = fontSize;
        //        item.Style.FontStyle = "NORMAL";
        //        item.Style.FontColor = "BLACK";
        //        item.Style.HorzontalAlignment = 0;
        //        cell.AddItem(item);
        //        pdfPTable.AddCell(RenderCell(cell));
        //    }

        //    if (this.receipt.CreditUsed > 0)
        //    {
        //        cell = new Cell();
        //        item = new Item();
        //        item.ItemType = "Label";
        //        item.Value = "";
        //        item.ColumnSpan = 1;
        //        item.Style.Font = "VERDANA";
        //        item.Style.FontSize = fontSize;
        //        item.Style.FontStyle = "BOLD";
        //        item.Style.FontColor = "BLACK";
        //        cell.AddItem(item);
        //        pdfPTable.AddCell(RenderCell(cell));

        //        cell = new Cell();
        //        item = new Item();
        //        item.ItemType = "Label";
        //        item.Value = "Member Credit Used";
        //        item.ColumnSpan = 1;
        //        item.Style.Font = "VERDANA";
        //        item.Style.FontSize = fontSize;
        //        item.Style.FontStyle = "BOLD";
        //        item.Style.FontColor = "BLACK";
        //        item.Style.HorzontalAlignment = 2;
        //        cell.AddItem(item);
        //        pdfPTable.AddCell(RenderCell(cell));

        //        cell = new Cell();
        //        item = new Item();
        //        item.ItemType = "Label";
        //        item.Value = "$" + this.receipt.CreditUsed.ToString("N2");
        //        item.ColumnSpan = 1;
        //        item.Style.Font = "VERDANA";
        //        item.Style.FontSize = fontSize;
        //        item.Style.FontStyle = "NORMAL";
        //        item.Style.FontColor = "BLACK";
        //        item.Style.HorzontalAlignment = 0;
        //        cell.AddItem(item);
        //        pdfPTable.AddCell(RenderCell(cell));
        //    }


        //    cell = new Cell();
        //    item = new Item();
        //    item.ItemType = "Label";
        //    item.Value = "";
        //    item.ColumnSpan = 1;
        //    item.Style.Font = "VERDANA";
        //    item.Style.FontSize = fontSize;
        //    item.Style.FontStyle = "BOLD";
        //    item.Style.FontColor = "BLACK";
        //    item.Style.HorzontalAlignment = 2;
        //    cell.AddItem(item);
        //    pdfPTable.AddCell(RenderCell(cell));

        //    cell = new Cell();
        //    item = new Item();
        //    item.ItemType = "Label";
        //    item.Value = "Paid by";
        //    item.ColumnSpan = 1;
        //    item.Style.Font = "VERDANA";
        //    item.Style.FontSize = fontSize;
        //    item.Style.FontStyle = "BOLD";
        //    item.Style.FontColor = "BLACK";
        //    item.Style.HorzontalAlignment = 2;
        //    cell.AddItem(item);
        //    pdfPTable.AddCell(RenderCell(cell));

        //    string paymentType = this.receipt.PaymentTransactions[0].PaymentType == "CreditCard" ?
        //                        this.receipt.PaymentTransactions[0].CardType : this.receipt.PaymentTransactions[0].PaymentType;

        //    cell = new Cell();
        //    item = new Item();
        //    item.ItemType = "Label";
        //    item.Value = paymentType;
        //    item.ColumnSpan = 1;
        //    item.Style.Font = "VERDANA";
        //    item.Style.FontSize = fontSize;
        //    item.Style.FontStyle = "NORMAL";
        //    item.Style.FontColor = "BLACK";
        //    item.Style.HorzontalAlignment = 0;
        //    cell.AddItem(item);
        //    pdfPTable.AddCell(RenderCell(cell));

        //    return (pdfPTable);
        //}
    }
}
