using System;
using System.Collections;

namespace Max.Core.Helpers.Pdf
{
	/// <summary>
	/// Summary description for ReportRow.
	/// </summary>
	public class Row
	{
		private int height;
		private int width;
		private int columnCount;
		private string cssStyle;
		private bool pageBreakNeeded;
		private int rowNumber;
		private int pageNumber;
		private string rowId;
		private int sectionId;
		public ArrayList reportItems = new ArrayList(); //List of Items

		public Row()
		{
			//empty const
		}

		public Row(int sectionId)
		{
			this.sectionId=sectionId;
			pageBreakNeeded=false;
			columnCount=0;
			height=0;
			width=0;

		}

		public void AddItem(Item ri) 
		{
			reportItems.Add(ri);
			columnCount+=1;
		}

		public int Height
		{
			get
			{
				return height;
			}
			set
			{
				height=value;
			}
		}
		public int Width
		{
			get
			{
				return width;
			}
			set
			{
				width=value;
			}
		}
		public string CssStyle
		{
			get
			{
				return cssStyle;
			}
			set
			{
				cssStyle=value;
			}
		}
		public bool PageBreak
		{
			get
			{
				return pageBreakNeeded;
			}
			set
			{
				pageBreakNeeded=value;
			}
		}
		public int RowNumber
		{
			get
			{
				return rowNumber;
			}
			set
			{
				rowNumber=value;
			}
		}
	
		public int PageNumber
		{
			get
			{
				return pageNumber;
			}
			set
			{
				pageNumber=value;
			}
		}
		public string RowId
		{
			get
			{
				return rowId;
			}
			set
			{
				rowId=value;
			}
		}
		public int SectionId
		{
			get
			{
				return sectionId;
			}
			set
			{
				sectionId=value;
			}
		}
	}
}
