using System.Threading;
using System.Threading.Tasks;

namespace BoincStatistic.Worker.Scraping;

public interface IResilientFetcher
{
    Task<string> FetchAsync(string targetUrl, string logContext, CancellationToken cancellationToken);
}
