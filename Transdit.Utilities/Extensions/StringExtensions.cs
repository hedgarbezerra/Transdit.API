using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Transdit.Utilities.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Troca os caracteres de uma string e um ponto à outro com um caractere específico
        /// </summary>
        /// <param name="value">string desejada</param>
        /// <param name="from">indice inicio</param>
        /// <param name="to">indice fim</param>
        /// <param name="substitution">charactere de substituição</param>
        /// <returns></returns>
        public static string Mask(this string value, int from, int to, char substitution)
        {
            if (value.Length < to || value.Length < from || from < 0)
                return value;

            return string.Create(value.Length, value, (span, str) =>
            {
                value.AsSpan().CopyTo(span);
                span[from..to].Fill(substitution);
            });
        }

        public static string HandleRequestEncodedParam(this string initialValue)
        {
            string decodedValue = HttpUtility.UrlDecode(initialValue);
            var cleanValue = decodedValue.Contains(' ') 
                ? decodedValue.Replace(' ', '+') 
                : decodedValue;

            return cleanValue;
        }
    }
}
