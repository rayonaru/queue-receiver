namespace QueueReceiver.Core
{
    public abstract class BaseHandler
    {
        public abstract string QueueName { get; }

        public abstract void Execute(string parameters);
    }
}
