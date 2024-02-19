using System;

namespace Max.Core.Helpers.PdfDocument
{
	/// <summary>
	/// Summary description for ItemStyle.
	/// </summary>
	public class ItemStyle
	{
		private int borderRight;
		private int borderLeft;
		private int borderTop;
		private int borderBottom;

		private string font;
		private int	fontSize;
		private string fontStyle;
		private string fontColor;
		private string background;

		private int horizontalAlignment;
		private int verticalAlignment;
        private string bordercolor;

		public ItemStyle()
		{
			this.font		=	"ARIAL";
			this.borderTop	=	0;
			this.borderBottom	=	0;
			this.borderRight	=	0;
			this.borderLeft		=	0;
			this.fontSize		=	10;
			this.fontStyle		=	"normal";
			this.fontColor		=	"black";
			this.background		=	"white";
			this.horizontalAlignment	=	1 ;//"CENTER";
			this.verticalAlignment		=	1; //"MIDDLE";
            this.bordercolor = "black";

		}

		public int BorderRight
		{
			get
			{
				return borderRight;
			}
			set
			{
				borderRight=value;
			}
		}
		public int BorderLeft
		{
			get
			{
				return borderLeft;
			}
			set
			{
				borderLeft=value;
			}
		}
		public int BorderTop
		{
			get
			{
				return borderTop;
			}
			set
			{
				borderTop=value;
			}
		}
		public int BorderBottom
		{
			get
			{
				return borderBottom;
			}
			set
			{
				borderBottom=value;
			}
		}
		public int FontSize
		{
			get
			{
				return fontSize;
			}
			set
			{
				fontSize=value;
			}
		}
		public string Font
		{
			get
			{
				return font;
			}
			set
			{
				font=value;
			}
		}
		public string FontColor
		{
			get
			{
				return fontColor;
			}
			set
			{
				fontColor=value;
			}
		}
		public string FontStyle
		{
			get
			{
				return fontStyle;
			}
			set
			{
				fontStyle=value;
			}
		}
		public string Background
		{
			get
			{
				return background;
			}
			set
			{
				background=value;
			}
		}
		public int HorzontalAlignment
		{
			get
			{
				return horizontalAlignment;
			}
			set
			{
				horizontalAlignment=value;
			}
		}
		public int VerticalAlignment
		{
			get
			{
				return verticalAlignment;
			}
			set
			{
				verticalAlignment=value;
			}
		}
        public string BorderColor
        {
            get
            {
                return bordercolor;
            }
            set
            {
                bordercolor = value;
            }
        }
	}
}
