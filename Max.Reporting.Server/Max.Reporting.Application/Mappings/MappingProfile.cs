using AutoMapper;
using Max.Reporting.Application.Features.ReportCategory.Commands.AddCategory;
using Max.Reporting.Application.Features.ReportCategory.Commands.UpdateCategory;
using Max.Reporting.Application.Features.ReportCategory.Queries.GetReportCategory;
using Max.Reporting.Application.Features.Reports.Commands.AddReport;
using Max.Reporting.Application.Features.Reports.Commands.CloneReport;
using Max.Reporting.Application.Features.Reports.Commands.UpdateReport;
using Max.Reporting.Application.Features.Reports.Queries.GetReportList;
using Max.Reporting.Application.Features.ReportTemplate.Queries.GetTemplateList;
using Max.Reporting.Domain.Entities;

namespace Max.Reporting.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TrReport, ReportVm>().ReverseMap();
            CreateMap<TrReport, AddReportCommand>().ReverseMap();
            CreateMap<TrReport,UpdateReportCommand>().ReverseMap();
            CreateMap<TrReport, CloneReportCommand>().ReverseMap();


            CreateMap<TrTemplate, ReportTemplateVm>().ReverseMap();

            CreateMap<TrCategory,ReportCategoryVm>().ReverseMap();
            CreateMap<TrCategory,AddCategoryCommand>().ReverseMap();
            CreateMap<TrCategory,UpdateCategoryCommand>().ReverseMap();


        }
    }
}
