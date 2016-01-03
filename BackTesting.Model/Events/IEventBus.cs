namespace BackTesting.Model.Events
{
    public interface IEventBus
    {
        void Put(Event message);
        Event Get();
    }
}
