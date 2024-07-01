using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Utilities.Extensions
{
    public static class EnumerationExtensions
    {
        public static void ForEachNext<T>(this IList<T> collection, Action<T, T> func)
        {
            for (int i = 0; i < collection.Count - 1; i++)
                func(collection[i], collection[i + 1]);
        }

        public static void AddIdentityErrors(this ICollection<string> target, IEnumerable<IdentityError> errors)
        {
            if (target is null || errors is null)
                return;
            foreach (var description in errors.Select(err => err.Description))
            {
                target.Add(description);
            }
        }
        public static void AddValidationErrors(this ICollection<string> target, IEnumerable<ValidationFailure> errors)
        {
            if (target is null || errors is null)
                return;
            foreach (var description in errors.Select(err => err.ErrorMessage).ToList())
            {
                target.Add(description);
            }
        }
        public static string DisplayName(this Enum value)
        {
            var name = (DescriptionAttribute[])value
                .GetType()
                .GetTypeInfo()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);

            return name.Length > 0 ? name[0].Description : value.ToString();
        }
    }
}
