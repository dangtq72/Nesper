using System;
using System.Threading;
using System.Collections.Generic;
using com.espertech.esper.client;

namespace SimpleExample
{
    class Program
    {
        private static Timer _eventTimer;
        private static Random _random;

        private static EPAdministrator _administrator;
        private static EPRuntime _runtime;
        static decimal _id = 0;

        static void Main(string[] args)
        {
            //InitializeContext();
            //InitializeEsper_new();

            InitializeEsper();
            InitializeEvents();


            // Make the application hang around - failure to do this will result
            // in the application terminating because there are no non-daemon
            // threads running.
            Console.WriteLine("Application waiting until you hit return to end");
            Console.WriteLine("");
            Console.ReadLine();
        }

        static void InitializeContext()
        {
            //const string ContextQuery = "create context Ctx initiated by pattern[timer: interval(0) or every timer: interval(10sec)]terminated after 10 seconds";
            //const string SelectQuery = "context Ctx SELECT COUNT(*) FROM SimpleExample.TestMessage output snapshot when terminated";

            string ContextQuery = "create context SegmentedByTestMessage partition by Id from SimpleExample.TestMessage";
            string SelectQuery = "context SegmentedByTestMessage SELECT sum(Price) as sum_price from SimpleExample.TestMessage";

            EPServiceProviderManager.GetDefaultProvider().EPAdministrator.CreateEPL(ContextQuery);
            EPStatement statement = EPServiceProviderManager.GetDefaultProvider().EPAdministrator.CreateEPL(SelectQuery);
            statement.AddEventHandlerWithReplay(TestHandler);
        }


        private static void TestHandler(object sender, UpdateEventArgs args)
        {
            //Console.Write(args.NewEvents.Count());

            if (args.NewEvents != null)
            {
                foreach (var e in args.NewEvents)
                {
                    Dictionary<string, object> _dc = (Dictionary<string, object>)e.Underlying;
                    foreach (object item in _dc.Values)
                        Console.WriteLine(" --> sum(Price): " + (decimal)item);
                }
            }
        }

        static void InitializeEsper_new()
        {
            var serviceProvider = EPServiceProviderManager.GetDefaultProvider();
            _runtime = serviceProvider.EPRuntime;
            _administrator = serviceProvider.EPAdministrator;


            var statement = _administrator.CreateEPL("context SegmentedByTestMessage select * from SimpleExample.TestMessage");
            statement.Events += new UpdateEventHandler(HandleEvent_new);
        }

        private static void HandleEvent_new(object sender, UpdateEventArgs args)
        {
            // if you have a window or a complex rule, you will find that the event args contains
            // information about events that are no longer applicable and those that are currently
            // applicable.  For now, we only care about those events that are directly applicable.
            foreach (var e in args.NewEvents)
            {
                // e is an "EventBean" which is a wrapper around the underlying object.  If you
                // are dealing with a native object, you can access it by looking directly at
                // the underlying as follows.
                Console.WriteLine("TradeEvent - " + e.Underlying.ToString());
            }
        }


        /// <summary>
        /// Initializes Esper and prepares event listeners.
        /// </summary>
        static void InitializeEsper()
        {
            var serviceProvider = EPServiceProviderManager.GetDefaultProvider();
            _runtime = serviceProvider.EPRuntime;
            _administrator = serviceProvider.EPAdministrator;

            // You must tell esper about your events, failure to do so means you will be using
            // the fully qualified names of your events.  There are many ways to do that, but
            // this one is short-hand which makes the name of your event aliased to the fully
            // qualified name of your type.
            _administrator.Configuration.AddEventType<TradeEvent>();
            // Create a statement or pattern; these are the bread and butter of esper.  This
            // method creates a statement.  You want to hang on to the statement if you intend
            // to listen directly to the results.
            //var statement = _administrator.CreateEPL("select * from TradeEvent.win:length(5)");
            //var statement = _administrator.CreateEPL("select sum(Quantity) as sum_qtty from TradeEvent.win:time(10 sec)");
            var statement = _administrator.CreateEPL("select Symbol, sum(Quantity) as sum_qtty from TradeEvent.win:time(10 sec) group by Symbol");

            // Hook up an event handler to the statement
            statement.Events += new UpdateEventHandler(HandleEvent_Trade);
             
        }

        // win:time(n sec) lấy trong 10 giây gần nhất
        // win.length(n) lấy n object send gần nhất
        // select * from TradeEvent(Quantity>=200).win:length(5) tradeEvent thỏa mãn Quantity >= 200 và chỉ lấy 5 TradeEvent thỏa mãn gần nhất
        // select * from TradeEvent.win:length(5) where Quantity >= 200 lấy tất cả các TradeEvent có Quantity >= 200 trong 5 TradeEvent gần nhất

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="com.espertech.esper.client.UpdateEventArgs"/> instance containing the event data.</param>
        private static void HandleEvent_Trade(object sender, UpdateEventArgs args)
        {
            // if you have a window or a complex rule, you will find that the event args contains
            // information about events that are no longer applicable and those that are currently
            // applicable.  For now, we only care about those events that are directly applicable.
            foreach (var e in args.NewEvents)
            {
                // e is an "EventBean" which is a wrapper around the underlying object.  If you
                // are dealing with a native object, you can access it by looking directly at
                // the underlying as follows.
                //Console.WriteLine("TradeEvent - " + e.Underlying + " detail " + e.ToString());
                Console.WriteLine("---------------------------------------> Symbol " + e.Get("Symbol") + " Total Quantity: " + e.Get("sum_qtty"));
                Console.WriteLine("");

                //Console.WriteLine("------> Out Total Quantity: " + e.Get("sum_qtty"));
                // However, more complex types and maps don't necessarily behave as cleanly and
                // the EventBean hides some of that from you.
                //Console.WriteLine("TradeEvent - Symbol = " + e.Get("Symbol"));
                //Console.WriteLine("TradeEvent - Price = " + e.Get("Price"));
                //Console.WriteLine("TradeEvent - Quantity = " + e.Get("Quantity"));
            }
        }

       
        /// <summary>
        /// Esper requires events to come from somewhere; this application is just a dummy
        /// application so we have to manufacture the events.  In a real application, you
        /// would receive these from "something" that produces these events.
        /// </summary>
        static void InitializeEvents()
        {
            _random = new Random();
            _eventTimer = new Timer(o => SendTradeEvent(), null, 5000, 5000);
        }

        /// <summary>
        /// Sends the trade event.
        /// </summary>
        static void SendTradeEvent()
        {
            // Create a phantom event
            _id++;
            var tradeEvent = new TradeEvent()
            {
                Symbol = "GOOG",
                Price = 700 + _random.Next(0, 100),
                Quantity = 100 + _random.Next(1, 10) * 100
            };
            
            // Events must be injected into the runtime
            Console.WriteLine(_id.ToString() + ": " + tradeEvent.ToString());
            Console.WriteLine("");

            _runtime.SendEvent(tradeEvent);

            var tradeEvent1 = new TradeEvent()
            {
                Symbol = "FACE",
                Price = 700 + _random.Next(0, 100),
                Quantity = 100 + _random.Next(1, 10) * 100
            };
            // Events must be injected into the runtime
            Console.WriteLine(_id.ToString() + ": " + tradeEvent1.ToString());
            Console.WriteLine("");
            _runtime.SendEvent(tradeEvent1);

        }
    }

    public class TradeEvent
    {
        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            //return string.Format("Symbol: {0}, Quantity: {1}, Price: {2}", Symbol, Quantity, Price);
            return string.Format("Symbol: {0}, Quantity: {1}", Symbol, Quantity);

        }
    }


    public class TestMessage
    {
        public decimal Id { get; set; }
        public string Symbol { get; set; }
        public decimal Price { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, Symbol: {1}, Price: {2}", Id, Symbol, Price);
        }
    }

    public class MonitorEvent
    {
        public string Symbol { get; set; }

        public TradeEvent TradeEvent { get; set; }
    }
}
