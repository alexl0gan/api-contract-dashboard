using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using api_contract_dashboard.Models;
using Newtonsoft.Json;

namespace api_contract_dashboard.Services
{
    public interface IPullRequestService
    {
        Task<int?> GetOpenPrCountAsync();
        Task<int?> GetNotInStagingPrCountAsync();
        Task<int?> GetNotInProductionPrCountAsync();
        Task<List<RepoMessage>> GetAllPullRequestsAsync();
        Task<List<RepoMessage>> GetAllOpenPullRequestsAsync();

    }
    public class PullRequestService : IPullRequestService
    {
        private HttpClient client;

        private string openPrUrl = "https://api.github.com/repos/ArmMbedCloud/mbed-cloud-api-contract/pulls?base=master&per_page=100";

        private string allPrUrl = "https://api.github.com/repos/ArmMbedCloud/mbed-cloud-api-contract/pulls?base=master&state=all&per_page=100";

        private int? openPrCount;

        private int? notInStagingPrCount;

        private int? notInProductionPrCount;

        private List<RepoMessage> allPullRequests;

        private List<RepoMessage> allOpenPullRequests;

        public PullRequestService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<int?> GetOpenPrCountAsync()
        {
            if (openPrCount.HasValue)
            {
                return openPrCount;
            }

            openPrCount = (await GetAllOpenPullRequestsAsync()).Count();

            return openPrCount;
        }

        public async Task<int?> GetNotInStagingPrCountAsync()
        {
            if (notInStagingPrCount.HasValue)
            {
                return notInStagingPrCount;
            }

            notInStagingPrCount = (await GetAllPullRequestsAsync())
                .Count(d => d.labels.Any(l => l.name == PullRequestLabels.NotInStaging));

            return notInStagingPrCount;
        }

        public async Task<int?> GetNotInProductionPrCountAsync()
        {
            if (notInProductionPrCount.HasValue)
            {
                return notInProductionPrCount;
            }

            notInProductionPrCount = (await GetAllPullRequestsAsync())
                .Count(d => d.labels.Any(l => l.name == PullRequestLabels.NotInProduction) && !d.labels.Any(l => l.name == PullRequestLabels.NotInStaging));

            return notInProductionPrCount;
        }

        public async Task<List<RepoMessage>> GetAllPullRequestsAsync()
        {
            if (allPullRequests != null)
            {
                return allPullRequests;
            }

            allPullRequests = await GithubRequestAsync<List<RepoMessage>>(allPrUrl);

            return allPullRequests;
        }

        public async Task<List<RepoMessage>> GetAllOpenPullRequestsAsync()
        {
            if (allOpenPullRequests != null)
            {
                return allOpenPullRequests;
            }

            allOpenPullRequests = await GithubRequestAsync<List<RepoMessage>>(openPrUrl);

            return allOpenPullRequests;
        }

        private async Task<T> GithubRequestAsync<T>(string uri)
        {
            var request = new HttpRequestMessage()
            {
                Method = new HttpMethod("GET"),
                RequestUri = new Uri(uri),
            };
            request.Headers.Add("Authorization", "Bearer d57cb4b9a0f8f234f7142fde6b909fe134192b3c");

            var res = await client.SendAsync(request);
            var stringData = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(stringData);
        }
    }
}