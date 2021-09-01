using AutoMapper;
using XieyiES.Api.Model;
using XieyiES.Api.Model.DtoModel;

namespace XieyiES.Api.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserUpdateOrAddDto, User>();
        }
    }
}