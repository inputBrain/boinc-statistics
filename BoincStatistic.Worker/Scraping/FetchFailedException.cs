using System;

namespace BoincStatistic.Worker.Scraping;

public class FetchFailedException : Exception
{
    public FetchFailedException(string reason) : base(reason) { }
}
