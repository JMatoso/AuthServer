using jwt_identity_api.Data;
using jwt_identity_api.Models.VModels;
using AutoMapper;

namespace jwt_identity_api.MappingProfiles
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<ApplicationUser, UserVM>();
        }
    }
}