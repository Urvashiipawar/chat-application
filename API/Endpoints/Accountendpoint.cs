using System;
using API.command;
using API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class AccountEndpoint
{
    public static RouteGroupBuilder MapAccountEndpoint(this WebApplication app)
    {
        var group = app.MapGroup("/api/account").WithTags("account");

        group.MapPost("/register", async (HttpContext context,
        UserManager<AppUser> UserManager, [FromForm] string fullName,
        [FromForm] string email, [FromForm] string password, [FromForm] string userName)
        =>
        {
            var userFromDb = await UserManager.FindByEmailAsync(email);

            if (userFromDb is not null)
            {
                return Results.BadRequest(Response<string>.Failure("User already exist."));

            }

            var user = new AppUser
            {
                Email = email,
                FullName = fullName,
                UserName = userName
            };

            var result = await UserManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return Results.BadRequest(Response<string>.Failure(result.Errors.Select(x => x.Description).FirstOrDefault()!));
            }

            return Results.Ok(Response<string>.Success("", "User created sucessfully."));

        }).DisableAntiforgery();

        return group;

    }
}



