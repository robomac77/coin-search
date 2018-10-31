using FindCoin.core;
using FindCoin.helper;
using FindCoin.Mysql;
using FindCoin.thinneo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace FindCoin.Block
{
    class SaveNotify:ISave
    {
        private static SaveNotify instance = null;
        public static SaveNotify getInstance()
        {
            if (instance == null)
            {
                return new SaveNotify();
            }
            return instance;
        }

        static WebClient wc = new WebClient();
        public override void Save(JToken jToken, string path)
        {
            JToken result = null;
            try
            {
                var getUrl = Helper.url + "?jsonrpc=2.0&id=1&method=getapplicationlog&params=[" + jToken["txid"] + "]";
                var info = wc.DownloadString(getUrl);
                var json = JObject.Parse(info);
                result = json["result"];
            }
            catch (Exception e) {
                LogHelper.printLog("txid " + jToken["txid"] + " has 500");
            }            
            if (result != null) {
                JObject jObject = new JObject();
                jObject["txid"] = jToken["txid"];
                jObject["vmstate"] = result["vmstate"];
                jObject["gas_consumed"] = result["gas_consumed"];
                jObject["stack"] = result["stack"];
                jObject["notifications"] = result["notifications"];
                jObject["blockindex"] = Helper.blockHeight;

                List<string> slist = new List<string>();
                slist.Add(jToken["txid"].ToString());
                slist.Add(result["vmstate"].ToString());
                slist.Add(result["gas_consumed"].ToString());
                slist.Add(result["stack"].ToString());
                slist.Add(result["notifications"].ToString());
                slist.Add(Helper.blockHeight.ToString());
                MysqlConn.ExecuteDataInsert("notify", slist);

                var notifyPath = "notify" + Path.DirectorySeparatorChar + result["txid"] + "_" + result["n"] + ".txt";
                File.Delete(notifyPath);
                File.WriteAllText(notifyPath, jObject.ToString(), Encoding.UTF8);

                foreach (JObject notify in jObject["notifications"]) {
                    if (notify["state"]["value"][0]["type"].ToString() == "ByteArray") {
                        string transfer = Encoding.UTF8.GetString(Helper.HexString2Bytes(notify["state"]["value"][0]["value"].ToString()));
                        string contract = notify["contract"].ToString();
                       
                        if (transfer == "transfer") {
                            JObject nep5 = new JObject();
                            nep5["assetid"] = contract;
                            SaveNEP5Asset.getInstance().Save(nep5, null);

                            //存储Nep5Transfer内容
                            JObject tx = new JObject();
                            tx["blockindex"] = Helper.blockHeight;
                            tx["txid"] = jToken["txid"].ToString();
                            tx["n"] = 0;
                            tx["asset"] = contract;
                            tx["from"] = Encoding.UTF8.GetString(Helper.HexString2Bytes(notify["state"]["value"][1]["value"].ToString()));
                            tx["to"] = Encoding.UTF8.GetString(Helper.HexString2Bytes(notify["state"]["value"][2]["value"].ToString()));
                            tx["value"] = Encoding.UTF8.GetString(Helper.HexString2Bytes(notify["state"]["value"][3]["value"].ToString()));
                            SaveNEP5Transfer.getInstance().Save(tx, null);
                        }
                    }                   
                }
            }
        }
    }
}
