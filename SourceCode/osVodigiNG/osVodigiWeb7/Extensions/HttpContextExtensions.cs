using System;
using Microsoft.AspNetCore.Http;
using TB.ComponentModel;

namespace osVodigiWeb7.Extensions
{
    public static class HttpContextExtensions
    {
        public static T GetContextItem<T>(this HttpContext httpContext, string key)
        {
            if (httpContext == null) return default(T);
            if (httpContext.Items[key] == null) return default(T);
            var val = httpContext.Items[key].As<T>();
            if (val.HasValue) return val.Value;
            return default(T);
        }
    }
}
