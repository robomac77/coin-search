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
	class SaveHeight : ISave
	{
		private static SaveHeight instance = null;
		public static SaveHeight getInstance()
		{
			if (instance == null)
			{
				return new SaveHeight();
			}
			return instance;
		}

		public override void Save(JToken jObject, string path)
		{
			//JObject result = new JObject();
			//result["txid"] = jObject["txid"];
			//result["blockindex"] = Helper.blockHeight;
			//result["blocktime"] = Helper.blockTime;

			foreach (JObject index in jObject["index"])
			{
				List<string> slist = new List<string>();
				slist.Add(index["index"].ToString());
			    slist.Add(Helper.blockHeight.ToString());
				slist.Add(Helper.blockTime.ToString());
				
				
				MysqlConn.ExecuteDataInsert("blockheight", slist);
			}

			//File.Delete(path);
			//File.WriteAllText(path, result.ToString(), Encoding.UTF8);
		}
	}
}
