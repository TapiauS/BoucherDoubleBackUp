using Boucher_DoubleModel.Models.Entitys;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace Boucher_Double_Back_End.Controllers
{
    public class SessionController : ControllerBase
    {

        public static ConcurrentDictionary<string, User> ConnectedUser { get; set; } = new();

        public static void AddUserWithExpiration(string key, User user, TimeSpan expirationTime)
        {
            ConnectedUser.TryAdd(key, user);

            // Start a timer to remove the key after the expiration time
            Timer timer = new Timer(RemoveUser, key, expirationTime, Timeout.InfiniteTimeSpan);
        }

        protected static void RemoveUser(object state)
        {
            string keyToRemove = (string)state;
            ConnectedUser.TryRemove(keyToRemove, out _);
        }
    }
}
