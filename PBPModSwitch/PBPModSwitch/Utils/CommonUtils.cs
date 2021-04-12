using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Memory;

namespace PBPModSwitch.Utils
{
    public static class CommonUtils
    {
        public static string StringToAoB(string str, bool zeroTerminated = false)
        {
            return Mem.ByteArrayToString(System.Text.UTF8Encoding.ASCII.GetBytes(str + (zeroTerminated ? "\0" : ""))).ToUpper().Trim();
        }

        public static IEnumerable<string> FilterFiles(string path, params string[] exts)
        {
            return
                exts.Select(x => "*." + x)
                .SelectMany(x =>
                    Directory.EnumerateFiles(path, x)
                    );
        }

        public static string ComputeHash(this FileInfo file)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = File.OpenRead(file.FullName))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "");
                }
            }
        }
    }
}
