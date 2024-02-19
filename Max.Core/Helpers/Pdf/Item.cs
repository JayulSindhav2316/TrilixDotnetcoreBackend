using System;
using Max.Core.Helpers.PdfDocument;

namespace Max.Core.Helpers.Pdf

{
	/// <summary>
	/// Summary description for ReportItem.
	/// </summary>
	public class Item
	{

		private int	itemId;
		private int sectionId;
		private string value;
		private string text;
		private int rowId;
		private int columnId;
		private int rowSpan;
		private int columnSpan;
		private string cssClass;
		private string itemDataType;
		private DataSourceTypes dataSourceType;
		private DataTypes dataType=DataTypes.StringType;
		private int height;
		private int width;
		private bool visible;
		private string label;
		private string itemType;
		private string fieldOrFile;
		private int dsType;
		private string fieldName;
		private int rowHeight;
		private ItemStyle style;

		public Item()
		{
			
			this.rowSpan=1;
			this.columnSpan=1;
			this.cssClass="nvcItem";
			this.style=new ItemStyle();
		}
		public int ItemId
		{
			get
			{
				return itemId;
			}
			set
			{
				itemId=value;
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
		
		public string ItemDataType
		{
			get 
			{
				return itemDataType;
			}

			set 
			{
				itemDataType=value;
			}
		}
		public DataSourceTypes TypeOfDataSource
		{
			get
			{
				return dataSourceType;
			}

			set
			{
				dataSourceType = value;
			}
		}
		public DataTypes TypeOfData
		{
			get 
			{
				return dataType;
			}

			set 
			{
				dataType=value;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value=value;
			}
		}
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text=value;
			}
		}
		public int RowId
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
		public int ColumnId
		{
			get
			{
				return columnId;
			}
			set
			{
				columnId=value;
			}
		}
		public int ColumnSpan
		{
			get
			{
				return columnSpan;
			}
			set
			{
				columnSpan=value;
			}
		}
		public int RowSpan
		{
			get
			{
				return rowSpan;
			}
			set
			{
				rowSpan=value;
			}
		}
		public string CssClass
		{
			get
			{
				return cssClass;
			}
			set
			{
				cssClass=value;
			}
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
		public bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				visible=value;
			}
		}
		public string Label
		{
			get
			{
				return label;
			}
			set
			{
				label = value;
			}
		}
		public string ItemType
		{
			get
			{
				return itemType;
			}
			set
			{
				itemType = value;
			}
		}
		public string FieldOrFile
		{
			get
			{
				return fieldOrFile;
			}
			set
			{
				fieldOrFile = value;
			}
		}
		public int DsType
		{
			get
			{
				return dsType;
			}
			set
			{
				dsType = value;
			}
		}
		public string FieldName
		{
			get
			{
				return fieldName;
			}
			set
			{
				fieldName = value;
			}
		}
		public int RowHeight
		{
			get
			{
				return rowHeight;
			}
			set
			{
				rowHeight = value;
			}
		}
		public ItemStyle Style
		{
			get
			{
				return style;
			}
			set
			{
				style = value;
			}

		}
	}
}
