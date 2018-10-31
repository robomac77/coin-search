using FindCoin.core;
using FindCoin.Mysql;
using FindCoin.thinneo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FindCoin.Block
{
    class SaveAddressTransaction:ISave
    {
        private static SaveAddressTransaction instance = null;
        public static SaveAddressTransaction getInstance()
        {
            if (instance == null)
            {
                return new SaveAddressTransaction();
            }
            return instance;
        }

        public override void Save(JToken jObject, string path)
        {         
            //JObject result = new JObject();
            //result["txid"] = jObject["txid"];
            //result["blockindex"] = Helper.blockHeight;
            //result["blocktime"] = Helper.blockTime;

            foreach (JObject vout in jObject["vout"]) {
                List<string> slist = new List<string>();
                slist.Add(vout["address"].ToString());
                slist.Add(jObject["txid"].ToString());
                slist.Add(Helper.blockHeight.ToString());
                slist.Add(Helper.blockTime.ToString());
				
				MysqlConn.ExecuteDataInsert("address_tx", slist);
            }           

            //File.Delete(path);
            //File.WriteAllText(path, result.ToString(), Encoding.UTF8);
        }
    }
}
