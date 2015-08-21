using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using MongoDB.Bson;


namespace mongoRead
{
    class Program
    {
        public static System.IO.StreamWriter Fileout = new System.IO.StreamWriter(@"C:\mongoReadSpeed.txt");
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Invalid arguments..\n Usage: mongoRead <serverName> <database>");
                return;
            }

            var myServer = args[0];
            var myDatabase = args[1];
            var connectionString = "mongodb://" + myServer;
            var databaseName = myDatabase;
            var client = new MongoClient(connectionString);
            var mongoServer = client.GetServer();
            var mongoDb = mongoServer.GetDatabase(databaseName);
            Int64 recordCounter = 0;

            Console.WriteLine("Press any key to stop reads...");
            var startInsertTime = DateTime.Now;
            while (Console.KeyAvailable == false)
            {
                var startRead = DateTime.Now;
                var cols = (from r in mongoDb.GetCollection("test").AsQueryable() select r);

                recordCounter = cols.Count();
                var data = cols.LastOrDefault();
                //data = cols.LastOrDefault();
                //Console.WriteLine(cols.First());
                TimeSpan tsNow = (DateTime.Now - startRead);
                if (recordCounter > 0)
                {
                    var recordsPerSecondNow = recordCounter / tsNow.TotalSeconds;
                    Console.Write("\rRecord Count: {0} - Records per Second {1}  ", recordCounter.ToString(), recordsPerSecondNow.ToString());
                    Fileout.WriteLine("{0};{1}  ", recordCounter.ToString(), recordsPerSecondNow.ToString());
                    Fileout.Flush();
                }
                else
                {
                    Console.Write("\rNo records found");
                }
            }

            //var endInsertTime = DateTime.Now;
            //TimeSpan ts = (endInsertTime - startInsertTime);
            //var recordsPerSecond = recordCounter / ts.TotalSeconds;
            //Console.WriteLine("\n\nCollection Record Count: {0}", mongoDb.GetCollection("test").Count());
            //Console.WriteLine("Total Records Inserted: {0}", recordCounter.ToString());
            //Console.WriteLine("Records per Second: {0}", recordsPerSecond.ToString());
            Console.WriteLine("\n\nPress any key to continue closing..");
            var myInput = Console.ReadKey();
            myInput = Console.ReadKey();
            Fileout.Flush();
            Fileout.Close();

        }
    }
}
