using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DioLive.Cache.WebUI.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
