using System.Threading;
using System.Threading.Tasks;

namespace BoincStatistic.Worker.Tor;

public interface ITorControlClient
{
    Task<bool> RotateIdentityAsync(CancellationToken cancellationToken);
}
