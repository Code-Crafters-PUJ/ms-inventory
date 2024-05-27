using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;

class BranchProductCreateUpdateProducer
{
    public async Task PublishMessage(BranchProductRMQDTO branchProductRMQDTO)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest",
        };

        try
        {
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "create-update-branch-product-queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string message = JsonSerializer.Serialize(branchProductRMQDTO);
            var body = Encoding.UTF8.GetBytes(message);

            await Task.Run(() => 
            {
                channel.BasicPublish(exchange: string.Empty,
                                     routingKey: "create-update-branch-product-queue",
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine($" BranchProduct[x] Sent '{message}'");
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
