using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Models;

public class AppUser: IdentityUser
{
    public string? FullName { get; set; }
    public string? ProfileImage { get; set; }


}
