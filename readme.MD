# FindCoin功能介绍
## 主要功能类
* Config 读取配置文件，目前只有数据库配置数据
* FindBlock 查找块数据
  * 主要功能：开启一个线程按顺序请求块数据，提交给SaveBlock中Save方法存储到mysql数据库中
* SaveBlock 把查找到的块数据存储到mysql数据库的block表中，并解析出transaction数据提交给SaveTransaction。
* SaveTransacation 把解析的transaction数据存入mysql数据库。
    * 解析出address数据提交给SaveAddress
    * 解析出utxo数据提交给SaveUTXO
    * 解析出asset数据并存入mysql数据库
    * 解析出addressTransaction提交给SaveAddressTransaction
    * 把InvocationTransaction交易的txid提交给SaveNotify，用来查询log数据
* SaveAddress 保存address数据到mysql数据库
* SaveAddressTransaction 保存addressTransaction数据到mysql数据库
* SaveUTXO 保存所有UTXO数据到mysql数据库
* SaveNotify 把获取的txid传入getapplicationlog方法获取log数据，并把此数据存入mysql数据库
    * 解析log数据获取NEP5Asset资产提交给SaveNEP5Asset。
    * 解析log数据获取NEP5Transfer资产提交给SaveNEP5Transfer。
* SaveNEP5Asset 保存NEP5Asset数据到mysql数据库
* SaveNEP5Transfer 保存NEP5Transfer数据到mysql数据库
## 辅助工具类
* MysqlConn 提供Mysql数据库的增改查方法。
* thinneo文件包 提供neo的一些类型
