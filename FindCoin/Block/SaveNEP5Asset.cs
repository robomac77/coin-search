using FindCoin.core;
using FindCoin.Mysql;
using FindCoin.thinneo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ThinNeo;
using Hash160 = ThinNeo.Hash160;
using Helper = FindCoin.thinneo.Helper;

namespace FindCoin.Block
{
    class SaveNEP5Asset:ISave
    {
        private static SaveNEP5Asset instance = null;
        public static SaveNEP5Asset getInstance()
        {
            if (instance == null)
            {
                return new SaveNEP5Asset();
            }
            return instance;
        }

        static WebClient wc = new WebClient();
        public override void Save(JToken jToken, string path)
        {
            string contract = jToken["assetid"].ToString();
            Dictionary<string, string> where = new Dictionary<string, string>();
            where.Add("assetid", contract);
            DataTable dt = MysqlConn.ExecuteDataSet("nep5asset", where).Tables[0];
            if (dt.Rows.Count == 0) {
                Start(contract);
            }          
        }

        public async void Start(string contract) {
            await getNEP5Asset(new Hash160(contract));
        }

        public async Task getNEP5Asset(Hash160 Contract) {
            ScriptBuilder sb = new ScriptBuilder();
            MyJson.JsonNode_Array array = new MyJson.JsonNode_Array();
            sb.EmitParamJson(array);
            sb.EmitPushString("totalSupply");
            sb.EmitAppCall(Contract);

            sb.EmitParamJson(array);
            sb.EmitPushString("name");
            sb.EmitAppCall(Contract);
            
            sb.EmitParamJson(array);
            sb.EmitPushString("symbol");
            sb.EmitAppCall(Contract);

            sb.EmitParamJson(array);
            sb.EmitPushString("decimals");
            sb.EmitAppCall(Contract);

            string scriptPublish = ThinNeo.Helper.Bytes2HexString(sb.ToArray());

            byte[] postdata;
            var url = Helper.MakeRpcUrlPost(Helper.url, "invokescript", out postdata, new MyJson.JsonNode_ValueString(scriptPublish));
            var result = await Helper.HttpPost(url, postdata);

            JObject jObject = JObject.Parse(result);
            JArray results = jObject["result"]["stack"] as JArray;
            string totalSupply = Encoding.UTF8.GetString(ThinNeo.Helper.HexString2Bytes(results[0]["value"].ToString()));
            string name = Encoding.UTF8.GetString(ThinNeo.Helper.HexString2Bytes(results[1]["value"].ToString()));            
            string symbol = Encoding.UTF8.GetString(ThinNeo.Helper.HexString2Bytes(results[2]["value"].ToString()));
            string decimals = Encoding.UTF8.GetString(ThinNeo.Helper.HexString2Bytes(results[3]["value"].ToString()));

            List<string> slist = new List<string>();
            slist.Add(Contract.ToString());
            slist.Add(totalSupply);
            slist.Add(name);
            slist.Add(symbol);
            slist.Add(decimals);
            MysqlConn.ExecuteDataInsert("nep5asset", slist);
        }
    }
}
