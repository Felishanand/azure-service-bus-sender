using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Send1
{
    class Program
    {
        static ITopicClient topicClient;
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {

            string ServiceBusConnectionString_primary = "Endpoint=sb://es-poc-service-bus.servicebus.windows.net/;SharedAccessKeyName=send-sub-key;SharedAccessKey=nTWKo5drapqfcGpjD0mwdyxI+GKXsdc2JJKE7ALsA5A=;";
            string ServiceBusConnectionString = "Endpoint=sb://es-poc-service-bus.servicebus.windows.net/;SharedAccessKeyName=send-sub-key;SharedAccessKey=L+HBNJ9ZtJAyRvY5Iwj6iRqJUKD02nQwDnvcIxKNsks=;";
            string TopicName = "topic-alert";

            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            Console.WriteLine("Enter the message count:");

            int messageCount = Convert.ToInt32(Console.ReadLine());

            for (int i = 1; i < messageCount; i++)
            {
                // Send messages.
                await SendUserMessage(i);
            }

            Console.WriteLine("Do you want send request from Network Service? if yes 0 or press any for exist");

            int hasRequest = Convert.ToInt32(Console.ReadLine());

            if (hasRequest == 0)
            {
                await MainAsync();
            }

            Console.ReadKey();

            await topicClient.CloseAsync();
        }

        static async Task SendUserMessage(int serialNo)
        {
            List<User> users = GetDummyDataForUser();

            var serializeUser = JsonConvert.SerializeObject(users);

            string messageType = $"Network-Service - SNo:{serialNo}";

            string messageId = Guid.NewGuid().ToString();

            var message = new ServiceBusMessage
            {
                Id = messageId,
                Type = messageType,
                Content = serializeUser
            };

            var serializeBody = JsonConvert.SerializeObject(message);

            // send data to bus

            var busMessage = new Message(Encoding.UTF8.GetBytes(serializeBody));
            busMessage.UserProperties.Add("Type", messageType);
            busMessage.MessageId = messageId;

            await topicClient.SendAsync(busMessage);

            Console.WriteLine("message has been sent");

        }

        public class User
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        public class ServiceBusMessage
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public string Content { get; set; }
        }

        private static List<User> GetDummyDataForUser()
        {
            User user = new User();
            List<User> lstUsers = new List<User>();

            for (int i = 1; i < 200; i++)
            {
                lstUsers.Add(new User { Id = i, Name = $"Felish Anand {i}" });
            }

            return lstUsers;
        }
    }

}
