using System;
using System.Collections;

namespace Max.Core.Helpers.Pdf
{
	/// <summary>
	/// Summary description for ReportRow.
	/// </summary>
	public class Cell
	{
		private int height;
		private int width;
		private int rowCount;
		private string cssStyle;
		private string columnId;
		private int sectionId;
		public ArrayList Items = new ArrayList(); //List of Items

		public Cell()
		{
			//empty const
		}

		public Cell(int sectionId)
		{
			this.sectionId=sectionId;
			rowCount=0;
			height=0;
			width=0;

		}

		public void AddItem(Item ri) 
		{
			Items.Add(ri);
			rowCount+=1;
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
		
		public int RowCount
		{
			get
			{
				return rowCount;
			}
			set
			{
				rowCount=value;
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
