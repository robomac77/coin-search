using FindCoin.core;
using FindCoin.Mysql;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace FindCoin.Block
{
    class SaveNEP5Transfer:ISave
    {
        private static SaveNEP5Transfer instance = null;
        public static SaveNEP5Transfer getInstance()
        {
            if (instance == null)
            {
                return new SaveNEP5Transfer();
            }
            return instance;
        }

        public override void Save(JToken jToken, string path)
        {
            List<string> slist = new List<string>();
            slist.Add(jToken["blockindex"].ToString());
            slist.Add(jToken["txid"].ToString());
            slist.Add(jToken["n"].ToString());
            slist.Add(jToken["asset"].ToString());
            slist.Add(jToken["from"].ToString());
            slist.Add(jToken["to"].ToString());
            slist.Add(jToken["value"].ToString());
            MysqlConn.ExecuteDataInsert("nep5transfer", slist);
        }
    }
}
