using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Net;

namespace DailyUnoThesis.Models;
public class AuthHandler : DelegatingHandler
{
    private int _retryCount = 0;
    private static readonly SemaphoreSlim _refreshLock = new(1, 1);

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized && _retryCount == 0)
        {
            _retryCount++;
            await _refreshLock.WaitAsync(cancellationToken);
            try
            {
                var refreshed = await APIHost.GetInstance().RefreshToken();
                if (refreshed)
                {
                    response.Dispose();
                    var retry = await CloneRequest(request);
                    retry.Headers.Authorization = new AuthenticationHeaderValue(
                        "Bearer", AuthorizedUser.GetInstance().AccessToken);
                    response = await base.SendAsync(retry, cancellationToken);
                }
            }
            finally { _refreshLock.Release(); }
        }

        return response;
    }
    private static async Task<HttpRequestMessage> CloneRequest(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);
        if (request.Content != null)
        {
            var body = await request.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(body);
            foreach (var header in request.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        foreach (var header in request.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        return clone;
    }
}
