using RabbitMQ.Client;

public class RabbitMQHelper
{
    public static IConnection GetConnection(string hostname, string username, string password)
    {
        var factory = new ConnectionFactory()
        {
            HostName = hostname,
            UserName = username,
            Password = password
        };
        return factory.CreateConnection();
    }
}