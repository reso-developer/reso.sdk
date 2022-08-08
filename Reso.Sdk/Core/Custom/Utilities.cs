using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using Reso.Sdk.Core.Attributes;

namespace Reso.Sdk.Core.Custom
{
    public static class Utilities
    {
        public static string ToSnakeCase(this string o)
        {
            return Regex.Replace(o, "(\\w)([A-Z])", "$1-$2").ToLower();
        }

        public static string SnackCaseToLower(this string o)
        {
            return o.Contains("-")
                ? string.Join(string.Empty, o.Split("-")).Trim().ToLower()
                : string.Join(string.Empty, o.Split("_")).Trim().ToLower();
        }

        public static void SetHeaderHttpClient(this HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml+json");
        }

        public static string GetQueryString(this object obj)
        {
            IEnumerable<string> source = from w in obj.GetType().GetProperties()
                where w.GetValue(obj, null) != null && !w.CustomAttributes.Any((CustomAttributeData a) =>
                    a.AttributeType == typeof(SkipAttribute))
                select w
                into s
                select s.Name.ToSnakeCase() + "=" + Uri.EscapeUriString(s.GetValue(obj, null)!.ToString());
            if (source.Any())
            {
                return string.Join("&", source.ToArray());
            }

            return string.Empty;
        }

        public static string DisplayName(this Enum value)
        {
            try
            {
                Type type = value.GetType();
                string name = Enum.GetName(type, value);
                MemberInfo memberInfo = type.GetMember(name)[0];
                object[] customAttributes = memberInfo.GetCustomAttributes(typeof(DisplayAttribute), inherit: false);
                string name2 = ((DisplayAttribute)customAttributes[0]).Name;
                if (((DisplayAttribute)customAttributes[0]).ResourceType != null)
                {
                    name2 = ((DisplayAttribute)customAttributes[0]).GetName();
                }

                return name2;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}