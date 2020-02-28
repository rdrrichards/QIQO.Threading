using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QIQO.Threading
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Let's do something with threads...");
            // Init().Wait();
            var aos = GetAnalysisOrdersToProcess().Result;
            aos.ForEach(ao => { Console.WriteLine(ao.AnalysisOrderId); });
            Console.ReadKey();
        }
        public static async Task<bool> Init()
        {
            var series = Enumerable.Range(1, 25).ToList();
            var tasks = new List<Task<(int, bool)>>();
            var msgs = new List<OMLMessage>();
            foreach (var i in series)
            {
                Console.WriteLine("Starting Process {0}", i);
                // tasks.Add(DoWorkAsync(i));
                msgs.Add(new OMLMessage(i.ToString()));
            }
            // await ProcessMessages(msgs);
            //foreach (var task in await Task.WhenAll(tasks))
            //{
            //    if (task.Item2)
            //    {
            //        Console.WriteLine("Ending Process {0}", task.Item1);
            //    }
            //}
            return true;
        }
        public static async Task<List<AnalysisOrder>> GetAnalysisOrdersToProcess()
        {
            var statusList = new List<string>
            {
                "AnalysisOrderStatus.HoldBy3rdParty",
                "AnalysisOrderStatus.HoldByAlissa",
                "AnalysisOrderStatus.SubmittedTo3rdParty",
                "AnalysisOrderStatus.SubmittedToAlissa",
            };

            var orders = new List<AnalysisOrder>();
            var tasks = new List<Task<List<AnalysisOrder>>>();
            statusList.ForEach(status =>
            {
                tasks.Add(GetAnalysisOrderList(null, status, 0));
            });
            var res = await Task.WhenAll(tasks);
            res.ToList().ForEach(aos => { orders.AddRange(aos); });
            return orders;
        }
        private static Task<List<AnalysisOrder>> GetAnalysisOrderList(object o, string s, int i)
        {
            return Task.FromResult(new List<AnalysisOrder> { new AnalysisOrder() });
        }
        public static async Task<(int, bool)> DoWorkAsync(int i)
        {
            Console.WriteLine("working..{0}", i);
            await Task.Delay(1000);
            return (i, true);
        }
        private static async Task ProcessMessages(IEnumerable<OMLMessage> messages)
        {
            var tasks = new List<Task<OMLMessage>>();
            tasks.AddRange(messages.Select(msg => ProcessMessage(msg)));
            await Task.WhenAll(tasks);
        }
        private static Task<OMLMessage> ProcessMessage(OMLMessage job)
        {
            return Task.Run(() =>
            {
                System.Threading.Thread.Sleep(2000);  //Just for examples sake
                return job.Process();
            });
        }
    }
    public class OMLMessage
    {
        private readonly string _id;

        public OMLMessage(string id)
        {
            _id = id;
        }
        public OMLMessage Process()
        {
            Console.WriteLine($"Processing {_id}...");
            return this;
        }
    }
    class AnalysisOrder
    {
        public int AnalysisOrderId { get; set; } = DateTime.Now.GetHashCode();
    }
}
