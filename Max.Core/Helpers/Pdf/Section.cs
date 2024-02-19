
using System.Collections;

namespace  Max.Core.Helpers.Pdf
{
	/// <summary>
	/// Summary description for ReportSection.
	/// </summary>
	public class Section
	{
		public enum SectionTypes	{ReportHeader=1,PageHeader,GroupHeader,ReportBody,GroupFooter,PageFooter,ReportFooter,BodyHeader};
		private int printOnEachPage;
		private int printOnFirstPage;
		private int printOnLastPage;
		private int totalColumns;
		private SectionTypes sectionType;
		private string groupExpression;
		public ArrayList cells = new ArrayList(); //List of Cells
		public ArrayList columns = new ArrayList(); //List of Columns


		public Section()
		{
			groupExpression="";
		}
		
		public void AddCell(Cell cell)
		{
			cells.Add(cell);
		}
		public void AddColumn(Column column)
		{
			columns.Add(column);
		}

		
		public SectionTypes TypeOfSection 
		{
			get 
			{
				return sectionType;
			}

			set 
			{
				sectionType=value;
			}
		}
		public int PrintOnEachPage
		{
			get
			{
				return printOnEachPage;
			}
			set
			{
				printOnEachPage=value;
			}
		}
		public int PrintOnFirstPage
		{
			get
			{
				return printOnFirstPage;
			}
			set
			{
				printOnFirstPage=value;
			}
		}
		public int PrintOnLastPage
		{
			get
			{
				return printOnLastPage;
			}
			set
			{
				printOnLastPage=value;
			}
		}
		public string GroupExpression
		{
			get
			{
				return groupExpression;
			}
			set
			{
				groupExpression=value;
			}
		}
		public int TotalColumn 
		{
			get
			{
				return totalColumns;
			}
			set
			{
				totalColumns = value;
			}
		}
	}
}
