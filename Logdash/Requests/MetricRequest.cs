using Logdash.Models;

namespace Logdash.Requests;

public record MetricRequest(string Name, object Value, MetricOperation Operation);