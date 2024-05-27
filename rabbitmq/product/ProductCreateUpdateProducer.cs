using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;

class ProductCreateUpdateProducer
{
    public async Task PublishMessage(ProductRMQDTO productRMQDTO)
    {
        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME") ?? "localhost",
            UserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest",
            Port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672")
        };

        try
        {
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "create-update-product-queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string message = JsonSerializer.Serialize(productRMQDTO);
            var body = Encoding.UTF8.GetBytes(message);

            await Task.Run(() => 
            {
                channel.BasicPublish(exchange: string.Empty,
                                     routingKey: "create-update-product-queue",
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine($" [x] Sent '{message}'");
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}