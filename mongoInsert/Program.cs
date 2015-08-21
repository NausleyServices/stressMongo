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


namespace mongoInsert
{
    class Program
    {
        public static System.IO.StreamWriter Fileout = new System.IO.StreamWriter(@"C:\mongoInsertSpeed.txt");
        // usage mongoInsert <server> <database> <NumberofFields> <YourName>
        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Invalid arguments..\n Usage: mongoInsert <serverName> <database> <NumberofFields> <YourName>");
                return;
            }

            var myServer = args[0];
            var myDatabase = args[1];
            var myFieldCount = args[2];
            var myName = args[3];
            var connectionString = "mongodb://" + myServer;
            var databaseName = myDatabase;
            var client = new MongoClient(connectionString);
            var mongoServer = client.GetServer();
            var mongoDb = mongoServer.GetDatabase(databaseName);

            var collection = mongoDb.GetCollection("test");
            string[] weekDays = new string[] {"Sun","Sat","Mon","Tue","Wed","Thu","Fri"};
            string[] names = new string[] {"Ivan","Brock","Ming","Nan","Jeremy","David","Ed"};
            //coll.Drop();
            var startInsertTime = DateTime.Now;
            var fieldName = "";
            var random = new Random();
            Int64 recordCounter = 0;
            Console.WriteLine("Press any key to stop inserts...");
            Console.WriteLine("Collection Record Start Count: {0}", collection.Count());

            var recordList = new List<BsonDocument>();
            while (Console.KeyAvailable == false)
            {
                recordCounter++;
                var document = new BsonDocument();
                document.Add("docID", myName + recordCounter.ToString());
                document.Add("UserName", myName);
                document.Add("DateTime", DateTime.Now);
                for (var i=1; i < Convert.ToInt32(myFieldCount)+1 ; i++)
                {
                    fieldName = "F" + i.ToString();
                    if ((i % 3) == 0) { document.Add(fieldName,(random.Next(1,100)*-i)); }
                    else if ((i % 5) == 0) { document.Add(fieldName,names[random.Next(0,6)]); }
                    else if ((i % 7) == 0) { document.Add(fieldName,weekDays[random.Next(0,6)]); }
                    else if ((i % 11) == 0) { document.Add(fieldName,Math.PI*i); }
                    else if((i % 13) == 0) { document.Add(fieldName,DateTime.UtcNow); }
                    else { document.Add(fieldName,(random.Next(1,100)*i)); }
                }

                //coll.Insert(document);
                recordList.Add(document);
                if ((recordCounter % 10000) == 0) {
                    collection.InsertBatch(recordList);
                    TimeSpan tsNow = (DateTime.Now - startInsertTime);
                    var recordsPerSecondNow = recordCounter / tsNow.TotalSeconds;
                    Console.Write("\rRecord Count: {0} - Records per Second {1}  ", recordCounter.ToString(), recordsPerSecondNow.ToString()); 
                    Fileout.WriteLine("{0};{1}  ", recordCounter.ToString(), recordsPerSecondNow.ToString());
                    recordList.Clear();
                }
            }
            if (recordList.Count() > 0)
            {
                collection.InsertBatch(recordList);
                recordList.Clear();
            }
            var endInsertTime = DateTime.Now;
            TimeSpan ts = (endInsertTime - startInsertTime);
            var recordsPerSecond = recordCounter/ ts.TotalSeconds;
            Console.WriteLine("\n\nCollection Record Count: {0}", collection.Count());
            Console.WriteLine("Total Records Inserted: {0}", recordCounter.ToString());
            Console.WriteLine("Records per Second: {0}", recordsPerSecond.ToString());
            Console.WriteLine("Press any key to continue closing..");
            var myInput = Console.ReadKey();
            myInput = Console.ReadKey();
            Fileout.Flush();
            Fileout.Close();

        }
    }
}
