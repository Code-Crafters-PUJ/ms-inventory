// Clase BranchProductQuantityUpdateConsumer
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

public class BranchProductQuantityUpdateConsumer
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly AppDbContext _context;

    public BranchProductQuantityUpdateConsumer(AppDbContext context)
    {
        _context = context;

        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
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

            await ProcessMessage(message);

            Console.WriteLine(" [x] Received {0}", message);
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

            try
            {
                var branchProductRelation = await _context.BranchHasProduct
                    .FirstOrDefaultAsync(bhp => bhp.ProductId == productId && bhp.BranchId == branchId);

                if (branchProductRelation != null)
                {
                    branchProductRelation.Quantity = newQuantity;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine($"No se encontró la relación para el producto con ID {productId} y la sucursal con ID {branchId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la cantidad del producto en la sucursal: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Invalid message format: " + message);
        }
    }
}
