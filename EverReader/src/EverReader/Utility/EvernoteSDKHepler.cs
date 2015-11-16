using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EverReader.Utility
{
    public static class EvernoteSDKHelper
    {
        public static DateTime ConvertEvernoteDateToDateTime(long ticks)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(ticks);
        }
    }
}
