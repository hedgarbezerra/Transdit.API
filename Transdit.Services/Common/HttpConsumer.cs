using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace Transdit.Services.Common
{
    [ExcludeFromCodeCoverage]
    public class HttpConsumer : IHttpConsumer
    {
        private readonly RestClient _request;
        public HttpConsumer()
        {
            _request = new RestClient();
        }

        public T Get<T>(string url)
        {
            var request = new RestRequest(url, Method.Get);

            var response = _request.ExecuteGet<T>(request);

            if (response.IsSuccessful)
                return response.Data;

            return default(T);
        }

        public T Get<T>(string url, List<KeyValuePair<string, object>> param = null)
        {
            var request = new RestRequest(url, Method.Get);

            if (param != null)
            {
                param.ForEach(p => request.AddParameter(p.Key, p.Value, ParameterType.GetOrPost));
            }

            var response = _request.ExecuteGet<T>(request);

            if (response.IsSuccessful)
                return response.Data;

            return default(T);
        }


        public async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest(url, Method.Get);

            var response = await _request.ExecuteGetAsync<T>(request, cancellationToken);

            if (response.IsSuccessful)
                return response.Data;

            return default(T);
        }

        public T Post<T>(string url, object param)
        {
            var request = new RestRequest(url, Method.Post);
            request.AddJsonBody(param);

            var response = _request.ExecutePost<T>(request);

            if (response.IsSuccessful)
                return response.Data;

            return default(T);
        }

        public T Post<T>(string url, List<KeyValuePair<string, object>> param)
        {
            var request = new RestRequest(url, Method.Post);

            param.ForEach(p => request.AddParameter(Parameter.CreateParameter(p.Key, p.Value, ParameterType.RequestBody)));

            var response = _request.ExecutePost<T>(request);

            if (response.IsSuccessful)
                return response.Data;

            return default(T);
        }

        public async Task<T> PostAsync<T>(string url, object param, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest(url, Method.Post);
            request.AddJsonBody(param);

            var response = await _request.ExecutePostAsync<T>(request, cancellationToken);

            if (response.IsSuccessful)
                return response.Data;

            return default(T);
        }
        public async Task<T> PostAsync<T>(string url, List<KeyValuePair<string, object>> param, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest(url, Method.Post);

            param.ForEach(p => request.AddParameter(Parameter.CreateParameter(p.Key, p.Value, ParameterType.RequestBody)));

            var response = await _request.ExecutePostAsync<T>(request, cancellationToken);

            if (response.IsSuccessful)
                return response.Data;

            return default(T);
        }

        public T Put<T>(string url, object param)
        {
            var request = new RestRequest(url, Method.Put);
            request.AddJsonBody(param);

            var response = _request.ExecutePut<T>(request);

            if (response.IsSuccessful)
                return response.Data;

            return default(T);
        }

        public T Put<T>(string url, List<KeyValuePair<string, object>> param)
        {
            var request = new RestRequest(url, Method.Put);

            param.ForEach(p => request.AddParameter(Parameter.CreateParameter(p.Key, p.Value, ParameterType.RequestBody)));

            var response = _request.ExecutePut<T>(request);

            if (response.IsSuccessful)
                return response.Data;

            return default(T);
        }

        public async Task<T> PutAsync<T>(string url, object param, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest(url, Method.Put);
            request.AddJsonBody(param);

            var response = await _request.ExecutePutAsync<T>(request, cancellationToken);

            if (response.IsSuccessful)
                return response.Data;

            return default(T);
        }
        public async Task<T> PutAsync<T>(string url, List<KeyValuePair<string, object>> param, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest(url, Method.Put);

            param.ForEach(p => request.AddParameter(Parameter.CreateParameter(p.Key, p.Value, ParameterType.RequestBody)));

            var response = await _request.ExecutePutAsync<T>(request, cancellationToken);

            if (response.IsSuccessful)
                return response.Data;

            return default(T);
        }

        public T Delete<T>(string url)
        {
            var request = new RestRequest(url, Method.Delete);
            var response = _request.Execute<T>(request);

            if (response.IsSuccessful)
                return response.Data;

            return default(T);
        }


        public async Task<T> DeleteAsync<T>(string url, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest(url, Method.Delete);
            var response = await _request.ExecuteAsync<T>(request, cancellationToken);

            if (response.IsSuccessful)
                return response.Data;

            return default(T);
        }
        public void AddHeaders(List<KeyValuePair<string, string>> headers)
        {
            headers.ForEach(header => this._request.AddDefaultHeader(header.Key, header.Value));
        }
        public void AddHeaders(KeyValuePair<string, string> header)
        {
            this._request.AddDefaultHeader(header.Key, header.Value);
        }
    }
}
