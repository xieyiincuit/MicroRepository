using System;
using System.ComponentModel.DataAnnotations;
using XieyiESLibrary.Entity;

namespace XieyiES.Api.Model
{
    public class UserWallet : ESBaseEntity
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        public DateTime CreateTime { get; set; }

        public decimal Money { get; set; }
    }

    public class Manager
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        public DateTime CreateTime { get; set; }

        public decimal Money { get; set; }
    }
}