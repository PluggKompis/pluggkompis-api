using Application.Coordinator.Dtos;
using Application.Coordinator.Queries.GetCoordinatorDashboard.Models;
using AutoMapper;

namespace Application.Coordinator.Mapping
{
    public class CoordinatorDashboardProfile : Profile
    {
        public CoordinatorDashboardProfile()
        {
            CreateMap<CoordinatorDashboardModel, CoordinatorDashboardDto>();

            CreateMap<SubjectCoverageModel, SubjectCoverageDto>();
            CreateMap<UpcomingShiftModel, UpcomingShiftDto>();
            CreateMap<VolunteerUtilizationModel, VolunteerUtilizationDto>();
        }
    }
}
