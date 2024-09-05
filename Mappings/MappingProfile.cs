using AutoMapper;
using ClinicManagementAPI.DTOs;
using ClinicManagementAPI.Models;

namespace ClinicManagementAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping from Doctor to DoctorListDto
            CreateMap<Doctor, DoctorListDto>()
                .ForMember(dest => dest.CabinetNumber, opt => opt.MapFrom(src => src.Cabinet.Number))
                .ForMember(dest => dest.SpecializationName, opt => opt.MapFrom(src => src.Specialization.Name))
                .ForMember(dest => dest.EstateNumber, opt => opt.MapFrom(src => src.Estate != null ? src.Estate.Number : string.Empty));

            // Mapping from Doctor to DoctorEditDto
            CreateMap<Doctor, DoctorEditDto>();

            // Mapping from DoctorEditDto to Doctor (for creating/updating Doctor)
            CreateMap<DoctorEditDto, Doctor>();
        }
    }
}
