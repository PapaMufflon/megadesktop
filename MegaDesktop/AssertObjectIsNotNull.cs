using System;

namespace MegaDesktop
{
    public static class AssertObjectIsNotNull
    {
        public static T AssertIsNotNull<T>(this T self, string parameterName) where T : class
        {
            if (self == null)
                throw new ArgumentNullException(string.IsNullOrEmpty(parameterName) ? typeof(T).ToString() : parameterName);

            return self;
        }
    }
}