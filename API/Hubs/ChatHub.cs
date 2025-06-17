using System;
using System.Collections.Concurrent;
using API.data;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

[Authorize]
public class ChatHub(UserManager<AppUser> userManager, AppDbContext context) : Hub
{
    public static readonly ConcurrentDictionary<string, OnlineUserDto>
    onlineUsers = new();

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var recevierId = httpContext?.Request.Query["SenderId"].ToString();
        var userName = Context.User!.Identity!.Name!;
        var currentUser = await userManager.FindByNameAsync(userName);
        var connectionId = Context.ConnectionId;

        if (onlineUsers.ContainsKey(userName))
        {
            onlineUsers[userName].ConnectionId = connectionId;
        }
        else
        {
            var user = new OnlineUserDto
            {
                ConnectionId = connectionId,
                UserName = userName,
                ProfilePicture = currentUser!.ProfileImage,
                FullName = currentUser!.FullName
            };

            onlineUsers.TryAdd(userName, user);

            await Clients.AllExcept(connectionId).SendAsync("Notify", currentUser);
        }

        await Clients.All.SendAsync("OnlineUsers", await GetAllUsers());
    }

    private async Task<IEnumerable<OnlineUserDto>> GetAllUsers()
    {
        var username = Context.User!.getUserName();
    }

}
