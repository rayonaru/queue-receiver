using QueueReceiver.App.Helpers;
using QueueReceiver.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QueueReceiver.App
{
    public class Worker<THandler> : BackgroundService
        where THandler : BaseHandler
    {
        private IConnection _connection;
        private IModel _channel;
        private string _consumerTag;
        private readonly THandler _handler;
        private readonly ILogger<THandler> _logger;
        private readonly IConfiguration _configuration;
	
	public Worker(THandler handler, IConfiguration configuration, ILogger<THandler> logger)
        {
            _handler = handler;
            _configuration = configuration;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Start");

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.BasicCancel(_consumerTag);
            _channel.Close();
            _connection.Close();
            _logger.LogInformation($"Finished");

            return base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken token)
        {
            _connection = RabbitMQHelper.CreateConnection(_configuration);
            _channel = RabbitMQHelper.CreateChannel(_connection, _handler.QueueName);
            var consumer = RabbitMQHelper.CreateConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();

                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation($"Received message: {message}");

                _handler.Execute(message);

                _logger.LogInformation($"Wait message...");
            };

            _consumerTag = _channel.BasicConsume(_handler.QueueName, true, consumer);

            _logger.LogInformation($"Wait message...");

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();

            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
