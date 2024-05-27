using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class BranchProductQuantityUpdateConsumer
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceProvider _serviceProvider;

    public BranchProductQuantityUpdateConsumer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME") ?? "localhost",
            UserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest",
            Port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672")
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void StartListening()
    {
        _channel.QueueDeclare(queue: "branch-product-quantity-update-queue",
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine($" [x] Received {message}");

            await ProcessMessage(message);
        };

        _channel.BasicConsume(queue: "branch-product-quantity-update-queue",
                              autoAck: true,
                              consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    private async Task ProcessMessage(string message)
    {
        var parts = message.Split(',');
        if (parts.Length == 3)
        {
            var productId = Convert.ToInt64(parts[0].Trim());
            var branchId = Convert.ToInt64(parts[1].Trim());
            var newQuantity = Convert.ToInt32(parts[2].Trim());

            Console.WriteLine($"Processing message: ProductId={productId}, BranchId={branchId}, NewQuantity={newQuantity}");

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var branchProductRelation = await context.BranchHasProduct
                        .FirstOrDefaultAsync(bhp => bhp.ProductId == productId && bhp.BranchId == branchId);

                    if (branchProductRelation != null)
                    {
                        Console.WriteLine("Relation found. Updating quantity...");
                        branchProductRelation.Quantity = newQuantity;
                        await context.SaveChangesAsync();
                        Console.WriteLine("Database updated successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"No relation found for ProductId={productId} and BranchId={branchId}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product quantity: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Invalid message format: " + message);
        }
    }
}
