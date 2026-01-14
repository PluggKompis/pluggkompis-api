using Application.Common.Interfaces;
using Application.Subjects.Dtos;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.Subjects;
using MediatR;

namespace Application.Subjects.GetSubjects
{
    public class GetSubjectsQueryHandler : IRequestHandler<GetSubjectsQuery, OperationResult<List<SubjectDto>>>
    {
        private readonly IGenericRepository<Subject> _subjectRepository;
        private readonly IMapper _mapper;

        public GetSubjectsQueryHandler(IGenericRepository<Subject> subjectRepository, IMapper mapper)
        {
            _subjectRepository = subjectRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<SubjectDto>>> Handle(GetSubjectsQuery request, CancellationToken cancellationToken)
        {
            var subjects = await _subjectRepository.GetAllAsync();

            var result = _mapper.Map<List<SubjectDto>>(subjects);

            return OperationResult<List<SubjectDto>>.Success(result);
        }
    }
}
