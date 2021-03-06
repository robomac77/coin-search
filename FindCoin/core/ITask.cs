﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace FindCoin.core
{
    /// <summary>
    /// 任务接口
    /// 
    /// </summary>    
    interface ITask
    {
        /// <summary>
        /// 初始化配置
        /// 
        /// </summary>
        /// <param name="config"></param>
        void Init(JObject config);

        /// <summary>
        /// 启动任务
        /// 
        /// </summary>
        void Start();
    }
}
