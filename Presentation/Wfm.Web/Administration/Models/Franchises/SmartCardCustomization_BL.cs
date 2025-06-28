using System;
using System.Text;
using Wfm.Core;
using Wfm.Core.Domain.Franchises;
using Wfm.Services.Franchises;


namespace Wfm.Admin.Models.Franchises
{
    public class SmartCardCustomization_BL
    {
        #region basic parameters

        private readonly int cardWidth = 648;
        private readonly int cardHeight = 1016;
        private readonly decimal screenScale = 0.5m;
        private readonly decimal printScale = CommonHelper.RoundDown(96m / 303m, 4);    // screen DPI by print DPI
        private readonly int screenBorder = 3;
        private readonly int printBorder = 1;

        #endregion


        public void SaveSmartCardSetting(IFranchiseService _franchiseService, IFranchiseSettingService _franchiseSettingService, SmartCardCustomizationModel model, Guid vendorGuid)
        {
            var franchise = _franchiseService.GetFranchiseByGuid(vendorGuid);

            _SaveSmartCardStyleSetting(_franchiseSettingService, model, franchise.Id);

            _SaveSmartCardOtherSetting(_franchiseSettingService, model, franchise.Id);
        }


        private void _SaveSmartCardStyleSetting(IFranchiseSettingService _franchiseSettingService, SmartCardCustomizationModel model, int franchiseId)
        {
            var style = new StringBuilder();

            _SaveSmartCardStyleForScreen(style, model);

            _SaveSmartCardStyleForPrint(style, model);

            var styleSetting = _franchiseSettingService.GetSettingByKey(franchiseId, "SmartCardSettings.Style");
            if (styleSetting == null)
            {
                styleSetting = new FranchiseSetting()
                {
                    FranchiseId = franchiseId,
                    Name = "SmartCardSettings.Style",
                    Value = style.ToString(),
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow
                };

                _franchiseSettingService.InsertSetting(styleSetting);
            }
            else
            {
                styleSetting.Value = style.ToString();
                styleSetting.UpdatedOnUtc = DateTime.UtcNow;

                _franchiseSettingService.UpdateSetting(styleSetting);
            }
        }


        private void _SaveSmartCardStyleForScreen(StringBuilder style, SmartCardCustomizationModel model)
        {
            style.AppendLine(".smartcard .side-container {");
            style.AppendLine("display: inline-block;");
            style.AppendLine("vertical-align: middle;");
            style.AppendLine("margin-left: 30px;");
            style.AppendLine("}");

            style.AppendLine("#front-container {");
            style.AppendLine(String.Format("width: calc({0}px * {1});", cardWidth, screenScale));
            style.AppendLine(String.Format("height: calc({0}px * {1});", cardHeight, screenScale));
            style.AppendLine("}");

            style.AppendLine("#back-container {");
            style.AppendLine(String.Format("width: calc({0}px * {1});", cardHeight, screenScale));
            style.AppendLine(String.Format("height: calc({0}px * {1});", cardWidth, screenScale));
            style.AppendLine("}");

            style.AppendLine(".smartcard .side {");
            style.AppendLine(String.Format("border: {0}px solid darkblue;", screenBorder));
            style.AppendLine("border-radius: 15px;");
            style.AppendLine(String.Format("transform: scale({0});", screenScale));
            style.AppendLine("transform-origin: 0 0;");
            style.AppendLine("}");

            style.AppendLine(".smartcard .row { display: block; }");

            style.AppendLine("#front {");
            style.AppendLine(String.Format("width: calc({0}px - {1}px);", cardWidth, screenBorder * 2));
            style.AppendLine(String.Format("height: calc({0}px - {1}px);", cardHeight, screenBorder * 2));
            style.AppendLine("}");

            style.AppendLine("#back {");
            style.AppendLine(String.Format("width: calc({0}px - {1}px);", cardHeight, screenBorder * 2));
            style.AppendLine(String.Format("height: calc({0}px - {1}px);", cardWidth, screenBorder * 2));
            style.AppendLine("}");

            style.AppendLine("#logo {");
            style.AppendLine(String.Format("width: {0}px;", model.Logo.Width));
            style.AppendLine(String.Format("height: {0}px;", model.Logo.Height));
            style.AppendLine(String.Format("margin-left: {0}px;", model.Logo.MarginLeft));
            style.AppendLine(String.Format("margin-top: {0}px;", model.Logo.MarginTop));
            style.AppendLine("}");

            style.AppendLine("#photo {");
            style.AppendLine(String.Format("width: {0}px;", model.Photo.Width));
            style.AppendLine(String.Format("height: {0}px;", model.Photo.Height));
            style.AppendLine(String.Format("margin-left: {0}px;", model.Photo.MarginLeft));
            style.AppendLine(String.Format("margin-top: {0}px;", model.Photo.MarginTop));
            style.AppendLine("}");

            style.AppendLine("img {");
            style.AppendLine("max-width: 100%;");
            style.AppendLine("max-height: 100%;");
            style.AppendLine("}");

            style.AppendLine("#fullname {");
            style.AppendLine(String.Format("margin-left: {0}px;", model.FullName.MarginLeft));
            style.AppendLine(String.Format("margin-top: {0}px;", model.FullName.MarginTop));
            style.AppendLine("text-align: left;");
            style.AppendLine("font-family: Arial, Helvetica, sans-serif;");
            style.AppendLine(String.Format("font-size: {0}px;", model.FullName.FontSize));
            style.AppendLine(String.Format("color: {0};", model.FullName.Color));
            style.AppendLine("}");

            style.AppendLine("#employeeid {");
            style.AppendLine(String.Format("margin-left: {0}px;", model.EmployeeId.MarginLeft));
            style.AppendLine(String.Format("margin-top: {0}px;", model.EmployeeId.MarginTop));
            style.AppendLine("text-align: left;");
            style.AppendLine("font-family: Arial, Helvetica, sans-serif;");
            style.AppendLine(String.Format("font-size: {0}px;", model.EmployeeId.FontSize));
            style.AppendLine(String.Format("color: {0};", model.EmployeeId.Color));
            style.AppendLine("}");

            style.AppendLine("#barcode {");
            style.AppendLine("float: left;");
            style.AppendLine(String.Format("width: {0}px;", model.BarCode.Width));
            style.AppendLine(String.Format("height: {0}px;", model.BarCode.Width));
            style.AppendLine(String.Format("margin: {0}px;", model.BarCode.MarginLeft));
            style.AppendLine("}");

            style.AppendLine("#note {");
            style.AppendLine(String.Format("margin-left: {0}px;", model.Note.MarginLeft));
            style.AppendLine(String.Format("margin-top: {0}px;", model.Note.MarginTop));
            style.AppendLine(String.Format("margin-right: {0}px;", model.Note.MarginRight));
            style.AppendLine(String.Format("margin-bottom: {0}px;", model.Note.MarginBottom));
            style.AppendLine("text-align: left;");
            style.AppendLine("font-family: Arial, Helvetica, sans-serif;");
            style.AppendLine(String.Format("font-size: {0}px;", model.Note.FontSize));
            style.AppendLine(String.Format("line-height: {0}px;", model.Note.LineHeight));
            style.AppendLine(String.Format("color: {0};", model.Note.Color));
            style.AppendLine("}");
        }


        private void _SaveSmartCardStyleForPrint(StringBuilder style, SmartCardCustomizationModel model)
        {
            style.AppendLine("@media print {");

            style.AppendLine("@page { margin: 0; }");
            style.AppendLine("body { margin: 0; }");

            style.AppendLine(".smartcard .side-container {");
            style.AppendLine("display: block;");
            style.AppendLine("margin: 0;");
            style.AppendLine("}");

            style.AppendLine("#front-container {");
            style.AppendLine(String.Format("width: calc({0}px * {1});", cardWidth, printScale, printBorder));
            style.AppendLine(String.Format("height: calc({0}px * {1});", cardHeight, printScale, printBorder));
            style.AppendLine("}");

            style.AppendLine("#back-container {");
            style.AppendLine(String.Format("width: calc({0}px * {1});", cardWidth, printScale, printBorder));
            style.AppendLine(String.Format("height: calc({0}px * {1});", cardHeight, printScale, printBorder));
            style.AppendLine(" page-break-before: always;");
            style.AppendLine("}");

            style.AppendLine(".smartcard .side { border: 1px solid white; }");

            style.AppendLine("#front {");
            style.AppendLine(String.Format("width: calc({0}px * {1} - {2}px);", cardWidth, printScale, printBorder * 2));
            style.AppendLine(String.Format("height: calc({0}px * {1} - {2}px);", cardHeight, printScale, printBorder * 2));
            style.AppendLine(String.Format("transform: scale(1);"));
            style.AppendLine("}");

            style.AppendLine("#back {");
            style.AppendLine(String.Format("width: calc({0}px * {1} - {2}px);", cardHeight, printScale, printBorder * 2));
            style.AppendLine(String.Format("height: calc({0}px * {1} - {2}px);", cardWidth, printScale, printBorder * 2));
            style.AppendLine(String.Format("transform: translateX({0}px) scale(1) rotate(90deg);", CommonHelper.RoundDown(cardWidth * printScale, 2)));
            style.AppendLine("}");

            style.AppendLine("#logo {");
            style.AppendLine(String.Format("width: calc({0}px * {1});", model.Logo.Width, printScale));
            style.AppendLine(String.Format("height: calc({0}px * {1});", model.Logo.Height, printScale));
            style.AppendLine(String.Format("margin-left: calc({0}px * {1});", model.Logo.MarginLeft, printScale));
            style.AppendLine(String.Format("margin-top: calc({0}px * {1});", model.Logo.MarginTop, printScale));
            style.AppendLine("}");

            style.AppendLine("#photo {");
            style.AppendLine(String.Format("width: calc({0}px * {1});", model.Photo.Width, printScale));
            style.AppendLine(String.Format("height: calc({0}px * {1});", model.Photo.Height, printScale));
            style.AppendLine(String.Format("margin-left: calc({0}px * {1});", model.Photo.MarginLeft, printScale));
            style.AppendLine(String.Format("margin-top: calc({0}px * {1});", model.Photo.MarginTop, printScale));
            style.AppendLine("}");

            style.AppendLine("#fullname {");
            style.AppendLine(String.Format("margin-left: calc({0}px * {1});", model.FullName.MarginLeft, printScale));
            style.AppendLine(String.Format("margin-top: calc({0}px * {1});", model.FullName.MarginTop, printScale));
            style.AppendLine(String.Format("font-size: calc({0}px * {1});", model.FullName.FontSize, printScale));
            style.AppendLine("}");

            style.AppendLine("#employeeid {");
            style.AppendLine(String.Format("margin-left: calc({0}px * {1});", model.EmployeeId.MarginLeft, printScale));
            style.AppendLine(String.Format("margin-top: calc({0}px * {1});", model.EmployeeId.MarginTop, printScale));
            style.AppendLine(String.Format("font-size: calc({0}px * {1});", model.EmployeeId.FontSize, printScale));
            style.AppendLine("}");

            style.AppendLine("#barcode {");
            style.AppendLine(String.Format("margin: calc({0}px * {1});", model.BarCode.MarginLeft, printScale));
            style.AppendLine(String.Format("width: calc({0}px * {1});", model.BarCode.Width, printScale));
            style.AppendLine(String.Format("height: calc({0}px * {1});", model.BarCode.Width, printScale));
            style.AppendLine("}");

            style.AppendLine("#note {");
            style.AppendLine(String.Format("margin-left: calc({0}px * {1});", model.Note.MarginLeft, printScale));
            style.AppendLine(String.Format("margin-top: calc({0}px * {1});", model.Note.MarginTop, printScale));
            style.AppendLine(String.Format("margin-right: calc({0}px * {1});", model.Note.MarginRight, printScale));
            style.AppendLine(String.Format("margin-bottom: calc({0}px * {1});", model.Note.MarginBottom, printScale));
            style.AppendLine(String.Format("font-size: calc({0}px * {1});", model.Note.FontSize, printScale));
            style.AppendLine(String.Format("line-height: calc({0}px * {1});", model.Note.LineHeight, printScale));
            style.AppendLine("}");

            style.AppendLine("}");
        }


        private void _SaveSmartCardOtherSetting(IFranchiseSettingService _franchiseSettingService, SmartCardCustomizationModel model, int franchiseId)
        {
            if (!String.IsNullOrWhiteSpace(model.NamePrefix))
                _franchiseSettingService.InsertOrUpdateSetting(franchiseId, "SmartCardSettings.NamePrefix", model.NamePrefix);

            if (!String.IsNullOrWhiteSpace(model.IdPrefix))
                _franchiseSettingService.InsertOrUpdateSetting(franchiseId, "SmartCardSettings.IdPrefix", model.IdPrefix);

            if (!String.IsNullOrWhiteSpace(model.NoteText))
                _franchiseSettingService.InsertOrUpdateSetting(franchiseId, "SmartCardSettings.NoteText", model.NoteText);
        }
    
    }
}
