using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace stressMongo_Read
{
    public class ReadThradObj
    {
        public static System.IO.StreamWriter Fileout = new System.IO.StreamWriter(@"C:\mongoReadSpeed.txt");
        private ManualResetEvent m_shutdownEvent;
        private TimeSpan m_delay;
        private Thread m_thread;
        private MongoDatabase m_mongoDb;

        public ReadThradObj(MongoDatabase mongoDb)
        {
            m_delay = new TimeSpan(0, 0, 0, 0, 5);
            m_mongoDb = mongoDb;
        }

        private void KeepReading()
        {
            try
            {
                bool bSignaled = false;
                Int64 recordCounter = 0;
                while (true)
                {
                    bSignaled = m_shutdownEvent.WaitOne(m_delay, true);
                    if (bSignaled)
                        break;
                    //start reading data
                    var startRead = DateTime.Now;
                    var cols = (from r in m_mongoDb.GetCollection("test").AsQueryable() select r);
                    //var xx = m_mongoDb.GetCollection("test").FindAll();

                    recordCounter = cols.Count();
                    //Console.WriteLine(cols.First());
                    TimeSpan tsNow = (DateTime.Now - startRead);
                    if (recordCounter > 0)
                    {
                        var recordsPerSecondNow = recordCounter / tsNow.TotalSeconds;
                        Console.Write("\rRecord Count: {0} - Records per Second {1}  ", recordCounter.ToString(), recordsPerSecondNow.ToString());
                        Fileout.WriteLine("{0},{1}  ", recordCounter.ToString(), recordsPerSecondNow.ToString());
                    }
                    else
                    {
                        Console.Write("\rNo records found");
                    }

                        //Console.WriteLine(cols.Last());
                    
                }
                Fileout.Flush();
                Fileout.Close();
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public void StartMongoReadThread()
        {
            try
            {
                ThreadStart ts = new ThreadStart(this.KeepReading);
                m_shutdownEvent = new ManualResetEvent(false);
                m_thread = new Thread(ts);
                m_thread.Start();
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public void StopMongoReadThread()
        {
            try
            {
                if (m_shutdownEvent != null)
                {
                    m_shutdownEvent.Set();
                    m_thread.Join(60 * 1000);
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }


    }


    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: stressmongo_read [server] [database]");
                    return;
                }

                var connectionString = "mongodb://" + args[0];
                var databaseName = args[1];
                var client = new MongoClient(connectionString);
                var mongoServer = client.GetServer();
                var mongoDb = mongoServer.GetDatabase(databaseName);

                var mongoread = new ReadThradObj(mongoDb);
                mongoread.StartMongoReadThread();
                Console.WriteLine("Reading thread is now running");

                Console.WriteLine("Press any key to stop reading thread ...");
                Console.ReadKey();
                mongoread.StopMongoReadThread();

            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
