using FindCoin.core;
using FindCoin.helper;
using FindCoin.thinneo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace FindCoin.Block
{
    class FindBlock : ContractTask
    {
        private JObject config;

        public FindBlock(string name) : base(name) {

        }

        public override void initConfig(JObject config)
        {
            this.config = config;
            initConfig();
        }

        public override void startTask()
        {
            run();
        }

        private int batchInterval = 50;
        private void initConfig() {

        }

        private void run() {           
            Helper.url = getUrl();
            Helper.blockHeight = int.Parse(Config.getConfig()["startblock"].ToString());
            while (Helper.blockHeight < 500000)
            {
                if (Helper.blockHeight > Helper.blockHeightMax)
                {
                    Console.WriteLine("wait for next block...sleep fifteen seconds");
                    Thread.Sleep(15 * 1000);
                    continue;
                }

                getBlockFromRpc();

                ping();

                Helper.blockHeight++;
            }
            //Console.WriteLine(SaveUTXO.getInstance().getUTXO("ARFe4mTKRTETerRoMsyzBXoPt2EKBvBXFX").Count);
        }

        static WebClient wc = new WebClient();

        private void getBlockFromRpc() {
            JToken result = null;
            try
            {
                var getcounturl = Helper.url + "?jsonrpc=2.0&id=1&method=getblock&params=[" + Helper.blockHeight + ",1]";
                var info = wc.DownloadString(getcounturl);
                var json = JObject.Parse(info);
                result = json["result"];              
            }
            catch (Exception e)
            {
                Helper.blockHeight--;
            }
            if (result != null) {
                Helper.blockHeightMax = int.Parse(result["confirmations"].ToString()) + Helper.blockHeight;                
                SaveBlock.getInstance().Save(result as JObject, null);
            }           
        } 

        private void ping()
        {
            LogHelper.ping(batchInterval, name());
        }
    }
}
