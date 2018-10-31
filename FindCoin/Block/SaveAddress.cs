using FindCoin.core;
using FindCoin.Mysql;
using FindCoin.thinneo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace FindCoin.Block
{
    class SaveAddress : ISave
    {

        private static SaveAddress instance = null;
        public static SaveAddress getInstance()
        {
            if (instance == null)
            {
                return new SaveAddress();
            }
            return instance;
        }

        public override void Save(JToken jObject, string path)
        {
            foreach (JObject j in jObject) {
                Dictionary<string, string> selectWhere = new Dictionary<string, string>();
                selectWhere.Add("addr", j["address"].ToString());
                DataTable dt = MysqlConn.ExecuteDataSet("address", selectWhere).Tables[0];
                if (dt.Rows.Count != 0)
                {
                    Dictionary<string, string> dirs = new Dictionary<string, string>();
                    dirs.Add("lastuse", Helper.blockTime.ToString());
                    dirs.Add("txcount", (int.Parse(dt.Rows[0]["txcount"].ToString()) + 1) + "");
                    Dictionary<string, string> where = new Dictionary<string, string>();
                    where.Add("addr", dt.Rows[0]["addr"].ToString());
                    MysqlConn.Update("address", dirs, where);
                }
                else
                {
                    JObject result = new JObject();
                    result["addr"] = j["address"];
                    result["firstuse"] = Helper.blockHeight;
                    result["lastuse"] = Helper.blockHeight;
                    result["txcount"] = 1;

                    List<string> slist = new List<string>();
                    slist.Add(j["address"].ToString());
                    slist.Add(Helper.blockHeight.ToString());
                    slist.Add(Helper.blockHeight.ToString());
                    slist.Add("1");
                    MysqlConn.ExecuteDataInsert("address", slist);
                }
            }              
        }
    }
}
