using System;
using System.Net.Http;

public static class RequestFactory
{
    public static HttpRequestMessage OpenPrCountRequest()
    {
        return CreateGithubRequest("https://api.github.com/repos/ArmMbedCloud/mbed-cloud-api-contract/pulls?base=master&per_page=100");
    }

    public static HttpRequestMessage AllPrCountRequest()
    {
        return CreateGithubRequest("https://api.github.com/repos/ArmMbedCloud/mbed-cloud-api-contract/pulls?base=master&state=all&per_page=100");
    }

    private static HttpRequestMessage CreateGithubRequest(string uri)
    {
        var request = new HttpRequestMessage()
        {
            Method = new HttpMethod("GET"),
            RequestUri = new Uri(uri),
        };
        request.Headers.Add("Authorization", "Bearer 19b8fe137472b1d7488a661ab91b9691e97c5889");

        return request;
    }
}