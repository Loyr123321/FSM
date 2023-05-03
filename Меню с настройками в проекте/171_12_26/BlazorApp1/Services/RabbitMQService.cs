using BlazorApp1.Models.Mobile.Responses;
using MongoDB.Bson;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace BlazorApp1.Services
{
    public class RabbitMQService
    {
        private readonly string HOSTNAME;
        private readonly int PORT;
        private readonly string USER;
        private readonly string PASSWORD;
        public RabbitMQService()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            HOSTNAME = configuration.GetValue<string>("RabbitMQ:HOSTNAME");
            PORT = configuration.GetValue<int>("RabbitMQ:PORT");
            USER = configuration.GetValue<string>("RabbitMQ:USER");
            PASSWORD = configuration.GetValue<string>("RabbitMQ:PASSWORD");
        }

        public async Task CreateDefaultExchange()
        {
            try
            {
                ConnectionFactory factory = new ConnectionFactory() { HostName = HOSTNAME, Port = PORT, UserName = USER, Password = PASSWORD };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        //Fanout exchange – все сообщения доставляются во все очереди
                        channel.ExchangeDeclare(exchange: "OrderUpdates", type: ExchangeType.Fanout, durable: true, autoDelete: false, arguments: null);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("RabbitMQService=>CreateQueue: " + ex.Message);
            }
        }

        public async Task CreateQueue(string queueName)
        {
            try
            {
                ConnectionFactory factory = new ConnectionFactory() { HostName = HOSTNAME, Port = PORT, UserName = USER, Password = PASSWORD };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                        //Console.WriteLine("Create new Queue: " + queueName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("RabbitMQService=>CreateQueue: " + ex.Message);
            }
        }

        public async Task DeleteQueue(string queueName)
        {
            try
            {
                ConnectionFactory factory = new ConnectionFactory() { HostName = HOSTNAME, Port = PORT, UserName = USER, Password = PASSWORD };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDelete(queue: queueName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("RabbitMQService=>DeleteQueue: " + ex.Message);
            }
        }

        public async Task SendMessageToQueues(string exchangeName, string queueName, OrderDataMessage message)
        {
            try
            {
                ConnectionFactory factory = new ConnectionFactory() { HostName = HOSTNAME, Port = PORT, UserName = USER, Password = PASSWORD };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        //OrderDataMessage mymessage = (OrderDataMessage)message;
                        string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(message);

                        var body = Encoding.UTF8.GetBytes(jsonString);
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        channel.BasicPublish(exchange: exchangeName, routingKey: queueName, basicProperties: properties, body: body);
                        //Console.WriteLine(" [x] Sent {0}", message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("RabbitMQService=>SendMessageToQueues_exception: " + ex.Message);
            }
        }

        public async Task SendMessageToExchange(string exchangeName, string message)
        {
            try
            {
                ConnectionFactory factory = new ConnectionFactory() { HostName = HOSTNAME, Port = PORT, UserName = USER, Password = PASSWORD };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {

                        var body = Encoding.UTF8.GetBytes(message);
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        channel.BasicPublish(exchange: exchangeName, routingKey: "", basicProperties: properties, body: body);
                        //Console.WriteLine(" [x] Sent {0}", message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("RabbitMQService=>SendMessageToExchange: " + ex.Message);
            }
        }

        public async Task SubscribeToExchange(string exchangeName)
        {
            try
            {
                Console.WriteLine("START");

                ConnectionFactory factory = new ConnectionFactory() { HostName = HOSTNAME, Port = PORT, UserName = USER, Password = PASSWORD };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        //channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

                        var queueName = channel.QueueDeclare().QueueName;
                        channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "");

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            //
                            Console.WriteLine(" [x] {0}", message);
                        };
                        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                        //Console.WriteLine(" Press [enter] to exit.");
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("RabbitMQService=>SubscribeToExchange: " + ex.Message);
            }

        }

        //The end
    }
}
