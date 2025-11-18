using AutoMapper;
using StudentTeacherManagment.Models.Domain;
using StudentTeacherManagment.Models.DTOs.Submissions;
using StudentTeacherManagment.Models.DTOs.Assignments;

namespace StudentTeacherManagment.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Assignment → DTO
            CreateMap<Assignment, AssignmentResponseDto>()
                 .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.FullName))
                 .ForMember(dest => dest.Submissions, opt => opt.MapFrom(src => src.Submissions));


            CreateMap<Assignment, AssignmentDetailsDto>()
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.FullName))
                .ForMember(dest => dest.Submissions, opt => opt.MapFrom(src => src.Submissions));

            // Submission → DTO
            CreateMap<Submission, SubmissionDto>()
    .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.FullName));


            // DTO → Assignment (for creation)
            CreateMap<CreateAssignmentDto, Assignment>();
        }
    }
}
