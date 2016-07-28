using com.espertech.esper.client;
using System;
using System.Linq;
using System.Threading;

namespace SimpleExample
{
    public class test_context
    {
        private static Random _random = new Random();

        public void Create()
        {
            //const string ContextQuery = "create context Ctx initiated by pattern[timer: interval(0) or every timer: interval(10sec)]terminated after 10 seconds";
            //const string SelectQuery = "context Ctx SELECT COUNT(*) FROM SimpleExample.TestMessage output snapshot when terminated";

            const string ContextQuery = "create context avc partition by Id from SimpleExample.TestMessage";
            const string SelectQuery = "context avc SELECT Id,sum(Price) FROM SimpleExample.TestMessage group by Id";

            EPServiceProviderManager.GetDefaultProvider().EPAdministrator.CreateEPL(ContextQuery);
            EPStatement statement = EPServiceProviderManager.GetDefaultProvider().EPAdministrator.CreateEPL(SelectQuery);
            statement.AddEventHandlerWithReplay(TestHandler);

            Thread _thr = new Thread(Runthread);
            _thr.IsBackground = true;
            _thr.Start();
        }

        void Runthread()
        {
            while (true)
            {
                TestMessage testMessage = new TestMessage();
                testMessage.Id = _random.Next(0, 100);
                testMessage.Symbol = "GOOG";
                testMessage.Price = _random.Next(100, 1000);

                EPServiceProviderManager.GetDefaultProvider().EPRuntime.SendEvent(testMessage);
                //Console.WriteLine("message sent");
                Thread.Sleep(100);
            }
        }

        private static void TestHandler(object sender, UpdateEventArgs args)
        {
            //Console.WriteLine("<-- Handler: ");
            //Console.Write(args.NewEvents.Count());

            foreach (var e in args.NewEvents)
            {
                Console.WriteLine(" --> TestMessage: " + e.Underlying.ToString());
            }
        }

        internal class TestMessage
        {
            public decimal Id { get; set; }
            public string Symbol { get; set; }
            public decimal Price { get; set; }

            public override string ToString()
            {
                return string.Format("Id: {0}, Symbol: {1}, Price: {2}", Id, Symbol, Price);
            }
        }
    }
}

//Result is "An unhandled exception of type 'System.StackOverflowException'
//occurred in NEsper.dll". NEsper version is 4.5.2.0.
//Have anybody met something like that?
//FAQ says that it may be caused by sending messages within a handler that causes the same handler to be called but this is obviously not the case in
//my example. So, i'm a bit lost and i need to go into production with NEsper
//next week.
//Thanks in advance!
//P.S.I'm aware of option of using win(10 sec) but this doesn't suit my
//needs since it stores original messages during entire window life and it is gonna be a problem for me.



