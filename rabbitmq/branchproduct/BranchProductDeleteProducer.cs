using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

class BranchProductDeleteProducer
{
    public async Task PublishMessage(int BranchId, int ProductId)
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

            channel.QueueDeclare(queue: "delete-branch-product-queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string message = BranchId.ToString() + "," + ProductId.ToString();
            var body = Encoding.UTF8.GetBytes(message);

            await Task.Run(() =>
            {
                channel.BasicPublish(exchange: string.Empty,
                                     routingKey: "delete-branch-product-queue",
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