using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

class ProductDeleteProducer
{
    public async Task PublishMessage(int id)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        try
        {
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "delete-product-queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string message = id.ToString();
            var body = Encoding.UTF8.GetBytes(message);

            await Task.Run(() => 
            {
                channel.BasicPublish(exchange: string.Empty,
                                     routingKey: "delete-product-queue",
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