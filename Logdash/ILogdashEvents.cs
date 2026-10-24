namespace Logdash;

public interface ILogdashEvents
{
    /// <summary>
    /// Tracks an event, such as an agent run or a signup, in the background.
    /// An invalid name, or a user ID that contains <c>@</c>, drops the event with a warning.
    /// </summary>
    /// <param name="name">The event name, matching <c>^[a-z][a-z0-9_]{0,63}$</c>, for example <c>agent_run</c>.</param>
    /// <param name="props">Up to 10 properties with string, number or boolean values. Never put personal data in them.</param>
    /// <param name="userId">The signed-in user's internal ID, never an email address.</param>
    void Track(string name, IReadOnlyDictionary<string, object>? props = null, string? userId = null);
}
