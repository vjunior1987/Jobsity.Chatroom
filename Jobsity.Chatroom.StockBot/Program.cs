using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jobsity.Chatroom.StockBot
{
    class Program
    {
        static void Main(string[] args)
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "chat1", durable: false, exclusive: false, autoDelete: false, arguments: null);

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body).Split('|');
                    if (Int32.TryParse(message[0], out int chatroomId))
                    {
                        var returnBody = Encoding.UTF8.GetBytes(message[0] + "|" + await GetStockQueue(message[1]));
                        channel.BasicPublish(exchange: "",
                                                 routingKey: "bot1",
                                                 mandatory: false,
                                                 basicProperties: null,
                                                 body: returnBody);

                        Console.WriteLine(" [x] Received {0}", message);
                    }
                };
                channel.BasicConsume(queue: "chat1", autoAck: true, consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        static async Task<string> GetStockQueue(string command)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (command.Trim().ToLower().Contains("/help"))
                        return "Use the command \"/stock=[stock_code]\", where [stock_code] is the identifier of the stock you wish to inquiry";

                    if (!command.Trim().ToLower().Contains("/stock="))
                        return "command not recognized. Use /help for more info.";

                    var code = command.Trim().ToLower().Replace("/stock=", "");

                    var response = await client.GetAsync(string.Format(@"https://stooq.com/q/l/?s={0}&f=sd2t2ohlcv&h&e=csv", code));
                    if (response.IsSuccessStatusCode)
                    {
                        //reading the csv file
                        using (StreamReader reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
                        {
                            List<string> listSymbol = new List<string>();
                            List<string> listOpen = new List<string>();

                            while (!reader.EndOfStream)
                            {
                                var row = await reader.ReadLineAsync();
                                if (row.Contains("N/D"))
                                {
                                    return "Stock queue not found. Try again with another code";
                                }
                                var list = row.Split(',');

                                listSymbol.Add(list[0]);
                                listOpen.Add(list[3]);
                            }
                            return string.Format("“{0} quote is ${1} per share”.", listSymbol.LastOrDefault(), listOpen.LastOrDefault());
                        }
                    }
                    else
                    {
                        //return the error message on chat
                        throw new Exception();
                    }
                }
            }
            catch (Exception)
            {
                return "there was an error when executing the command";
            }
        }
    }
}
