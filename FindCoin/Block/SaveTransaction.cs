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
    class SaveTransaction : ISave
    {
        private static SaveTransaction instance = null;
        public static SaveTransaction getInstance()
        {
            if (instance == null)
            {
                return new SaveTransaction();
            }
            return instance;
        }

        public override void Save(JToken jObject, string path)
        {
            JObject result = new JObject();
            result["txid"] = jObject["txid"];
            result["size"] = jObject["size"];
            result["type"] = jObject["type"];
            result["version"] = jObject["version"];
            result["attributes"] = jObject["attributes"];
            result["vin"] = jObject["vin"];
            result["vout"] = jObject["vout"];
            result["sys_fee"] = jObject["sys_fee"];
            result["net_fee"] = jObject["net_fee"];
            result["scripts"] = jObject["scripts"];
            result["nonce"] = jObject["nonce"];
            result["blockindex"] = Helper.blockHeight;

            List<string> slist = new List<string>();
            slist.Add(result["txid"].ToString());
            slist.Add(result["size"].ToString());
            slist.Add(result["type"].ToString());
            slist.Add(result["version"].ToString());
            slist.Add(result["attributes"].ToString());
            slist.Add(result["vin"].ToString());
            slist.Add(result["vout"].ToString());
            slist.Add(result["sys_fee"].ToString());
            slist.Add(result["net_fee"].ToString());
            slist.Add(result["scripts"].ToString());
            slist.Add(result["nonce"].ToString());
            slist.Add(Helper.blockHeight.ToString());
            MysqlConn.ExecuteDataInsert("tx", slist);

            //File.Delete(path);
            //File.WriteAllText(path, result.ToString(), Encoding.UTF8);

            SaveAddress.getInstance().Save(result["vout"], null);

            SaveUTXO.getInstance().Save(result, null);

            var addressTransactionPath = "addressTransaction" + Path.DirectorySeparatorChar + result["txid"] + ".txt";
            SaveAddressTransaction.getInstance().Save(result, addressTransactionPath);

            if (result["type"].ToString() == "RegisterTransaction")
            {
                var assetPath = "asset" + Path.DirectorySeparatorChar + result["txid"] + ".txt";
                saveAsset(jObject, assetPath);
            }
            else if (result["type"].ToString() == "InvocationTransaction")
            {
                SaveNotify.getInstance().Save(result, null);
            }
        }

        private void saveAsset(JToken jObject, string path)
        {
            JObject result = new JObject();
            result["version"] = jObject["version"];
            result["id"] = jObject["txid"];
            result["type"] = jObject["asset"]["type"];
            result["name"] = jObject["asset"]["name"];
            result["amount"] = jObject["asset"]["amount"];
            result["available"] = 1;
            result["precision"] = jObject["asset"]["precision"];
            result["owner"] = jObject["asset"]["owner"];
            result["admin"] = jObject["asset"]["admin"];
            result["issuer"] = 1;
            result["expiration"] = 0;
            result["frozen"] = 0;

            List<string> slist = new List<string>();
            slist.Add(result["version"].ToString());
            slist.Add(result["id"].ToString());
            slist.Add(result["type"].ToString());
            slist.Add(result["name"].ToString());
            slist.Add(result["amount"].ToString());
            slist.Add(result["available"].ToString());
            slist.Add(result["precision"].ToString());
            slist.Add(result["owner"].ToString());
            slist.Add(result["admin"].ToString());
            slist.Add(result["issuer"].ToString());
            slist.Add(result["expiration"].ToString());
            slist.Add(result["frozen"].ToString());
            MysqlConn.ExecuteDataInsert("asset", slist);

            //File.Delete(path);
            //File.WriteAllText(path, result.ToString(), Encoding.UTF8);
        }
    }
}
