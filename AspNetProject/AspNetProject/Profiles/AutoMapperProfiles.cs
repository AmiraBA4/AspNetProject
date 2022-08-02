using AutoMapper;
using StudentAdminPortal.API.DataModels;
using StudentAdminPortal.API.DomainModels;
using StudentAdminPortal.API.Profiles.AfterMaps;

namespace StudentAdminPortal.API.Profiles
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Student, StudentModel>()
                .ForMember(x => x.Gender, opt => opt.MapFrom(src => src.Gender))
                 .ForMember(x => x.Address, opt => opt.MapFrom(src => src.Address))
                .ReverseMap();

            CreateMap<Gender, GenderModel>()
                .ReverseMap();

            CreateMap<Address, AddressModel>()
                .ReverseMap();

            CreateMap<UpdateStudentRequestModel, Student>()
                .AfterMap<UpdateStudentRequestAfterMap>();

            CreateMap<AddStudentRequestModel,Student>()
                .AfterMap<AddStudentRequestAfterMap>();
        }
    }
}
