using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using Reso.Sdk.Core.Attributes;
using Reso.Sdk.Core.ViewModels;

namespace Reso.Sdk.Core.Utilities
{
    public static class LinQUtils
    {
        public static IQueryable<TEntity> DynamicFilter<TEntity>(this IQueryable<TEntity> source, TEntity entity)
        {
            PropertyInfo[] properties = entity.GetType().GetProperties();
            PropertyInfo[] array = properties;
            foreach (PropertyInfo propertyInfo in array)
            {
                if (entity.GetType().GetProperty(propertyInfo.Name) == null)
                {
                    continue;
                }

                object value = entity.GetType().GetProperty(propertyInfo.Name)!.GetValue(entity, null);
                if (value == null || propertyInfo.CustomAttributes.Any((CustomAttributeData a) =>
                        a.AttributeType == typeof(SkipAttribute)))
                {
                    continue;
                }

                if (propertyInfo.PropertyType == typeof(DateTime))
                {
                    DateTime dateTime = (DateTime)value;
                    source = source.Where(propertyInfo.Name + " >= @0 && " + propertyInfo.Name + " < @1", dateTime.Date,
                        dateTime.Date.AddDays(1.0));
                }
                else if (propertyInfo.CustomAttributes.Any((CustomAttributeData a) =>
                             a.AttributeType == typeof(ContainAttribute)))
                {
                    IList list = (IList)value;
                    source = source.Where(propertyInfo.Name + ".Any(a=> @0.Contains(a))", list);
                }
                else if (propertyInfo.CustomAttributes.Any((CustomAttributeData a) =>
                             a.AttributeType == typeof(MultiAttribute)))
                {
                    IList list2 = (IList)value;
                    string text = propertyInfo.CustomAttributes
                        .FirstOrDefault((CustomAttributeData a) => a.AttributeType == typeof(MultiAttribute))
                        .ConstructorArguments.FirstOrDefault().Value!.ToString();
                    source = source.Where(" @0.Contains(" + text + ")", list2);
                }
                else if (propertyInfo.CustomAttributes.Any((CustomAttributeData a) =>
                             a.AttributeType == typeof(TimeRangeAttribute)))
                {
                    TimeRange timeRange = (TimeRange)value;
                    if (timeRange.StartTime.HasValue && timeRange.EndTime.HasValue)
                    {
                        string text2 = propertyInfo.CustomAttributes
                            .FirstOrDefault((CustomAttributeData a) => a.AttributeType == typeof(TimeRangeAttribute))
                            .ConstructorArguments.FirstOrDefault().Value!.ToString();
                        if (timeRange.StartTime == timeRange.EndTime)
                        {
                            timeRange.EndTime = timeRange.StartTime.Value.AddDays(1.0).Date;
                        }

                        source = source.Where(text2 + " >= @0 && " + text2 + " <= @1", timeRange.StartTime.Value,
                            timeRange.EndTime.Value);
                    }
                }
                else if (propertyInfo.CustomAttributes.Any((CustomAttributeData a) =>
                             a.AttributeType == typeof(MapFieldAttribute)))
                {
                    PropertyInfo[] properties2 = propertyInfo.PropertyType.GetProperties();
                    object value2 = propertyInfo.CustomAttributes
                        .FirstOrDefault((CustomAttributeData a) => a.AttributeType == typeof(MapFieldAttribute))
                        .NamedArguments.FirstOrDefault().TypedValue.Value;
                    source = source.Where($"{value2}=\"{((string)value).Trim()}\"");
                }
                else if (propertyInfo.CustomAttributes.Any((CustomAttributeData a) =>
                             a.AttributeType == typeof(ChildAttribute)))
                {
                    PropertyInfo[] properties3 = propertyInfo.PropertyType.GetProperties();
                    PropertyInfo[] array2 = properties3;
                    foreach (PropertyInfo propertyInfo2 in array2)
                    {
                        object value3 = value.GetType().GetProperty(propertyInfo2.Name)!.GetValue(value, null);
                        if (value3 != null && propertyInfo2.PropertyType.Name.ToLower() == "string")
                        {
                            source = source.Where($"{propertyInfo.Name}.{propertyInfo2.Name} = \"{value3}\"");
                        }
                    }
                }
                else if (propertyInfo.CustomAttributes.Any((CustomAttributeData a) =>
                             a.AttributeType == typeof(ExcludeAttribute)))
                {
                    PropertyInfo[] properties4 = propertyInfo.PropertyType.GetProperties();
                    object value4 = propertyInfo.CustomAttributes
                        .FirstOrDefault((CustomAttributeData a) => a.AttributeType == typeof(ExcludeAttribute))
                        .NamedArguments.FirstOrDefault().TypedValue.Value;
                    IEnumerable<int?> enumerable = ((List<int>)value).Cast<int?>();
                    source = source.Where($"!@0.Contains(it.{value4})", enumerable);
                }
                else
                {
                    source =
                        ((!propertyInfo.CustomAttributes.Any((CustomAttributeData a) =>
                            a.AttributeType == typeof(StringAttribute)))
                            ? ((!(propertyInfo.PropertyType == typeof(string)))
                                ? source.Where($"{propertyInfo.Name} = {value}")
                                : source.Where(propertyInfo.Name + " = \"" + ((string)value).Trim() + "\""))
                            : source.Where(propertyInfo.Name + ".ToLower().Contains(@0)", value.ToString()!.ToLower()));
                }
            }

            return source;
        }


        /// <summary>
        /// Sort list entities by SortDirection and SortBy custom attribute
        /// </summary>
        /// <param name="source"></param>
        /// <param name="entity"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// 
        /// <example>
        /// SortDirection: asc|desc
        /// SortProperty is property of entity that you want to be sorted.
        /// services
        /// .AddMvc()
        /// .AddNewtonsoftJson(opts => opts.SerializerSettings.Converters.Add(new StringEnumConverter()));
        /// </example>
        /// <returns></returns>
        public static IQueryable<TEntity> DynamicSort<TEntity>(this IQueryable<TEntity> source, TEntity entity)
        {
            if (entity.GetType().GetProperties()
                    .Any(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(SortDirectionAttribute))) &&
                entity.GetType().GetProperties()
                    .Any(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(SortPropertyAttribute))))
            {
                var sortDirection = entity.GetType().GetProperties().SingleOrDefault(x =>
                        x.CustomAttributes.Any(a => a.AttributeType == typeof(SortDirectionAttribute)))?
                    .GetValue(entity, null);
                var sortBy = entity.GetType().GetProperties().SingleOrDefault(x =>
                        x.CustomAttributes.Any(a => a.AttributeType == typeof(SortPropertyAttribute)))?
                    .GetValue(entity, null);

                if (sortDirection != null && sortBy != null)
                {
                    if ((string)sortDirection == "asc")
                    {
                        source = source.OrderBy((string)sortBy);
                    }
                    else if ((string)sortDirection == "desc")
                    {
                        source = source.OrderBy((string)sortBy + " descending");
                    }
                }
            }

            return source;
        }

        public static (int, IQueryable<TResult>) PagingIQueryable<TResult>(this IQueryable<TResult> source, int page,
            int size, int limitPaging, int defaultPaging)
        {
            if (size > limitPaging)
            {
                size = limitPaging;
            }

            if (size < 1)
            {
                size = defaultPaging;
            }

            if (page < 1)
            {
                page = 1;
            }

            int item = source.Count();
            IQueryable<TResult> item2 = source.Skip((page - 1) * size).Take(size);
            return (item, item2);
        }

        public static string ToDynamicSelector<TEntity>(this string[] selectorArray)
        {
            List<string> selectors = (from a in selectorArray
                where !string.IsNullOrEmpty(a)
                select a
                into s
                select s.SnackCaseToLower()).ToList();
            string[] source = (from s in typeof(TEntity).GetProperties()
                select s.Name.SnackCaseToLower()).ToArray();
            source = source.Where((string w) => selectors.Contains(w)).ToArray();
            return "new {" + string.Join(',', source) + "}";
        }

        public static string ToDynamicSelector(this string[] selectorArray)
        {
            return "new {" + string.Join(',', selectorArray) + "}";
        }

        public static string SnackCaseToLower(this string o)
        {
            return o.Contains("-")
                ? string.Join(string.Empty, o.Split("-")).Trim().ToLower()
                : string.Join(string.Empty, o.Split("_")).Trim().ToLower();
        }
    }
}