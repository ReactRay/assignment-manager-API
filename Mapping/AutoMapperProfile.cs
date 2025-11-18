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
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.FullName));

            CreateMap<Assignment, AssignmentDetailsDto>()
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.FullName))
                .ForMember(dest => dest.Submissions, opt => opt.MapFrom(src => src.Submissions));

            // Submission → DTO
            CreateMap<Submission, SubmissionResponseDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.FullName))
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.FilePath))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName));

            // DTO → Assignment (for creation)
            CreateMap<CreateAssignmentDto, Assignment>();
        }
    }
}
