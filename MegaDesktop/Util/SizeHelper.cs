using System;

namespace MegaDesktop.Util
{
    public static class SizeHelper
    {
        // taken from http://stackoverflow.com/a/4975942/453024
        public static string BytesToString(this long size)
        {
            string[] suffix = { " B", " KB", " MB", " GB", " TB", " PB", " EB" };

            if (size == 0)
                return "0" + suffix[0];

            var bytes = Math.Abs(size);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);

            return (Math.Sign(size) * num) + suffix[place];
        }
    }
}