using XieyiES.Api.Model;
using XieyiES.Api.Model.DtoModel;

namespace XieyiES.Api.Profile
{
    public class UserWalletProfile : AutoMapper.Profile
    {
        public UserWalletProfile()
        {
            CreateMap<UserWalletUpdateDto, UserWallet>();
        }
    }
}