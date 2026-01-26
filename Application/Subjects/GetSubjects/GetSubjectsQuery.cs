using Application.Subjects.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Subjects.GetSubjects
{
    public class GetSubjectsQuery : IRequest<OperationResult<List<SubjectDto>>>
    {
    }
}
