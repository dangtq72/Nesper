using com.espertech.esper.client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Nvs_Nesper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        bool _is_run_thread = false;

        private EPAdministrator _administrator;
        private EPRuntime _runtime;

        //hello every body
        // commit all

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeEsper();

            Thread _thr_send = new Thread(AutoSend);
            _thr_send.IsBackground = true;
            _thr_send.Start();
        }

        // win:time(n sec) lấy trong 10 giây gần nhất
        // win.length(n) lấy n object send gần nhất
        // select * from TradeEvent(Quantity>=200).win:length(5) tradeEvent thỏa mãn Quantity >= 200 và chỉ lấy 5 TradeEvent thỏa mãn gần nhất
        // select * from TradeEvent.win:length(5) where Quantity >= 200 lấy tất cả các TradeEvent có Quantity >= 200 trong 5 TradeEvent gần nhất
        // select * from TradeEvent.win:time_batch(4 sec) lấy tất cả TradEvent trong khoảng 4s (1->4) (5->8) (9->12)
        // select Symbol, sum(Quantity) as sum_qtty from TradeEvent.win:time_batch(10 sec); cộng dồn Quantity trong khoảng 10s. Cứ 1 event vào trả ra 1 dòng
        /// <summary>
        /// Initializes Esper and prepares event listeners.
        /// </summary>
        void InitializeEsper()
        {
            try
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

                //string _epl = "select * from TradeEvent.win:length(5)";
                //string _epl = "select sum(Quantity) as sum_qtty from TradeEvent.win:time(10 sec)";
                //string _epl = "select Symbol, sum(Quantity) as sum_qtty from TradeEvent.win:time(10 sec) group by Symbol";
                //string _epl = "select Symbol, sum(Quantity) as sum_qtty from TradeEvent.win:time_batch(10 sec)";
                //string _epl = "select Symbol, sum(Quantity) as sum_qtty from TradeEvent.win:time(30 sec).std:firstunique(Symbol) retain-union";
                //string _epl = "select Symbol, sum(Quantity) as sum_qtty from TradeEvent.win:time(30 sec).std:firstunique(Symbol) retain-intersection";

                //string _epl = "create context Ctx partition by Symbol from TradeEvent ";
                //_epl = "context Ctx select Symbol, sum(Quantity) as sum_qtty from TradeEvent group by Symbol";

                //string _epl = "create context Ctx group Quantity < 500 as low_qtty, group Quantity between 500 and 1000 as normal, group Quantity > 1000 as large from TradeEvent ";
                //_epl = "context Ctx select context.label, count(*) from TradeEvent";

                //string _epl = "create context Ctx partition by Symbol and Price from TradeEvent";

                string _epl = "create context Ctx partition by Symbol from TradeEvent";
                _administrator.CreateEPL(_epl);


                //_epl = "context Ctx select * from TradeEvent";
                //_epl = "context Ctx select context.name, context.id, context.key1, context.key2 from TradeEvent";

                _epl = "context Ctx select * from TradeEvent as t1 unidirectional, TradeEvent.win:time(30) t2 where t2.Symbol = t1.Symbol";
                var statement = _administrator.CreateEPL(_epl);

                // Hook up an event handler to the statement
                statement.Events += new UpdateEventHandler(HandleEvent_Trade);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }



        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="com.espertech.esper.client.UpdateEventArgs"/> instance containing the event data.</param>
        private void HandleEvent_Trade(object sender, UpdateEventArgs args)
        {
            // if you have a window or a complex rule, you will find that the event args contains
            // information about events that are no longer applicable and those that are currently
            // applicable.  For now, we only care about those events that are directly applicable.
            foreach (var e in args.NewEvents)
            {
                // e is an "EventBean" which is a wrapper around the underlying object.  If you
                // are dealing with a native object, you can access it by looking directly at
                // the underlying as follows.

                //string _s = "------------------> lable " + e.Get("label") + " count(*): " + e.Get("count(*)");
                //string _s = "------------------> Symbol: " + e.Get("Symbol") + " Quantity: " + e.Get("Quantity");
                string _s = "------------------> t1: " + e.Get("t1") + ",   t2: " + e.Get("t2");

                ShowonTextbox(_s);
            }
        }

        void AutoSend()
        {
            Random _random = new Random();
            int _id = 0;
            string _time = "";
            while (true)
            {
                if (_is_run_thread)
                {
                    _time = DateTime.Now.ToString("HH:mm:ss:ffffff");
                    // Create a phantom event
                    _id++;
                    TradeEvent tradeEvent;
                    if (_random.Next(1, 1000) % 2 == 0)
                    {
                        tradeEvent = new TradeEvent()
                        {
                            Symbol = "GOOG",
                            Price = 700 + _random.Next(0, 100),
                            Quantity = 100 + _random.Next(1, 10) * 100
                        };
                    }
                    else
                    {
                        tradeEvent = new TradeEvent()
                        {
                            Symbol = "FACE",
                            Price = 700 + _random.Next(0, 100),
                            Quantity = 100 + _random.Next(1, 10) * 100
                        };
                    }

                    ShowonTextbox(_time + ": " + tradeEvent.ToString());
                    _runtime.SendEvent(tradeEvent);
                }

                Thread.Sleep(3000);
            }
        }

        private delegate void UpdateTextboxDelegate(string p_Msg);
        private void ShowonTextbox(string p_Msg)
        {
            if (lvMsgIn.InvokeRequired)
            {
                lvMsgIn.BeginInvoke(new UpdateTextboxDelegate(this.ShowonTextbox), p_Msg);
            }
            else
            {
                if (ckbShowMsg.Checked == true)
                {
                    ListViewItem lstViewTemp = new ListViewItem();
                    lstViewTemp = new ListViewItem(new string[] { p_Msg });
                    lvMsgIn.Items.Add(lstViewTemp);
                    lvMsgIn.TopItem = lstViewTemp;
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            _is_run_thread = true;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _is_run_thread = false;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
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
            return string.Format("Symbol: {0}, Quantity: {1}, Price: {2}", Symbol, Quantity, Price);
            //return string.Format("Symbol: {0}, Quantity: {1}", Symbol, Quantity);

        }
    }
}