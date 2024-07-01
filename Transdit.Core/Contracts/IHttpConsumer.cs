using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Contracts
{
    public interface IHttpConsumer
    {
        void AddHeaders(KeyValuePair<string, string> header);
        void AddHeaders(List<KeyValuePair<string, string>> headers);
        T Delete<T>(string url);
        Task<T> DeleteAsync<T>(string url, CancellationToken cancellationToken = default);
        T Get<T>(string url);
        T Get<T>(string url, List<KeyValuePair<string, object>> param = null);
        Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default);
        T Post<T>(string url, List<KeyValuePair<string, object>> param);
        T Post<T>(string url, object param);
        Task<T> PostAsync<T>(string url, List<KeyValuePair<string, object>> param, CancellationToken cancellationToken = default);
        Task<T> PostAsync<T>(string url, object param, CancellationToken cancellationToken = default);
        T Put<T>(string url, List<KeyValuePair<string, object>> param);
        T Put<T>(string url, object param);
        Task<T> PutAsync<T>(string url, List<KeyValuePair<string, object>> param, CancellationToken cancellationToken = default);
        Task<T> PutAsync<T>(string url, object param, CancellationToken cancellationToken = default);
    }
}
