using System;

using System.Collections;

using System.Data;



namespace Max.Core.Helpers.Pdf
{
	/// <summary>
	/// Summary description for Report.
	/// </summary>
	public class MaxPdfDocument
	{


		private string fileName;    //FileName
		private decimal pageWidth;  //Report Page Width in Inches
		private decimal pageHight;  //Page Height in Inches
		private int totalPages; //Total pages in the report
		private int pageNumber; //Current Page number
		private float topMargin; //Report page margins
		private float bottomMargin;
		private float leftMargin;
		private float rightMargin;
		private int printGridLines; //Print Grid Lines
		private int blankLines; //Print Blank Lines at the end
		private PageLayoutTypes pageLayout;  //Layout Portrait/landscape
		public ArrayList sections = new ArrayList(); //List of Sections
		private DataSet ds;
		public int FontSize {get;set;}

		public MaxPdfDocument(string docName)
		{
			this.fileName		= "PdfDocument_"+ docName;
			this.totalPages		= 0;
			this.pageNumber		= 0;
		}
	
		public void AddSection(Section section) 
		{
			sections.Add(section);
		}

		public decimal PageWidth
		{
			get
			{
				return pageWidth;
			}
			set
			{
				pageWidth=value;
			}
		}
		public decimal PageHight
		{
			get
			{
				return pageHight;
			}
			set
			{
				pageHight=value;
			}
		}
		public PageLayoutTypes LayOut
		{
			get
			{
				return pageLayout;
			}
			set
			{
				pageLayout=value;
			}
		}
		
		public DataSet ReportDataSet
		{
			get
			{
				return ds;
			}
			set
			{
				ds=value;
			}
		}
		public float TopMargin
		{
			get
			{
				return topMargin;
			}
			set
			{
				topMargin=value;
			}
		}
		public float BottomMargin
		{
			get
			{
				return bottomMargin;
			}
			set
			{
				bottomMargin = value;
			}
		}
		public float LeftMargin
		{
			get
			{
				return leftMargin;
			}
			set
			{
				leftMargin = value;
			}

		}
		public float RightMargin
		{
			get
			{
				return rightMargin;
			}
			set
			{
				rightMargin = value;
			}
		}
		public int BlankLines
		{
			get
			{
				return blankLines;
			}
			set
			{
				blankLines = value;
			}
		}
       
	}
}
