using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projekt_v2.Models.Account
{
    public class AccountLogOnMdel
    {
        [Required]
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool Anonymous { get; set; }
        public string Message { get; set; }

    }
}