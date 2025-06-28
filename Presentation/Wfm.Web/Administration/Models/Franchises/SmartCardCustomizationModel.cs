using FluentValidation.Attributes;
using System.ComponentModel;
using System.Web.Mvc;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Franchises
{
    public class SmartCardCustomizationModel
    {
        public string LogoStr { get; set; }
        public BoxModel Logo { get; set; }

        public string PhotoStr { get; set; }
        public BoxModel Photo { get; set; }

        public string NamePrefix { get; set; }
        public TextBoxModel FullName { get; set; }

        public string IdPrefix { get; set; }
        public TextBoxModel EmployeeId { get; set; }

        public BoxModel BarCode { get; set; }

        public TextBoxModel Note { get; set; }
        [AllowHtml]
        public string NoteText { get; set; }
    }


    public class TextBoxModel : BoxModel
    {
        [DisplayName("Text Align")]
        public int TextAlign { get; set; }
        [DisplayName("Font Family")]
        public int FontFamily { get; set; }
        [DisplayName("Font Size")]
        public int FontSize { get; set; }
        [DisplayName("Color")]
        public string Color { get; set; }
        [DisplayName("Line Height")]
        public int LineHeight { get; set; }
    }


    public class BoxModel
    {
        [DisplayName("Width")]
        public int Width { get; set; }
        [DisplayName("Height")]
        public int Height { get; set; }
        [DisplayName("Margin Top")]

        public int MarginTop { get; set; }
        [DisplayName("Margin Right")]
        public int MarginRight { get; set; }
        [DisplayName("Margin Bottom")]
        public int MarginBottom { get; set; }
        [DisplayName("Margin Left")]
        public int MarginLeft { get; set; }
    }
}
