using AutoMapper;
using StudentTeacherManagment.Models.Domain;
using StudentTeacherManagment.Models.DTOs.Assignments;
using StudentTeacherManagment.Models.DTOs.Submissions;

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

        // Submission → SubmissionDto
        CreateMap<Submission, SubmissionDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.FullName));

        // Submission → SubmissionResponseDto   ❗ ADDED
        CreateMap<Submission, SubmissionResponseDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.FullName))
            .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.FilePath))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName));

        // DTO → Assignment
        CreateMap<CreateAssignmentDto, Assignment>();
    }
}
