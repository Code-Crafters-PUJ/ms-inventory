using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class RabbitMqConsumerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _companyQueueName = "company_queue";
    private readonly string _branchQueueName = "branch_queue";

    public RabbitMqConsumerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        var factory = new ConnectionFactory()
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME") ?? "localhost",
            UserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest",
            Port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672"),
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel.QueueDeclare(queue: _companyQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: _branchQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var companyConsumer = new EventingBasicConsumer(_channel);
        companyConsumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            // Add a log message to visualize the received JSON
            Console.WriteLine($"Received JSON message for company: {message}");

            try
            {
                var company = ParseCompanyMessage(message);
                if (company != null)
                {
                    await ProcessCompanyMessage(company);
                }
                else
                {
                    Console.WriteLine("Failed to parse Company message.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing Company message: {ex.Message}");
            }
        };

        var branchConsumer = new EventingBasicConsumer(_channel);
        branchConsumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            // Add a log message to visualize the received JSON
            Console.WriteLine($"Received JSON message for branch: {message}");

            try
            {
                var branch = ParseBranchMessage(message);
                if (branch != null)
                {
                    await ProcessBranchMessage(branch);
                }
                else
                {
                    Console.WriteLine("Failed to parse Branch message.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing Branch message: {ex.Message}");
            }
        };

        _channel.BasicConsume(queue: _companyQueueName, autoAck: true, consumer: companyConsumer);
        _channel.BasicConsume(queue: _branchQueueName, autoAck: true, consumer: branchConsumer);

        return Task.CompletedTask;
    }

  private Company ParseCompanyMessage(string message)
{
    var parts = message.Split(',');
    var company = new Company();
    foreach (var part in parts)
    {
        var keyValue = part.Split(':');
        var key = keyValue[0].Trim();
        var value = keyValue[1].Trim();

        switch (key)
        {
            case "businessName":
                company.Name = value;
                break;
            case "employeeNumber":
                company.employeeNumber = value;
                break;
            case "NIT":
                company.NIT = value;
                break;
            case "businessArea":
                company.businessArea = value;
                break;
            case "IDCompany":
                if (int.TryParse(value, out int companyId))
                {
                    company.CompanyId = companyId;
                }
                break;
            default:
                // Handle unknown keys or ignore them
                break;
        }
    }
    return company;
}

    private Branch ParseBranchMessage(string message)
    {
        var parts = message.Split(',');
        var branch = new Branch();
        foreach (var part in parts)
        {
            var keyValue = part.Split(':');
            var key = keyValue[0].Trim();
            var value = keyValue[1].Trim();

            switch (key)
            {
                case "name":
                    branch.Name = value;
                    break;
                case "address":
                    branch.Address = value;
                    break;
                case "NIT":
                    branch.BranchId = int.Parse(value);
                    break;
                case "CompanyId":
                    if (int.TryParse(value, out int companyId))
                    {
                        branch.CompanyId = companyId;
                    }
                    break;
                default:
                    // Handle unknown keys or ignore them
                    break;
            }
        }
        return branch;
    }

    private async Task ProcessCompanyMessage(Company company)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Company.Add(company);
            await context.SaveChangesAsync();
        }
    }

    private async Task ProcessBranchMessage(Branch branch)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Branch.Add(branch);
            await context.SaveChangesAsync();
        }
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
