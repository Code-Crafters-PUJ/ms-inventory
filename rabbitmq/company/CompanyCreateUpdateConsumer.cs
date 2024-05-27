using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class CompanyCreateUpdateConsumer
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceProvider _serviceProvider;

        public CompanyCreateUpdateConsumer(IServiceProvider serviceProvider)
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
        _channel.QueueDeclare(queue: "company_queue",
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

        _channel.BasicConsume(queue: "company_queue",
                              autoAck: true,
                              consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");
    }

        private async Task ProcessMessage(string message)
        {
             var parts = message.Split(',');
        if (parts.Length == 5)
        {
    // Extrae solo los valores, ignorando las claves
    var businessName = parts[0].Split(':')[1].Trim();
    var employeeNumber = parts[1].Split(':')[1].Trim();
    var NIT = parts[2].Split(':')[1].Trim();
    var businessArea = parts[3].Split(':')[1].Trim();
    var CompanyId = Convert.ToInt32(parts[4].Split(':')[1].Trim());

            Console.WriteLine($"Processing message: BusinessName={businessName}, EmployeeNumber={employeeNumber}, NIT={NIT}, BusinessArea={businessArea}, CompanyId={CompanyId}");
            try{
                            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var company = await dbContext.Company.FirstOrDefaultAsync(c => c.CompanyId == CompanyId);

                if (company == null)
                {
                    company = new Company
                    {
                        Name = businessName,
                        employeeNumber = employeeNumber,
                        NIT = NIT,
                        businessArea = businessArea
                    };

                    dbContext.Company.Add(company);
                }
                else
                {
                    company.Name = businessName;
                    company.employeeNumber = employeeNumber;
                    company.NIT = NIT;
                    company.businessArea = businessArea;
                }

                await dbContext.SaveChangesAsync();
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