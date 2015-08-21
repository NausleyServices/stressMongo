using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Data;
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel;

namespace stressMongo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MongoServer mongoServer;
        private MongoDatabase mongoDb; 
        private Boolean loop;
     
        
        
 
        public MainWindow()
        {
            this.insertCnt = 0;
            this.selectCnt = 0;

            InitializeComponent();
            loop = false;
            this.DataContext = this;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            loop = true;
            runTest();
            //Thread th = new Thread(runTest);
            //th.Start();
        }

        private void runTest(){
            var connectionString = "mongodb://" + txtServer.Text;
            var databaseName = txtDb.Text;
            var client = new MongoClient(connectionString);
            var mongoServer = client.GetServer();
            var mongoDb = mongoServer.GetDatabase(databaseName);
            //mongoServer = MongoServer.Create(new MongoConnectionStringBuilder("server=" + txtServer.Text + ";database=" + txtDb.Text  ));
            //mongoDb = mongoServer.GetDatabase(txtDb.Text);
            var coll = mongoDb.GetCollection("test");
            string[] weekDays = new string[] {"Sun","Sat","Mon","Tue","Wed","Thu","Fri"};
            string[] names = new string[] {"Ivan","Brock","Ming","Nan","Jeremy","David","Ed"};
            while(loop){
                if (chkInsert.IsChecked == true)
                {
                    //coll.Drop();
                    var startInsertTime = DateTime.Now;
                    
                    var rowCount = Convert.ToInt32(txtNRows.Text);
                    var fieldCount = Convert.ToInt32(txtNFields.Text);
                    var fieldName = "";
                    var random = new Random();


                    for (var row = 0; row < rowCount; row++) 
                    {
                        var document = new BsonDocument(); 
                        document.Add("docID",row);
                        for (var i=1; i < fieldCount+1 ; i++)
                        {
                            fieldName = "Field " + i.ToString();
                            if ((i % 3) == 0) { document.Add(fieldName,(random.Next(1,100)*-i)); }
                            else if ((i % 5) == 0) { document.Add(fieldName,names[random.Next(0,6)]); }
                            else if ((i % 7) == 0) { document.Add(fieldName,weekDays[random.Next(0,6)]); }
                            else if ((i % 11) == 0) { document.Add(fieldName,Math.PI*i); }
                            else if((i % 13) == 0) { document.Add(fieldName,DateTime.UtcNow); }
                            else { document.Add(fieldName,(random.Next(1,100)*i)); }
                        }

                        coll.Insert(document);
                        //Application.Current.Dispatcher.BeginInvoke(
                        //    DispatcherPriority.Background,
                        //    (Action)(() => txtInsertCnt.Text = row.ToString()));
                    }

                    var endInsertTime = DateTime.Now;
                    //txtInsertCnt.Text = (endInsertTime - startInsertTime).ToString();

                     MessageBox.Show((endInsertTime - startInsertTime).ToString());
                     
                }

                if (chkSelect.IsChecked == true)
                {
                    var x = coll.FindAllAs<BsonDocument>();
                    this.selectCnt = this.selectCnt + 1;

                    Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        (Action)(() => txtSelectCnt.Text = txtSelectCnt.Text = selectCnt.ToString()));


                }
                if (chkDelete.IsChecked == true)
                {

                  
                }
                //loop = false;
            }

        }

        
        private DataTable  buildData(int rows, int fields){
            DataTable dt = new DataTable();
            for (var i = 0; i  < fields; i++){
                dt.Columns.Add("col_" + i, typeof(string));
            }
            for (var i = 0; i < rows; i++){
                DataRow row = dt.NewRow();
                for (var c = 0; c < fields; c++){
                    row["col_" + c] = "row_" + i + " col_" + c;
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = true;
            loop = false;

        }

        private void chkUpdate_Checked(object sender, RoutedEventArgs e)
        {

        }

     

        public int insertCnt { get; set; }

        public int selectCnt { get; set; }

        private void txtInsertCnt_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtNFields_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


     
        
    }
}
