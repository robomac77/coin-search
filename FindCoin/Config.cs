using FindCoin.Mysql;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FindCoin
{
    class Config
    {
        // 主配置文件
        private static JObject config;
        public static JObject getConfig()
        {
            return config;
        }
        public static void loadConfig(string filename)
        {
            if (config == null)
            {
                config = JObject.Parse(File.ReadAllText(filename));
                initDb();
            }
        }
        
        private static void initDb()
        {
            JObject mysql = config["mysql"] as JObject;
            foreach (var mq in mysql) {
                MysqlConn.conf += mq.Key + "=" + mq.Value;
                MysqlConn.conf += ";";
            }
            MysqlConn.conf = MysqlConn.conf.Substring(0, MysqlConn.conf.Length - 1);           
        }
        
        public string getNetType()
        {
            return config["startNetType"].ToString();
        }


    }
}
