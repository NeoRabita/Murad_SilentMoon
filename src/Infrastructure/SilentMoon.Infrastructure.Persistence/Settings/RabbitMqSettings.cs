namespace SilentMoon.Infrastructure.Persistence.Settings
{
    public class RabbitMqSettings
    {
        public string Host { get; set; }
        public int Port { get; set; } 
        public string UserName { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
    }
}
