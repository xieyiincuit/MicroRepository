using System;

namespace XieyiES.Api.Model
{
    public class UserLogin
    {
        public string LoginId { get; set; }
        public string UserCode { get; set; }
        public string NickName { get; set; }
        public string College { get; set; }
        public long LoginInTime { get; set; }
        public long LoginOutTime { get; set; }
        public long OnLineTime { get; set; }
        public DateTime CreateTime { get; set; }
    }
}