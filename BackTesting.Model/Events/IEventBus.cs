namespace BackTesting.Model.Events
{
    public interface IEventBus
    {
        void Put(EventBase message);
        EventBase Get();
    }
}
