using System;
using System.ComponentModel.DataAnnotations;
using API.command;
using API.Models;
using API.Services;
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
        [FromForm] string email, [FromForm] string password, [FromForm] string userName,
        [FromForm] IFormFile? profileImage )
        =>
        {
            var userFromDb = await UserManager.FindByEmailAsync(email);

            if (userFromDb is not null)
            {
                return Results.BadRequest(Response<string>.Failure("User already exist."));

            }

            if (profileImage is null)
            {
                return Results.BadRequest(Response<string>.Failure("Profile image is Required."));
            }

            var picture = await FileUpload.Upload(profileImage);
            picture = $"{context.Request.Scheme}://{context.Request.Host}/uploads/{picture}";

            var user = new AppUser
            {
                Email = email,
                FullName = fullName,
                UserName = userName,
                ProfileImage = picture
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



