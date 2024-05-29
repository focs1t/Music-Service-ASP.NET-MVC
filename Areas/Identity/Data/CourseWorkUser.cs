using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CourseWork.Areas.Identity.Data;

// Add profile data for application users by adding properties to the CourseWorkUser class
public class CourseWorkUser : IdentityUser
{
    public string Name { get; set; }
}

