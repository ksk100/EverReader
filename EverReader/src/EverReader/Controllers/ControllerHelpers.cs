using EverReader.DataAccess;
using EverReader.Models;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EverReader.Controllers
{
    public static class ControllerHelpers
    {
        public static async Task<ApplicationUser> GetCurrentUserAsync(UserManager<ApplicationUser> userManager, IEverReaderDataAccess dataAccess, string currentUserId)
        {
            ApplicationUser user = await userManager.FindByIdAsync(currentUserId);
            user.EvernoteCredentials = dataAccess.EFDbEvernoteCredentials.SingleOrDefault(cred => cred.Id == user.EvernoteCredentialsId); ;
            return user;
        }
    }
}
