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

            string ServiceBusConnectionString = "Endpoint=sb://es-poc-service-bus.servicebus.windows.net/;SharedAccessKeyName=send-sub-key;SharedAccessKey=nTWKo5drapqfcGpjD0mwdyxI+GKXsdc2JJKE7ALsA5A=;";
            string TopicName = "topic-alert";

            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            // Send messages.
            await SendUserMessage();

            Console.WriteLine("Do you want send request from service 2? if yes 0 or No 1");

            int hasRequest = Convert.ToInt32(Console.ReadLine());

            if (hasRequest == 0)
            {
                await MainAsync();
            }

            Console.ReadKey();

            await topicClient.CloseAsync();
        }


        static async Task SendUserMessage()
        {
            List<User> users = GetDummyDataForUser();

            var serializeUser = JsonConvert.SerializeObject(users);

            string messageType = "Service 2";

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

            for (int i = 1; i < 100; i++)
            {
                lstUsers.Add(new User { Id = i, Name = $"Felish {i}" });
            }

            return lstUsers;
        }
    }

}
