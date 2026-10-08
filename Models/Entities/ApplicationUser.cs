using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace MyId.Domain.Entities
{
    public class ApplicationUser: IdentityUser
    {
        
        public string FullName { get; set; } = string.Empty;
       
    }
}
