using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace QueueReceiver.App.Helpers
{
    public static class RabbitMQHelper
    {
        public static EventingBasicConsumer CreateConsumer(IModel channel)
        {
            return new EventingBasicConsumer(channel);
        }

        public static IModel CreateChannel(IConnection connection, string queueName)
        {
            var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            return channel;
        }

        public static IConnection CreateConnection(IConfiguration configuration)
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = configuration["Queue:HostName"],
                Port = Convert.ToInt32(configuration["Queue:Port"]),
                UserName = configuration["Queue:UserName"],
                Password = configuration["Queue:Password"],
                VirtualHost = configuration["Queue:VirtualHost"]
            };

            var connection = connectionFactory.CreateConnection();

            return connection;
        }
    }
}
