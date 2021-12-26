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

            Task.Run(async () => {
                while (true)
                {
                    CheckAndDelete();
                    await Task.Delay(TimeSpan.FromSeconds(60));
                }
            });
        }
        public static void CheckAndDelete()
        {
            foreach (string f in Directory.GetFiles("tmp"))
            {
                try
                {
                    DateTime lastMod = new[]{ File.GetLastAccessTime(f), File.GetLastWriteTime(f) }.Max();
                    if (lastMod.AddMinutes(1).CompareTo(DateTime.Now) < 0)
                    {
                        File.Delete(f);
                    }
                }
                catch (IOException) { }
            }

            foreach (string f in Directory.GetDirectories("tmp"))
            {
                try
                {
                    DateTime lastMod = new[]{ Directory.GetLastAccessTime("tmp/" + f), File.GetLastWriteTime("tmp/" + f) }.Max();
                    if (lastMod.AddMinutes(1).CompareTo(DateTime.Now) < 0)
                    {
                        Directory.Delete(f, true);
                    }
                }
                catch (IOException) { }
            }
        }
    }
}
