    public class RedisConnectorHelper  
    {                  
        static RedisConnectorHelper()  
        {  
            RedisConnectorHelper.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>  
            {  
                return ConnectionMultiplexer.Connect("localhost");   // rediscache config file should provide  db configuration
            });  
        }  
          
        private static Lazy<ConnectionMultiplexer> lazyConnection;          
      
        public static ConnectionMultiplexer Connection  
        {  
            get  
            {  
                return lazyConnection.Value;  
            }  
        }  
    }  
    class Program  
    {  
        static void Main(string[] args)  
        {  
            var program = new Program();  
      
            Console.WriteLine("Saving random data in cache");  
            program.SaveBigData();  
      
            Console.WriteLine("Reading data from cache");  
            program.ReadData();  
      
            Console.ReadLine();  
        }  
      
        public void ReadData()  
        {  
            var cache = RedisConnectorHelper.Connection.GetDatabase();  
            var devicesCount = 10000;  
            for (int i = 0; i < devicesCount; i++)  
            {  
                var value = cache.StringGet($"Device_Status:{i}");  
                Console.WriteLine($"Valor={value}");  
            }  
        }  
      
        public void SaveBigData()  
        {  
            var devicesCount = 10000;  
            var rnd = new Random();  
            var cache = RedisConnectorHelper.Connection.GetDatabase();  
      
            for (int i = 1; i < devicesCount; i++)  
            {  
                var value = rnd.Next(0, 10000);  
                cache.StringSet($"Device_Status:{i}", value);  
            }  
        }  
    }  
public static int ExecuteDataInsert(string tableName, List<string> parameter)    // class to persist data into database
        {
               // var mongoconn = new MongoDB();
                 //var mongoconn = MongoServer.Create(conf)
               // mongoconn.Connect();               
              //  var db = mongoconn.GetDatabase("block");
              //var collection = mongoconn.GetCollection<tableName>()
              //db.collection.insert(parameter)
              //var cache = RedisConnectorHelper.Connection.GetDatabase();
              // cache = StringSet(parameter);
            using (MySqlConnection conn = new MySqlConnection(conf))
            {
                conn.Open();
                string mysql = $"insert into " + tableName + " values (null,"; //
                foreach (string param in parameter) {
                    mysql += "'" + param + "',";
                }               
                mysql = mysql.Substring(0, mysql.Length - 1);  
                mysql += ");";
                MySqlCommand mc = new MySqlCommand(mysql, conn);
                //collection.Save(List)
                //db.collection.insert(parameter)
                int count = mc.ExecuteNonQuery(); 
                return count;
            }
        }

         public static DataSet ExecuteDataSet(string tableName, Dictionary<string, string> where) {   // class to retrieve data from database
            // var mongoconn = new MongoDB();
           //var mongoconn = MongoServer.Create(conf)

               // mongoconn.Connect();
            using (MySqlConnection conn = new MySqlConnection(conf)) {
                conn.Open();

               
                string select = "select * from " + tableName + " where";
                foreach (var dir in where)
                {
                    select += " " + dir.Key + "='" + dir.Value + "'";
                    select += " and";
                }
                select = select.Substring(0, select.Length - 4);
                MySqlDataAdapter adapter = new MySqlDataAdapter(select, conf);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                return ds;

                //db.collection.find()
                // var select = collection.Linq();
                
            }
        }