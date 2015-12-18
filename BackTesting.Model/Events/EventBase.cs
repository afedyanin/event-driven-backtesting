namespace BackTesting.Model.Events
{
    /// <summary>
    /// Event is base class providing an interface for all subsequent 
    /// (inherited) events, that will trigger further events in the
    ///  trading infrastructure.
    /// </summary>
    public abstract class EventBase
    {
        public abstract EventType EventType { get; }
    }
}
