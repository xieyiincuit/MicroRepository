using System.ComponentModel.DataAnnotations;

namespace XieyiES.Api.Model.DtoModel
{
    public class UserUpdateOrAddDto
    {
        [Required(ErrorMessage = "userId is required")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "userName is required")]
        [StringLength(30, ErrorMessage = "userName lengths should in range[2-30]",MinimumLength = 2)]
        public string UserName { get; set; }

        public decimal Money { get; set; }
    }
}