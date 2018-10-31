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
    class SaveUTXO:ISave
    {
        private static SaveUTXO instance = null;
        public static SaveUTXO getInstance()
        {
            if (instance == null)
            {
                return new SaveUTXO();
            }
            return instance;
        }

        public override void Save(JToken jObject, string path)
        {
            foreach (JObject vout in jObject["vout"]) {
                JObject result = new JObject();
                result["addr"] = vout["address"];
                result["txid"] = jObject["txid"];
                result["n"] = vout["n"];
                result["asset"] = vout["asset"];
                result["value"] = vout["value"];
                result["createHeight"] = Helper.blockHeight;
                result["used"] = 0;
                result["useHeight"] = 0;
                result["claimed"] = "";

                List<string> slist = new List<string>();
                slist.Add(result["addr"].ToString());
                slist.Add(result["txid"].ToString());
                slist.Add(result["n"].ToString());
                slist.Add(result["asset"].ToString());
                slist.Add(result["value"].ToString());
                slist.Add(result["createHeight"].ToString());
                slist.Add(result["used"].ToString());
                slist.Add(result["useHeight"].ToString());
                slist.Add(result["claimed"].ToString());               
                MysqlConn.ExecuteDataInsert("utxo", slist);

                //var utxoPath = "utxo" + Path.DirectorySeparatorChar + result["txid"] + "_" + result["n"] + "_" + result["addr"] + ".txt";
                //File.Delete(utxoPath);
                //File.WriteAllText(utxoPath, result.ToString(), Encoding.UTF8);
            }
            foreach (JObject vin in jObject["vin"])
            {                
                ChangeUTXO(vin["txid"].ToString(), vin["vout"].ToString());
            }
        }

        public void ChangeUTXO(string txid, string voutNum) {
            Dictionary<string, string> dirs = new Dictionary<string, string>();
            dirs.Add("used", "1");
            dirs.Add("useHeight", Helper.blockHeight.ToString());
            Dictionary<string, string> where = new Dictionary<string, string>();
            where.Add("txid", txid);
            where.Add("n", voutNum);
            MysqlConn.Update("utxo", dirs, where);

            //JObject result = JObject.Parse(File.ReadAllText(path, Encoding.UTF8));
            //result["used"] = 1;
            //result["useHeight"] = Helper.blockHeight;
            //File.WriteAllText(path, result.ToString(), Encoding.UTF8);


        }

        public Dictionary<string, List<Utxo>> getUTXO(string address) {
            Dictionary<string, List<Utxo>> dir = new Dictionary<string, List<Utxo>>();
            var path = Directory.GetCurrentDirectory();
            
            foreach (string filePath in Directory.GetFiles(path + "/utxo")) {
                if (filePath.Contains(address)) {
                    JObject jObject = JObject.Parse(File.ReadAllText(filePath));
                    if (dir.ContainsKey(jObject["asset"].ToString()) && jObject["used"].ToString() == "0")
                    {
                        dir[jObject["asset"].ToString()].Add(new Utxo(jObject["addr"].ToString(),
                        new Hash256(Helper.HexString2Bytes(jObject["txid"].ToString())),
                        jObject["asset"].ToString(),
                        decimal.Parse(jObject["value"].ToString()),
                        int.Parse(jObject["n"].ToString())));
                    }
                    else {
                        dir.Add(jObject["asset"].ToString(),new List<Utxo>());
                    }                   
                }
            }
            return dir;
        }
    }   
}
