using System;

namespace XieyiES.Api.Model.DtoModel
{
    public class UserWalletUpdateDto
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public DateTime CreateTime { get; set; }

        public decimal Money { get; set; }
    }
}