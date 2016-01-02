namespace BackTesting.Model.Events
{
    using System.Collections.Generic;

    public class QueuedEventBus : IEventBus
    {
        private readonly Queue<EventBase> queue;
        private static readonly object sync = new object();

        public QueuedEventBus()
        {
            this.queue = new Queue<EventBase>();
        }

        public EventBase Get()
        {
            lock (sync)
            {
                return this.queue.Count <= 0 ? null : this.queue.Dequeue();
            }
        }

        public void Put(EventBase message)
        {
            lock (sync)
            {
                this.queue.Enqueue(message);
            }
        }
    }
}
