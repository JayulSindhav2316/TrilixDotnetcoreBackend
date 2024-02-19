using System;

namespace Max.Core.Helpers.Pdf
{
	/// <summary>
	/// Summary description for ReportColumn.
	/// </summary>
	public class Column
	{
		float columnWidth;
		float columnWidthPercentage;

		public Column(float width, float withPercentage)
		{
			columnWidth				=	width;
			columnWidthPercentage	=	withPercentage;
		}

		public float Width
		{
			get
			{
				return columnWidth;
			}
			set
			{
				columnWidth=value;
			}
		}
		public float WidthPercentage
		{
			get
			{
				return columnWidthPercentage;
			}
			set
			{
				columnWidthPercentage=value;
			}
		}
	}
}
