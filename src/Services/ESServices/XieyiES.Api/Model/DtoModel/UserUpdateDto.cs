using System;
using System.ComponentModel.DataAnnotations;

namespace XieyiES.Api.Model.DtoModel
{
    public class UserUpdateOrAddDto
    {
        [Required(ErrorMessage = "需填写UserId")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "需填写UserName")]
        public string UserName { get; set; }

        public DateTimeOffset CreateTime { get; set; }

        public decimal Money { get; set; }
    }
}