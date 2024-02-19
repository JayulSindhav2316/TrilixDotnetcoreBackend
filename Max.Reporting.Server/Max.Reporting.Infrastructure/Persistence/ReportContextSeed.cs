﻿using Max.Reporting.Domain.Entities;
using Microsoft.Extensions.Logging;


namespace Max.Reporting.Infrastructure.Persistence
{
    public class ReportContextSeed
    {
        public static async Task SeedAsync(ReportContext reportContext, ILogger<ReportContextSeed> logger)
        {
            if (!reportContext.TrTemplates.Any())
            {
                reportContext.TrTemplates.AddRange(GetReportTemplate());
                await reportContext.SaveChangesAsync();
                logger.LogInformation("Seed database associated with context {DbContextName}", typeof(ReportContext).Name);
            }
            if (!reportContext.TrCategories.Any())
            {
                reportContext.TrCategories.AddRange(GetReportCategory());
                await reportContext.SaveChangesAsync();
                logger.LogInformation("Seed database associated with context {DbContextName}", typeof(ReportContext).Name);

            }
        }
        private static IEnumerable<TrTemplate> GetReportTemplate()
        {
            return new List<TrTemplate>
            {
                new TrTemplate() { Name  = "Blank Template", Definition = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Report Width=\"6.5in\" Name=\"Report1\" xmlns=\"http://schemas.telerik.com/reporting/2023/1.1\"><Items><PageHeaderSection Height=\"1in\" Name=\"pageHeaderSection1\"><Items><PictureBox Width=\"1in\" Height=\"1in\" Left=\"0in\" Top=\"0in\" Sizing=\"ScaleProportional\" Docking=\"Left\" Name=\"pictureBox1\" /><TextBox Width=\"5.5in\" Height=\"0.2in\" Left=\"1in\" Top=\"0in\" Value=\"Header1\" Docking=\"Top\" Name=\"textBox1\" /></Items></PageHeaderSection><DetailSection Height=\"2in\" Name=\"detailSection1\" /><PageFooterSection Height=\"0.5in\" Name=\"pageFooterSection1\"><Items><TextBox Width=\"6.5in\" Height=\"0.5in\" Left=\"0in\" Top=\"0in\" Value=\"Footer1\" Docking=\"Fill\" Name=\"textBox2\" /></Items></PageFooterSection></Items><PageSettings PaperKind=\"Letter\" Landscape=\"False\" ColumnCount=\"1\" ColumnSpacing=\"0in\"><Margins><MarginsU Left=\"1in\" Right=\"1in\" Top=\"1in\" Bottom=\"1in\" /></Margins></PageSettings><StyleSheet><StyleRule><Style><Padding Left=\"2pt\" Right=\"2pt\" /></Style><Selectors><TypeSelector Type=\"TextItemBase\" /><TypeSelector Type=\"HtmlTextBox\" /></Selectors></StyleRule></StyleSheet></Report>" }
            };
        }
        private static IEnumerable<TrCategory> GetReportCategory()
        {
            return new List<TrCategory>
            {
                new TrCategory() { Name  = "General", Description = "General Category" },
                new TrCategory() { Name  = "Membership", Description = "Membership Category" },
            };
        }
    }
}
