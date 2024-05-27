using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class BranchCreateUpdateConsumer
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceProvider _serviceProvider;

    public BranchCreateUpdateConsumer(IServiceProvider serviceProvider)
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
        _channel.QueueDeclare(queue: "branch_queue",
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine($"[x] Received {message}");

            await ProcessMessage(message);
        };

        _channel.BasicConsume(queue: "branch_queue",
                              autoAck: true,
                              consumer: consumer);

        Console.WriteLine("Press [enter] to exit.");
    }

    private async Task ProcessMessage(string message)
    {
        var parts = message.Split(',');
        if (parts.Length == 4)
        {
            var name = parts[0].Split(':')[1].Trim();
            var address = parts[1].Split(':')[1].Trim();
            var NIT = parts[2].Split(':')[1].Trim();
            var CompanyId = Convert.ToInt32(parts[3].Split(':')[1].Trim());

            Console.WriteLine($"Processing message: Name={name}, Address={address}, NIT={NIT}, CompanyId={CompanyId}");

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var branch = await dbContext.Branch.FirstOrDefaultAsync(b => b.Name == name && b.CompanyId == CompanyId);

                    if (branch == null)
                    {
                        branch = new Branch
                        {
                            Name = name,
                            Address = address,
                            CompanyId = CompanyId,
                            Enabled = true
                        };

                        dbContext.Branch.Add(branch);
                    }
                    else
                    {
                        branch.Name = name;
                        branch.Address = address;
                        branch.Enabled = true;
                    }

                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating branch: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Invalid message format: " + message);
        }
    }
}
