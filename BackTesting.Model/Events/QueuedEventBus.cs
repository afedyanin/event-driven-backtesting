namespace BackTesting.Model.Events
{
    using System.Collections.Generic;

    public class QueuedEventBus : IEventBus
    {
        private readonly Queue<Event> queue;
        private static readonly object sync = new object();

        public QueuedEventBus()
        {
            this.queue = new Queue<Event>();
        }

        public Event Get()
        {
            lock (sync)
            {
                return this.queue.Count <= 0 ? null : this.queue.Dequeue();
            }
        }

        public void Put(Event message)
        {
            lock (sync)
            {
                this.queue.Enqueue(message);
            }
        }
    }
}
