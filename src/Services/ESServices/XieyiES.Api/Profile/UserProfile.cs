using XieyiES.Api.Model;
using XieyiES.Api.Model.DtoModel;

namespace XieyiES.Api.Profile
{
    public class UserProfile : AutoMapper.Profile
    {
        public UserProfile()
        {
            CreateMap<UserUpdateOrAddDto, User>();
        }
    }
}