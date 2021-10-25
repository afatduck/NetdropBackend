using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Netdrop
{
    public class ClearTmp
    { 
        public static void StartInterval()
        {
            var token = new CancellationTokenSource();
            Task.Run(async () => {
                while (!token.IsCancellationRequested)
                {
                    CheckAndDelete();
                    await Task.Delay(TimeSpan.FromSeconds(60 * 60), token.Token);
                }
            }, token.Token);
        }
        public static void CheckAndDelete()
        {
            foreach (var f in Directory.GetFiles("tmp"))
            {
                try
                {
                    DateTime lastMod = File.GetLastAccessTime(f);
                    if (lastMod.AddHours(1).CompareTo(DateTime.Now) < 0)
                    {
                        File.Delete(f);
                    }
                }
                catch (IOException) { }
            }
        }
    }
}
