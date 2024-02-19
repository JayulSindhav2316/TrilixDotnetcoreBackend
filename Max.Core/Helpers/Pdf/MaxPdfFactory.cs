using System;
using System.Drawing;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace Max.Core.Helpers.Pdf
{
    /// <summary>
    /// This class will act as a super class for all PDF Report generating classes to
    /// avoid code duplication. 
    /// </summary>
    public abstract class MaxPdfFactory
    {

        private iTextSharp.text.Document document;
        private PdfWriter writer;
        private MaxPdfPageEvent pageEvent;
        private int serialNumber;
        private MaxPdfDocument pdfDocument;
        private int defaultFontSize;
        private bool printBlankRow;
        private iTextSharp.text.Font baseFont; 

        public MaxPdfFactory()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public iTextSharp.text.Document CreatePdfDocument(string fileName, MaxPdfDocument pdfocument, int defaultFontSize)
        {
            this.pdfDocument = pdfocument;
            this.defaultFontSize = defaultFontSize;
            this.baseFont =  FontFactory.GetFont("Calibri", 16);

            if (pdfDocument.LayOut == PageLayoutTypes.Landscape)
            {
                document = new iTextSharp.text.Document(PageSize.Letter.Rotate(), pdfocument.LeftMargin, pdfocument.RightMargin, pdfocument.TopMargin, pdfocument.BottomMargin);
            }
            else
            {
                document = new iTextSharp.text.Document(PageSize.Letter, pdfocument.LeftMargin, pdfocument.RightMargin, pdfocument.TopMargin, pdfocument.BottomMargin);

            }

            return (document);
        }
        protected Cell CreateReportTextCell(string text, int fontSize, bool isBold)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = fontSize;
            item.Style.HorzontalAlignment = PdfPCell.ALIGN_LEFT;
            item.Style.Font = "Calibri";
            if (isBold)
            {
                item.Style.FontStyle = "BOLD";
            }
            cell.AddItem(item);
            return cell;

        }
        protected Cell CreateReportTextCell(string text, int fontSize, bool isBold, int colSpan)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = fontSize;
            item.Style.HorzontalAlignment = PdfPCell.ALIGN_LEFT;
            item.ColumnSpan = colSpan;
            item.Style.Font = "Calibri";
            if (isBold)
            {
                item.Style.FontStyle = "BOLD";
            }
            cell.AddItem(item);
            return cell;

        }
        protected Cell CreateReportTextCell(Item item)
        {
            Cell cell = new Cell();
            cell.AddItem(item);
            return cell;

        }
        protected Cell CreateReportTextCell(string text, int fontSize, bool isBold, int colSpan, int alignment, string backGround, string FontColor)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = fontSize;
            item.Style.Background = backGround;
            item.Style.HorzontalAlignment = alignment;
            item.Style.FontColor = FontColor;
            item.Style.Font = "Calibri";
            item.ColumnSpan = colSpan;


            if (isBold)
            {
                item.Style.FontStyle = "BOLD";
            }
            cell.AddItem(item);
            return cell;

        }
        protected Cell CreateReportTextCell(string text)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = defaultFontSize;
            item.Style.HorzontalAlignment = PdfPCell.ALIGN_LEFT;
            item.Style.Font = "Calibri";
            cell.AddItem(item);
            return cell;

        }
        protected Cell CreateReportTextCellWithBorder(string text)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = defaultFontSize;
            item.Style.HorzontalAlignment = PdfPCell.ALIGN_LEFT;
            item.Style.BorderBottom = 1;
            item.Style.BorderLeft = 0;
            item.Style.BorderRight = 1;
            item.Style.BorderTop = 0;
            cell.AddItem(item);
            return cell;

        }

        protected Cell CreateReportTextCellWithBorder(string text, string FontColor)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = defaultFontSize;
            item.Style.HorzontalAlignment = PdfPCell.ALIGN_LEFT;
            item.Style.BorderBottom = 1;
            item.Style.BorderLeft = 0;
            item.Style.BorderRight = 1;
            item.Style.BorderTop = 0;
            item.Style.FontColor = FontColor;
            cell.AddItem(item);
            return cell;

        }

        protected Cell CreateReportTextCellWithBorder(string text, string FontColor, int fontSize)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = fontSize;
            item.Style.HorzontalAlignment = PdfPCell.ALIGN_LEFT;
            item.Style.BorderBottom = 1;
            item.Style.BorderLeft = 0;
            item.Style.BorderRight = 1;
            item.Style.BorderTop = 0;
            item.Style.FontColor = FontColor;
            cell.AddItem(item);
            return cell;

        }

        protected Cell CreateReportTextCellWithBorder(string text, int alignment)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = defaultFontSize;
            item.Style.HorzontalAlignment = alignment;
            item.Style.BorderBottom = 1;
            item.Style.BorderLeft = 0;
            item.Style.BorderRight = 1;
            item.Style.BorderTop = 0;
            cell.AddItem(item);
            return cell;

        }
        protected Cell CreateReportTextCellWithAllBorder(string text, int alignment)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = defaultFontSize;
            item.Style.HorzontalAlignment = alignment;
            item.Style.BorderBottom = 1;
            item.Style.BorderLeft = 1;
            item.Style.BorderRight = 1;
            item.Style.BorderTop = 1;
            cell.AddItem(item);
            return cell;

        }
        protected Cell CreateReportTextCellWithBorder(string text, int colSpan, int alignment)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = defaultFontSize;
            item.Style.HorzontalAlignment = alignment;
            item.Style.BorderBottom = 1;
            item.Style.BorderLeft = 0;
            item.Style.BorderRight = 1;
            item.Style.BorderTop = 0;
            item.ColumnSpan = colSpan;
            cell.AddItem(item);
            return cell;

        }
        protected Cell CreateReportTextCellWithLeftBorder(string text)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = defaultFontSize;
            item.Style.HorzontalAlignment = PdfPCell.ALIGN_LEFT;
            item.Style.BorderBottom = 1;
            item.Style.BorderLeft = 1;
            item.Style.BorderRight = 1;
            item.Style.BorderTop = 0;
            cell.AddItem(item);
            return cell;

        }

        protected Cell CreateReportTextCellWithLeftBorder(string text, string FontColor)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = defaultFontSize;
            item.Style.HorzontalAlignment = PdfPCell.ALIGN_LEFT;
            item.Style.BorderBottom = 1;
            item.Style.BorderLeft = 1;
            item.Style.BorderRight = 1;
            item.Style.BorderTop = 0;
            item.Style.FontColor = FontColor;
            cell.AddItem(item);
            return cell;

        }

        protected Cell CreateReportTextCellWithLeftBorder(string text, string FontColor, int fontSize)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = fontSize;
            item.Style.HorzontalAlignment = PdfPCell.ALIGN_LEFT;
            item.Style.BorderBottom = 1;
            item.Style.BorderLeft = 1;
            item.Style.BorderRight = 1;
            item.Style.BorderTop = 0;
            item.Style.FontColor = FontColor;
            cell.AddItem(item);
            return cell;

        }

        protected Cell CreateReportTextCellWithLeftBorder(string text, int alignment)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = defaultFontSize;
            item.Style.HorzontalAlignment = alignment;
            item.Style.BorderBottom = 1;
            item.Style.BorderLeft = 1;
            item.Style.BorderRight = 1;
            item.Style.BorderTop = 0;
            cell.AddItem(item);
            return cell;

        }
        protected Cell CreateReportTextCell(string text, int colSpan)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = defaultFontSize;
            item.Style.HorzontalAlignment = PdfPCell.ALIGN_LEFT;
            item.ColumnSpan = colSpan;


            cell.AddItem(item);
            return cell;

        }
        protected Cell CreateReportTextCell(string labelText, string labelValue)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = labelText + " " + labelValue;
            item.Style.FontSize = defaultFontSize;
            item.Style.HorzontalAlignment = PdfPCell.ALIGN_LEFT;

            cell.AddItem(item);
            return cell;
        }
        protected Cell CreateReportFooterTextCellLeftBorder(string labelText, int alignment, int colSpan)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = labelText;
            item.Style.FontSize = 10;
            item.Style.FontStyle = "BOLD";
            item.Style.HorzontalAlignment = alignment;
            item.Style.Background = "LIGHT_GRAY";
            item.Style.BorderBottom = 1;
            item.Style.BorderLeft = 1;
            item.Style.BorderRight = 1;
            item.Style.BorderTop = 0;
            item.ColumnSpan = colSpan;
            cell.AddItem(item);
            return cell;
        }

        protected Cell CreateReportFooterTextCellWithoutBorder(string labelText, int alignment, int colSpan)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = labelText;
            item.Style.FontSize = 10;
            item.Style.FontStyle = "BOLD";
            item.Style.HorzontalAlignment = alignment;
            item.Style.Background = "LIGHT_GRAY";
            item.Style.BorderBottom = 0;
            item.Style.BorderLeft = 0;
            item.Style.BorderRight = 0;
            item.Style.BorderTop = 0;
            item.ColumnSpan = colSpan;
            cell.AddItem(item);
            return cell;
        }

        protected Cell CreateReportFooterTextCell(string labelText, int alignment, int colSpan)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = labelText;
            item.Style.FontSize = 10;
            item.Style.FontStyle = "BOLD";
            item.Style.HorzontalAlignment = alignment;
            item.Style.Background = "LIGHT_GRAY";
            item.Style.BorderBottom = 1;
            item.Style.BorderLeft = 0;
            item.Style.BorderRight = 1;
            item.Style.BorderTop = 0;
            item.ColumnSpan = colSpan;
            cell.AddItem(item);
            return cell;
        }
        protected PdfPCell RenderCell(Cell Cell)
        {
            PdfPCell cell = new PdfPCell();
            int columnSpan = 1;
            BaseColor textBaseColor = new BaseColor(0, 0, 0);
            int itemFont = iTextSharp.text.Font.NORMAL;
            PdfPTable cellTable = new PdfPTable(1);

            for (int row = 0; row < Cell.Items.Count; row++)
            {
                Item item = (Item)Cell.Items[row];
                if (item.Style.FontColor == "WHITE")
                    textBaseColor = new BaseColor(255, 255, 255);

                if (item.Style.FontColor == "EXTRA_LIGHT_GRAY")
                    textBaseColor = new BaseColor(211, 211, 211);

                if (item.Style.FontColor == "LIGHT_GRAY")
                    textBaseColor = new BaseColor(191, 191, 191);

                if (item.Style.FontColor == "GRAY")
                    textBaseColor = new BaseColor(127, 127, 127);

                if (item.Style.FontColor == "RED")
                    textBaseColor = new BaseColor(255, 0, 0);

                if (item.Style.FontStyle == "BOLD")
                    itemFont = iTextSharp.text.Font.BOLD;

                if (columnSpan < item.ColumnSpan)
                    columnSpan = item.ColumnSpan;

                if (item.ItemType == "iTextSharp.text.Image" && File.Exists(item.Value))
                {
                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(item.Value);
                    img.ScaleAbsolute((float)item.Width, (float)item.Height);
                    Chunk ck = new Chunk(img, 0, 0);
                    Phrase ph = new Phrase();
                    ph.Add(ck);

                    PdfPCell innerCell = new PdfPCell(ph);

                    innerCell.BorderWidthBottom = item.Style.BorderBottom;
                    innerCell.BorderWidthTop = item.Style.BorderTop;
                    innerCell.BorderWidthLeft = item.Style.BorderLeft;
                    innerCell.BorderWidthRight = item.Style.BorderRight;
                    innerCell.Colspan = item.ColumnSpan;
                    innerCell.HorizontalAlignment = PdfPCell.ALIGN_MIDDLE;
                    cellTable.AddCell(innerCell);
                }
                else if (item.ItemType == "iTextSharp.text.ImageAligned")
                {
                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(item.Value);
                    img.ScaleAbsolute((float)item.Width, (float)item.Height);
                    Chunk ck = new Chunk(img, 0, 0);
                    Phrase ph = new Phrase();
                    ph.Add(ck);

                    PdfPCell innerCell = new PdfPCell(ph);

                    innerCell.BorderWidthBottom = item.Style.BorderBottom;
                    innerCell.BorderWidthTop = item.Style.BorderTop;
                    innerCell.BorderWidthLeft = item.Style.BorderLeft;
                    innerCell.BorderWidthRight = item.Style.BorderRight;
                    innerCell.Colspan = item.ColumnSpan;
                    innerCell.HorizontalAlignment = item.Style.HorzontalAlignment;
                    cellTable.AddCell(innerCell);
                }
                else if (item.ItemType == "Label")
                {
                    PdfPCell innerCell = new PdfPCell(new Phrase(item.Value, FontFactory.GetFont(item.Style.Font, item.Style.FontSize, itemFont, textBaseColor)));
                    innerCell.BorderWidthBottom = item.Style.BorderBottom;
                    innerCell.BorderWidthTop = item.Style.BorderTop;
                    innerCell.BorderWidthLeft = item.Style.BorderLeft;
                    innerCell.BorderWidthRight = item.Style.BorderRight;
                    innerCell.BorderColor = new iTextSharp.text.BaseColor(System.Drawing.Color.Gray);
                    innerCell.Colspan = item.ColumnSpan;
                    if (item.Style.HorzontalAlignment == PdfPCell.ALIGN_LEFT)
                    {
                        innerCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    }
                    else if (item.Style.HorzontalAlignment == PdfPCell.ALIGN_RIGHT)
                    {
                        innerCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    }
                    else
                    {
                        innerCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    }
                    if (item.Style.Background == "GRAY")
                    {
                        innerCell.BackgroundColor = new iTextSharp.text.BaseColor(System.Drawing.Color.Gray);
                    }
                    if (item.Style.Background == "DIM_GRAY")
                    {
                        innerCell.BackgroundColor = new iTextSharp.text.BaseColor(176, 176, 176, 80);
                    }
                    if (item.Style.Background == "LIGHT_GRAY")
                    {
                        innerCell.BackgroundColor = new iTextSharp.text.BaseColor(System.Drawing.Color.LightGray);
                    }
                    if (item.Style.Background == "EXTRA_LIGHT_GRAY")
                    {
                        innerCell.BackgroundColor = new iTextSharp.text.BaseColor(System.Drawing.Color.WhiteSmoke);
                    }
                    if (item.Style.BorderColor == "BLACK")
                    {
                        innerCell.BorderColor = new iTextSharp.text.BaseColor(System.Drawing.Color.Black);
                    }
                    if (item.Style.Background == "BLACK")
                    {
                        innerCell.BackgroundColor = new iTextSharp.text.BaseColor(System.Drawing.Color.Black);
                    }
                    if (item.Style.FontColor == "WHITE")
                    {
                        textBaseColor = new BaseColor(255, 255, 255);
                    }
                    cellTable.AddCell(innerCell);

                }
                else if (item.ItemType == "Function")
                {
                    string functionValue = "";
                    if (item.Value == "SerialNumber()")
                    {
                        functionValue = serialNumber.ToString();
                        serialNumber++;
                    }
                    PdfPCell innerCell = new PdfPCell(new Phrase(functionValue, FontFactory.GetFont(item.Style.Font, item.Style.FontSize, itemFont, textBaseColor)));
                    innerCell.BorderWidthBottom = item.Style.BorderBottom;
                    innerCell.BorderWidthTop = item.Style.BorderTop;
                    innerCell.BorderWidthLeft = item.Style.BorderLeft;
                    innerCell.BorderWidthRight = item.Style.BorderRight;
                    innerCell.Colspan = item.ColumnSpan;
                    innerCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    if (item.Style.Background == "GRAY")
                        innerCell.BackgroundColor = new iTextSharp.text.BaseColor(System.Drawing.Color.Gray);
                    cellTable.AddCell(innerCell);

                }
                else
                {
                    if (item.TypeOfDataSource == DataSourceTypes.DataSet)
                    {
                        string value = "";
                        if (item.TypeOfData == DataTypes.DecimalType)
                        {
                            decimal amount = Convert.ToDecimal(item.Value.ToString());
                            value = item.Text + amount.ToString("C");
                        }
                        else
                        {
                            value = item.Text + item.Value.ToString();
                        }
                        Phrase ph = new Phrase(value, FontFactory.GetFont(item.Style.Font, item.Style.FontSize, itemFont, textBaseColor));
                        PdfPCell innerCell = new PdfPCell(ph);
                        innerCell.BorderWidthBottom = item.Style.BorderBottom;
                        innerCell.BorderWidthTop = item.Style.BorderTop;
                        innerCell.BorderWidthLeft = item.Style.BorderLeft;
                        innerCell.BorderWidthRight = item.Style.BorderRight;
                        innerCell.Colspan = item.ColumnSpan;
                        innerCell.HorizontalAlignment = PdfPCell.ALIGN_MIDDLE;
                        cellTable.AddCell(innerCell);
                    }
                }

            }
            cell = new PdfPCell(cellTable);
            cell.BorderWidthBottom = 0;
            cell.BorderWidthTop = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthRight = 0;
            cell.Colspan = columnSpan;
            return (cell);

        }
        protected Cell CreateReportTextCellWithBottomBorder(string text, int fontSize, bool isBold, int colSpan, int horalignment, string backGround, string FontColor, string borderBaseColor)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = fontSize;
            item.Style.Background = backGround;
            item.Style.HorzontalAlignment = horalignment;
            item.Style.FontColor = FontColor;
            item.Style.BorderBottom = 1;
            item.Style.BorderLeft = 0;
            item.Style.BorderRight = 0;
            item.Style.BorderTop = 0;
            item.ColumnSpan = colSpan;
            item.Style.BorderColor = borderBaseColor;



            if (isBold)
            {
                item.Style.FontStyle = "BOLD";
            }
            cell.AddItem(item);
            return cell;

        }

        protected Cell CreateReportTextCellWithTopBorder(string text, int fontSize, bool isBold, int colSpan, int horalignment, string backGround, string FontColor, string borderBaseColor)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = fontSize;
            item.Style.Background = backGround;
            item.Style.HorzontalAlignment = horalignment;
            item.Style.FontColor = FontColor;
            item.Style.BorderBottom = 0;
            item.Style.BorderLeft = 0;
            item.Style.BorderRight = 0;
            item.Style.BorderTop = 1;
            item.ColumnSpan = colSpan;
            item.Style.BorderColor = borderBaseColor;



            if (isBold)
            {
                item.Style.FontStyle = "BOLD";
            }
            cell.AddItem(item);
            return cell;
        }

        protected Cell CreateReportTextCell(string text, int fontSize, bool isBold, int colSpan, int alignment, string backGround, string FontColor, int rowSpan)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = fontSize;
            item.Style.Background = backGround;
            item.Style.HorzontalAlignment = alignment;
            item.Style.FontColor = FontColor;
            item.ColumnSpan = colSpan;
            item.RowSpan = rowSpan;



            if (isBold)
            {
                item.Style.FontStyle = "BOLD";
            }
            cell.AddItem(item);
            return cell;

        }

        protected Cell CreateReportTextCellWithTopAndBottomBorder(string text, int fontSize, bool isBold, int colSpan, int alignment, string backGround, string FontColor, string borderBaseColor)
        {
            Item item = new Item();
            Cell cell = new Cell();
            PdfPCell innerCell = new PdfPCell();
            BaseColor textBaseColor = new BaseColor(0, 0, 0);
            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = fontSize;
            item.Style.Background = backGround;
            item.Style.HorzontalAlignment = alignment;
            item.Style.BorderBottom = 1;
            item.Style.BorderLeft = 0;
            item.Style.BorderRight = 0;
            item.Style.BorderTop = 1;
            item.Style.FontColor = FontColor;
            item.ColumnSpan = colSpan;
            if (item.Style.Background == "BLACK")
                innerCell.BackgroundColor = new iTextSharp.text.BaseColor(System.Drawing.Color.Black);
            if (item.Style.FontColor == "WHITE")
                textBaseColor = new BaseColor(System.Drawing.Color.White);
            if (item.Style.FontColor == "RED")
                textBaseColor = new BaseColor(System.Drawing.Color.Red);
            if (isBold)
            {
                item.Style.FontStyle = "BOLD";
            }
            cell.AddItem(item);
            return cell;

        }

        protected Cell CreateReportTemplateDetailedHeaderTextCell(string text, int alignment, bool isLeftMost)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = 8;
            item.Style.HorzontalAlignment = alignment;
            item.Style.BorderBottom = 1;
            item.Style.BorderLeft = 0;
            item.Style.BorderRight = 1;
            item.Style.BorderTop = 0;
            item.ColumnSpan = 1;
            item.Style.Background = "LIGHT_GRAY";
            if (isLeftMost)
            {
                item.Style.BorderLeft = 1;
            }
            cell.AddItem(item);
            return cell;

        }
        protected Cell CreateReportTemplateDetailedTextCell(string text, int alignment, bool isLeftMost)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = 8;
            item.Style.HorzontalAlignment = alignment;
            item.Style.BorderBottom = 1;
            item.Style.BorderLeft = 0;
            item.Style.BorderRight = 1;
            item.Style.BorderTop = 0;
            item.ColumnSpan = 1;
            item.Style.Background = "EXTRA_LIGHT_GRAY";
            if (isLeftMost)
            {
                item.Style.BorderLeft = 1;
            }
            cell.AddItem(item);
            return cell;

        }

        protected Cell CreateReportTemplateSummaryTextCell(string text, int alignment, bool isLeftMost, bool isAlternate)
        {
            Item item = new Item();
            Cell cell = new Cell();

            item.ItemType = "Label";
            item.Value = text;
            item.Style.FontSize = 8;
            item.Style.HorzontalAlignment = alignment;
            item.Style.BorderBottom = 1;
            item.Style.BorderLeft = 0;
            item.Style.BorderRight = 1;
            item.Style.BorderTop = 0;
            item.ColumnSpan = 1;
            if (isAlternate)
            {
                item.Style.Background = "EXTRA_LIGHT_GRAY";
            }
            else
            {
                item.Style.Background = "WHITE";
            }
            if (isLeftMost)
            {
                item.Style.BorderLeft = 1;
            }
            cell.AddItem(item);
            return cell;

        }

    }

    public class MaxPdfPageEvent : PdfPageEventHelper
    {

        // This is the contentbyte object of the writer
        PdfContentByte cb;

        // we will put the final number of pages in a template
        PdfTemplate template;

        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;
        private PdfPTable pageHeader;
        iTextSharp.text.Image NvcImg;

        private bool printBlankRow;
        private bool printLogoInFooter = false;
        private bool printPrintedOnDateInFooter = true;
        private bool printPageNoInFooter = false;

        private string logoImagePath;

        private bool printPageHeader;
        public bool PrintPageHeader
        {
            get
            {
                return printPageHeader;
            }
            set
            {
                printPageHeader = value;
            }
        }
        public string LogoImagePath
        {
            get
            {
                return logoImagePath;
            }
            set
            {
                logoImagePath = value;
            }
        }
        public bool PrintLogoInFooter
        {
            get
            {
                return printLogoInFooter;
            }
            set
            {
                printLogoInFooter = value;
            }
        }
        public bool PrintPrintedOnDateInFooter
        {
            get
            {
                return printPrintedOnDateInFooter;
            }
            set
            {
                printPrintedOnDateInFooter = value;
            }
        }

        public bool PrintPageNoInFooter
        {
            get
            {
                return printPageNoInFooter;
            }
            set
            {
                printPageNoInFooter = value;
            }
        }
        public PdfPTable PageHeader
        {
            get
            {
                return pageHeader;
            }
            set
            {
                pageHeader = value;
            }
        }
        public bool PrintBlankRow
        {
            get
            {
                return printBlankRow;
            }
            set
            {
                printBlankRow = value;
            }
        }
        public MaxPdfPageEvent(string imagePath)
        {
            NvcImg = iTextSharp.text.Image.GetInstance(imagePath);
            NvcImg.ScaleAbsolute(38, 33);
        }
        public override void OnEndPage(PdfWriter writer, iTextSharp.text.Document document)
        {
            //setHeader(writer, document);
            setFooter(writer, document);


        }
        public override void OnOpenDocument(PdfWriter writer, iTextSharp.text.Document document)
        {
            bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            cb = writer.DirectContent;
            template = cb.CreateTemplate(50, 50);

        }
        public override void OnCloseDocument(PdfWriter writer, iTextSharp.text.Document document)
        {
            template.BeginText();
            template.SetFontAndSize(bf, 8);
            template.ShowText((writer.PageNumber - 1).ToString());
            template.EndText();
        }

        public void setHeader(PdfWriter writer, iTextSharp.text.Document document)
        {
            if (writer.PageNumber > 0)
            {
                if (printPageHeader)
                {
                    if(pageHeader != null)
                    {
                        pageHeader.TotalWidth = document.PageSize.Width - 20;
                        pageHeader.WriteSelectedRows(0, -1, 10, document.PageSize.Top - 5, writer.DirectContent);
                    }
                }
                else
                {
                    printPageHeader = true;
                }
            }
        }
        public void setFooter(PdfWriter writer, iTextSharp.text.Document document)
        {

            string dateText = "Printed On: " + DateTime.Now.ToString("MM/dd/yyyy");
            NvcImg.SetAbsolutePosition(writer.PageSize.Width / 2 - (100 + 10) / 2, 8);
            NvcImg.BorderWidth = 0;
            if (printLogoInFooter)
            {
                cb.AddImage(NvcImg);
            }
        
            int pageN = writer.PageNumber;
            String text = "Page " + pageN + " of ";
            float len = bf.GetWidthPoint(text, 8);

            float pageTextX = writer.PageSize.Width - len - 20;
            if (printPageNoInFooter)
            {
                cb.BeginText();
                cb.SetFontAndSize(bf, 8);
                cb.SetTextMatrix(pageTextX, 20);
                cb.ShowText(text);
                cb.EndText();
                cb.AddTemplate(template, pageTextX + len, 20);
                cb.BeginText();
                cb.SetFontAndSize(bf, 8);
                cb.SetTextMatrix(pageTextX, 820);
                cb.EndText();
            }


            len = bf.GetWidthPoint(dateText, 8);
            pageTextX = writer.PageSize.Width / 2 - (100 + 10) / 2;

            if (printPrintedOnDateInFooter)
            {
                cb.BeginText();
                cb.SetFontAndSize(bf, 8);
                cb.SetTextMatrix(pageTextX, 20);
                cb.ShowText(dateText);
                cb.EndText();
            }
            if (printBlankRow)
            {
                printBlankRow = false;
            }


        }


    }
}
