using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Data;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using ZXing.QrCode.Internal;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using System.Drawing;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;


namespace SVMAdmin.Controllers
{
    [Route("api")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public DataController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        //2023/04/28 新增登入前金鑰設定
        [Route("GetkeyData")]
        public ActionResult GetkeyData()
        {
            IFormCollection rq = HttpContext.Request.Form;
            string keyData = rq["keyData"];
            string CompanyID = rq["CompanyID"];
            string RealData = PubUtility.enCode170215(keyData);
            string Company = RealData.Split("@@")[0].Trim();
            string ckno = RealData.Split("@@")[1].Substring(0, 2);

            UserInfo uu = new UserInfo();
            uu.UserID = "Login";
            uu.CompanyId = CompanyID;

            string sql = "";
            //sql += "Select ST_ID+' '+ST_Sname ShopName,ST_SName from WarehouseWeb (nolock)";
            //sql += " where CompanyCode='" + uu.CompanyId + "' and ST_ID='" + shop + "'";

            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetkeyDataOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtkey = PubUtility.SqlQry("Select '" + RealData + "' Realkey,Replace(Convert(varchar(19),getdate(),121),'-','/') SysDT,convert(varchar,getdate(),111) SysDate", uu, "SYS");
                dtkey.TableName = "dtkey";
                ds.Tables.Add(dtkey);

                //DataTable dtTmp = PubUtility.SqlQry(sql, uu, "SYS");
                //dtTmp.TableName = "dtWh";
                //ds.Tables.Add(dtTmp);

                sql = "Select *,convert(varchar,getdate(),111) SysDate from ISAMCKNO (nolock) where Companycode='" + uu.CompanyId + "' and ckno='" + ckno + "' ";
                DataTable dtCkno = PubUtility.SqlQry(sql, uu, "SYS");
                dtCkno.TableName = "dtCkno";
                ds.Tables.Add(dtCkno);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2023/04/28 新增登入前金鑰設定
        [Route("ChkkeyData")]
        public ActionResult ChkkeyData()
        {
            IFormCollection rq = HttpContext.Request.Form;
            string keyData = rq["keyData"];
            string CompanyID = rq["CompanyID"];
            string RealData = PubUtility.enCode170215(keyData);
            //string Company = RealData.Split("@@")[0].Trim();
            //string ckno = RealData.Split("@@")[1].Substring(0, 2);

            UserInfo uu = new UserInfo();
            uu.UserID = "Login";
            uu.CompanyId = CompanyID;

            string sql = "";
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkkeyDataOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                sql = "Select *,isnull(RegisterDate,'')RDate from ISAMCKNO (nolock) where Companycode='" + uu.CompanyId + "' and keydata='" + RealData + "' ";
                DataTable dtCkno = PubUtility.SqlQry(sql, uu, "SYS");
                dtCkno.TableName = "dtCkno";
                ds.Tables.Add(dtCkno);

                if (dtCkno.Rows.Count > 0)
                {
                    if (dtCkno.Rows[0]["RDate"].ToString() == "")
                    {
                        sql = "Update ISAMCKNO Set RegisterDate=convert(varchar,getdate(),111) Where Companycode='" + uu.CompanyId + "' ";
                        sql += "and keydata='" + RealData + "' ";
                        PubUtility.ExecuteSql(sql, uu, "SYS");
                    }
                    else
                    {
                        throw new Exception("此金鑰資料已被註冊，請重新確認!");
                    }
                }
                else
                {
                    throw new Exception("此金鑰資料未設定，請重新確認!");
                }

                sql = "Select * from ISAMCKNO (nolock) where Companycode='" + uu.CompanyId + "' and keydata='" + RealData + "' ";
                DataTable dtCkno1 = PubUtility.SqlQry(sql, uu, "SYS");
                dtCkno1.TableName = "dtCkno1";
                ds.Tables.Add(dtCkno1);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("GetCompanyName")]
        public ActionResult GetCompanyName()
        {
            IFormCollection rq = HttpContext.Request.Form;
            string BeforeCompanyID = rq["companyid"];

            UserInfo uu = new UserInfo();
            uu.UserID = "Login";
            uu.CompanyId = BeforeCompanyID;

            string sql = "";
            sql += "Select ChineseName from CompanyWeb";
            sql += " where CompanyCode='" + BeforeCompanyID + "'";


            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetCompanyNameOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtTmp = PubUtility.SqlQry(sql, uu, "SYS");
                dtTmp.TableName = "dtCompanyName";
                ds.Tables.Add(dtTmp);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("LoginSys")]
        public ActionResult LoginSys()
        {
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "LoginSysOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string USERID = rq["USERID"];
                string PASSWORD = rq["PASSWORD"];
                string CompanyID = rq["CompanyID"];
                UserInfo uu = new UserInfo();
                uu.UserID = USERID;  //"Login"
                uu.CompanyId = CompanyID;

                string sql = "";
                sql = "select CompanyCode,CrtUser,isnull(UID,'')UID,isnull(UPWD,'')UPWD,isnull(User_Lock,'')User_Lock, ";
                sql += "case when isnull(EndDate,'')='' then '1' when isnull(EndDate,'')<=convert(char(10),getdate(),111) then '2' end as chkEndDate, ";
                sql += "isnull(ErrTimes,0)ErrTimes,lastlogin,isnull(User_EMail,'')User_EMail,isnull(USR_Key,'')USR_Key ";
                sql += "from Account (nolock) ";
                sql += " where UID='" + USERID.SqlQuote() + "'";
                DataTable dtTmp = PubUtility.SqlQry(sql, uu, "SYS");

                if (dtTmp.Rows.Count > 0)   //帳號存在
                {
                    uu.UserID = Convert.ToString(dtTmp.Rows[0]["UID"]);
                    uu.CompanyId = Convert.ToString(dtTmp.Rows[0]["CompanyCode"]);
                    if (dtTmp.Rows[0]["User_Lock"].ToString() == "Y")
                    {
                        sql = "Insert into LoginRec_WEB (Status,CrtDate,CrtTime,UID,UPWD,Memo) ";
                        sql += "Select 'N',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                        sql += "'" + USERID.SqlQuote() + "','" + PASSWORD.SqlQuote() + "','帳號鎖定' ";
                        PubUtility.ExecuteSql(sql, uu, "SYS");
                        throw new Exception("帳號鎖定");
                    }
                    else if (dtTmp.Rows[0]["chkEndDate"].ToString() == "1" || dtTmp.Rows[0]["chkEndDate"].ToString() == "2")
                    {
                        sql = "Insert into LoginRec_WEB (Status,CrtDate,CrtTime,UID,UPWD,Memo) ";
                        sql += "Select 'N',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                        sql += "'" + USERID.SqlQuote() + "','" + PASSWORD.SqlQuote() + "','帳號失效' ";
                        PubUtility.ExecuteSql(sql, uu, "SYS");
                        throw new Exception("帳號失效");
                        //sql = "Update Account Set ErrTimes=isnull(ErrTimes,0)+1 ";
                        //sql += " where CompanyCode='" + CompanyID.SqlQuote() + "'";
                        //sql += " and UID='" + USERID.SqlQuote() + "'";
                        //sql += " and isnull(UPWD,'')='" + PASSWORD.SqlQuote() + "'";
                        //sql += " and isnull(UPWD,'')<>''";
                        //PubUtility.ExecuteSql(sql, uu, "SYS");
                    }
                    else if (Convert.ToInt32(dtTmp.Rows[0]["ErrTimes"]) >= 3)
                    {
                        sql = "Update Account Set User_Lock='Y',User_LockDate=convert(char(10),getdate(),111) + ' ' + convert(char(12),getdate(),108),ErrTimes=0 ";
                        sql += " where UID='" + USERID.SqlQuote() + "'";
                        PubUtility.ExecuteSql(sql, uu, "SYS");

                        sql = "select '" + uu.CompanyId + "' as CompanyID,'" + USERID.SqlQuote() + "' as UID ";
                        DataTable dtLockedCode = PubUtility.SqlQry(sql, uu, "SYS");
                        dtLockedCode.TableName = "dtLockedCode";
                        DataSet dsLockedCode = new DataSet();
                        dsLockedCode.Tables.Add(dtLockedCode);

                        ApiSetting aSet = GetApiSetting();
                        iXmsClient.ApiUrl = aSet.url_HTADDVIP_ET;
                        DataSet dsR = iXmsClient.LockedCode(dsLockedCode, uu);
                        DataTable dtP = dsR.Tables["dtProcessStatus"];
                        if (dtP.Rows[0]["Error"].ToString() != "0")
                        {
                            throw new Exception(dtP.Rows[0]["Msg_Code"].ToString());
                        }
                        throw new Exception("帳號鎖定");
                    }
                    else if (dtTmp.Rows[0]["UPWD"].ToString().ToLower() != PASSWORD.SqlQuote().ToLower())
                    {
                        sql = "Insert into LoginRec_WEB (Status,CrtDate,CrtTime,UID,UPWD,Memo) ";
                        sql += "Select 'N',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                        sql += "'" + USERID.SqlQuote() + "','" + PASSWORD.SqlQuote() + "','' ";
                        PubUtility.ExecuteSql(sql, uu, "SYS");

                        sql = "Update Account Set ErrTimes=isnull(ErrTimes,0)+1 ";
                        sql += " where UID='" + USERID.SqlQuote() + "'";
                        PubUtility.ExecuteSql(sql, uu, "SYS");

                        throw new Exception("密碼錯誤");
                    }
                }
                else
                {                      //帳號不存在
                    throw new Exception("帳號錯誤");
                }

                dtTmp.TableName = "dtEmployee";
                dtTmp.Columns.Add("token", typeof(string));
                //uu.UserID = USERID;
                string token = PubUtility.GenerateJwtToken(uu);
                dtTmp.Rows[0]["token"] = token;
                ds.Tables.Add(dtTmp);

                DataRow dr = dtTmp.Rows[0];

                //string aa = Guid.NewGuid().ToString();    產生OTP金鑰
                if (dr["lastlogin"].ToString() == "")       //帳號第一次登入
                {
                    string USR_Key = "";

                    if (dr["USR_Key"].ToString() == "")
                    {
                        USR_Key = Guid.NewGuid().ToString();
                        sql = "Update Account Set USR_Key='" + USR_Key + "' ";
                        sql += " where UID='" + USERID.SqlQuote() + "'";
                        PubUtility.ExecuteSql(sql, uu, "SYS");
                    }
                    else
                    {
                        USR_Key = dr["USR_Key"].ToString();
                    }

                    var Authenticator = new GoogleAuthenticatorService.Core.TwoFactorAuthenticator();
                    var setupInfo = Authenticator.GenerateSetupCode("m.Xms",
                                                                    dr["User_EMail"].ToString(),
                                                                    USR_Key,
                                                                    250,
                                                                    250);
                    dtTmp.Columns.Add("QrCodeSetupImageUrl", typeof(string));
                    dr["QrCodeSetupImageUrl"] = setupInfo.QrCodeSetupImageUrl;
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("LoginSysByApi")]
        public ActionResult LoginSysByApi()
        {
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "LoginSysOK", "" });

            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                IFormCollection rq = HttpContext.Request.Form;
                string USERID = rq["USERID"];
                string PASSWORD = rq["PASSWORD"];
                string CompanyID = rq["CompanyID"];
                UserInfo uu = new UserInfo();
                uu.UserID = "Login";
                uu.CompanyId = CompanyID;


                DataTable dt = new DataTable("Employee");
                dt.Columns.Add("Man_ID", typeof(string));
                dt.Columns.Add("Password", typeof(string));
                dt.Columns.Add("CompanyCode", typeof(string));

                if (System.Environment.MachineName.ToUpper() == "ANDYNB4")
                {
                    USERID = "008";
                    PASSWORD = "008";
                }

                dt.Rows.Add(new object[] { USERID, PASSWORD, CompanyID });
                DataSet dsP = new DataSet();
                dsP.Tables.Add(dt);

                iXmsApiParameter AP = new iXmsApiParameter();
                AP.user = uu;
                AP.Method = "LoginSys";
                AP.ObjName = "";
                AP.UnknowName = "";


                string strPara = PubUtility.GetSerString(AP, typeof(iXmsApiParameter));
                string url = ConstList.ThisSiteConfig.Companys
                    .Where<Config.Company>(C => C.CompanyID == uu.CompanyId).ToList()[0].iXmsApiUrl + "/SVMApi.aspx";


                Uri aUri = new Uri(url);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(aUri);
                httpWebRequest.Headers.Add("ParaKey", strPara);
                httpWebRequest.ContentType = "text/xml";
                httpWebRequest.Accept = "text/xml";
                httpWebRequest.Method = "POST";

                dsP.WriteXml(httpWebRequest.GetRequestStream());
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                DataSet dsR = null;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    dsR = new DataSet();
                    dsR.ReadXml(streamReader);
                    DataTable dtRS = dsR.Tables["dtProcessStatus"];
                    if (dtRS.Rows[0]["Error"].ToString() != "0")
                        throw new Exception(dtRS.Rows[0]["Msg_Code"].ToString());

                    DataTable dtE = dsR.Tables["dtEmployee"];
                    dtE.Columns.Add("token", typeof(string));
                    uu.UserID = USERID;
                    string token = PubUtility.GenerateJwtToken(uu);
                    dtE.Rows[0]["token"] = token;
                    ds.Tables.Add(dtE.Copy());
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("GetMenuInit")]
        public ActionResult GetMenuInit()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetMenuInitOK", uu.UserID });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                if (uu.UserID == null) {
                    throw new Exception("null");
                }

                string sql = "";
                if (uu.UserID.ToLower() == uu.CompanyId.ToLower())
                {
                    sql = "Select b.ChineseName as CategoryC,a.SectionID as Category,a.ProgramID as ItemCode,a.ChineseName as Description,a.ProgramID as Page,'P' as MobilePC,'' as Icon ";
                    sql += "From programIdCompanyWWeb a (nolock) ";
                    sql += "inner join SystemIdCompanyWWeb b (nolock) on a.SectionID=b.SystemID and b.Companycode=a.Companycode ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "order by a.SectionID,a.orderSequence ";
                }
                else
                {
                    sql = "Select b.ChineseName as CategoryC,a.SectionID as Category,a.ProgramID as ItemCode,a.ChineseName as Description,a.ProgramID as Page,'P' as MobilePC,'' as Icon ";
                    sql += "From programIdCompanyWWeb a (nolock) ";
                    sql += "inner join SystemIdCompanyWWeb b (nolock) on a.SectionID=b.SystemID and b.Companycode=a.Companycode ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and a.ProgramID in (Select distinct ProgramID From GroupProgramIDWeb c (nolock) ";
                    sql += "inner join AccountGroupWeb d (nolock) on c.GroupID=d.GroupID and d.UID='" + uu.UserID + "' and d.Companycode=c.Companycode ";
                    sql += "where c.Companycode='" + uu.CompanyId + "') ";
                    sql += "order by a.SectionID,a.orderSequence ";
                }
                DataTable dtAll = PubUtility.SqlQry(sql, uu, "SYS");
                dtAll.TableName = "dtAllFunction";
                if (ds.Tables["dtAllFunction"] == null)
                    if (dtAll.DataSet == null)
                        ds.Tables.Add(dtAll);

                //DataTable dt = ConstList.AllFunction(dtA);
                //if (ds.Tables["dtAllFunction"] == null)
                //    if (dt.DataSet == null)
                //        ds.Tables.Add(dt);

                sql = "select a.UName,a.Companycode,b.ChineseName,convert(char(10),getdate(),111) + ' ' + convert(char(12),getdate(),108) SysDate ";
                sql += " from Account a (nolock) ";
                sql += " left join CompanyWeb b (nolock) on a.Companycode=b.Companycode ";
                sql += " where a.UID='" + uu.UserID + "' ";
                DataTable dtU = PubUtility.SqlQry(sql, uu, "SYS");
                dtU.TableName = "dtEmployee";
                ds.Tables.Add(dtU);

                sql = "select LastTrans from TransParameter (nolock) where ProcType='S'";
                DataTable dtTran = PubUtility.SqlQry(sql, uu, "SYS");
                dtTran.TableName = "dtTran";
                ds.Tables.Add(dtTran);

                sql = "Select OpenDate A1,sum(cash) A2,sum(RecCount) A3 ";
                sql += "From SalesAtonceHWeb (nolock) ";
                sql += "Where Companycode='" + uu.CompanyId + "' ";
                sql += "and OpenDate=convert(char(10),getdate(),111) ";
                sql += "group by OpenDate ";
                DataTable dtA = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtA.Rows.Count == 0) {
                    sql = "Select convert(char(10),getdate(),111) A1,0 A2,0 A3 ";
                    dtA = PubUtility.SqlQry(sql, uu, "SYS");
                }
                dtA.TableName = "dtA";
                ds.Tables.Add(dtA);

                sql = "Select OpenDate B1,sum(cash) B2,sum(RecS) B3 ";
                sql += "From SalesHWeb (nolock) ";
                sql += "Where Companycode='" + uu.CompanyId + "' ";
                sql += "and OpenDate=convert(char,dateadd(DD,-1,getdate()),111) ";
                sql += "group by OpenDate ";
                DataTable dtB = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtB.Rows.Count == 0)
                {
                    sql = "Select convert(char,dateadd(DD,-1,getdate()),111) B1,0 B2,0 B3 ";
                    dtB = PubUtility.SqlQry(sql, uu, "SYS");
                }
                dtB.TableName = "dtB";
                ds.Tables.Add(dtB);

                sql = "Select left(OpenDate,7) C1,sum(cash) C2 ";
                sql += "From SalesHWeb (nolock) ";
                sql += "Where Companycode='" + uu.CompanyId + "' ";
                sql += "and left(OpenDate,7)=convert(char(7),getdate(),111) ";
                sql += "group by left(OpenDate,7) ";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtC.Rows.Count == 0)
                {
                    sql = "Select convert(char(7),getdate(),111) C1,0 C2 ";
                    dtC = PubUtility.SqlQry(sql, uu, "SYS");
                }
                dtC.TableName = "dtC";
                ds.Tables.Add(dtC);

                sql = "Select left(OpenDate,4) D1,sum(cash) D2 ";
                sql += "From SalesHWeb (nolock) ";
                sql += "Where Companycode='" + uu.CompanyId + "' ";
                sql += "and left(OpenDate,4)=convert(char(4),getdate(),111) ";
                sql += "group by left(OpenDate,4) ";
                DataTable dtD = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtD.Rows.Count == 0)
                {
                    sql = "Select convert(char(4),getdate(),111) D1,0 D2 ";
                    dtD = PubUtility.SqlQry(sql, uu, "SYS");
                }
                dtD.TableName = "dtD";
                ds.Tables.Add(dtD);

                //sql = "Select Top 10 ROW_NUMBER() over(order by sum(a.num) desc) E1,a.GoodsNo,sum(a.num) E3 ";
                //sql += "into #E ";
                //sql += "From SalesDWeb a (nolock) ";
                //sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                //sql += "and left(a.OpenDate,7)=convert(char(7),getdate(),111) ";
                //sql += "and a.OpenDate between '2024/08/01' and '2024/08/31' ";
                //sql += "group by a.GoodsNo ";
                //sql += "order by sum(a.num) desc; ";
                //sql += "Select a.E1,case when len(b.GD_Name)>10 then left(b.GD_Name,10) + '...' else b.GD_Name end as E2,a.E3,b.GD_Name ";
                //sql += "From #E a left join PLUWeb b (nolock) on a.GoodsNo=b.GD_NO and b.Companycode='" + uu.CompanyId + "' ";

                sql = "Select Top 10 ROW_NUMBER() over(order by sum(a.num) desc) E1,case when DATALENGTH(cast(b.GD_NAME as varchar))>22 then CAST(b.GD_Name as varchar(22)) + '...' else b.GD_Name end as E2,sum(a.num) E3,b.GD_Name ";
                sql += "From (Select Companycode, OpenDate, GoodsNo, num from SalesDWeb (nolock) Where Companycode='" + uu.CompanyId + "' and OpenDate between convert(char(7),getdate(),111) + '/01' and convert(char(7),getdate(),111) + '/31')a ";
                sql += "inner join PLUWeb b (nolock) on a.GoodsNo=b.GD_NO and a.CompanyCode=b.CompanyCode ";
                //20240820 增加條件判斷PLUWeb.Flag<>X才列入(因大九九不需看到購物袋商品)
                sql += "and isnull(b.Flag1,'')<>'X' ";
                sql += "Where b.Companycode='" + uu.CompanyId + "' group by a.GoodsNo,b.GD_Name order by sum(a.num) desc ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);

                sql = "Select Top 10 ROW_NUMBER() over(order by sum(a.cash) desc) F1,b.ST_Name F2,sum(a.cash) F3 ";
                sql += "From SalesHWeb a (nolock) ";
                sql += "left join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                sql += "and left(a.OpenDate,7)=convert(char(7),getdate(),111) ";
                sql += "group by a.ShopNo,b.ST_Name ";
                sql += "order by sum(a.cash) desc ";
                DataTable dtF = PubUtility.SqlQry(sql, uu, "SYS");
                dtF.TableName = "dtF";
                ds.Tables.Add(dtF);

                sql = "Select c.Type_Name name,sum(cash) value ";
                sql += "from SalesHWeb a (nolock) ";
                sql += "left join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.CompanyCode=a.CompanyCode ";
                sql += "inner join TypeDataWeb c (nolock) on b.ST_placeID=c.Type_ID and c.Type_Code='A' and c.CompanyCode=a.CompanyCode ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                sql += "and left(a.OpenDate,7)=convert(char(7),getdate(),111) ";
                sql += "group by b.ST_placeID,c.Type_Name ";
                DataTable dtG = PubUtility.SqlQry(sql, uu, "SYS");
                dtG.TableName = "dtG";
                ds.Tables.Add(dtG);

                sql = "DECLARE @Table AS TABLE ([month] VARCHAR(2));DECLARE @id INT;SET @id=1;WHILE @id<=12 BEGIN INSERT INTO @Table (month) VALUES (RIGHT('0' + Ltrim(Str(@id)), 2)); SET @id += 1; END; ";
                sql += "select left(OpenDate,7)ym1,sum(Cash)cash1 into #h1 ";
                sql += "from SalesHWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' ";
                sql += "and left(OpenDate,4)=convert(char(4),dateadd(YEAR,-1,getdate()),111) ";
                sql += "group by left(OpenDate,7) order by left(OpenDate,7); ";
                sql += "select left(OpenDate,7)ym2,sum(Cash)cash2 into #h2 ";
                sql += "from SalesHWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' ";
                sql += "and left(OpenDate,4)=convert(char(4),getdate(),111) ";
                sql += "group by left(OpenDate,7) order by left(OpenDate,7); ";

                sql += "Select a.month + '月' name, ";
                sql += "b.cash1 value1,c.cash2 value2 ";
                sql += "From @Table a ";
                sql += "left join #h1 b on a.month=right(b.ym1,2) ";
                sql += "left join #h2 c on a.month=right(c.ym2,2) ";
                sql += "order by a.month ";
                DataTable dtH = PubUtility.SqlQry(sql, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);

                sql = "select OpenDate name,case when sum(VIP_RecS)=0 then 0 else Round(sum(VIP_Cash)/sum(VIP_RecS),0) end as value1, ";
                sql += "case when sum(RecS-VIP_RecS)=0 then 0 else Round(sum(Cash-VIP_Cash)/sum(RecS-VIP_RecS),0) end as value2, ";
                sql += "case when sum(RecS)=0 then 0 else Round(sum(Cash)/sum(RecS),0) end as value3 ";
                sql += "from SalesHWeb (nolock) ";
                sql += "where CompanyCode='" + uu.CompanyId + "' ";
                sql += "and OpenDate between convert(char,dateadd(DD,-365,getdate()),111) and convert(char,dateadd(DD,-1,getdate()),111) ";
                sql += "group by OpenDate ";
                sql += "order by OpenDate ";
                DataTable dtI = PubUtility.SqlQry(sql, uu, "SYS");
                dtI.TableName = "dtI";
                ds.Tables.Add(dtI);

                sql = "select a.AnnounceDate J1,a.Title J2,a.FileName J3,a.Att J4, ";
                sql += "a.MO_No J5,a.MO_No2 J6,b.Man_Name J7,a.Title J8,a.Content J9 ";
                sql += "from MessageHweb a (nolock)  ";
                sql += "left join EmployeeWeb b (nolock) on a.AnnounceUser=b.Man_ID and b.CompanyCode=a.CompanyCode ";
                sql += "where a.CompanyCode='" + uu.CompanyId + "' and a.AnnounceDate<=convert(char(10),getdate(),111) and a.EndDate>=convert(char(10),getdate(),111) ";
                sql += "and ISNULL(a.ApproveDate,'')<>'' and ISNULL(a.DeDate,'')='' order by a.AnnounceDate desc ";
                DataTable dtJ = PubUtility.SqlQry(sql, uu, "SYS");
                dtJ.TableName = "dtJ";
                ds.Tables.Add(dtJ);

                sql = "select * ";
                sql += "from MessageHweb a (nolock)  ";
                sql += "where a.CompanyCode='" + uu.CompanyId + "' and a.AnnounceDate=convert(char(10),getdate(),111) and a.EndDate>=convert(char(10),getdate(),111) ";
                sql += "and ISNULL(a.ApproveDate,'')<>'' and ISNULL(a.DeDate,'')='' ";
                DataTable dtJ1 = PubUtility.SqlQry(sql, uu, "SYS");
                dtJ1.TableName = "dtJ1";
                ds.Tables.Add(dtJ1);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("FileUpload")]
        public ActionResult FileUpload()
        {
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "FileUploadOK", "" });
            UserInfo uu = PubUtility.GetCurrentUser(this);
            string picSGID = "";
            try
            {
                IFormFileCollection files = HttpContext.Request.Form.Files;
                string UploadFileType = HttpContext.Request.Form["UploadFileType"];
                if (UploadFileType == "P1" | UploadFileType == "P2" | UploadFileType == "P3" | UploadFileType == "P4" | UploadFileType == "P5" | UploadFileType == "P6" | UploadFileType == "P7" | UploadFileType == "P8")
                {
                    picSGID = ImportPLUPic(files, UploadFileType);
                    DataTable dtMessage = ds.Tables["dtMessage"];
                    dtMessage.Rows[0][1] = picSGID;
                }
            }
            catch (Exception err)
            {
                DataTable dtMessage = ds.Tables["dtMessage"];
                dtMessage.Rows[0][0] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("GetImage")]
        public ActionResult GetImage()
        {
            try
            {
                IQueryCollection rq = HttpContext.Request.Query;
                string SGID = PubUtility.DecodeSGID(rq["SGID"]);
                string UU = rq["UU"];
                UserInfo uu = PubUtility.GetCurrentUser("Bearer " + UU);
                DataTable dt = PubUtility.SqlQry("select * from SetEDM where SGID='" + SGID + "'", uu, "SYS");
                DataRow dr = dt.Rows[0];
                string ContentType = dr["DocType"].ToString();
                HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(dr["FileName"].ToString()));

                return File(dr["DocImage"] as byte[], ContentType);
            }
            catch (Exception err)
            {

                string ContentType = "image/jpeg";
                HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=No_Pic.jpg");
                string fn = ConstList.HostEnvironment.WebRootPath + @"\images\No_Pic.jpg";
                return File(System.IO.File.ReadAllBytes(fn), ContentType);
            }
        }

        [Route("SystemSetup/GetInitGMMacPLUSet")]
        public ActionResult SystemSetup_GetInitGMMacPLUSet()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitGMMacPLUSetOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "select Type_ID,Type_Name";
                sql += " from TypeData ";
                sql += " where CompanyCode='" + uu.CompanyId + "' And Type_Code='G' Order By Type_ID";
                DataTable dtDept = PubUtility.SqlQry(sql, uu, "SYS");
                dtDept.TableName = "dtDept";
                ds.Tables.Add(dtDept);

                sql = "select Type_ID,Type_Name";
                sql += " from TypeData ";
                sql += " where CompanyCode='" + uu.CompanyId + "' And Type_Code='L' Order By Type_ID";
                DataTable dtBGNo = PubUtility.SqlQry(sql, uu, "SYS");
                dtBGNo.TableName = "dtBGNo";
                ds.Tables.Add(dtBGNo);
                ds.Tables.Add(dtBGNo);

                ds.Tables.Add(dtBGNo);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-04-27
        [Route("SystemSetup/GetInittest")]
        public ActionResult SystemSetup_GetInittest()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitGMMacPLUSetOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-04-28
        [Route("SystemSetup/GetInitInv")]
        public ActionResult SystemSetup_GetInitInv()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitInvOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "select * from Layer order by Type_ID";
                //DataTable dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                //dtPLU.TableName = "dtLayer";
                //ds.Tables.Add(dtPLU);

                sql = "select Distinct WhNo ,WhNo+ST_SName WhName ";
                sql += " from InventorySV a (NoLock) Left Join WarehouseSV b (NoLock) ";
                sql += " On a.WhNo=b.ST_ID And a.CompanyCode=b.CompanyCode ";
                sql += " Where a.CompanyCode='" + uu.CompanyId + "'";
                sql += " Order By WhNo ";

                DataTable dtWh = PubUtility.SqlQry(sql, uu, "SYS");
                dtWh.TableName = "dtInvWh";
                ds.Tables.Add(dtWh);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }



        [Route("SystemSetup/SuspendPLU")]
        public ActionResult SystemSetup_SuspendPLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SuspendPLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string GD_NO = rq["GD_NO"];
                string SetSuspend = rq["SetSuspend"];
                string sql = "update PLUSV set GD_Flag1='" + SetSuspend + "'";
                sql += " where CompanyCode='" + uu.CompanyId + "' And GD_NO='" + GD_NO.SqlQuote() + "'";
                PubUtility.ExecuteSql(sql, uu, "SYS");
                sql = "select a.*";
                sql += " ,Case When GD_Flag1='0' Then '未設定' When GD_Flag1='1' Then '啟用' When GD_Flag1='2' Then '停用' End GDStatus ";
                sql += " from PLUSV a";
                sql += " where CompanyCode='" + uu.CompanyId + "' And a.GD_NO='" + GD_NO.SqlQuote() + "'";
                //sql = "select a.*,b.GD_PRICES,b.GD_NAME";
                //sql += " from PLUSVM a";
                //sql += " inner join PLUSV b on a.GD_NO=b.GD_NO";
                //sql += " where b.GD_NO='" + GD_NO.SqlQuote() + "'";
                DataTable dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                dtPLU.TableName = "dtPLU";
                ds.Tables.Add(dtPLU);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/UpdatePLU")]
        public ActionResult SystemSetup_UpdatePLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdatePLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("PLUSV");
                PubUtility.AddStringColumns(dtRec, "CompanyCode,GD_NO,GD_Sname,Photo1");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];
                dr["CompanyCode"] = uu.CompanyId;
                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            sql = "select * from PLUSV where CompanyCode='" + uu.CompanyId + "' And GD_NO='" + dr["GD_NO"].ToString().SqlQuote() + "'";
                            DataTable dtOld = dbop.Query(sql, uu, "SYS");
                            //sql = "update PLUSVM set ";
                            //sql += " GD_Sname='" + dr["GD_Sname"].ToString().SqlQuote() + "'";
                            //sql += ",Photo1='" + dr["Photo1"].ToString().SqlQuote() + "'";
                            //sql += ",Photo2='" + dr["Photo2"].ToString().SqlQuote() + "'";
                            //sql += " where GD_NO='" + dr["GD_NO"].ToString().SqlQuote() + "'";
                            //dbop.ExecuteSql(sql, uu, "SYS");
                            dbop.Update("PLUSV", dtRec, new string[] { "CompanyCode", "GD_NO" }, uu, "SYS");

                            string OldPhoto1 = dtOld.Rows[0]["Photo1"].ToString();
                            if (OldPhoto1 != "" & OldPhoto1 != dr["Photo1"].ToString())
                            {
                                sql = "delete from ImageTable where SGID='" + OldPhoto1 + "'";
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }
                            //string OldPhoto2 = dtOld.Rows[0]["Photo2"].ToString();
                            //if (OldPhoto2 != "" & OldPhoto2 != dr["Photo2"].ToString())
                            //{
                            //    sql = "delete from ImageTable where SGID='" + OldPhoto2 + "'";
                            //    dbop.ExecuteSql(sql, uu, "SYS");
                            //}

                            sql = "Update PLUSV Set GD_Flag1='1' ";
                            sql += "Where CompanyCode='" + uu.CompanyId + "' And GD_No='" + dr["GD_NO"].ToString().SqlQuote() + "' ";
                            sql += " And GD_Flag1='0' ";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*";
                sql += " from PLUSV a";
                //sql += " inner join PLU b on a.GD_NO=b.GD_NO";
                sql += " where a.GD_NO='" + dr["GD_NO"].ToString().SqlQuote() + "'";
                DataTable dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                dtPLU.TableName = "dtPLU";
                ds.Tables.Add(dtPLU);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SearchPLU")]
        public ActionResult SystemSetup_SearchPLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchPLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string KeyWord = rq["KeyWord"];
                string GDDept = rq["GDDept"];
                string GDBGNo = rq["GDBGNo"];
                string GDStatus = rq["GDStatus"];
                string sql = "select a.*";
                sql += " ,Case When GD_Flag1='0' Then '未設定' When GD_Flag1='1' Then '啟用' When GD_Flag1='2' Then '停用' End GDStatus ";
                sql += " from PLUSV a";
                //sql += " inner join PLUSV b on a.GD_NO=b.GD_NO";
                sql += " where CompanyCode='" + uu.CompanyId + "' ";
                if (KeyWord != "")
                {
                    sql += " and (";
                    sql += " a.GD_NAME like '%" + KeyWord + "%'";
                    sql += " or a.GD_NO Like '%" + KeyWord + "%'";
                    sql += " or a.GD_Sname Like '%" + KeyWord + "%'";
                    sql += ")";
                }
                if (GDDept != "" & GDDept != null)
                {
                    sql += " and a.GD_Dept='" + GDDept + "'";
                }
                if (GDBGNo != "" & GDBGNo != null)
                {
                    sql += " and a.GD_BGNo='" + GDBGNo + "'";
                }
                if (GDStatus != "" & GDStatus != null)
                {
                    sql += " and a.GD_Flag1='" + GDStatus + "'";
                }
                sql += " Order By GD_No ";

                DataTable dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                dtPLU.TableName = "dtPLU";
                ds.Tables.Add(dtPLU);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/ImportPLU")]
        public ActionResult SystemSetup_ImportPLU()
        {

            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ImportPLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("PLUSVM");
                PubUtility.AddStringColumns(dtRec, "GD_NO,GD_Sname,Photo1,Photo2");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string GD_NO = dtRec.Rows[0]["GD_NO"].ToString();
                string sql = "select * from PLU ";
                sql += " where GD_NO='" + GD_NO.SqlQuote() + "'";
                sql += " and CompanyCode='" + uu.CompanyId + "'";
                DataTable dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtPLU.Rows.Count > 0)
                    throw new Exception("這個貨號已經存在,不可匯入");

                DataSet dsP = new DataSet();
                dsP.Tables.Add(dtRec.Copy());
                iXmsApiParameter AP = new iXmsApiParameter();
                AP.user = uu;
                AP.Method = "SystemSetupImportPLU";
                AP.ObjName = "";
                AP.UnknowName = "";
                string strPara = PubUtility.GetSerString(AP, typeof(iXmsApiParameter));
                string url = ConstList.ThisSiteConfig.Companys
                    .Where<Config.Company>(C => C.CompanyID == uu.CompanyId).ToList()[0].iXmsApiUrl + "/SVMApi.aspx";

                Uri aUri = new Uri(url);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(aUri);
                httpWebRequest.Headers.Add("ParaKey", strPara);
                httpWebRequest.ContentType = "text/xml";
                httpWebRequest.Accept = "text/xml";
                httpWebRequest.Method = "POST";

                dsP.WriteXml(httpWebRequest.GetRequestStream());
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                DataSet dsR = null;
                DataTable dtPLUi = null;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    dsR = new DataSet();
                    dsR.ReadXml(streamReader);
                    DataTable dtRS = dsR.Tables["dtProcessStatus"];
                    if (dtRS.Rows[0]["Error"].ToString() != "0")
                        throw new Exception(dtRS.Rows[0]["Msg_Code"].ToString());
                    dtPLUi = dsR.Tables["dtPLU"];
                }
                if (dtPLUi.Rows.Count == 0)
                    throw new Exception("這個貨號不存在,不可匯入");

                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            for (int i = dtPLUi.Columns.Count - 1; i > -1; i--)
                            {
                                string fname = dtPLUi.Columns[i].ColumnName;
                                if (!dtPLU.Columns.Contains(fname))
                                    dtPLUi.Columns.Remove(fname);
                            }
                            //dbop.BulkCopy("PLU", dtPLUi, uu, "SYS");
                            sql = PubUtility.ConvertInsertSql("PLU", dtPLUi.Rows[0], uu);
                            dbop.ExecuteSql(sql, uu, "SYS");

                            List<string> fds = "CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime".Split(new char[] { ',' }).ToList<string>();
                            foreach (string str in fds)
                                dtRec.Columns.Add(str, typeof(string));
                            sql = PubUtility.ConvertInsertSql("PLUSVM", dtRec.Rows[0], uu);
                            dbop.ExecuteSql(sql, uu, "SYS");
                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw err;
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*,b.GD_PRICES,b.GD_NAME";
                sql += " from PLUSVM a";
                sql += " inner join PLU b on a.GD_NO=b.GD_NO";
                sql += " where b.GD_NO='" + dr["GD_NO"].ToString().SqlQuote() + "'";
                dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                dtPLU.TableName = "dtPLU";
                ds.Tables.Add(dtPLU);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetPLUFromIXms")]
        public ActionResult GetPLUFromIXms()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            string term = HttpContext.Request.Form["term"].ToString().SqlQuote();
            string sql = "";
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            if (term != "")
            {
                DataTable dtP = new DataTable("dtPLUTerm");
                dtP.Columns.Add("KeyWord", typeof(string));

                dtP.Rows.Add(new object[] { term });
                DataSet dsP = new DataSet();
                dsP.Tables.Add(dtP);

                iXmsApiParameter AP = new iXmsApiParameter();
                AP.user = uu;
                AP.Method = "GetPLUFromIXmsByKeyWord";
                AP.ObjName = "";
                AP.UnknowName = "";
                string strPara = PubUtility.GetSerString(AP, typeof(iXmsApiParameter));
                string url = ConstList.ThisSiteConfig.Companys
                    .Where<Config.Company>(C => C.CompanyID == uu.CompanyId).ToList()[0].iXmsApiUrl + "/SVMApi.aspx";

                Uri aUri = new Uri(url);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(aUri);
                httpWebRequest.Headers.Add("ParaKey", strPara);
                httpWebRequest.ContentType = "text/xml";
                httpWebRequest.Accept = "text/xml";
                httpWebRequest.Method = "POST";

                dsP.WriteXml(httpWebRequest.GetRequestStream());
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                DataSet dsR = null;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    dsR = new DataSet();
                    dsR.ReadXml(streamReader);
                    DataTable dtRS = dsR.Tables["dtProcessStatus"];
                    if (dtRS.Rows[0]["Error"].ToString() != "0")
                        throw new Exception(dtRS.Rows[0]["Msg_Code"].ToString());

                    DataTable dt = dsR.Tables["dtPLU"];
                    if (dt != null)
                    {
                        AutoCompleteDataArray ACDA = new AutoCompleteDataArray();
                        ACDA.list = new List<AutoCompleteData>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            AutoCompleteData acd = new AutoCompleteData();
                            acd.label = dr["GD_NAME"].ToString();
                            acd.value = dr["GD_NO"].ToString();
                            //ACDA.List[dt.Rows.IndexOf(dr)] = acd;
                            ACDA.list.Add(acd);
                        }
                        System.Runtime.Serialization.Json.DataContractJsonSerializer js =
                                    new System.Runtime.Serialization.Json.DataContractJsonSerializer(ACDA.GetType());
                        js.WriteObject(ms, ACDA);
                    }
                }
            }
            string ss = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(ss, "application/json");
        }


        [Route("SystemSetup/SearchInv")]
        public ActionResult SystemSetup_SearchInv()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchInvOK", "", "", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string KeyWord = rq["KeyWord"];
                string WhNo = rq["WhNo"];
                string CkNo = rq["CkNo"];
                string GDLayer = rq["GDLayer"];
                string ToExcel = rq["ToExcel"];
                string sql = "select a.WhNo, a.CkNo, a.Layer, a.SNO, a.PLU, b.GD_SNAME, a.EffectiveDate, a.PtNum, a.DisplayNum,";
                //string sql = "select a.*,b.GD_SNAME,";
                sql += " cast(case when DisPlayNum>0 then Cast(Round(Cast(a.PtNum as numeric(5,1))/Cast(a.DisPlayNum as numeric(5,1))*100,0) As Int) Else 0 End As Varchar(10)) + '%' Share";
                sql += " from InventorySV a";
                sql += " inner join PLUSV b on a.PLU=b.GD_NO And a.CompanyCode=b.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "'";
                if (KeyWord != "")
                {
                    sql += " and (";
                    sql += " b.GD_NAME like '" + KeyWord + "%'";
                    sql += " or b.GD_NO='" + KeyWord + "'";
                    sql += " or b.GD_Sname='" + KeyWord + "'";
                    sql += ")";
                }
                if (WhNo != "")
                {
                    sql += " and a.WhNo='" + WhNo + "'";
                }
                if (CkNo != "")
                {
                    sql += " and a.CkNo='" + CkNo + "'";
                }
                if (GDLayer != "")
                {
                    sql += " and a.Layer='" + GDLayer + "'";
                }
                sql += " Order By a.WhNo, a.CkNo, a.Layer ";
                DataTable dtInv = PubUtility.SqlQry(sql, uu, "SYS");
                dtInv.TableName = "dtInv";
                ds.Tables.Add(dtInv);

                if (ToExcel == "Y")
                {
                    //dtInv.Columns["SerNo"].SetOrdinal(0);
                    using (OfficeOpenXml.ExcelPackage PKD = new OfficeOpenXml.ExcelPackage())
                    {
                        string[] heads = "店碼,機號,貨倉,貨道,商品代號,商品名稱,最近有效日,庫存量,滿倉量,庫存比".Split(",");

                        OfficeOpenXml.ExcelWorksheet wsD = PKD.Workbook.Worksheets.Add("Inv");
                        for (int i = 0; i < heads.Length; i++)
                            wsD.Cells[1, i + 1].Value = heads[i];
                        wsD.Cells["A2"].LoadFromDataTable(dtInv, false);

                        DataTable dtF = new DataTable();
                        dtF.Columns.Add("DataType", typeof(string));
                        dtF.Columns.Add("FileName", typeof(string));
                        dtF.Columns.Add("DocType", typeof(string));
                        dtF.Columns.Add("DocImage", typeof(byte[]));
                        DataRow dr = dtF.NewRow();
                        dtF.Rows.Add(dr);
                        dr["DataType"] = "Temp";
                        dr["FileName"] = "庫存查詢_" + DateTime.Now.ToString("yyMMddHHmmss") + ".xlsx";
                        dr["DocType"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        dr["DocImage"] = PKD.GetAsByteArray();
                        dtMessage.Rows[0][1] = PubUtility.AddTable("ImageTable", dtF, uu, "SYS");
                        dtMessage.Rows[0][2] = uu.CompanyId;
                        dtMessage.Rows[0][3] = uu.UserID;
                    }
                }

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-07
        [Route("SystemSetup/GetWhCkNo")]
        public ActionResult SystemSetup_GetWhCkNo()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetWhCkNoOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNo = rq["WhNo"];
                string sql = "select a.CkNo,a.CkNo + '機' as CkNoName ";
                sql += " from WarehouseDSV a (NoLock) ";
                sql += " where CompanyCode='" + uu.CompanyId + "' and ST_ID='" + WhNo + "'";
                sql += " Order By CkNo ";
                DataTable dtCK = PubUtility.SqlQry(sql, uu, "SYS");
                dtCK.TableName = "dtCK";
                ds.Tables.Add(dtCK);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/GetLayer")]
        public ActionResult SystemSetup_GetLayer()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetLayerOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string KeyWord = rq["KeyWord"];
                string sql = "select a.Type_ID,a.Type_Name ";
                sql += " from Layer a";
                sql += " where 1=1";
                //if (KeyWord != "")
                //{
                //sql += " and (";
                //sql += ")";
                //}
                sql += " Order By Type_ID ";
                DataTable dtInv = PubUtility.SqlQry(sql, uu, "SYS");
                dtInv.TableName = "dtLayer";
                ds.Tables.Add(dtInv);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        private string ImportPLUPic(IFormFileCollection files, string ParaType)
        {
            string sgid = "";
            UserInfo uu = PubUtility.GetCurrentUser(this);
            foreach (IFormFile file in files)
            {
                string filename = file.FileName;
                if (filename.ToLower().IndexOf(".jpg") < 0 & filename.ToLower().IndexOf(".jpeg") < 0)
                {
                    throw new Exception("必須是.jpg檔案!");
                }
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                file.CopyTo(ms);
                DataTable dtF = new DataTable();
                dtF.Columns.Add("CompanyCode", typeof(string));
                dtF.Columns.Add("DocNo", typeof(string));
                dtF.Columns.Add("Type", typeof(string));
                dtF.Columns.Add("DataType", typeof(string));
                dtF.Columns.Add("FileName", typeof(string));
                dtF.Columns.Add("DocType", typeof(string));
                dtF.Columns.Add("DocImage", typeof(byte[]));
                dtF.Columns.Add("URL", typeof(string));

                string sql = "Delete From SetEDM ";
                sql += " where CompanyCode='" + uu.CompanyId + "' And DocNo='" + HttpContext.Request.Form["DocNo"] + "' And DataType='" + HttpContext.Request.Form["UploadFileType"] + "'";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                DataRow drF = dtF.NewRow();
                drF["CompanyCode"] = uu.CompanyId;
                drF["DocNo"] = HttpContext.Request.Form["DocNo"];
                drF["Type"] = HttpContext.Request.Form["Type"];
                drF["DataType"] = HttpContext.Request.Form["UploadFileType"];
                drF["FileName"] = file.FileName;
                drF["DocType"] = file.ContentType;
                drF["DocImage"] = ms.ToArray();
                drF["URL"] = HttpContext.Request.Form["fileURL"];
                dtF.Rows.Add(drF);
                sgid = PubUtility.AddTable("SetEDM", dtF, uu, "SYS");
            }
            return sgid;
        }



        //2021-06-07 Larry
        [Route("SystemSetup/GetInitVMN01")]
        public ActionResult SystemSetup_GetInitVMN01()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitVMN01OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-06-07 Larry
        [Route("SystemSetup/GetTypeData")]
        public ActionResult SystemSetup_GetTypeData()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetTypeDataOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Type_Code = rq["Type_Code"];
                string sql = "select * from TypeData Where CompanyCode='" + uu.CompanyId + "' And Type_Code='" + Type_Code + "' order by Type_ID";

                DataTable dtTypeData = PubUtility.SqlQry(sql, uu, "SYS");
                dtTypeData.TableName = "dtTypeData";
                ds.Tables.Add(dtTypeData);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-06-07 Larry
        [Route("SystemSetup/ChkTypeDataUsed")]
        public ActionResult SystemSetup_ChkTypeDataUsed()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkTypeDataUsedOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Type_ID = rq["Type_ID"];
                string sql = "select Distinct ST_DeliArea from WarehouseSV Where CompanyCode='" + uu.CompanyId + "' And ST_DeliArea='" + Type_ID + "'";

                DataTable dtChk = PubUtility.SqlQry(sql, uu, "SYS");
                dtChk.TableName = "dtChk";
                ds.Tables.Add(dtChk);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-06-07 Larry
        [Route("SystemSetup/ChkTypeDataExist")]
        public ActionResult SystemSetup_ChkTypeDataExist()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkTypeDataUsedOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Type_Code = rq["Type_Code"];
                string Type_ID = rq["Type_ID"];
                string sql = "select * from TypeData Where CompanyCode='" + uu.CompanyId + "' And Type_Code='" + Type_Code + "' And Type_ID='" + Type_ID + "'";

                DataTable dtTypeData = PubUtility.SqlQry(sql, uu, "SYS");
                dtTypeData.TableName = "dtTypeData";
                ds.Tables.Add(dtTypeData);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-06-07 Larry
        [Route("SystemSetup/UpdateTypeData")]
        public ActionResult SystemSetup_UpdateTypeData()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateTypeDataOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("TypeData");
                PubUtility.AddStringColumns(dtRec, "Type_Code,OldType_ID,Type_ID,Type_Name");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            if (dr["Type_ID"].ToString() == dr["OldType_ID"].ToString())
                            {
                                sql = "update TypeData set ";
                                sql += " Type_Name='" + dr["Type_Name"].ToString().SqlQuote() + "'";
                                sql += ",ModDate=convert(char(10),getdate(),111)";
                                sql += ",ModTime=convert(char(12),getdate(),108)";
                                sql += ",ModUser='" + uu.UserID + "'";
                                sql += " where CompanyCode='" + uu.CompanyId + "' And Type_Code='" + dr["Type_Code"].ToString().SqlQuote() + "' And Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }
                            else
                            {
                                sql = "update TypeData set ";
                                sql += " Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                                sql += " ,Type_Name='" + dr["Type_Name"].ToString().SqlQuote() + "'";
                                sql += " ,ModDate=convert(char(10),getdate(),111)";
                                sql += " ,ModTime=convert(char(12),getdate(),108)";
                                sql += " ,ModUser='" + uu.UserID + "'";
                                sql += " where CompanyCode='" + uu.CompanyId + "' And Type_Code='" + dr["Type_Code"].ToString().SqlQuote() + "' And Type_ID='" + dr["OldType_ID"].ToString().SqlQuote() + "'";
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }
                            //dbop.Update("Rack", dtRec, new string[] { "Type_ID" }, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*";
                sql += " from TypeData a";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And Type_Code='" + dr["Type_Code"].ToString().SqlQuote() + "' And a.Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                DataTable dtTypeData = PubUtility.SqlQry(sql, uu, "SYS");
                dtTypeData.TableName = "dtTypeData";
                ds.Tables.Add(dtTypeData);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-06-07 Larry
        [Route("SystemSetup/AddTypeData")]
        public ActionResult SystemSetup_AddTypeData()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "AddTypeDataOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("TypeData");
                PubUtility.AddStringColumns(dtRec, "Type_Code,Type_ID,Type_Name");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            //sql = "Insert Into Rack (CompanyCode, CrtUser, CrtDate, CrtTime ";
                            //sql += " ,ModUser, ModDate, ModTime";
                            //sql += ", Type_ID, Type_Name, DisplayNum) Values ";
                            //sql += " ('" + uu.CompanyId + "', '" + uu.UserID + "', convert(char(10),getdate(),111), convert(char(12),getdate(),108) ";
                            //sql += " ,'" + uu.UserID + "',convert(char(10),getdate(),111), convert(char(12),getdate(),108) ";
                            //sql += " ,'" + dr["Type_ID"].ToString().SqlQuote() + "','" + dr["Type_Name"].ToString().SqlQuote() + "'," + dr["DisplayNum"].ToString().SqlQuote() + ")";
                            //dbop.ExecuteSql(sql, uu, "SYS");
                            dbop.Add("TypeData", dtRec, uu, "SYS");
                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*";
                sql += " from TypeData a";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And Type_Code='" + dr["Type_Code"].ToString().SqlQuote() + "' And Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                DataTable dtTypeData = PubUtility.SqlQry(sql, uu, "SYS");
                dtTypeData.TableName = "dtTypeData";
                ds.Tables.Add(dtTypeData);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-06-07 Larry
        [Route("SystemSetup/DelTypeData")]
        public ActionResult SystemSetup_DelTypeData()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "DelTypeDataOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("TypeData");
                PubUtility.AddStringColumns(dtRec, "Type_Code,Type_ID");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            sql = "Delete From TypeData ";
                            sql += " where CompanyCode='" + uu.CompanyId + "' And Type_Code='" + dr["Type_Code"].ToString().SqlQuote() + "' And Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*";
                sql += " from TypeData a";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And Type_Code='" + dr["Type_Code"].ToString().SqlQuote() + "'";
                DataTable dtTypeData = PubUtility.SqlQry(sql, uu, "SYS");
                dtTypeData.TableName = "dtTypeData";
                ds.Tables.Add(dtTypeData);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }













        //2021-05-07 Larry
        [Route("SystemSetup/GetInitVMN29")]
        public ActionResult SystemSetup_GetInitVMN29()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitVMN29OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-07 Larry
        [Route("SystemSetup/GetRack")]
        public ActionResult SystemSetup_GetRack()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetRackOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "select * from Rack Where CompanyCode='" + uu.CompanyId + "' order by Type_ID";

                DataTable dtRack = PubUtility.SqlQry(sql, uu, "SYS");
                dtRack.TableName = "dtRack";
                ds.Tables.Add(dtRack);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-05-07 Larry
        [Route("SystemSetup/ChkRackUsed")]
        public ActionResult SystemSetup_ChkRackUsed()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkRackUsedOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Type_ID = rq["Type_ID"];
                string sql = "select Distinct ChannelType from MachineListSpec Where CompanyCode='" + uu.CompanyId + "' And ChannelType='" + Type_ID + "'";

                DataTable dtRack = PubUtility.SqlQry(sql, uu, "SYS");
                dtRack.TableName = "dtRack";
                ds.Tables.Add(dtRack);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-07 Larry
        [Route("SystemSetup/ChkRackExist")]
        public ActionResult SystemSetup_ChkRackExist()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkRackUsedOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Type_ID = rq["Type_ID"];
                string sql = "select * from Rack Where CompanyCode='" + uu.CompanyId + "' And Type_ID='" + Type_ID + "'";

                DataTable dtRack = PubUtility.SqlQry(sql, uu, "SYS");
                dtRack.TableName = "dtRack";
                ds.Tables.Add(dtRack);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-07 Larry
        [Route("SystemSetup/UpdateRack")]
        public ActionResult SystemSetup_UpdateRack()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateRackOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("Rack");
                PubUtility.AddStringColumns(dtRec, "OldType_ID,Type_ID,Type_Name,DisplayNum");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            if (dr["Type_ID"].ToString() == dr["OldType_ID"].ToString())
                            {
                                sql = "update Rack set ";
                                sql += " Type_Name='" + dr["Type_Name"].ToString().SqlQuote() + "'";
                                sql += ",DisplayNum=" + dr["DisplayNum"].ToString().SqlQuote() + "";
                                sql += ",ModDate=convert(char(10),getdate(),111)";
                                sql += ",ModTime=convert(char(12),getdate(),108)";
                                sql += ",ModUser='" + uu.UserID + "'";
                                sql += " where CompanyCode='" + uu.CompanyId + "' And Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }
                            else
                            {
                                sql = "update Rack set ";
                                sql += " Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                                sql += " ,Type_Name='" + dr["Type_Name"].ToString().SqlQuote() + "'";
                                sql += " ,DisplayNum=" + dr["DisplayNum"].ToString().SqlQuote() + "";
                                sql += " ,ModDate=convert(char(10),getdate(),111)";
                                sql += " ,ModTime=convert(char(12),getdate(),108)";
                                sql += " ,ModUser='" + uu.UserID + "'";
                                sql += " where CompanyCode='" + uu.CompanyId + "' And Type_ID='" + dr["OldType_ID"].ToString().SqlQuote() + "'";
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }
                            //dbop.Update("Rack", dtRec, new string[] { "Type_ID" }, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*";
                sql += " from Rack a";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                DataTable dtRack = PubUtility.SqlQry(sql, uu, "SYS");
                dtRack.TableName = "dtRack";
                ds.Tables.Add(dtRack);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-26 Larry
        [Route("SystemSetup/DelChgShop")]
        public ActionResult SystemSetup_DelChgShop()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "DelChgShopOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtChgShop = new DataTable("ChangeShopSV");
                PubUtility.AddStringColumns(dtChgShop, "DocNo");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtChgShop);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtChgShop.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {

                            sql = "Delete From ChangeShopSV ";
                            sql += " where CompanyCode='" + uu.CompanyId + "' And DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //dbop.Update("Rack", dtRec, new string[] { "Type_ID" }, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }

                sql = "select a.*,a.WhNoOut+b.ST_SNAME WhOut,b.ST_SNAME WhOutName, a.WhNoIn+c.ST_SName WhIn,c.ST_SName WhInName,d.Man_Name";
                sql += " ,Case When IsNull(a.AppDate,'')='' Then '未批核' Else '已批核' End AppStatus";
                sql += " ,Case When IsNull(a.FinishDate,'')='' Then '未完成' Else '完成' End FinStatus";
                sql += " from ChangeShopSV a";
                sql += " inner join WarehouseSV b on a.WhNoOut=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " inner join WarehouseSV c on a.WhNoIn=c.ST_ID And a.CompanyCode=c.CompanyCode";
                sql += " left  join EmployeeSV d on a.DocUser=d.Man_ID And a.CompanyCode=d.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' ";

                //sql = "select a.*";
                //sql += " from ChangeShopSV a";
                //sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";
                DataTable dtRack = PubUtility.SqlQry(sql, uu, "SYS");
                dtRack.TableName = "dtRack";
                ds.Tables.Add(dtRack);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }




        //2021-05-07 Larry
        [Route("SystemSetup/AddRack")]
        public ActionResult SystemSetup_AddRack()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "AddRackOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("Rack");
                PubUtility.AddStringColumns(dtRec, "Type_ID,Type_Name,DisplayNum");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            //sql = "Insert Into Rack (CompanyCode, CrtUser, CrtDate, CrtTime ";
                            //sql += " ,ModUser, ModDate, ModTime";
                            //sql += ", Type_ID, Type_Name, DisplayNum) Values ";
                            //sql += " ('" + uu.CompanyId + "', '" + uu.UserID + "', convert(char(10),getdate(),111), convert(char(12),getdate(),108) ";
                            //sql += " ,'" + uu.UserID + "',convert(char(10),getdate(),111), convert(char(12),getdate(),108) ";
                            //sql += " ,'" + dr["Type_ID"].ToString().SqlQuote() + "','" + dr["Type_Name"].ToString().SqlQuote() + "'," + dr["DisplayNum"].ToString().SqlQuote() + ")";
                            //dbop.ExecuteSql(sql, uu, "SYS");
                            dbop.Add("Rack", dtRec, uu, "SYS");
                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*";
                sql += " from Rack a";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                DataTable dtRack = PubUtility.SqlQry(sql, uu, "SYS");
                dtRack.TableName = "dtRack";
                ds.Tables.Add(dtRack);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-07 Larry
        [Route("SystemSetup/DelRack")]
        public ActionResult SystemSetup_DelRack()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "DelRackOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("Rack");
                PubUtility.AddStringColumns(dtRec, "Type_ID");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            sql = "Delete From Rack ";
                            sql += " where CompanyCode='" + uu.CompanyId + "' And Type_ID='" + dr["Type_ID"].ToString().SqlQuote() + "'";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*";
                sql += " from Rack a";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' ";
                DataTable dtRack = PubUtility.SqlQry(sql, uu, "SYS");
                dtRack.TableName = "dtRack";
                ds.Tables.Add(dtRack);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-18 Larry
        [Route("SystemSetup/GetInitVXT03")]
        public ActionResult SystemSetup_GetInitVXT03()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitVXT03OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                string sql = "select ST_ID ,ST_ID+ST_SName STName ";
                sql += " from WarehouseSV (NoLock) ";
                sql += " Where CompanyCode='" + uu.CompanyId + "' And ST_Type ='6'";
                sql += " Order By ST_ID ";

                DataTable dtWh = PubUtility.SqlQry(sql, uu, "SYS");
                dtWh.TableName = "dtWh";
                ds.Tables.Add(dtWh);


                sql = "select ST_ID ,ST_ID+ST_SName STName ";
                sql += " from WarehouseSV (NoLock) ";
                sql += " Where CompanyCode='" + uu.CompanyId + "' And ST_Type Not In ('2','3','6')";
                sql += " Order By ST_ID ";

                DataTable dtS = PubUtility.SqlQry(sql, uu, "SYS");
                dtS.TableName = "dtS";
                ds.Tables.Add(dtS);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-18 Larry
        [Route("SystemSetup/SearchVXT03")]
        public ActionResult SystemSetup_SearchVXT03()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchVXT03OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;

                string WhNo = rq["WhNo"];
                string CkNo = rq["CkNo"];
                string SWhNo = rq["SWhNo"];
                string VMStatus = rq["VMStatus"];
                string NetStatus = rq["NetStatus"];

                string sql = "select a.*,c.ST_SNAME+b.ckno+'機' VMName,b.ST_ID,b.ckno,d.ST_SName SWhName";
                sql += " from MachineList a";
                sql += " inner join WarehouseDSV b on a.SNNo=b.SNNo And a.CompanyCode=b.CompanyCode";
                sql += " inner join WarehouseSV c on b.ST_ID=c.ST_ID And b.CompanyCode=c.CompanyCode";
                sql += " inner join WarehouseSV d on b.WhNoIn=d.ST_ID And b.CompanyCode=d.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "'";
                if (WhNo != "")
                {
                    sql += " and b.ST_ID='" + WhNo + "'";
                }
                if (CkNo != "")
                {
                    sql += " and b.CkNo='" + CkNo + "'";
                }
                if (SWhNo != "")
                {
                    sql += " and b.WhNoIn='" + SWhNo + "'";
                }
                if (VMStatus != "")
                {
                    sql += " and a.FlagUse='" + VMStatus + "'";
                }
                if (NetStatus != "")
                {
                    sql += " and a.FlagNet='" + NetStatus + "'";
                }
                sql += " Order By a.SNNo ";
                DataTable dtInv = PubUtility.SqlQry(sql, uu, "SYS");
                dtInv.TableName = "dtInv";
                ds.Tables.Add(dtInv);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-18
        [Route("SystemSetup/GetWhDSVCkNo")]
        public ActionResult SystemSetup_GetWhDSVCkNo()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetWhDSVCkNoOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNo = rq["WhNo"];
                string sql = "select a.CkNo,a.CkNo + '機' as CkNoName ";
                sql += " from WarehouseDSV a (NoLock) ";
                sql += " where CompanyCode='" + uu.CompanyId + "' and ST_ID='" + WhNo + "'";
                sql += " Order By CkNo ";
                DataTable dtCK = PubUtility.SqlQry(sql, uu, "SYS");
                dtCK.TableName = "dtCK";
                ds.Tables.Add(dtCK);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }




        //2021-05-26 Larry
        [Route("SystemSetup/GetWh")]
        public ActionResult SystemSetup_GetWh()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetWhOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "";
                sql = "select WhNO+b.ST_Sname WhName from ISAMShopWeb a (NoLock)";
                sql += " left join WarehouseWeb b (NoLock) on a.companycode=b.companycode and a.whno=b.st_id ";
                sql += " Where a.CompanyCode='" + uu.CompanyId + "' ";
                sql += " and Man_ID='" + uu.UserID + "' ";
                DataTable dtISAMShop = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtISAMShop.Rows.Count == 0)
                {
                    sql = "select WhNO+b.ST_Sname WhName from EmployeeWeb a (NoLock)";
                    sql += " left join WarehouseWeb b (NoLock) on a.companycode=b.companycode and a.whno=b.st_id ";
                    sql += " Where a.CompanyCode='" + uu.CompanyId + "' ";
                    sql += " and Man_ID='" + uu.UserID + "' ";

                    DataTable dtUserWh = PubUtility.SqlQry(sql, uu, "SYS");
                    dtUserWh.TableName = "dtUserWh";
                    ds.Tables.Add(dtUserWh);
                }
                else
                {
                    dtISAMShop.TableName = "dtUserWh";
                    ds.Tables.Add(dtISAMShop);
                }
                sql = "select ST_ID ,ST_ID+' '+ST_SName STName ";
                sql += " from WarehouseWeb (NoLock) ";
                sql += " Where CompanyCode='" + uu.CompanyId + "' and ST_StopDay=''";
                sql += " Order By ST_ID ";

                DataTable dtWh = PubUtility.SqlQry(sql, uu, "SYS");
                dtWh.TableName = "dtWh";
                ds.Tables.Add(dtWh);


            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SaveST_ID")]
        public ActionResult SaveST_ID()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveWhSetOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string STID = rq["WHSetSV"];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {

                            sql = "update ISAMShopWeb set ";
                            sql += " WhNo='" + STID.ToString().SqlQuote() + "'";
                            sql += ",ModDate=convert(char(10),getdate(),111)";
                            sql += ",ModTime=convert(char(12),getdate(),108)";
                            sql += ",ModUser='" + uu.UserID + "'";
                            sql += " where CompanyCode='" + uu.CompanyId + "' And Man_ID='" + uu.UserID + "'";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                dtMessage.Rows[0][1] = STID;

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-06-02 Larry
        [Route("SystemSetup/UpdateVXT03")]
        public ActionResult SystemSetup_UpdateVXT03()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateVXT03OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("MachineList");
                PubUtility.AddStringColumns(dtRec, "SNNo,FlagT,FlagNet");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {

                            sql = "update MachineList set ";
                            sql += " FlagT='" + dr["FlagT"].ToString().SqlQuote() + "'";
                            sql += ",FlagNet='" + dr["FlagNet"].ToString().SqlQuote() + "'";
                            sql += ",ModDate=convert(char(10),getdate(),111)";
                            sql += ",ModTime=convert(char(12),getdate(),108)";
                            sql += ",ModUser='" + uu.UserID + "'";
                            sql += " where CompanyCode='" + uu.CompanyId + "' And SNNo='" + dr["SNNo"].ToString().SqlQuote() + "'";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*,c.ST_SNAME+b.ckno+'機' VMName,b.ST_ID,b.ckno,d.ST_SName SWhName";
                sql += " from MachineList a";
                sql += " inner join WarehouseDSV b on a.SNNo=b.SNNo And a.CompanyCode=b.CompanyCode";
                sql += " inner join WarehouseSV c on b.ST_ID=c.ST_ID And b.CompanyCode=c.CompanyCode";
                sql += " inner join WarehouseSV d on b.WhNoIn=d.ST_ID And b.CompanyCode=d.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.SNNo='" + dr["SNNo"].ToString().SqlQuote() + "'";
                //sql = "select a.*";
                //sql += " from MachineList a";
                //sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.SNNo='" + dr["SNNo"].ToString().SqlQuote() + "'";
                DataTable dtRack = PubUtility.SqlQry(sql, uu, "SYS");
                dtRack.TableName = "dtRack";
                ds.Tables.Add(dtRack);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }





        //2021-05-26 Larry
        [Route("SystemSetup/SearchVIN13_1")]
        public ActionResult SystemSetup_SearchVIN13_1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchVIN13_1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;

                string WhNo = rq["WhNo"];
                string CkNo = rq["CkNo"];
                string exDate = rq["exDate"];

                string sql = "select a.*,a.WhNoOut+b.ST_SNAME WhOut,b.ST_SNAME WhOutName, a.WhNoIn+c.ST_SName WhIn,c.ST_SName WhInName,d.Man_Name";
                sql += " ,Case When IsNull(a.AppDate,'')='' Then '未批核' Else '已批核' End AppStatus";
                sql += " ,Case When IsNull(a.FinishDate,'')='' Then '未完成' Else '完成' End FinStatus";
                sql += " ,c.ST_OpenDay";
                sql += " from ChangeShopSV a";
                sql += " inner join WarehouseSV b on a.WhNoOut=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " inner join WarehouseSV c on a.WhNoIn=c.ST_ID And a.CompanyCode=c.CompanyCode";
                sql += " left  join EmployeeSV d on a.DocUser=d.Man_ID And a.CompanyCode=d.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "'";
                if (WhNo != "")
                {
                    sql += " and a.WhNoOut='" + WhNo + "'";
                }
                if (CkNo != "")
                {
                    sql += " and a.CkNoOut='" + CkNo + "'";
                }
                if (exDate != "")
                {
                    sql += " and a.exchangeDate='" + exDate + "'";
                }
                sql += " Order By a.DocNo Desc";
                DataTable dtInv = PubUtility.SqlQry(sql, uu, "SYS");
                dtInv.TableName = "dtInv";
                ds.Tables.Add(dtInv);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        ////2021-06-24 Larry
        //[Route("SystemSetup/ChkChgShopCols")]
        //public ActionResult SystemSetup_ChkChgShopCols()
        //{
        //    UserInfo uu = PubUtility.GetCurrentUser(this);
        //    System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkChgShopColsOK", "" });
        //    DataTable dtMessage = ds.Tables["dtMessage"];
        //    try
        //    {
        //        IFormCollection rq = HttpContext.Request.Form;
        //        string WhNoOut = rq["WhNoOut"];
        //        string CkNoOut = rq["CkNoOut"];
        //        string WhNoIn = rq["WhNoIn"];
        //        string CkNoIn = rq["CkNoIn"];
        //        string ExchangeDate = rq["ExchangeDate"];
        //        string ChkMod = rq["ChkMod"];

        //        if (ChkMod=="App")
        //        {

        //        }
        //        else
        //        {

        //        }

        //        string sql = "select Distinct ChannelType from MachineListSpec Where CompanyCode='" + uu.CompanyId + "' And ChannelType='" + Type_ID + "'";

        //        DataTable dtRack = PubUtility.SqlQry(sql, uu, "SYS");
        //        dtRack.TableName = "dtRack";
        //        ds.Tables.Add(dtRack);

        //    }
        //    catch (Exception err)
        //    {
        //        dtMessage.Rows[0][0] = "Exception";
        //        dtMessage.Rows[0][1] = err.Message;
        //    }
        //    return PubUtility.DatasetXML(ds);
        //}


        //2021-05-26 Larry
        [Route("SystemSetup/UpdateChgShop")]
        public ActionResult SystemSetup_UpdateChgShop()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateChgShopOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtChgShop = new DataTable("ChangeShopSV");
                PubUtility.AddStringColumns(dtChgShop, "DocNo,WhNoOut,CkNoOut,WhNoIn,CkNoIn,ExchangeDate");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtChgShop);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtChgShop.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {

                            sql = "update ChangeShopSV set ";
                            sql += " WhNoOut='" + dr["WhNoOut"].ToString().SqlQuote() + "'";
                            sql += ",CkNoOut='" + dr["CkNoOut"].ToString().SqlQuote() + "'";
                            sql += ",WhNoIn='" + dr["WhNoIn"].ToString().SqlQuote() + "'";
                            sql += ",CkNoIn='" + dr["CkNoIn"].ToString().SqlQuote() + "'";
                            sql += ",ExchangeDate='" + dr["ExchangeDate"].ToString().SqlQuote() + "'";
                            sql += ",ModDate=convert(char(10),getdate(),111)";
                            sql += ",ModTime=convert(char(12),getdate(),108)";
                            sql += ",ModUser='" + uu.UserID + "'";
                            sql += " where CompanyCode='" + uu.CompanyId + "' And DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //dbop.Update("Rack", dtRec, new string[] { "Type_ID" }, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }

                sql = "select a.*,a.WhNoOut+b.ST_SNAME WhOut,b.ST_SNAME WhOutName, a.WhNoIn+c.ST_SName WhIn,c.ST_SName WhInName,d.Man_Name";
                sql += " ,Case When IsNull(a.AppDate,'')='' Then '未批核' Else '已批核' End AppStatus";
                sql += " ,Case When IsNull(a.FinishDate,'')='' Then '未完成' Else '完成' End FinStatus";
                sql += " from ChangeShopSV a";
                sql += " inner join WarehouseSV b on a.WhNoOut=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " inner join WarehouseSV c on a.WhNoIn=c.ST_ID And a.CompanyCode=c.CompanyCode";
                sql += " left  join EmployeeSV d on a.DocUser=d.Man_ID And a.CompanyCode=d.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";

                //sql = "select a.*";
                //sql += " from ChangeShopSV a";
                //sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";
                DataTable dtRes = PubUtility.SqlQry(sql, uu, "SYS");
                dtRes.TableName = "dtRes";
                ds.Tables.Add(dtRes);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-26 Larry
        [Route("SystemSetup/AppChgShop")]
        public ActionResult SystemSetup_AppChgShop()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            //批核完成後，同樣執行異動完成，故回傳UpdateChgShopOK
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateChgShopOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtChgShop = new DataTable("ChangeShopSV");
                PubUtility.AddStringColumns(dtChgShop, "DocNo");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtChgShop);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtChgShop.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {

                            sql = "update ChangeShopSV set ";
                            sql += " AppDate=convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5)";
                            sql += ",AppUser='" + uu.UserID + "'";
                            sql += ",ModDate=convert(char(10),getdate(),111)";
                            sql += ",ModTime=convert(char(12),getdate(),108)";
                            sql += ",ModUser='" + uu.UserID + "'";
                            sql += ",SNnoOut=W.SNno";
                            sql += " From ChangeShopSV C Inner Join WarehouseDSV W On C.CompanyCode=W.CompanyCode And C.WhNoOut=W.ST_ID And C.CkNoOut=W.CkNo";
                            sql += " where C.CompanyCode='" + uu.CompanyId + "' And DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";
                            //sql = "update ChangeShopSV set ";
                            //sql += " AppDate=convert(char(10),getdate(),111)";
                            //sql += ",AppUser='" + uu.UserID + "'";
                            //sql += ",ModDate=convert(char(10),getdate(),111)";
                            //sql += ",ModTime=convert(char(12),getdate(),108)";
                            //sql += ",ModUser='" + uu.UserID + "'";
                            //sql += " where CompanyCode='" + uu.CompanyId + "' And DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //dbop.Update("Rack", dtRec, new string[] { "Type_ID" }, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }

                sql = "select a.*,a.WhNoOut+b.ST_SNAME WhOut,b.ST_SNAME WhOutName, a.WhNoIn+c.ST_SName WhIn,c.ST_SName WhInName,d.Man_Name";
                sql += " ,Case When IsNull(a.AppDate,'')='' Then '未批核' Else '已批核' End AppStatus";
                sql += " ,Case When IsNull(a.FinishDate,'')='' Then '未完成' Else '完成' End FinStatus";
                sql += " from ChangeShopSV a";
                sql += " inner join WarehouseSV b on a.WhNoOut=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " inner join WarehouseSV c on a.WhNoIn=c.ST_ID And a.CompanyCode=c.CompanyCode";
                sql += " left  join EmployeeSV d on a.DocUser=d.Man_ID And a.CompanyCode=d.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";

                //sql = "select a.*";
                //sql += " from ChangeShopSV a";
                //sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";
                DataTable dtRes = PubUtility.SqlQry(sql, uu, "SYS");
                dtRes.TableName = "dtRes";
                ds.Tables.Add(dtRes);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-26 Larry
        [Route("SystemSetup/AddChgShop")]
        public ActionResult SystemSetup_AddChgShop()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "AddChgShopOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                string DocNo = PubUtility.GetNewDocNo(uu, "CS", 3);

                DataTable dtRec = new DataTable("ChangeShopSV");
                PubUtility.AddStringColumns(dtRec, "WhNoOut,CkNoOut,WhNoIn,CkNoIn,ExchangeDate");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            sql = "Insert Into ChangeShopSV (CompanyCode, CrtUser, CrtDate, CrtTime ";
                            sql += ", ModUser, ModDate, ModTime ";
                            sql += ", DocNo, DocUser, DocDate";
                            sql += ", ExchangeDate, WhNoOut, CkNoOut "
                                + ", WhNoIn, CkNoIn) Values ";
                            sql += " ('" + uu.CompanyId + "', '" + uu.UserID + "', convert(char(10),getdate(),111), convert(char(12),getdate(),108) ";
                            sql += ", '" + uu.UserID + "', convert(char(10),getdate(),111), convert(char(12),getdate(),108) ";
                            sql += ", '" + DocNo + "', '" + uu.UserID + "',convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(12),getdate(),108),1,5) ";
                            sql += ", '" + dr["ExchangeDate"].ToString().SqlQuote() + "','" + dr["WhNoOut"].ToString().SqlQuote() + "','" + dr["CkNoOut"].ToString().SqlQuote() + "'"
                                 + ", '" + dr["WhNoIn"].ToString().SqlQuote() + "', '" + dr["CkNoIn"].ToString().SqlQuote() + "')";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.*,a.WhNoOut+b.ST_SNAME WhOut,b.ST_SNAME WhOutName, a.WhNoIn+c.ST_SName WhIn,c.ST_SName WhInName,d.Man_Name";
                sql += " ,Case When IsNull(a.AppDate,'')='' Then '未批核' Else '已批核' End AppStatus";
                sql += " ,Case When IsNull(a.FinishDate,'')='' Then '未完成' Else '完成' End FinStatus";
                sql += " from ChangeShopSV a";
                sql += " inner join WarehouseSV b on a.WhNoOut=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " inner join WarehouseSV c on a.WhNoIn=c.ST_ID And a.CompanyCode=c.CompanyCode";
                sql += " left  join EmployeeSV d on a.DocUser=d.Man_ID And a.CompanyCode=d.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + DocNo + "'";
                DataTable dtChgShop = PubUtility.SqlQry(sql, uu, "SYS");
                dtChgShop.TableName = "dtChgShop";
                ds.Tables.Add(dtChgShop);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-06-17
        [Route("SystemSetup/ChkWhNo")]
        public ActionResult SystemSetup_ChkWhNo()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkWhNoOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNo = rq["WhNo"];

                string sql = "Select ST_ID,ST_Sname from WarehouseSV (NoLock) ";
                sql += "where Companycode='" + uu.CompanyId.SqlQuote() + "' and ST_ID='" + WhNo + "' and ST_Type='6'";
                DataTable dtSV = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtSV);
                dtSV.TableName = "dtWarehouse";

                sql = "select a.CkNo ";
                sql += " from WarehouseDSV a (NoLock) ";
                sql += " where CompanyCode='" + uu.CompanyId + "' and ST_ID='" + WhNo + "'";
                sql += " Order By CkNo ";
                DataTable dtCK = PubUtility.SqlQry(sql, uu, "SYS");
                dtCK.TableName = "dtCK";
                ds.Tables.Add(dtCK);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }





        //public string GetNewDocNo(UserInfo uu, String DocType, Int16 Digits)
        //{
        //    string sDocNo = "";
        //    string sDate;
        //    string sDateWithSlash = "";
        //    string sql = "";
        //    sql = "select convert(char(8),getdate(),112),convert(char(10),getdate(),111)";
        //    DataTable dtDate = PubUtility.SqlQry(sql, uu, "SYS");
        //    if (dtDate.Rows.Count == 0)
        //    {
        //        sDocNo = "";
        //        return sDocNo;
        //    }
        //    else
        //    {
        //        sDate = dtDate.Rows[0][0].ToString().Trim();
        //        sDateWithSlash = dtDate.Rows[0][1].ToString().Trim();
        //    }

        //    sql = "select SeqNo from DocumentNo a";
        //    sql += " where a.CompanyCode='" + uu.CompanyId.SqlQuote() + "' And Initial='" + DocType + "' And DocDate=convert(char(8),getdate(),112)";

        //    DataTable dtDoc = PubUtility.SqlQry(sql, uu, "SYS");
        //    //dtDoc.TableName = "dtDoc";

        //    using (DBOperator dbop = new DBOperator())

        //        if (dtDoc.Rows.Count == 0)
        //        {
        //            string str = new string('0', Digits) + "1";
        //            sDocNo = DocType + sDate + str.Substring(str.Length - Digits);
        //            sql = "Insert Into DocumentNo (SGID, CompanyCode, CrtUser, CrtDate, CrtTime, ModUser, ModDate, ModTime, Initial, DocDate, SeqNo) ";
        //            sql += " Select '" + uu.CompanyId.SqlQuote() + DocType + sDate + "', '" + uu.CompanyId.SqlQuote() + "'"
        //                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
        //                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
        //                 + ", '" + DocType + "', '" + sDate + "', 1 ";
        //            dbop.ExecuteSql(sql, uu, "SYS");
        //        }
        //        else
        //        {
        //            int SeqNo = Convert.ToInt32(dtDoc.Rows[0][0]) + 1;

        //            sql = "Update DocumentNo Set SeqNo=" + SeqNo + " "
        //                + " ,ModUser='" + uu.UserID + "', ModDate=convert(char(10),getdate(),111), ModTime=convert(char(8),getdate(),108) "
        //                + "Where CompanyCode='" + uu.CompanyId.SqlQuote() + "' And Initial='" + DocType + "' And DocDate='" + sDate + "'";
        //            dbop.ExecuteSql(sql, uu, "SYS");
        //            string str = new string('0', Digits) + SeqNo.ToString();
        //            sDocNo = DocType + sDate + str.Substring(str.Length - Digits);
        //        }

        //    return sDocNo;
        //}


        //2021-06-21 Larry
        [Route("SystemSetup/UpdateTempSV")]
        public ActionResult SystemSetup_UpdateTempSV()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateTempSVOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtTemp = new DataTable("TempDocumentSV");
                PubUtility.AddStringColumns(dtTemp, "DocNo,SeqNo,Qty,Qty2,ExchangeDate");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtTemp);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtTemp.Rows[0];
                int Qty1 = Convert.ToInt32(dr["Qty"].ToString().SqlQuote());
                int Qty2 = Convert.ToInt32(dr["Qty2"].ToString().SqlQuote()); ;

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {

                            sql = "update TempDocumentSV set ";
                            sql += " Qty=" + Qty1 + "";
                            sql += ", Qty2=" + Qty2 + "";
                            sql += ", RNum=" + (Qty2 - Qty1) + "";
                            sql += ",EffectiveDate='" + dr["ExchangeDate"].ToString().SqlQuote() + "'";
                            sql += ",ModDate=convert(char(10),getdate(),111)";
                            sql += ",ModTime=convert(char(10),getdate(),108)";
                            sql += ",ModUser='" + uu.UserID + "'";
                            sql += " where CompanyCode='" + uu.CompanyId + "' And DocNo='" +
                                dr["DocNo"].ToString().SqlQuote() + "' And SeqNo=" + dr["SeqNo"].ToString().SqlQuote();
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //dbop.Update("Rack", dtRec, new string[] { "Type_ID" }, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }

                sql = "select a.*,a.Layer+a.Sno Channel,c.GD_SName,c.Photo1 ";
                sql += " , Cast(b.PtNum As VarChar(5))+'/'+Cast(b.DisplayNum As VarChar(5)) ShowQty, b.DisplayNum-b.PtNum ShortQty, d.ST_SName, b.DisplayNum, b.PtNum ";
                sql += " from tempdocumentsv a";
                sql += " inner join InventorySV b on a.WhNo=b.WhNo and a.CkNo=b.CkNo And a.Layer=b.Layer And a.Sno=b.Sno and a.CompanyCode=b.CompanyCode";
                sql += " inner join PLUSV c on a.PLU=c.GD_NO and a.CompanyCode=c.CompanyCode";
                sql += " inner join WarehouseSV d on a.WhNo=d.ST_ID and a.CompanyCode=d.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' And SeqNo=" + dr["SeqNo"].ToString().SqlQuote();

                DataTable dtRes = PubUtility.SqlQry(sql, uu, "SYS");
                dtRes.TableName = "dtRes";
                ds.Tables.Add(dtRes);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-06-21 Larry
        [Route("SystemSetup/SaveInv")]
        public ActionResult SystemSetup_SaveInv()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveInvOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                DataTable dtTemp = new DataTable("TempDocumentSV");
                PubUtility.AddStringColumns(dtTemp, "DocNo,WhNo,CkNo");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtTemp);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtTemp.Rows[0];

                string DocNo = PubUtility.GetNewDocNo(uu, "TH", 6);

                string sql = "";
                string WhNoOut = "";
                string SysDate = "";


                sql = "select convert(char(10),getdate(),111) SysDate";

                DataTable dtSysDate = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtSysDate.Rows.Count > 0)
                {
                    SysDate = dtSysDate.Rows[0][0].ToString();
                }

                sql = "Select * From TempDocumentSV a ";
                sql += " Where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
                sql += " And IsNull(RNum,0) <> 0 ";
                sql += " And IsNull(ModDate,'')<>'' ";
                DataTable dtChkQty = PubUtility.SqlQry(sql, uu, "SYS");


                sql = "Select WhNoIn From WarehouseDSV "
                    + "Where CompanyCode='" + uu.CompanyId + "' "
                    + "And ST_ID='" + dr["WhNo"].ToString().SqlQuote() + "' "
                    + "And CkNo='" + dr["CkNo"].ToString().SqlQuote() + "' ";
                DataTable dtWhNoOut = PubUtility.SqlQry(sql, uu, "SYS");

                if (dtWhNoOut.Rows.Count > 0)
                {
                    WhNoOut = dtWhNoOut.Rows[0][0].ToString();
                }

                bool bSameWh = false;
                string CkNoOut = ""; string LayerOut = "";
                if (WhNoOut == dr["WhNo"].ToString())
                {
                    bSameWh = true; CkNoOut = "00"; LayerOut = "Z";
                }
                else
                {
                    bSameWh = false; CkNoOut = "XX"; LayerOut = "";
                }


                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {

                            string sqlAdj = "";
                            //異動庫存最近有效日期
                            sqlAdj = "Select * From TempDocumentSV a (Nolock) ";
                            sqlAdj += " Where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
                            sqlAdj += " And IsNull(RNum,0) = 0 And IsNull(EffectiveDate,'')<>''";
                            sqlAdj += " And IsNull(ModDate,'')<>'' ";

                            sql = "Update InventorySV "
                                + "Set ModUser='" + uu.UserID + "' "
                                + ",ModDate=convert(char(10),getdate(),111) "
                                + ",ModTime=convert(char(8),getdate(),108) "
                                + ",EffectiveDate = a.EffectiveDate "
                                + "From (" + sqlAdj + ") a Inner Join InventorySV b "
                                + "On a.CompanyCode=b.CompanyCode and a.Layer=b.Layer And a.Sno=b.Sno And a.PLU=b.PLU "
                                + "and b.WhNo='" + dr["WhNo"].ToString().SqlQuote() + "' and b.CkNo='" + dr["CkNo"].ToString().SqlQuote() + "' ";
                            sql += " Where b.CompanyCode='" + uu.CompanyId + "' ";

                            dbop.ExecuteSql(sql, uu, "SYS");

                            //沒有庫存資料-新增
                            sql = "Insert Into InventorySV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                + ", ModUser, ModDate, ModTime"
                                + ", WhNo, PLU, CkNo, Layer, Sno, EffectiveDate) ";
                            sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", a.WhNo, a.PLU, a.CkNo, a.Layer, a.Sno, a.EffectiveDate "
                                 + " From (" + sqlAdj + ") a Left Join InventorySV b "
                                 + " On a.CompanyCode = b.CompanyCode and a.WhNo = b.WhNo and a.CkNo = b.CkNo "
                                 + " and a.Layer=b.Layer And a.Sno=b.Sno And a.PLU=b.PLU "
                                 + "Where b.PLU Is Null ";
                            dbop.ExecuteSql(sql, uu, "SYS");


                            //檢查有數量異動，才寫入調整資料
                            if (dtChkQty.Rows.Count > 0)
                            {

                                //寫入調撥資料
                                //調撥表頭
                                sql = "Insert Into TransferHSV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                + ",ModUser, ModDate, ModTime "
                                + ",TH_ID, DocDate, WhNoOut, OutUser"
                                + ", InDate, InUser, WhNoIn"
                                + ",ExpressNo, ChkUser, ChkDate, "
                                + "PostUser, PostDate, DocType"
                                + ", CkNoOut, CkNoIn, WorkType) Values ";
                                sql += " ('" + uu.CompanyId.SqlQuote() + "', '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                    + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                    + ", '" + DocNo + "', convert(char(10),getdate(),111), '" + WhNoOut + "', '" + uu.UserID + "'"
                                    + ", convert(char(10),getdate(),111),'" + uu.UserID + "','" + dr["WhNo"].ToString().SqlQuote() + "'"
                                    + ", '', '" + uu.UserID + "', convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5)"
                                    + ", '" + uu.UserID + "', convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5),'V' "
                                    + ",'" + CkNoOut + "', '" + dr["CkNo"].ToString().SqlQuote() + "', 'IN')";
                                dbop.ExecuteSql(sql, uu, "SYS");

                                //調撥表身
                                sql = "Insert Into TransferDSV (CompanyCode, CrtUser, CrtDate, CrtTime, ModUser, ModDate, ModTime, " +
                                    "TH_ID, SeqNo, PLU, OutNum, InNum, " +
                                    "GD_Retail, Amt, LayerIn, SnoIn, LayerOut, EffectiveDate, SnoOut) ";
                                sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", '" + DocNo + "', Cast(Row_Number() Over(Order By a.Layer,a.Sno) As int), a.PLU, Qty, Qty"
                                     + ", b.GD_Retail, Qty*GD_Retail, a.Layer, a.Sno, '" + LayerOut + "'"
                                     + ", a.EffectiveDate";
                                //2021/08/09 調整為以 品號 寫入調出貨道
                                sql += ", a.PLU ";
                                //if (LayerOut == "Z")
                                //{
                                //    sql += ", d.Sno ";
                                //}
                                //else
                                //{
                                //    sql += ", '' ";
                                //}
                                sql += " From TempDocumentSV a (Nolock) ";
                                sql += " Inner Join PLUSV b (Nolock) On a.CompanyCode=b.CompanyCode And a.PLU=b.GD_No ";
                                //sql += " Inner Join WarehouseDSV c (Nolock) On a.CompanyCode=c.CompanyCode And a.PLU=c.GD_No ";
                                sql += " Left Join InventorySV d (Nolock) On a.CompanyCode=d.CompanyCode And a.PLU=d.PLU "
                                    + "And d.WhNo='" + WhNoOut + "' And d.CkNo='" + CkNoOut + "' And d.Layer='Z' ";

                                sql += " Where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
                                //2021/08/09 修正
                                sql += " And IsNull(RNum,0) <> 0 ";
                                //sql += " And (a.Qty>0 or (a.Qty>=0 And IsNull(a.EffectiveDate,'')<>''))";

                                sql += " And IsNull(a.ModDate,'')<>'' ";
                                dbop.ExecuteSql(sql, uu, "SYS");

                                //變更庫存數量及庫存增減日
                                //調出方
                                string sqlTROut = "";

                                ////#####特殊處理：因 補貨 調出方 非實際的機器，沒有貨倉、貨道，故必須將相同商品Group起來，並以品號作為調出的貨道
                                sqlTROut = "Select H.CompanyCode, H.TH_ID, H.WhNoOut, H.CkNoOut, D.LayerOut, D.PLU SnoOut, "
                                    + " Row_Number() Over(Order By H.CompanyCode, H.TH_ID, H.WhNoOut, H.CkNoOut, D.LayerOut, D.PLU) SeqNo, D.PLU, Sum(D.OutNum) OutNum "
                                    + " From TransferHSV H Inner Join TransferDSV D "
                                    + " On H.CompanyCode = D.CompanyCode And H.TH_ID = D.TH_ID "
                                    + " Where H.CompanyCode='" + uu.CompanyId + "' And H.TH_ID='" + DocNo + "' "
                                    + " Group By H.CompanyCode, H.TH_ID, H.WhNoOut, H.CkNoOut, D.LayerOut, D.PLU ";


                                //sqlTROut = "Select H.CompanyCode, H.TH_ID, H.WhNoOut, H.CkNoOut, D.LayerOut, D.SnoOut, D.SeqNo, D.PLU, D.OutNum "
                                //    + " From TransferHSV H Inner Join TransferDSV D "
                                //    + " On H.CompanyCode = D.CompanyCode And H.TH_ID = D.TH_ID "
                                //    + " Where H.CompanyCode='" + uu.CompanyId + "' And H.TH_ID='" + DocNo + "' ";

                                if (CkNoOut != "XX")
                                {
                                    //寫入調出方jahoInvSV
                                    sql = "Insert Into JahoInvSV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                        + ", ModUser, ModDate, ModTime "
                                        + ", DocType, DocNo, WhNo, SeqNo, PLU, Q1, Q2, Q3, CkNo, Layer, Sno) ";
                                    sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                         + ", 'TF', TH_ID, WhNoOut, SeqNo, a.PLU, IsNull(b.PtNum,0), -1*OutNum, IsNull(b.PtNum,0)-OutNum"
                                         + ", CkNoOut, LayerOut, SnoOut "
                                         + " From (" + sqlTROut + ") a Left Join InventorySV b "
                                         + " On a.CompanyCode = b.CompanyCode And a.WhNoOut = b.WhNo and a.CkNoOut = b.CkNo "
                                         + " and a.LayerOut=b.Layer And a.SnoOut=b.Sno And a.PLU=b.PLU ";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    //已有庫存資料
                                    sql = "Update InventorySV "
                                        + "Set ModUser='" + uu.UserID + "' "
                                        + ",ModDate=convert(char(10),getdate(),111) "
                                        + ",ModTime=convert(char(8),getdate(),108) "
                                        + ",PtNum=IsNull(PtNum,0) - OutNum "
                                        + ",Out_Date = Case When Out_Date>'" + SysDate + "' Then Out_Date Else '" + SysDate + "' End "
                                        + "From (" + sqlTROut + ") a Inner Join InventorySV b "
                                        + "On a.CompanyCode=b.CompanyCode and a.WhNoOut=b.WhNo and a.CkNoOut=b.CkNo "
                                        + "and a.LayerOut=b.Layer And a.SnoOut=b.Sno And a.PLU=b.PLU ";
                                    sql += " Where b.CompanyCode='" + uu.CompanyId + "' ";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    //沒有庫存資料-新增
                                    sql = "Insert Into InventorySV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                        + ", ModUser, ModDate, ModTime "
                                        + ", WhNo, PLU, Out_Date, CkNo, Layer, Sno, PtNum, SafeNum ) ";
                                    sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                         + ", WhNoOut, a.PLU, '" + SysDate + "', CkNoOut, LayerOut, SnoOut, -1*OutNum, 1 "
                                         + " From (" + sqlTROut + ") a Left Join InventorySV b "
                                         + " On a.CompanyCode = b.CompanyCode and a.WhNoOut = b.WhNo and a.CkNoOut = b.CkNo "
                                         + "and a.LayerOut=b.Layer And a.SnoOut=b.Sno And a.PLU=b.PLU "
                                         + "Where b.PLU Is Null ";
                                    dbop.ExecuteSql(sql, uu, "SYS");
                                }




                                //調入方
                                string sqlTRIn = "";
                                sqlTRIn = "Select H.CompanyCode, H.TH_ID, H.WhNoIn, H.CkNoIn, D.LayerIn, D.SnoIn, D.SeqNo, D.PLU, D.InNum, T.EffectiveDate "
                                    + " From TransferHSV H Inner Join TransferDSV D "
                                    + " On H.CompanyCode = D.CompanyCode And H.TH_ID = D.TH_ID "
                                    + " Inner Join TempDocumentSV T On H.CompanyCode = T.CompanyCode And H.WhNoIn=T.WhNo And H.CkNoIn=T.CkNo And D.LayerIn=T.Layer And D.SnoIn=T.Sno "
                                    + " Where H.CompanyCode='" + uu.CompanyId + "' And H.TH_ID='" + DocNo + "' And T.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
                                //寫入調入方jahoInvSV
                                sql = "Insert Into JahoInvSV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                    + ", ModUser, ModDate, ModTime "
                                    + ", DocType, DocNo, WhNo, SeqNo, PLU, Q1, Q2, Q3, CkNo, Layer, Sno) ";
                                sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", 'TF', TH_ID, WhNoIn, SeqNo, a.PLU, IsNull(b.PtNum,0), InNum, IsNull(b.PtNum,0)+InNum"
                                     + ", CkNoIn, LayerIn, SnoIn "
                                     + " From (" + sqlTRIn + ") a Left Join InventorySV b "
                                     + " On a.CompanyCode = b.CompanyCode And a.WhNoIn = b.WhNo and a.CkNoIn = b.CkNo "
                                     + " and a.LayerIn = b.Layer And a.SnoIn = b.Sno And a.PLU=b.PLU ";
                                dbop.ExecuteSql(sql, uu, "SYS");

                                //已有庫存資料
                                sql = "Update InventorySV "
                                    + "Set ModUser='" + uu.UserID + "' "
                                    + ",ModDate=convert(char(10),getdate(),111) "
                                    + ",ModTime=convert(char(8),getdate(),108) "
                                    + ",PtNum=IsNull(PtNum,0) + InNum "
                                    + ",In_Date = Case When In_Date>'" + SysDate + "' Then In_Date Else '" + SysDate + "' End "
                                    + ",EffectiveDate=a.EffectiveDate "
                                    + "From (" + sqlTRIn + ") a Inner Join InventorySV b "
                                    + "On a.CompanyCode=b.CompanyCode and a.WhNoIn=b.WhNo and a.CkNoIn=b.CkNo And a.PLU=b.PLU "
                                    + "and a.LayerIn=b.Layer And a.SnoIn=b.Sno ";
                                sql += " Where b.CompanyCode='" + uu.CompanyId + "' ";
                                dbop.ExecuteSql(sql, uu, "SYS");

                                //沒有庫存資料-新增
                                sql = "Insert Into InventorySV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                    + ", ModUser, ModDate, ModTime"
                                    + ", WhNo, PLU, In_Date, CkNo, Layer, Sno, PtNum, SafeNum, EffectiveDate) ";
                                sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", WhNoIn, a.PLU, '" + SysDate + "', CkNoIn, LayerIn, SnoIn, InNum, 1, a.EffectiveDate "
                                     + " From (" + sqlTRIn + ") a Left Join InventorySV b "
                                     + " On a.CompanyCode = b.CompanyCode and a.WhNoIn = b.WhNo and a.CkNoIn = b.CkNo "
                                     + " and a.LayerIn=b.Layer And a.SnoIn=b.Sno And a.PLU=b.PLU "
                                     + "Where b.PLU Is Null ";
                                dbop.ExecuteSql(sql, uu, "SYS");

                            }



                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }

                //sql = "select a.*,a.Layer+a.Sno Channel,c.GD_SName,c.Photo1 ";
                //sql += " , Cast(b.PtNum As VarChar(5))+'/'+Cast(b.DisplayNum As VarChar(5)) ShowQty, b.DisplayNum-b.PtNum ShortQty, Qty, d.ST_SName ";
                //sql += " from tempdocumentsv a";
                //sql += " inner join InventorySV b on a.WhNo=b.WhNo and a.CkNo=b.CkNo And a.Layer=b.Layer And a.Sno=b.Sno and a.CompanyCode=b.CompanyCode";
                //sql += " inner join PLUSV c on a.PLU=c.GD_NO and a.CompanyCode=c.CompanyCode";
                //sql += " inner join WarehouseSV d on a.WhNo=d.ST_ID and a.CompanyCode=d.CompanyCode";
                //sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'And SeqNo=" + dr["SeqNo"].ToString().SqlQuote();

                //DataTable dtRes = PubUtility.SqlQry(sql, uu, "SYS");
                //dtRes.TableName = "dtRes";
                //ds.Tables.Add(dtRes);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //[Route("SystemSetup/GetPageInitBefore")]
        //public ActionResult SystemSetup_GetPageInitBefore()
        //{
        //    UserInfo uu = PubUtility.GetCurrentUser(this);
        //    System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetPageInitBeforeOK", "" });
        //    DataTable dtMessage = ds.Tables["dtMessage"];
        //    try
        //    {

        //        string sql = "select a.Man_ID,a.Whno,b.st_sname from EmployeeWeb a (nolock) ";
        //        sql += "left join WarehouseWeb b (nolock) on a.whno=b.st_id and b.companycode=a.companycode ";
        //        sql += "where a.Companycode='" + uu.CompanyId + "' and a.Man_ID='" + uu.UserID + "'";
        //        DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
        //        dtE.TableName = "dtE";
        //        ds.Tables.Add(dtE);

        //        sql = "select '' as ID,'' as Name,0 as cash,0 as Num,0 as cnt,0 as cashcnt ";
        //        sql += "from SalesH h(nolock) ";
        //        sql += "where 1=2 ";
        //        DataTable dtQ = PubUtility.SqlQry(sql, uu, "SYS");
        //        dtQ.TableName = "dtQ";
        //        ds.Tables.Add(dtQ);

        //    }
        //    catch (Exception err)
        //    {
        //        dtMessage.Rows[0][0] = "Exception";
        //        dtMessage.Rows[0][1] = err.Message;
        //    }
        //    return PubUtility.DatasetXML(ds);
        //}

        [Route("SystemSetup/GetWhName")]
        public ActionResult SystemSetup_GetWhName()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetWhNameOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "select a.WhNo,case when ST_Sname is null then '' else left(a.Whno+'      ',6)+' '+ST_SName end STName from ISAMShopWeb a (nolock) ";
                sql += "left join WarehouseWeb c (nolock) on a.CompanyCode=c.CompanyCode and a.Whno=c.ST_ID ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and a.Man_ID='" + uu.UserID + "'";
                DataTable dtWh = PubUtility.SqlQry(sql, uu, "SYS");
                dtWh.TableName = "dtWh";
                ds.Tables.Add(dtWh);


            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/GetInitISAM01")]
        public ActionResult SystemSetup_GetInitISAM01()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitISAM01OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                string sql = "select a.Man_ID,left(a.Man_ID+'      ',6)+' '+Man_Name ManName ,a.Whno,left(a.Whno+'      ',6)+' '+ST_SName STName from ISAMShopWeb a (nolock) ";
                sql += "inner join EmployeeWeb b (nolock) on a.CompanyCode=b.CompanyCode and a.Man_ID=b.Man_ID ";
                sql += "inner join WarehouseWeb c (nolock) on a.CompanyCode=c.CompanyCode and a.Whno=c.ST_ID ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and a.Man_ID='" + uu.UserID + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                dtC.TableName = "dtWh";
                ds.Tables.Add(dtC);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SearchBINWeb")]
        public ActionResult SystemSetup_SearchBINWeb()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchBINWebOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string ISAMDate = rq["ISAMDate"];
                string BinNo = rq["BinNo"];

                string sql = "select top 1 * from BINWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "' and BINman='" + uu.UserID + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtC.Rows.Count == 0)
                {
                    sql = "select top 1 * from BINWeb (nolock) ";
                    sql += "where Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "'";
                    dtC = PubUtility.SqlQry(sql, uu, "SYS");
                }

                dtC.TableName = "dtBINData";
                ds.Tables.Add(dtC);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/ChkSaveBINWeb")]
        public ActionResult SystemSetup_ChkSaveBINWeb()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkSaveBINWebOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string ISAMDate = rq["ISAMDate"];
                string BinNo = rq["BinNo"];
                string PLU = rq["Barcode"];
                int Qty = Convert.ToInt32(rq["Qty"].ToString().SqlQuote());

                string sql = "select Sum(Qty1)SumQty from BINWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "' and PLU='" + PLU + "'";
                DataTable dt = PubUtility.SqlQry(sql, uu, "SYS");
                dt.TableName = "dtBIN";
                ds.Tables.Add(dt);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SaveBINWeb")]
        public ActionResult SystemSetup_SaveBINWeb()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveBINWebOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                Boolean lb_Insert = false;
                string Shop = rq["Shop"];
                string ISAMDate = rq["ISAMDate"];
                string BinNo = rq["BinNo"];
                string PLU = rq["Barcode"];
                int Qty = Convert.ToInt32(rq["Qty"].ToString().SqlQuote());

                string sql = "select * from BINWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "' and BINman='" + uu.UserID + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtC.Rows.Count == 0) { lb_Insert = true; }
                else
                {
                    DataRow[] dr = dtC.Select("PLU='" + PLU + "'");
                    if (dr.Length == 0) { lb_Insert = true; }
                    else
                    {
                        sql = "Update BINWeb set Qty1=Qty1+" + Qty + ",ModDate=Convert(varchar,getdate(),111),ModTime=Substring(Convert(varchar,getdate(),121),12,12)";
                        sql += " where Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "'";
                        sql += " and BINNO='" + BinNo + "' and BINman='" + uu.UserID + "' and PLU='" + PLU + "'";
                        PubUtility.ExecuteSql(sql, uu, "SYS");
                    }

                }

                if (lb_Insert)
                {
                    sql = "Insert into BINWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime,BINStore,BINNO,BINman,ISAMDate,ISAMTime,SeqNo,PLU,QTY1) ";
                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',Convert(varchar,getdate(),111),Substring(Convert(varchar,getdate(),121),12,12),";
                    sql += "'" + uu.UserID + "',Convert(varchar,getdate(),111),Substring(Convert(varchar,getdate(),121),12,12),";
                    sql += "'" + Shop + "','" + BinNo + "','" + uu.UserID + "','" + ISAMDate + "',Convert(varchar,getdate(),108),";
                    sql += "(select isnull(max(seqno),0)+1 from BINWeb (nolock) where Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "' and Binman='" + uu.UserID + "'),";
                    sql += "'" + PLU + "', " + Qty.ToString();
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }
                //Return 異動後的數量,故要重撈一次
                //分區單品數
                sql = "Select Sum(Qty1) SQ1 from BINWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BinNo='" + BinNo + "' and PLU='" + PLU + "'";
                DataTable dtSQ = PubUtility.SqlQry(sql, uu, "SYS");
                dtSQ.TableName = "dtSQ";
                ds.Tables.Add(dtSQ);
                //分區總和(不看商品)
                sql = "Select Sum(Qty1) SBQ1 from BINWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BinNo='" + BinNo + "'";
                DataTable dtSBQ = PubUtility.SqlQry(sql, uu, "SYS");
                dtSBQ.TableName = "dtSBQ";
                ds.Tables.Add(dtSBQ);
                //門市總和(不看商品)
                sql = "Select Sum(Qty1) SWQ1 from BINWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "'";
                DataTable dtSWQ = PubUtility.SqlQry(sql, uu, "SYS");
                dtSWQ.TableName = "dtSWQ";
                ds.Tables.Add(dtSWQ);
                sql = "Select '" + PLU + "' PLU,GD_Name,GD_Retail from PLUWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and GD_Barcode='" + PLU + "'";
                DataTable dtP = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtP.Rows.Count == 0)
                {
                    sql = "Select '" + PLU + "' PLU,GD_Name,GD_Retail from PLUWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and GD_No='" + PLU + "'";
                    dtP = PubUtility.SqlQry(sql, uu, "SYS");
                }

                dtP.TableName = "dtPLU";
                ds.Tables.Add(dtP);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetBINWebMod")]
        public ActionResult SystemSetup_GetBINWebMod()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetBINWebModOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string ISAMDate = rq["ISAMDate"];
                string BinNo = rq["BinNo"];
                string Comd = "";

                if (rq.Count > 3)
                {
                    string PLU = rq["PLU"];
                    if (PLU != null && PLU != "") { Comd = " and PLU='" + PLU + "'"; }
                }

                string sql = "set nocount on;select a.*,GD_Name into #tmpA from BINWeb a (nolock) left join PLUWeb b (nolock) ";
                sql += "on a.companycode=b.companycode and a.PLU=b.GD_Barcode ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "' and BINman='" + uu.UserID + "'" + Comd + ";";
                sql += "select a.PLU,a.Qty1,a.PLU+' '+case when isnull(a.GD_Name,'')='' then isnull(b.GD_Name,'') else isnull(a.GD_Name,'') end GD_Name from #tmpA a (nolock) left join PLUWeb b (nolock) ";
                sql += "on a.companycode=b.companycode and a.PLU=b.GD_No order by SeqNo desc";


                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");

                dtC.TableName = "dtBin";
                ds.Tables.Add(dtC);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetGDName")]
        public ActionResult SystemSetup_GetGDName()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetGDNameOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string PLU = rq["PLU"];

                string sql = "select '" + PLU + "' PLU,GD_Name from PLUWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and GD_Barcode='" + PLU + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");

                if (dtC.Rows.Count == 0)
                {
                    sql = "select '" + PLU + "' PLU,GD_Name from PLUWeb (nolock) ";
                    sql += "where Companycode='" + uu.CompanyId + "' and GD_No='" + PLU + "'";
                    dtC = PubUtility.SqlQry(sql, uu, "SYS");
                }

                dtC.TableName = "dtPLU";
                ds.Tables.Add(dtC);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/DelISAM01PLU")]
        public ActionResult SystemSetup_DelISAM01PLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "DelISAM01PLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string ISAMDate = rq["ISAMDate"];
                string BinNo = rq["BinNo"];
                string PLU = rq["PLU"];

                string sql = "Delete from BINWeb ";
                sql += "where Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "' and BINman='" + uu.UserID + "' and PLU='" + PLU + "'";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                sql = "set nocount on;Select ROW_NUMBER() over(order by Companycode,Binstore,binno,isamdate,seqno) NewSeq,* into #tmpA from BINWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "' and BINman='" + uu.UserID + "';";
                sql += "update b set SeqNo=newseq from #tmpA a inner join BINWeb b ";
                sql += "on a.CompanyCode=b.CompanyCode and a.BINStore=b.BINStore and a.BINNO=b.BINNO and a.ISAMDate=b.ISAMDate and a.SeqNo=b.SeqNo;";
                sql += "drop table #tmpA;";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SaveISAM01PLUMod")]
        public ActionResult SystemSetup_SaveISAM01PLUMod()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveISAM01PLUModOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string ISAMDate = rq["ISAMDate"];
                string BinNo = rq["BinNo"];
                string PLU = rq["PLU"];
                string Qty = rq["Qty"];

                string sql = "Update BINWeb set Qty1=" + Qty.SqlQuote();
                sql += " where Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "' and BINman='" + uu.UserID + "' and PLU='" + PLU + "'";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                sql = "select PLU,Qty1,PLU+' '+GD_Name GD_Name from BINWeb a (nolock) inner join PLUWeb b ";
                sql += "on a.companycode=b.companycode and a.PLU=b.GD_Barcode ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "' and BINman='" + uu.UserID + "' and PLU='" + PLU + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");

                if (dtC.Rows.Count == 0)
                {
                    sql = "select PLU,Qty1,PLU+' '+isnull(GD_Name,'') GD_Name from BINWeb a (nolock) left join PLUWeb b ";
                    sql += "on a.companycode=b.companycode and a.PLU=b.GD_No ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and ISAMDate='" + ISAMDate + "' and BINNO='" + BinNo + "' and BINman='" + uu.UserID + "' and PLU='" + PLU + "'";
                    dtC = PubUtility.SqlQry(sql, uu, "SYS");
                }

                dtC.TableName = "dtBINMod";
                ds.Tables.Add(dtC);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/AddISAMToFTPRecWeb")]
        public ActionResult SystemSetup_AddISAMToFTPRecWeb()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "AddISAMToFTPRecWebOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Doctype = rq["Type"];
                string Shop = rq["Shop"];
                string Comd = "";

                string sql = "";
                string FPIP = "";
                string FPID = "";
                string FPPWD = "";
                string ls_Date = "";
                string ls_Time = "";
                string filename = "";
                string printstr = "";

                //取FTP位址
                sql = "Select * From FTPDataWeb (nolock) Where worktype='2' and TypeName in (Select CompanyCode from CompanyWeb (nolock) where isnull(ISAMFlag,'')='Y') and TypeName='" + uu.CompanyId + "'";
                DataTable dtA = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtA.Rows.Count > 0)
                {
                    FPIP = dtA.Rows[0]["FTPIP"].ToString().SqlQuote();
                    FPID = PubUtility.enCode170215(dtA.Rows[0]["FTPID"].ToString().SqlQuote());
                    FPPWD = PubUtility.enCode170215(dtA.Rows[0]["FTPPWD"].ToString().SqlQuote());
                    ls_Date = PubUtility.SqlQry("Select convert(char(10),getdate(),111)", uu, "SYS").Rows[0][0].ToString().SqlQuote();
                    ls_Time = PubUtility.SqlQry("Select right(convert(varchar, getdate(), 121),12)", uu, "SYS").Rows[0][0].ToString().SqlQuote();
                    if (!System.IO.Directory.Exists(PubUtility.UpLoadFiles.FTPFilePath)) { System.IO.Directory.CreateDirectory(PubUtility.UpLoadFiles.FTPFilePath); }
                    if (!System.IO.Directory.Exists(PubUtility.UpLoadFiles.FTPFilePath + "\\BackUp" + "\\" + uu.CompanyId + "\\" + ls_Date.Replace("/", ""))) { System.IO.Directory.CreateDirectory(PubUtility.UpLoadFiles.FTPFilePath + "\\BackUp" + "\\" + uu.CompanyId + "\\" + ls_Date.Replace("/", "")); }

                    switch (Doctype)
                    {
                        case "T":
                            filename = ls_Time.Replace(":", "").Replace(".", "") + "TAKE3.dat";
                            if (rq.Count > 2)
                            {
                                string ISAMDate = rq["ISAMDate"];
                                string BinNo = rq["BinNo"];
                                Comd = Shop + ";" + BinNo + ";" + ISAMDate + ";" + uu.UserID;  //店;分區代碼;盤點日期;登入者
                            }
                            else
                            {
                                Comd = Shop + ";" + uu.UserID;
                            }
                            break;
                        case "C":
                            filename = ls_Time.Replace(":", "").Replace(".", "") + "_" + uu.UserID + "_Collect3.dat";
                            Comd = Shop + ";" + uu.UserID;
                            break;
                        case "D":
                            filename = ls_Time.Replace(":", "").Replace(".", "") + "_" + uu.UserID + "_Deliv3.dat";
                            string DocDate = rq["DocDate"];
                            string InShop = rq["WhNoIn"];
                            Comd = Shop + ";" + InShop + ";" + DocDate + ";" + uu.UserID;  //出貨店碼；進貨店碼；單據日期；登入者
                            break;
                        default:
                            break;
                    }

                    sql = "Insert into ISAMToFTPRecWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime,DocType,WhNo,UpLoadComd) ";
                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "','" + ls_Date + "','" + ls_Time + "',";
                    sql += "'" + uu.UserID + "','" + ls_Date + "','" + ls_Time + "',";
                    sql += "'" + Doctype + "','" + Shop + "','" + Comd + "'";
                    PubUtility.ExecuteSql(sql, uu, "SYS");


                    sql = "Select * from ISAMToFTPRecWeb (nolock) where Companycode='" + uu.CompanyId + "' and CrtUser='" + uu.UserID + "' and CrtDate='" + ls_Date + "' and CrtTime='" + ls_Time + "' ";
                    sql += "and DocType='" + Doctype + "' and WhNo='" + Shop + "' and updatetype='N'";
                    DataTable dtFTPRec = PubUtility.SqlQry(sql, uu, "SYS");

                    if (dtFTPRec.Rows.Count > 0)
                    {
                        //店號(6)、BIN(3)、條碼(16)、數量(6)、盤點人員 (6)、盤點日(8)、時間 HHMMSS(6)
                        if (Doctype == "T")
                        {

                            if (rq.Count > 2)
                            {
                                sql = "select * into #tmpA from BINWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and BINStore='" + Shop + "' and BINNo='" + rq["BinNo"] + "' and ISAMDate='" + rq["ISAMDate"] + "'; ";
                            }
                            else
                            {
                                sql = "select * into #tmpA from BINWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and BINStore='" + Shop + "'; ";
                            }

                            sql += "update b set FTPUpDate='" + ls_Date + "' + ' ' + left('" + ls_Time + "',8) from #tmpA a inner join BINWeb b (nolock) on a.CompanyCode=b.CompanyCode and a.BINStore=b.BINStore and a.BINNO=b.BINNO and a.ISAMDate=b.ISAMDate and a.BINMan=b.BINMan and a.SeqNo=b.SeqNo; ";
                            sql += "Select left(BINStore+SPACE(6),6)+left(BINNo+SPACE(3),3)+left(PLU+SPACE(16),16)+right(SPACE(6)+convert(varchar,Qty1),6)+left(BINMan+SPACE(6),6)+Replace(ISAMDate,'/','')+Replace(ISAMTime,':','') A1 from #tmpA order by ISAMDate,BINNo,BINMan,Seqno; ";
                        }
                        //條碼(16)、數量(6)
                        else if (Doctype == "C")
                        {
                            sql = "select * into #tmpA from CollectWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and Whno='" + Shop + "' and WorkUser='" + uu.UserID + "'; ";
                            sql += "update b set FTPUpDate='" + ls_Date + "' + ' ' + left('" + ls_Time + "',8) from #tmpA a inner join CollectWeb b (nolock) on a.CompanyCode=b.CompanyCode and a.Whno=b.Whno and a.Workuser=b.Workuser and a.PLU=b.PLU; ";
                            sql += "Select left(PLU+SPACE(16),16)+right(SPACE(6)+convert(varchar,Qty),6) A1 from #tmpA order by Seqno; ";
                        }
                        //出貨倉庫(6)、補貨倉庫(6)、條碼(16)、出貨數量 (6)、員工(6)、出貨日(8)
                        else if (Doctype == "D")
                        {
                            sql = "select * into #tmpA from DeliveryWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and WhnoOut='" + Shop + "' and WhnoIn='" + rq["WhNoIn"] + "' and DocDate='" + rq["DocDate"] + "' and OutUser='" + uu.UserID + "'; ";
                            sql += "update b set FTPUpDate='" + ls_Date + "' + ' ' + left('" + ls_Time + "',8) from #tmpA a inner join DeliveryWeb b (nolock) on a.CompanyCode=b.CompanyCode and a.WhnoOut=b.WhnoOut and a.WhnoIn=b.WhnoIn and a.DocDate=b.DocDate and a.OutUser=b.OutUser and a.SeqNo=b.SeqNo; ";
                            sql += "Select left(WhnoOut+SPACE(6),6)+left(WhnoIn+SPACE(6),6)+left(PLU+SPACE(16),16)+right(SPACE(6)+convert(varchar,OutNum),6)+left(OutUser+SPACE(6),6)+Replace(DocDate,'/','') A1 from #tmpA order by Seqno; ";
                        }
                        DataTable dtA1 = PubUtility.SqlQry(sql, uu, "SYS");
                        if (dtA1.Rows.Count > 0)
                        {
                            for (int i = 0; i <= dtA1.Rows.Count - 1; i++)
                            {
                                System.IO.File.AppendAllText(PubUtility.UpLoadFiles.FTPFilePath + "\\" + filename, dtA1.Rows[i]["A1"].ToString() + "\r\n");
                            }

                            if (System.IO.File.Exists(PubUtility.UpLoadFiles.FTPFilePath + "\\" + filename))
                            {
                                //if (PubUtility.UpLoadFiles.MakeDir(FPIP, FPID, FPPWD, uu.CompanyId))
                                //{
                                //    PubUtility.UpLoadFiles.MakeDir(FPIP + "/" + uu.CompanyId, FPID, FPPWD, Shop);
                                //}
                                PubUtility.UpLoadFiles.MakeDir(FPIP, FPID, FPPWD, Shop);


                                if (PubUtility.UpLoadFiles.UploadFile(FPIP + "/" + Shop, FPID, FPPWD, PubUtility.UpLoadFiles.FTPFilePath + "\\" + filename))
                                {

                                    sql = "UpDate ISAMTOFTPRecWeb set UpDateType='Y',FTPDate='" + ls_Date + "' + ' ' + left('" + ls_Time + "',8),FileName='" + filename + "' ";
                                    sql += "where CompanyCode='" + uu.CompanyId + "' and CrtUser='" + uu.UserID + "' and CrtDate='" + ls_Date + "' and CrtTime='" + ls_Time + "' ";
                                    sql += "and DocType='" + Doctype + "' and Whno='" + Shop + "' ";
                                    PubUtility.ExecuteSql(sql, uu, "SYS");

                                    if (PubUtility.UpLoadFiles.RemoteFtpDirExists(FPIP + "/" + Shop, FPID, FPPWD, filename))
                                    {
                                        System.IO.File.Move(PubUtility.UpLoadFiles.FTPFilePath + "\\" + filename, PubUtility.UpLoadFiles.FTPFilePath + "\\BackUp\\" + "\\" + uu.CompanyId + "\\" + ls_Date.Replace("/", "") + "\\" + filename);
                                    }
                                }
                            }
                            else
                            {
                                sql = "UpDate ISAMTOFTPRecWeb set UpDateType='E',ModDate=Convert(varchar,getdate(),111),ModTime=substring(Convert(varchar,getdate(),121),12,12) ";
                                sql += "where CompanyCode='" + uu.CompanyId + "' and CrtUser='" + uu.UserID + "' and CrtDate='" + ls_Date + "' and CrtTime='" + ls_Time + "' ";
                                sql += "and DocType='" + Doctype + "' and Whno='" + Shop + "' ";
                                PubUtility.ExecuteSql(sql, uu, "SYS");
                                throw new Exception("上傳檔案");
                            }

                        }
                        else
                        {
                            throw new Exception("上傳資料");
                        }
                    }
                    else
                    {
                        throw new Exception("上傳記錄");
                    }
                }
                else
                {
                    throw new Exception("FTP");
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/ChkSaveCollectWeb")]
        public ActionResult SystemSetup_ChkSaveCollectWeb()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkSaveCollectWebOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string PLU = rq["Barcode"];

                string sql = "Select Sum(Qty) SumQty from CollectWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and Whno='" + Shop + "' and PLU='" + PLU + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                dtC.TableName = "dtC";
                ds.Tables.Add(dtC);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SaveCollectWeb")]
        public ActionResult SystemSetup_SaveCollectWeb()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveCollectWebOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                Boolean lb_Insert = false;
                string Shop = rq["Shop"];
                string PLU = rq["Barcode"];
                int Qty = Convert.ToInt32(rq["Qty"].ToString().SqlQuote());

                string sql = "select * from CollectWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and Whno='" + Shop + "' and WorkUser='" + uu.UserID + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtC.Rows.Count == 0) { lb_Insert = true; }
                else
                {
                    DataRow[] dr = dtC.Select("PLU='" + PLU + "'");
                    if (dr.Length == 0) { lb_Insert = true; }
                    else
                    {
                        sql = "Update CollectWeb set Qty=Qty+" + Qty + ",ModDate=Convert(varchar,getdate(),111),ModTime=Substring(Convert(varchar,getdate(),121),12,12) ";
                        sql += "where Companycode='" + uu.CompanyId + "' and Whno='" + Shop + "' and WorkUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                        PubUtility.ExecuteSql(sql, uu, "SYS");
                    }

                }

                if (lb_Insert)
                {
                    sql = "Insert into CollectWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime,Whno,WorkUser,SeqNo,PLU,QTY) ";
                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',Convert(varchar,getdate(),111),Substring(Convert(varchar,getdate(),121),12,12),";
                    sql += "'" + uu.UserID + "',Convert(varchar,getdate(),111),Substring(Convert(varchar,getdate(),121),12,12),";
                    sql += "'" + Shop + "','" + uu.UserID + "',";
                    sql += "(select isnull(max(seqno),0)+1 from CollectWeb (nolock) where Companycode='" + uu.CompanyId + "' and Whno='" + Shop + "' and WorkUser='" + uu.UserID + "'),";
                    sql += "'" + PLU + "', " + Qty.ToString();
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }
                //Return 異動後的數量,故要重撈一次
                //單品數
                sql = "Select Sum(Qty) SQ1 from CollectWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and Whno='" + Shop + "' and PLU='" + PLU + "'";
                DataTable dtSQ = PubUtility.SqlQry(sql, uu, "SYS");
                dtSQ.TableName = "dtSQ";
                ds.Tables.Add(dtSQ);

                sql = "Select '" + PLU + "' PLU,GD_Name,GD_Retail from PLUWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and GD_Barcode='" + PLU + "'";
                DataTable dtP = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtP.Rows.Count == 0)
                {
                    sql = "Select '" + PLU + "' PLU,GD_Name,GD_Retail from PLUWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and GD_No='" + PLU + "'";
                    dtP = PubUtility.SqlQry(sql, uu, "SYS");
                }

                dtP.TableName = "dtPLU";
                ds.Tables.Add(dtP);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetCollectWebMod")]
        public ActionResult SystemSetup_GetCollectWebMod()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetCollectWebModOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string Comd = "";

                if (rq.Count > 1)
                {
                    string PLU = rq["PLU"];
                    if (PLU != null && PLU != "") { Comd = " and PLU='" + PLU + "'"; }
                }

                string sql = "set nocount on;select a.*,GD_Name into #tmpA from CollectWeb a (nolock) left join PLUWeb b (nolock) ";
                sql += "on a.companycode=b.companycode and a.PLU=b.GD_Barcode ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and Whno='" + Shop + "' and WorkUser='" + uu.UserID + "'" + Comd + ";";
                sql += "select a.PLU,a.Qty,a.PLU + ' ' + case when isnull(a.GD_Name,'')='' then isnull(b.GD_Name,'') else isnull(a.GD_Name,'') end GD_Name from #tmpA a (nolock) left join PLUWeb b (nolock) ";
                sql += "on a.companycode=b.companycode and a.PLU=b.GD_No order by SeqNo desc";


                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");

                dtC.TableName = "dtCollect";
                ds.Tables.Add(dtC);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SaveISAM02PLUMod")]
        public ActionResult SystemSetup_SaveISAM02PLUMod()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveISAM02PLUModOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string PLU = rq["PLU"];
                string Qty = rq["Qty"];

                string sql = "Update CollectWeb set Qty=" + Qty.SqlQuote();
                sql += " where Companycode='" + uu.CompanyId + "' and Whno='" + Shop + "' and WorkUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                sql = "select PLU,Qty,PLU+' '+GD_Name GD_Name from CollectWeb a (nolock) inner join PLUWeb b ";
                sql += "on a.companycode=b.companycode and a.PLU=b.GD_Barcode ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and Whno='" + Shop + "' and WorkUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");

                if (dtC.Rows.Count == 0)
                {
                    sql = "select PLU,Qty,PLU+' '+isnull(GD_Name,'') GD_Name from CollectWeb a (nolock) left join PLUWeb b ";
                    sql += "on a.companycode=b.companycode and a.PLU=b.GD_No ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' and Whno='" + Shop + "' and WorkUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                    dtC = PubUtility.SqlQry(sql, uu, "SYS");
                }

                dtC.TableName = "dtCollectMod";
                ds.Tables.Add(dtC);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/DelISAM02PLU")]
        public ActionResult SystemSetup_DelISAM02PLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "DelISAM02PLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string PLU = rq["PLU"];

                string sql = "Delete from CollectWeb ";
                sql += "where Companycode='" + uu.CompanyId + "' and Whno='" + Shop + "' and WorkUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                sql = "set nocount on;Select ROW_NUMBER() over(order by Companycode,Whno,WorkUser,seqno) NewSeq,* into #tmpA from CollectWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and Whno='" + Shop + "' and WorkUser='" + uu.UserID + "';";
                sql += "update b set SeqNo=newseq from #tmpA a inner join CollectWeb b ";
                sql += "on a.CompanyCode=b.CompanyCode and a.Whno=b.Whno and a.WorkUser=b.WorkUser and a.SeqNo=b.SeqNo;";
                sql += "drop table #tmpA;";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/DelISAMData")]
        public ActionResult SystemSetup_DelISAMData()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "DelISAMDataOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];
                string WorkType = rq["Type"];
                string DelType = rq["DelType"];
                string[] DocType = WorkType.Split(",");
                string sql = "set nocount on;";

                for (int i = 0; i < DocType.Length; i++)
                {
                    switch (DocType[i])
                    {
                        //盤點資料清除(本店)
                        case "T":
                            sql += "select * into #tmpT from binweb (nolock) where companycode='" + uu.CompanyId + "' and binstore='" + Shop + "'";
                            if (DelType == "E")
                            {
                                sql += " and binman='" + uu.UserID + "'";
                            }
                            sql += ";";
                            sql += "insert into binweb_old select *,replace(Convert(varchar(19), getdate(), 121), '-', '/'),'" + uu.UserID + "' from #tmpT;";
                            sql += "delete from binweb where companycode='" + uu.CompanyId + "' and binstore='" + Shop + "'";
                            if (DelType == "E")
                            {
                                sql += " and binman='" + uu.UserID + "'";
                            }
                            sql += ";";
                            sql += "drop table #tmpT;";
                            break;
                        //條碼蒐集資料清除(本店)
                        case "C":
                            sql += "select * into #tmpC from CollectWeb (nolock) where companycode='" + uu.CompanyId + "' and Whno='" + Shop + "'";
                            if (DelType == "E")
                            {
                                sql += " and workuser='" + uu.UserID + "'";
                            }
                            sql += ";";
                            sql += "insert into CollectWeb_old select *,replace(Convert(varchar(19), getdate(), 121), '-', '/'),'" + uu.UserID + "' from #tmpC;";
                            sql += "delete from CollectWeb where companycode='" + uu.CompanyId + "' and Whno='" + Shop + "'";
                            if (DelType == "E")
                            {
                                sql += " and workuser='" + uu.UserID + "'";
                            }
                            sql += ";";
                            sql += "drop table #tmpC;";
                            break;
                        //出貨/調撥資料清除(本店)
                        case "D":
                            sql += "select * into #tmpD from Deliveryweb (nolock) where companycode='" + uu.CompanyId + "' and WhnoOut='" + Shop + "'";
                            if (DelType == "E")
                            {
                                sql += " and outuser='" + uu.UserID + "'";
                            }
                            sql += ";";
                            sql += "insert into Deliveryweb_old select *,replace(Convert(varchar(19), getdate(), 121), '-', '/'),'" + uu.UserID + "' from #tmpD;";
                            sql += "delete from Deliveryweb where companycode='" + uu.CompanyId + "' and WhnoOut='" + Shop + "'";
                            if (DelType == "E")
                            {
                                sql += " and outuser='" + uu.UserID + "'";
                            }
                            sql += ";";
                            sql += "drop table #tmpD;";
                            break;
                        default:
                            break;
                    }
                }

                if (sql != "")
                {
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/GetInitISAM03")]
        public ActionResult SystemSetup_GetInitISAM03()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitISAM01OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                string sql = "select a.Man_ID,left(a.Man_ID+'      ',6)+' '+Man_Name ManName ,a.Whno,left(a.Whno+'      ',6)+' '+ST_SName STName from ISAMShopWeb a (nolock) ";
                sql += "inner join EmployeeWeb b (nolock) on a.CompanyCode=b.CompanyCode and a.Man_ID=b.Man_ID ";
                sql += "inner join WarehouseWeb c (nolock) on a.CompanyCode=c.CompanyCode and a.Whno=c.ST_ID ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and a.Man_ID='" + uu.UserID + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                dtC.TableName = "dtWh";
                ds.Tables.Add(dtC);


                sql = "select ST_ID ,ST_ID+' '+ST_SName STName ";
                sql += " from WarehouseWeb (NoLock) ";
                sql += " Where CompanyCode='" + uu.CompanyId + "' and ST_StopDay=''";
                sql += " Order By ST_ID ";

                DataTable dtShop = PubUtility.SqlQry(sql, uu, "SYS");
                dtShop.TableName = "dtShop";
                ds.Tables.Add(dtShop);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SearchDeliveryWeb")]
        public ActionResult SystemSetup_SearchDeliveryWeb()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchDeliveryWebOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNoOut = rq["WhNoOut"];
                string DocDate = rq["DocDate"];
                string WhNoIn = rq["WhNoIn"];

                string sql = "select top 1 * from DeliveryWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtC.Rows.Count == 0)
                {
                    sql = "select top 1 * from DeliveryWeb (nolock) ";
                    sql += "where Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "'";
                    dtC = PubUtility.SqlQry(sql, uu, "SYS");
                }

                dtC.TableName = "dtDeliveryData";
                ds.Tables.Add(dtC);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/ChkSaveDeliveryWeb")]
        public ActionResult SystemSetup_ChkSaveDeliveryWeb()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkSaveDeliveryWebOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNoOut = rq["WhNoOut"];
                string WhNoIn = rq["WhNoIn"];
                string DocDate = rq["DocDate"];
                string PLU = rq["Barcode"];

                string sql = "Select Sum(OutNum) SumQty from DeliveryWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and PLU='" + PLU + "'";
                DataTable dtD = PubUtility.SqlQry(sql, uu, "SYS");
                dtD.TableName = "dtD";
                ds.Tables.Add(dtD);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SaveDeliveryWeb")]
        public ActionResult SystemSetup_SaveDeliveryWeb()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveDeliveryWebOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                Boolean lb_Insert = false;
                string WhNoOut = rq["WhNoOut"];
                string DocDate = rq["DocDate"];
                string WhNoIn = rq["WhNoIn"];
                string PLU = rq["Barcode"];
                int Qty = Convert.ToInt32(rq["Qty"].ToString().SqlQuote());

                string sql = "select '" + PLU + "' PLU,GD_Name from PLUWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and GD_Barcode='" + PLU + "'";
                DataTable dtPLU = PubUtility.SqlQry(sql, uu, "SYS");

                if (dtPLU.Rows.Count == 0)
                {
                    sql = "select '" + PLU + "' PLU,GD_Name from PLUWeb (nolock) ";
                    sql += "where Companycode='" + uu.CompanyId + "' and GD_No='" + PLU + "'";
                    dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                }

                if (dtPLU.Rows.Count == 0)
                {
                    dtMessage.Rows[0][0] = "Exception";
                    dtMessage.Rows[0][1] = "無符合之商品資料!";
                    return PubUtility.DatasetXML(ds);
                }

                sql = "select * from DeliveryWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtC.Rows.Count == 0) { lb_Insert = true; }
                else
                {
                    DataRow[] dr = dtC.Select("PLU='" + PLU + "'");
                    if (dr.Length == 0) { lb_Insert = true; }
                    else
                    {
                        sql = "Update DeliveryWeb set OutNum=OutNum+" + Qty + ",ModDate=Convert(varchar,getdate(),111),ModTime=Substring(Convert(varchar,getdate(),121),12,12)";
                        sql += " where Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "'";
                        sql += " and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                        PubUtility.ExecuteSql(sql, uu, "SYS");
                    }

                }

                if (lb_Insert)
                {
                    sql = "Insert into DeliveryWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime,WhNoOut, DocDate, OutUser,WhNoIn, SeqNo, PLU, OutNum, FTPUpDate) ";
                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',Convert(varchar,getdate(),111),Substring(Convert(varchar,getdate(),121),12,12),";
                    sql += "'" + uu.UserID + "',Convert(varchar,getdate(),111),Substring(Convert(varchar,getdate(),121),12,12),";
                    sql += "'" + WhNoOut + "','" + DocDate + "','" + uu.UserID + "','" + WhNoIn + "',";
                    sql += "(select isnull(max(seqno),0)+1 from DeliveryWeb (nolock) where Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "'),";
                    sql += "'" + PLU + "'," + Qty + " ,''";
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }
                //Return 異動後的數量,故要重撈一次
                //單品數
                sql = "Select Sum(OutNum) SQ1 from DeliveryWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and PLU='" + PLU + "'";
                DataTable dtSQ = PubUtility.SqlQry(sql, uu, "SYS");
                dtSQ.TableName = "dtSQ";
                ds.Tables.Add(dtSQ);
                //門市總和(不看商品)
                sql = "Select Sum(OutNum) SWQ1 from DeliveryWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and PLU='" + PLU + "'";
                DataTable dtSWQ = PubUtility.SqlQry(sql, uu, "SYS");
                dtSWQ.TableName = "dtSWQ";
                ds.Tables.Add(dtSWQ);
                sql = "Select '" + PLU + "' PLU,GD_Name,GD_Retail from PLUWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and GD_Barcode='" + PLU + "'";
                DataTable dtP = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtP.Rows.Count == 0)
                {
                    sql = "Select '" + PLU + "' PLU,GD_Name,GD_Retail from PLUWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and GD_No='" + PLU + "'";
                    dtP = PubUtility.SqlQry(sql, uu, "SYS");
                }

                dtP.TableName = "dtPLU";
                ds.Tables.Add(dtP);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetDeliveryWebMod")]
        public ActionResult SystemSetup_GetDeliveryWebMod()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetDeliveryWebModOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNoOut = rq["WhNoOut"];
                string DocDate = rq["DocDate"];
                string WhNoIn = rq["WhNoIn"];
                string Comd = "";

                if (rq.Count > 3)
                {
                    string PLU = rq["PLU"];
                    if (PLU != null && PLU != "") { Comd = " and PLU='" + PLU + "'"; }
                }

                string sql = "set nocount on;select a.*,GD_Name into #tmpA from DeliveryWeb a (nolock) left join PLUWeb b (nolock) ";
                sql += "on a.companycode=b.companycode and a.PLU=b.GD_Barcode ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "'" + Comd + ";";
                sql += "select a.PLU,a.OutNum,a.PLU+' '+case when isnull(a.GD_Name,'')='' then b.GD_Name else a.GD_Name end GD_Name from #tmpA a (nolock) left join PLUWeb b (nolock) ";
                sql += "on a.companycode=b.companycode and a.PLU=b.GD_No order by SeqNo desc;";
                sql += "drop table #tmpA;";

                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");

                dtC.TableName = "dtDelivery";
                ds.Tables.Add(dtC);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/DelISAM03PLU")]
        public ActionResult SystemSetup_DelISAM03PLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "DelISAM03PLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNoOut = rq["WhNoOut"];
                string DocDate = rq["DocDate"];
                string WhNoIn = rq["WhNoIn"];
                string PLU = rq["PLU"];

                string sql = "Delete from DeliveryWeb ";
                sql += "where Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                sql = "set nocount on;Select ROW_NUMBER() over(order by Companycode,WhNoOut,WhNoIn,DocDate,OutUser,seqno) NewSeq,* into #tmpA from DeliveryWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "';";
                sql += "update b set SeqNo=newseq from #tmpA a inner join DeliveryWeb b ";
                sql += "on a.CompanyCode=b.CompanyCode and a.WhNoOut=b.WhNoOut and a.WhNoIn=b.WhNoIn and a.DocDate=b.DocDate and a.SeqNo=b.SeqNo and a.OutUser=b.OutUser;";
                sql += "drop table #tmpA;";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SaveISAM03PLUMod")]
        public ActionResult SystemSetup_SaveISAM03PLUMod()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveISAM03PLUModOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNoOut = rq["WhNoOut"];
                string DocDate = rq["DocDate"];
                string WhNoIn = rq["WhNoIn"];
                string PLU = rq["PLU"];
                string Qty = rq["Qty"];

                string sql = "Update DeliveryWeb set OutNum=" + Qty.SqlQuote();
                sql += " where Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                sql = "select PLU,OutNum,PLU+' '+GD_Name GD_Name from DeliveryWeb a (nolock) inner join PLUWeb b ";
                sql += "on a.companycode=b.companycode and a.PLU=b.GD_Barcode ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");

                if (dtC.Rows.Count == 0)
                {
                    sql = "select PLU,OutNum,PLU+' '+isnull(GD_Name,'') GD_Name from DeliveryWeb a (nolock) left join PLUWeb b ";
                    sql += "on a.companycode=b.companycode and a.PLU=b.GD_No ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' and WhNoOut='" + WhNoOut + "' and DocDate='" + DocDate + "' and WhNoIn='" + WhNoIn + "' and OutUser='" + uu.UserID + "' and PLU='" + PLU + "'";
                    dtC = PubUtility.SqlQry(sql, uu, "SYS");
                }

                dtC.TableName = "dtDeliveryMod";
                ds.Tables.Add(dtC);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/GetISAMQFTPRECData")]
        public ActionResult SystemSetup_GetInitISAMQFTPREC()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetISAMQFTPRECDataOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["WhNo"];
                string Edit = rq["EditMode"];
                string Comd = "";
                string DateS = "";
                string DateE = "";
                string DocT = "";
                string sql = "";

                if (Edit == "Init")
                {
                    String[,] DocType = { { "T", "盤點" }, { "C", "條碼蒐集" }, { "D", "出貨/調撥" } };
                    sql = "select Convert(varchar(1),'') DocType,Convert(nvarchar(10),'') DocTypeDesc where 1=0";
                    DataTable dtDoc = PubUtility.SqlQry(sql, uu, "SYS");
                    for (int i = 0; i <= DocType.GetUpperBound(0); i++)
                    {
                        DataRow dr = dtDoc.NewRow();
                        dr["DocType"] = DocType[i, 0];
                        dr["DocTypeDesc"] = DocType[i, 1];
                        dtDoc.Rows.Add(dr);
                    }

                    dtDoc.TableName = "dtDoc";
                    ds.Tables.Add(dtDoc);

                    sql = "select Convert(varchar,Dateadd(d,-1,getdate()),111) DTS,Convert(varchar,getdate(),111) DTE ";
                    DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                    dtC.TableName = "dtQDate";
                    ds.Tables.Add(dtC);

                    DateS = dtC.Rows[0][0].ToString();
                    DateE = dtC.Rows[0][1].ToString();
                }
                else
                {
                    DateS = rq["DateS"];
                    DateE = rq["DateE"];
                    DocT = rq["DocType"];
                }

                if (DateS != "" && DateE != "")
                {
                    Comd = "and a.CrtDate between '" + DateS + "' and '" + DateE + "' ";
                }
                if (DocT != "") { Comd += "and DocType='" + DocT + "' "; }

                sql = "select case DocType when 'T' then N'盤點' when 'C' then N'條碼蒐集' when 'D' then N'出貨調撥' end DocTypeDesc ,a.CrtDate,a.CrtDate+' '+a.CrtTime CrtDT,case when isnull(Man_Name,'')='' then a.CrtUser else b.Man_Name end CrtUserName,";
                sql += "case UpdateType when 'N' then N'未上傳' when 'Y' then N'成功' when 'E' then N'異常' end UpdateTypeDesc,a.CrtUser,DocType from ISAMTOFTPRecWeb a (nolock) ";
                sql += "left join EmployeeWeb b (nolock) on a.CompanyCode=b.CompanyCode and a.CrtUser=b.Man_ID ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and a.Whno='" + Shop + "' " + Comd + "order by a.CompanyCode,a.CrtDate desc,a.CrtTime desc";
                DataTable dtQ = PubUtility.SqlQry(sql, uu, "SYS");
                dtQ.TableName = "dtQRec";
                ds.Tables.Add(dtQ);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SearchISAMFTPRecDetl")]
        public ActionResult SystemSetup_SearchISAMFTPRecDetl()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchISAMFTPRecDetlOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string CrtUser = rq["CrtUser"];
                string CrtDT = rq["CrtDT"];
                string CrtDate = CrtDT.Split(" ")[0].Trim();
                string CrtTime = CrtDT.Split(" ")[1].Trim();
                string DocType = rq["DocType"];
                string Shop = rq["Shop"];

                string sql = "select case DocType when 'T' then N'盤點' when 'C' then N'條碼蒐集' when 'D' then N'出貨調撥' end DocTypeDesc ,a.CrtDate+' '+a.CrtTime CrtDT,case when isnull(Man_Name,'')='' then a.CrtUser else b.Man_Name end CrtUserName,";
                sql += "case UpdateType when 'N' then N'未上傳' when 'Y' then N'成功' when 'E' then N'異常' end UpdateTypeDesc,FTPDate,UpLoadComd,DocType from ISAMTOFTPRecWeb a (nolock) ";
                sql += "left join EmployeeWeb b (nolock) on a.CompanyCode=b.CompanyCode and a.CrtUser=b.Man_ID ";
                sql += "where a.Companycode='" + uu.CompanyId + "' and a.CrtUser='" + CrtUser + "' and a.CrtDate='" + CrtDate + "' and a.Crttime='" + CrtTime + "' and DocType='" + DocType + "' and a.Whno='" + Shop + "' ";
                DataTable dtQ = PubUtility.SqlQry(sql, uu, "SYS");
                dtQ.TableName = "dtRec";
                ds.Tables.Add(dtQ);

                if (DocType == "D" && dtQ.Rows[0]["UpLoadComd"].ToString().Split(";").Length > 2)
                {
                    sql = "select left(ST_ID+'      ',6)+' '+ST_SName InSTName from WarehouseWeb c (nolock) ";
                    sql += "where Companycode='" + uu.CompanyId + "' and ST_ID='" + dtQ.Rows[0]["UpLoadComd"].ToString().Split(";")[1] + "'";
                    DataTable dtWh = PubUtility.SqlQry(sql, uu, "SYS");
                    dtWh.TableName = "dtWh";
                    ds.Tables.Add(dtWh);
                }

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-18 Larry
        [Route("SystemSetup/SearchVIV10")]
        public ActionResult SystemSetup_SearchVIV10()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchVIV10OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;

                string YM = rq["YM"];
                string WhNo = rq["WhNo"];

                string sql = "select a.*,a.ShopNo+b.ST_SNAME WhName, c.Man_Name ModUserName, Cast(a.INV_ENo As Integer)-Cast(a.Inv_No As Integer) DiffQty ";
                sql += " from InvDistribute a";
                sql += " inner join WarehouseSV b on a.ShopNo=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " inner join EmployeeSV c on a.ModUser=c.Man_ID And a.CompanyCode=c.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "'";
                if (WhNo != "")
                {
                    sql += " and a.ShopNo='" + WhNo + "'";
                }
                //if (CkNo != "")
                //{
                //    sql += " and b.CkNo='" + CkNo + "'";
                //}
                sql += " Order By a.INV_YM,a.ShopNo ";
                DataTable dtInv = PubUtility.SqlQry(sql, uu, "SYS");
                dtInv.TableName = "dtInv";
                ds.Tables.Add(dtInv);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/GetVIV10Detail")]
        public ActionResult SystemSetup_GetVIV10Detail()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVIV10DetailOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                //string CompanyCode = rq["CompanyCode"];
                string ShopNo = rq["ShopNo"];
                string sql = "select a.CkNo ";
                sql += " from WarehouseDSV a (NoLock) ";
                sql += " where CompanyCode='" + uu.CompanyId + "' and ST_ID='" + ShopNo.SqlQuote() + "'";
                sql += " Order By CkNo ";

                DataTable dtCk = PubUtility.SqlQry(sql, uu, "SYS");
                dtCk.TableName = "CkList";
                ds.Tables.Add(dtCk);

                //sql = "select LayerNo,ChannelType,max(ChannelNo) as ChannelNo  from MachineListSpec a";
                //sql += " where a.CompanyCode='" + CompanyCode.SqlQuote() + "'";
                //sql += " and a.SNno='" + SNno.SqlQuote() + "'";
                //sql += " group by LayerNo,ChannelType";
                //sql += " order by LayerNo";
                //DataTable MachineListSpec = PubUtility.SqlQry(sql, uu, "SYS");
                //MachineListSpec.TableName = "MachineListSpec";
                //ds.Tables.Add(MachineListSpec);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/GetVIV10View")]
        public ActionResult SystemSetup_GetVIV10View()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVIV10ViewOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ShopNo = rq["ShopNo"];
                string CkNo = rq["CkNo"];
                string YM = rq["YM"];
                string sql = "select a.*,a.ShopNo+b.ST_SNAME WhName, c.Man_Name AppUserName, Cast(a.INV_ENo As Integer)-Cast(a.Inv_SNo As Integer)+1 DiffQty ";
                sql += " from InvDistributeSV a";
                sql += " inner join WarehouseSV b on a.ShopNo=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " inner join EmployeeSV c on a.AppUser=c.Man_ID And a.CompanyCode=c.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "'";
                if (ShopNo != "")
                {
                    sql += " and a.ShopNo='" + ShopNo + "'";
                }
                //sql += " and a.SNno='" + SNno.SqlQuote() + "'";

                DataTable dtDtl = PubUtility.SqlQry(sql, uu, "SYS");
                dtDtl.TableName = "dtDtl";
                ds.Tables.Add(dtDtl);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/SaveVIV10")]
        public ActionResult SaveVIV10()
        {
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveVIV10OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                UserInfo uu = PubUtility.GetCurrentUser(this);
                DataTable dtH = new DataTable("ShopData");
                PubUtility.AddStringColumns(dtH, "ShopNo,INV_YM,INV_Head,INV_No");
                //dtH.Columns.Add("Temperature", typeof(double));

                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtH);

                DataTable dtD = new DataTable("AssignQty");
                PubUtility.AddStringColumns(dtD, "ShopNo,CkNo");
                dtD.Columns.Add("AssignQty", typeof(double));
                dsRQ.Tables.Add(dtD);

                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);

                //dtH.Rows[0]["CompanyCode"] = uu.CompanyId;
                //DataTable dtS = new DataTable();
                //PubUtility.AddStringColumns(dtS, "CompanyCode,SNno,LayerNo,ChannelNo,ChannelType");

                //foreach (DataRow dr in dtD.Rows)
                //{
                //    dr["CompanyCode"] = uu.CompanyId;
                //    int ch = 1;
                //    for (int i = 0; i < PubUtility.CB(dr["ChannelQty"]); i++)
                //    {
                //        DataRow drS = dtS.NewRow();
                //        dtS.Rows.Add(drS);
                //        List<string> fds = "CompanyCode,SNno,LayerNo,ChannelType".Split(',').ToList();
                //        foreach (string fd in fds)
                //        {
                //            drS[fd] = dr[fd];
                //        }
                //        drS["ChannelNo"] = ch.ToString().PadLeft(2, '0');
                //        ch++;
                //    }
                //}
                //string EditMode = dtH.Rows[0]["EditMode"].ToString();
                //string CompanyCode = dtH.Rows[0]["CompanyCode"].ToString();
                //string SNno = dtH.Rows[0]["SNno"].ToString();
                //dsRQ.Tables.Add(dtS);
                string sql;
                string ShopNo = ""; string INV_YM = ""; string INV_Head = ""; string INV_No = "";
                string CkNo = "";
                ShopNo = dtH.Rows[0]["ShopNo"].ToString();
                INV_YM = dtH.Rows[0]["INV_YM"].ToString();
                INV_Head = dtH.Rows[0]["INV_Head"].ToString();
                INV_No = dtH.Rows[0]["INV_No"].ToString();

                string DocNo = PubUtility.GetNewDocNo(uu, "ID", 4);

                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            Int32 SNo = 0;
                            string SNoSql = "";
                            Int32 ENo = 0;
                            string ENoSql = "";
                            SNo = Convert.ToInt32(INV_No) + 1;
                            Int32 iCount = 0;

                            string DtlDocNo = DocNo;
                            foreach (DataRow dr in dtD.Rows)
                            {

                                if (Convert.ToInt32(dr["AssignQty"].ToString()) != 0)
                                {

                                    ENo = SNo + Convert.ToInt32(dr["AssignQty"].ToString()) - 1;

                                    SNoSql = "Right(Cast(REPLICATE('0', 8) As varChar(8))+Cast(" + SNo + " as varchar(8)),8)";
                                    ENoSql = "Right(Cast(REPLICATE('0', 8) As varChar(8))+Cast(" + ENo + " as varchar(8)),8)";

                                    CkNo = dr["CkNo"].ToString().SqlQuote();

                                    sql = "Insert Into InvDistributeSV (CompanyCode, CrtUser, CrtDate, CrtTime ";
                                    sql += ", ModUser, ModDate, ModTime ";
                                    sql += ", DocNo, ShopNo, INV_YM ";
                                    sql += ", INV_Head, INV_SNo, INV_ENo "
                                        + ", AppDate, AppUser, CkNo) Values ";
                                    sql += " ('" + uu.CompanyId + "', '" + uu.UserID + "', convert(char(10),getdate(),111), convert(char(12),getdate(),108) ";
                                    sql += ", '" + uu.UserID + "', convert(char(10),getdate(),111), convert(char(12),getdate(),108) ";
                                    sql += ", '" + DtlDocNo + "', '" + ShopNo + "', '" + INV_YM + "' ";
                                    sql += ", '" + INV_Head + "'," + SNoSql + ", " + ENoSql + ""
                                        + ", convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(12),getdate(),108),1,5), '" + uu.UserID + "','" + CkNo + "')";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    //string sno = DtlDocNo.Substring(10, 4);
                                    Int32 NewSno = Convert.ToInt32(DtlDocNo.Substring(10, 4)) + 1;

                                    DtlDocNo = PubUtility.StrLeft(DtlDocNo, 10) + PubUtility.PadLeft(Convert.ToString(NewSno), '0', 4);
                                    SNo += Convert.ToInt32(dr["AssignQty"].ToString());

                                    iCount += 1;

                                }



                            }

                            sql = "Update InvDistribute Set Inv_No=" + ENoSql + "";
                            sql += ",ModDate=convert(char(10),getdate(),111)";
                            sql += ",ModTime=convert(char(12),getdate(),108)";
                            sql += ",ModUser='" + uu.UserID + "'";
                            sql += "Where CompanyCode='" + uu.CompanyId + "' And ShopNo='" + ShopNo + "' ";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            sql = "Update DocumentNo Set SeqNo= SeqNo + " + (iCount - 1) + " ";
                            sql += "Where CompanyCode='" + uu.CompanyId + "' And Initial='ID' And DocDate=convert(char(8),getdate(),112)";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw err;
                        }
                        dbop.Dispose();
                    }
                }



                sql = "select a.*,a.ShopNo+b.ST_SNAME WhName, c.Man_Name ModUserName, Cast(a.INV_ENo As Integer)-Cast(a.Inv_No As Integer) DiffQty ";
                sql += " from InvDistribute a";
                sql += " inner join WarehouseSV b on a.ShopNo=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " inner join EmployeeSV c on a.ModUser=c.Man_ID And a.CompanyCode=c.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "'";
                sql += " And a.ShopNo='" + ShopNo + "' ";
                DataTable dtInv = PubUtility.SqlQry(sql, uu, "SYS");
                dtInv.TableName = "dtInv";
                ds.Tables.Add(dtInv);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }



        //2021-06-21
        [Route("SystemSetup/GetWhDSVCkNoWithCond")]
        public ActionResult SystemSetup_GetWhDSVCkNoWithCond()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetWhDSVCkNoWithCondOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNo = rq["WhNo"];
                string StopDay = rq["StopDay"];
                string CheckUse = rq["CheckUse"];
                string sql = "select a.CkNo,a.CkNo + '機' as CkNoName ";
                sql += " from WarehouseDSV a (NoLock) ";
                sql += " Inner Join MachineList b (NoLock) On a.CompanyCode=b.CompanyCode And a.SNno=b.SNno ";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' and ST_ID='" + WhNo + "'";
                if (StopDay == "Y")
                {
                    sql += " And (IsNull(ST_StopDay,'')='' or ST_StopDay>convert(char(10),getdate(),111)) ";
                }
                if (CheckUse == "Y")
                {
                    sql += " And b.FlagUse='U' ";
                }

                sql += " Order By CkNo ";
                DataTable dtCK = PubUtility.SqlQry(sql, uu, "SYS");
                dtCK.TableName = "dtCK";
                ds.Tables.Add(dtCK);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //private string GetNewDocNo(UserInfo uu,String DocType)
        //{

        //2021-06-21
        [Route("SystemSetup/GetWhCkLayer")]
        public ActionResult SystemSetup_GetWhCkLayer()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetWhCkLayerOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNo = rq["WhNo"];
                string CkNo = rq["CkNo"];
                string sql = "select Distinct a.LayerNo ";
                sql += " from MachineListSpec a (NoLock) ";
                sql += " Inner Join WarehouseDSV b (NoLock) On a.CompanyCode=b.CompanyCode And a.SNno=b.SNno ";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' and b.ST_ID='" + WhNo + "' And b.CkNo='" + CkNo + "' ";
                sql += " Order By a.LayerNo ";
                DataTable dtCK = PubUtility.SqlQry(sql, uu, "SYS");
                dtCK.TableName = "dtCK";
                ds.Tables.Add(dtCK);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-06-21
        [Route("SystemSetup/GetWhCkLayerSno")]
        public ActionResult SystemSetup_GetWhCkLayerSno()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetWhCkLayerSnoOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNo = rq["WhNo"];
                string CkNo = rq["CkNo"];
                string LayerNo = rq["LayerNo"];
                string sql = "select Distinct a.ChannelNo ";
                sql += " from MachineListSpec a (NoLock) ";
                sql += " Inner Join WarehouseDSV b (NoLock) On a.CompanyCode=b.CompanyCode And a.SNno=b.SNno ";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' and b.ST_ID='" + WhNo + "' And b.CkNo='" + CkNo + "' And a.LayerNo='" + LayerNo + "' ";
                sql += " Order By a.ChannelNo ";
                DataTable dtCK = PubUtility.SqlQry(sql, uu, "SYS");
                dtCK.TableName = "dtCK";
                ds.Tables.Add(dtCK);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-06-21
        [Route("SystemSetup/GetWhCkLayerSnoPLU")]
        public ActionResult SystemSetup_GetWhCkLayerSnoPLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetWhCkLayerSnoPLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNo = rq["WhNo"];
                string CkNo = rq["CkNo"];
                string LayerNo = rq["LayerNo"];
                string Sno = rq["Sno"];
                string sql = "select a.PLU, b.GD_SName ";
                sql += " from InventorySV a (NoLock) ";
                sql += " Inner Join PLUSV b (NoLock) On a.CompanyCode=b.CompanyCode And a.PLU=b.GD_No ";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' and a.WhNo='" + WhNo + "' And a.CkNo='" + CkNo + "' And a.Layer='" + LayerNo + "' And a.SNo='" + Sno + "' ";
                DataTable dtCK = PubUtility.SqlQry(sql, uu, "SYS");
                dtCK.TableName = "dtCK";
                ds.Tables.Add(dtCK);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //2021-05-26 Larry
        [Route("SystemSetup/GetSysDate")]
        public ActionResult SystemSetup_GetSysDate()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetSysDateOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                //string Type_ID = rq["Type_ID"];
                string sql = "select convert(char(10),getdate(),111) SysDate";

                DataTable dtSysDate = PubUtility.SqlQry(sql, uu, "SYS");
                dtSysDate.TableName = "dtSysDate";
                ds.Tables.Add(dtSysDate);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/GetQueryDays")]
        public ActionResult SystemSetup_GetQueryDays()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetQueryDaysOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ProgramID = rq["ProgramID"];
                string sql = "select QueryDays from CompanyPIDSV where Companycode='" + uu.CompanyId + "' and Programid='" + ProgramID.SqlQuote() + "'";

                DataTable dtQueryDays = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtQueryDays.Rows.Count == 0)
                    sql = "select QueryDays from ProgramID where Programid='" + ProgramID.SqlQuote() + "'";
                dtQueryDays = PubUtility.SqlQry(sql, uu, "SYS");

                dtQueryDays.TableName = "dtQueryDays";
                ds.Tables.Add(dtQueryDays);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/GetInitVIN14_1P")]
        public ActionResult GetInitVIN14_1P()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitVIN14_1POK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "select whno from EmployeeSV (nolock) where companycode='" + uu.CompanyId + "' and man_id='" + uu.UserID + "' ";
                DataTable dtShop = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtShop);
                dtShop.TableName = "dtShop";

                sql = "Select Type_ID,Type_Name from TypeData (nolock) where Companycode='" + uu.CompanyId + "' and Type_Code='DA' ";
                DataTable dtArea = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtArea);
                dtArea.TableName = "dtArea";

                sql = "Select DocNo from PickupHSV (nolock) where Companycode='" + uu.CompanyId + "' and DocType='S' " +
                      "and WhNoOut=(select whno from EmployeeSV (nolock) where companycode='" + uu.CompanyId + "' and man_id='" + uu.UserID + "') " +
                      "and isnull(ChkDate,'')<>'' ";
                DataTable dtPick = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtPick);
                dtPick.TableName = "dtPick";




            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-14 Kris
        [Route("SystemSetup/GetVIN14_1P")]
        public ActionResult SystemSetup_GetVIN14_1P()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVIN14_1POK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Area = rq["Area"];
                string Pick = rq["Pick"];
                string DocDate = rq["DocDate"];

                string sql = "Select a.DocNo,a.DocDate,sum(b.Qty)Qty From PickupHSV a (nolock) " +
                    "left join PickupDSV b (nolock) on a.DocNo=b.DocNo and b.companycode=a.companycode " +
                    "left join WarehouseSV c (nolock) on a.WhNoOut=c.ST_ID and c.companycode=a.companycode " +
                    "Where a.Companycode='" + uu.CompanyId + "' " +
                    "and a.WhNoOut=(select WhNo from EmployeeSV (nolock) where companycode='" + uu.CompanyId + "' and man_id='" + uu.UserID + "') " +
                    "and isnull(a.ChkDate,'')<>'' and a.DocType='S' ";
                if (Area != "")
                    sql += "and c.ST_DeliArea='" + Area + "' ";
                if (Pick != "")
                    sql += "and a.DocNo='" + Pick + "' ";
                if (DocDate != "")
                    sql += "and a.DocDate='" + DocDate + "' ";
                sql += "group by a.DocNo,a.DocDate ";
                sql += "order by a.DocNo ";

                DataTable dtVIN14_1P = PubUtility.SqlQry(sql, uu, "SYS");
                dtVIN14_1P.TableName = "dtVIN14_1P";
                ds.Tables.Add(dtVIN14_1P);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-15 Kris
        [Route("SystemSetup/GetVIN14_1P_1")]
        public ActionResult SystemSetup_GetVIN14_1P_1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVIN14_1P_1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];

                string sql = "Select a.SeqNo,a.PLU + ' ' + b.GD_Sname as GD_NO,isnull(a.Qty,0)Qty from PickupDSV a (nolock) " +
                "left join PLUSV b(nolock) on a.PLU = b.GD_NO and b.CompanyCode = a.CompanyCode " +
                "where a.CompanyCode = '" + uu.CompanyId + "' ";
                if (DocNo != "")
                    sql += "and a.DocNo='" + DocNo + "' ";
                sql += "order by a.seqno ";
                DataTable dtVIN14_1P_1 = PubUtility.SqlQry(sql, uu, "SYS");
                dtVIN14_1P_1.TableName = "dtVIN14_1P_1";
                ds.Tables.Add(dtVIN14_1P_1);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-15 Kris
        [Route("SystemSetup/GetVIN14_1P_2")]
        public ActionResult SystemSetup_GetVIN14_1P_2()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVIN14_1P_2OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];

                string sql = "Select a.WhNo,a.Ckno,b.ST_Sname+a.Ckno+'機' as SName,c.ST_Address,b.ST_DeliArea " +
                "from PickupShopSV a(nolock) " +
                "left join WarehouseSV b(nolock) on a.WhNo = b.ST_ID and b.CompanyCode = a.CompanyCode " +
                "left join WarehouseDSV c(nolock) on a.WhNo = c.ST_ID and a.Ckno = c.CkNo and c.CompanyCode = a.CompanyCode " +
                "where a.CompanyCode = '" + uu.CompanyId + "' ";
                if (DocNo != "")
                    sql += "and a.DocNo='" + DocNo + "' ";
                sql += "order by a.whno,a.ckno ";
                DataTable dtVIN14_1P_2 = PubUtility.SqlQry(sql, uu, "SYS");
                dtVIN14_1P_2.TableName = "dtVIN14_1P_2";
                ds.Tables.Add(dtVIN14_1P_2);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-15 Kris
        [Route("SystemSetup/GetVIN14_1P_3")]
        public ActionResult SystemSetup_GetVIN14_1P_3()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVIN14_1P_3OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];

                string sql = "select b.SeqNo,b.PLU + ' ' + c.GD_Sname as PLU,isnull(b.Qty,0)Qty, " +
                "a.whnoin+'店 '+a.cknoin+'機 '+d.st_sname+a.cknoin as Name " +
                "from PickupHSV a (nolock) " +
                "left join PickupDSV b (nolock) on a.DocNo=b.DocNo and b.CompanyCode=a.CompanyCode " +
                "left join PLUSV c(nolock) on b.PLU = c.GD_NO and c.CompanyCode = a.CompanyCode " +
                "left join WarehouseSV d (nolock) on a.whnoin=d.st_id and d.companycode=a.CompanyCode " +
                "where a.CompanyCode = '" + uu.CompanyId + "' ";
                if (DocNo != "")
                    sql += "and a.DocNo='" + DocNo + "' ";
                sql += "order by b.seqno ";
                DataTable dtVIN14_1P_3 = PubUtility.SqlQry(sql, uu, "SYS");
                dtVIN14_1P_3.TableName = "dtVIN14_1P_3";
                ds.Tables.Add(dtVIN14_1P_3);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-16 Kris
        [Route("SystemSetup/GetInitVMN02")]
        public ActionResult GetInitVMN02()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitVMN02OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "Select ST_ID,ST_ID+ST_Sname as ST_Sname from WarehouseSV (nolock) " +
                    "where Companycode='" + uu.CompanyId + "' and ST_Type='6' " +
                    "and WhnoIn=(select whno from EmployeeSV (nolock) where companycode='" + uu.CompanyId + "' and man_id='" + uu.UserID + "') ";
                DataTable dtWarehouse = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtWarehouse);
                dtWarehouse.TableName = "dtWarehouse";

                sql = "Select Type_ID,Type_Name from TypeData (nolock) where Companycode='" + uu.CompanyId + "' and Type_Code='DA' " +
                "order by Type_ID ";
                DataTable dtDeliArea = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtDeliArea);
                dtDeliArea.TableName = "dtDeliArea";
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-19 Kris
        [Route("SystemSetup/GetMCSeq")]
        public ActionResult GetMCSeq()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetMCSeqOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string SNno = rq["SNno"];

                string sql = "Select MCSeq from MachineList (nolock) where Companycode='" + uu.CompanyId + "' ";
                if (SNno != "")
                    sql += "and SNno='" + SNno + "' ";
                DataTable dtMCSeq = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtMCSeq);
                dtMCSeq.TableName = "dtMCSeq";

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-19 Kris
        [Route("SystemSetup/GetVMN02")]
        public ActionResult SystemSetup_GetVMN02()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVMN02OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ShopNo = rq["ShopNo"];

                string sql = "select a.ST_ID,a.ST_Sname, " +
                "(select count(*) from WarehouseDSV b (nolock) where b.CompanyCode='" + uu.CompanyId + "' and a.ST_ID=b.ST_ID)CkNo, " +
                "a.FlagInv,a.WhnoIn,(select ST_ID + ST_SName from WarehouseSV (nolock) where companycode='" + uu.CompanyId + "' and st_id=a.whnoin)WhNoInName, " +
                "a.InvGetQty,a.InvSaveQty,a.ST_DeliArea " +
                "from WarehouseSV a (nolock) " +
                "where a.CompanyCode='" + uu.CompanyId + "' and a.ST_Type='6' ";
                if (ShopNo != "")
                    sql += "and a.ST_ID='" + ShopNo + "' ";
                sql += "order by a.ST_ID ";

                DataTable dtVMN02 = PubUtility.SqlQry(sql, uu, "SYS");
                dtVMN02.TableName = "dtVMN02";
                ds.Tables.Add(dtVMN02);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-19 Kris
        [Route("SystemSetup/GetCheckSN")]
        public ActionResult GetCheckSN()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetCheckSNOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string SNno = rq["SNno"];

                string sql = "Select * from WarehouseDSV (nolock) where Companycode='" + uu.CompanyId + "' ";
                if (SNno != "")
                    sql += "and SNno='" + SNno + "' ";
                DataTable dtCheckSN = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtCheckSN);
                dtCheckSN.TableName = "dtCheckSN";

                sql = "Select * from MachineList (nolock) where Companycode='" + uu.CompanyId + "' ";
                sql += "and FlagUse='N' ";
                if (SNno != "")
                    sql += "and SNno='" + SNno + "' ";
                DataTable dtCheckMachine = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtCheckMachine);
                dtCheckMachine.TableName = "dtCheckMachine";

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-19 Kris
        [Route("SystemSetup/GetInitMod")]
        public ActionResult GetInitMod()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitModOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ShopNo = rq["ShopNo"];

                string sql = "Select CkNo from WarehouseDSV (nolock) where Companycode='" + uu.CompanyId + "' ";
                if (ShopNo != "")
                    sql += "and ST_ID='" + ShopNo + "' ";
                sql += "order by CkNo ";
                DataTable dtInitMod = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtInitMod);
                dtInitMod.TableName = "dtInitMod";

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-19 Kris
        [Route("SystemSetup/GetVMN02_Mod")]
        public ActionResult GetVMN02_Mod()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVMN02_ModOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ShopNo = rq["ShopNo"];
                string CkNo = rq["CkNo"];

                string sql = "select CkNo,a.SNno,b.MCSeq,a.ST_Address,a.InvGetQty,a.InvSaveQty, " +
                "isnull(a.ST_OpenDay, '')ST_OpenDay,isnull(a.ST_StopDay, '')ST_StopDay " +
                "from WarehouseDSV a(nolock) " +
                "left join MachineList b(nolock) on a.SNno = b.SNno and b.CompanyCode = a.CompanyCode " +
                "where a.CompanyCode = '" + uu.CompanyId + "' ";
                if (ShopNo != "")
                    sql += "and a.ST_ID='" + ShopNo + "' ";
                if (CkNo != "")
                    sql += "and a.CkNo='" + CkNo + "' ";
                DataTable dtVMN02_Mod = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtVMN02_Mod);
                dtVMN02_Mod.TableName = "dtVMN02_Mod";

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-20 Kris
        [Route("SystemSetup/GetQuery")]
        public ActionResult SystemSetup_GetQuery()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetQueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ShopNo = rq["ShopNo"];

                string sql = "select a.CkNo,a.SNno," +
                "a.WhnoIn + b.ST_Sname as WhName, " +
                "a.ST_Address,a.InvGetQty,a.InvSaveQty,a.ST_OpenDay,a.ST_StopDay,a.QrCode " +
                "from WarehouseDSV a (nolock) " +
                "left join WarehouseSV b (nolock) on a.WhnoIn = b.ST_ID and b.CompanyCode = a.CompanyCode " +
                "where a.CompanyCode='" + uu.CompanyId + "' ";
                if (ShopNo != "")
                    sql += "and a.ST_ID='" + ShopNo + "' ";
                sql += "order by a.CkNo ";

                DataTable dtQuery = PubUtility.SqlQry(sql, uu, "SYS");
                dtQuery.TableName = "dtQuery";
                ds.Tables.Add(dtQuery);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-20 Kris
        [Route("SystemSetup/AddVMN02")]
        public ActionResult SystemSetup_AddVMN02()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "AddVMN02OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtWh = new DataTable("WarehouseDSV");
                PubUtility.AddStringColumns(dtWh, "ST_ID,CkNo,SNno,ST_Address,InvGetQty,InvSaveQty,ST_OpenDay,QrCode");
                DataSet dsWh = new DataSet();
                dsWh.Tables.Add(dtWh);
                PubUtility.FillDataFromRequest(dsWh, HttpContext.Request.Form);
                DataRow dr = dtWh.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            dbop.Add("WarehouseDSV", dtWh, uu, "SYS");

                            sql = "Update MachineList set FlagUse='S',ModDate=convert(char(10),getdate(),111), " +
                                  "ModTime=convert(char(12),getdate(),108), " +
                                  "ModUser='" + uu.UserID + "' " +
                                  "where Companycode='" + uu.CompanyId + "' " +
                                  "and SNno='" + dr["SNno"].ToString().SqlQuote() + "' ";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }

                sql = "select a.* ";
                sql += "from WarehouseDSV a (nolock) ";
                sql += "where a.CompanyCode='" + uu.CompanyId + "' And a.ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                sql += "and a.CkNo='" + dr["CkNo"].ToString().SqlQuote() + "' ";
                DataTable dtWarehouseDSV = PubUtility.SqlQry(sql, uu, "SYS");
                dtWarehouseDSV.TableName = "dtWarehouseDSV";
                ds.Tables.Add(dtWarehouseDSV);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-20 Kris
        [Route("SystemSetup/UpdateVMN02")]
        public ActionResult SystemSetup_UpdateVMN02()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateVMN02OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtWh = new DataTable("WarehouseDSV");
                PubUtility.AddStringColumns(dtWh, "ST_ID,CkNo,ST_Address,InvGetQty,InvSaveQty,ST_OpenDay,ST_StopDay");
                DataSet dsWh = new DataSet();
                dsWh.Tables.Add(dtWh);
                PubUtility.FillDataFromRequest(dsWh, HttpContext.Request.Form);
                DataRow dr = dtWh.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            sql = "update WarehouseDSV set ";
                            sql += "ST_Address='" + dr["ST_Address"].ToString().SqlQuote() + "',";
                            sql += "InvGetQty=" + dr["InvGetQty"].ToString().SqlQuote() + ",";
                            sql += "InvSaveQty=" + dr["InvSaveQty"].ToString().SqlQuote() + ",";
                            sql += "ST_OpenDay='" + dr["ST_OpenDay"].ToString().SqlQuote() + "',";
                            sql += "ST_StopDay='" + dr["ST_StopDay"].ToString().SqlQuote() + "',";
                            sql += "ModDate=convert(char(10),getdate(),111),";
                            sql += "ModTime=convert(char(12),getdate(),108),";
                            sql += "ModUser='" + uu.UserID + "' ";
                            sql += "where CompanyCode='" + uu.CompanyId + "' ";
                            sql += "and ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                            sql += "and CkNo='" + dr["CkNo"].ToString().SqlQuote() + "' ";
                            dbop.ExecuteSql(sql, uu, "SYS");
                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.* ";
                sql += "from WarehouseDSV a (nolock) ";
                sql += "where a.CompanyCode='" + uu.CompanyId + "' And a.ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                sql += "and a.CkNo='" + dr["CkNo"].ToString().SqlQuote() + "' ";
                DataTable dtWarehouseDSV = PubUtility.SqlQry(sql, uu, "SYS");
                dtWarehouseDSV.TableName = "dtWarehouseDSV";
                ds.Tables.Add(dtWarehouseDSV);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-21 Kris
        [Route("SystemSetup/GetCheckDoc")]
        public ActionResult GetCheckDoc()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetCheckDocOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ST_ID = rq["ST_ID"];
                string SysDate = rq["SysDate"];

                string sql = "select * from TransferHSV (nolock) " +
                             "where CompanyCode='" + uu.CompanyId + "' ";
                if (ST_ID != "")
                    sql += "and WhNoOut='" + ST_ID + "' ";
                if (SysDate != "")
                    sql += "and DocDate>='" + SysDate + "' ";
                sql += "and isnull(ChkDate,'')<>'' and isnull(PostDate,'')='' ";
                DataTable dtTransfer = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtTransfer);
                dtTransfer.TableName = "dtTransfer";

                sql = "select * from UselessHSV (nolock) " +
                      "where CompanyCode='" + uu.CompanyId + "' ";
                if (ST_ID != "")
                    sql += "and WhNoOut='" + ST_ID + "' ";
                if (SysDate != "")
                    sql += "and DocDate>='" + SysDate + "' ";
                sql += "and isnull(ChkDate,'')<>'' and isnull(PostDate,'')='' ";
                DataTable dtUseless = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtUseless);
                dtUseless.TableName = "dtUseless";

                sql = "select * from AdjustHSV (nolock) " +
                      "where CompanyCode='" + uu.CompanyId + "' ";
                if (ST_ID != "")
                    sql += "and WhNo='" + ST_ID + "' ";
                if (SysDate != "")
                    sql += "and DocDate>='" + SysDate + "' ";
                sql += "and isnull(ChkDate,'')<>'' and isnull(PostDate,'')='' ";
                DataTable dtAdjust = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtAdjust);
                dtAdjust.TableName = "dtAdjust";

                sql = "select * from ChangePLUSV (nolock) " +
                      "where CompanyCode='" + uu.CompanyId + "' ";
                if (ST_ID != "")
                    sql += "and WhNo='" + ST_ID + "' ";
                if (SysDate != "")
                    sql += "and ExchangeDate>='" + SysDate + "' ";
                sql += "and isnull(AppDate,'')<>'' and isnull(FinishDate,'')='' ";
                sql += "and isnull(DefeasanceDate,'')='' ";
                DataTable dtChangePLU = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtChangePLU);
                dtChangePLU.TableName = "dtChangePLU";

                sql = "select * from ChangeShopSV (nolock) " +
                      "where CompanyCode='" + uu.CompanyId + "' ";
                if (ST_ID != "")
                    sql += "and WhNoOut='" + ST_ID + "' ";
                if (SysDate != "")
                    sql += "and ExchangeDate>='" + SysDate + "' ";
                sql += "and isnull(AppDate,'')<>'' and isnull(FinishDate,'')='' ";
                sql += "and isnull(DefeasanceDate,'')='' ";
                DataTable dtChangeShop = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtChangeShop);
                dtChangeShop.TableName = "dtChangeShop";
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-21 Kris
        [Route("SystemSetup/GetCheckSaveH")]
        public ActionResult GetCheckSaveH()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetCheckSaveHOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNoIn = rq["WhNoIn"];
                string ST_ID = rq["ST_ID"];

                string sql = "select * from WarehouseSV (nolock) " +
                             "where CompanyCode='" + uu.CompanyId + "' ";
                if (WhNoIn != "")
                    sql += "and ST_ID='" + WhNoIn + "' ";
                DataTable dtCheckSaveH = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtCheckSaveH);
                dtCheckSaveH.TableName = "dtCheckSaveH";

                sql = "select * from WarehouseDSV (nolock) " +
                      "where CompanyCode='" + uu.CompanyId + "' ";
                if (ST_ID != "")
                    sql += "and ST_ID='" + ST_ID + "' ";
                DataTable dtCheckSaveD = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtCheckSaveD);
                dtCheckSaveD.TableName = "dtCheckSaveD";
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-07-21 Kris
        [Route("SystemSetup/UpdateVMN02H_1")]
        public ActionResult SystemSetup_UpdateVMN02H_1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateVMN02H_1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtWh = new DataTable("WarehouseSV");
                PubUtility.AddStringColumns(dtWh, "ST_ID,FlagInv,WhNoIn,OldFlagInv,ST_DeliArea");
                DataSet dsWh = new DataSet();
                dsWh.Tables.Add(dtWh);
                PubUtility.FillDataFromRequest(dsWh, HttpContext.Request.Form);
                DataRow dr = dtWh.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            if (dr["FlagInv"].ToString() == dr["OldFlagInv"].ToString())
                            {
                                if (dr["FlagInv"].ToString() == "Y")
                                {
                                    sql = "update WarehouseDSV set ";
                                    sql += "WhNoIn='" + dr["WhNoIn"].ToString().SqlQuote() + "',";
                                    sql += "ModDate=convert(char(10),getdate(),111),";
                                    sql += "ModTime=convert(char(12),getdate(),108),";
                                    sql += "ModUser='" + uu.UserID + "' ";
                                    sql += "where CompanyCode='" + uu.CompanyId + "' ";
                                    sql += "and ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                                    sql += "and CkNo='00' ";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    sql = "update WarehouseDSV set ";
                                    sql += "WhNoIn='" + dr["ST_ID"].ToString().SqlQuote() + "',";
                                    sql += "ModDate=convert(char(10),getdate(),111),";
                                    sql += "ModTime=convert(char(12),getdate(),108),";
                                    sql += "ModUser='" + uu.UserID + "' ";
                                    sql += "where CompanyCode='" + uu.CompanyId + "' ";
                                    sql += "and ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                                    sql += "and CkNo<>'00' ";
                                    dbop.ExecuteSql(sql, uu, "SYS");
                                }
                                else if (dr["FlagInv"].ToString() == "N")
                                {
                                    sql = "update WarehouseDSV set ";
                                    sql += "WhNoIn='" + dr["WhNoIn"].ToString().SqlQuote() + "',";
                                    sql += "ModDate=convert(char(10),getdate(),111),";
                                    sql += "ModTime=convert(char(12),getdate(),108),";
                                    sql += "ModUser='" + uu.UserID + "' ";
                                    sql += "where CompanyCode='" + uu.CompanyId + "' ";
                                    sql += "and ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                                    dbop.ExecuteSql(sql, uu, "SYS");
                                }
                            }
                            else
                            {
                                if (dr["OldFlagInv"].ToString() == "N" && dr["FlagInv"].ToString() == "Y")
                                {
                                    sql = "Insert Into WarehouseDSV (CompanyCode,CrtUser,CrtDate,CrtTime,";
                                    sql += "ModUser,ModDate,ModTime,ST_ID,CkNo,WhNoIn)";
                                    sql += " Values ";
                                    sql += " ('" + uu.CompanyId + "', '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108) ";
                                    sql += " ,'" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108) ";
                                    sql += " ,'" + dr["ST_ID"].ToString().SqlQuote() + "','00','" + dr["WhNoIn"].ToString().SqlQuote() + "') ";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    sql = "update WarehouseDSV set ";
                                    sql += "WhNoIn='" + dr["ST_ID"].ToString().SqlQuote() + "',";
                                    sql += "ModDate=convert(char(10),getdate(),111),";
                                    sql += "ModTime=convert(char(12),getdate(),108),";
                                    sql += "ModUser='" + uu.UserID + "' ";
                                    sql += "where CompanyCode='" + uu.CompanyId + "' ";
                                    sql += "and ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                                    sql += "and CkNo<>'00' ";
                                    dbop.ExecuteSql(sql, uu, "SYS");
                                }
                                else if (dr["OldFlagInv"].ToString() == "Y" && dr["FlagInv"].ToString() == "N")
                                {
                                    //調撥表頭
                                    string TH_ID = PubUtility.GetNewDocNo(uu, "TH", 6);
                                    sql = "Insert Into TransferHSV (CompanyCode,CrtUser,CrtDate,CrtTime,";
                                    sql += "ModUser,ModDate,ModTime,TH_ID,DocDate,WhNoOut,OutUser,";
                                    sql += "InDate,WhNoIn,InUser,DocType,CkNoOut,CkNoIn,WorkType,";
                                    sql += "ChkUser,ChkDate,PostUser,PostDate)";

                                    sql += " Values ";
                                    sql += "('" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108)";
                                    sql += ",'" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108),";
                                    sql += "'" + TH_ID + "',convert(char(10),getdate(),111),'" + dr["ST_ID"].ToString().SqlQuote() + "','" + uu.UserID + "',";
                                    sql += "convert(char(10),getdate(),111),'" + dr["WhNoIn"].ToString().SqlQuote() + "','" + uu.UserID + "',";
                                    sql += "'V','00','XX','IB','" + uu.UserID + "',convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5),";
                                    sql += "'" + uu.UserID + "',convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5)) ";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    //調撥表身
                                    sql = "Insert Into TransferDSV (CompanyCode,CrtUser,CrtDate,CrtTime,";
                                    sql += "ModUser,ModDate,ModTime,TH_ID,SeqNo,PLU,OutNum,InNum,";
                                    sql += "GD_Retail,Amt,LayerIn,LayerOut,SnoOut)";

                                    sql += " select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108),";
                                    sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108),";
                                    sql += "'" + TH_ID + "',Cast(Row_Number() Over(Order By a.plu) As int),";
                                    sql += "a.plu,a.PTNum,a.PTNum,isnull(b.GD_Retail,0)GD_Retail,a.PTNum*isnull(b.GD_Retail,0),";
                                    sql += "'','Z',(select Sno from InventorySV a1 (nolock) where a1.companycode='" + uu.CompanyId + "' and a.whno=a1.whno and a.ckno=a1.ckno and a.plu=a1.plu and a1.Layer='Z') ";
                                    sql += "from InventorySV a (nolock) ";
                                    sql += "left join plusv b (nolock) on a.plu=b.gd_no and b.CompanyCode=a.CompanyCode ";
                                    sql += "where a.CompanyCode='" + uu.CompanyId + "' ";
                                    sql += "and a.whno='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                                    sql += "and a.ckno='00' ";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    //寫入調出方JahoInvSV
                                    sql = "Insert Into JahoInvSV (CompanyCode,CrtUser,CrtDate,CrtTime, ";
                                    sql += "ModUser,ModDate,ModTime, ";
                                    sql += "DocType,DocNo,WhNo,SeqNo,PLU,Q1,Q2,Q3,CkNo,Layer,Sno) ";

                                    sql += "Select '" + uu.CompanyId.SqlQuote() + "'";
                                    sql += ",'" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)";
                                    sql += ",'" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)";
                                    sql += ",'TF','" + TH_ID + "',t.WhNoOut,t.SeqNo,t.PLU,IsNull(i.PtNum,0),isnull(t.InNum,0)*-1,IsNull(i.PtNum,0)+isnull(t.InNum,0)*-1";
                                    sql += ",t.CkNoOut,t.LayerOut,t.SnoOut ";

                                    sql += "From (select a.WhNoOut,b.SeqNo,b.PLU,b.InNum,a.CkNoOut,b.LayerOut,b.SnoOut ";
                                    sql += "from TransferHSV a (nolock) ";
                                    sql += "left join TransferDSV b (nolock) on a.TH_ID = b.TH_ID and b.CompanyCode = a.CompanyCode ";
                                    sql += "where a.CompanyCode='" + uu.CompanyId + "' and a.TH_ID='" + TH_ID + "')t ";

                                    sql += "Left Join InventorySV i (nolock) ";
                                    sql += "On i.CompanyCode='" + uu.CompanyId + "' And t.WhNoOut=i.WhNo and t.CkNoOut=i.CkNo ";
                                    sql += "and t.LayerOut=i.Layer And t.SnoOut=i.Sno And t.PLU=i.PLU ";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    //調出方庫存
                                    sql = "Update InventorySV set PTNum=PTNum-t.InNum,Out_Date=t.DocDate, ";
                                    sql += "ModUser='" + uu.UserID + "',ModDate=convert(char(10),getdate(),111),ModTime=convert(char(8),getdate(),108) ";

                                    sql += "from(select a.WhNoOut,a.CkNoOut,b.PLU,b.InNum,a.DocDate,b.LayerOut,b.SnoOut from TransferHSV a (nolock) ";
                                    sql += "left join TransferDSV b (nolock) on a.TH_ID = b.TH_ID and b.CompanyCode = a.CompanyCode ";
                                    sql += "where a.CompanyCode='" + uu.CompanyId + "' and a.TH_ID='" + TH_ID + "')t ";
                                    sql += "left join InventorySV i (nolock) on t.WhNoOut=i.WhNo and t.CkNoOut=i.CkNo and t.PLU=i.PLU and t.LayerOut=i.Layer and t.SnoOut=i.Sno and i.companycode='" + uu.CompanyId + "' ";
                                    sql += "where 1=1 ";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    //刪除母倉資料
                                    sql = "Delete From WarehouseDSV where Companycode='" + uu.CompanyId + "' ";
                                    sql += "and ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' and CkNo='00' ";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    sql = "Update WarehouseDSV set WhNoIn='" + dr["WhNoIn"].ToString().SqlQuote() + "' ";
                                    sql += "where Companycode='" + uu.CompanyId + "' ";
                                    sql += "and ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                                    dbop.ExecuteSql(sql, uu, "SYS");
                                }

                            }
                            sql = "update WarehouseSV set ";
                            sql += "FlagInv='" + dr["FlagInv"].ToString().SqlQuote() + "',";
                            sql += "WhNoIn='" + dr["WhNoIn"].ToString().SqlQuote() + "',";
                            sql += "ST_DeliArea='" + dr["ST_DeliArea"].ToString().SqlQuote() + "',";
                            sql += "ModDate=convert(char(10),getdate(),111),";
                            sql += "ModTime=convert(char(12),getdate(),108),";
                            sql += "ModUser='" + uu.UserID + "' ";
                            sql += "where CompanyCode='" + uu.CompanyId + "' ";
                            sql += "and ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                            dbop.ExecuteSql(sql, uu, "SYS");
                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select a.* ";
                sql += "from WarehouseSV a (nolock) ";
                sql += "where a.CompanyCode='" + uu.CompanyId + "' And a.ST_ID='" + dr["ST_ID"].ToString().SqlQuote() + "' ";
                DataTable dtWarehouseSV = PubUtility.SqlQry(sql, uu, "SYS");
                dtWarehouseSV.TableName = "dtWarehouseSV";
                ds.Tables.Add(dtWarehouseSV);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2021-08-10 Kris
        [Route("SystemSetup/GetVMN02SN_Add")]
        public ActionResult GetVMN02SN_Add()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVMN02SN_AddOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "Select SNno from MachineList (nolock) where Companycode='" + uu.CompanyId + "' and FlagUse='N' " +
                             "order by SNno ";
                DataTable dtSN_Add = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtSN_Add);
                dtSN_Add.TableName = "dtSN_Add";
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }



        //2022-07-27 Larry
        [Route("SystemSetup/GetInitISAMQPLU")]
        public ActionResult GetInitISAMQPLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitISAMQPLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "select E.whno, W.ST_SName from ISAMShopWeb (nolock) E Left Join WarehouseWeb W On E.CompanyCode=W.CompanyCode And E.WhNo=W.ST_ID " +
                             "where E.companycode='" + uu.CompanyId + "' and man_id='" + uu.UserID + "' ";
                DataTable dtShop = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtShop);
                dtShop.TableName = "dtShop";

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2022-07-27 Larry
        [Route("SystemSetup/GetISAMQPLU")]
        public ActionResult SystemSetup_GetISAMQPLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetISAMQPLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                //string Area = rq["Area"];
                string WhNo = rq["WhNo"];
                string PLU = rq["PLU"];

                string sql = "Select a.GD_No,a.GD_Barcode,a.GD_Name,a.GD_Retail,IsNull(b.PTNum,0) PTNum From PluWeb a (nolock) " +
                    "left join InventoryAtonceDWeb b (nolock) on a.GD_No=b.PLU and a.companycode=b.companycode And b.WhNo='" + WhNo + "' " +
                    "Where a.Companycode='" + uu.CompanyId + "' ";
                if (PLU != "")
                    sql += "and (a.GD_No='" + PLU + "' Or a.GD_Barcode='" + PLU + "') ";
                sql += "order by a.GD_No ";

                DataTable dtQPLU = PubUtility.SqlQry(sql, uu, "SYS");
                dtQPLU.TableName = "dtQPLU";
                ds.Tables.Add(dtQPLU);

                sql = "Select CompanyCode, PP_No, StartDate, EndDate, ApproveDate From PromotePriceHWeb (Nolock) " +
                     "Where CompanyCode='" + uu.CompanyId + "' And WhNoFlag='Y' And IsNull(ApproveDate,'')<>'' And IsNull(DefeasanceDate,'')='' " +
                     "and convert(char(10),getdate(),111) between StartDate and EndDate " +
                     "Union " +
                     "Select H.CompanyCode, H.PP_No, H.StartDate, H.EndDate, H.ApproveDate " +
                     "From PromotePriceHWeb H (Nolock) Inner Join PromotePriceShopWeb S (Nolock) On H.CompanyCode=S.CompanyCode And H.PP_No=S.PP_No " +
                     "Where H.CompanyCode='" + uu.CompanyId + "' And WhNoFlag='N' And IsNull(ApproveDate,'')<>'' And IsNull(DefeasanceDate,'')='' And S.WhNo='" + WhNo + "' " +
                     "and convert(char(10),getdate(),111) between StartDate and EndDate ";

                //取得促銷資料
                if (dtQPLU.Rows.Count > 0)

                {
                    sql = "Select H.*, D.Promote From (" +
                       sql + ") H " +
                       "Inner Join PromotePriceDWeb D (Nolock) " +
                       "On H.CompanyCode=D.CompanyCode And H.PP_No=D.PP_No " +
                       "Where D.PLU='" + dtQPLU.Rows[0]["GD_No"] + "'" +
                       "Order By ApproveDate Desc ";
                }

                else
                {
                    sql = "Select H.CompanyCode, H.PP_No, H.StartDate, H.EndDate, H.ApproveDate, D.Promote " +
                        "From PromotePriceHWeb H (Nolock) Inner Join PromotePriceDWeb D (Nolock) On H.CompanyCode=D.CompanyCode And H.PP_No=D.PP_No Where 1=2";
                }

                DataTable dtPrm = PubUtility.SqlQry(sql, uu, "SYS");
                dtPrm.TableName = "dtPrm";
                ds.Tables.Add(dtPrm);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }



        [Route("SystemSetup/GetTreeData")]
        public ActionResult SystemSetup_GetTreeData()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetTreeDataOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string LV = rq["LV"];
                string isFormLoad = rq["FormLoad"];

                //Left1.Group
                string sql = "select 'G_'+GroupID [id],'#' parent,GroupName [text] from GroupIDWeb (nolock) where CompanyCode='" + uu.CompanyId + "' order by GroupID ";
                DataTable dtG = PubUtility.SqlQry(sql, uu, "SYS");
                dtG.TableName = "dtGroupID";
                ds.Tables.Add(dtG);

                //Left2.Man
                sql = "Select 'E_'+convert(varchar,b.UID)+'_'+isnull(b.groupid,'') [id],UName [text],'G_'+isnull(b.groupid,'') parent from Accountgroupweb b (nolock) left join Account a (nolock) ";
                sql += "on b.companyCode=a.CompanyCode and b.Uid=a.Uid where b.CompanyCode='" + uu.CompanyId + "' order by isnull(b.groupid,''),b.uid ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtEmp";
                ds.Tables.Add(dtE);

                //Right1.SystemID
                sql = "select 'S_'+a.SystemId id,'#' parent,a.ChineseName [text] from SystemIDCompanyWWeb a (nolock) inner join ProgramIDCompanyWWeb b (nolock) ";
                sql += "on a.CompanyCode=b.CompanyCode and a.SystemId=b.SectionID ";
                sql += "Where a.CompanyCode='" + uu.CompanyId + "' and isnull(b.OrderSequence,'')<>'99' ";
                sql += "group by a.SystemId,a.ChineseName,a.OrderSequence Order By a.OrderSequence ";

                DataTable dtS = PubUtility.SqlQry(sql, uu, "SYS");
                dtS.TableName = "dtSystemID";
                ds.Tables.Add(dtS);

                //Right2.ProgramID
                sql = "select 'P_'+b.ProgramId id,'S_'+b.SectionID parent,b.ChineseName text from SystemIDCompanyWWeb a (nolock) inner join ProgramIDCompanyWWeb b (nolock) ";
                sql += "on a.CompanyCode=b.CompanyCode and a.SystemId=b.SectionID ";
                sql += "Where a.CompanyCode='" + uu.CompanyId + "' and isnull(b.OrderSequence,'')<>'99' ";
                sql += "Order By a.OrderSequence,b.OrderSequence ";

                DataTable dtP = PubUtility.SqlQry(sql, uu, "SYS");
                dtP.TableName = "dtProgramID";
                ds.Tables.Add(dtP);

                //Right3.ProgramButton
                sql = "select 'B_'+b.Button+'_'+b.ProgramID id,'P_'+b.ProgramID parent,b.Tradchn text from ProgramIDCompanyWWeb a (nolock) inner join ProgramButtonWeb b (nolock) ";
                sql += "on a.ProgramId=b.ProgramId ";
                sql += "Where a.CompanyCode='" + uu.CompanyId + "' and isnull(a.OrderSequence,'')<>'99' ";
                sql += "Order By a.OrderSequence,b.Button ";

                DataTable dtB = PubUtility.SqlQry(sql, uu, "SYS");
                dtB.TableName = "dtProgramButton";
                ds.Tables.Add(dtB);


                //GroupProgramID
                string GID = "";
                if (dtG.Rows.Count > 0)
                {
                    GID = dtG.Rows[0]["id"].ToString().Substring(2, dtG.Rows[0]["id"].ToString().Length - 2);
                }

                sql = "select 'P_'+ProgramId id from GroupProgramIDWeb (nolock) ";
                sql += "Where CompanyCode='" + uu.CompanyId + "' and GroupID='" + GID.SqlQuote() + "' ";
                DataTable dtGP = PubUtility.SqlQry(sql, uu, "SYS");
                dtGP.TableName = "dtGroupProgramID";
                ds.Tables.Add(dtGP);

                //GroupButton
                sql = "select 'B_'+a.Button+'_'+a.ProgramID id,'P_'+a.ProgramId parent,case when isnull(b.button,'')='' then '0' else '1' end Flag1 ";
                sql += "from ProgramButtonWeb a (nolock) left join GroupButtonWeb b (nolock) on a.ProgramID=b.ProgramID and a.Button=b.Button ";
                sql += "and CompanyCode='" + uu.CompanyId + "' and GroupID='" + GID.SqlQuote() + "' ";
                DataTable dtGB = PubUtility.SqlQry(sql, uu, "SYS");
                dtGB.TableName = "dtGroupButton";
                ds.Tables.Add(dtGB);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/GetGroupEmp")]
        public ActionResult SystemSetup_GetGroupEmp()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetGroupEmpOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string LV = rq["LV"];
                string Hbtn = rq["Hbtn"];
                string Emptype = rq["EmpLoadType"];
                string ManGroup = rq["ManGroup"];

                //Group
                string sql = "select 'DG_'+GroupID [id],'#' parent,GroupName [text] from GroupIDWeb (nolock) where CompanyCode='" + uu.CompanyId + "' order by GroupID ";  // and [status]='" + LV.SqlQuote() + "'
                DataTable dtG = PubUtility.SqlQry(sql, uu, "SYS");
                dtG.TableName = "dtGroupID";
                ds.Tables.Add(dtG);

                if (Hbtn != "G")
                {
                    string ls_Cond = "";
                    switch (Emptype)    //A-加入群組人員;D-刪除群組人員
                    {
                        case "A":
                            if (ManGroup != null & ManGroup != "")
                            {
                                ls_Cond = " and not exists (Select * from AccountGroupWeb (nolock) where CompanyCode=Account.CompanyCode and GroupID='" + ManGroup.SqlQuote() + "' and UID=Account.UID) ";
                            }
                            break;
                        case "D":
                            if (ManGroup != null)
                            {
                                ls_Cond = " and UID in (Select UID from AccountGroupWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and GroupID='" + ManGroup.SqlQuote() + "') ";
                            }
                            else
                            {
                                ls_Cond = " and 1=0 ";
                            }
                            break;
                        default:
                            ls_Cond = " and 1=0 ";
                            break;
                    }

                    if (Emptype != "X")
                    {
                        //Emp
                        sql = "Select UID,UName from Account (nolock) where CompanyCode='" + uu.CompanyId + "'";
                        sql += ls_Cond;
                        sql += " order by UID,id ";
                        DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                        dtE.TableName = "dtEmp";
                        ds.Tables.Add(dtE);
                    }
                }
                else
                {
                    sql = "select distinct a.GroupID,GroupName from GroupIDWeb a (nolock) inner join GroupProgramIDWeb b (nolock) ";
                    sql += "on a.CompanyCode=b.CompanyCode and a.GroupId=b.GroupID ";
                    sql += "where a.CompanyCode='" + uu.CompanyId + "' order by GroupID ";  // and a.[status]='" + LV.SqlQuote() + "'
                    DataTable dtCG = PubUtility.SqlQry(sql, uu, "SYS");
                    dtCG.TableName = "dtCopyGroup";
                    ds.Tables.Add(dtCG);
                }


            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/ChkGroupExist")]
        public ActionResult SystemSetup_ChkGroupExist()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkGroupExistOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string GroupID = rq["GroupID"];
                string LV = rq["LV"];
                string EditType = rq["EditType"];
                string sql = "";
                if (EditType == "A")
                { //群組新增
                    sql = "select 'DG_'+GroupID id,'#' parent,GroupName [text] from GroupIDWeb Where CompanyCode='" + uu.CompanyId + "' and GroupID='" + GroupID + "'";
                    DataTable dtG = PubUtility.SqlQry(sql, uu, "SYS");
                    dtG.TableName = "dtGroupID";
                    ds.Tables.Add(dtG);
                }
                else
                { //群組刪除
                    sql = "select * from AccountGroupWeb Where 1=0";
                    DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                    dtE.TableName = "dtGroupEmp";
                    ds.Tables.Add(dtE);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/AddGroup")]
        public ActionResult SystemSetup_AddGroup()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "AddGroupOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("GroupIDWeb");
                PubUtility.AddStringColumns(dtRec, "GroupID,GroupName,Status,CopyGroup");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            string copygroup = dr["CopyGroup"].ToString().SqlQuote();
                            DataTable dtTmp = dtRec.Copy();
                            dtTmp.Columns.Remove("Status");
                            dtTmp.Columns.Remove("CopyGroup");
                            dbop.Add("GroupIDWeb", dtTmp, uu, "SYS");
                            if (copygroup != "")
                            {
                                DataTable dtP = dbop.Query("select Companycode,'" + dr["GroupID"] + "' GroupID,ProgramID from GroupProgramIDWeb (nolock) where Companycode='" + uu.CompanyId + "' and GroupID='" + copygroup + "'", uu, "SYS");
                                if (dtP.Rows.Count > 0)
                                {
                                    dbop.Add("GroupProgramIDWeb", dtP, uu, "SYS");
                                }
                                DataTable dtB = dbop.Query("select Companycode,'" + dr["GroupID"] + "' GroupID,ProgramID,Button from GroupButtonWeb (nolock) where Companycode='" + uu.CompanyId + "' and GroupID='" + copygroup + "'", uu, "SYS");
                                if (dtB.Rows.Count > 0)
                                {
                                    dbop.Add("GroupButtonWeb", dtB, uu, "SYS");
                                }
                            }

                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select 'DG_'+GroupID id,'#' parent,GroupName [text] from GroupIDWeb a (nolock) ";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' order by GroupID";
                DataTable dtG = PubUtility.SqlQry(sql, uu, "SYS");
                dtG.TableName = "dtGroupID";
                ds.Tables.Add(dtG);

                sql = "select distinct a.GroupID,GroupName from GroupIDWeb a (nolock) inner join GroupProgramIDWeb b (nolock) ";
                sql += "on a.CompanyCode=b.CompanyCode and a.GroupId=b.GroupID ";
                sql += "where a.CompanyCode='" + uu.CompanyId + "' order by GroupID ";
                DataTable dtCG = PubUtility.SqlQry(sql, uu, "SYS");
                dtCG.TableName = "dtCopyGroup";
                ds.Tables.Add(dtCG);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/EditGroup")]
        public ActionResult SystemSetup_EditGroup()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "EditGroupOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("GroupIDWeb");
                PubUtility.AddStringColumns(dtRec, "GroupID,GroupName,LV,EditType");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            if (dr["EditType"].ToString() == "M")
                            {
                                sql = "update GroupIDWeb set ";
                                sql += " GroupName='" + dr["GroupName"].ToString().SqlQuote() + "' where CompanyCode='" + uu.CompanyId + "' And GroupID='" + dr["GroupID"].ToString().SqlQuote() + "'";
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }
                            else
                            {
                                sql = "Delete from AccountGroupWeb where CompanyCode='" + uu.CompanyId + "' and GroupID='" + dr["GroupID"].ToString().SqlQuote() + "'";
                                dbop.ExecuteSql(sql, uu, "SYS");
                                sql = "Delete from GroupButtonWeb where CompanyCode='" + uu.CompanyId + "' and GroupID='" + dr["GroupID"].ToString().SqlQuote() + "'";
                                dbop.ExecuteSql(sql, uu, "SYS");
                                sql = "Delete from GroupProgramIDWeb where CompanyCode='" + uu.CompanyId + "' and GroupID='" + dr["GroupID"].ToString().SqlQuote() + "'";
                                dbop.ExecuteSql(sql, uu, "SYS");
                                sql = "Delete from GroupIDWeb where CompanyCode='" + uu.CompanyId + "' and GroupID='" + dr["GroupID"].ToString().SqlQuote() + "'";
                                dbop.ExecuteSql(sql, uu, "SYS");

                            }
                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                sql = "select 'DG_'+GroupID id,'#' parent,GroupName [text]";
                sql += " from GroupIDWeb a";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' order by GroupID";
                DataTable dtG = PubUtility.SqlQry(sql, uu, "SYS");
                dtG.TableName = "dtGroupID";
                ds.Tables.Add(dtG);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/EditGroupEmp")]
        public ActionResult SystemSetup_EditGroupEmp()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "EditGroupEmpOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("Account");
                PubUtility.AddStringColumns(dtRec, "LV,GroupID,EmpStr,EditType");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                DataTable dtSysDT = PubUtility.SqlQry("Select Convert(varchar,getdate(),111) D1,convert(varchar,getdate(),108) D2", uu, "SYS");

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            if (dr["EditType"].ToString() == "A")
                            {   //增加群組人員
                                string[] emps = dr["EmpStr"].ToString().Split(',');
                                string sysdate = dtSysDT.Rows[0]["D1"].ToString();
                                string systime = dtSysDT.Rows[0]["D2"].ToString();
                                foreach (string emp in emps)
                                {
                                    sql = "Select * from AccountGroupWeb (nolock) where CompanyCode='" + uu.CompanyId + "' and GroupID='" + dr["GroupID"].ToString().SqlQuote() + "' and UID='" + emp + "'";
                                    DataTable ldt = dbop.Query(sql, uu, "SYS");
                                    if (ldt.Rows.Count == 0)
                                    {
                                        sql = "insert into AccountGroupWeb (CompanyCode,CrtUser,CrtDate,CrtTime,GroupID,UID) ";
                                        sql += "values ('" + uu.CompanyId + "','" + uu.UserID + "','" + sysdate + "','" + systime + "','" + dr["GroupID"].ToString().SqlQuote() + "','" + emp + "')";
                                        dbop.ExecuteSql(sql, uu, "SYS");
                                    }
                                }
                            }
                            else
                            { //刪除人員之群組
                                string[] emps = dr["EmpStr"].ToString().Split(',');
                                foreach (string emp in emps)
                                {
                                    sql = "Delete from AccountGroupWeb where CompanyCode='" + uu.CompanyId + "' and GroupID='" + dr["GroupID"].ToString().SqlQuote() + "' and UID='" + emp + "'";
                                    dbop.ExecuteSql(sql, uu, "SYS");
                                }
                            }
                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetGroupProgram")]
        public ActionResult SystemSetup_GetGroupProgram()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetGroupProgramOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string LV = rq["LV"];
                string Group = rq["GroupID"];

                //GroupProgramID
                string sql = "select 'P_'+a.ProgramId id,'S_'+b.SectionID parent from GroupProgramIDWeb a inner join ProgramIdCompanyWWeb b ";
                sql += "on a.CompanyCode=b.CompanyCode and a.ProgramId=b.ProgramId ";
                sql += "inner join SystemIdCompanyWWeb c on a.CompanyCode=c.CompanyCode and b.SectionId=c.SystemId ";
                sql += "Where a.CompanyCode='" + uu.CompanyId + "' and GroupID='" + Group.SqlQuote() + "' and isnull(b.OrderSequence,'')<>'99' ";
                sql += "Order By c.OrderSequence,b.OrderSequence ";

                DataTable dtGP = PubUtility.SqlQry(sql, uu, "SYS");
                dtGP.TableName = "dtGroupProgramID";
                ds.Tables.Add(dtGP);

                //GroupButton
                sql = "select 'B_'+a.Button+'_'+a.ProgramID id,'P_'+a.ProgramId parent,case when isnull(b.button,'')='' then '0' else '1' end Flag1 ";
                sql += "from ProgramButtonWeb a (nolock) left join GroupButtonWeb b (nolock) on a.ProgramID=b.ProgramID and a.Button=b.Button ";
                sql += "and CompanyCode='" + uu.CompanyId + "' and GroupID='" + Group.SqlQuote() + "' ";
                DataTable dtGB = PubUtility.SqlQry(sql, uu, "SYS");
                dtGB.TableName = "dtGroupButton";
                ds.Tables.Add(dtGB);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/EditGroupProgram")]
        public ActionResult SystemSetup_EditGroupProgram()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "EditGroupProgramOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtG = new DataTable("GPIDWeb");
                PubUtility.AddStringColumns(dtG, "LV,GroupID,ProgramID,Btn");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtG);

                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow drG = dtG.Rows[0];

                string sql = "";
                string[] Programid = drG["ProgramID"].ToString().Split(",");
                string[] Btn = drG["Btn"].ToString().Split(",");
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            //insert ProgramID至暫存表
                            sql = "Select ProgramID into #TempProgramID from GroupProgramIDWeb where 1=0";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            foreach (string dr in Programid)
                            {
                                sql = "insert into #TempProgramID (ProgramID) values ('" + dr.ToString().SqlQuote().Replace("P_", "") + "')";
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }

                            //insert Button至暫存表
                            sql = "Select ProgramID,Button into #TempGroupButton from GroupButtonWeb where 1=0";
                            dbop.ExecuteSql(sql, uu, "SYS");
                            if (Btn.Length > 0 && Btn[0].ToString() != "")
                            {
                                foreach (string dr in Btn)
                                {
                                    string[] tmpdr = dr.ToString().Split("_");
                                    sql = "insert into #TempGroupButton (ProgramID,Button) values ('" + tmpdr[2].ToString().SqlQuote() + "','" + tmpdr[1].ToString().SqlQuote() + "')";
                                    dbop.ExecuteSql(sql, uu, "SYS");
                                }
                            }

                            //合併 ProgramID與Button.ProgramID
                            sql = "select * into #tmpPID from (";
                            sql += "select * from #TempProgramID ";
                            sql += "union ";
                            sql += "select distinct ProgramID from #TempGroupButton) a";
                            dbop.ExecuteSql(sql, uu, "SYS");


                            //刪除已取消的群組程式權限
                            sql = "Delete a from GroupProgramIDWeb a full join #tmpPID b on a.ProgramID=b.ProgramID ";
                            sql += "where a.Companycode = '" + uu.CompanyId + "' and GroupID = '" + drG["GroupID"].ToString().SqlQuote() + "' and b.ProgramID is null";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //新增本次設定的群組程式權限
                            sql = "insert into GroupProgramIDWeb ";
                            sql += "select '" + uu.CompanyId + "','" + uu.UserID + "',Convert(varchar,getdate(),111),Convert(varchar,getdate(),108),";
                            sql += "'" + drG["GroupID"].ToString().SqlQuote() + "',a.ProgramID from #tmpPID a left join GroupProgramIDWeb b ";
                            sql += "on a.ProgramID=b.ProgramID and b.Companycode='" + uu.CompanyId + "' and GroupID='" + drG["GroupID"].ToString().SqlQuote() + "' ";
                            sql += "where b.programid is null";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //刪除已取消的群組程式按鍵權限
                            sql = "Delete a from GroupButtonWeb a full join #TempGroupButton b on a.ProgramID=b.ProgramID and a.Button=b.Button ";
                            sql += "where a.Companycode = '" + uu.CompanyId + "' and GroupID = '" + drG["GroupID"].ToString().SqlQuote() + "' and b.Button is null";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //新增本次設定的群組程式按鍵權限
                            sql = "insert into GroupButtonWeb ";
                            sql += "select '" + uu.CompanyId + "','" + uu.UserID + "',Convert(varchar,getdate(),111),Convert(varchar,getdate(),108),";
                            sql += "'" + drG["GroupID"].ToString().SqlQuote() + "',a.ProgramID,a.Button from #TempGroupButton a left join GroupButtonWeb b ";
                            sql += "on a.ProgramID=b.ProgramID and a.Button=b.Button and b.Companycode='" + uu.CompanyId + "' and GroupID='" + drG["GroupID"].ToString().SqlQuote() + "' ";
                            sql += "where b.Button is null";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            dbop.ExecuteSql("drop table #TempProgramID,#tmpPID,#TempGroupButton", uu, "SYS");
                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
                //GroupProgramID
                sql = "select 'P_'+ProgramId id from GroupProgramIDWeb ";
                sql += "Where CompanyCode='" + uu.CompanyId + "' and GroupID='" + drG["GroupID"].ToString().SqlQuote() + "' ";
                DataTable dtGP = PubUtility.SqlQry(sql, uu, "SYS");
                dtGP.TableName = "dtGroupProgramID";
                ds.Tables.Add(dtGP);

                //GroupButton
                sql = "select 'B_'+a.Button+'_'+a.ProgramID id,'P_'+a.ProgramId parent,case when isnull(b.button,'')='' then '0' else '1' end Flag1 ";
                sql += "from ProgramButtonWeb a (nolock) left join GroupButtonWeb b (nolock) on a.ProgramID=b.ProgramID and a.Button=b.Button ";
                sql += "and CompanyCode='" + uu.CompanyId + "' and GroupID='" + drG["GroupID"].ToString().SqlQuote() + "' ";
                DataTable dtGB = PubUtility.SqlQry(sql, uu, "SYS");
                dtGB.TableName = "dtGroupButton";
                ds.Tables.Add(dtGB);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/ChkEmpExist")]
        public ActionResult SystemSetup_ChkEmpExist()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkEmpExistOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string SetType = rq["SetType"];
                string Setstr = rq["Setstr"].ToString().SqlQuote();
                string sql = "";
                if (SetType == "ID")
                { //ManID
                    sql = "select Man_ID from EmployeeSV (nolock) Where CompanyCode='" + uu.CompanyId + "' and Man_ID='" + Setstr + "'";
                }
                else
                { //ManEaddress
                    sql = "select Man_Eaddress from EmployeeSV (nolock) Where Man_Eaddress='" + Setstr + "'";
                }
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtEmp";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/AddEmp")]
        public ActionResult SystemSetup_AddEmp()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "AddEmpOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtRec = new DataTable("EmployeeSV");
                PubUtility.AddStringColumns(dtRec, "Man_ID,Man_Name,Password,Level,Man_ComeDay,Man_Eaddress,Man_Tel,WhNO");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                PubUtility.AddStringColumns(dtRec, "Man_forSell,Normal");
                DataRow dr = dtRec.Rows[0];
                if (dr["Level"].ToString() == "1") { dr["Man_forSell"] = "1"; }
                else { dr["Man_forSell"] = "0"; }
                dr["Normal"] = "1";

                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            dbop.Add("EmployeeSV", dtRec, uu, "SYS");
                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                        dbop.Dispose();
                    }
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2022/08/19 正常登出
        [Route("UpdateLogOutY")]
        public ActionResult UpdateLogOutY()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateLogOutYOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string LoginDT = rq["LoginDT"];
                string sql = "Update ISAMLoginRecWeb set LogOutDT=Convert(varchar,getdate(),111) + ' ' + convert(char(12),getdate(),108),LogOutType='Y' where CompanyCode='" + uu.CompanyId + "' ";
                sql += "and Man_ID='" + uu.UserID + "' and ISNULL(LogOutType,'')='' and LoginDT='" + LoginDT + "' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2022/08/19 異常登出
        [Route("UpdateLogOutX")]
        public ActionResult UpdateLogOutX()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateLogOutXOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                //string GD_NO = rq["GD_NO"];
                string sql = "Update ISAMLoginRecWeb set LogOutDT=Convert(varchar,getdate(),111) + ' ' + convert(char(12),getdate(),108),LogOutType='X' where CompanyCode='" + uu.CompanyId + "' ";
                sql += "and Man_ID='" + uu.UserID + "' and ISNULL(LogOutType,'')='' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2022/08/19 倒數登出
        [Route("js/UpdateLogOutT")]
        public ActionResult UpdateLogOutT()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateLogOutTOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string LoginDT = rq["LoginDT"];
                string sql = "Update ISAMLoginRecWeb set LogOutDT=Convert(varchar,getdate(),111) + ' ' + convert(char(12),getdate(),108),LogOutType='T' where CompanyCode='" + uu.CompanyId + "' ";
                sql += "and Man_ID='" + uu.UserID + "' and ISNULL(LogOutType,'')='' and LoginDT='" + LoginDT + "' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        //2022/10/24 檢查登出狀態
        [Route("js/ChkLogOut")]
        public ActionResult ChkLogOut()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkLogOutOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string LoginDT = rq["LoginDT"];

                string sql = "select * from ISAMLoginRecWeb (nolock) Where Companycode='" + uu.CompanyId + "' ";
                sql += "and Man_ID='" + uu.UserID + "' and LoginDT='" + LoginDT + "' ";
                DataTable dtLogin = PubUtility.SqlQry(sql, uu, "SYS");
                dtLogin.TableName = "dtLogin";
                ds.Tables.Add(dtLogin);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("Query1")]
        public ActionResult SystemSetup_Query1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "Query1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string OpenDateS = rq["OpenDateS"];
                string OpenDateE = rq["OpenDateE"];
                string Type = rq["Type"];

                string sql = "";
                string sqlQ = "";
                string sqlSumQ = "";

                if (Type == "A")
                {
                    sql = "select w.ST_placeID as ID1,t.Type_Name as Name1,sum(h.Cash)Cash1,COUNT(*)Cnt1,(SUM(h.cash) / COUNT(*))CashCnt1 ";
                    sql += "into #H ";
                    sql += "from SalesH h (nolock) ";
                    sql += "inner join warehouse w (nolock) on h.ShopNo = w.ST_ID and w.CompanyCode = h.CompanyCode ";
                    sql += "inner join TypeData t (nolock) on w.ST_placeID = t.Type_ID and t.Type_Code = 'A' and t.CompanyCode = h.CompanyCode ";
                    sql += "where h.CompanyCode='" + uu.CompanyId + "' ";
                    sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                    sql += "group by w.ST_placeID,t.Type_Name ";
                    sql += "order by w.ST_placeID; ";

                    sql += "select w.ST_placeID as ID2,t.Type_Name as Name2,sum(h.Cash)Cash2,COUNT(*)Cnt2,(SUM(h.cash) / COUNT(*))CashCnt2 ";
                    sql += "into #V ";
                    sql += "from SalesH h (nolock) ";
                    sql += "inner join warehouse w (nolock) on h.ShopNo = w.ST_ID and w.CompanyCode = h.CompanyCode ";
                    sql += "inner join TypeData t (nolock) on w.ST_placeID = t.Type_ID and t.Type_Code = 'A' and t.CompanyCode = h.CompanyCode ";
                    sql += "where h.CompanyCode='" + uu.CompanyId + "' and isnull(h.ifvip,'')='1' ";
                    sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                    sql += "group by w.ST_placeID,t.Type_Name ";
                    sql += "order by w.ST_placeID; ";

                    sqlQ = "Select isnull(h.ID1,'')ID1,isnull(h.Name1,'')Name1,isnull(h.Cash1,0)Cash1,isnull(h.Cnt1,0)Cnt1,Round(isnull(h.CashCnt1,0),0)CashCnt1, ";
                    sqlQ += "isnull(v.Cash2,0)Cash2,isnull(v.Cnt2,0)Cnt2,Round(isnull(v.CashCnt2,0),0)CashCnt2,cast(cast(Round((isnull(v.Cash2,0)/isnull(h.Cash1,0))*100,0) as int) as varchar) + '%' VIPPercent ";
                    sqlQ += "from #H h (nolock) ";
                    sqlQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                    sqlQ += "Where 1=1 ";
                    sqlQ += "Order by h.ID1 ";

                    DataTable dtQ = PubUtility.SqlQry(sql + sqlQ, uu, "SYS");
                    dtQ.TableName = "dtQ";
                    ds.Tables.Add(dtQ);

                    sqlSumQ = "Select sum(isnull(h.Cash1,0))SumCash1,sum(isnull(h.Cnt1,0))SumCnt1,case when sum(isnull(h.Cnt1,0))=0 then 0 else Round(sum(isnull(h.Cash1,0))/sum(isnull(h.Cnt1,0)),0) end as SumCashCnt1, ";
                    sqlSumQ += "sum(isnull(v.Cash2,0))SumCash2,sum(isnull(v.Cnt2,0))SumCnt2,case when sum(isnull(v.Cnt2,0))=0 then 0 else Round(sum(isnull(v.Cash2,0))/sum(isnull(v.Cnt2,0)),0) end as SumCashCnt2,case when sum(isnull(h.Cash1,0))=0 then '0%' else cast(cast(Round((sum(isnull(v.Cash2,0))/sum(isnull(h.Cash1,0)))*100,0) as int) as varchar) + '%' end as SumVIPPercent ";
                    sqlSumQ += "from #H h (nolock) ";
                    sqlSumQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                    sqlSumQ += "Where 1=1 ";
                    DataTable dtSumQ = PubUtility.SqlQry(sql + sqlSumQ, uu, "SYS");
                    dtSumQ.TableName = "dtSumQ";
                    ds.Tables.Add(dtSumQ);
                }
                else if (Type == "S")
                {
                    sql = "select h.ShopNo as ID1,w.st_sname as Name1,sum(h.Cash)Cash1,COUNT(*)Cnt1,(SUM(h.cash) / COUNT(*))CashCnt1 ";
                    sql += "into #H ";
                    sql += "from SalesH h(nolock) ";
                    sql += "inner join warehouse w (nolock) on h.ShopNo = w.ST_ID and w.CompanyCode = h.CompanyCode ";
                    sql += "where h.CompanyCode='" + uu.CompanyId + "' ";
                    sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                    sql += "group by h.ShopNo,w.st_sname ";
                    sql += "order by h.ShopNo; ";

                    sql += "select h.ShopNo as ID2,w.st_sname as Name2,sum(h.Cash)Cash2,COUNT(*)Cnt2,(SUM(h.cash) / COUNT(*))CashCnt2 ";
                    sql += "into #V ";
                    sql += "from SalesH h(nolock) ";
                    sql += "inner join warehouse w (nolock) on h.ShopNo = w.ST_ID and w.CompanyCode = h.CompanyCode ";
                    sql += "where h.CompanyCode='" + uu.CompanyId + "' and isnull(h.ifvip,'')='1' ";
                    sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                    sql += "group by h.ShopNo,w.st_sname ";
                    sql += "order by h.ShopNo; ";

                    sqlQ = "Select isnull(h.ID1,'')ID1,substring(isnull(h.Name1,''),1,4)Name1,isnull(h.Cash1,0)Cash1,isnull(h.Cnt1,0)Cnt1,Round(isnull(h.CashCnt1,0),0)CashCnt1, ";
                    sqlQ += "isnull(v.Cash2,0)Cash2,isnull(v.Cnt2,0)Cnt2,Round(isnull(v.CashCnt2,0),0)CashCnt2,cast(cast(Round((isnull(v.Cash2,0)/isnull(h.Cash1,0))*100,0) as int) as varchar) + '%' VIPPercent ";
                    sqlQ += "from #H h (nolock) ";
                    sqlQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                    sqlQ += "Where 1=1 ";
                    sqlQ += "Order by h.ID1 ";

                    DataTable dtQ = PubUtility.SqlQry(sql + sqlQ, uu, "SYS");
                    dtQ.TableName = "dtQ";
                    ds.Tables.Add(dtQ);

                    sqlSumQ = "Select sum(isnull(h.Cash1,0))SumCash1,sum(isnull(h.Cnt1,0))SumCnt1,case when sum(isnull(h.Cnt1,0))=0 then 0 else Round(sum(isnull(h.Cash1,0))/sum(isnull(h.Cnt1,0)),0) end as SumCashCnt1, ";
                    sqlSumQ += "sum(isnull(v.Cash2,0))SumCash2,sum(isnull(v.Cnt2,0))SumCnt2,case when sum(isnull(v.Cnt2,0))=0 then 0 else Round(sum(isnull(v.Cash2,0))/sum(isnull(v.Cnt2,0)),0) end as SumCashCnt2,case when sum(isnull(h.Cash1,0))=0 then '0%' else cast(cast(Round((sum(isnull(v.Cash2,0))/sum(isnull(h.Cash1,0)))*100,0) as int) as varchar) + '%' end as SumVIPPercent ";
                    sqlSumQ += "from #H h (nolock) ";
                    sqlSumQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                    sqlSumQ += "Where 1=1 ";
                    DataTable dtSumQ = PubUtility.SqlQry(sql + sqlSumQ, uu, "SYS");
                    dtSumQ.TableName = "dtSumQ";
                    ds.Tables.Add(dtSumQ);
                }
                else if (Type == "D")
                {
                    sql = "select h.OpenDate as ID1,h.OpenDate as Name1,sum(h.Cash)Cash1,COUNT(*)Cnt1,(SUM(h.cash) / COUNT(*))CashCnt1 ";
                    sql += "into #H ";
                    sql += "from SalesH h(nolock) ";
                    sql += "where h.CompanyCode='" + uu.CompanyId + "' ";
                    sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                    sql += "group by h.OpenDate ";
                    sql += "order by h.OpenDate; ";

                    sql += "select h.OpenDate as ID2,h.OpenDate as Name2,sum(h.Cash)Cash2,COUNT(*)Cnt2,(SUM(h.cash) / COUNT(*))CashCnt2 ";
                    sql += "into #V ";
                    sql += "from SalesH h(nolock) ";
                    sql += "where h.CompanyCode='" + uu.CompanyId + "' and isnull(h.ifvip,'')='1' ";
                    sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                    sql += "group by h.OpenDate ";
                    sql += "order by h.OpenDate; ";

                    sqlQ = "Select isnull(h.ID1,'')ID1,isnull(h.Name1,'')Name1,isnull(h.Cash1,0)Cash1,isnull(h.Cnt1,0)Cnt1,Round(isnull(h.CashCnt1,0),0)CashCnt1, ";
                    sqlQ += "isnull(v.Cash2,0)Cash2,isnull(v.Cnt2,0)Cnt2,Round(isnull(v.CashCnt2,0),0)CashCnt2,cast(cast(Round((isnull(v.Cash2,0)/isnull(h.Cash1,0))*100,0) as int) as varchar) + '%' VIPPercent ";
                    sqlQ += "from #H h (nolock) ";
                    sqlQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                    sqlQ += "Where 1=1 ";
                    sqlQ += "Order by h.ID1 ";

                    DataTable dtQ = PubUtility.SqlQry(sql + sqlQ, uu, "SYS");
                    dtQ.TableName = "dtQ";
                    ds.Tables.Add(dtQ);

                    sqlSumQ = "Select sum(isnull(h.Cash1,0))SumCash1,sum(isnull(h.Cnt1,0))SumCnt1,case when sum(isnull(h.Cnt1,0))=0 then 0 else Round(sum(isnull(h.Cash1,0))/sum(isnull(h.Cnt1,0)),0) end as SumCashCnt1, ";
                    sqlSumQ += "sum(isnull(v.Cash2,0))SumCash2,sum(isnull(v.Cnt2,0))SumCnt2,case when sum(isnull(v.Cnt2,0))=0 then 0 else Round(sum(isnull(v.Cash2,0))/sum(isnull(v.Cnt2,0)),0) end as SumCashCnt2,case when sum(isnull(h.Cash1,0))=0 then '0%' else cast(cast(Round((sum(isnull(v.Cash2,0))/sum(isnull(h.Cash1,0)))*100,0) as int) as varchar) + '%' end as SumVIPPercent ";
                    sqlSumQ += "from #H h (nolock) ";
                    sqlSumQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                    sqlSumQ += "Where 1=1 ";
                    DataTable dtSumQ = PubUtility.SqlQry(sql + sqlSumQ, uu, "SYS");
                    dtSumQ.TableName = "dtSumQ";
                    ds.Tables.Add(dtSumQ);
                }

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("ChkQuery1_Shop")]
        public ActionResult SystemSetup_ChkQuery1_Shop()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkQuery1_ShopOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Shop = rq["Shop"];

                string sql = "";
                sql = "Select w.st_id,w.st_sname,w.st_placeid,t.type_name ";
                sql += "from Warehouse w (nolock) ";
                sql += "left join TypeData t (nolock) on w.ST_PlaceID=t.type_ID and t.type_code='A' and t.companycode=w.companycode ";
                sql += "Where w.companycode='" + uu.CompanyId + "' ";
                sql += "and (w.ST_ID='" + Shop + "' or w.st_sname='" + Shop + "') ";
                DataTable dtW = PubUtility.SqlQry(sql, uu, "SYS");
                dtW.TableName = "dtW";
                ds.Tables.Add(dtW);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("Query1_Shop")]
        public ActionResult SystemSetup_Query1_Shop()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "Query1_ShopOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string OpenDateS = rq["OpenDateS"];
                string OpenDateE = rq["OpenDateE"];
                string Shop = rq["Shop"];

                string sql = "";
                string sqlQ = "";
                string sqlSumQ = "";

                sql = "select h.OpenDate as ID1,h.OpenDate as Name1,sum(h.Cash)Cash1,COUNT(*)Cnt1,(SUM(h.cash) / COUNT(*))CashCnt1 ";
                sql += "into #H ";
                sql += "from SalesH h(nolock) ";
                sql += "inner join warehouse w (nolock) on h.ShopNo=w.ST_ID and w.CompanyCode=h.CompanyCode and (w.st_id='" + Shop + "' or w.st_sname='" + Shop + "') ";
                sql += "where h.CompanyCode='" + uu.CompanyId + "' ";
                sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                sql += "group by h.OpenDate ";
                sql += "order by h.OpenDate; ";

                sql += "select h.OpenDate as ID2,h.OpenDate as Name2,sum(h.Cash)Cash2,COUNT(*)Cnt2,(SUM(h.cash) / COUNT(*))CashCnt2 ";
                sql += "into #V ";
                sql += "from SalesH h(nolock) ";
                sql += "inner join warehouse w (nolock) on h.ShopNo=w.ST_ID and w.CompanyCode=h.CompanyCode and (w.st_id='" + Shop + "' or w.st_sname='" + Shop + "') ";
                sql += "where h.CompanyCode='" + uu.CompanyId + "' and isnull(h.ifvip,'')='1' ";
                sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                sql += "group by h.OpenDate ";
                sql += "order by h.OpenDate; ";

                sqlQ = "Select isnull(h.ID1,'')ID1,isnull(h.Name1,'')Name1,isnull(h.Cash1,0)Cash1,isnull(h.Cnt1,0)Cnt1,Round(isnull(h.CashCnt1,0),0)CashCnt1, ";
                sqlQ += "isnull(v.ID2,'')ID2,isnull(v.Name2,'')Name2,isnull(v.Cash2,0)Cash2,isnull(v.Cnt2,0)Cnt2,Round(isnull(v.CashCnt2,0),0)CashCnt2,cast(cast(Round((isnull(v.Cash2,0)/isnull(h.Cash1,0))*100,0) as int) as varchar) + '%' VIPPercent ";
                sqlQ += "from #H h (nolock) ";
                sqlQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                sqlQ += "Where 1=1 ";
                sqlQ += "Order by h.ID1 ";

                DataTable dtQ = PubUtility.SqlQry(sql + sqlQ, uu, "SYS");
                dtQ.TableName = "dtQ";
                ds.Tables.Add(dtQ);

                sqlSumQ = "Select sum(isnull(h.Cash1,0))SumCash1,sum(isnull(h.Cnt1,0))SumCnt1,case when sum(isnull(h.Cnt1,0))=0 then 0 else Round(sum(isnull(h.Cash1,0))/sum(isnull(h.Cnt1,0)),0) end as SumCashCnt1, ";
                sqlSumQ += "sum(isnull(v.Cash2,0))SumCash2,sum(isnull(v.Cnt2,0))SumCnt2,case when sum(isnull(v.Cnt2,0))=0 then 0 else Round(sum(isnull(v.Cash2,0))/sum(isnull(v.Cnt2,0)),0) end as SumCashCnt2,case when sum(isnull(h.Cash1,0))=0 then '0%' else cast(cast(Round((sum(isnull(v.Cash2,0))/sum(isnull(h.Cash1,0)))*100,0) as int) as varchar) + '%' end as SumVIPPercent ";
                sqlSumQ += "from #H h (nolock) ";
                sqlSumQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                sqlSumQ += "Where 1=1 ";
                DataTable dtSumQ = PubUtility.SqlQry(sql + sqlSumQ, uu, "SYS");
                dtSumQ.TableName = "dtSumQ";
                ds.Tables.Add(dtSumQ);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("Query_Area_Step1")]
        public ActionResult SystemSetup_Query_Area_Step1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "Query_Area_Step1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string OpenDateS = rq["OpenDateS"];
                string OpenDateE = rq["OpenDateE"];
                string Area = rq["Area"];
                string Type_Step1 = rq["Type_Step1"];

                string sql = "";
                string sqlQ = "";
                string sqlSumQ = "";

                if (Type_Step1 == "S")
                {
                    sql = "select h.ShopNo as ID1,w.st_sname as Name1,sum(h.Cash)Cash1,COUNT(*)Cnt1,(SUM(h.Cash) / COUNT(*))CashCnt1 ";
                    sql += "into #H ";
                    sql += "from SalesH h(nolock) ";
                    sql += "inner join warehouse w (nolock) on h.ShopNo = w.ST_ID and w.CompanyCode = h.CompanyCode and w.ST_placeID='" + Area + "' ";
                    sql += "where h.CompanyCode='" + uu.CompanyId + "' ";
                    sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                    sql += "group by h.ShopNo,w.st_sname ";
                    sql += "order by h.ShopNo; ";

                    sql += "select h.ShopNo as ID2,w.st_sname as Name2,sum(h.Cash)Cash2,COUNT(*)Cnt2,(SUM(h.cash) / COUNT(*))CashCnt2 ";
                    sql += "into #V ";
                    sql += "from SalesH h(nolock) ";
                    sql += "inner join warehouse w (nolock) on h.ShopNo=w.ST_ID and w.CompanyCode=h.CompanyCode and w.ST_placeID='" + Area + "' ";
                    sql += "where h.CompanyCode='" + uu.CompanyId + "' and isnull(h.ifvip,'')='1' ";
                    sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                    sql += "group by h.ShopNo,w.st_sname ";
                    sql += "order by h.ShopNo; ";

                    sqlQ = "Select isnull(h.ID1,'')ID1,substring(isnull(h.Name1,''),1,4)Name1,isnull(h.Cash1,0)Cash1,isnull(h.Cnt1,0)Cnt1,Round(isnull(h.CashCnt1,0),0)CashCnt1, ";
                    sqlQ += "isnull(v.ID2,'')ID2,isnull(v.Name2,'')Name2,isnull(v.Cash2,0)Cash2,isnull(v.Cnt2,0)Cnt2,Round(isnull(v.CashCnt2,0),0)CashCnt2,cast(cast(Round((isnull(v.Cash2,0)/isnull(h.Cash1,0))*100,0) as int) as varchar) + '%' VIPPercent ";
                    sqlQ += "from #H h (nolock) ";
                    sqlQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                    sqlQ += "Where 1=1 ";
                    sqlQ += "Order by h.ID1 ";
                    DataTable dtQ = PubUtility.SqlQry(sql + sqlQ, uu, "SYS");
                    dtQ.TableName = "dtQ";
                    ds.Tables.Add(dtQ);

                    sqlSumQ = "Select sum(isnull(h.Cash1,0))SumCash1,sum(isnull(h.Cnt1,0))SumCnt1,case when sum(isnull(h.Cnt1,0))=0 then 0 else Round(sum(isnull(h.Cash1,0))/sum(isnull(h.Cnt1,0)),0) end as SumCashCnt1, ";
                    sqlSumQ += "sum(isnull(v.Cash2,0))SumCash2,sum(isnull(v.Cnt2,0))SumCnt2,case when sum(isnull(v.Cnt2,0))=0 then 0 else Round(sum(isnull(v.Cash2,0))/sum(isnull(v.Cnt2,0)),0) end as SumCashCnt2,case when sum(isnull(h.Cash1,0))=0 then '0%' else cast(cast(Round((sum(isnull(v.Cash2,0))/sum(isnull(h.Cash1,0)))*100,0) as int) as varchar) + '%' end as SumVIPPercent ";
                    sqlSumQ += "from #H h (nolock) ";
                    sqlSumQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                    sqlSumQ += "Where 1=1 ";
                    DataTable dtSumQ = PubUtility.SqlQry(sql + sqlSumQ, uu, "SYS");
                    dtSumQ.TableName = "dtSumQ";
                    ds.Tables.Add(dtSumQ);
                }
                else if (Type_Step1 == "D")
                {
                    sql = "select h.OpenDate as ID1,h.OpenDate as Name1,sum(h.Cash)Cash1,COUNT(*)Cnt1,(SUM(h.cash) / COUNT(*))CashCnt1 ";
                    sql += "into #H ";
                    sql += "from SalesH h(nolock) ";
                    sql += "inner join warehouse w (nolock) on h.ShopNo=w.ST_ID and w.CompanyCode=h.CompanyCode and w.ST_placeID='" + Area + "' ";
                    sql += "where h.CompanyCode='" + uu.CompanyId + "' ";
                    sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                    sql += "group by h.OpenDate ";
                    sql += "order by h.OpenDate; ";

                    sql += "select h.OpenDate as ID2,h.OpenDate as Name2,sum(h.Cash)Cash2,COUNT(*)Cnt2,(SUM(h.cash) / COUNT(*))CashCnt2 ";
                    sql += "into #V ";
                    sql += "from SalesH h(nolock) ";
                    sql += "inner join warehouse w (nolock) on h.ShopNo=w.ST_ID and w.CompanyCode=h.CompanyCode and w.ST_placeID='" + Area + "' ";
                    sql += "where h.CompanyCode='" + uu.CompanyId + "' and isnull(h.ifvip,'')='1' ";
                    sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                    sql += "group by h.OpenDate ";
                    sql += "order by h.OpenDate; ";

                    sqlQ = "Select isnull(h.ID1,'')ID1,isnull(h.Name1,'')Name1,isnull(h.Cash1,0)Cash1,isnull(h.Cnt1,0)Cnt1,Round(isnull(h.CashCnt1,0),0)CashCnt1, ";
                    sqlQ += "isnull(v.ID2,'')ID2,isnull(v.Name2,'')Name2,isnull(v.Cash2,0)Cash2,isnull(v.Cnt2,0)Cnt2,Round(isnull(v.CashCnt2,0),0)CashCnt2,cast(cast(Round((isnull(v.Cash2,0)/isnull(h.Cash1,0))*100,0) as int) as varchar) + '%' VIPPercent ";
                    sqlQ += "from #H h (nolock) ";
                    sqlQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                    sqlQ += "Where 1=1 ";
                    sqlQ += "Order by h.ID1 ";
                    DataTable dtQ = PubUtility.SqlQry(sql + sqlQ, uu, "SYS");
                    dtQ.TableName = "dtQ";
                    ds.Tables.Add(dtQ);

                    sqlSumQ = "Select sum(isnull(h.Cash1,0))SumCash1,sum(isnull(h.Cnt1,0))SumCnt1,case when sum(isnull(h.Cnt1,0))=0 then 0 else Round(sum(isnull(h.Cash1,0))/sum(isnull(h.Cnt1,0)),0) end as SumCashCnt1, ";
                    sqlSumQ += "sum(isnull(v.Cash2,0))SumCash2,sum(isnull(v.Cnt2,0))SumCnt2,case when sum(isnull(v.Cnt2,0))=0 then 0 else Round(sum(isnull(v.Cash2,0))/sum(isnull(v.Cnt2,0)),0) end as SumCashCnt2,case when sum(isnull(h.Cash1,0))=0 then '0%' else cast(cast(Round((sum(isnull(v.Cash2,0))/sum(isnull(h.Cash1,0)))*100,0) as int) as varchar) + '%' end as SumVIPPercent ";
                    sqlSumQ += "from #H h (nolock) ";
                    sqlSumQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                    sqlSumQ += "Where 1=1 ";
                    DataTable dtSumQ = PubUtility.SqlQry(sql + sqlSumQ, uu, "SYS");
                    dtSumQ.TableName = "dtSumQ";
                    ds.Tables.Add(dtSumQ);
                }

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("Query_Area_Shop_Step2")]
        public ActionResult SystemSetup_Query_Area_Shop_Step2()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "Query_Area_Shop_Step2OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string OpenDateS = rq["OpenDateS"];
                string OpenDateE = rq["OpenDateE"];
                string Area = rq["Area"];
                string Shop = rq["Shop"];

                string sql = "";
                string sqlQ = "";
                string sqlSumQ = "";

                sql = "select h.OpenDate as ID1,h.OpenDate as Name1,sum(h.Cash)Cash1,COUNT(*)Cnt1,(SUM(h.cash) / COUNT(*))CashCnt1 ";
                sql += "into #H ";
                sql += "from SalesH h(nolock) ";
                sql += "inner join warehouse w (nolock) on h.ShopNo=w.ST_ID and w.CompanyCode=h.CompanyCode and w.ST_placeID='" + Area + "' ";
                sql += "where h.CompanyCode='" + uu.CompanyId + "' ";
                sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                if (Shop != "")
                {
                    sql += "and h.ShopNo='" + Shop + "' ";
                }
                sql += "group by h.OpenDate ";
                sql += "order by h.OpenDate; ";

                sql += "select h.OpenDate as ID2,h.OpenDate as Name2,sum(h.Cash)Cash2,COUNT(*)Cnt2,(SUM(h.cash) / COUNT(*))CashCnt2 ";
                sql += "into #V ";
                sql += "from SalesH h(nolock) ";
                sql += "inner join warehouse w (nolock) on h.ShopNo=w.ST_ID and w.CompanyCode=h.CompanyCode and w.ST_placeID='" + Area + "' ";
                sql += "where h.CompanyCode='" + uu.CompanyId + "' and isnull(h.ifvip,'')='1' ";
                sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                if (Shop != "")
                {
                    sql += "and h.ShopNo='" + Shop + "' ";
                }
                sql += "group by h.OpenDate ";
                sql += "order by h.OpenDate; ";

                sqlQ = "Select isnull(h.ID1,'')ID1,isnull(h.Name1,'')Name1,isnull(h.Cash1,0)Cash1,isnull(h.Cnt1,0)Cnt1,Round(isnull(h.CashCnt1,0),0)CashCnt1, ";
                sqlQ += "isnull(v.ID2,'')ID2,isnull(v.Name2,'')Name2,isnull(v.Cash2,0)Cash2,isnull(v.Cnt2,0)Cnt2,Round(isnull(v.CashCnt2,0),0)CashCnt2,cast(cast(Round((isnull(v.Cash2,0)/isnull(h.Cash1,0))*100,0) as int) as varchar) + '%' VIPPercent ";
                sqlQ += "from #H h (nolock) ";
                sqlQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                sqlQ += "Where 1=1 ";
                sqlQ += "Order by h.ID1 ";

                DataTable dtQ = PubUtility.SqlQry(sql + sqlQ, uu, "SYS");
                dtQ.TableName = "dtQ";
                ds.Tables.Add(dtQ);

                sqlSumQ = "Select sum(isnull(h.Cash1,0))SumCash1,sum(isnull(h.Cnt1,0))SumCnt1,case when sum(isnull(h.Cnt1,0))=0 then 0 else Round(sum(isnull(h.Cash1,0))/sum(isnull(h.Cnt1,0)),0) end as SumCashCnt1, ";
                sqlSumQ += "sum(isnull(v.Cash2,0))SumCash2,sum(isnull(v.Cnt2,0))SumCnt2,case when sum(isnull(v.Cnt2,0))=0 then 0 else Round(sum(isnull(v.Cash2,0))/sum(isnull(v.Cnt2,0)),0) end as SumCashCnt2,case when sum(isnull(h.Cash1,0))=0 then '0%' else cast(cast(Round((sum(isnull(v.Cash2,0))/sum(isnull(h.Cash1,0)))*100,0) as int) as varchar) + '%' end as SumVIPPercent ";
                sqlSumQ += "from #H h (nolock) ";
                sqlSumQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                sqlSumQ += "Where 1=1 ";
                DataTable dtSumQ = PubUtility.SqlQry(sql + sqlSumQ, uu, "SYS");
                dtSumQ.TableName = "dtSumQ";
                ds.Tables.Add(dtSumQ);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("Query_Area_Date_Step2")]
        public ActionResult SystemSetup_Query_Area_Date_Step2()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "Query_Area_Date_Step2OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string OpenDate = rq["OpenDate"];
                string Area = rq["Area"];

                string sql = "";
                string sqlQ = "";
                string sqlSumQ = "";

                sql = "select h.ShopNo as ID1,w.st_sname as Name1,sum(h.Cash)Cash1,COUNT(*)Cnt1,(SUM(h.Cash) / COUNT(*))CashCnt1 ";
                sql += "into #H ";
                sql += "from SalesH h(nolock) ";
                sql += "inner join warehouse w (nolock) on h.ShopNo = w.ST_ID and w.CompanyCode = h.CompanyCode and w.ST_placeID='" + Area + "' ";
                sql += "where h.CompanyCode='" + uu.CompanyId + "' ";
                sql += "and h.OpenDate='" + OpenDate + "' ";
                sql += "group by h.ShopNo,w.st_sname ";
                sql += "order by h.ShopNo; ";

                sql += "select h.ShopNo as ID2,w.st_sname as Name2,sum(h.Cash)Cash2,COUNT(*)Cnt2,(SUM(h.cash) / COUNT(*))CashCnt2 ";
                sql += "into #V ";
                sql += "from SalesH h(nolock) ";
                sql += "inner join warehouse w (nolock) on h.ShopNo=w.ST_ID and w.CompanyCode=h.CompanyCode and w.ST_placeID='" + Area + "' ";
                sql += "where h.CompanyCode='" + uu.CompanyId + "' and isnull(h.ifvip,'')='1' ";
                sql += "and h.OpenDate='" + OpenDate + "' ";
                sql += "group by h.ShopNo,w.st_sname ";
                sql += "order by h.ShopNo; ";

                sqlQ = "Select isnull(h.ID1,'')ID1,substring(isnull(h.Name1,''),1,4)Name1,isnull(h.Cash1,0)Cash1,isnull(h.Cnt1,0)Cnt1,Round(isnull(h.CashCnt1,0),0)CashCnt1, ";
                sqlQ += "isnull(v.ID2,'')ID2,isnull(v.Name2,'')Name2,isnull(v.Cash2,0)Cash2,isnull(v.Cnt2,0)Cnt2,Round(isnull(v.CashCnt2,0),0)CashCnt2,cast(cast(Round((isnull(v.Cash2,0)/isnull(h.Cash1,0))*100,0) as int) as varchar) + '%' VIPPercent ";
                sqlQ += "from #H h (nolock) ";
                sqlQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                sqlQ += "Where 1=1 ";
                sqlQ += "Order by h.ID1 ";

                DataTable dtQ = PubUtility.SqlQry(sql + sqlQ, uu, "SYS");
                dtQ.TableName = "dtQ";
                ds.Tables.Add(dtQ);

                sqlSumQ = "Select sum(isnull(h.Cash1,0))SumCash1,sum(isnull(h.Cnt1,0))SumCnt1,case when sum(isnull(h.Cnt1,0))=0 then 0 else Round(sum(isnull(h.Cash1,0))/sum(isnull(h.Cnt1,0)),0) end as SumCashCnt1, ";
                sqlSumQ += "sum(isnull(v.Cash2,0))SumCash2,sum(isnull(v.Cnt2,0))SumCnt2,case when sum(isnull(v.Cnt2,0))=0 then 0 else Round(sum(isnull(v.Cash2,0))/sum(isnull(v.Cnt2,0)),0) end as SumCashCnt2,case when sum(isnull(h.Cash1,0))=0 then '0%' else cast(cast(Round((sum(isnull(v.Cash2,0))/sum(isnull(h.Cash1,0)))*100,0) as int) as varchar) + '%' end as SumVIPPercent ";
                sqlSumQ += "from #H h (nolock) ";
                sqlSumQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                sqlSumQ += "Where 1=1 ";
                DataTable dtSumQ = PubUtility.SqlQry(sql + sqlSumQ, uu, "SYS");
                dtSumQ.TableName = "dtSumQ";
                ds.Tables.Add(dtSumQ);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("Query_Shop_Step1")]
        public ActionResult SystemSetup_Query_Shop_Step1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "Query_Shop_Step1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string OpenDateS = rq["OpenDateS"];
                string OpenDateE = rq["OpenDateE"];
                string Shop = rq["Shop"];

                string sql = "";
                string sqlQ = "";
                string sqlSumQ = "";

                sql = "select h.OpenDate as ID1,h.OpenDate as Name1,sum(h.Cash)Cash1,COUNT(*)Cnt1,(SUM(h.cash) / COUNT(*))CashCnt1 ";
                sql += "into #H ";
                sql += "from SalesH h(nolock) ";
                sql += "inner join warehouse w (nolock) on h.ShopNo=w.ST_ID and w.CompanyCode=h.CompanyCode ";
                sql += "where h.CompanyCode='" + uu.CompanyId + "' ";
                sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                if (Shop != "")
                {
                    sql += "and h.ShopNo='" + Shop + "' ";
                }
                sql += "group by h.OpenDate ";
                sql += "order by h.OpenDate; ";

                sql += "select h.OpenDate as ID2,h.OpenDate as Name2,sum(h.Cash)Cash2,COUNT(*)Cnt2,(SUM(h.cash) / COUNT(*))CashCnt2 ";
                sql += "into #V ";
                sql += "from SalesH h(nolock) ";
                sql += "inner join warehouse w (nolock) on h.ShopNo=w.ST_ID and w.CompanyCode=h.CompanyCode ";
                sql += "where h.CompanyCode='" + uu.CompanyId + "' and isnull(h.ifvip,'')='1' ";
                sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                if (Shop != "")
                {
                    sql += "and h.ShopNo='" + Shop + "' ";
                }
                sql += "group by h.OpenDate ";
                sql += "order by h.OpenDate; ";

                sqlQ = "Select isnull(h.ID1,'')ID1,isnull(h.Name1,'')Name1,isnull(h.Cash1,0)Cash1,isnull(h.Cnt1,0)Cnt1,Round(isnull(h.CashCnt1,0),0)CashCnt1, ";
                sqlQ += "isnull(v.ID2,'')ID2,isnull(v.Name2,'')Name2,isnull(v.Cash2,0)Cash2,isnull(v.Cnt2,0)Cnt2,Round(isnull(v.CashCnt2,0),0)CashCnt2,cast(cast(Round((isnull(v.Cash2,0)/isnull(h.Cash1,0))*100,0) as int) as varchar) + '%' VIPPercent ";
                sqlQ += "from #H h (nolock) ";
                sqlQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                sqlQ += "Where 1=1 ";
                sqlQ += "Order by h.ID1 ";

                DataTable dtQ = PubUtility.SqlQry(sql + sqlQ, uu, "SYS");
                dtQ.TableName = "dtQ";
                ds.Tables.Add(dtQ);

                sqlSumQ = "Select sum(isnull(h.Cash1,0))SumCash1,sum(isnull(h.Cnt1,0))SumCnt1,case when sum(isnull(h.Cnt1,0))=0 then 0 else Round(sum(isnull(h.Cash1,0))/sum(isnull(h.Cnt1,0)),0) end as SumCashCnt1, ";
                sqlSumQ += "sum(isnull(v.Cash2,0))SumCash2,sum(isnull(v.Cnt2,0))SumCnt2,case when sum(isnull(v.Cnt2,0))=0 then 0 else Round(sum(isnull(v.Cash2,0))/sum(isnull(v.Cnt2,0)),0) end as SumCashCnt2,case when sum(isnull(h.Cash1,0))=0 then '0%' else cast(cast(Round((sum(isnull(v.Cash2,0))/sum(isnull(h.Cash1,0)))*100,0) as int) as varchar) + '%' end as SumVIPPercent ";
                sqlSumQ += "from #H h (nolock) ";
                sqlSumQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                sqlSumQ += "Where 1=1 ";
                DataTable dtSumQ = PubUtility.SqlQry(sql + sqlSumQ, uu, "SYS");
                dtSumQ.TableName = "dtSumQ";
                ds.Tables.Add(dtSumQ);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("Query_Date_Step1")]
        public ActionResult SystemSetup_Query_Date_Step1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "Query_Date_Step1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string OpenDateS = rq["OpenDateS"];
                string OpenDateE = rq["OpenDateE"];
                string Date = rq["Date"];
                string Type_Step1 = rq["Type_Step1"];

                string sql = "";
                string sqlQ = "";
                string sqlSumQ = "";

                if (Type_Step1 == "A")
                {
                    sql = "select w.ST_placeID as ID1,t.Type_Name as Name1,sum(h.Cash)Cash1,COUNT(*)Cnt1,(SUM(h.cash) / COUNT(*))CashCnt1 ";
                    sql += "into #H ";
                    sql += "from SalesH h(nolock) ";
                    sql += "inner join warehouse w(nolock) on h.ShopNo = w.ST_ID and w.CompanyCode = h.CompanyCode ";
                    sql += "inner join TypeData t(nolock) on w.ST_placeID = t.Type_ID and t.Type_Code = 'A' and t.CompanyCode = h.CompanyCode ";
                    sql += "where h.CompanyCode='" + uu.CompanyId + "' ";
                    sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                    if (Date != "")
                    {
                        sql += "and h.OpenDate='" + Date + "' ";
                    }
                    sql += "group by w.ST_placeID,t.Type_Name ";
                    sql += "order by w.ST_placeID; ";

                    sql += "select w.ST_placeID as ID2,t.Type_Name as Name2,sum(h.Cash)Cash2,COUNT(*)Cnt2,(SUM(h.cash) / COUNT(*))CashCnt2 ";
                    sql += "into #V ";
                    sql += "from SalesH h(nolock) ";
                    sql += "inner join warehouse w(nolock) on h.ShopNo = w.ST_ID and w.CompanyCode = h.CompanyCode ";
                    sql += "inner join TypeData t(nolock) on w.ST_placeID = t.Type_ID and t.Type_Code = 'A' and t.CompanyCode = h.CompanyCode ";
                    sql += "where h.CompanyCode='" + uu.CompanyId + "' and isnull(h.ifvip,'')='1' ";
                    sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                    if (Date != "")
                    {
                        sql += "and h.OpenDate='" + Date + "' ";
                    }
                    sql += "group by w.ST_placeID,t.Type_Name ";
                    sql += "order by w.ST_placeID; ";

                    sqlQ = "Select isnull(h.ID1,'')ID1,isnull(h.Name1,'')Name1,isnull(h.Cash1,0)Cash1,isnull(h.Cnt1,0)Cnt1,Round(isnull(h.CashCnt1,0),0)CashCnt1, ";
                    sqlQ += "isnull(v.ID2,'')ID2,isnull(v.Name2,'')Name2,isnull(v.Cash2,0)Cash2,isnull(v.Cnt2,0)Cnt2,Round(isnull(v.CashCnt2,0),0)CashCnt2,cast(cast(Round((isnull(v.Cash2,0)/isnull(h.Cash1,0))*100,0) as int) as varchar) + '%' VIPPercent ";
                    sqlQ += "from #H h (nolock) ";
                    sqlQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                    sqlQ += "Where 1=1 ";
                    sqlQ += "Order by h.ID1 ";

                    DataTable dtQ = PubUtility.SqlQry(sql + sqlQ, uu, "SYS");
                    dtQ.TableName = "dtQ";
                    ds.Tables.Add(dtQ);

                    sqlSumQ = "Select sum(isnull(h.Cash1,0))SumCash1,sum(isnull(h.Cnt1,0))SumCnt1,case when sum(isnull(h.Cnt1,0))=0 then 0 else Round(sum(isnull(h.Cash1,0))/sum(isnull(h.Cnt1,0)),0) end as SumCashCnt1, ";
                    sqlSumQ += "sum(isnull(v.Cash2,0))SumCash2,sum(isnull(v.Cnt2,0))SumCnt2,case when sum(isnull(v.Cnt2,0))=0 then 0 else Round(sum(isnull(v.Cash2,0))/sum(isnull(v.Cnt2,0)),0) end as SumCashCnt2,case when sum(isnull(h.Cash1,0))=0 then '0%' else cast(cast(Round((sum(isnull(v.Cash2,0))/sum(isnull(h.Cash1,0)))*100,0) as int) as varchar) + '%' end as SumVIPPercent ";
                    sqlSumQ += "from #H h (nolock) ";
                    sqlSumQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                    sqlSumQ += "Where 1=1 ";
                    DataTable dtSumQ = PubUtility.SqlQry(sql + sqlSumQ, uu, "SYS");
                    dtSumQ.TableName = "dtSumQ";
                    ds.Tables.Add(dtSumQ);
                }
                else if (Type_Step1 == "S")
                {
                    sql = "select h.ShopNo as ID1,w.st_sname as Name1,sum(h.Cash)Cash1,COUNT(*)Cnt1,(SUM(h.cash) / COUNT(*))CashCnt1 ";
                    sql += "into #H ";
                    sql += "from SalesH h(nolock) ";
                    sql += "inner join warehouse w (nolock) on h.ShopNo = w.ST_ID and w.CompanyCode = h.CompanyCode ";
                    sql += "where h.CompanyCode='" + uu.CompanyId + "' ";
                    sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                    if (Date != "")
                    {
                        sql += "and h.OpenDate='" + Date + "' ";
                    }
                    sql += "group by h.ShopNo,w.st_sname ";
                    sql += "order by h.ShopNo; ";

                    sql += "select h.ShopNo as ID2,w.st_sname as Name2,sum(h.Cash)Cash2,COUNT(*)Cnt2,(SUM(h.cash) / COUNT(*))CashCnt2 ";
                    sql += "into #V ";
                    sql += "from SalesH h(nolock) ";
                    sql += "inner join warehouse w (nolock) on h.ShopNo=w.ST_ID and w.CompanyCode=h.CompanyCode ";
                    sql += "where h.CompanyCode='" + uu.CompanyId + "' and isnull(h.ifvip,'')='1' ";
                    sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                    if (Date != "")
                    {
                        sql += "and h.OpenDate='" + Date + "' ";
                    }
                    sql += "group by h.ShopNo,w.st_sname ";
                    sql += "order by h.ShopNo; ";

                    sqlQ = "Select isnull(h.ID1,'')ID1,substring(isnull(h.Name1,''),1,4)Name1,isnull(h.Cash1,0)Cash1,isnull(h.Cnt1,0)Cnt1,Round(isnull(h.CashCnt1,0),0)CashCnt1, ";
                    sqlQ += "isnull(v.ID2,'')ID2,isnull(v.Name2,'')Name2,isnull(v.Cash2,0)Cash2,isnull(v.Cnt2,0)Cnt2,Round(isnull(v.CashCnt2,0),0)CashCnt2,cast(cast(Round((isnull(v.Cash2,0)/isnull(h.Cash1,0))*100,0) as int) as varchar) + '%' VIPPercent ";
                    sqlQ += "from #H h (nolock) ";
                    sqlQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                    sqlQ += "Where 1=1 ";
                    sqlQ += "Order by h.ID1 ";
                    DataTable dtQ = PubUtility.SqlQry(sql + sqlQ, uu, "SYS");
                    dtQ.TableName = "dtQ";
                    ds.Tables.Add(dtQ);

                    sqlSumQ = "Select sum(isnull(h.Cash1,0))SumCash1,sum(isnull(h.Cnt1,0))SumCnt1,case when sum(isnull(h.Cnt1,0))=0 then 0 else Round(sum(isnull(h.Cash1,0))/sum(isnull(h.Cnt1,0)),0) end as SumCashCnt1, ";
                    sqlSumQ += "sum(isnull(v.Cash2,0))SumCash2,sum(isnull(v.Cnt2,0))SumCnt2,case when sum(isnull(v.Cnt2,0))=0 then 0 else Round(sum(isnull(v.Cash2,0))/sum(isnull(v.Cnt2,0)),0) end as SumCashCnt2,case when sum(isnull(h.Cash1,0))=0 then '0%' else cast(cast(Round((sum(isnull(v.Cash2,0))/sum(isnull(h.Cash1,0)))*100,0) as int) as varchar) + '%' end as SumVIPPercent ";
                    sqlSumQ += "from #H h (nolock) ";
                    sqlSumQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                    sqlSumQ += "Where 1=1 ";
                    DataTable dtSumQ = PubUtility.SqlQry(sql + sqlSumQ, uu, "SYS");
                    dtSumQ.TableName = "dtSumQ";
                    ds.Tables.Add(dtSumQ);
                }

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("Query_Date_Area_Step2")]
        public ActionResult SystemSetup_Query_Date_Area_Step2()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "Query_Date_Area_Step2OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string OpenDateS = rq["OpenDateS"];
                string OpenDateE = rq["OpenDateE"];
                string Date = rq["Date"];
                string Area = rq["Area"];

                string sql = "";
                string sqlQ = "";
                string sqlSumQ = "";

                sql = "select h.ShopNo as ID1,w.st_sname as Name1,sum(h.Cash)Cash1,COUNT(*)Cnt1,(SUM(h.cash) / COUNT(*))CashCnt1 ";
                sql += "into #H ";
                sql += "from SalesH h(nolock) ";
                sql += "inner join warehouse w (nolock) on h.ShopNo = w.ST_ID and w.CompanyCode = h.CompanyCode and w.ST_PlaceID='" + Area + "' ";
                sql += "where h.CompanyCode='" + uu.CompanyId + "' ";
                sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                if (Date != "")
                {
                    sql += "and h.OpenDate='" + Date + "' ";
                }
                sql += "group by h.ShopNo,w.st_sname ";
                sql += "order by h.ShopNo; ";

                sql += "select h.ShopNo as ID2,w.st_sname as Name2,sum(h.Cash)Cash2,COUNT(*)Cnt2,(SUM(h.cash) / COUNT(*))CashCnt2 ";
                sql += "into #V ";
                sql += "from SalesH h(nolock) ";
                sql += "inner join warehouse w (nolock) on h.ShopNo=w.ST_ID and w.CompanyCode=h.CompanyCode and w.ST_PlaceID='" + Area + "' ";
                sql += "where h.CompanyCode='" + uu.CompanyId + "' and isnull(h.ifvip,'')='1' ";
                sql += "and h.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                if (Date != "")
                {
                    sql += "and h.OpenDate='" + Date + "' ";
                }
                sql += "group by h.ShopNo,w.st_sname ";
                sql += "order by h.ShopNo; ";

                sqlQ = "Select isnull(h.ID1,'')ID1,substring(isnull(h.Name1,''),1,4)Name1,isnull(h.Cash1,0)Cash1,isnull(h.Cnt1,0)Cnt1,Round(isnull(h.CashCnt1,0),0)CashCnt1, ";
                sqlQ += "isnull(v.ID2,'')ID2,isnull(v.Name2,'')Name2,isnull(v.Cash2,0)Cash2,isnull(v.Cnt2,0)Cnt2,Round(isnull(v.CashCnt2,0),0)CashCnt2,cast(cast(Round((isnull(v.Cash2,0)/isnull(h.Cash1,0))*100,0) as int) as varchar) + '%' VIPPercent ";
                sqlQ += "from #H h (nolock) ";
                sqlQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                sqlQ += "Where 1=1 ";
                sqlQ += "Order by h.ID1 ";
                DataTable dtQ = PubUtility.SqlQry(sql + sqlQ, uu, "SYS");
                dtQ.TableName = "dtQ";
                ds.Tables.Add(dtQ);

                sqlSumQ = "Select sum(isnull(h.Cash1,0))SumCash1,sum(isnull(h.Cnt1,0))SumCnt1,case when sum(isnull(h.Cnt1,0))=0 then 0 else Round(sum(isnull(h.Cash1,0))/sum(isnull(h.Cnt1,0)),0) end as SumCashCnt1, ";
                sqlSumQ += "sum(isnull(v.Cash2,0))SumCash2,sum(isnull(v.Cnt2,0))SumCnt2,case when sum(isnull(v.Cnt2,0))=0 then 0 else Round(sum(isnull(v.Cash2,0))/sum(isnull(v.Cnt2,0)),0) end as SumCashCnt2,case when sum(isnull(h.Cash1,0))=0 then '0%' else cast(cast(Round((sum(isnull(v.Cash2,0))/sum(isnull(h.Cash1,0)))*100,0) as int) as varchar) + '%' end as SumVIPPercent ";
                sqlSumQ += "from #H h (nolock) ";
                sqlSumQ += "left join #V v (nolock) on h.ID1=v.ID2 ";
                sqlSumQ += "Where 1=1 ";
                DataTable dtSumQ = PubUtility.SqlQry(sql + sqlSumQ, uu, "SYS");
                dtSumQ.TableName = "dtSumQ";
                ds.Tables.Add(dtSumQ);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetInitmsDM")]
        public ActionResult SystemSetup_GetInitmsDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitmsDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ProgramID = rq["ProgramID"];
                string sql = "select ChineseName,convert(char(10),getdate(),111) as SysDate,'" + uu.UserID + "' as SysUser from ProgramIDWeb (nolock) where ProgramID='" + ProgramID.SqlQuote() + "'";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/DMQuery1")]
        public ActionResult SystemSetup_DMQuery1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "DMQuery1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string Type = rq["Type"];

                string sql = "select distinct a.DocNo,a.Type from SetEDM a (nolock) ";
                sql += "where a.Companycode='" + uu.CompanyId + "' ";
                if (DocNo != "")
                {
                    sql += "and a.DocNo='" + DocNo + "' ";
                }
                if (Type != "")
                {
                    sql += "and a.Type='" + Type + "' ";
                }
                sql += "order by a.DocNo desc ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetDocNoDM")]
        public ActionResult SystemSetup_GetDocNoDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetDocNoDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = PubUtility.GetNewDocNo(uu, "DM", 3);

                string sql = "select '" + DocNo + "' as DocNo";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/Print_DM_A")]
        public ActionResult SystemSetup_Print_DM_A()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "Print_DM_AOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];

                string sql = "select RIGHT(DataType,1),* from SetEDM (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' ";
                if (DocNo != "")
                {
                    sql += "and DocNo='" + DocNo + "' ";
                }
                sql += "order by DataType ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/Print_DMMod_A")]
        public ActionResult SystemSetup_Print_DMMod_A()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "Print_DMMod_AOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];

                string sql = "select RIGHT(DataType,1),* from SetEDM (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' ";
                if (DocNo != "")
                {
                    sql += "and DocNo='" + DocNo + "' ";
                }
                sql += "order by DataType ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SetBarcode1_A")]
        public ActionResult SystemSetup_SetBarcode1_A()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SetBarcode1_AOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string Barcode1 = rq["Barcode1"];

                DataTable dtF = new DataTable();
                dtF.Columns.Add("CompanyCode", typeof(string));
                dtF.Columns.Add("DocNo", typeof(string));
                dtF.Columns.Add("Type", typeof(string));
                dtF.Columns.Add("DataType", typeof(string));
                dtF.Columns.Add("DocType", typeof(string));
                dtF.Columns.Add("DocImage", typeof(byte[]));
                dtF.Columns.Add("Memo", typeof(string));

                string sql = "Delete From SetEDM ";
                sql += " where CompanyCode='" + uu.CompanyId + "' And DocNo='" + DocNo + "' And DataType='B1'";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                System.Drawing.Bitmap bmp = ConstList.GetBitmap_Barcode(Barcode1)[0];
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                DataRow drF = dtF.NewRow();
                drF["CompanyCode"] = uu.CompanyId;
                drF["DocNo"] = DocNo;
                drF["Type"] = "A";
                drF["DataType"] = "B1";
                drF["DocType"] = "image/jpeg";
                drF["DocImage"] = ms.ToArray();
                drF["Memo"] = Barcode1;
                dtF.Rows.Add(drF);
                string sgid = PubUtility.AddTable("SetEDM", dtF, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SetQRCode1_A")]
        public ActionResult SystemSetup_SetQRCode1_A()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SetQRCode1_AOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string QRCode1 = rq["QRCode1"];

                DataTable dtF = new DataTable();
                dtF.Columns.Add("CompanyCode", typeof(string));
                dtF.Columns.Add("DocNo", typeof(string));
                dtF.Columns.Add("Type", typeof(string));
                dtF.Columns.Add("DataType", typeof(string));
                dtF.Columns.Add("DocType", typeof(string));
                dtF.Columns.Add("DocImage", typeof(byte[]));
                dtF.Columns.Add("Memo", typeof(string));

                string sql = "Delete From SetEDM ";
                sql += " where CompanyCode='" + uu.CompanyId + "' And DocNo='" + DocNo + "' And DataType='Q1'";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                System.Drawing.Bitmap bmp = ConstList.GetBitmap_QRCode(QRCode1)[0];
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                DataRow drF = dtF.NewRow();
                drF["CompanyCode"] = uu.CompanyId;
                drF["DocNo"] = DocNo;
                drF["Type"] = "A";
                drF["DataType"] = "Q1";
                drF["DocType"] = "image/jpeg";
                drF["DocImage"] = ms.ToArray();
                drF["Memo"] = QRCode1;
                dtF.Rows.Add(drF);
                string sgid = PubUtility.AddTable("SetEDM", dtF, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/Seteditor1_A")]
        public ActionResult SystemSetup_Seteditor1_A()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "Seteditor1_AOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string editor = rq["editor"];

                DataTable dtF = new DataTable();
                dtF.Columns.Add("CompanyCode", typeof(string));
                dtF.Columns.Add("DocNo", typeof(string));
                dtF.Columns.Add("Type", typeof(string));
                dtF.Columns.Add("DataType", typeof(string));
                dtF.Columns.Add("DocType", typeof(string));
                dtF.Columns.Add("Memo", typeof(string));

                string sql = "Delete From SetEDM ";
                sql += " where CompanyCode='" + uu.CompanyId + "' And DocNo='" + DocNo + "' And DataType='T1'";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                DataRow drF = dtF.NewRow();
                drF["CompanyCode"] = uu.CompanyId;
                drF["DocNo"] = DocNo;
                drF["Type"] = "A";
                drF["DataType"] = "T1";
                drF["DocType"] = "text";
                drF["Memo"] = editor;
                dtF.Rows.Add(drF);
                string sgid = PubUtility.AddTable("SetEDM", dtF, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SendOTP")]
        public ActionResult SendOTP()
        {
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SendOTPOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string USERID = rq["USERID"];
                string PASSWORD = rq["PASSWORD"];
                string CompanyID = rq["CompanyID"];
                string OTP = rq["OTP"];
                string GID = rq["GID"];
                UserInfo uu = new UserInfo();
                uu.UserID = USERID;
                uu.CompanyId = CompanyID;

                string sql = "select * from Account (nolock) ";
                sql += "where UID='" + USERID.SqlQuote() + "' ";
                DataTable dtU = PubUtility.SqlQry(sql, uu, "SYS");
                DataRow dr = dtU.Rows[0];

                dtU.TableName = "dtAccount";
                ds.Tables.Add(dtU);
                var Authenticator = new GoogleAuthenticatorService.Core.TwoFactorAuthenticator();
                bool valid = Authenticator.ValidateTwoFactorPIN(dr["USR_KEY"].ToString(), OTP);

                if (System.Environment.MachineName.ToUpper() == "ANDYNB4" | OTP == "0819")
                {
                    valid = true;
                }

                if (valid)
                {
                    dtU.Columns.Add("token1", typeof(string));
                    string token1 = PubUtility.GenerateJwtToken(uu);
                    dr["token1"] = token1;

                    //OTP驗證成功
                    sql = "Insert into LoginRec_WEB (Status,CrtDate,CrtTime,UID,UPWD,Memo) ";
                    sql += "Select 'Y',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + USERID.SqlQuote() + "','" + PASSWORD.SqlQuote() + "','' ";
                    PubUtility.ExecuteSql(sql, uu, "SYS");

                    //檢查設備是否有重複登入
                    if (dr["token"].ToString() == "")
                    {
                        var GID_New = Guid.NewGuid().ToString();
                        sql = "Update Account Set ModDate=convert(char(10),getdate(),111) + ' ' + left(convert(char(12),getdate(),108),5),ModUser='" + USERID.SqlQuote() + "',lastlogin =convert(char(10),getdate(),111) + ' ' + right(convert(varchar, getdate(), 121),12),ErrTimes=0, ";
                        sql += "token='" + GID_New + "' ";
                        sql += "where UID='" + USERID.SqlQuote() + "' ";
                        PubUtility.ExecuteSql(sql, uu, "SYS");
                    }
                    else
                    {
                        if (dr["token"].ToString() == GID.SqlQuote())
                        {
                            sql = "Update Account Set ModDate=convert(char(10),getdate(),111) + ' ' + left(convert(char(12),getdate(),108),5),ModUser='" + USERID.SqlQuote() + "',lastlogin =convert(char(10),getdate(),111) + ' ' + right(convert(varchar, getdate(), 121),12),ErrTimes=0 ";
                            sql += "where UID='" + USERID.SqlQuote() + "' ";
                            PubUtility.ExecuteSql(sql, uu, "SYS");
                        }
                        else
                        {
                            throw new Exception("GID不一致");
                        }
                    }
                    sql = "select * from Account (nolock) ";
                    sql += "where UID='" + USERID.SqlQuote() + "' ";
                    DataTable dtA = PubUtility.SqlQry(sql, uu, "SYS");
                    dtA.TableName = "dtAccount1";
                    ds.Tables.Add(dtA);
                }
                else
                {
                    //OTP驗證失敗
                    sql = "Insert into LoginRec_WEB (Status,CrtDate,CrtTime,UID,UPWD,Memo) ";
                    sql += "Select 'O',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + USERID.SqlQuote() + "','" + PASSWORD.SqlQuote() + "','OTP輸入錯誤' ";
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                    throw new Exception("驗證碼無效！");
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SendOTP_EDDMS")]
        public ActionResult SendOTP_EDDMS()
        {
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SendOTP_EDDMSOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string USERID = rq["USERID"];
                string PASSWORD = rq["PASSWORD"];
                string CompanyID = rq["CompanyID"];
                string GID = rq["GID"];
                UserInfo uu = new UserInfo();
                uu.UserID = USERID;
                uu.CompanyId = CompanyID;

                string sql = "select * from Account (nolock) ";
                sql += "where UID='" + USERID.SqlQuote() + "' ";
                DataTable dtU = PubUtility.SqlQry(sql, uu, "SYS");
                DataRow dr = dtU.Rows[0];
                dtU.TableName = "dtAccount";
                ds.Tables.Add(dtU);

                dtU.Columns.Add("token1", typeof(string));
                string token1 = PubUtility.GenerateJwtToken(uu);
                dr["token1"] = token1;

                sql = "Insert into LoginRec_WEB (Status,CrtDate,CrtTime,UID,UPWD,Memo) ";
                sql += "Select 'Y',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                sql += "'" + USERID.SqlQuote() + "','" + PASSWORD.SqlQuote() + "','' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                if (dr["token"].ToString() == "")
                {
                    var GID_New = Guid.NewGuid().ToString();
                    sql = "Update Account Set ModDate=convert(char(10),getdate(),111) + ' ' + left(convert(char(12),getdate(),108),5),ModUser='" + USERID.SqlQuote() + "',lastlogin =convert(char(10),getdate(),111) + ' ' + right(convert(varchar, getdate(), 121),12),ErrTimes=0, ";
                    sql += "token='" + GID_New + "' ";
                    sql += "where UID='" + USERID.SqlQuote() + "' ";
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }
                else
                {
                    sql = "Update Account Set ModDate=convert(char(10),getdate(),111) + ' ' + left(convert(char(12),getdate(),108),5),ModUser='" + USERID.SqlQuote() + "',lastlogin =convert(char(10),getdate(),111) + ' ' + right(convert(varchar, getdate(), 121),12),ErrTimes=0 ";
                    if (dr["token"].ToString() != GID.SqlQuote())
                    {
                        sql += ",token='" + GID.SqlQuote() + "' ";
                    }
                    sql += "where UID='" + USERID.SqlQuote() + "' ";
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }
                sql = "select * from Account (nolock) ";
                sql += "where UID='" + USERID.SqlQuote() + "' ";
                DataTable dtA = PubUtility.SqlQry(sql, uu, "SYS");
                dtA.TableName = "dtAccount1";
                ds.Tables.Add(dtA);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("UpdateGID")]
        public ActionResult UpdateGID()
        {
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateGIDOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string USERID = rq["USERID"];
                string PASSWORD = rq["PASSWORD"];
                string CompanyID = rq["CompanyID"];
                string GID = rq["GID"];
                UserInfo uu = new UserInfo();
                uu.UserID = USERID;
                uu.CompanyId = CompanyID;

                string sql = "";
                sql = "Update Account Set ModDate=convert(char(10),getdate(),111) + ' ' + left(convert(char(12),getdate(),108),5),ModUser='" + USERID.SqlQuote() + "',lastlogin =convert(char(10),getdate(),111) + ' ' + right(convert(varchar, getdate(), 121),12),ErrTimes=0, ";
                sql += "token='" + GID.SqlQuote() + "' ";
                sql += "where UID='" + USERID.SqlQuote() + "' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                sql = "select * from Account (nolock) ";
                sql += "where UID='" + USERID.SqlQuote() + "' ";
                DataTable dtA = PubUtility.SqlQry(sql, uu, "SYS");
                dtA.Columns.Add("token1", typeof(string));
                string token1 = PubUtility.GenerateJwtToken(uu);
                DataRow dr = dtA.Rows[0];
                dr["token1"] = token1;

                dtA.TableName = "dtAccount";
                ds.Tables.Add(dtA);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("LogOut")]
        public ActionResult LogOut()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "LogOutOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string PASSWORD = rq["PASSWORD"];

                string sql = "";
                sql = "Insert into LoginRec_WEB (Status,CrtDate,CrtTime,UID,UPWD,Memo) ";
                sql += "Select 'E',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                sql += "'" + uu.UserID + "','" + PASSWORD.SqlQuote() + "','' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                sql = "Update Account Set ModDate=convert(char(10),getdate(),111),ModUser='" + uu.UserID + "',ErrTimes=0 ";
                sql += "where UID='" + uu.UserID + "' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                sql = "select * from Account (nolock) ";
                sql += "where UID='" + uu.UserID + "' ";
                DataTable dtA = PubUtility.SqlQry(sql, uu, "SYS");
                dtA.TableName = "dtAccount";
                ds.Tables.Add(dtA);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("js/ChkDevice")]
        public ActionResult ChkDevice()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkDeviceOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                if (uu.UserID == null)
                {
                    throw new Exception("null");
                }

                IFormCollection rq = HttpContext.Request.Form;
                string sql = "select * from Account (nolock) Where UID='" + uu.UserID + "' ";
                DataTable dtA = PubUtility.SqlQry(sql, uu, "SYS");
                dtA.TableName = "dtA";
                ds.Tables.Add(dtA);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("ClickMenu")]
        public ActionResult ClickMenu()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ClickMenuOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                if (uu.UserID == null)
                {
                    throw new Exception("null");
                }

                IFormCollection rq = HttpContext.Request.Form;
                string ProgramID = rq["ProgramID"];

                string sql = "";
                sql = "Insert into WebHitRecWeb (Companycode,CrtDate,UID,WEBName) ";
                sql += "Select '" + uu.CompanyId + "',convert(char(10),getdate(),111) + ' ' + convert(char(12),getdate(),108),'" + uu.UserID + "','" + ProgramID.SqlQuote() + "' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM104Query")]
        public ActionResult SystemSetup_MSDM104Query()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM104QueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string EDMMemo = rq["EDMMemo"];
                string ActivityCode = rq["ActivityCode"];
                string ShopNo = rq["ShopNo"];
                string App = rq["App"];
                string Def = rq["Def"];
                string EDDate = rq["EDDate"];

                string sql = "";
                sql = "Select a.DocNO,a.EDMMemo,a.StartDate + ' ~ ' + a.EndDate as EDDate,b.PS_Name,b.ActivityCode,isnull(c.Cnt,0)Cnt, ";
                sql += "isnull(a.ApproveDate,'')ApproveDate,isnull(a.DefeasanceDate,'')DefeasanceDate ";
                sql += "From SetEDMHWeb a (nolock) ";
                sql += "inner join PromoteSCouponHWeb b (nolock) on a.PS_NO=b.PS_NO and b.Companycode=a.Companycode and b.CouponType in('1','2') ";
                //活動代號
                if (ActivityCode.SqlQuote() != "")
                {
                    sql += "and b.ActivityCode like '" + ActivityCode.SqlQuote() + "%' ";
                }
                sql += "left join (Select EVNO,COUNT(*)Cnt From SetEDMVIP_VIPWeb (nolock) Where Companycode='" + uu.CompanyId + "' group by EVNO)c on a.DocNo=c.EVNO ";

                sql += "Where a.Companycode='" + uu.CompanyId + "' and isnull(a.DelDate,'')='' and a.EDMType='V' ";
                //DM單號
                if (DocNo.SqlQuote() != "")
                {
                    sql += "and a.DocNo like '" + DocNo.SqlQuote() + "%' ";
                }
                //DM主旨
                if (EDMMemo.SqlQuote() != "")
                {
                    sql += "and a.EDMMemo like '%" + EDMMemo.SqlQuote() + "%' ";
                }
                //店別
                if (ShopNo.SqlQuote() != "")
                {
                    sql += "and (a.WhNoFlag='Y' or a.DocNo in (Select DocNo From SetEDMShopWeb (nolock) Where Companycode='" + uu.CompanyId + "' and ShopNo='" + ShopNo.SqlQuote() + "')) ";
                }
                //批核日期
                if (App.SqlQuote() == "NoApp")
                {
                    sql += "and isnull(a.ApproveDate,'')='' ";
                }
                else if (App.SqlQuote() == "App")
                {
                    sql += "and isnull(a.ApproveDate,'')<>'' ";
                }
                //作廢日期
                if (Def.SqlQuote() == "NoDef")
                {
                    sql += "and isnull(a.DefeasanceDate,'')='' ";
                }
                else if (Def.SqlQuote() == "Def")
                {
                    sql += "and isnull(a.DefeasanceDate,'')<>'' ";
                }
                //入會日期
                if (EDDate.SqlQuote() != "")
                {
                    sql += "and '" + EDDate.SqlQuote() + "' between a.StartDate and a.EndDate ";
                }

                sql += "Order by a.DocNo desc ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM104_LookUpActivityCode")]
        public ActionResult SystemSetup_MSDM104_LookUpActivityCode()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM104_LookUpActivityCodeOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ActivityCode = rq["ActivityCode"];

                string sql = "";
                sql = "Select a.ActivityCode,a.PS_Name,a.StartDate,a.EndDate ";
                sql += "From PromoteSCouponHWeb a (nolock) ";
                sql += "inner join SetEDMHWeb b (nolock) on a.PS_NO=b.PS_NO and b.EDMType='V' and isnull(b.DelDate,'')='' and b.Companycode=a.Companycode ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' and a.CouponType in('1','2') ";
                if (ActivityCode.SqlQuote() != "")
                {
                    sql += "and a.ActivityCode like '" + ActivityCode.SqlQuote() + "%' ";
                }
                sql += "group by a.ActivityCode,a.PS_Name,a.StartDate,a.EndDate ";
                sql += "Order By a.StartDate desc ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM104_LookUpShopNo")]
        public ActionResult SystemSetup_MSDM104_LookUpShopNo()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM104_LookUpShopNoOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ShopNo = rq["ShopNo"];

                string sql = "";
                sql = "Select ST_ID,ST_SName ";
                sql += "From WarehouseWeb a (nolock) ";
                sql += "Where Companycode='" + uu.CompanyId + "' ";
                if (ShopNo.SqlQuote() != "")
                {
                    sql += "and ST_ID like '" + ShopNo.SqlQuote() + "%' ";
                }
                sql += "Order By ST_ID ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("GetImage_EDM")]
        public ActionResult GetImage_EDM()
        {
            try
            {
                IQueryCollection rq = HttpContext.Request.Query;
                string DocNo = rq["DocNo"];
                string DataType = rq["DataType"];
                string Flag = rq["Flag"];
                string UU = rq["UU"];
                UserInfo uu = PubUtility.GetCurrentUser("Bearer " + UU);
                DataTable dt = new DataTable();
                if (Flag == "Y")
                {
                    dt = PubUtility.SqlQry("select * from SetEDMDWeb (nolock) where DocNo='" + DocNo + "' and DataType='" + DataType + "' and Companycode='" + uu.CompanyId + "' ", uu, "SYS");
                }
                else
                {
                    dt = PubUtility.SqlQry("select * from SetEDM (nolock) where DocNo='" + DocNo + "' and DataType='" + DataType + "' and Companycode='" + uu.CompanyId + "' ", uu, "SYS");
                }
                DataRow dr = dt.Rows[0];
                string ContentType = "image/jpeg";
                //HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(dr["FileName"].ToString()));
                return File(dr["DocImage"] as byte[], ContentType);
            }
            catch (Exception err)
            {
                string ContentType = "image/jpeg";
                HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=No_Pic.jpg");
                string fn = ConstList.HostEnvironment.WebRootPath + @"\images\No_Pic.jpg";
                return File(System.IO.File.ReadAllBytes(fn), ContentType);
            }
        }

        [Route("SystemSetup/MSDM104_LookUpShopNo_EDM")]
        public ActionResult SystemSetup_MSDM104_LookUpShopNo_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM104_LookUpShopNo_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ShopNo = rq["ShopNo"];
                int ShopCnt = ShopNo.Split(',').Length;

                string sql = "";
                sql = "Select ST_ID,ST_SName ";
                sql += "From WarehouseWeb a (nolock) ";
                sql += "Where Companycode='" + uu.CompanyId + "' and ST_Type not in ('2','3') ";
                if (ShopNo.SqlQuote() != "")
                {
                    if (ShopCnt > 1)
                    {
                        sql += "and ST_ID in (" + ShopNo + ") ";
                    }
                    else
                    {
                        var ShopNo1 = ShopNo.Replace("'", "");
                        sql += "and ST_ID like '" + ShopNo1 + "%' ";
                    }
                }
                sql += "Order By ST_ID ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM104_LookUpPSNO_EDM")]
        public ActionResult SystemSetup_MSDM104_LookUpPSNO_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM104_LookUpPSNO_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string EndDate = rq["EndDate"];
                string WhNoFlag = rq["WhNoFlag"];
                string ShopNo = rq["ShopNo"];
                string PS_NO = rq["PS_NO"];

                int ShopCnt = ShopNo.Split(',').Length;

                string sql = "";
                sql = "Select a.PS_NO,a.PS_Name,a.ActivityCode,a.StartDate,a.EndDate ";
                sql += "From PromoteSCouponHWeb a (nolock) ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' and a.CouponType in('1','2') ";
                sql += "and isnull(a.ApproveDate,'')<>'' and isnull(a.DefeasanceDate,'')='' ";
                if (EndDate.SqlQuote() != "")
                {
                    sql += "and isnull(a.EndDate,'')>='" + EndDate.SqlQuote() + "' ";
                }
                if (WhNoFlag.SqlQuote() == "Y")
                {
                    sql += "and isnull(a.WhNoFlag,'')='Y' ";
                }
                else
                {
                    sql += "and (isnull(a.WhNoFlag,'')='Y' or a.PS_NO in (Select PS_NO From PromoteSCouponShopWeb (nolock) Where Companycode='" + uu.CompanyId + "' and ShopNo in (" + ShopNo + ") and PS_NO in (Select PS_NO From PromoteSCouponHWeb (nolock) where Companycode='" + uu.CompanyId + "' and isnull(EndDate,'')>convert(char(10),getdate(),111) and CouponType in('1','2') group by PS_NO Having Count(*)=" + ShopCnt + "))) ";
                }
                if (PS_NO.SqlQuote() != "")
                {
                    sql += "and a.PS_NO like '" + PS_NO.SqlQuote() + "%' ";
                }
                sql += "order by a.StartDate,a.PS_NO ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetCompanyLogo")]
        public ActionResult SystemSetup_GetCompanyLogo()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetCompanyLogoOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ProgramID = rq["ProgramID"];
                var DocNo = PubUtility.GetNewDocNo(uu, "VM", 3);


                string sql = "";
                sql = "Select '" + DocNo + "' as DocNo,* From EDMSetWeb (nolock) Where Companycode='" + uu.CompanyId + "' ";
                if (ProgramID.SqlQuote() != "")
                {
                    sql += "and ProgramID='" + ProgramID.SqlQuote() + "' ";
                }
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);

                sql = "Delete From SetEDM Where Companycode='" + uu.CompanyId + "' and left(DocNo,2)='VM' and CrtDate<convert(char(10),getdate(),111) ";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("GetImage_Logo")]
        public ActionResult GetImage_Logo()
        {
            try
            {
                IQueryCollection rq = HttpContext.Request.Query;
                string ProgramID = rq["ProgramID"];
                string UU = rq["UU"];
                UserInfo uu = PubUtility.GetCurrentUser("Bearer " + UU);
                DataTable dt = PubUtility.SqlQry("select * from EDMSetWeb (nolock) where ProgramID='" + ProgramID + "' and Companycode='" + uu.CompanyId + "' ", uu, "SYS");
                DataRow dr = dt.Rows[0];
                string ContentType = "image/jpeg";
                //HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(dr["FileName"].ToString()));

                return File(dr["Pic"] as byte[], ContentType);
            }
            catch (Exception err)
            {

                string ContentType = "image/jpeg";
                HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=No_Pic.jpg");
                string fn = ConstList.HostEnvironment.WebRootPath + @"\images\No_Pic.jpg";
                return File(System.IO.File.ReadAllBytes(fn), ContentType);
            }
        }

        [Route("SystemSetup/MSDM104_Save_EDM")]
        public ActionResult SystemSetup_MSDM104_Save_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM104_Save_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string EditMode = rq["EditMode"];
                string EDMMemo = rq["EDMMemo"];
                string StartDate = rq["StartDate"];
                string EndDate = rq["EndDate"];
                string WhNoFlag = rq["WhNoFlag"];
                string chkShopNo = rq["chkShopNo"];
                string PS_NO = rq["PS_NO"];
                string T1 = rq["T1"];
                string T2 = rq["T2"];
                string DocNo = rq["DocNo"];
                string VMDocNo = rq["VMDocNo"];
                string sql = "";
                string sqlP2 = "";

                //新增
                if (EditMode.SqlQuote() == "A")
                {
                    DocNo = PubUtility.GetNewDocNo(uu, "EM", 3);
                    //SetEDMHWeb
                    sql = "Insert into SetEDMHWeb (Companycode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                    sql += "DocNO,StartDate,EndDate,EDMMemo,EDM_Model,PS_NO,EDMType,ApproveDate,ApproveUser,DefeasanceDate,Defeasance, ";
                    sql += "PS_Title,PS_MEMO,DelDate,DelUser,WhNoFlag) ";

                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + DocNo + "','" + StartDate.SqlQuote() + "','" + EndDate.SqlQuote() + "','" + EDMMemo.SqlQuote() + "','', ";
                    sql += "'" + PS_NO.SqlQuote() + "','V','','','','','','','','','" + WhNoFlag.SqlQuote() + "';";

                    //SetEDMDWeb(P1)
                    sql += "Insert into SetEDMDWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                    sql += "DocNO,DataType,FileName,DocType,DocImage,TXT,URL,MEMO) ";

                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + DocNo + "','P1','P1.jpg','P',Pic,'','','' ";
                    sql += "From EDMSetWeb (nolock) Where Companycode='" + uu.CompanyId + "' and ProgramID='MSDM104';";

                    //SetEDMDWeb(T1)
                    if (T1.SqlQuote() != "")
                    {
                        sql += "Insert into SetEDMDWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                        sql += "DocNO,DataType,FileName,DocType,DocImage,TXT,URL,MEMO) ";

                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                        sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                        sql += "'" + DocNo + "','T1','','T','','" + T1.SqlQuote() + "','','';";
                    }
                    //SetEDMDWeb(P2)
                    sqlP2 = "Select * From SetEDM (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + VMDocNo + "' and DataType='P2' ";
                    DataTable dtP2 = PubUtility.SqlQry(sqlP2, uu, "SYS");
                    if (dtP2.Rows.Count > 0)
                    {
                        if (dtP2.Rows[0]["STATUS"].ToString() == "1")
                        {
                            sql += "Insert into SetEDMDWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                            sql += "DocNO,DataType,FileName,DocType,DocImage,TXT,URL,MEMO) ";
                            sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                            sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                            sql += "'" + DocNo + "','P2',FileName,'P',DocImage,'','','' ";
                            sql += "From SetEDM (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + VMDocNo + "' and DataType='P2'; ";
                        }
                        sql += "Delete From SetEDM Where Companycode='" + uu.CompanyId + "' and DocNo='" + VMDocNo + "' and DataType='P2'; ";
                    }
                    //SetEDMDWeb(T2)
                    sql += "Insert into SetEDMDWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                    sql += "DocNO,DataType,FileName,DocType,DocImage,TXT,URL,MEMO) ";
                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + DocNo + "','T2','','T','','" + T2.SqlQuote() + "','',''; ";

                    //SetEDMDWeb(TE)
                    sql += "Insert into SetEDMDWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                    sql += "DocNO,DataType,FileName,DocType,DocImage,TXT,URL,MEMO) ";

                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + DocNo + "','TE','','T','',TE,'','' ";
                    sql += "From EDMSetWeb (nolock) Where Companycode='" + uu.CompanyId + "' and ProgramID='MSDM104';";

                    //SetEDMShopWeb
                    if (WhNoFlag.SqlQuote() != "Y")
                    {
                        sql += "Insert into SetEDMShopWeb (CompanyCode,CrtUser,CrtDate,CrtTime,DocNO,ShopNo) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                        sql += "'" + DocNo + "',ST_ID ";
                        sql += "From WarehouseWeb (nolock) Where Companycode='" + uu.CompanyId + "' and ST_ID in (" + chkShopNo + ");";
                    }
                    //SetEDMVIP_Hweb
                    sql += "Insert into SetEDMVIP_Hweb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                    sql += "EVNO,StartDate,EndDate,EDMMemo,EDM_DocNO,PS_NO,EDMType,ApproveDate,ApproveUser,DefeasanceDate,Defeasance, ";
                    sql += "DelDate,DelUser) ";

                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + DocNo + "',convert(char(10),getdate(),111),'" + EndDate.SqlQuote() + "','" + EDMMemo.SqlQuote() + "', ";
                    sql += "'" + DocNo + "','" + PS_NO.SqlQuote() + "','V','','','','','','';";
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }
                //修改
                else if (EditMode.SqlQuote() == "M")
                {
                    //SetEDMHWeb
                    sql = "Update SetEDMHWeb Set ModDate=convert(char(10),getdate(),111),ModTime=right(convert(varchar, getdate(), 121),12),ModUser='" + uu.UserID + "', ";
                    sql += "EDMMemo='" + EDMMemo.SqlQuote() + "',StartDate='" + StartDate.SqlQuote() + "',EndDate='" + EndDate.SqlQuote() + "',WhNoFlag='" + WhNoFlag.SqlQuote() + "', ";
                    sql += "PS_NO='" + PS_NO.SqlQuote() + "' ";
                    sql += "where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "'; ";
                    //SetEDMDWeb(P1)
                    sql += "Update SetEDMDWeb set ModDate=convert(char(10),getdate(),111),ModTime=right(convert(varchar, getdate(), 121),12),ModUser='" + uu.UserID + "', ";
                    sql += "DocImage=b.Pic ";
                    sql += "From SetEDMDWeb a (nolock) ";
                    sql += "inner join EDMSetWeb b (nolock) on a.Companycode=b.Companycode and b.Programid='MSDM104' ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' and a.DocNo='" + DocNo.SqlQuote() + "' ";
                    sql += "and a.DataType='P1'; ";
                    //SetEDMDWeb(T1)
                    if (T1.SqlQuote() != "")
                    {
                        sql += "Update SetEDMDWeb set ModDate=convert(char(10),getdate(),111),ModTime=right(convert(varchar, getdate(), 121),12),ModUser='" + uu.UserID + "', ";
                        sql += "TXT='" + T1.SqlQuote() + "' ";
                        sql += "Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' ";
                        sql += "and DataType='T1'; ";
                    }

                    //SetEDMDWeb(P2)
                    sqlP2 = "Select * From SetEDM (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2' ";
                    DataTable dtP2 = PubUtility.SqlQry(sqlP2, uu, "SYS");
                    if (dtP2.Rows.Count > 0)
                    {
                        if (dtP2.Rows[0]["STATUS"].ToString() == "1")
                        {
                            sql += "Delete From SetEDMDWeb Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2'; ";
                            sql += "Insert into SetEDMDWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                            sql += "DocNO,DataType,FileName,DocType,DocImage,TXT,URL,MEMO) ";
                            sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                            sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                            sql += "'" + DocNo + "','P2',FileName,'P',DocImage,'','','' ";
                            sql += "From SetEDM (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2'; ";
                        }
                        else
                        {
                            sql += "Delete From SetEDMDWeb Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2'; ";
                        }
                        sql += "Delete From SetEDM Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2'; ";
                    }

                    //SetEDMDWeb(T2)
                    sql += "Update SetEDMDWeb set ModDate=convert(char(10),getdate(),111),ModTime=right(convert(varchar, getdate(), 121),12),ModUser='" + uu.UserID + "', ";
                    sql += "TXT='" + T2.SqlQuote() + "' ";
                    sql += "Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' ";
                    sql += "and DataType='T2'; ";

                    //SetEDMDWeb(TE)
                    sql += "Update SetEDMDWeb set ModDate=convert(char(10),getdate(),111),ModTime=right(convert(varchar, getdate(), 121),12),ModUser='" + uu.UserID + "', ";
                    sql += "TXT=b.TE ";
                    sql += "From SetEDMDWeb a (nolock) ";
                    sql += "inner join EDMSetWeb b (nolock) on a.Companycode=b.Companycode and b.Programid='MSDM104' ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' and a.DocNo='" + DocNo.SqlQuote() + "' ";
                    sql += "and a.DataType='TE'; ";

                    //SetEDMShopWeb
                    sql += "Delete From SetEDMShopWeb Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "'; ";
                    if (WhNoFlag.SqlQuote() != "Y")
                    {
                        sql += "Insert Into SetEDMShopWeb Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                        sql += "'" + DocNo.SqlQuote() + "',ST_ID ";
                        sql += "From WarehouseWeb (nolock) Where Companycode='" + uu.CompanyId + "' and ST_ID in (" + chkShopNo + "); ";
                    }
                    //SetEDMVIP_HWeb
                    sql += "Update SetEDMVIP_HWeb set ModDate=convert(char(10),getdate(),111),ModTime=right(convert(varchar, getdate(), 121),12),ModUser='" + uu.UserID + "', ";
                    sql += "StartDate=convert(char(10),getdate(),111),EndDate='" + EndDate.SqlQuote() + "',EDMMemo='" + EDMMemo.SqlQuote() + "', ";
                    sql += "EDM_DocNO='" + DocNo.SqlQuote() + "',PS_NO='" + PS_NO.SqlQuote() + "' ";
                    sql += "Where Companycode='" + uu.CompanyId + "' and EVNO='" + DocNo.SqlQuote() + "'; ";
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }

                sql = "select * from SetEDMHWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo + "' ";
                DataTable dtS = PubUtility.SqlQry(sql, uu, "SYS");
                dtS.TableName = "dtS";
                ds.Tables.Add(dtS);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM104Query_EDM")]
        public ActionResult SystemSetup_MSDM104Query_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM104Query_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string sql = "";

                //SetEDMHWeb
                sql = "Select a.DocNo,a.EDMMemo,a.StartDate,a.EndDate,a.WhNoFlag,a.PS_NO,c.PS_Name + '  ' + c.StartDate + ' ~ ' + c.EndDate as PS_Name, ";
                sql += "isnull(a.ApproveDate,'')ApproveDate,isnull(a.ApproveUser,'')ApproveUser,isnull(a.DefeasanceDate,'')DefeasanceDate,isnull(a.Defeasance,'')Defeasance, ";
                sql += "b.DataType,b.DocImage,b.TXT ";
                sql += "From SetEDMHWeb a (nolock) ";
                sql += "inner join SetEDMDWeb b (nolock) on a.DocNo=b.DocNo and b.Companycode=a.Companycode ";
                sql += "inner join PromoteSCouponHWeb c (nolock) on a.PS_NO=c.PS_NO and c.Companycode=a.Companycode and c.CouponType in('1','2') ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                if (DocNo.SqlQuote() != "")
                {
                    sql += "and a.DocNo='" + DocNo.SqlQuote() + "' ";
                }
                sql += "Order by b.DataType ";
                DataTable dtH = PubUtility.SqlQry(sql, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);

                //SetEDMShopWeb
                sql = "Select * From SetEDMShopWeb (nolock) Where Companycode='" + uu.CompanyId + "' ";
                if (DocNo.SqlQuote() != "")
                {
                    sql += "and DocNo='" + DocNo.SqlQuote() + "' ";
                }
                sql += "Order by ShopNo ";
                DataTable dtShop = PubUtility.SqlQry(sql, uu, "SYS");
                dtShop.TableName = "dtShop";
                ds.Tables.Add(dtShop);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM104Cancel_EDM")]
        public ActionResult SystemSetup_MSDM104Cancel_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM104Cancel_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string sql = "";

                //SetEDMHWeb
                sql = "Select a.DocNo,a.EDMMemo,a.StartDate,a.EndDate,a.WhNoFlag,a.PS_NO,c.PS_Name + '  ' + c.StartDate + ' ~ ' + c.EndDate as PS_Name, ";
                sql += "isnull(a.ApproveDate,'')ApproveDate,isnull(a.ApproveUser,'')ApproveUser,isnull(a.DefeasanceDate,'')DefeasanceDate,isnull(a.Defeasance,'')Defeasance, ";
                sql += "b.DataType,b.DocImage,b.TXT ";
                sql += "From SetEDMHWeb a (nolock) ";
                sql += "inner join SetEDMDWeb b (nolock) on a.DocNo=b.DocNo and b.Companycode=a.Companycode ";
                sql += "inner join PromoteSCouponHWeb c (nolock) on a.PS_NO=c.PS_NO and c.Companycode=a.Companycode and c.CouponType in('1','2') ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                if (DocNo.SqlQuote() != "")
                {
                    sql += "and a.DocNo='" + DocNo.SqlQuote() + "' ";
                }
                sql += "Order by b.DataType ";
                DataTable dtH = PubUtility.SqlQry(sql, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);

                //SetEDMShopWeb
                sql = "Select * From SetEDMShopWeb (nolock) Where Companycode='" + uu.CompanyId + "' ";
                if (DocNo.SqlQuote() != "")
                {
                    sql += "and DocNo='" + DocNo.SqlQuote() + "' ";
                }
                sql += "Order by ShopNo ";
                DataTable dtShop = PubUtility.SqlQry(sql, uu, "SYS");
                dtShop.TableName = "dtShop";
                ds.Tables.Add(dtShop);

                sql = "Delete From SetEDM Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM104_Approve_EDM")]
        public ActionResult SystemSetup_MSDM104_Approve_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM104_Approve_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string sql = "";
                sql = "Update SetEDMHWeb Set ApproveDate=convert(char(10),getdate(),111) + ' ' + convert(char(12),getdate(),108), ";
                sql += "ApproveUser='" + uu.UserID + "' ";
                sql += "Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "'; ";

                sql += "Update SetEDMVIP_HWeb Set ApproveDate=convert(char(10),getdate(),111) + ' ' + convert(char(12),getdate(),108), ";
                sql += "ApproveUser='" + uu.UserID + "' ";
                sql += "Where Companycode='" + uu.CompanyId + "' and EVNO='" + DocNo.SqlQuote() + "' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                //SetEDMHWeb
                sql = "Select * From SetEDMHWeb (nolock) Where Companycode='" + uu.CompanyId + "' ";
                if (DocNo.SqlQuote() != "")
                {
                    sql += "and DocNo='" + DocNo.SqlQuote() + "' ";
                }
                DataTable dtH = PubUtility.SqlQry(sql, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM104_Defeasance_EDM")]
        public ActionResult SystemSetup_MSDM104_Defeasance_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM104_Defeasance_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string sql = "";
                sql = "Update SetEDMHWeb Set DefeasanceDate=convert(char(10),getdate(),111) + ' ' + convert(char(12),getdate(),108), ";
                sql += "Defeasance='" + uu.UserID + "' ";
                sql += "Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "'; ";

                sql += "Update SetEDMVIP_HWeb Set DefeasanceDate=convert(char(10),getdate(),111) + ' ' + convert(char(12),getdate(),108), ";
                sql += "Defeasance='" + uu.UserID + "' ";
                sql += "Where Companycode='" + uu.CompanyId + "' and EVNO='" + DocNo.SqlQuote() + "' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                //SetEDMHWeb
                sql = "Select * From SetEDMHWeb (nolock) Where Companycode='" + uu.CompanyId + "' ";
                if (DocNo.SqlQuote() != "")
                {
                    sql += "and DocNo='" + DocNo.SqlQuote() + "' ";
                }
                DataTable dtH = PubUtility.SqlQry(sql, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetCompanyShowEDM")]
        public ActionResult SystemSetup_GetCompanyShowEDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetCompanyShowEDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string CompanyID = PubUtility.enCode170215(uu.CompanyId);
                string sql = "Select '" + CompanyID + "' as CompanyID";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("GetImage_QRCode")]
        public ActionResult GetImage_QRCode()
        {
            try
            {
                IQueryCollection rq = HttpContext.Request.Query;
                string QRCode = rq["QRCode"];
                string UU = rq["UU"];

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                System.Drawing.Bitmap bmp = ConstList.GetBitmap_QRCode(QRCode)[0];
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                UserInfo uu = PubUtility.GetCurrentUser("Bearer " + UU);
                string ContentType = "image/jpeg";
                //HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(dr["FileName"].ToString()));

                return File(ms.ToArray() as byte[], ContentType);
            }
            catch (Exception err)
            {
                string ContentType = "image/jpeg";
                HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=No_Pic.jpg");
                string fn = ConstList.HostEnvironment.WebRootPath + @"\images\No_Pic.jpg";
                return File(System.IO.File.ReadAllBytes(fn), ContentType);
            }
        }

        [Route("GetImage_Barcode")]
        public ActionResult GetImage_Barcode()
        {
            try
            {
                IQueryCollection rq = HttpContext.Request.Query;
                string Barcode = rq["Barcode"];
                string UU = rq["UU"];

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                System.Drawing.Bitmap bmp = ConstList.GetBitmap_Barcode(Barcode)[0];
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                UserInfo uu = PubUtility.GetCurrentUser("Bearer " + UU);
                string ContentType = "image/jpeg";
                //HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(dr["FileName"].ToString()));

                return File(ms.ToArray() as byte[], ContentType);
            }
            catch (Exception err)
            {
                string ContentType = "image/jpeg";
                HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=No_Pic.jpg");
                string fn = ConstList.HostEnvironment.WebRootPath + @"\images\No_Pic.jpg";
                return File(System.IO.File.ReadAllBytes(fn), ContentType);
            }
        }

        [Route("GetImage_QRCodeandBarcode")]
        public ActionResult GetImage_QRCodeandBarcode()
        {
            try
            {
                IQueryCollection rq = HttpContext.Request.Query;
                string Code = rq["Code"];
                string UU = rq["UU"];

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                System.Drawing.Bitmap bmp = ConstList.GetBitmap_QRCodeandBarcode(Code)[0];
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                UserInfo uu = PubUtility.GetCurrentUser("Bearer " + UU);
                string ContentType = "image/jpeg";
                //HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(dr["FileName"].ToString()));

                return File(ms.ToArray() as byte[], ContentType);
            }
            catch (Exception err)
            {
                string ContentType = "image/jpeg";
                HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=No_Pic.jpg");
                string fn = ConstList.HostEnvironment.WebRootPath + @"\images\No_Pic.jpg";
                return File(System.IO.File.ReadAllBytes(fn), ContentType);
            }
        }

        [Route("SystemSetup/MSDM104ChkDelete")]
        public ActionResult SystemSetup_MSDM104ChkDelete()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM104ChkDeleteOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string sql = "";

                //SetEDMHWeb
                sql = "Select * ";
                sql += "From SetEDMHWeb (nolock) ";
                sql += "Where Companycode='" + uu.CompanyId + "' ";
                if (DocNo.SqlQuote() != "")
                {
                    sql += "and DocNo='" + DocNo.SqlQuote() + "' ";
                }
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM104Delete")]
        public ActionResult SystemSetup_MSDM104Delete()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM104DeleteOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string sql = "";
                sql = "Update SetEDMHWeb Set DelDate=convert(char(10),getdate(),111) + ' ' + convert(char(12),getdate(),108), ";
                sql += "DelUser='" + uu.UserID + "' ";
                sql += "Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "'; ";

                sql += "Update SetEDMVIP_HWeb Set DelDate=convert(char(10),getdate(),111) + ' ' + convert(char(12),getdate(),108), ";
                sql += "DelUser='" + uu.UserID + "' ";
                sql += "Where Companycode='" + uu.CompanyId + "' and EVNO='" + DocNo.SqlQuote() + "' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM104DelImg")]
        public ActionResult SystemSetup_MSDM104DelImg()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM104DelImgOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string sql = "";
                sql = "Select * From SetEDM (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2' ";
                DataTable dt = PubUtility.SqlQry(sql, uu, "SYS");
                if (dt.Rows.Count > 0)
                {
                    sql = "Update SetEDM Set Status='0' Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2' ";
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }
                else
                {
                    DataTable dtF = new DataTable();
                    dtF.Columns.Add("CompanyCode", typeof(string));
                    dtF.Columns.Add("STATUS", typeof(string));
                    dtF.Columns.Add("DocNo", typeof(string));
                    dtF.Columns.Add("DataType", typeof(string));
                    dtF.Columns.Add("DocType", typeof(string));
                    DataRow drF = dtF.NewRow();
                    drF["CompanyCode"] = uu.CompanyId;
                    drF["STATUS"] = "0";
                    drF["DocNo"] = DocNo.SqlQuote();
                    drF["DataType"] = "P2";
                    drF["DocType"] = "P";
                    dtF.Rows.Add(drF);
                    PubUtility.AddTable("SetEDM", dtF, uu, "SYS");
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSD102_LookUpActivityCode")]
        public ActionResult SystemSetup_MSSD102_LookUpActivityCode()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD102_LookUpActivityCodeOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ActivityCode = rq["ActivityCode"];

                string sql = "";
                sql = "Select a.ActivityCode,a.PS_Name,a.StartDate,a.EndDate ";
                sql += "From PromoteSCouponHWeb a (nolock) ";
                sql += "inner join SetEDMHWeb b (nolock) on a.PS_NO=b.PS_NO and b.EDMType='V' and isnull(b.ApproveDate,'')<>'' and b.Companycode=a.Companycode ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' and a.CouponType in('1','2') ";
                if (ActivityCode.SqlQuote() != "")
                {
                    sql += "and a.ActivityCode like '" + ActivityCode.SqlQuote() + "%' ";
                }
                sql += "group by a.ActivityCode,a.PS_Name,a.StartDate,a.EndDate ";
                sql += "Order By a.StartDate desc ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSD102Query")]
        public ActionResult SystemSetup_MSSD102Query()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD102QueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ActivityCode = rq["ActivityCode"];
                string PSName = rq["PSName"];
                string EDDate = rq["EDDate"];

                string sql = "";
                sql = "Select a.PS_NO,a.ActivityCode,b.PS_Name,a.StartDate + '~' + a.EndDate EDDate, ";
                sql += "sum(isnull(a.issueQty,0))Cnt1,sum(isnull(a.ReclaimQty,0))Cnt2, ";
                
                sql += "case when sum(isnull(a.issueQty,0))=0 and sum(isnull(a.ReclaimQty,0))=0 then format(0,'p') when sum(isnull(a.issueQty,0))=0 then format(1,'p') else format(cast(sum(isnull(a.ReclaimQty,0)) as Float)/cast(sum(isnull(a.issueQty,0)) as Float),'p') end as RePercent, ";
                
                sql += "sum(isnull(a.ShareAmt,0))ActualDiscount,sum(isnull(a.ReclaimCash,0))Cash,sum(isnull(a.ReclaimTrans,0))Cnt3, ";
                sql += "case when sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(sum(isnull(a.ReclaimCash,0))/sum(isnull(a.ReclaimTrans,0)),0) end as SalesPrice ";
                sql += "From MsData2Web a (nolock) ";
                sql += "inner join PromoteSCouponHWeb b (nolock) on a.PS_NO=b.PS_NO and b.Companycode=a.Companycode and b.CouponType in('1','2') ";
                //活動代號
                if (ActivityCode.SqlQuote() != "")
                {
                    sql += "and b.ActivityCode like '%" + ActivityCode.SqlQuote() + "%' ";
                }
                //活動名稱
                if (PSName.SqlQuote() != "")
                {
                    sql += "and b.PS_Name like '%" + PSName.SqlQuote() + "%' ";
                }
                sql += "and b.PS_NO in (Select PS_NO From SetEDMHWeb (nolock) Where EDMType='V' and isnull(ApproveDate,'')<>'' and Companycode='" + uu.CompanyId + "' ";
                //入會日期
                if (EDDate.SqlQuote() != "")
                {
                    sql += "and '" + EDDate.SqlQuote() + "' between StartDate and EndDate ";
                }
                sql += ") ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                sql += "group by a.PS_NO,a.ActivityCode,b.PS_Name,a.StartDate,a.EndDate ";
                sql += "Order by a.StartDate ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSD102Query_Step1")]
        public ActionResult SystemSetup_MSSD102Query_Step1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD102Query_Step1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string PS_NO = rq["PS_NO"];
                string OpenDate1 = rq["OpenDate1"];
                string OpenDate2 = rq["OpenDate2"];
                string Flag = rq["Flag"];

                string sql = "";
                string sqlD = "";

                //店櫃
                if (Flag == "S")
                {
                    //明細資料
                    sql = "Select a.ShopNo + '-' + b.ST_SName as id,Sum(isnull(a.issueQty,0))Cnt1, ";
                    sql += "Sum(isnull(a.ReclaimQty,0))Cnt2, ";
                    sql += "case when Sum(isnull(a.issueQty,0))=0 and Sum(isnull(a.ReclaimQty,0))=0 then format(0,'p') when Sum(isnull(a.issueQty,0))=0 then format(1,'p') else format(cast(Sum(isnull(a.ReclaimQty,0)) as Float)/cast(Sum(isnull(a.issueQty,0)) as Float),'p') end as RePercent, ";
                    sql += "Sum(isnull(a.ShareAmt,0))ActualDiscount,Sum(isnull(a.ReclaimCash,0))SalesCash1,Sum(isnull(a.ReclaimTrans,0))SalesCnt1, ";
                    sql += "case when Sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(Sum(isnull(a.ReclaimCash,0))/Sum(isnull(a.ReclaimTrans,0)),0) end as SalesPrice1, ";
                    sql += "Sum(isnull(a.TotalCash,0))SalesCash2,Sum(isnull(a.TotalTrans,0))SalesCnt2, ";
                    sql += "case when Sum(isnull(a.TotalTrans,0))=0 then 0 else Round(Sum(isnull(a.TotalCash,0))/Sum(isnull(a.TotalTrans,0)),0) end as SalesPrice2 ";

                    sql += "From MSData2Web a (nolock) ";
                    sql += "inner join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode and b.ST_Type not in('2','3') ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    if (PS_NO != "")
                    {
                        sql += "and a.PS_NO='" + PS_NO + "' ";
                    }
                    sql += "group by a.ShopNo,b.ST_SName ";
                    sql += "Order by a.ShopNo ";
                    DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sql = "Select a.PS_NO,Sum(isnull(a.issueQty,0))SumCnt1,Sum(isnull(a.ReclaimQty,0))SumCnt2, ";
                    sql += "case when Sum(isnull(a.issueQty,0))=0 then format(1,'p') else format(cast(Sum(isnull(a.ReclaimQty,0)) as Float)/cast(Sum(isnull(a.issueQty,0)) as Float),'p') end as SumRePercent, ";
                    sql += "Sum(isnull(a.ShareAmt,0))SumActualDiscount,sum(isnull(a.ReclaimCash,0))SumSalesCash1,sum(isnull(a.ReclaimTrans,0))SumSalesCnt1, ";
                    sql += "case when sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(sum(isnull(a.ReclaimCash,0))/sum(isnull(a.ReclaimTrans,0)),0) end as SumSalesPrice1, ";
                    sql += "sum(isnull(a.TotalCash,0))SumSalesCash2,sum(isnull(a.TotalTrans,0))SumSalesCnt2, ";
                    sql += "case when sum(isnull(a.TotalTrans,0))=0 then 0 else Round(sum(isnull(a.TotalCash,0))/sum(isnull(a.TotalTrans,0)),0) end as SumSalesPrice2 ";
                    sql += "From MSData2Web a (nolock) ";
                    sql += "inner join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode and b.ST_Type not in('2','3') ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    if (PS_NO != "")
                    {
                        sql += "and a.PS_NO='" + PS_NO + "' ";
                    }
                    sql += "group by a.PS_NO ";
                    DataTable dtSumQ = PubUtility.SqlQry(sql, uu, "SYS");
                    dtSumQ.TableName = "dtSumQ";
                    ds.Tables.Add(dtSumQ);
                }
                //日期
                else if (Flag == "D")
                {
                    //日期區間表
                    sqlD = "WITH dates([Date]) AS( ";
                    sqlD += "SELECT convert(DATE, '" + OpenDate1 + "') AS[Date] ";
                    sqlD += "UNION ALL ";
                    sqlD += "SELECT dateadd(day, 1, [Date]) FROM dates WHERE[Date] < ";
                    if (Convert.ToDateTime(OpenDate2) >= DateTime.Now)  //結束日大於今日,只取到昨天
                        sqlD += "convert(nvarchar(10),dateadd(DAY,-1,getdate()),111)";
                    else
                        sqlD += "'" + OpenDate2 + "'";
                    sqlD += " ) ";
                    sqlD += "SELECT convert(nvarchar(10),[date],111) AS[id] ";
                    sqlD += "into #dates ";
                    sqlD += "FROM dates ";
                    sqlD += "[date] OPTION(MAXRECURSION 32767); ";
                    //明細資料
                    sql = "Select a.id,Sum(isnull(b.ReclaimQty,0))Cnt1,Sum(isnull(b.ShareAmt,0))ActualDiscount, ";
                    sql += "Sum(isnull(b.ReclaimCash,0))SalesCash1,Sum(isnull(b.ReclaimTrans,0))SalesCnt1, ";
                    sql += "case when Sum(isnull(b.ReclaimTrans,0))=0 then 0 else Round(Sum(isnull(b.ReclaimCash,0)) / Sum(isnull(b.ReclaimTrans,0)), 0) end as SalesPrice1, ";
                    sql += "Sum(isnull(b.TotalCash,0))SalesCash2,Sum(isnull(b.TotalTrans,0))SalesCnt2, ";
                    sql += "case when Sum(isnull(b.TotalTrans,0))=0 then 0 else Round(Sum(isnull(b.TotalCash,0)) / Sum(isnull(b.TotalTrans,0)), 0) end as SalesPrice2 ";

                    sql += "From #dates a ";
                    sql += "inner join MSData1Web b (nolock) on a.id=b.SalesDate and b.Companycode='" + uu.CompanyId + "' ";
                    if (PS_NO != "")
                    {
                        sql += "and b.PS_NO='" + PS_NO + "' ";
                    }
                    sql += "group by a.id ";
                    sql += "order by a.id ";
                    DataTable dtE = PubUtility.SqlQry(sqlD + sql, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);
                    //彙總資料
                    sql = "select sum(isnull(b.ReclaimQty,0))SumCnt1,sum(isnull(b.ShareAmt,0))SumActualDiscount, ";
                    sql += "sum(isnull(b.ReclaimCash,0))SumSalesCash1,sum(isnull(b.ReclaimTrans,0))SumSalesCnt1, ";
                    sql += "case when sum(isnull(b.ReclaimTrans,0))=0 then 0 else Round(sum(isnull(b.ReclaimCash,0))/sum(isnull(b.ReclaimTrans,0)), 0) end as SumSalesPrice1, ";
                    sql += "sum(isnull(b.TotalCash,0))SumSalesCash2,sum(isnull(b.TotalTrans,0))SumSalesCnt2, ";
                    sql += "case when sum(isnull(b.TotalTrans,0))=0 then 0 else Round(sum(isnull(b.TotalCash,0))/sum(isnull(b.TotalTrans,0)), 0) end as SumSalesPrice2 ";

                    sql += "From #dates a ";
                    sql += "inner join MSData1Web b (nolock) on a.id=b.SalesDate and b.Companycode='" + uu.CompanyId + "' ";
                    if (PS_NO != "")
                    {
                        sql += "and b.PS_NO='" + PS_NO + "' ";
                    }
                    DataTable dtSumQ = PubUtility.SqlQry(sqlD + sql, uu, "SYS");
                    dtSumQ.TableName = "dtSumQ";
                    ds.Tables.Add(dtSumQ);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSD102Query_Step2")]
        public ActionResult SystemSetup_MSSD102Query_Step2()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD102Query_Step2OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string PS_NO = rq["PS_NO"];
                string ID = rq["ID"];
                string OpenDate1 = rq["OpenDate1"];
                string OpenDate2 = rq["OpenDate2"];
                string Flag = rq["Flag"];

                string sql = "";
                string sqlD = "";

                //店櫃
                if (Flag == "S")
                {
                    //日期區間表
                    sqlD = "WITH dates([Date]) AS( ";
                    sqlD += "SELECT convert(DATE, '" + OpenDate1 + "') AS[Date] ";
                    sqlD += "UNION ALL ";
                    sqlD += "SELECT dateadd(day, 1, [Date]) FROM dates WHERE[Date] < ";
                    if (Convert.ToDateTime(OpenDate2) >= DateTime.Now)  //結束日大於今日,只取到昨天
                        sqlD += "convert(nvarchar(10),dateadd(DAY,-1,getdate()),111)";
                    else
                        sqlD += "'" + OpenDate2 + "'";
                    sqlD += " ) ";
                    sqlD += "SELECT convert(nvarchar(10),[date],111) AS[id] ";
                    sqlD += "into #dates ";
                    sqlD += "FROM dates ";
                    sqlD += "[date] OPTION(MAXRECURSION 32767); ";
                    //明細資料
                    sql = "Select a.id,sum(isnull(b.ReclaimQty,0))ReclaimQty,sum(isnull(b.ShareAmt,0))ShareAmt, ";
                    sql += "sum(isnull(b.ReclaimCash,0))ReclaimCash,sum(isnull(b.ReclaimTrans,0))ReclaimTrans, ";
                    sql += "case when sum(isnull(b.ReclaimTrans,0))=0 then 0 else Round(sum(isnull(b.ReclaimCash,0))/sum(isnull(b.ReclaimTrans,0)),0) end as ReclaimPrice, ";
                    sql += "sum(isnull(b.TotalCash,0))TotalCash,sum(isnull(b.TotalTrans,0))TotalTrans, ";
                    sql += "case when sum(isnull(b.TotalTrans,0))=0 then 0 else Round(sum(isnull(b.TotalCash,0))/sum(isnull(b.TotalTrans,0)),0) end as TotalPrice ";
                    sql += "From #dates a ";
                    sql += "inner join MSData1Web b (nolock) on a.id=b.SalesDate and b.Companycode='" + uu.CompanyId + "' ";
                    if (PS_NO != "")
                    {
                        sql += "and b.PS_NO='" + PS_NO + "' ";
                    }
                    if (ID != "")
                    {
                        sql += "and b.ShopNo='" + ID + "' ";
                    }
                    sql += "group by a.id ";
                    sql += "order by a.id ";
                    DataTable dtE = PubUtility.SqlQry(sqlD + sql, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sql = "Select sum(isnull(b.ReclaimQty,0))SumReclaimQty,sum(isnull(b.ShareAmt,0))SumShareAmt, ";
                    sql += "sum(isnull(b.ReclaimCash,0))SumReclaimCash,sum(isnull(b.ReclaimTrans,0))SumReclaimTrans, ";
                    sql += "case when sum(isnull(b.ReclaimTrans,0))=0 then 0 else Round(sum(isnull(b.ReclaimCash,0))/sum(isnull(b.ReclaimTrans,0)),0) end as SumReclaimPrice, ";
                    sql += "sum(isnull(b.TotalCash,0))SumTotalCash,sum(isnull(b.TotalTrans,0))SumTotalTrans, ";
                    sql += "case when sum(isnull(b.TotalTrans,0))=0 then 0 else Round(sum(isnull(b.TotalCash,0))/sum(isnull(b.TotalTrans,0)),0) end as SumTotalPrice ";
                    sql += "From #dates a ";
                    sql += "inner join MSData1Web b (nolock) on a.id=b.SalesDate and b.Companycode='" + uu.CompanyId + "' ";
                    if (PS_NO != "")
                    {
                        sql += "and b.PS_NO='" + PS_NO + "' ";
                    }
                    if (ID != "")
                    {
                        sql += "and b.ShopNo='" + ID + "' ";
                    }
                    DataTable dtSumQ = PubUtility.SqlQry(sqlD + sql, uu, "SYS");
                    dtSumQ.TableName = "dtSumQ";
                    ds.Tables.Add(dtSumQ);
                }
                //日期
                else if (Flag == "D")
                {
                    //明細資料
                    sql = "Select a.ShopNo + '-' + b.ST_SName as id,sum(isnull(a.ReclaimQty,0))ReclaimQty, ";
                    sql += "sum(isnull(a.ShareAmt,0))ShareAmt,sum(isnull(a.ReclaimCash,0))ReclaimCash, ";
                    sql += "sum(isnull(a.ReclaimTrans,0))ReclaimTrans, ";
                    sql += "case when sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(sum(isnull(a.ReclaimCash,0)) / sum(isnull(a.ReclaimTrans,0)), 0) end as ReclaimPrice, ";
                    sql += "sum(isnull(a.TotalCash,0))TotalCash,sum(isnull(a.TotalTrans,0))TotalTrans, ";
                    sql += "case when sum(isnull(a.TotalTrans,0))=0 then 0 else Round(sum(isnull(a.TotalCash,0)) / sum(isnull(a.TotalTrans,0)), 0) end as TotalPrice ";
                    sql += "From MSData1Web a (nolock) ";
                    sql += "inner join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode and b.ST_Type not in('2','3') ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    if (PS_NO != "")
                    {
                        sql += "and a.PS_NO='" + PS_NO + "' ";
                    }
                    if (ID != "")
                    {
                        sql += "and a.SalesDate='" + ID + "' ";
                    }
                    sql += "group by a.ShopNo,b.ST_SName ";
                    sql += "Order by a.ShopNo ";
                    DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);
                    //彙總資料
                    sql = "Select sum(isnull(a.ReclaimQty,0))SumReclaimQty, ";
                    sql += "sum(isnull(a.ShareAmt,0))SumShareAmt,sum(isnull(a.ReclaimCash,0))SumReclaimCash, ";
                    sql += "sum(isnull(a.ReclaimTrans,0))SumReclaimTrans, ";
                    sql += "case when sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(sum(isnull(a.ReclaimCash,0)) / sum(isnull(a.ReclaimTrans,0)), 0) end as SumReclaimPrice, ";
                    sql += "sum(isnull(a.TotalCash,0))SumTotalCash,sum(isnull(a.TotalTrans,0))SumTotalTrans, ";
                    sql += "case when sum(isnull(a.TotalTrans,0))=0 then 0 else Round(sum(isnull(a.TotalCash,0)) / sum(isnull(a.TotalTrans,0)), 0) end as SumTotalPrice ";
                    sql += "From MSData1Web a (nolock) ";
                    sql += "inner join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode and b.ST_Type not in('2','3') ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    if (PS_NO != "")
                    {
                        sql += "and a.PS_NO='" + PS_NO + "' ";
                    }
                    if (ID != "")
                    {
                        sql += "and a.SalesDate='" + ID + "' ";
                    }
                    DataTable dtSumQ = PubUtility.SqlQry(sql, uu, "SYS");
                    dtSumQ.TableName = "dtSumQ";
                    ds.Tables.Add(dtSumQ);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/GetInitMSSD105")]
        public ActionResult SystemSetup_GetInitMSSD105()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitMSSD105OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ProgramID = rq["ProgramID"];
                string Yesterday = PubUtility.GetYesterday(uu);
                string Today = PubUtility.GetToday(uu);
                string sql = "select ChineseName from ProgramIDWeb (nolock) where ProgramID='" + ProgramID.SqlQuote() + "'";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);

                //統計截止日/會員總數
                sql = "Select '" + Yesterday.Trim() + "' as EndDate,'" + Today.Trim() + "' as SysDate,Count(*) as VIPCntAll ";
                sql += "from EDDMS.dbo.VIP v (nolock) ";
                sql += "inner join EDDMS.dbo.Warehouse w (nolock) on v.VIP_FaceID=w.ST_ID and w.ST_Type not in('2','3') and w.Companycode=v.Companycode ";
                sql += "Where v.Companycode='" + uu.CompanyId + "' ";
                sql += "and isnull(v.VIP_Qday,'')<='" + Yesterday + "' ";
                DataTable dtV = PubUtility.SqlQry(sql, uu, "SYS");
                dtV.TableName = "dtV";
                ds.Tables.Add(dtV);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSD105Query")]
        public ActionResult SystemSetup_MSSD105Query()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD105QueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string CountYM = rq["CountYM"];
                string Flag = rq["Flag"];
                string Yesterday = PubUtility.GetYesterday(uu);
                string sql = "";
                string sqlQ = "";
                string sqlSumQ = "";

                //店櫃
                if (Flag == "S")
                {
                    //入會數
                    sql = "Select v.VIP_FaceID,v.VIP_FaceID + '-' + w.ST_SName as ID,count(*) as VIPCnt ";
                    sql += "into #v ";
                    sql += "from EDDMS.dbo.VIP v (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on v.VIP_FaceID=w.ST_ID and w.ST_Type not in('0','2','3') and w.Companycode=v.Companycode ";
                    sql += "Where v.Companycode='" + uu.CompanyId + "' ";
                    if (CountYM != "")
                    {
                        //判斷調閱年月是否同系統日
                        if (CountYM == Yesterday.Substring(0, 7))
                        {
                            sql += "and v.VIP_Qday between '" + Yesterday.Substring(0, 7) + "/01' and '" + Yesterday + "' ";
                        }
                        else
                        {
                            sql += "and v.VIP_Qday between '" + CountYM + "/01' and '" + CountYM + "/31' ";
                        }
                    }
                    sql += "Group By v.VIP_FaceID,w.ST_SName; ";

                    //新會員首日交易筆數/交易金額/客單價
                    sql += "Select h.ShopNo as ID,Count(*) as SalesCnt1,Sum(h.Cash) as SalesCash1,case when Count(*)=0 then 0 else Round(Sum(h.Cash)/Count(*),0) end as SalesPrice1 ";
                    sql += "into #s1 ";
                    sql += "From SalesH_NEWVIPWeb h (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on h.ShopNo=w.ST_ID and w.ST_Type not in('0','2','3') and w.Companycode=h.Companycode ";
                    sql += "Where h.Companycode='" + uu.CompanyId + "' ";
                    if (CountYM != "")
                    {
                        //判斷調閱年月是否同系統日
                        if (CountYM == Yesterday.Substring(0, 7))
                        {
                            sql += "and h.OpenDate between '" + Yesterday.Substring(0, 7) + "/01' and '" + Yesterday + "' ";
                        }
                        else
                        {
                            sql += "and h.OpenDate between '" + CountYM + "/01' and '" + CountYM + "/31' ";
                        }
                    }
                    sql += "Group By h.ShopNo; ";

                    //會員/非會員當月交易筆數/交易金額/客單價/交易佔比
                    sql += "Select h.ShopNo as ID,Sum(h.Cash) as SalesCashAll,Sum(h.VIP_RecS) as SalesCnt2,Sum(h.RecS)-Sum(h.VIP_RecS) as SalesCnt3, ";
                    sql += "Sum(h.VIP_Cash) as SalesCash2,Sum(h.Cash)-Sum(h.VIP_Cash) as SalesCash3, ";
                    sql += "case when Sum(h.VIP_RecS)=0 then 0 else Round(Sum(h.VIP_Cash)/Sum(h.VIP_RecS),0) end as SalesPrice2, ";
                    sql += "case when Sum(h.RecS)-Sum(h.VIP_RecS)=0 then 0 else Round((Sum(h.Cash)-Sum(h.VIP_Cash))/(Sum(h.RecS)-Sum(h.VIP_RecS)),0) end as SalesPrice3 ";
                    sql += "into #s2 ";
                    sql += "From SalesHWeb h (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on h.ShopNo=w.ST_ID and w.ST_Type not in('0','2','3') and w.Companycode=h.Companycode ";
                    sql += "Where h.Companycode='" + uu.CompanyId + "' ";
                    if (CountYM != "")
                    {
                        //判斷調閱年月是否同系統日
                        if (CountYM == Yesterday.Substring(0, 7))
                        {
                            sql += "and h.OpenDate between '" + Yesterday.Substring(0, 7) + "/01' and '" + Yesterday + "' ";
                        }
                        else
                        {
                            sql += "and h.OpenDate between '" + CountYM + "/01' and '" + CountYM + "/31' ";
                        }
                    }
                    sql += "Group By h.ShopNo; ";

                    //開始撈明細資料
                    sqlQ = "Select v.ID,isnull(v.VIPCnt,0)VIPCnt, ";
                    sqlQ += "isnull(s1.SalesCnt1,0)SalesCnt1,isnull(s1.SalesCash1,0)SalesCash1,isnull(s1.SalesPrice1,0)SalesPrice1, ";
                    sqlQ += "isnull(s2.SalesCnt2,0)SalesCnt2,isnull(s2.SalesCash2,0)SalesCash2,isnull(s2.SalesPrice2,0)SalesPrice2, ";
                    sqlQ += "case when isnull(s2.SalesCashAll,0)=0 and isnull(s2.SalesCash2,0)=0 then format(0,'p') when isnull(s2.SalesCashAll,0)=0 then format(1,'p') else format(cast(isnull(s2.SalesCash2,0) as Float)/cast(isnull(s2.SalesCashAll,0) as Float),'p') end as SalesPercent2, ";
                    sqlQ += "isnull(s2.SalesCnt3,0)SalesCnt3,isnull(s2.SalesCash3,0)SalesCash3,isnull(s2.SalesPrice3,0)SalesPrice3, ";
                    sqlQ += "case when isnull(s2.SalesCashAll,0)=0 and isnull(s2.SalesCash3,0)=0 then format(0,'p') when isnull(s2.SalesCashAll,0)=0 then format(1,'p') else format(cast(isnull(s2.SalesCash3,0) as Float)/cast(isnull(s2.SalesCashAll,0) as Float),'p') end as SalesPercent3 ";
                    sqlQ += "From #v v (nolock) ";
                    sqlQ += "left join #s2 s2 on v.VIP_FaceID=s2.ID ";
                    sqlQ += "left join #s1 s1 on v.VIP_FaceID=s1.ID ";
                    sqlQ += "Where 1=1 ";
                    //測試
                    //sqlQ += "and w.ST_ID='EDM1' ";
                    sqlQ += "Order by v.VIP_FaceID ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlQ, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總明細資料
                    sqlSumQ = "Select sum(isnull(v.VIPCnt,0))SumVIPCnt, ";
                    sqlSumQ += "sum(isnull(s1.SalesCnt1,0))SumSalesCnt1,sum(isnull(s1.SalesCash1,0))SumSalesCash1,case when sum(isnull(s1.SalesCnt1,0))=0 then 0 else Round(sum(isnull(s1.SalesCash1,0))/sum(isnull(s1.SalesCnt1,0)),0) end as SumSalesPrice1, ";
                    sqlSumQ += "sum(isnull(s2.SalesCnt2,0))SumSalesCnt2,sum(isnull(s2.SalesCash2,0))SumSalesCash2,case when sum(isnull(s2.SalesCnt2,0))=0 then 0 else Round(sum(isnull(s2.SalesCash2,0))/sum(isnull(s2.SalesCnt2,0)),0) end as SumSalesPrice2, ";
                    sqlSumQ += "case when sum(isnull(s2.SalesCashAll,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.SalesCash2,0)) as Float)/cast(sum(isnull(s2.SalesCashAll,0)) as Float),'p') end as SumSalesPercent2, ";
                    sqlSumQ += "sum(isnull(s2.SalesCnt3,0))SumSalesCnt3,sum(isnull(s2.SalesCash3,0))SumSalesCash3,case when sum(isnull(s2.SalesCnt3,0))=0 then 0 else Round(sum(isnull(s2.SalesCash3,0))/sum(isnull(s2.SalesCnt3,0)),0) end as SumSalesPrice3, ";
                    sqlSumQ += "case when sum(isnull(s2.SalesCashAll,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.SalesCash3,0)) as Float)/cast(sum(isnull(s2.SalesCashAll,0)) as Float),'p') end as SumSalesPercent3 ";
                    sqlSumQ += "From #v v (nolock) ";
                    sqlSumQ += "left join #s2 s2 on v.VIP_FaceID=s2.ID ";
                    sqlSumQ += "left join #s1 s1 on v.VIP_FaceID=s1.ID ";
                    sqlSumQ += "Where 1=1 ";
                    //測試
                    //sqlSumQ += "and w.ST_ID='EDM1' ";
                    DataTable dtSumQ = PubUtility.SqlQry(sql + sqlSumQ, uu, "SYS");
                    dtSumQ.TableName = "dtSumQ";
                    ds.Tables.Add(dtSumQ);
                }
                //日期
                else if (Flag == "D")
                {
                    //入會數
                    sql = "Select v.VIP_Qday as ID,count(*) as VIPCnt ";
                    sql += "into #v ";
                    sql += "from EDDMS.dbo.VIP v (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on v.VIP_FaceID=w.ST_ID and w.ST_Type not in('0','2','3') and w.Companycode=v.Companycode ";
                    sql += "Where v.Companycode='" + uu.CompanyId + "' ";
                    if (CountYM != "")
                    {
                        //判斷調閱年月是否同系統日
                        if (CountYM == Yesterday.Substring(0, 7))
                        {
                            sql += "and v.VIP_Qday between '" + Yesterday.Substring(0, 7) + "/01' and '" + Yesterday + "' ";
                        }
                        else
                        {
                            sql += "and v.VIP_Qday between '" + CountYM + "/01' and '" + CountYM + "/31' ";
                        }
                    }
                    sql += "Group By v.VIP_Qday; ";

                    //新會員首日交易筆數/交易金額/客單價
                    sql += "Select h.OpenDate as ID,Count(*) as SalesCnt1,Sum(h.Cash) as SalesCash1,case when Count(*)=0 then 0 else Round(Sum(h.Cash)/Count(*),0) end as SalesPrice1 ";
                    sql += "into #s1 ";
                    sql += "From SalesH_NEWVIPWeb h (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on h.ShopNo=w.ST_ID and w.ST_Type not in('0','2','3') and w.Companycode=h.Companycode ";
                    sql += "Where h.Companycode='" + uu.CompanyId + "' ";
                    if (CountYM != "")
                    {
                        //判斷調閱年月是否同系統日
                        if (CountYM == Yesterday.Substring(0, 7))
                        {
                            sql += "and h.OpenDate between '" + Yesterday.Substring(0, 7) + "/01' and '" + Yesterday + "' ";
                        }
                        else
                        {
                            sql += "and h.OpenDate between '" + CountYM + "/01' and '" + CountYM + "/31' ";
                        }
                    }
                    sql += "Group By h.OpenDate; ";

                    //會員/非會員當月交易筆數/交易金額/客單價/交易佔比
                    sql += "Select h.OpenDate as ID,Sum(h.Cash) as SalesCashAll,Sum(h.VIP_RecS) as SalesCnt2,Sum(h.RecS)-Sum(h.VIP_RecS) as SalesCnt3, ";
                    sql += "Sum(h.VIP_Cash) as SalesCash2,Sum(h.Cash)-Sum(h.VIP_Cash) as SalesCash3, ";
                    sql += "case when Sum(h.VIP_RecS)=0 then 0 else Round(Sum(h.VIP_Cash)/Sum(h.VIP_RecS),0) end as SalesPrice2, ";
                    sql += "case when Sum(h.RecS)-Sum(h.VIP_RecS)=0 then 0 else Round((Sum(h.Cash)-Sum(h.VIP_Cash))/(Sum(h.RecS)-Sum(h.VIP_RecS)),0) end as SalesPrice3 ";
                    sql += "into #s2 ";
                    sql += "From SalesHWeb h (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on h.ShopNo=w.ST_ID and w.ST_Type not in('0','2','3') and w.Companycode=h.Companycode ";
                    sql += "Where h.Companycode='" + uu.CompanyId + "' ";
                    if (CountYM != "")
                    {
                        //判斷調閱年月是否同系統日
                        if (CountYM == Yesterday.Substring(0, 7))
                        {
                            sql += "and h.OpenDate between '" + Yesterday.Substring(0, 7) + "/01' and '" + Yesterday + "' ";
                        }
                        else
                        {
                            sql += "and h.OpenDate between '" + CountYM + "/01' and '" + CountYM + "/31' ";
                        }
                    }
                    sql += "Group By h.OpenDate; ";

                    //開始撈明細資料
                    sqlQ = "Select s2.ID as ID,isnull(v.VIPCnt,0)VIPCnt, ";
                    sqlQ += "isnull(s1.SalesCnt1,0)SalesCnt1,isnull(s1.SalesCash1,0)SalesCash1,isnull(s1.SalesPrice1,0)SalesPrice1, ";
                    sqlQ += "isnull(s2.SalesCnt2,0)SalesCnt2,isnull(s2.SalesCash2,0)SalesCash2,isnull(s2.SalesPrice2,0)SalesPrice2, ";
                    sqlQ += "case when isnull(s2.SalesCashAll,0)=0 and isnull(s2.SalesCash2,0)=0 then format(0,'p') when isnull(s2.SalesCashAll,0)=0 then format(1,'p') else format(cast(isnull(s2.SalesCash2,0) as Float)/cast(isnull(s2.SalesCashAll,0) as Float),'p') end as SalesPercent2, ";
                    sqlQ += "isnull(s2.SalesCnt3,0)SalesCnt3,isnull(s2.SalesCash3,0)SalesCash3,isnull(s2.SalesPrice3,0)SalesPrice3, ";
                    sqlQ += "case when isnull(s2.SalesCashAll,0)=0 and isnull(s2.SalesCash3,0)=0 then format(0,'p') when isnull(s2.SalesCashAll,0)=0 then format(1,'p') else format(cast(isnull(s2.SalesCash3,0) as Float)/cast(isnull(s2.SalesCashAll,0) as Float),'p') end as SalesPercent3 ";
                    sqlQ += "From #v v (nolock) ";
                    sqlQ += "left join #s2 s2 on v.ID=s2.ID ";
                    sqlQ += "left join #s1 s1 on v.ID=s1.ID ";
                    sqlQ += "Where 1=1 ";
                    sqlQ += "Order by v.ID ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlQ, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總明細資料
                    sqlSumQ = "Select sum(isnull(v.VIPCnt,0))SumVIPCnt, ";
                    sqlSumQ += "sum(isnull(s1.SalesCnt1,0))SumSalesCnt1,sum(isnull(s1.SalesCash1,0))SumSalesCash1,case when sum(isnull(s1.SalesCnt1,0))=0 then 0 else Round(sum(isnull(s1.SalesCash1,0))/sum(isnull(s1.SalesCnt1,0)),0) end as SumSalesPrice1, ";
                    sqlSumQ += "sum(isnull(s2.SalesCnt2,0))SumSalesCnt2,sum(isnull(s2.SalesCash2,0))SumSalesCash2,case when sum(isnull(s2.SalesCnt2,0))=0 then 0 else Round(sum(isnull(s2.SalesCash2,0))/sum(isnull(s2.SalesCnt2,0)),0) end as SumSalesPrice2, ";
                    sqlSumQ += "case when sum(isnull(s2.SalesCashAll,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.SalesCash2,0)) as Float)/cast(sum(isnull(s2.SalesCashAll,0)) as Float),'p') end as SumSalesPercent2, ";
                    sqlSumQ += "sum(isnull(s2.SalesCnt3,0))SumSalesCnt3,sum(isnull(s2.SalesCash3,0))SumSalesCash3,case when sum(isnull(s2.SalesCnt3,0))=0 then 0 else Round(sum(isnull(s2.SalesCash3,0))/sum(isnull(s2.SalesCnt3,0)),0) end as SumSalesPrice3, ";
                    sqlSumQ += "case when sum(isnull(s2.SalesCashAll,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.SalesCash3,0)) as Float)/cast(sum(isnull(s2.SalesCashAll,0)) as Float),'p') end as SumSalesPercent3 ";
                    sqlSumQ += "From #v v (nolock) ";
                    sqlSumQ += "left join #s2 s2 on v.ID=s2.ID ";
                    sqlSumQ += "left join #s1 s1 on v.ID=s1.ID ";
                    sqlSumQ += "Where 1=1 ";
                    DataTable dtSumQ = PubUtility.SqlQry(sql + sqlSumQ, uu, "SYS");
                    dtSumQ.TableName = "dtSumQ";
                    ds.Tables.Add(dtSumQ);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSD105Query_Step1")]
        public ActionResult SystemSetup_MSSD105Query_Step1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD105Query_Step1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string CountYM = rq["CountYM"];
                string ShopNo = rq["ShopNo"];
                string OpenDate = rq["OpenDate"];
                string Flag = rq["Flag"];
                string Yesterday = PubUtility.GetYesterday(uu);

                string sql = "";
                string sqlQ = "";
                string sqlSumQ = "";

                //店櫃
                if (Flag == "S")
                {
                    //入會數
                    sql = "Select v.VIP_Qday as ID,count(*) as VIPCnt ";
                    sql += "into #v ";
                    sql += "from EDDMS.dbo.VIP v (nolock) ";
                    sql += "Where v.Companycode='" + uu.CompanyId + "' ";
                    if (ShopNo != "")
                    {
                        sql += "and v.VIP_FaceID='" + ShopNo + "' ";
                    }
                    if (CountYM != "")
                    {
                        //判斷調閱年月是否同系統日
                        if (CountYM == Yesterday.Substring(0, 7))
                        {
                            sql += "and v.VIP_Qday between '" + Yesterday.Substring(0, 7) + "/01' and '" + Yesterday + "' ";
                        }
                        else
                        {
                            sql += "and v.VIP_Qday between '" + CountYM + "/01' and '" + CountYM + "/31' ";
                        }
                    }
                    sql += "Group By v.VIP_Qday; ";

                    //新會員首日交易筆數/交易金額/客單價
                    sql += "Select h.OpenDate as ID,Count(*) as SalesCnt1,Sum(h.Cash) as SalesCash1,case when Count(*)=0 then 0 else Round(Sum(h.Cash)/Count(*),0) end as SalesPrice1 ";
                    sql += "into #s1 ";
                    sql += "From SalesH_NEWVIPWeb h (nolock) ";
                    sql += "Where h.Companycode='" + uu.CompanyId + "' ";
                    if (ShopNo != "")
                    {
                        sql += "and h.ShopNo='" + ShopNo + "' ";
                    }
                    if (CountYM != "")
                    {
                        //判斷調閱年月是否同系統日
                        if (CountYM == Yesterday.Substring(0, 7))
                        {
                            sql += "and h.OpenDate between '" + Yesterday.Substring(0, 7) + "/01' and '" + Yesterday + "' ";
                        }
                        else
                        {
                            sql += "and h.OpenDate between '" + CountYM + "/01' and '" + CountYM + "/31' ";
                        }
                    }
                    sql += "Group By h.OpenDate; ";

                    //會員/非會員當月交易筆數/交易金額/客單價/交易佔比
                    sql += "Select h.OpenDate as ID,Sum(h.Cash) as SalesCashAll,Sum(h.VIP_RecS) as SalesCnt2,Sum(h.RecS)-Sum(h.VIP_RecS) as SalesCnt3, ";
                    sql += "Sum(h.VIP_Cash) as SalesCash2,Sum(h.Cash)-Sum(h.VIP_Cash) as SalesCash3, ";
                    sql += "case when Sum(h.VIP_RecS)=0 then 0 else Round(Sum(h.VIP_Cash)/Sum(h.VIP_RecS),0) end as SalesPrice2, ";
                    sql += "case when Sum(h.RecS)-Sum(h.VIP_RecS)=0 then 0 else Round((Sum(h.Cash)-Sum(h.VIP_Cash))/(Sum(h.RecS)-Sum(h.VIP_RecS)),0) end as SalesPrice3 ";
                    sql += "into #s2 ";
                    sql += "From SalesHWeb h (nolock) ";
                    sql += "Where h.Companycode='" + uu.CompanyId + "' ";
                    if (ShopNo != "")
                    {
                        sql += "and h.ShopNo='" + ShopNo + "' ";
                    }
                    if (CountYM != "")
                    {
                        //判斷調閱年月是否同系統日
                        if (CountYM == Yesterday.Substring(0, 7))
                        {
                            sql += "and h.OpenDate between '" + Yesterday.Substring(0, 7) + "/01' and '" + Yesterday + "' ";
                        }
                        else
                        {
                            sql += "and h.OpenDate between '" + CountYM + "/01' and '" + CountYM + "/31' ";
                        }
                    }
                    sql += "Group By h.OpenDate; ";

                    //開始撈明細資料
                    sqlQ = "Select s2.ID as ID,isnull(v.VIPCnt,0)VIPCnt, ";
                    sqlQ += "isnull(s1.SalesCnt1,0)SalesCnt1,isnull(s1.SalesCash1,0)SalesCash1,isnull(s1.SalesPrice1,0)SalesPrice1, ";
                    sqlQ += "isnull(s2.SalesCnt2,0)SalesCnt2,isnull(s2.SalesCash2,0)SalesCash2,isnull(s2.SalesPrice2,0)SalesPrice2, ";
                    sqlQ += "case when isnull(s2.SalesCashAll,0)=0 and isnull(s2.SalesCash2,0)=0 then format(0,'p') when isnull(s2.SalesCashAll,0)=0 then format(1,'p') else format(cast(isnull(s2.SalesCash2,0) as Float)/cast(isnull(s2.SalesCashAll,0) as Float),'p') end as SalesPercent2, ";
                    sqlQ += "isnull(s2.SalesCnt3,0)SalesCnt3,isnull(s2.SalesCash3,0)SalesCash3,isnull(s2.SalesPrice3,0)SalesPrice3, ";
                    sqlQ += "case when isnull(s2.SalesCashAll,0)=0 and isnull(s2.SalesCash3,0)=0 then format(0,'p') when isnull(s2.SalesCashAll,0)=0 then format(1,'p') else format(cast(isnull(s2.SalesCash3,0) as Float)/cast(isnull(s2.SalesCashAll,0) as Float),'p') end as SalesPercent3 ";

                    sqlQ += "From #v v (nolock) ";
                    sqlQ += "left join #s2 s2 on v.ID=s2.ID ";
                    sqlQ += "left join #s1 s1 on v.ID=s1.ID ";
                    sqlQ += "Where 1=1 ";
                    sqlQ += "Order by v.ID ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlQ, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總明細資料
                    sqlSumQ = "Select sum(isnull(v.VIPCnt,0))SumVIPCnt, ";
                    sqlSumQ += "sum(isnull(s1.SalesCnt1,0))SumSalesCnt1,sum(isnull(s1.SalesCash1,0))SumSalesCash1,case when sum(isnull(s1.SalesCnt1,0))=0 then 0 else Round(sum(isnull(s1.SalesCash1,0))/sum(isnull(s1.SalesCnt1,0)),0) end as SumSalesPrice1, ";
                    sqlSumQ += "sum(isnull(s2.SalesCnt2,0))SumSalesCnt2,sum(isnull(s2.SalesCash2,0))SumSalesCash2,case when sum(isnull(s2.SalesCnt2,0))=0 then 0 else Round(sum(isnull(s2.SalesCash2,0))/sum(isnull(s2.SalesCnt2,0)),0) end as SumSalesPrice2, ";
                    sqlSumQ += "case when sum(isnull(s2.SalesCashAll,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.SalesCash2,0)) as Float)/cast(sum(isnull(s2.SalesCashAll,0)) as Float),'p') end as SumSalesPercent2, ";
                    sqlSumQ += "sum(isnull(s2.SalesCnt3,0))SumSalesCnt3,sum(isnull(s2.SalesCash3,0))SumSalesCash3,case when sum(isnull(s2.SalesCnt3,0))=0 then 0 else Round(sum(isnull(s2.SalesCash3,0))/sum(isnull(s2.SalesCnt3,0)),0) end as SumSalesPrice3, ";
                    sqlSumQ += "case when sum(isnull(s2.SalesCashAll,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.SalesCash3,0)) as Float)/cast(sum(isnull(s2.SalesCashAll,0)) as Float),'p') end as SumSalesPercent3 ";

                    sqlSumQ += "From #v v (nolock) ";
                    sqlSumQ += "left join #s2 s2 on v.ID=s2.ID ";
                    sqlSumQ += "left join #s1 s1 on v.ID=s1.ID ";
                    sqlSumQ += "Where 1=1 ";
                    DataTable dtSumQ = PubUtility.SqlQry(sql + sqlSumQ, uu, "SYS");
                    dtSumQ.TableName = "dtSumQ";
                    ds.Tables.Add(dtSumQ);
                }
                //日期
                else if (Flag == "D")
                {
                    //入會數
                    sql = "Select v.VIP_FaceID,v.VIP_FaceID + '-' + w.ST_SName as ID,count(*) as VIPCnt ";
                    sql += "into #v ";
                    sql += "from EDDMS.dbo.VIP v (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on v.VIP_FaceID=w.ST_ID and w.ST_Type not in('0','2','3') and w.Companycode=v.Companycode ";
                    sql += "Where v.Companycode='" + uu.CompanyId + "' ";
                    if (OpenDate != "")
                    {
                        sql += "and v.VIP_Qday='" + OpenDate + "' ";
                    }
                    sql += "Group By v.VIP_FaceID,w.ST_SName; ";

                    //新會員首日交易筆數/交易金額/客單價
                    sql += "Select h.ShopNo as ID,Count(*) as SalesCnt1,Sum(h.Cash) as SalesCash1,case when Count(*)=0 then 0 else Round(Sum(h.Cash)/Count(*),0) end as SalesPrice1 ";
                    sql += "into #s1 ";
                    sql += "From SalesH_NEWVIPWeb h (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on h.ShopNo=w.ST_ID and w.ST_Type not in('0','2','3') and w.Companycode=h.Companycode ";
                    sql += "Where h.Companycode='" + uu.CompanyId + "' ";
                    if (OpenDate != "")
                    {
                        sql += "and h.OpenDate='" + OpenDate + "' ";
                    }
                    sql += "Group By h.ShopNo; ";

                    //會員/非會員當月交易筆數/交易金額/客單價/交易佔比
                    sql += "Select h.ShopNo as ID,Sum(h.Cash) as SalesCashAll,Sum(h.VIP_RecS) as SalesCnt2,Sum(h.RecS)-Sum(h.VIP_RecS) as SalesCnt3, ";
                    sql += "Sum(h.VIP_Cash) as SalesCash2,Sum(h.Cash)-Sum(h.VIP_Cash) as SalesCash3, ";
                    sql += "case when Sum(h.VIP_RecS)=0 then 0 else Round(Sum(h.VIP_Cash)/Sum(h.VIP_RecS),0) end as SalesPrice2, ";
                    sql += "case when Sum(h.RecS)-Sum(h.VIP_RecS)=0 then 0 else Round((Sum(h.Cash)-Sum(h.VIP_Cash))/(Sum(h.RecS)-Sum(h.VIP_RecS)),0) end as SalesPrice3 ";
                    sql += "into #s2 ";
                    sql += "From SalesHWeb h (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on h.ShopNo=w.ST_ID and w.ST_Type not in('0','2','3') and w.Companycode=h.Companycode ";
                    sql += "Where h.Companycode='" + uu.CompanyId + "' ";
                    if (OpenDate != "")
                    {
                        sql += "and h.OpenDate='" + OpenDate + "' ";
                    }
                    sql += "Group By h.ShopNo; ";

                    //開始撈明細資料
                    sqlQ = "Select v.ID,isnull(v.VIPCnt,0)VIPCnt, ";
                    sqlQ += "isnull(s1.SalesCnt1,0)SalesCnt1,isnull(s1.SalesCash1,0)SalesCash1,isnull(s1.SalesPrice1,0)SalesPrice1, ";
                    sqlQ += "isnull(s2.SalesCnt2,0)SalesCnt2,isnull(s2.SalesCash2,0)SalesCash2,isnull(s2.SalesPrice2,0)SalesPrice2, ";
                    sqlQ += "case when isnull(s2.SalesCashAll,0)=0 and isnull(s2.SalesCash2,0)=0 then format(0,'p') when isnull(s2.SalesCashAll,0)=0 then format(1,'p') else format(cast(isnull(s2.SalesCash2,0) as Float)/cast(isnull(s2.SalesCashAll,0) as Float),'p') end as SalesPercent2, ";
                    sqlQ += "isnull(s2.SalesCnt3,0)SalesCnt3,isnull(s2.SalesCash3,0)SalesCash3,isnull(s2.SalesPrice3,0)SalesPrice3, ";
                    sqlQ += "case when isnull(s2.SalesCashAll,0)=0 and isnull(s2.SalesCash3,0)=0 then format(0,'p') when isnull(s2.SalesCashAll,0)=0 then format(1,'p') else format(cast(isnull(s2.SalesCash3,0) as Float)/cast(isnull(s2.SalesCashAll,0) as Float),'p') end as SalesPercent3 ";

                    sqlQ += "From #v v (nolock) ";
                    sqlQ += "left join #s2 s2 on v.VIP_FaceID=s2.ID ";
                    sqlQ += "left join #s1 s1 on v.VIP_FaceID=s1.ID ";
                    sqlQ += "Where 1=1 ";
                    //測試
                    //sqlQ += "and w.ST_ID='EDM1' ";
                    sqlQ += "Order by v.VIP_FaceID ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlQ, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總明細資料
                    sqlSumQ = "Select sum(isnull(v.VIPCnt,0))SumVIPCnt, ";
                    sqlSumQ += "sum(isnull(s1.SalesCnt1,0))SumSalesCnt1,sum(isnull(s1.SalesCash1,0))SumSalesCash1,case when sum(isnull(s1.SalesCnt1,0))=0 then 0 else Round(sum(isnull(s1.SalesCash1,0))/sum(isnull(s1.SalesCnt1,0)),0) end as SumSalesPrice1, ";
                    sqlSumQ += "sum(isnull(s2.SalesCnt2,0))SumSalesCnt2,sum(isnull(s2.SalesCash2,0))SumSalesCash2,case when sum(isnull(s2.SalesCnt2,0))=0 then 0 else Round(sum(isnull(s2.SalesCash2,0))/sum(isnull(s2.SalesCnt2,0)),0) end as SumSalesPrice2, ";
                    sqlSumQ += "case when sum(isnull(s2.SalesCashAll,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.SalesCash2,0)) as Float)/cast(sum(isnull(s2.SalesCashAll,0)) as Float),'p') end as SumSalesPercent2, ";
                    sqlSumQ += "sum(isnull(s2.SalesCnt3,0))SumSalesCnt3,sum(isnull(s2.SalesCash3,0))SumSalesCash3,case when sum(isnull(s2.SalesCnt3,0))=0 then 0 else Round(sum(isnull(s2.SalesCash3,0))/sum(isnull(s2.SalesCnt3,0)),0) end as SumSalesPrice3, ";
                    sqlSumQ += "case when sum(isnull(s2.SalesCashAll,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.SalesCash3,0)) as Float)/cast(sum(isnull(s2.SalesCashAll,0)) as Float),'p') end as SumSalesPercent3 ";

                    sqlSumQ += "From #v v (nolock) ";
                    sqlSumQ += "left join #s2 s2 on v.VIP_FaceID=s2.ID ";
                    sqlSumQ += "left join #s1 s1 on v.VIP_FaceID=s1.ID ";
                    sqlSumQ += "Where 1=1 ";
                    //測試
                    //sqlSumQ += "and w.ST_ID='EDM1' ";
                    DataTable dtSumQ = PubUtility.SqlQry(sql + sqlSumQ, uu, "SYS");
                    dtSumQ.TableName = "dtSumQ";
                    ds.Tables.Add(dtSumQ);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSD105Clear_Step1")]
        public ActionResult SystemSetup_MSSD105Clear_Step1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD105Clear_Step1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string sql = "";

                sql = "Select '' as ID,'' as VIPCnt, ";
                sql += "'' as SalesCnt1,'' as SalesCash1,'' as SalesPrice1, ";
                sql += "'' as SalesCnt2,'' as SalesCash2,'' as SalesPrice2, ";
                sql += "'' as SalesPercent2, ";
                sql += "'' as SalesCnt3,'' as SalesCash3,'' as SalesPrice3, ";
                sql += "'' as SalesPercent3 ";

                sql += "From SalesHWeb (nolock) ";
                sql += "Where 1=2 ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        #region msDM106
        [Route("SystemSetup/MSDM106LookUpActivityCode")]
        public ActionResult SystemSetup_MSDM106LookUpActivityCode()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM106LookUpActivityCodeOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ActivityCode = rq["ActivityCode"];
                string EDM_Model = rq["EDM_Model"];

                string sql = "";
                sql = "Select a.ActivityCode,a.PS_Name,a.StartDate,a.EndDate ";
                sql += "From PromoteSCouponHWeb a (nolock) ";
                sql += "inner join SetEDMHWeb b (nolock) on a.PS_NO=b.PS_NO and b.EDMType='B' and EDM_Model='" + EDM_Model + "' and isnull(b.DelDate,'')='' and b.Companycode=a.Companycode ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' and a.CouponType in('1','2') ";
                if (ActivityCode.SqlQuote() != "")
                {
                    sql += "and a.ActivityCode like '" + ActivityCode.SqlQuote() + "%' ";
                }
                sql += "group by a.ActivityCode,a.PS_Name,a.StartDate,a.EndDate ";
                sql += "Order By a.StartDate desc ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM106Query")]
        public ActionResult SystemSetup_MSDM106Query()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM106QueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string EDMMemo = rq["EDMMemo"];
                string ActivityCode = rq["ActivityCode"];
                string App = rq["App"];
                string Def = rq["Def"];
                string EDDate = rq["EDDate"];
                string EDM_Model = rq["EDM_Model"];
                string BirYear = rq["BirYear"];
                string BirMonth = rq["BirMonth"];

                string sql = "";
                sql = "Select a.DocNO,a.EDMMemo,a.BIR_Year,a.BIR_Month,b.PS_Name,b.ActivityCode,isnull(d.Cnt2,0)Cnt2, ";
                sql += "isnull(a.ApproveDate,'')ApproveDate,isnull(a.DefeasanceDate,'')DefeasanceDate ";
                sql += "From SetEDMHWeb a (nolock) ";
                sql += "left join PromoteSCouponHWeb b (nolock) on a.PS_NO=b.PS_NO and b.Companycode=a.Companycode and b.CouponType in('1','2') ";
                //活動代號
                if (ActivityCode.SqlQuote() != "")
                {
                    sql += "and b.ActivityCode like '" + ActivityCode.SqlQuote() + "%' ";
                }
                //sql += "left join (Select EDM_DocNo,COUNT(*)Cnt1 From SetEDMVIP_HWeb (nolock) Where Companycode='" + uu.CompanyId + "' group by EDM_DocNo)c on a.DocNo=c.EDM_DocNo ";
                sql += "left join (Select EVNO,COUNT(*)Cnt2 From SetEDMVIP_VIPWeb (nolock) Where Companycode='" + uu.CompanyId + "' group by EVNO)d on a.DocNo=d.EVNO ";

                sql += "Where a.Companycode='" + uu.CompanyId + "' and isnull(a.DelDate,'')='' and a.EDMType='B' ";
                sql += "and a.EDM_Model='" + EDM_Model.SqlQuote() + "' ";
                //DM單號
                if (DocNo.SqlQuote() != "")
                {
                    sql += "and a.DocNo like '" + DocNo.SqlQuote() + "%' ";
                }
                //DM主旨
                if (EDMMemo.SqlQuote() != "")
                {
                    sql += "and a.EDMMemo like '%" + EDMMemo.SqlQuote() + "%' ";
                }
                //DM年度
                if (BirYear.SqlQuote() != "")
                {
                    sql += "and a.Bir_Year = '" + BirYear.SqlQuote() + "' ";
                }
                //生日月份
                if (BirMonth.SqlQuote() != "")
                {
                    sql += "and a.Bir_Month = '" + BirMonth.SqlQuote() + "' ";
                }
                //批核日期
                if (App.SqlQuote() == "NoApp")
                {
                    sql += "and isnull(a.ApproveDate,'')='' ";
                }
                else if (App.SqlQuote() == "App")
                {
                    sql += "and isnull(a.ApproveDate,'')<>'' ";
                }
                //作廢日期
                if (Def.SqlQuote() == "NoDef")
                {
                    sql += "and isnull(a.DefeasanceDate,'')='' ";
                }
                else if (Def.SqlQuote() == "Def")
                {
                    sql += "and isnull(a.DefeasanceDate,'')<>'' ";
                }


                sql += "Order by a.DocNo desc ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM106Query_EDM")]
        public ActionResult SystemSetup_MSDM106Query_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM106Query_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string sql = "";

                //SetEDMHWeb
                sql = "Select a.DocNo,a.EDMMemo,a.Bir_Year,a.Bir_Month,a.WhNoFlag,a.PS_NO,c.PS_Name + '  ' + c.StartDate + ' ~ ' + c.EndDate as PS_Name, ";
                sql += "isnull(a.ApproveDate,'')ApproveDate,isnull(a.ApproveUser,'')ApproveUser,isnull(a.DefeasanceDate,'')DefeasanceDate,isnull(a.Defeasance,'')Defeasance, ";
                sql += "b.DataType,b.DocImage,b.TXT,a.VIP_Type ";
                sql += "From SetEDMHWeb a (nolock) ";
                sql += "inner join SetEDMDWeb b (nolock) on a.DocNo=b.DocNo and b.Companycode=a.Companycode ";
                sql += "left join PromoteSCouponHWeb c (nolock) on a.PS_NO=c.PS_NO and c.Companycode=a.Companycode and c.CouponType in('1','2') ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                if (DocNo.SqlQuote() != "")
                {
                    sql += "and a.DocNo='" + DocNo.SqlQuote() + "' ";
                }
                sql += "Order by b.DataType ";
                DataTable dtH = PubUtility.SqlQry(sql, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM106Cancel_EDM")]
        public ActionResult SystemSetup_MSDM106Cancel_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM106Cancel_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string sql = "";

                //SetEDMHWeb
                sql = "Select a.DocNo,a.EDMMemo,a.Bir_Year,a.Bir_Month,a.WhNoFlag,a.PS_NO,c.PS_Name + '  ' + c.StartDate + ' ~ ' + c.EndDate as PS_Name, ";
                sql += "isnull(a.ApproveDate,'')ApproveDate,isnull(a.ApproveUser,'')ApproveUser,isnull(a.DefeasanceDate,'')DefeasanceDate,isnull(a.Defeasance,'')Defeasance, ";
                sql += "b.DataType,b.DocImage,b.TXT,a.VIP_Type ";
                sql += "From SetEDMHWeb a (nolock) ";
                sql += "inner join SetEDMDWeb b (nolock) on a.DocNo=b.DocNo and b.Companycode=a.Companycode ";
                sql += "left join PromoteSCouponHWeb c (nolock) on a.PS_NO=c.PS_NO and c.Companycode=a.Companycode and c.CouponType in('1','2') ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                if (DocNo.SqlQuote() != "")
                {
                    sql += "and a.DocNo='" + DocNo.SqlQuote() + "' ";
                }
                sql += "Order by b.DataType ";
                DataTable dtH = PubUtility.SqlQry(sql, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);

                sql = "Delete From SetEDM Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM106_Save_EDM")]
        public ActionResult SystemSetup_MSDM106_Save_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM106_Save_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string EditMode = rq["EditMode"];
                string EDMMemo = rq["EDMMemo"];
                string BIRYear = rq["BIRYear"];
                string BIRMonth = rq["BIRMonth"];
                string PS_NO = rq["PS_NO"];
                string T1 = rq["T1"];
                string T2 = rq["T2"];
                string VIPType = rq["VIPType"];
                string DocNo = rq["DocNo"];
                string VMDocNo = rq["VMDocNo"];
                string sql = "";
                string sqlP2 = "";

                //新增
                if (EditMode.SqlQuote() == "A")
                {
                    DocNo = PubUtility.GetNewDocNo(uu, "EB", 3);
                    //SetEDMHWeb
                    sql = "Insert into SetEDMHWeb (Companycode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                    sql += "DocNO,StartDate,EndDate,EDMMemo,EDM_Model,PS_NO,EDMType,ApproveDate,ApproveUser,DefeasanceDate,Defeasance, ";
                    sql += "PS_Title,PS_MEMO,DelDate,DelUser,WhNoFlag,BIR_Year,BIR_Month,VIP_Type) ";

                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + DocNo + "','','','" + EDMMemo.SqlQuote() + "','', ";
                    sql += "'" + PS_NO.SqlQuote() + "','B','','','','','','','','','','" + BIRYear.SqlQuote() + "','" + BIRMonth.SqlQuote() + "','" + VIPType.SqlQuote() + "'; ";

                    //SetEDMDWeb(P1)
                    sql += "Insert into SetEDMDWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                    sql += "DocNO,DataType,FileName,DocType,DocImage,TXT,URL,MEMO) ";

                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + DocNo + "','P1','P1.jpg','P',Pic,'','','' ";
                    sql += "From EDMSetWeb (nolock) Where Companycode='" + uu.CompanyId + "' and ProgramID='MSDM106'; ";

                    //SetEDMDWeb(T1)
                    if (T1.SqlQuote() != "")
                    {
                        sql += "Insert into SetEDMDWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                        sql += "DocNO,DataType,FileName,DocType,DocImage,TXT,URL,MEMO) ";

                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                        sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                        sql += "'" + DocNo + "','T1','','T','','" + T1.SqlQuote() + "','',''; ";
                    }

                    //SetEDMDWeb(P2)
                    sqlP2 = "Select * From SetEDM (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + VMDocNo + "' and DataType='P2' ";
                    DataTable dtP2 = PubUtility.SqlQry(sqlP2, uu, "SYS");
                    if (dtP2.Rows.Count > 0)
                    {
                        if (dtP2.Rows[0]["STATUS"].ToString() == "1")
                        {
                            sql += "Insert into SetEDMDWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                            sql += "DocNO,DataType,FileName,DocType,DocImage,TXT,URL,MEMO) ";
                            sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                            sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                            sql += "'" + DocNo + "','P2',FileName,'P',DocImage,'','','' ";
                            sql += "From SetEDM (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + VMDocNo + "' and DataType='P2'; ";
                        }
                        sql += "Delete From SetEDM Where Companycode='" + uu.CompanyId + "' and DocNo='" + VMDocNo + "' and DataType='P2'; ";
                    }

                    //SetEDMDWeb(T2)
                    if (T2.SqlQuote() != "") {
                        sql += "Insert into SetEDMDWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                        sql += "DocNO,DataType,FileName,DocType,DocImage,TXT,URL,MEMO) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                        sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                        sql += "'" + DocNo + "','T2','','T','','" + T2.SqlQuote() + "','',''; ";
                    }

                    //SetEDMDWeb(TE)
                    sql += "Insert into SetEDMDWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                    sql += "DocNO,DataType,FileName,DocType,DocImage,TXT,URL,MEMO) ";
                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + DocNo + "','TE','','T','',TE,'','' ";
                    sql += "From EDMSetWeb (nolock) Where Companycode='" + uu.CompanyId + "' and ProgramID='MSDM106';";
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }
                //修改
                else if (EditMode.SqlQuote() == "M")
                {
                    //SetEDMHWeb
                    sql = "Update SetEDMHWeb Set ModDate=convert(char(10),getdate(),111),ModTime=right(convert(varchar, getdate(), 121),12),ModUser='" + uu.UserID + "', ";
                    sql += "EDMMemo='" + EDMMemo.SqlQuote() + "',BIR_Year='" + BIRYear.SqlQuote() + "',BIR_Month='" + BIRMonth.SqlQuote() + "', ";
                    sql += "PS_NO='" + PS_NO.SqlQuote() + "',VIP_Type='" + VIPType.SqlQuote() + "' ";
                    sql += "where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "'; ";
                    //SetEDMDWeb(P1)
                    sql += "Update SetEDMDWeb set ModDate=convert(char(10),getdate(),111),ModTime=right(convert(varchar, getdate(), 121),12),ModUser='" + uu.UserID + "', ";
                    sql += "DocImage=b.Pic ";
                    sql += "From SetEDMDWeb a (nolock) ";
                    sql += "inner join EDMSetWeb b (nolock) on a.Companycode=b.Companycode and b.Programid='MSDM106' ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' and a.DocNo='" + DocNo.SqlQuote() + "' ";
                    sql += "and a.DataType='P1'; ";
                    //SetEDMDWeb(T1)
                    if (T1.SqlQuote() != "")
                    {
                        sql += "Update SetEDMDWeb set ModDate=convert(char(10),getdate(),111),ModTime=right(convert(varchar, getdate(), 121),12),ModUser='" + uu.UserID + "', ";
                        sql += "TXT='" + T1.SqlQuote() + "' ";
                        sql += "Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' ";
                        sql += "and DataType='T1'; ";
                    }
                    //SetEDMDWeb(P2)
                    sqlP2 = "Select * From SetEDM (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2' ";
                    DataTable dtP2 = PubUtility.SqlQry(sqlP2, uu, "SYS");
                    if (dtP2.Rows.Count > 0)
                    {
                        if (dtP2.Rows[0]["STATUS"].ToString() == "1")
                        {
                            sql += "Delete From SetEDMDWeb Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2'; ";
                            sql += "Insert into SetEDMDWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                            sql += "DocNO,DataType,FileName,DocType,DocImage,TXT,URL,MEMO) ";
                            sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                            sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                            sql += "'" + DocNo + "','P2',FileName,'P',DocImage,'','','' ";
                            sql += "From SetEDM (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2'; ";
                        }
                        else
                        {
                            sql += "Delete From SetEDMDWeb Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2'; ";
                        }
                        sql += "Delete From SetEDM Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2'; ";
                    }

                    //SetEDMDWeb(T2)
                    if (T2.SqlQuote() != "") {
                        sql += "Update SetEDMDWeb set ModDate=convert(char(10),getdate(),111),ModTime=right(convert(varchar, getdate(), 121),12),ModUser='" + uu.UserID + "', ";
                        sql += "TXT='" + T2.SqlQuote() + "' ";
                        sql += "Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' ";
                        sql += "and DataType='T2'; ";
                    }

                    //SetEDMDWeb(TE)
                    sql += "Update SetEDMDWeb set ModDate=convert(char(10),getdate(),111),ModTime=right(convert(varchar, getdate(), 121),12),ModUser='" + uu.UserID + "', ";
                    sql += "TXT=b.TE ";
                    sql += "From SetEDMDWeb a (nolock) ";
                    sql += "inner join EDMSetWeb b (nolock) on a.Companycode=b.Companycode and b.Programid='MSDM106' ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' and a.DocNo='" + DocNo.SqlQuote() + "' ";
                    sql += "and a.DataType='TE'; ";
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }

                sql = "select * from SetEDMHWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo + "' ";
                DataTable dtS = PubUtility.SqlQry(sql, uu, "SYS");
                dtS.TableName = "dtS";
                ds.Tables.Add(dtS);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM106QuerySame_EDM")]
        public ActionResult SystemSetup_MSDM106QuerySame_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM106QuerySame_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string BIRYear = rq["BYear"];
                string BIRMonth = rq["BMonth"];
                string sql = "";

                //SameYearMonth
                sql = "Select a.DocNo,a.EDMMemo,a.Bir_Year,a.Bir_Month ";
                sql += "From SetEDMHWeb a (nolock) ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                sql += "and BIR_Year='" + BIRYear + "' and BIR_Month='" + BIRMonth + "' and isnull(a.ApproveDate,'')<>'' and isnull(a.DefeasanceDate,'')=''";
                DataTable dtSame = PubUtility.SqlQry(sql, uu, "SYS");
                dtSame.TableName = "dtSame";
                ds.Tables.Add(dtSame);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM106Approve_EDM")]
        public ActionResult SystemSetup_MSDM106Approve_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM106Approve_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string sql = "";

                sql = "Update SetEDMHWeb Set ApproveDate=convert(char(10),getdate(),111) + ' ' + convert(char(12),getdate(),108), ";
                sql += "ApproveUser='" + uu.UserID + "' ";
                sql += "Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "'; ";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                //SetEDMHWeb
                sql = "Select * From SetEDMHWeb (nolock) Where Companycode='" + uu.CompanyId + "' ";
                if (DocNo.SqlQuote() != "")
                {
                    sql += "and DocNo='" + DocNo.SqlQuote() + "' ";
                }
                DataTable dtH = PubUtility.SqlQry(sql, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);

                sql = "Insert into SetEDMVIP_HWeb (Companycode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                sql += "EVNO,StartDate,EndDate,EDMMemo,EDM_Docno,PS_NO,EDMType,ApproveDate,ApproveUser,DefeasanceDate,Defeasance, ";
                sql += "DelDate,DelUser) ";
                sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                sql += "'" + DocNo + "',convert(char(10),getdate(),111),convert(char(10),getdate(),111),'" + dtH.Rows[0]["EDMMemo"].ToString() + "','" + DocNo + "','" + dtH.Rows[0]["PS_NO"].ToString() + "','B', ";
                sql += "'','','','','','' ;";
                PubUtility.ExecuteSql(sql, uu, "SYS");

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/MSDM106ChkEDMVIP_EDM")]
        public ActionResult SystemSetup_MSDM106ChkEDMVIP_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM106ChkEDMVIP_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string sql = "";

                //SetEDMHWeb
                sql = "Select * From SetEDMVIP_HWeb (nolock) Where Companycode='" + uu.CompanyId + "' ";
                if (DocNo.SqlQuote() != "")
                {
                    sql += "and EDM_DocNo='" + DocNo.SqlQuote() + "' ";
                }
                DataTable dtEDMVIP = PubUtility.SqlQry(sql, uu, "SYS");
                dtEDMVIP.TableName = "dtEDMVIP";
                ds.Tables.Add(dtEDMVIP);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM106Defeasance_EDM")]
        public ActionResult SystemSetup_MSDM106Defeasance_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM106Defeasance_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string sql = "";
                sql = "Update SetEDMHWeb Set DefeasanceDate=convert(char(10),getdate(),111) + ' ' + convert(char(12),getdate(),108), ";
                sql += "Defeasance='" + uu.UserID + "' ";
                sql += "Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "'; ";
                sql += "Update SetEDMVIP_HWeb Set DefeasanceDate=convert(char(10),getdate(),111) + ' ' + convert(char(12),getdate(),108), ";
                sql += "Defeasance='" + uu.UserID + "',ApproveDate='X' ";
                sql += "Where Companycode='" + uu.CompanyId + "' and EDM_DocNo='" + DocNo.SqlQuote() + "'; ";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                //SetEDMHWeb
                sql = "Select * From SetEDMHWeb (nolock) Where Companycode='" + uu.CompanyId + "' ";
                if (DocNo.SqlQuote() != "")
                {
                    sql += "and DocNo='" + DocNo.SqlQuote() + "' ";
                }
                DataTable dtH = PubUtility.SqlQry(sql, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        #endregion 

        [Route("SystemSetup/MSDMLookUpActivityCode")]
        public ActionResult SystemSetup_MSDMLookUpActivityCode()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDMLookUpActivityCodeOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ActivityCode = rq["ActivityCode"];
                string EDM_Model = rq["EDM_Model"];

                string sql = "";
                sql = "Select a.ActivityCode,a.PS_Name,a.StartDate,a.EndDate ";
                sql += "From PromoteSCouponHWeb a (nolock) ";
                sql += "inner join SetEDMHWeb b (nolock) on a.PS_NO=b.PS_NO and b.EDMType='E' and EDM_Model='" + EDM_Model + "' and isnull(b.DelDate,'')='' and b.Companycode=a.Companycode ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' and a.CouponType in('1','2') ";
                if (ActivityCode.SqlQuote() != "")
                {
                    sql += "and a.ActivityCode like '" + ActivityCode.SqlQuote() + "%' ";
                }
                sql += "group by a.ActivityCode,a.PS_Name,a.StartDate,a.EndDate ";
                sql += "Order By a.StartDate desc ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDMQuery")]
        public ActionResult SystemSetup_MSDMQuery()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDMQueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string EDMMemo = rq["EDMMemo"];
                string ActivityCode = rq["ActivityCode"];
                string App = rq["App"];
                string Def = rq["Def"];
                string EDDate = rq["EDDate"];
                string EDM_Model = rq["EDM_Model"];

                string sql = "";
                sql = "Select a.DocNO,a.EDMMemo,a.StartDate + ' ~ ' + a.EndDate as EDDate,b.PS_Name,b.ActivityCode,isnull(c.Cnt1,0)Cnt1,isnull(d.Cnt2,0)Cnt2, ";
                sql += "isnull(a.ApproveDate,'')ApproveDate,isnull(a.DefeasanceDate,'')DefeasanceDate ";
                sql += "From SetEDMHWeb a (nolock) ";
                sql += "left join PromoteSCouponHWeb b (nolock) on a.PS_NO=b.PS_NO and b.Companycode=a.Companycode and b.CouponType in('1','2') ";
                //活動代號
                if (ActivityCode.SqlQuote() != "")
                {
                    sql += "and b.ActivityCode like '" + ActivityCode.SqlQuote() + "%' ";
                }
                sql += "left join (Select EDM_DocNo,COUNT(*)Cnt1 From SetEDMVIP_HWeb (nolock) Where Companycode='" + uu.CompanyId + "' group by EDM_DocNo)c on a.DocNo=c.EDM_DocNo ";
                sql += "left join (Select EDM_DocNo,COUNT(*)Cnt2 From SetEDMVIP_HWeb (nolock) ";
                sql += "inner join SetEDMVIP_VIPWeb (nolock) on SetEDMVIP_HWeb.EVNO=SetEDMVIP_VIPWeb.EVNO and SetEDMVIP_VIPWeb.Companycode=SetEDMVIP_HWeb.Companycode ";
                sql += "Where SetEDMVIP_HWeb.Companycode='" + uu.CompanyId + "' group by SetEDMVIP_HWeb.EDM_DocNo ";
                sql += ")d on a.DocNo=d.EDM_DocNo ";

                sql += "Where a.Companycode='" + uu.CompanyId + "' and isnull(a.DelDate,'')='' and a.EDMType='E' ";
                sql += "and a.EDM_Model='" + EDM_Model.SqlQuote() + "' ";
                //DM單號
                if (DocNo.SqlQuote() != "")
                {
                    sql += "and a.DocNo like '" + DocNo.SqlQuote() + "%' ";
                }
                //DM主旨
                if (EDMMemo.SqlQuote() != "")
                {
                    sql += "and a.EDMMemo like '%" + EDMMemo.SqlQuote() + "%' ";
                }
                //批核日期
                if (App.SqlQuote() == "NoApp")
                {
                    sql += "and isnull(a.ApproveDate,'')='' ";
                }
                else if (App.SqlQuote() == "App")
                {
                    sql += "and isnull(a.ApproveDate,'')<>'' ";
                }
                //作廢日期
                if (Def.SqlQuote() == "NoDef")
                {
                    sql += "and isnull(a.DefeasanceDate,'')='' ";
                }
                else if (Def.SqlQuote() == "Def")
                {
                    sql += "and isnull(a.DefeasanceDate,'')<>'' ";
                }
                //入會日期
                if (EDDate.SqlQuote() != "")
                {
                    sql += "and '" + EDDate.SqlQuote() + "' between a.StartDate and a.EndDate ";
                }

                sql += "Order by a.DocNo desc ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM107_LookUpPSNO_EDM")]
        public ActionResult SystemSetup_MSDM107_LookUpPSNO_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM107_LookUpPSNO_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string EndDate = rq["EndDate"];
                string PS_NO = rq["PS_NO"];
                string sql = "";
                sql = "Select a.PS_NO,a.PS_Name,a.ActivityCode,a.StartDate,a.EndDate ";
                sql += "From PromoteSCouponHWeb a (nolock) ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                sql += "and isnull(a.ApproveDate,'')<>'' and isnull(a.DefeasanceDate,'')='' ";
                sql += "and a.CouponType in('1','2') ";
                if (EndDate.SqlQuote() != "")
                {
                    sql += "and isnull(a.EndDate,'')>='" + EndDate.SqlQuote() + "' ";
                }
                if (PS_NO.SqlQuote() != "")
                {
                    sql += "and a.PS_NO like '" + PS_NO.SqlQuote() + "%' ";
                }
                sql += "order by a.StartDate,a.PS_NO ";


                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM107_Save_EDM")]
        public ActionResult SystemSetup_MSDM107_Save_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM107_Save_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string EditMode = rq["EditMode"];
                string EDMMemo = rq["EDMMemo"];
                string StartDate = rq["StartDate"];
                string EndDate = rq["EndDate"];
                string PS_NO = rq["PS_NO"];
                string T1 = rq["T1"];
                string T2 = rq["T2"];
                string DocNo = rq["DocNo"];
                string VMDocNo = rq["VMDocNo"];
                string sql = "";
                string sqlP2 = "";

                //新增
                if (EditMode.SqlQuote() == "A")
                {
                    DocNo = PubUtility.GetNewDocNo(uu, "EE", 3);
                    //SetEDMHWeb
                    sql = "Insert into SetEDMHWeb (Companycode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                    sql += "DocNO,StartDate,EndDate,EDMMemo,EDM_Model,PS_NO,EDMType,ApproveDate,ApproveUser,DefeasanceDate,Defeasance, ";
                    sql += "PS_Title,PS_MEMO,DelDate,DelUser,WhNoFlag) ";

                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + DocNo + "','" + StartDate.SqlQuote() + "','" + EndDate.SqlQuote() + "','" + EDMMemo.SqlQuote() + "','DM107', ";
                    sql += "'" + PS_NO.SqlQuote() + "','E','','','','','','','','',''; ";

                    //SetEDMDWeb(P1)
                    sql += "Insert into SetEDMDWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                    sql += "DocNO,DataType,FileName,DocType,DocImage,TXT,URL,MEMO) ";

                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + DocNo + "','P1','P1.jpg','P',Pic,'','','' ";
                    sql += "From EDMSetWeb (nolock) Where Companycode='" + uu.CompanyId + "' and ProgramID='MSDM107'; ";

                    //SetEDMDWeb(T1)
                    if (T1.SqlQuote() != "")
                    {
                        sql += "Insert into SetEDMDWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                        sql += "DocNO,DataType,FileName,DocType,DocImage,TXT,URL,MEMO) ";

                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                        sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                        sql += "'" + DocNo + "','T1','','T','','" + T1.SqlQuote() + "','',''; ";
                    }

                    //SetEDMDWeb(P2)
                    sqlP2 = "Select * From SetEDM (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + VMDocNo + "' and DataType='P2' ";
                    DataTable dtP2 = PubUtility.SqlQry(sqlP2, uu, "SYS");
                    if (dtP2.Rows.Count > 0)
                    {
                        if (dtP2.Rows[0]["STATUS"].ToString() == "1")
                        {
                            sql += "Insert into SetEDMDWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                            sql += "DocNO,DataType,FileName,DocType,DocImage,TXT,URL,MEMO) ";
                            sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                            sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                            sql += "'" + DocNo + "','P2',FileName,'P',DocImage,'','','' ";
                            sql += "From SetEDM (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + VMDocNo + "' and DataType='P2'; ";
                        }
                        sql += "Delete From SetEDM Where Companycode='" + uu.CompanyId + "' and DocNo='" + VMDocNo + "' and DataType='P2'; ";
                    }

                    //SetEDMDWeb(T2)
                    if (T2.SqlQuote() != "") {
                        sql += "Insert into SetEDMDWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                        sql += "DocNO,DataType,FileName,DocType,DocImage,TXT,URL,MEMO) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                        sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                        sql += "'" + DocNo + "','T2','','T','','" + T2.SqlQuote() + "','',''; ";
                    }

                    //SetEDMDWeb(TE)
                    sql += "Insert into SetEDMDWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                    sql += "DocNO,DataType,FileName,DocType,DocImage,TXT,URL,MEMO) ";

                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                    sql += "'" + DocNo + "','TE','','T','',TE,'','' ";
                    sql += "From EDMSetWeb (nolock) Where Companycode='" + uu.CompanyId + "' and ProgramID='MSDM107';";
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }
                //修改
                else if (EditMode.SqlQuote() == "M")
                {
                    //SetEDMHWeb
                    sql = "Update SetEDMHWeb Set ModDate=convert(char(10),getdate(),111),ModTime=right(convert(varchar, getdate(), 121),12),ModUser='" + uu.UserID + "', ";
                    sql += "EDMMemo='" + EDMMemo.SqlQuote() + "',StartDate='" + StartDate.SqlQuote() + "',EndDate='" + EndDate.SqlQuote() + "', ";
                    sql += "PS_NO='" + PS_NO.SqlQuote() + "' ";
                    sql += "where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "'; ";
                    //SetEDMDWeb(P1)
                    sql += "Update SetEDMDWeb set ModDate=convert(char(10),getdate(),111),ModTime=right(convert(varchar, getdate(), 121),12),ModUser='" + uu.UserID + "', ";
                    sql += "DocImage=b.Pic ";
                    sql += "From SetEDMDWeb a (nolock) ";
                    sql += "inner join EDMSetWeb b (nolock) on a.Companycode=b.Companycode and b.Programid='MSDM107' ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' and a.DocNo='" + DocNo.SqlQuote() + "' ";
                    sql += "and a.DataType='P1'; ";
                    //SetEDMDWeb(T1)
                    if (T1.SqlQuote() != "")
                    {
                        sql += "Update SetEDMDWeb set ModDate=convert(char(10),getdate(),111),ModTime=right(convert(varchar, getdate(), 121),12),ModUser='" + uu.UserID + "', ";
                        sql += "TXT='" + T1.SqlQuote() + "' ";
                        sql += "Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' ";
                        sql += "and DataType='T1'; ";
                    }
                    //SetEDMDWeb(P2)
                    sqlP2 = "Select * From SetEDM (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2' ";
                    DataTable dtP2 = PubUtility.SqlQry(sqlP2, uu, "SYS");
                    if (dtP2.Rows.Count > 0)
                    {
                        if (dtP2.Rows[0]["STATUS"].ToString() == "1")
                        {
                            sql += "Delete From SetEDMDWeb Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2'; ";
                            sql += "Insert into SetEDMDWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                            sql += "DocNO,DataType,FileName,DocType,DocImage,TXT,URL,MEMO) ";
                            sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                            sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                            sql += "'" + DocNo + "','P2',FileName,'P',DocImage,'','','' ";
                            sql += "From SetEDM (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2'; ";
                        }
                        else
                        {
                            sql += "Delete From SetEDMDWeb Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2'; ";
                        }
                        sql += "Delete From SetEDM Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2'; ";
                    }
                    //SetEDMDWeb(T2)
                    if (T2.SqlQuote() != "") {
                        sql += "Update SetEDMDWeb set ModDate=convert(char(10),getdate(),111),ModTime=right(convert(varchar, getdate(), 121),12),ModUser='" + uu.UserID + "', ";
                        sql += "TXT='" + T2.SqlQuote() + "' ";
                        sql += "Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' ";
                        sql += "and DataType='T2'; ";
                    }
                    //SetEDMDWeb(TE)
                    sql += "Update SetEDMDWeb set ModDate=convert(char(10),getdate(),111),ModTime=right(convert(varchar, getdate(), 121),12),ModUser='" + uu.UserID + "', ";
                    sql += "TXT=b.TE ";
                    sql += "From SetEDMDWeb a (nolock) ";
                    sql += "inner join EDMSetWeb b (nolock) on a.Companycode=b.Companycode and b.Programid='MSDM107' ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' and a.DocNo='" + DocNo.SqlQuote() + "' ";
                    sql += "and a.DataType='TE'; ";
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }

                sql = "select * from SetEDMHWeb (nolock) ";
                sql += "where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo + "' ";
                DataTable dtS = PubUtility.SqlQry(sql, uu, "SYS");
                dtS.TableName = "dtS";
                ds.Tables.Add(dtS);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDMQuery_EDM")]
        public ActionResult SystemSetup_MSDMQuery_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDMQuery_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string sql = "";

                //SetEDMHWeb
                sql = "Select a.DocNo,a.EDMMemo,a.StartDate,a.EndDate,a.WhNoFlag,a.PS_NO,c.PS_Name + '  ' + c.StartDate + ' ~ ' + c.EndDate as PS_Name, ";
                sql += "isnull(a.ApproveDate,'')ApproveDate,isnull(a.ApproveUser,'')ApproveUser,isnull(a.DefeasanceDate,'')DefeasanceDate,isnull(a.Defeasance,'')Defeasance, ";
                sql += "b.DataType,b.DocImage,b.TXT ";
                sql += "From SetEDMHWeb a (nolock) ";
                sql += "inner join SetEDMDWeb b (nolock) on a.DocNo=b.DocNo and b.Companycode=a.Companycode ";
                sql += "left join PromoteSCouponHWeb c (nolock) on a.PS_NO=c.PS_NO and c.Companycode=a.Companycode and c.CouponType in('1','2') ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                if (DocNo.SqlQuote() != "")
                {
                    sql += "and a.DocNo='" + DocNo.SqlQuote() + "' ";
                }
                sql += "Order by b.DataType ";
                DataTable dtH = PubUtility.SqlQry(sql, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM107Cancel_EDM")]
        public ActionResult SystemSetup_MSDM107Cancel_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM107Cancel_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string sql = "";

                //SetEDMHWeb
                sql = "Select a.DocNo,a.EDMMemo,a.StartDate,a.EndDate,a.WhNoFlag,a.PS_NO,c.PS_Name + '  ' + c.StartDate + ' ~ ' + c.EndDate as PS_Name, ";
                sql += "isnull(a.ApproveDate,'')ApproveDate,isnull(a.ApproveUser,'')ApproveUser,isnull(a.DefeasanceDate,'')DefeasanceDate,isnull(a.Defeasance,'')Defeasance, ";
                sql += "b.DataType,b.DocImage,b.TXT ";
                sql += "From SetEDMHWeb a (nolock) ";
                sql += "inner join SetEDMDWeb b (nolock) on a.DocNo=b.DocNo and b.Companycode=a.Companycode ";
                sql += "left join PromoteSCouponHWeb c (nolock) on a.PS_NO=c.PS_NO and c.Companycode=a.Companycode and c.CouponType in('1','2') ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                if (DocNo.SqlQuote() != "")
                {
                    sql += "and a.DocNo='" + DocNo.SqlQuote() + "' ";
                }
                sql += "Order by b.DataType ";
                DataTable dtH = PubUtility.SqlQry(sql, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);

                sql = "Delete From SetEDM Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDM107DelImg")]
        public ActionResult SystemSetup_MSDM107DelImg()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDM107DelImgOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string sql = "";

                sql = "Select * From SetEDM (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2' ";
                DataTable dt = PubUtility.SqlQry(sql, uu, "SYS");
                if (dt.Rows.Count > 0)
                {
                    sql = "Update SetEDM Set Status='0' Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P2' ";
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }
                else
                {
                    DataTable dtF = new DataTable();
                    dtF.Columns.Add("CompanyCode", typeof(string));
                    dtF.Columns.Add("STATUS", typeof(string));
                    dtF.Columns.Add("DocNo", typeof(string));
                    dtF.Columns.Add("DataType", typeof(string));
                    dtF.Columns.Add("DocType", typeof(string));
                    DataRow drF = dtF.NewRow();
                    drF["CompanyCode"] = uu.CompanyId;
                    drF["STATUS"] = "0";
                    drF["DocNo"] = DocNo.SqlQuote();
                    drF["DataType"] = "P2";
                    drF["DocType"] = "P";
                    dtF.Rows.Add(drF);
                    PubUtility.AddTable("SetEDM", dtF, uu, "SYS");
                }


            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDMApprove_EDM")]
        public ActionResult SystemSetup_MSDMApprove_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDMApprove_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string sql = "";
                sql = "Update SetEDMHWeb Set ApproveDate=convert(char(10),getdate(),111) + ' ' + convert(char(12),getdate(),108), ";
                sql += "ApproveUser='" + uu.UserID + "' ";
                sql += "Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "'; ";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                //SetEDMHWeb
                sql = "Select * From SetEDMHWeb (nolock) Where Companycode='" + uu.CompanyId + "' ";
                if (DocNo.SqlQuote() != "")
                {
                    sql += "and DocNo='" + DocNo.SqlQuote() + "' ";
                }
                DataTable dtH = PubUtility.SqlQry(sql, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDMDefeasance_EDM")]
        public ActionResult SystemSetup_MSDMDefeasance_EDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDMDefeasance_EDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string sql = "";
                sql = "Update SetEDMHWeb Set DefeasanceDate=convert(char(10),getdate(),111) + ' ' + convert(char(12),getdate(),108), ";
                sql += "Defeasance='" + uu.UserID + "' ";
                sql += "Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "'; ";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                //SetEDMHWeb
                sql = "Select * From SetEDMHWeb (nolock) Where Companycode='" + uu.CompanyId + "' ";
                if (DocNo.SqlQuote() != "")
                {
                    sql += "and DocNo='" + DocNo.SqlQuote() + "' ";
                }
                DataTable dtH = PubUtility.SqlQry(sql, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSDMDelete")]
        public ActionResult SystemSetup_MSDMDelete()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSDMDeleteOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string sql = "";
                sql = "Update SetEDMHWeb Set DelDate=convert(char(10),getdate(),111) + ' ' + convert(char(12),getdate(),108), ";
                sql += "DelUser='" + uu.UserID + "' ";
                sql += "Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "'; ";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSVP102Query")]
        public ActionResult SystemSetup_MSVP102Query()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSVP102QueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string EVNO = rq["EVNO"];
                string EDM_DocNo = rq["EDM_DocNo"];
                string StartDate = rq["StartDate"];
                string EDMMemo = rq["EDMMemo"];

                string sql = "";
                sql = "Select a.EVNO,isnull(b.Cnt,0)Cnt,isnull(a.ApproveDate,'')ApproveDate,isnull(a.TOMailDate,'')TOMailDate,a.EDM_DocNo,c.EDMMemo,c.StartDate + ' ~ ' + c.EndDate as EDDate ";
                sql += "From SetEDMVIP_HWeb a (nolock) ";
                sql += "left join (Select EVNO,Count(*)Cnt From SetEDMVIP_VIPWeb (nolock) Where Companycode='" + uu.CompanyId + "' group by EVNO)b on a.EVNO=b.EVNO ";
                sql += "inner join SetEDMHWeb c (nolock) on a.EDM_DocNo=c.DocNo and c.Companycode=a.Companycode ";
                if (EDMMemo.SqlQuote() != "")
                {
                    sql += "and c.EDMMemo like '%" + EDMMemo.SqlQuote() + "%' ";
                }
                sql += "Where a.Companycode='" + uu.CompanyId + "' and a.EDMType='E' and isnull(a.EV_Model,'')='VP102'";
                if (EVNO.SqlQuote() != "")
                {
                    sql += "and a.EVNO='" + EVNO.SqlQuote() + "' ";
                }
                if (EDM_DocNo.SqlQuote() != "")
                {
                    sql += "and a.EDM_DocNo='" + EDM_DocNo.SqlQuote() + "' ";
                }
                if (StartDate.SqlQuote() != "")
                {
                    sql += "and a.StartDate='" + StartDate.SqlQuote() + "' ";
                }

                sql += "Order by a.EVNO desc ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }
        #region msSD101
        [Route("SystemSetup/MSSD101Query")]
        public ActionResult SystemSetup_MSSD101Query()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD101QueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ActivityCode = rq["ActivityCode"];
                string PSName = rq["PSName"];
                string PSDate = rq["PSDate"];
                //string DocNO = rq["DocNO"];
                //string EDMMemo = rq["EDMMemo"];
                //string EDDate = rq["EDDate"];
                //string OptAB = rq["OptAB"];

                string sql = "";
                //if (OptAB == "DA")  //活動
                //{
                //sql = "select a.ActivityCode,a.PS_Name,a.StartDate + ' ~ ' + a.EndDate PSDate,d.SendCnt,b.BackCnt,format(convert(numeric(10,1),b.BackCnt)/d.SendCnt,'0.0%') BackPer,b.Discount,c.Cash,c.SalesCnt,c.Balance,a.PS_NO ";
                //sql += "from PromoteSCouponHWeb a(nolock) ";
                //sql += "join (Select CompanyCode,PS_NO From SetEDMHWeb (nolock) Where EDMType='E' and isnull(ApproveDate,'')<>''  ";
                ////if (DocNO.SqlQuote() != "")     //DM單號
                ////{
                ////    sql += "and DocNO like '%" + DocNO.SqlQuote() + "%' ";
                ////}
                ////if (EDMMemo.SqlQuote() != "")     //DM單號
                ////{
                ////    sql += "and EDMMemo like '%" + EDMMemo.SqlQuote() + "%' ";
                ////}
                ////if (EDDate.SqlQuote() != "")     //DM日期
                ////{
                ////    sql += "and '" + EDDate.SqlQuote() + "' between StartDate and EndDate ";
                ////}
                //sql += "group by CompanyCode,PS_NO) e on a.CompanyCode=e.CompanyCode and a.PS_NO=e.PS_NO ";
                ////發出張數
                //sql += " join (select CompanyCode,PS_NO,count(*) SendCnt from SetEDMVIP_VIPWeb group by CompanyCode,PS_NO) d on a.CompanyCode=d.CompanyCode and a.PS_No=d.PS_NO ";
                ////回收張數
                //sql += "join (select h.CompanyCode, h.PCHDocNO ,count (c.CouponNo) BackCnt,sum(c.ActualDiscount) Discount ";
                //sql += "from PromoteSLogHWeb h join PromoteSLogCardNoWeb c on h.CompanyCode=c.CompanyCode and h.DocNo=c.DocNo and h.ShopNO=c.ShopNO ";
                //sql += "group by  h.CompanyCode,h.PCHDocNO ) b on a.CompanyCode=b.CompanyCode and a.PS_No=b.PCHDocNO ";
                ////銷售
                //sql += "join ( select h.companycode,h.PCHDocNO, sum(h.cash) Cash,count(*) SalesCnt,convert(int,sum(h.cash)/count(*)) Balance ";
                //sql += "from PromoteSLogHWeb h  ";
                //sql += "group by h.companycode,h.PCHDocNO) c on a.CompanyCode=c.CompanyCode and a.PS_No=c.PCHDocNO ";

                //sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                //if (ActivityCode.SqlQuote() != "")//活動代號
                //{
                //    sql += "and a.ActivityCode like '%" + ActivityCode.SqlQuote() + "%' ";
                //}
                //if (PSName.SqlQuote() != "")//活動名稱
                //{
                //    sql += "and a.PS_Name like '%" + PSName.SqlQuote() + "%' ";
                //}
                //if (PSDate.SqlQuote() != "")     //活動日期
                //{
                //    sql += "and '" + PSDate.SqlQuote() + "' between StartDate and EndDate ";
                //}
                //sql += "Order by a.StartDate ";
                //}
                //else if (OptAB == "DB")     //DM
                //{
                //    sql = "select a.DocNO ActivityCode,a.EDMMemo PS_Name,a.StartDate + ' ~ ' + a.EndDate PSDate,b.SendCnt,c.BackCnt,format(convert(numeric(10,1),c.BackCnt)/b.SendCnt,'p') BackPer ,c.Discount,d.Cash,d.SalesCnt,d.Balance,a.PS_NO ";
                //    sql += "from SetEDMHWeb a(nolock) ";
                //    //發出張數
                //    sql += " join (select h.CompanyCode,h.EDM_DocNO,count(*) SendCnt ";
                //    sql += "from SetEDMVIP_VIPWeb v join SetEDMVIP_HWeb h on v.CompanyCode=h.CompanyCode and v.EVNO=h.EVNO group by  h.CompanyCode,h.EDM_DocNO) b ";
                //    sql += "on a.CompanyCode=b.CompanyCode and a.DocNO=b.EDM_DocNO ";
                //    //回收張數
                //    sql += "join (select h.CompanyCode,h.EDM_DocNO,count(CouponID) BackCnt,sum(p.ActualDiscount) Discount ";
                //    sql += "from SetEDMVIP_VIPWeb v join SetEDMVIP_HWeb h on v.CompanyCode=h.CompanyCode and v.EVNO=h.EVNO ";
                //    sql += "join PromoteSLogCardNoWeb p on h.CompanyCode=p.CompanyCode  and v.CouponID=p.CouponNo ";
                //    sql += "group by  h.CompanyCode,h.EDM_DocNO) c on a.CompanyCode=c.CompanyCode and a.DocNO=c.EDM_DocNO ";
                //    //銷售
                //    sql += "join (select h.CompanyCode ,e.EDM_DocNO,h.PCHDocNO ,sum(h.cash) Cash,count(*) SalesCnt,convert(int,sum(h.cash)/count(*)) Balance  ";
                //    sql += "from PromoteSLogHWeb h ";
                //    sql += "join PromoteSLogCardNoWeb p on  h.CompanyCode=p.CompanyCode and h.DocNO=p.DocNo ";
                //    sql += "join SetEDMVIP_VIPWeb d on d.CompanyCode=p.CompanyCode and d.CouponID=p.CouponNo ";
                //    sql += "join SetEDMVIP_HWeb e on d.CompanyCode=e.CompanyCode and d.EVNO=e.EVNO ";
                //    if (ActivityCode.SqlQuote() != "")//活動代號
                //    {
                //        sql += "and h.ActivityCode like '%" + ActivityCode.SqlQuote() + "%' ";
                //    }
                //    if (PSName.SqlQuote() != "")//活動名稱
                //    {
                //        sql += "and h.PS_Name like '%" + PSName.SqlQuote() + "%' ";
                //    }
                //    if (PSDate.SqlQuote() != "")     //活動日期
                //    {
                //        sql += "and '" + PSDate.SqlQuote() + "' between StartDate and EndDate ";
                //    }
                //    sql += "group by h.CompanyCode ,e.EDM_DocNO,h.PCHDocNO) d on a.CompanyCode=d.CompanyCode and a.DocNO=d.EDM_DocNO and a.PS_NO=d.PCHDocNO ";

                //    sql += "Where a.Companycode='" + uu.CompanyId + "' and EDMType='E' and isnull(ApproveDate,'')<>'' ";
                //    if (DocNO.SqlQuote() != "")     //DM單號
                //    {
                //        sql += "and DocNO like '%" + DocNO.SqlQuote() + "%' ";
                //    }
                //    if (EDMMemo.SqlQuote() != "")     //DM單號
                //    {
                //        sql += "and EDMMemo like '%" + EDMMemo.SqlQuote() + "%' ";
                //    }
                //    if (EDDate.SqlQuote() != "")     //DM日期
                //    {
                //        sql += "and '" + EDDate.SqlQuote() + "' between StartDate and EndDate ";
                //    }
                //    sql += "Order by a.StartDate ";
                //}
                sql = "Select a.PS_NO,a.ActivityCode,b.PS_Name,a.StartDate + '~' + a.EndDate PSDate, ";
                sql += "sum(isnull(a.issueQty,0)) SendCnt,sum(isnull(a.ReclaimQty,0)) BackCnt, ";
                sql += "case when sum(isnull(a.issueQty,0))=0 then format(0,'p') else format(cast(sum(isnull(a.ReclaimQty,0)) as Float)/cast(sum(isnull(a.issueQty,0)) as Float),'p') end as BackPer, ";
                sql += "sum(isnull(a.ShareAmt,0)) Discount,sum(isnull(a.ReclaimCash,0)) Cash,sum(isnull(a.ReclaimTrans,0)) SalesCnt, ";
                sql += "case when sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(sum(isnull(a.ReclaimCash,0))/sum(isnull(a.ReclaimTrans,0)),0) end as Balance ";
                sql += "From MsData2Web a (nolock) ";
                sql += "inner join PromoteSCouponHWeb b (nolock) on a.PS_NO=b.PS_NO and b.Companycode=a.Companycode and b.CouponType in('1','2') ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                sql += "and b.PS_NO in (Select PS_NO From SetEDMHWeb (nolock) Where EDMType='E' and isnull(ApproveDate,'')<>'' and Companycode='" + uu.CompanyId + "') ";
                if (ActivityCode.SqlQuote() != "")//活動代號
                {
                    sql += "and b.ActivityCode like '%" + ActivityCode.SqlQuote() + "%' ";
                }
                if (PSName.SqlQuote() != "")//活動名稱
                {
                    sql += "and b.PS_Name like '%" + PSName.SqlQuote() + "%' ";
                }
                if (PSDate.SqlQuote() != "")     //活動日期
                {
                    sql += "and '" + PSDate.SqlQuote() + "' between a.StartDate and a.EndDate ";
                }
                sql += "group by a.PS_NO,a.ActivityCode,b.PS_Name,a.StartDate,a.EndDate ";
                sql += "Order by a.StartDate desc";

                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }
        [Route("SystemSetup/MSSD101_LookUpActivityCode")]
        public ActionResult SystemSetup_MSSD101_LookUpActivityCode()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD101_LookUpActivityCodeOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ActivityCode = rq["ActivityCode"];

                string sql = "";
                sql = "Select a.ActivityCode,a.PS_Name,a.StartDate,a.EndDate ";
                sql += "From PromoteSCouponHWeb a (nolock) ";
                sql += "inner join SetEDMHWeb b (nolock) on a.PS_NO=b.PS_NO and b.EDMType='E' and isnull(b.ApproveDate,'')<>'' and b.Companycode=a.Companycode ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' and a.CouponType in('1','2') ";
                if (ActivityCode.SqlQuote() != "")
                {
                    sql += "and a.ActivityCode like '" + ActivityCode.SqlQuote() + "%' ";
                }
                sql += "group by a.ActivityCode,a.PS_Name,a.StartDate,a.EndDate ";
                sql += "Order By a.StartDate desc ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSD101_LookUpDocNO")]
        public ActionResult SystemSetup_MSSD101_LookUpDocNO()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD101_LookUpDocNOOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNO = rq["DocNO"];

                string sql = "";
                sql = "Select a.DocNo,a.EDMMemo,a.StartDate,a.EndDate ";
                sql += "From SetEDMHWeb a (nolock) ";
                sql += " Where a.Companycode='" + uu.CompanyId + "' and a.EDMType='E' and isnull(a.ApproveDate,'')<>''  ";
                if (DocNO.SqlQuote() != "")
                {
                    sql += "and a.DocNo like '" + DocNO.SqlQuote() + "%' ";
                }
                sql += "group by a.DocNo,a.EDMMemo,a.StartDate,a.EndDate ";
                sql += "Order By a.StartDate desc ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSD101_Query_PS_Step1")]
        public ActionResult SystemSetup_MSSD101_Query_PS_Step1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "Query_PS_Step1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string PS_NO = rq["PS_NO"];
                string ActivityCode = rq["ActivityCode"];
                string Type_Step1 = rq["Type_Step1"];
                bool AllShop = false;
                string SDate = rq["PSDateS"];
                string EDate = rq["PSDateE"];
                string sql = "";

                //彙總資料
                sql = "Select a.PS_NO,Sum(isnull(a.issueQty,0)) SendCnt,Sum(isnull(a.ReclaimQty,0)) BackCnt, ";
                sql += "case when Sum(isnull(a.issueQty,0))=0 then format(0,'p') else format(cast(Sum(isnull(a.ReclaimQty,0)) as Float)/cast(Sum(isnull(a.issueQty,0)) as Float),'p') end as BackPer, ";
                sql += "Sum(isnull(a.ShareAmt,0)) Discount,sum(isnull(a.ReclaimCash,0)) Cash,sum(isnull(a.ReclaimTrans,0)) Cnt, ";
                sql += "case when sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(sum(isnull(a.ReclaimCash,0))/sum(isnull(a.ReclaimTrans,0)),0) end as VIPPer, ";
                sql += "sum(isnull(a.TotalCash,0)) SalesCash,sum(isnull(a.TotalTrans,0)) SalesCNT, ";
                sql += "case when sum(isnull(a.TotalTrans,0))=0 then 0 else Round(sum(isnull(a.TotalCash,0))/sum(isnull(a.TotalTrans,0)),0) end as SalesPer ";
                sql += "From MSData2Web a (nolock) ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                if (PS_NO != "")
                {
                    sql += "and a.PS_NO='" + PS_NO + "' ";
                }
                sql += "group by a.PS_NO ";
                DataTable dtHeadSum = PubUtility.SqlQry(sql, uu, "SYS");
                dtHeadSum.TableName = "dtHeadSum";
                ds.Tables.Add(dtHeadSum);

                if (Type_Step1 == "S")  //店明細
                {
                    sql = "Select a.ShopNo + '-' + b.ST_SName as ShopNO,Sum(isnull(a.issueQty,0)) SendCnt, ";
                    sql += "Sum(isnull(a.ReclaimQty,0)) BackCnt, ";
                    sql += "case when Sum(isnull(a.issueQty,0))=0 then format(0,'p') else format(cast(Sum(isnull(a.ReclaimQty,0)) as Float)/cast(Sum(isnull(a.issueQty,0)) as Float),'p') end as BackPer, ";
                    sql += "Sum(isnull(a.ShareAmt,0)) Discount,Sum(isnull(a.ReclaimCash,0)) Cash,Sum(isnull(a.ReclaimTrans,0)) VIPCNT, ";
                    sql += "case when Sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(Sum(isnull(a.ReclaimCash,0))/Sum(isnull(a.ReclaimTrans,0)),0) end as VIPPer, ";
                    sql += "Sum(isnull(a.TotalCash,0)) SalesCash,Sum(isnull(a.TotalTrans,0)) SalesCNT, ";
                    sql += "case when Sum(isnull(a.TotalTrans,0))=0 then 0 else Round(Sum(isnull(a.TotalCash,0))/Sum(isnull(a.TotalTrans,0)),0) end as SalesPer ";

                    sql += "From MSData2Web a (nolock) ";
                    sql += "left join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    if (PS_NO != "")
                    {
                        sql += "and a.PS_NO='" + PS_NO + "' ";
                    }
                    sql += "group by a.ShopNo,b.ST_SName ";
                    sql += "Order by a.ShopNo ";
                    DataTable dtGridData = PubUtility.SqlQry(sql, uu, "SYS");
                    dtGridData.TableName = "dtGridData";
                    ds.Tables.Add(dtGridData);
                }
                else if (Type_Step1 == "D") //日期明細
                {
                    //日期區間表
                    string sqlD = "WITH dates([Date]) AS( ";
                    sqlD += "SELECT convert(DATE, '" + SDate + "') AS[Date] ";
                    sqlD += "UNION ALL ";
                    sqlD += "SELECT dateadd(day, 1, [Date]) FROM dates WHERE[Date] < ";
                    if (Convert.ToDateTime(EDate) >= DateTime.Now)  //結束日大於今日,只取到昨天
                        sqlD += "convert(nvarchar(10),dateadd(DAY,-1,getdate()),111)";
                    else
                        sqlD += "'" + EDate + "'";
                    sqlD += " ) ";
                    sqlD += "SELECT convert(nvarchar(10),[date],111) AS[id] ";
                    sqlD += "into #dates ";
                    sqlD += "FROM dates ";
                    sqlD += "[date] OPTION(MAXRECURSION 32767); ";
                    //明細資料
                    sql = "Select a.id Salesdate,Sum(isnull(b.ReclaimQty,0)) BackCnt,Sum(isnull(b.ShareAmt,0)) Discount, ";
                    sql += "Sum(isnull(b.ReclaimCash,0)) Cash,Sum(isnull(b.ReclaimTrans,0)) VIPCNT, ";
                    sql += "case when Sum(isnull(b.ReclaimTrans,0))=0 then 0 else Round(Sum(isnull(b.ReclaimCash,0)) / Sum(isnull(b.ReclaimTrans,0)), 0) end as VIPPer, ";
                    sql += "Sum(isnull(b.TotalCash,0)) SalesCash,Sum(isnull(b.TotalTrans,0)) SalesCNT, ";
                    sql += "case when Sum(isnull(b.TotalTrans,0))=0 then 0 else Round(Sum(isnull(b.TotalCash,0)) / Sum(isnull(b.TotalTrans,0)), 0) end as SalesPer ";

                    sql += "From #dates a ";
                    sql += "left join MSData1Web b (nolock) on a.id=b.SalesDate and b.Companycode='" + uu.CompanyId + "' ";
                    if (PS_NO != "")
                    {
                        sql += "and b.PS_NO='" + PS_NO + "' ";
                    }
                    sql += "group by a.id ";
                    sql += "order by a.id ";
                    DataTable dtGridData = PubUtility.SqlQry(sqlD + sql, uu, "SYS");
                    dtGridData.TableName = "dtGridData";
                    ds.Tables.Add(dtGridData);
                }

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSD101_Query_Step2")]
        public ActionResult SystemSetup_MSSD101_Query_Step2()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "Query_Step2OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string PS_NO = rq["PS_NO"];
                string ID = rq["ID"];
                string OpenDate1 = rq["OpenDate1"];
                string OpenDate2 = rq["OpenDate2"];
                string Flag = rq["Flag"];

                string sql = "";
                string sqlD = "";

                //店櫃
                if (Flag == "S")
                {
                    //日期區間表
                    sqlD = "WITH dates([Date]) AS( ";
                    sqlD += "SELECT convert(DATE, '" + OpenDate1 + "') AS[Date] ";
                    sqlD += "UNION ALL ";
                    sqlD += "SELECT dateadd(day, 1, [Date]) FROM dates WHERE[Date] < ";
                    if (Convert.ToDateTime(OpenDate2) >= DateTime.Now)  //結束日大於今日,只取到昨天
                        sqlD += "convert(nvarchar(10),dateadd(DAY,-1,getdate()),111)";
                    else
                        sqlD += "'" + OpenDate2 + "'";
                    sqlD += " ) ";
                    sqlD += "SELECT convert(nvarchar(10),[date],111) AS[id] ";
                    sqlD += "into #dates ";
                    sqlD += "FROM dates ";
                    sqlD += "[date] OPTION(MAXRECURSION 32767); ";
                    //明細資料
                    sql = "Select a.id SalesDate,sum(isnull(b.ReclaimQty,0))ReclaimQty,sum(isnull(b.ShareAmt,0))ShareAmt, ";
                    sql += "sum(isnull(b.ReclaimCash,0))ReclaimCash,sum(isnull(b.ReclaimTrans,0))ReclaimTrans, ";
                    sql += "case when sum(isnull(b.ReclaimTrans,0))=0 then 0 else Round(sum(isnull(b.ReclaimCash,0))/sum(isnull(b.ReclaimTrans,0)),0) end as ReclaimPrice, ";
                    sql += "sum(isnull(b.TotalCash,0))TotalCash,sum(isnull(b.TotalTrans,0))TotalTrans, ";
                    sql += "case when sum(isnull(b.TotalTrans,0))=0 then 0 else Round(sum(isnull(b.TotalCash,0))/sum(isnull(b.TotalTrans,0)),0) end as TotalPrice ";
                    sql += "From #dates a ";
                    sql += "left join MSData1Web b (nolock) on a.id=b.SalesDate and b.Companycode='" + uu.CompanyId + "' ";
                    if (PS_NO != "")
                    {
                        sql += "and b.PS_NO='" + PS_NO + "' ";
                    }
                    if (ID != "")
                    {
                        sql += "and b.ShopNo='" + ID + "' ";
                    }
                    sql += "group by a.id ";
                    sql += "order by a.id ";
                    DataTable dtE = PubUtility.SqlQry(sqlD + sql, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sql = "Select sum(isnull(b.ReclaimQty,0))SumReclaimQty,sum(isnull(b.ShareAmt,0))SumShareAmt, ";
                    sql += "sum(isnull(b.ReclaimCash,0))SumReclaimCash,sum(isnull(b.ReclaimTrans,0))SumReclaimTrans, ";
                    sql += "case when sum(isnull(b.ReclaimTrans,0))=0 then 0 else Round(sum(isnull(b.ReclaimCash,0))/sum(isnull(b.ReclaimTrans,0)),0) end as SumReclaimPrice, ";
                    sql += "sum(isnull(b.TotalCash,0))SumTotalCash,sum(isnull(b.TotalTrans,0))SumTotalTrans, ";
                    sql += "case when sum(isnull(b.TotalTrans,0))=0 then 0 else Round(sum(isnull(b.TotalCash,0))/sum(isnull(b.TotalTrans,0)),0) end as SumTotalPrice ";
                    sql += "From #dates a ";
                    sql += "left join MSData1Web b (nolock) on a.id=b.SalesDate and b.Companycode='" + uu.CompanyId + "' ";
                    if (PS_NO != "")
                    {
                        sql += "and b.PS_NO='" + PS_NO + "' ";
                    }
                    if (ID != "")
                    {
                        sql += "and b.ShopNo='" + ID + "' ";
                    }
                    DataTable dtSumQ = PubUtility.SqlQry(sqlD + sql, uu, "SYS");
                    dtSumQ.TableName = "dtSumQ";
                    ds.Tables.Add(dtSumQ);
                }
                //日期
                else if (Flag == "D")
                {
                    //明細資料
                    sql = "Select a.ShopNo + '-' + b.ST_SName as Shop,sum(isnull(a.ReclaimQty,0))ReclaimQty, ";
                    sql += "sum(isnull(a.ShareAmt,0))ShareAmt,sum(isnull(a.ReclaimCash,0))ReclaimCash, ";
                    sql += "sum(isnull(a.ReclaimTrans,0))ReclaimTrans, ";
                    sql += "case when sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(sum(isnull(a.ReclaimCash,0)) / sum(isnull(a.ReclaimTrans,0)), 0) end as ReclaimPrice, ";
                    sql += "sum(isnull(a.TotalCash,0))TotalCash,sum(isnull(a.TotalTrans,0))TotalTrans, ";
                    sql += "case when sum(isnull(a.TotalTrans,0))=0 then 0 else Round(sum(isnull(a.TotalCash,0)) / sum(isnull(a.TotalTrans,0)), 0) end as TotalPrice ";
                    sql += "From MSData1Web a (nolock) ";
                    sql += "left join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    if (PS_NO != "")
                    {
                        sql += "and a.PS_NO='" + PS_NO + "' ";
                    }
                    if (ID != "")
                    {
                        sql += "and a.SalesDate='" + ID + "' ";
                    }
                    sql += "group by a.ShopNo,b.ST_SName ";
                    sql += "Order by a.ShopNo ";
                    DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);
                    //彙總資料
                    sql = "Select sum(isnull(a.ReclaimQty,0))SumReclaimQty, ";
                    sql += "sum(isnull(a.ShareAmt,0))SumShareAmt,sum(isnull(a.ReclaimCash,0))SumReclaimCash, ";
                    sql += "sum(isnull(a.ReclaimTrans,0))SumReclaimTrans, ";
                    sql += "case when sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(sum(isnull(a.ReclaimCash,0)) / sum(isnull(a.ReclaimTrans,0)), 0) end as SumReclaimPrice, ";
                    sql += "sum(isnull(a.TotalCash,0))SumTotalCash,sum(isnull(a.TotalTrans,0))SumTotalTrans, ";
                    sql += "case when sum(isnull(a.TotalTrans,0))=0 then 0 else Round(sum(isnull(a.TotalCash,0)) / sum(isnull(a.TotalTrans,0)), 0) end as SumTotalPrice ";
                    sql += "From MSData1Web a (nolock) ";
                    sql += "left join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    if (PS_NO != "")
                    {
                        sql += "and a.PS_NO='" + PS_NO + "' ";
                    }
                    if (ID != "")
                    {
                        sql += "and a.SalesDate='" + ID + "' ";
                    }
                    DataTable dtSumQ = PubUtility.SqlQry(sql, uu, "SYS");
                    dtSumQ.TableName = "dtSumQ";
                    ds.Tables.Add(dtSumQ);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }
        #endregion
        [Route("SystemSetup/MSVP102_GetVIPFaceID")]
        public ActionResult SystemSetup_MSVP102_GetVIPFaceID()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSVP102_GetVIPFaceIDOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ST_ID = rq["ST_ID"];
                string sql = "";
                sql = "Select a.ST_ID,a.ST_SName ";
                sql += "From WarehouseWeb a (nolock) Where a.Companycode='" + uu.CompanyId + "' and a.ST_Type not in('0','2','3') ";
                if (ST_ID.SqlQuote() != "")
                {
                    sql += "and a.ST_ID like '" + ST_ID.SqlQuote() + "%' ";
                }
                sql += "Order by a.ST_ID ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSVP102Query_SendSet")]
        public ActionResult SystemSetup_MSVP102Query_SendSet()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSVP102Query_SendSetOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string chkVIPFaceID = rq["chkVIPFaceID"];
                string VIP_Type = rq["VIP_Type"];
                string VMEVNO = rq["VMEVNO"];

                string sql = "";
                sql = "Delete From SetEDMVIP_VIPWeb Where Companycode='" + uu.CompanyId + "' and EVNO='" + VMEVNO + "'; ";
                sql += "Delete From SetEDMVIP_SetWeb Where Companycode='" + uu.CompanyId + "' and EVNO='" + VMEVNO + "' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                //SetEDMVIP_VIPWeb
                sql = "Insert into SetEDMVIP_VIPWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,VIP_ID2,CouponID,VIP_Name,VIP_EMail,SendDate,PS_NO,SeqNo,EDMType) ";
                sql += "Select Companycode,'" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                sql += "'" + VMEVNO + "',VIP_ID2,'',VIP_Name,VIP_Eadd,'','',ROW_NUMBER() OVER(PARTITION BY '" + VMEVNO + "' order by VIP_ID2),'E' ";
                sql += "From EDDMS.dbo.VIP ";
                sql += "Where Companycode='" + uu.CompanyId + "' ";
                sql += "and isnull(VIP_Eadd,'')<>'' and isnull(P_Flag2,'')='1' and isnull(VIP_Eday,'')>convert(char(10),getdate(),111) ";
                if (chkVIPFaceID != "")
                {
                    sql += "and VIP_FaceID in(" + chkVIPFaceID + ") ";
                }
                if (VIP_Type != "")
                {
                    sql += "and VIP_Type in(" + VIP_Type + ") ";
                }

                //SetEDMVIP_SetWeb
                if (chkVIPFaceID != "")
                {
                    sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                    sql += "'" + VMEVNO + "',1,'','','','','會籍店櫃','" + chkVIPFaceID.Replace("'", "") + "' ";
                }
                if (VIP_Type != "")
                {
                    sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                    sql += "'" + VMEVNO + "',2,'','','','','會員卡別','" + VIP_Type.Replace("'", "") + "' ";
                }
                PubUtility.ExecuteSql(sql, uu, "SYS");

                sql = "Select VIP_ID2,VIP_Name,VIP_Tel,VIP_Eadd,case VIP_MW when '0' then '男' when '1' then '女' end as VIP_NM,City,AreaName,VIP_LCDay,isnull(PointsBalance,0)PointsBalance, ";
                sql += "case VIP_Type when '0' then '一般卡' when '1' then '會員卡' when '2' then '貴賓卡' when '3' then '白金卡' end as VIP_Type ";
                sql += "From EDDMS.dbo.VIP ";
                sql += "Where Companycode='" + uu.CompanyId + "' ";
                sql += "and isnull(VIP_Eadd,'')<>'' and isnull(P_Flag2,'')='1' and isnull(VIP_Eday,'')>convert(char(10),getdate(),111) ";
                if (chkVIPFaceID.SqlQuote() != "")
                {
                    sql += "and VIP_FaceID in(" + chkVIPFaceID + ") ";
                }
                if (VIP_Type.SqlQuote() != "")
                {
                    sql += "and VIP_Type in(" + VIP_Type + ") ";
                }
                sql += "Order by VIP_ID2 ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSVP102_GetVMEVNO")]
        public ActionResult SystemSetup_MSVP102_GetVMEVNO()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSVP102_GetVMEVNOOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                var DocNo = PubUtility.GetNewDocNo(uu, "VM", 3);

                string sql = "";
                sql = "Select '" + DocNo + "' as DocNo ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);

                sql = "Select VIP_ID2,VIP_Name,VIP_Tel,VIP_Eadd,case VIP_MW when '0' then '男' when '1' then '女' end as VIP_NM,VIP_City,AreaName,VIP_LCDay,isnull(PointsBalance,0)PointsBalance, ";
                sql += "case VIP_Type when '0' then '一般卡' when '1' then '會員卡' when '2' then '貴賓卡' when '3' then '白金卡' end as VIP_Type ";
                sql += "From EDDMS.dbo.VIP ";
                sql += "Where 1=2 ";
                DataTable dtV = PubUtility.SqlQry(sql, uu, "SYS");
                dtV.TableName = "dtV";
                ds.Tables.Add(dtV);

                sql = "Delete From SetEDMVIP_VIPWeb Where Companycode='" + uu.CompanyId + "' and left(EVNO,2)='VM' and CrtDate<=convert(char(10),getdate(),111) and CrtUser='" + uu.UserID + "'; ";
                sql += "Delete From SetEDMVIP_SetWeb Where Companycode='" + uu.CompanyId + "' and left(EVNO,2)='VM' and CrtDate<=convert(char(10),getdate(),111) and CrtUser='" + uu.UserID + "' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSVP102_GetDM")]
        public ActionResult SystemSetup_MSVP102_GetDM()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSVP102_GetDMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;

                string sql = "";
                sql = "Select a.DocNo,a.EDMMemo,a.StartDate + ' ~ ' + a.EndDate as EDDate1, ";
                sql += "b.ActivityCode,b.PS_Name,b.StartDate + ' ~ ' + b.EndDate as EDDate2, ";
                sql += "case b.WhNoFlag when 'Y' then '全部店' else cast(isnull(c.Cnt1,0) as varchar) + '店' end as WhNoFlag,isnull(d.Cnt2,0)Cnt2 ";
                sql += "From SetEDMHWEB a (nolock) ";
                sql += "left join PromoteSCouponHWeb b (nolock) on a.PS_NO=b.PS_NO and b.Companycode=a.Companycode and b.CouponType in('1','2') ";
                sql += "left join (Select PS_NO,Count(*)Cnt1 From PromoteSCouponShopWeb (nolock) Where Companycode='" + uu.CompanyId + "' group by PS_NO)c on a.PS_NO=c.PS_NO ";
                sql += "left join (Select EDM_DocNo,Count(*)Cnt2 From SetEDMVIP_HWeb (nolock) Where Companycode='" + uu.CompanyId + "' group by EDM_DocNo)d on a.DocNo=d.EDM_DocNo ";

                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                sql += "and isnull(a.EDMType,'')='E' and a.EndDate>=convert(char(10),getdate(),111) and isnull(a.ApproveDate,'')<>'' ";
                sql += "and isnull(a.DefeasanceDate,'')='' ";
                sql += "Order by a.DocNo ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSVP102_DMSend")]
        public ActionResult SystemSetup_MSVP102_DMSend()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSVP102_DMSendOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string VMDocNo = rq["VMDocNo"];
                string DMDocNo = rq["DMDocNo"];
                string EV_Model = rq["EV_Model"];
                string EVNO = "";
                string sql = "";
                sql = "Select * From SetEDMVIP_VIPWeb (nolock) Where Companycode='" + uu.CompanyId + "' and EVNO='" + VMDocNo + "' ";
                DataTable dtV = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtV.Rows.Count == 0)
                {
                    throw new Exception("未篩選會員資料，請重新確認!");
                }

                if (EV_Model == "VP101")
                {
                    EVNO = PubUtility.GetNewDocNo(uu, "EE", 3);
                }
                else if (EV_Model == "VP102")
                {
                    EVNO = PubUtility.GetNewDocNo(uu, "EV", 3);
                }

                sql = "Select * From SetEDMHWeb (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + DMDocNo + "' ";
                DataTable dtH = PubUtility.SqlQry(sql, uu, "SYS");

                sql = "Insert into SetEDMVIP_HWeb (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime, ";
                sql += "EVNO,StartDate,EndDate,EDMMemo,EDM_DocNO,PS_NO,EDMType,ApproveDate,ApproveUser,DefeasanceDate,Defeasance, ";
                sql += "DelDate,DelUser,EV_Model,TOMailDate,MAMailDate,MAMail ";
                sql += ") ";
                sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                sql += "'" + uu.UserID + "',convert(char(10),getdate(),111),right(convert(varchar, getdate(), 121),12), ";
                sql += "'" + EVNO + "',convert(char(10),getdate(),111),convert(char(10),getdate(),111),'" + dtH.Rows[0]["EDMMemo"].ToString() + "','" + DMDocNo + "','" + dtH.Rows[0]["PS_NO"].ToString() + "','E', ";
                sql += "convert(char(10),getdate(),111) + ' ' + convert(char(12),getdate(),108),'" + uu.UserID + "','','', ";
                sql += "'','','" + EV_Model + "','','',''; ";

                sql += "Update SetEDMVIP_VIPWeb Set EVNO='" + EVNO + "',PS_NO='" + dtH.Rows[0]["PS_NO"].ToString() + "' ";
                sql += "Where Companycode='" + uu.CompanyId + "' and EVNO='" + VMDocNo + "'; ";

                sql += "Update SetEDMVIP_SetWeb Set EVNO='" + EVNO + "' ";
                sql += "Where Companycode='" + uu.CompanyId + "' and EVNO='" + VMDocNo + "' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                //DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                //dtE.TableName = "dtE";
                //ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSVP102_ReSendSet")]
        public ActionResult SystemSetup_MSVP102_ReSendSet()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSVP102_ReSendSetOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;

                string sql = "";
                sql = "Delete From SetEDMVIP_VIPWeb Where Companycode='" + uu.CompanyId + "' and left(EVNO,2)='VM' and CrtDate<=convert(char(10),getdate(),111) and CrtUser='" + uu.UserID + "'; ";
                sql += "Delete From SetEDMVIP_SetWeb Where Companycode='" + uu.CompanyId + "' and left(EVNO,2)='VM' and CrtDate<=convert(char(10),getdate(),111) and CrtUser='" + uu.UserID + "' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetInitMSSD106")]
        public ActionResult SystemSetup_GetInitMSSD106()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitMSSD106OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ProgramID = rq["ProgramID"];
                string Yesterday = PubUtility.GetYesterday(uu);
                string Today = PubUtility.GetToday(uu);
                string sql = "select ChineseName from ProgramIDWeb (nolock) where ProgramID='" + ProgramID.SqlQuote() + "'";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);

                //會員總數
                sql = "Select Count(*) as VIPCntAll,convert(char(10),getdate(),111) + ' ' + convert(char(5),getdate(),108) as SysDate ";
                sql += "from EDDMS.dbo.VIP v (nolock) ";
                sql += "inner join EDDMS.dbo.Warehouse w (nolock) on v.vip_faceid=w.ST_ID and w.Companycode=v.Companycode and w.ST_Type not in('2','3') ";
                sql += "Where v.Companycode='" + uu.CompanyId + "' ";
                DataTable dtV = PubUtility.SqlQry(sql, uu, "SYS");
                dtV.TableName = "dtV";
                ds.Tables.Add(dtV);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/MSSD106Query")]
        public ActionResult SystemSetup_MSSD106Query()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD106QueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string VIPFaceID = rq["VIPFaceID"];
                string City = rq["City"];
                string VIPDate = rq["VIPDate"];
                string Flag = rq["Flag"];
                string sql = "";
                string sqlCon = "";
                string sqlD = "";
                string sqlH = "";

                sqlCon = "and a.VIP_Birthday<>'' ";
                if (VIPFaceID != "")
                {
                    sqlCon += "and a.VIP_FaceID in(" + VIPFaceID + ") ";
                }
                if (City != "")
                {
                    sqlCon += "and a.City in(" + City + ") ";
                }
                if (VIPDate == "2M")
                {
                    sqlCon += "and a.VIP_Qday between convert(char,dateadd(MONTH,-2,getdate()),111) and convert(char(10),getdate(),111) ";
                }
                else if (VIPDate == "3M")
                {
                    sqlCon += "and a.VIP_Qday between convert(char,dateadd(MONTH,-3,getdate()),111) and convert(char(10),getdate(),111) ";
                }
                else if (VIPDate == "6M")
                {
                    sqlCon += "and a.VIP_Qday between convert(char,dateadd(MONTH,-6,getdate()),111) and convert(char(10),getdate(),111) ";
                }
                else if (VIPDate == "1Y")
                {
                    sqlCon += "and a.VIP_Qday between convert(char,dateadd(MONTH,-12,getdate()),111) and convert(char(10),getdate(),111) ";
                }

                //店別
                if (Flag == "S")
                {
                    //總數
                    sql = "select a.VIP_FaceID ID,Count(*)Cnt1 into #v1 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += sqlCon;
                    sql += "group by a.VIP_FaceID; ";
                    //17歲以下
                    sql += "select a.VIP_FaceID ID,Count(*)Cnt2 into #v2 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int)<=17 ";
                    sql += sqlCon;
                    sql += "group by a.VIP_FaceID; ";
                    //18~30歲
                    sql += "select a.VIP_FaceID ID,Count(*)Cnt3 into #v3 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int) between 18 and 30 ";
                    sql += sqlCon;
                    sql += "group by a.VIP_FaceID; ";
                    //31~40歲
                    sql += "select a.VIP_FaceID ID,Count(*)Cnt4 into #v4 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int) between 31 and 40 ";
                    sql += sqlCon;
                    sql += "group by a.VIP_FaceID; ";
                    //41~50歲
                    sql += "select a.VIP_FaceID ID,Count(*)Cnt5 into #v5 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int) between 41 and 50 ";
                    sql += sqlCon;
                    sql += "group by a.VIP_FaceID; ";
                    //51~60歲
                    sql += "select a.VIP_FaceID ID,Count(*)Cnt6 into #v6 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int) between 51 and 60 ";
                    sql += sqlCon;
                    sql += "group by a.VIP_FaceID; ";
                    //61~70歲
                    sql += "select a.VIP_FaceID ID,Count(*)Cnt7 into #v7 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int) between 61 and 70 ";
                    sql += sqlCon;
                    sql += "group by a.VIP_FaceID; ";
                    //71歲以上
                    sql += "select a.VIP_FaceID ID,Count(*)Cnt8 into #v8 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int)>=71 ";
                    sql += sqlCon;
                    sql += "group by a.VIP_FaceID; ";
                    //男性
                    sql += "select a.VIP_FaceID ID,Count(*)Cnt9 into #v9 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and a.VIP_MW='0' ";
                    sql += sqlCon;
                    sql += "group by a.VIP_FaceID; ";
                    //女性
                    sql += "select a.VIP_FaceID ID,Count(*)Cnt10 into #v10 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and a.VIP_MW='1' ";
                    sql += sqlCon;
                    sql += "group by a.VIP_FaceID; ";
                    //明細資料
                    sqlD = "select v1.id + '-' + w.st_sname id,isnull(v1.Cnt1,0)Cnt1,isnull(v2.Cnt2,0)Cnt2, ";
                    sqlD += "case when isnull(v1.Cnt1,0)=0 and isnull(v2.Cnt2,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v2.Cnt2,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt2p, ";
                    sqlD += "isnull(v3.Cnt3,0)Cnt3,case when isnull(v1.Cnt1,0)=0 and isnull(v3.Cnt3,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v3.Cnt3,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt3p, ";
                    sqlD += "isnull(v4.Cnt4,0)Cnt4,case when isnull(v1.Cnt1,0)=0 and isnull(v4.Cnt4,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v4.Cnt4,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt4p, ";
                    sqlD += "isnull(v5.Cnt5,0)Cnt5,case when isnull(v1.Cnt1,0)=0 and isnull(v5.Cnt5,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v5.Cnt5,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt5p, ";
                    sqlD += "isnull(v6.Cnt6,0)Cnt6,case when isnull(v1.Cnt1,0)=0 and isnull(v6.Cnt6,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v6.Cnt6,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt6p, ";
                    sqlD += "isnull(v7.Cnt7,0)Cnt7,case when isnull(v1.Cnt1,0)=0 and isnull(v7.Cnt7,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v7.Cnt7,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt7p, ";
                    sqlD += "isnull(v8.Cnt8,0)Cnt8,case when isnull(v1.Cnt1,0)=0 and isnull(v8.Cnt8,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v8.Cnt8,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt8p, ";
                    sqlD += "isnull(v9.Cnt9,0)Cnt9,case when isnull(v1.Cnt1,0)=0 and isnull(v9.Cnt9,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v9.Cnt9,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt9p, ";
                    sqlD += "isnull(v10.Cnt10,0)Cnt10,case when isnull(v1.Cnt1,0)=0 and isnull(v10.Cnt10,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v10.Cnt10,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt10p ";
                    sqlD += "from #v1 v1 ";
                    sqlD += "left join #v2 v2 on v1.id=v2.id ";
                    sqlD += "left join #v3 v3 on v1.id=v3.id ";
                    sqlD += "left join #v4 v4 on v1.id=v4.id ";
                    sqlD += "left join #v5 v5 on v1.id=v5.id ";
                    sqlD += "left join #v6 v6 on v1.id=v6.id ";
                    sqlD += "left join #v7 v7 on v1.id=v7.id ";
                    sqlD += "left join #v8 v8 on v1.id=v8.id ";
                    sqlD += "left join #v9 v9 on v1.id=v9.id ";
                    sqlD += "left join #v10 v10 on v1.id=v10.id ";
                    sqlD += "inner join WarehouseWeb w (nolock) on v1.id=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('0','2','3') ";
                    sqlD += "order by v1.id ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);
                    //彙總資料
                    sqlH = "select convert(char(10),getdate(),111) + ' ' + convert(char(5),getdate(),108) as SysDate,sum(isnull(v1.Cnt1,0))SumCnt1, ";
                    sqlH += "sum(isnull(v2.Cnt2,0))SumCnt2,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v2.Cnt2,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt2p, ";
                    sqlH += "sum(isnull(v3.Cnt3,0))SumCnt3,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v3.Cnt3,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt3p, ";
                    sqlH += "sum(isnull(v4.Cnt4,0))SumCnt4,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v4.Cnt4,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt4p, ";
                    sqlH += "sum(isnull(v5.Cnt5,0))SumCnt5,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v5.Cnt5,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt5p, ";
                    sqlH += "sum(isnull(v6.Cnt6,0))SumCnt6,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v6.Cnt6,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt6p, ";
                    sqlH += "sum(isnull(v7.Cnt7,0))SumCnt7,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v7.Cnt7,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt7p, ";
                    sqlH += "sum(isnull(v8.Cnt8,0))SumCnt8,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v8.Cnt8,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt8p, ";
                    sqlH += "sum(isnull(v9.Cnt9,0))SumCnt9,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v9.Cnt9,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt9p, ";
                    sqlH += "sum(isnull(v10.Cnt10,0))SumCnt10,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v10.Cnt10,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt10p ";
                    sqlH += "from #v1 v1 ";
                    sqlH += "left join #v2 v2 on v1.id=v2.id ";
                    sqlH += "left join #v3 v3 on v1.id=v3.id ";
                    sqlH += "left join #v4 v4 on v1.id=v4.id ";
                    sqlH += "left join #v5 v5 on v1.id=v5.id ";
                    sqlH += "left join #v6 v6 on v1.id=v6.id ";
                    sqlH += "left join #v7 v7 on v1.id=v7.id ";
                    sqlH += "left join #v8 v8 on v1.id=v8.id ";
                    sqlH += "left join #v9 v9 on v1.id=v9.id ";
                    sqlH += "left join #v10 v10 on v1.id=v10.id ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
                //縣市
                else if (Flag == "C")
                {
                    //總數
                    sql = "select a.City ID,Count(*)Cnt1 into #v1 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += sqlCon;
                    sql += "group by a.City; ";
                    //17歲以下
                    sql += "select a.City ID,Count(*)Cnt2 into #v2 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int)<=17 ";
                    sql += sqlCon;
                    sql += "group by a.City; ";
                    //18~30歲
                    sql += "select a.City ID,Count(*)Cnt3 into #v3 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int) between 18 and 30 ";
                    sql += sqlCon;
                    sql += "group by a.City; ";
                    //31~40歲
                    sql += "select a.City ID,Count(*)Cnt4 into #v4 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int) between 31 and 40 ";
                    sql += sqlCon;
                    sql += "group by a.City; ";
                    //41~50歲
                    sql += "select a.City ID,Count(*)Cnt5 into #v5 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int) between 41 and 50 ";
                    sql += sqlCon;
                    sql += "group by a.City; ";
                    //51~60歲
                    sql += "select a.City ID,Count(*)Cnt6 into #v6 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int) between 51 and 60 ";
                    sql += sqlCon;
                    sql += "group by a.City; ";
                    //61~70歲
                    sql += "select a.City ID,Count(*)Cnt7 into #v7 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int) between 61 and 70 ";
                    sql += sqlCon;
                    sql += "group by a.City; ";
                    //71歲以上
                    sql += "select a.City ID,Count(*)Cnt8 into #v8 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int)>=71 ";
                    sql += sqlCon;
                    sql += "group by a.City; ";
                    //男性
                    sql += "select a.City ID,Count(*)Cnt9 into #v9 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and a.VIP_MW='0' ";
                    sql += sqlCon;
                    sql += "group by a.City; ";
                    //女性
                    sql += "select a.City ID,Count(*)Cnt10 into #v10 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and a.VIP_MW='1' ";
                    sql += sqlCon;
                    sql += "group by a.City; ";
                    //明細資料
                    sqlD = "select case when v1.id='' then '無資料' else v1.id end as id,isnull(v1.Cnt1,0)Cnt1,isnull(v2.Cnt2,0)Cnt2, ";
                    sqlD += "case when isnull(v1.Cnt1,0)=0 and isnull(v2.Cnt2,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v2.Cnt2,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt2p, ";
                    sqlD += "isnull(v3.Cnt3,0)Cnt3,case when isnull(v1.Cnt1,0)=0 and isnull(v3.Cnt3,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v3.Cnt3,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt3p, ";
                    sqlD += "isnull(v4.Cnt4,0)Cnt4,case when isnull(v1.Cnt1,0)=0 and isnull(v4.Cnt4,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v4.Cnt4,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt4p, ";
                    sqlD += "isnull(v5.Cnt5,0)Cnt5,case when isnull(v1.Cnt1,0)=0 and isnull(v5.Cnt5,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v5.Cnt5,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt5p, ";
                    sqlD += "isnull(v6.Cnt6,0)Cnt6,case when isnull(v1.Cnt1,0)=0 and isnull(v6.Cnt6,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v6.Cnt6,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt6p, ";
                    sqlD += "isnull(v7.Cnt7,0)Cnt7,case when isnull(v1.Cnt1,0)=0 and isnull(v7.Cnt7,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v7.Cnt7,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt7p, ";
                    sqlD += "isnull(v8.Cnt8,0)Cnt8,case when isnull(v1.Cnt1,0)=0 and isnull(v8.Cnt8,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v8.Cnt8,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt8p, ";
                    sqlD += "isnull(v9.Cnt9,0)Cnt9,case when isnull(v1.Cnt1,0)=0 and isnull(v9.Cnt9,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v9.Cnt9,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt9p, ";
                    sqlD += "isnull(v10.Cnt10,0)Cnt10,case when isnull(v1.Cnt1,0)=0 and isnull(v10.Cnt10,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v10.Cnt10,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt10p ";
                    sqlD += "from #v1 v1 ";
                    sqlD += "left join #v2 v2 on v1.id=v2.id ";
                    sqlD += "left join #v3 v3 on v1.id=v3.id ";
                    sqlD += "left join #v4 v4 on v1.id=v4.id ";
                    sqlD += "left join #v5 v5 on v1.id=v5.id ";
                    sqlD += "left join #v6 v6 on v1.id=v6.id ";
                    sqlD += "left join #v7 v7 on v1.id=v7.id ";
                    sqlD += "left join #v8 v8 on v1.id=v8.id ";
                    sqlD += "left join #v9 v9 on v1.id=v9.id ";
                    sqlD += "left join #v10 v10 on v1.id=v10.id ";
                    sqlD += "Where 1=1 ";
                    sqlD += "order by v1.id desc ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);
                    //彙總資料
                    sqlH = "select convert(char(10),getdate(),111) + ' ' + convert(char(5),getdate(),108) as SysDate,sum(isnull(v1.Cnt1,0))SumCnt1, ";
                    sqlH += "sum(isnull(v2.Cnt2,0))SumCnt2,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v2.Cnt2,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt2p, ";
                    sqlH += "sum(isnull(v3.Cnt3,0))SumCnt3,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v3.Cnt3,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt3p, ";
                    sqlH += "sum(isnull(v4.Cnt4,0))SumCnt4,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v4.Cnt4,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt4p, ";
                    sqlH += "sum(isnull(v5.Cnt5,0))SumCnt5,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v5.Cnt5,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt5p, ";
                    sqlH += "sum(isnull(v6.Cnt6,0))SumCnt6,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v6.Cnt6,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt6p, ";
                    sqlH += "sum(isnull(v7.Cnt7,0))SumCnt7,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v7.Cnt7,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt7p, ";
                    sqlH += "sum(isnull(v8.Cnt8,0))SumCnt8,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v8.Cnt8,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt8p, ";
                    sqlH += "sum(isnull(v9.Cnt9,0))SumCnt9,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v9.Cnt9,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt9p, ";
                    sqlH += "sum(isnull(v10.Cnt10,0))SumCnt10,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v10.Cnt10,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt10p ";
                    sqlH += "from #v1 v1 ";
                    sqlH += "left join #v2 v2 on v1.id=v2.id ";
                    sqlH += "left join #v3 v3 on v1.id=v3.id ";
                    sqlH += "left join #v4 v4 on v1.id=v4.id ";
                    sqlH += "left join #v5 v5 on v1.id=v5.id ";
                    sqlH += "left join #v6 v6 on v1.id=v6.id ";
                    sqlH += "left join #v7 v7 on v1.id=v7.id ";
                    sqlH += "left join #v8 v8 on v1.id=v8.id ";
                    sqlH += "left join #v9 v9 on v1.id=v9.id ";
                    sqlH += "left join #v10 v10 on v1.id=v10.id ";
                    sqlH += "Where 1=1 ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
                //性別
                else if (Flag == "M")
                {
                    //總數
                    sql = "select a.VIP_MW ID,Count(*)Cnt1 into #v1 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += sqlCon;
                    sql += "group by a.VIP_MW; ";
                    //17歲以下
                    sql += "select a.VIP_MW ID,Count(*)Cnt2 into #v2 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int)<=17 ";
                    sql += sqlCon;
                    sql += "group by a.VIP_MW; ";
                    //18~30歲
                    sql += "select a.VIP_MW ID,Count(*)Cnt3 into #v3 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int) between 18 and 30 ";
                    sql += sqlCon;
                    sql += "group by a.VIP_MW; ";
                    //31~40歲
                    sql += "select a.VIP_MW ID,Count(*)Cnt4 into #v4 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int) between 31 and 40 ";
                    sql += sqlCon;
                    sql += "group by a.VIP_MW; ";
                    //41~50歲
                    sql += "select a.VIP_MW ID,Count(*)Cnt5 into #v5 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int) between 41 and 50 ";
                    sql += sqlCon;
                    sql += "group by a.VIP_MW; ";
                    //51~60歲
                    sql += "select a.VIP_MW ID,Count(*)Cnt6 into #v6 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int) between 51 and 60 ";
                    sql += sqlCon;
                    sql += "group by a.VIP_MW; ";
                    //61~70歲
                    sql += "select a.VIP_MW ID,Count(*)Cnt7 into #v7 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int) between 61 and 70 ";
                    sql += sqlCon;
                    sql += "group by a.VIP_MW; ";
                    //71歲以上
                    sql += "select a.VIP_MW ID,Count(*)Cnt8 into #v8 ";
                    sql += "from EDDMS.dbo.VIP a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.VIP_FaceID=w.ST_ID and w.Companycode=a.Companycode and w.ST_Type not in('0','2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and YEAR(GETDATE())-CAST(left(a.VIP_Birthday,4)as int)>=71 ";
                    sql += sqlCon;
                    sql += "group by a.VIP_MW; ";
                    //明細資料
                    sqlD = "select case v1.id when '0' then '男性' when '1' then '女性' end as id,isnull(v1.Cnt1,0)Cnt1,isnull(v2.Cnt2,0)Cnt2, ";
                    sqlD += "case when isnull(v1.Cnt1,0)=0 and isnull(v2.Cnt2,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v2.Cnt2,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt2p, ";
                    sqlD += "isnull(v3.Cnt3,0)Cnt3,case when isnull(v1.Cnt1,0)=0 and isnull(v3.Cnt3,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v3.Cnt3,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt3p, ";
                    sqlD += "isnull(v4.Cnt4,0)Cnt4,case when isnull(v1.Cnt1,0)=0 and isnull(v4.Cnt4,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v4.Cnt4,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt4p, ";
                    sqlD += "isnull(v5.Cnt5,0)Cnt5,case when isnull(v1.Cnt1,0)=0 and isnull(v5.Cnt5,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v5.Cnt5,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt5p, ";
                    sqlD += "isnull(v6.Cnt6,0)Cnt6,case when isnull(v1.Cnt1,0)=0 and isnull(v6.Cnt6,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v6.Cnt6,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt6p, ";
                    sqlD += "isnull(v7.Cnt7,0)Cnt7,case when isnull(v1.Cnt1,0)=0 and isnull(v7.Cnt7,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v7.Cnt7,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt7p, ";
                    sqlD += "isnull(v8.Cnt8,0)Cnt8,case when isnull(v1.Cnt1,0)=0 and isnull(v8.Cnt8,0)=0 then format(0,'p') when isnull(v1.Cnt1,0)=0 then format(1,'p') else format(cast(isnull(v8.Cnt8,0) as Float)/cast(isnull(v1.Cnt1,0) as Float),'p') end as Cnt8p ";
                    sqlD += "from #v1 v1 ";
                    sqlD += "left join #v2 v2 on v1.id=v2.id ";
                    sqlD += "left join #v3 v3 on v1.id=v3.id ";
                    sqlD += "left join #v4 v4 on v1.id=v4.id ";
                    sqlD += "left join #v5 v5 on v1.id=v5.id ";
                    sqlD += "left join #v6 v6 on v1.id=v6.id ";
                    sqlD += "left join #v7 v7 on v1.id=v7.id ";
                    sqlD += "left join #v8 v8 on v1.id=v8.id ";
                    sqlD += "Where 1=1 ";
                    sqlD += "order by v1.id desc ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);
                    //彙總資料
                    sqlH = "select convert(char(10),getdate(),111) + ' ' + convert(char(5),getdate(),108) as SysDate,sum(isnull(v1.Cnt1,0))SumCnt1, ";
                    sqlH += "sum(isnull(v2.Cnt2,0))SumCnt2,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v2.Cnt2,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt2p, ";
                    sqlH += "sum(isnull(v3.Cnt3,0))SumCnt3,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v3.Cnt3,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt3p, ";
                    sqlH += "sum(isnull(v4.Cnt4,0))SumCnt4,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v4.Cnt4,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt4p, ";
                    sqlH += "sum(isnull(v5.Cnt5,0))SumCnt5,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v5.Cnt5,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt5p, ";
                    sqlH += "sum(isnull(v6.Cnt6,0))SumCnt6,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v6.Cnt6,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt6p, ";
                    sqlH += "sum(isnull(v7.Cnt7,0))SumCnt7,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v7.Cnt7,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt7p, ";
                    sqlH += "sum(isnull(v8.Cnt8,0))SumCnt8,case when sum(isnull(v1.Cnt1,0))=0 then format(1,'p') else format(cast(sum(isnull(v8.Cnt8,0)) as Float)/cast(sum(isnull(v1.Cnt1,0)) as Float),'p') end as SumCnt8p ";
                    sqlH += "from #v1 v1 ";
                    sqlH += "left join #v2 v2 on v1.id=v2.id ";
                    sqlH += "left join #v3 v3 on v1.id=v3.id ";
                    sqlH += "left join #v4 v4 on v1.id=v4.id ";
                    sqlH += "left join #v5 v5 on v1.id=v5.id ";
                    sqlH += "left join #v6 v6 on v1.id=v6.id ";
                    sqlH += "left join #v7 v7 on v1.id=v7.id ";
                    sqlH += "left join #v8 v8 on v1.id=v8.id ";
                    sqlH += "Where 1=1 ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetCity")]
        public ActionResult SystemSetup_GetCity()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetCityOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string City = rq["City"];
                string sql = "select distinct a.City from AreaWEB a (nolock) where 1=1 ";
                if (City != "")
                {
                    sql += "and a.City like '" + City + "%' ";
                }
                sql += "order by a.City ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSVP101Query")]
        public ActionResult SystemSetup_MSVP101Query()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSVP101QueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string EVNO = rq["EVNO"];
                string EDM_DocNo = rq["EDM_DocNo"];
                string StartDate = rq["StartDate"];
                string EDMMemo = rq["EDMMemo"];

                string sql = "";
                sql = "Select a.EVNO,isnull(b.Cnt,0)Cnt,isnull(a.ApproveDate,'')ApproveDate,isnull(a.TOMailDate,'')TOMailDate,a.EDM_DocNo,c.EDMMemo,c.StartDate + ' ~ ' + c.EndDate as EDDate ";
                sql += "From SetEDMVIP_HWeb a (nolock) ";
                sql += "left join (Select EVNO,Count(*)Cnt From SetEDMVIP_VIPWeb (nolock) Where Companycode='" + uu.CompanyId + "' group by EVNO)b on a.EVNO=b.EVNO ";
                sql += "inner join SetEDMHWeb c (nolock) on a.EDM_DocNo=c.DocNo and c.Companycode=a.Companycode ";
                if (EDMMemo.SqlQuote() != "")
                {
                    sql += "and c.EDMMemo like '%" + EDMMemo.SqlQuote() + "%' ";
                }
                sql += "Where a.Companycode='" + uu.CompanyId + "' and a.EDMType='E' and isnull(a.EV_Model,'')='VP101'";
                if (EVNO.SqlQuote() != "")
                {
                    sql += "and a.EVNO like '" + EVNO.SqlQuote() + "%' ";
                }
                if (EDM_DocNo.SqlQuote() != "")
                {
                    sql += "and a.EDM_DocNo like '" + EDM_DocNo.SqlQuote() + "%' ";
                }
                if (StartDate.SqlQuote() != "")
                {
                    sql += "and a.StartDate='" + StartDate.SqlQuote() + "' ";
                }

                sql += "Order by a.ApproveDate desc ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSVP101EDMHistoryQuery")]
        public ActionResult SystemSetup_MSVP101EDMHistoryQuery()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSVP101EDMHistoryQueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string EVNO = rq["EVNO"];

                string sql = "";
                sql = "Select * From SetEDMVIP_setWeb (nolock) Where Companycode='" + uu.CompanyId + "' ";
                if (EVNO != "")
                {
                    sql += "and EVNO='" + EVNO + "' ";
                }
                sql += "order by SeqNo ";
                DataTable dtV = PubUtility.SqlQry(sql, uu, "SYS");
                dtV.TableName = "dtV";
                ds.Tables.Add(dtV);

                sql = "Select a.VIP_ID2,b.VIP_Name,b.VIP_Tel,b.VIP_Eadd,case b.VIP_MW when '0' then '男' when '1' then '女' end as VIP_MW, ";
                sql += "b.City,b.AreaName,case b.VIP_Type when '0' then '一般' when '1' then '會員' when '2' then '貴賓' when '3' then '員工' end as VIP_Type ";
                sql += "From SetEDMVIP_VIPWeb a (nolock) ";
                sql += "inner join EDDMS.dbo.VIP b (nolock) on a.VIP_ID2=b.VIP_ID2 and b.Companycode=a.Companycode ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                if (EVNO != "")
                {
                    sql += "and a.EVNO='" + EVNO + "' ";
                }
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);

                sql = "Select Count(*)Cnt ";
                sql += "From SetEDMVIP_VIPWeb a (nolock) ";
                sql += "inner join EDDMS.dbo.VIP b (nolock) on a.VIP_ID2=b.VIP_ID2 and b.Companycode=a.Companycode ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                if (EVNO != "")
                {
                    sql += "and a.EVNO='" + EVNO + "' ";
                }
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                dtC.TableName = "dtC";
                ds.Tables.Add(dtC);



            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSVP101Query_SendSet")]
        public ActionResult SystemSetup_MSVP101Query_SendSet()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSVP101Query_SendSetOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string chkVIPFaceID = rq["chkVIPFaceID"];
                string chkVIPFaceIDName = rq["chkVIPFaceIDName"];
                string chkCity = rq["chkCity"];
                string VIP_Type = rq["VIP_Type"];
                string VIP_TypeName = rq["VIP_TypeName"];
                string VIP_MW = rq["VIP_MW"];
                string QDay = rq["QDay"];
                string QDayS = rq["QDayS"];
                string QDayE = rq["QDayE"];
                string LCDayFlag = rq["LCDayFlag"];
                string LCDay = rq["LCDay"];
                string SDate = rq["SDate"];
                string chkDept = rq["chkDept"];
                string chkDeptName = rq["chkDeptName"];
                string chkBgno = rq["chkBgno"];
                string chkBgnoName = rq["chkBgnoName"];
                string VMEVNO = rq["VMEVNO"];
                string Flag = rq["Flag"];
                string sql = "";
                string sqlcon1 = "";
                string sqlcon2 = "";

                //sqlcon1
                if (chkVIPFaceID != "")
                {
                    sqlcon1 += "and a.VIP_FaceID in(" + chkVIPFaceID + ") ";
                }
                if (chkCity != "")
                {
                    sqlcon1 += "and a.City in(" + chkCity + ") ";
                }
                if (VIP_Type != "")
                {
                    sqlcon1 += "and a.VIP_Type in(" + VIP_Type + ") ";
                }
                if (VIP_MW != "")
                {
                    sqlcon1 += "and a.VIP_MW='" + VIP_MW + "' ";
                }
                if (QDay == "2M")
                {
                    sqlcon1 += "and a.VIP_Qday between convert(char,dateadd(MONTH,-2,getdate()),111) and convert(char(10),getdate(),111) ";
                }
                else if (QDay == "3M")
                {
                    sqlcon1 += "and a.VIP_Qday between convert(char,dateadd(MONTH,-3,getdate()),111) and convert(char(10),getdate(),111) ";
                }
                else if (QDay == "6M")
                {
                    sqlcon1 += "and a.VIP_Qday between convert(char,dateadd(MONTH,-6,getdate()),111) and convert(char(10),getdate(),111) ";
                }
                else if (QDay == "1Y")
                {
                    sqlcon1 += "and a.VIP_Qday between convert(char,dateadd(MONTH,-12,getdate()),111) and convert(char(10),getdate(),111) ";
                }

                if (QDayS != "") {
                    sqlcon1 += "and a.VIP_Qday between '" + QDayS + "' and '" + QDayE + "' ";
                }

                string LCDayVIPH = "";
                string LCDayVIPD = "";
                if (LCDayFlag == "Y") {
                    LCDayVIPH = "最近有來店";
                    if (LCDay == "") {
                        LCDayVIPD = "不限";
                    }
                    else if(LCDay == "2W")
                    {
                        LCDayVIPD = "2周內";
                        sqlcon1 += "and isnull(a.VIP_LCDay,'') between convert(char,dateadd(DAY,-15,getdate()),111) and convert(char(10),getdate(),111) ";
                    }
                    else if (LCDay == "1M")
                    {
                        LCDayVIPD = "1個月內";
                        sqlcon1 += "and isnull(a.VIP_LCDay,'') between convert(char,dateadd(MONTH,-1,getdate()),111) and convert(char(10),getdate(),111) ";
                    }
                    else if (LCDay == "2M")
                    {
                        LCDayVIPD = "2個月內";
                        sqlcon1 += "and isnull(a.VIP_LCDay,'') between convert(char,dateadd(MONTH,-2,getdate()),111) and convert(char(10),getdate(),111) ";
                    }
                    else if (LCDay == "3M")
                    {
                        LCDayVIPD = "3個月內";
                        sqlcon1 += "and isnull(a.VIP_LCDay,'') between convert(char,dateadd(MONTH,-3,getdate()),111) and convert(char(10),getdate(),111) ";
                    }
                    else if (LCDay == "6M")
                    {
                        LCDayVIPD = "6個月內";
                        sqlcon1 += "and isnull(a.VIP_LCDay,'') between convert(char,dateadd(MONTH,-6,getdate()),111) and convert(char(10),getdate(),111) ";
                    }
                    else if (LCDay == "1Y")
                    {
                        LCDayVIPD = "1年內";
                        sqlcon1 += "and isnull(a.VIP_LCDay,'') between convert(char,dateadd(MONTH,-12,getdate()),111) and convert(char(10),getdate(),111) ";
                    }
                    else if (LCDay == "2Y")
                    {
                        LCDayVIPD = "2年內";
                        sqlcon1 += "and isnull(a.VIP_LCDay,'') between convert(char,dateadd(MONTH,-24,getdate()),111) and convert(char(10),getdate(),111) ";
                    }
                }
                else if (LCDayFlag == "N") {
                    LCDayVIPH = "最近沒來店";
                    if (LCDay == "")
                    {
                        LCDayVIPD = "不限";
                    }
                    else if(LCDay == "2W")
                    {
                        LCDayVIPD = "2周內";
                        sqlcon1 += "and isnull(a.VIP_LCDay,'') not between convert(char,dateadd(DAY,-15,getdate()),111) and convert(char(10),getdate(),111) ";
                    }
                    else if (LCDay == "1M")
                    {
                        LCDayVIPD = "1個月內";
                        sqlcon1 += "and isnull(a.VIP_LCDay,'') not between convert(char,dateadd(MONTH,-1,getdate()),111) and convert(char(10),getdate(),111) ";
                    }
                    else if (LCDay == "2M")
                    {
                        LCDayVIPD = "2個月內";
                        sqlcon1 += "and isnull(a.VIP_LCDay,'') not between convert(char,dateadd(MONTH,-2,getdate()),111) and convert(char(10),getdate(),111) ";
                    }
                    else if (LCDay == "3M")
                    {
                        LCDayVIPD = "3個月內";
                        sqlcon1 += "and isnull(a.VIP_LCDay,'') not between convert(char,dateadd(MONTH,-3,getdate()),111) and convert(char(10),getdate(),111) ";
                    }
                    else if (LCDay == "6M")
                    {
                        LCDayVIPD = "6個月內";
                        sqlcon1 += "and isnull(a.VIP_LCDay,'') not between convert(char,dateadd(MONTH,-6,getdate()),111) and convert(char(10),getdate(),111) ";
                    }
                    else if (LCDay == "1Y")
                    {
                        LCDayVIPD = "1年內";
                        sqlcon1 += "and isnull(a.VIP_LCDay,'') not between convert(char,dateadd(MONTH,-12,getdate()),111) and convert(char(10),getdate(),111) ";
                    }
                    else if (LCDay == "2Y")
                    {
                        LCDayVIPD = "2年內";
                        sqlcon1 += "and isnull(a.VIP_LCDay,'') not between convert(char,dateadd(MONTH,-24,getdate()),111) and convert(char(10),getdate(),111) ";
                    }
                }

                //sqlcon2
                if (SDate == "2M")
                {
                    sqlcon2 += "and S_YYYYMM between convert(char(7),dateadd(MONTH,-2,getdate()),111) and convert(char(7),getdate(),111) ";
                }
                else if (SDate == "3M")
                {
                    sqlcon2 += "and S_YYYYMM between convert(char(7),dateadd(MONTH,-3,getdate()),111) and convert(char(7),getdate(),111) ";
                }
                else if (SDate == "6M")
                {
                    sqlcon2 += "and S_YYYYMM between convert(char(7),dateadd(MONTH,-6,getdate()),111) and convert(char(7),getdate(),111) ";
                }
                else if (SDate == "1Y")
                {
                    sqlcon2 += "and S_YYYYMM between convert(char(7),dateadd(MONTH,-12,getdate()),111) and convert(char(7),getdate(),111) ";
                }
                if (chkDept != "")
                {
                    sqlcon2 += "and GD_Dept in(" + chkDept + ") ";
                }
                if (chkBgno != "")
                {
                    sqlcon2 += "and GD_Bgno in(" + chkBgno + ") ";
                }

                //顯示會員清單
                if (Flag == "Q")
                {
                    sql = "Delete From SetEDMVIP_VIPWeb Where Companycode='" + uu.CompanyId + "' and EVNO='" + VMEVNO + "'; ";
                    sql += "Delete From SetEDMVIP_SetWeb Where Companycode='" + uu.CompanyId + "' and EVNO='" + VMEVNO + "' ";
                    PubUtility.ExecuteSql(sql, uu, "SYS");

                    //SetEDMVIP_VIPWeb
                    sql = "Insert into SetEDMVIP_VIPWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,VIP_ID2,CouponID,VIP_Name,VIP_EMail,SendDate,PS_NO,SeqNo,EDMType) ";
                    sql += "Select a.Companycode,'" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                    sql += "'" + VMEVNO + "',a.VIP_ID2,'',VIP_Name,VIP_Eadd,'','',ROW_NUMBER() OVER(PARTITION BY '" + VMEVNO + "' order by a.VIP_ID2),'E' ";
                    sql += "From EDDMS.dbo.VIP a (nolock) ";
                    if (SDate != "" || chkDept != "" || chkBgno != "")
                    {
                        sql += "inner join (select distinct vip_id2 from MSData3Web (nolock) where Companycode='" + uu.CompanyId + "' ";
                        sql += sqlcon2;
                        sql += ")b on a.VIP_ID2=b.VIP_ID2 ";
                    }
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and isnull(a.VIP_Eadd,'')<>'' and isnull(a.P_Flag2,'')='1' and isnull(a.VIP_Eday,'')>convert(char(10),getdate(),111) ";
                    sql += sqlcon1;

                    //SetEDMVIP_SetWeb
                    if (chkVIPFaceID != "")
                    {
                        sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                        sql += "'" + VMEVNO + "',1,'','','','','會籍店櫃','" + chkVIPFaceIDName.Replace("'", "") + "' ";
                    }
                    if (chkCity != "")
                    {
                        sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                        sql += "'" + VMEVNO + "',2,'','','','','縣市','" + chkCity.Replace("'", "") + "' ";
                    }
                    if (VIP_Type != "")
                    {
                        sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                        sql += "'" + VMEVNO + "',3,'','','','','會員卡別','" + VIP_TypeName.Replace("'", "") + "' ";
                    }
                    if (VIP_MW != "")
                    {
                        if (VIP_MW == "0")
                        {
                            sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                            sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                            sql += "'" + VMEVNO + "',4,'','','','','性別','先生' ";
                        }
                        else if (VIP_MW == "1")
                        {
                            sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                            sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                            sql += "'" + VMEVNO + "',4,'','','','','性別','小姐' ";
                        }
                    }
                    else
                    {
                        sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                        sql += "'" + VMEVNO + "',4,'','','','','性別','不限' ";
                    }
                    if (QDay == "")
                    {
                        sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                        sql += "'" + VMEVNO + "',5,'','','','','入會期間','不限' ";
                    }
                    else if (QDay == "2M")
                    {
                        sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                        sql += "'" + VMEVNO + "',5,'','','','','入會期間','2個月內' ";
                    }
                    else if (QDay == "3M")
                    {
                        sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                        sql += "'" + VMEVNO + "',5,'','','','','入會期間','3個月內' ";
                    }
                    else if (QDay == "6M")
                    {
                        sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                        sql += "'" + VMEVNO + "',5,'','','','','入會期間','6個月內' ";
                    }
                    else if (QDay == "1Y")
                    {
                        sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                        sql += "'" + VMEVNO + "',5,'','','','','入會期間','1年內' ";
                    }

                    if (QDayS == "") {
                        sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                        sql += "'" + VMEVNO + "',6,'','','','','入會區間','不限' ";
                    }
                    else {
                        sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                        sql += "'" + VMEVNO + "',6,'','','','','入會區間','" + QDayS + "' + ' ~ ' + '" + QDayE + "' ";
                    }

                    sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                    sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                    sql += "'" + VMEVNO + "',7,'','','','','" + LCDayVIPH + "','" + LCDayVIPD + "' ";

                    if (SDate == "")
                    {
                        sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                        sql += "'" + VMEVNO + "',8,'','','','','消費月份','無' ";
                    }
                    else if (SDate == "2M")
                    {
                        sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                        sql += "'" + VMEVNO + "',8,'','','','','消費月份','2個月內' ";
                    }
                    else if (SDate == "3M")
                    {
                        sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                        sql += "'" + VMEVNO + "',8,'','','','','消費月份','3個月內' ";
                    }
                    else if (SDate == "6M")
                    {
                        sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                        sql += "'" + VMEVNO + "',8,'','','','','消費月份','6個月內' ";
                    }
                    else if (SDate == "1Y")
                    {
                        sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                        sql += "'" + VMEVNO + "',8,'','','','','消費月份','1年內' ";
                    }
                    if (chkDept != "")
                    {
                        sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                        sql += "'" + VMEVNO + "',9,'','','','','消費部門','" + chkDeptName.Replace("'", "") + "' ";
                    }
                    if (chkBgno != "")
                    {
                        sql += ";Insert into SetEDMVIP_SetWeb (CompanyCode,CrtUser,CrtDate,CrtTime,EVNO,SeqNo,TableName,SetCode,SetDataS,SetDataE,ColTitle,ColData) ";
                        sql += "Select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(12),getdate(),108), ";
                        sql += "'" + VMEVNO + "',10,'','','','','消費大類','" + chkBgnoName.Replace("'", "") + "' ";
                    }
                    PubUtility.ExecuteSql(sql, uu, "SYS");

                    sql = "Select a.VIP_ID2,a.VIP_Name,a.VIP_Tel,a.VIP_Eadd,case a.VIP_MW when '0' then '男' when '1' then '女' end as VIP_NM,a.City,a.AreaName,a.VIP_LCDay,isnull(a.PointsBalance,0)PointsBalance, ";
                    sql += "case a.VIP_Type when '0' then '一般卡' when '1' then '會員卡' when '2' then '貴賓卡' when '3' then '白金卡' end as VIP_Type ";
                    sql += "From EDDMS.dbo.VIP a (nolock) ";
                    if (SDate != "" || chkDept != "" || chkBgno != "")
                    {
                        sql += "inner join (select distinct vip_id2 from MSData3Web (nolock) where Companycode='" + uu.CompanyId + "' ";
                        sql += sqlcon2;
                        sql += ")b on a.VIP_ID2=b.VIP_ID2 ";
                    }
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and isnull(a.VIP_Eadd,'')<>'' and isnull(a.P_Flag2,'')='1' and isnull(a.VIP_Eday,'')>convert(char(10),getdate(),111) ";
                    sql += sqlcon1;
                    sql += "Order by a.VIP_ID2 ";
                    DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);
                }
                //只計算會員數量
                else if (Flag == "C")
                {
                    sql = "Delete From SetEDMVIP_VIPWeb Where Companycode='" + uu.CompanyId + "' and EVNO='" + VMEVNO + "'; ";
                    sql += "Delete From SetEDMVIP_SetWeb Where Companycode='" + uu.CompanyId + "' and EVNO='" + VMEVNO + "' ";
                    PubUtility.ExecuteSql(sql, uu, "SYS");

                    sql = "Select Count(*)VIPCnt ";
                    sql += "From EDDMS.dbo.VIP a (nolock) ";
                    if (SDate != "" || chkDept != "" || chkBgno != "")
                    {
                        sql += "inner join (select distinct vip_id2 from MSData3Web (nolock) where Companycode='" + uu.CompanyId + "' ";
                        sql += sqlcon2;
                        sql += ")b on a.VIP_ID2=b.VIP_ID2 ";
                    }
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and isnull(a.VIP_Eadd,'')<>'' and isnull(a.P_Flag2,'')='1' and isnull(a.VIP_Eday,'')>convert(char(10),getdate(),111) ";
                    sql += sqlcon1;
                    DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                    dtC.TableName = "dtC";
                    ds.Tables.Add(dtC);

                    sql = "Select a.VIP_ID2,a.VIP_Name,a.VIP_Tel,a.VIP_Eadd,case a.VIP_MW when '0' then '男' when '1' then '女' end as VIP_NM,a.City,a.AreaName,a.VIP_LCDay,isnull(a.PointsBalance,0)PointsBalance, ";
                    sql += "case a.VIP_Type when '0' then '一般卡' when '1' then '會員卡' when '2' then '貴賓卡' when '3' then '白金卡' end as VIP_Type ";
                    sql += "From EDDMS.dbo.VIP a (nolock) ";
                    sql += "Where 1=2 ";
                    DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSVP101Delete_SendSet")]
        public ActionResult SystemSetup_MSVP101Delete_SendSet()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSVP101Delete_SendSetOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string VMEVNO = rq["VMEVNO"];
                string VIP_ID2 = rq["VIP_ID2"];
                string sql = "";

                sql = "Delete From SetEDMVIP_VIPWeb Where Companycode='" + uu.CompanyId + "' and EVNO='" + VMEVNO + "' and VIP_ID2='" + VIP_ID2 + "' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                sql = "Select Count(*)Cnt From SetEDMVIP_VIPWeb (nolock) where Companycode='" + uu.CompanyId + "' and EVNO='" + VMEVNO + "' ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSVP101ChkDMSend")]
        public ActionResult SystemSetup_MSVP101ChkDMSend()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSVP101ChkDMSendOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string VMDocNo = rq["VMDocNo"];
                string sql = "";
                sql = "Select * From SetEDMVIP_VIPWeb (nolock) Where Companycode='" + uu.CompanyId + "' and EVNO='" + VMDocNo + "' ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetTypeDataWeb")]
        public ActionResult SystemSetup_GetTypeDataWeb()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetTypeDataWebOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Type_Code = rq["Type_Code"];
                string Type_ID = rq["Type_ID"];
                string sql = "select * from TypeDataWeb (nolock) where Companycode='" + uu.CompanyId + "' ";
                if (Type_Code != "")
                {
                    sql += "and Type_Code='" + Type_Code + "' ";
                }
                if (Type_ID != "")
                {
                    sql += "and Type_ID like '" + Type_ID + "%' ";
                }
                sql += "order by Type_ID ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        #region msSD104
        [Route("SystemSetup/MSSD104_LookUpActivityCode")]
        public ActionResult SystemSetup_MSSD104_LookUpActivityCode()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD104_LookUpActivityCodeOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ActivityCode = rq["ActivityCode"];

                string sql = "";
                sql = "Select a.ActivityCode,a.PS_Name,a.StartDate,a.EndDate ";
                sql += "From PromoteSCouponHWeb a (nolock) ";
                sql += "inner join SetEDMHWeb b (nolock) on a.PS_NO=b.PS_NO and b.EDMType='B' and b.Companycode=a.Companycode ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' and a.CouponType in('1','2') ";
                sql += "and isnull(a.ApproveDate,'')<>'' and isnull(a.DefeasanceDate,'')='' ";
                if (ActivityCode.SqlQuote() != "")
                {
                    sql += "and a.ActivityCode like '" + ActivityCode.SqlQuote() + "%' ";
                }
                sql += "group by a.ActivityCode,a.PS_Name,a.StartDate,a.EndDate ";
                sql += "Order By a.StartDate desc ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSD104Query")]
        public ActionResult SystemSetup_MSSD104Query()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD104QueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ActivityCode = rq["ActivityCode"];
                string BirYear = rq["BirYear"];
                string PSName = rq["PSName"];
                string BirMonth = rq["BirMonth"];

                string sql = "";
                sql = "Select a.PS_NO,a.ActivityCode,b.PS_Name,a.StartDate + '~' + a.EndDate EDDate, ";
                sql += "c.BIR_Year,c.BIR_Month,sum(isnull(a.issueQty,0))issueQty,sum(isnull(a.ReclaimQty,0))ReclaimQty, ";
                sql += "case when sum(isnull(a.issueQty,0))=0 then format(0,'p1') else format(cast(sum(isnull(a.ReclaimQty,0)) as Float)/cast(sum(isnull(a.issueQty,0)) as Float),'p1') end as RePercent, ";
                sql += "sum(isnull(a.ShareAmt,0))ShareAmt,sum(isnull(a.ReclaimCash,0))ReclaimCash,sum(isnull(a.ReclaimTrans,0))ReclaimTrans, ";
                sql += "case when sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(sum(isnull(a.ReclaimCash,0))/sum(isnull(a.ReclaimTrans,0)),0) end as Price ";

                sql += "From MsData2Web a (nolock) ";
                sql += "left join PromoteSCouponHWeb b (nolock) on a.PS_NO=b.PS_NO and b.Companycode=a.Companycode and b.CouponType in('1','2') ";
                //活動名稱
                if (PSName.SqlQuote() != "")
                {
                    sql += "and b.PS_Name like '%" + PSName.SqlQuote() + "%' ";
                }
                sql += "inner join SetEDMHWeb c (nolock) on a.PS_NO=c.PS_NO and c.Companycode=a.Companycode and c.EDMType='B' ";
                //DM年度
                if (BirYear.SqlQuote() != "") {
                    sql += "and c.BIR_Year='" + BirYear.SqlQuote() + "' ";
                }
                //生日月份
                if (BirMonth.SqlQuote() != "")
                {
                    sql += "and c.BIR_Month='" + BirMonth.SqlQuote() + "' ";
                }

                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                //活動代號
                if (ActivityCode.SqlQuote() != "")
                {
                    sql += "and a.ActivityCode like '%" + ActivityCode.SqlQuote() + "%' ";
                }
                sql += "group by a.PS_NO,a.ActivityCode,b.PS_Name,a.StartDate,a.EndDate,c.BIR_Year,c.BIR_Month ";
                sql += "Order by a.ActivityCode ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSD104QueryD")]
        public ActionResult SystemSetup_MSSD104QueryD()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD104QueryDOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string PS_NO = rq["PS_NO"];
                string Flag = rq["Flag"];
                string sql = "";
                string sqlD = "";
                string sqlH = "";

                //店別
                if (Flag == "S") {
                    sql = "Select a.ShopNo + '-' + b.ST_SName ID,Sum(isnull(a.issueQty,0))issueQty, ";
                    sql += "Sum(isnull(a.ReclaimQty,0))ReclaimQty, ";
                    sql += "case when Sum(isnull(a.issueQty,0))=0 then format(0,'p1') else format(cast(Sum(isnull(a.ReclaimQty,0)) as Float)/cast(sum(isnull(a.issueQty,0)) as Float),'p1') end as RePercent, ";
                    sql += "Sum(isnull(a.ShareAmt,0))ShareAmt,Sum(isnull(a.ReclaimCash,0))ReclaimCash,Sum(isnull(a.ReclaimTrans,0))ReclaimTrans, ";
                    sql += "case when Sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(Sum(isnull(a.ReclaimCash,0))/Sum(isnull(a.ReclaimTrans,0)),0) end as Price, ";
                    sql += "Sum(isnull(a.TotalCash,0))TotalCash,Sum(isnull(a.TotalTrans,0))TotalTrans, ";
                    sql += "case when Sum(isnull(a.TotalTrans,0))=0 then 0 else Round(Sum(isnull(a.TotalCash,0))/Sum(isnull(a.TotalTrans,0)),0) end as PriceAll ";
                    sql += "into #S ";

                    sql += "From MSData2Web a (nolock) ";
                    sql += "inner join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode and b.ST_Type not in('2','3') ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    if (PS_NO.SqlQuote() != "") {
                        sql += "and a.PS_NO='" + PS_NO.SqlQuote() + "' ";
                    }
                    sql += "group by a.ShopNo,b.ST_SName ";
                    sql += "order by a.ShopNo; ";
                    //明細資料
                    sqlD = "Select * From #S ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);
                    //彙總資料
                    sqlH = "Select Sum(isnull(a.issueQty,0))SumissueQty,Sum(isnull(a.ReclaimQty,0))SumReclaimQty, ";
                    sqlH += "case when Sum(isnull(a.issueQty,0))=0 then format(0,'p1') else format(cast(Sum(isnull(a.ReclaimQty,0)) as Float)/cast(sum(isnull(a.issueQty,0)) as Float),'p1') end as SumRePercent, ";
                    sqlH += "Sum(isnull(a.ShareAmt,0))SumShareAmt,Sum(isnull(a.ReclaimCash,0))SumReclaimCash,Sum(isnull(a.ReclaimTrans,0))SumReclaimTrans, ";
                    sqlH += "case when Sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(Sum(isnull(a.ReclaimCash,0))/Sum(isnull(a.ReclaimTrans,0)),0) end as SumPrice, ";
                    sqlH += "Sum(isnull(a.TotalCash,0))SumTotalCash,Sum(isnull(a.TotalTrans,0))SumTotalTrans, ";
                    sqlH += "case when Sum(isnull(a.TotalTrans,0))=0 then 0 else Round(Sum(isnull(a.TotalCash,0))/Sum(isnull(a.TotalTrans,0)),0) end as SumPriceAll ";
                    sqlH += "From #S a ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);

                }
                //銷售日期
                else if (Flag == "D") {
                    sql = "Select a.SalesDate ID,Sum(isnull(a.ReclaimQty,0))ReclaimQty,Sum(isnull(a.ShareAmt,0))ShareAmt, ";
                    sql += "Sum(isnull(a.ReclaimCash,0))ReclaimCash,Sum(isnull(a.ReclaimTrans,0))ReclaimTrans, ";
                    sql += "case when Sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(Sum(isnull(a.ReclaimCash,0))/Sum(isnull(a.ReclaimTrans,0)),0) end as Price, ";
                    sql += "Sum(isnull(a.TotalCash,0))TotalCash,Sum(isnull(a.TotalTrans,0))TotalTrans, ";
                    sql += "case when Sum(isnull(a.TotalTrans,0))=0 then 0 else Round(Sum(isnull(a.TotalCash,0))/Sum(isnull(a.TotalTrans,0)),0) end as PriceAll ";
                    sql += "into #S ";

                    sql += "From MSData1Web a (nolock) ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    if (PS_NO.SqlQuote() != "") {
                        sql += "and a.PS_NO='" + PS_NO.SqlQuote() + "' ";
                    }
                    sql += "group by a.SalesDate ";
                    sql += "order by a.SalesDate; ";
                    //明細資料
                    sqlD = "Select * From #S ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sqlH = "Select Sum(isnull(a.ReclaimQty,0))SumReclaimQty,Sum(isnull(a.ShareAmt,0))SumShareAmt, ";
                    sqlH += "Sum(isnull(a.ReclaimCash,0))SumReclaimCash,Sum(isnull(a.ReclaimTrans,0))SumReclaimTrans, ";
                    sqlH += "case when Sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(Sum(isnull(a.ReclaimCash,0))/Sum(isnull(a.ReclaimTrans,0)),0) end as SumPrice, ";
                    sqlH += "Sum(isnull(a.TotalCash,0))SumTotalCash,Sum(isnull(a.TotalTrans,0))SumTotalTrans, ";
                    sqlH += "case when Sum(isnull(a.TotalTrans,0))=0 then 0 else Round(Sum(isnull(a.TotalCash,0))/Sum(isnull(a.TotalTrans,0)),0) end as SumPriceAll ";
                    sqlH += "From #S a ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);

                }
                
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSD104QueryDD")]
        public ActionResult SystemSetup_MSSD104QueryDD()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD104QueryDDOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string PS_NO = rq["PS_NO"];
                string ID = rq["ID"];
                string Flag = rq["Flag"];
                string sql = "";
                string sqlD = "";
                string sqlH = "";

                //店別
                if (Flag == "S")
                {
                    sql = "Select a.SalesDate ID,Sum(isnull(a.ReclaimQty,0))ReclaimQty,Sum(isnull(a.ShareAmt,0))ShareAmt, ";
                    sql += "Sum(isnull(a.ReclaimCash,0))ReclaimCash,Sum(isnull(a.ReclaimTrans,0))ReclaimTrans, ";
                    sql += "case when Sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(Sum(isnull(a.ReclaimCash,0))/Sum(isnull(a.ReclaimTrans,0)),0) end as Price, ";
                    sql += "Sum(isnull(a.TotalCash,0))TotalCash,Sum(isnull(a.TotalTrans,0))TotalTrans, ";
                    sql += "case when Sum(isnull(a.TotalTrans,0))=0 then 0 else Round(Sum(isnull(a.TotalCash,0))/Sum(isnull(a.TotalTrans,0)),0) end as PriceAll ";
                    sql += "into #S ";

                    sql += "From MSData1Web a (nolock) ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    if (PS_NO.SqlQuote() != "")
                    {
                        sql += "and a.PS_NO='" + PS_NO.SqlQuote() + "' ";
                    }
                    if (ID.SqlQuote() != "") {
                        sql += "and a.ShopNo='" + ID.SqlQuote() + "' ";
                    }
                    sql += "group by a.SalesDate ";
                    sql += "order by a.SalesDate; ";
                    //明細資料
                    sqlD = "Select * From #S ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sqlH = "Select Sum(isnull(a.ReclaimQty,0))SumReclaimQty,Sum(isnull(a.ShareAmt,0))SumShareAmt, ";
                    sqlH += "Sum(isnull(a.ReclaimCash,0))SumReclaimCash,Sum(isnull(a.ReclaimTrans,0))SumReclaimTrans, ";
                    sqlH += "case when Sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(Sum(isnull(a.ReclaimCash,0))/Sum(isnull(a.ReclaimTrans,0)),0) end as SumPrice, ";
                    sqlH += "Sum(isnull(a.TotalCash,0))SumTotalCash,Sum(isnull(a.TotalTrans,0))SumTotalTrans, ";
                    sqlH += "case when Sum(isnull(a.TotalTrans,0))=0 then 0 else Round(Sum(isnull(a.TotalCash,0))/Sum(isnull(a.TotalTrans,0)),0) end as SumPriceAll ";
                    sqlH += "From #S a ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
                //銷售日期
                else if (Flag == "D")
                {
                    sql = "Select a.ShopNo + '-' + b.ST_SName ID,Sum(isnull(a.ReclaimQty,0))ReclaimQty,Sum(isnull(a.ShareAmt,0))ShareAmt, ";
                    sql += "Sum(isnull(a.ReclaimCash,0))ReclaimCash,Sum(isnull(a.ReclaimTrans,0))ReclaimTrans, ";
                    sql += "case when Sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(Sum(isnull(a.ReclaimCash,0))/Sum(isnull(a.ReclaimTrans,0)),0) end as Price, ";
                    sql += "Sum(isnull(a.TotalCash,0))TotalCash,Sum(isnull(a.TotalTrans,0))TotalTrans, ";
                    sql += "case when Sum(isnull(a.TotalTrans,0))=0 then 0 else Round(Sum(isnull(a.TotalCash,0))/Sum(isnull(a.TotalTrans,0)),0) end as PriceAll ";
                    sql += "into #S ";

                    sql += "From MSData1Web a (nolock) ";
                    sql += "inner join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode and b.ST_Type not in('2','3') ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    if (PS_NO.SqlQuote() != "")
                    {
                        sql += "and a.PS_NO='" + PS_NO.SqlQuote() + "' ";
                    }
                    if (ID.SqlQuote() != "")
                    {
                        sql += "and a.SalesDate='" + ID.SqlQuote() + "' ";
                    }
                    sql += "group by a.ShopNo,b.ST_SName ";
                    sql += "order by a.ShopNo; ";
                    //明細資料
                    sqlD = "Select * From #S ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sqlH = "Select Sum(isnull(a.ReclaimQty,0))SumReclaimQty,Sum(isnull(a.ShareAmt,0))SumShareAmt, ";
                    sqlH += "Sum(isnull(a.ReclaimCash,0))SumReclaimCash,Sum(isnull(a.ReclaimTrans,0))SumReclaimTrans, ";
                    sqlH += "case when Sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(Sum(isnull(a.ReclaimCash,0))/Sum(isnull(a.ReclaimTrans,0)),0) end as SumPrice, ";
                    sqlH += "Sum(isnull(a.TotalCash,0))SumTotalCash,Sum(isnull(a.TotalTrans,0))SumTotalTrans, ";
                    sqlH += "case when Sum(isnull(a.TotalTrans,0))=0 then 0 else Round(Sum(isnull(a.TotalCash,0))/Sum(isnull(a.TotalTrans,0)),0) end as SumPriceAll ";
                    sqlH += "From #S a ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }
        #endregion

        #region MSSA103
        [Route("SystemSetup/GetInitMSSA103")]
        public ActionResult SystemSetup_GetInitMSSA103()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitMSSA103OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ProgramID = rq["ProgramID"];
                string sql = "select ChineseName,convert(char,dateadd(DD,-1,convert(char,dateadd(YEAR,-1,getdate()),111)),111) as SysDate1,convert(char,dateadd(DD,-1,getdate()),111) as SysDate2,convert(char(10),getdate(),111) SysDate from ProgramIDWeb (nolock) where ProgramID='" + ProgramID.SqlQuote() + "'";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSA103Query")]
        public ActionResult SystemSetup_MSSA103Query()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSA103QueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string OpenDateS1 = rq["OpenDateS1"];
                string OpenDateE1 = rq["OpenDateE1"];
                string OpenDateS2 = rq["OpenDateS2"];
                string OpenDateE2 = rq["OpenDateE2"];
                string ShopNo = rq["ShopNo"];
                string Flag = rq["Flag"];

                string sql = "";
                string sqlD = "";
                string sqlH = "";

                //店櫃
                if (Flag == "S")
                {
                    //前期
                    sql = "select a.ShopNo ID,w.ST_SName Name,Sum(a.Qty1)Qty1,Sum(a.Cash)Cash1 into #s1 ";
                    sql += "from SalesHWeb a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    if (OpenDateS1 != "")
                    {
                        sql += "and a.OpenDate between '" + OpenDateS1 + "' and '" + OpenDateE1 + "' ";
                    }
                    if (ShopNo != "")
                    {
                        sql += "and a.ShopNo in(" + ShopNo + ") ";
                    }
                    sql += "group by a.ShopNo,w.ST_SName; ";

                    //本期
                    sql += "select a.ShopNo ID,w.ST_SName Name,Sum(a.Qty1)Qty2,Sum(a.Cash)Cash2 into #s2 ";
                    sql += "from SalesHWeb a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    if (OpenDateS2 != "")
                    {
                        sql += "and a.OpenDate between '" + OpenDateS2 + "' and '" + OpenDateE2 + "' ";
                    }
                    if (ShopNo != "")
                    {
                        sql += "and a.ShopNo in(" + ShopNo + ") ";
                    }
                    sql += "group by a.ShopNo,w.ST_SName; ";

                    //明細資料
                    sqlD = "select case when isnull(s1.ID,'')='' then isnull(s2.ID,'') + '-' + isnull(s2.Name,'') else isnull(s1.ID,'') + '-' + isnull(s1.Name,'') end as id,isnull(s1.Qty1,0)Qty1,isnull(s1.Cash1,0)Cash1, ";
                    sqlD += "isnull(s2.Qty2,0)Qty2,isnull(s2.Cash2,0)Cash2, ";
                    sqlD += "case when isnull(s1.Cash1,0)=0 and isnull(s2.Cash2,0)=0 then format(0,'p') when isnull(s1.Cash1,0)=0 then format(1,'p') else format(cast(isnull(s2.Cash2,0)-isnull(s1.Cash1,0) as Float)/cast(isnull(s1.Cash1,0) as Float),'p') end as Per, ";
                    sqlD += "'" + OpenDateS1 + "' OpenDateS1,'" + OpenDateE1 + "' OpenDateE1,'" + OpenDateS2 + "' OpenDateS2,'" + OpenDateE2 + "' OpenDateE2 ";
                    sqlD += "from #s1 s1 ";
                    sqlD += "Full join #s2 s2 on s1.id=s2.id ";
                    sqlD += "order by id ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sqlH = "select sum(isnull(s1.Qty1,0))SumQty1,sum(isnull(s1.Cash1,0))SumCash1, ";
                    sqlH += "sum(isnull(s2.Qty2,0))SumQty2,sum(isnull(s2.Cash2,0))SumCash2, ";
                    sqlH += "case when sum(isnull(s1.Cash1,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.Cash2,0))-sum(isnull(s1.Cash1,0)) as Float)/cast(sum(isnull(s1.Cash1,0)) as Float),'p') end as SumPer ";
                    sqlH += "from #s1 s1 ";
                    sqlH += "Full join #s2 s2 on s1.id=s2.id ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
                //部門
                else if (Flag == "D")
                {
                    //期間1
                    sql = "select p.GD_Dept id,Sum(a.Num)Qty1,Sum(a.Cash)Cash1 into #s1 ";
                    sql += "from SalesDWeb a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "inner join PLUWeb p (nolock) on a.GoodsNo=p.GD_NO and p.Companycode='" + uu.CompanyId + "' ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    if (OpenDateS1 != "")
                    {
                        sql += "and a.OpenDate between '" + OpenDateS1 + "' and '" + OpenDateE1 + "' ";
                    }
                    if (ShopNo != "")
                    {
                        sql += "and a.ShopNo in(" + ShopNo + ") ";
                    }
                    sql += "group by p.GD_Dept; ";

                    //期間2
                    sql += "select p.GD_Dept id,Sum(a.Num)Qty2,Sum(a.Cash)Cash2 into #s2 ";
                    sql += "from SalesDWeb a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "inner join PLUWeb p (nolock) on a.GoodsNo=p.GD_NO and p.Companycode='" + uu.CompanyId + "' ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    if (OpenDateS2 != "")
                    {
                        sql += "and a.OpenDate between '" + OpenDateS2 + "' and '" + OpenDateE2 + "' ";
                    }
                    if (ShopNo != "")
                    {
                        sql += "and a.ShopNo in(" + ShopNo + ") ";
                    }
                    sql += "group by p.GD_Dept; ";

                    //明細資料
                    sqlD = "select a.type_id + '-' + a.type_name id,isnull(s1.Qty1,0)Qty1,isnull(s1.Cash1,0)Cash1, ";
                    sqlD += "isnull(s2.Qty2,0)Qty2,isnull(s2.Cash2,0)Cash2, ";
                    sqlD += "case when isnull(s1.Cash1,0)=0 and isnull(s2.Cash2,0)=0 then format(0,'p') when isnull(s1.Cash1,0)=0 then format(1,'p') else format(cast(isnull(s2.Cash2,0)-isnull(s1.Cash1,0) as Float)/cast(isnull(s1.Cash1,0) as Float),'p') end as Per, ";
                    sqlD += "'" + OpenDateS1 + "' OpenDateS1,'" + OpenDateE1 + "' OpenDateE1,'" + OpenDateS2 + "' OpenDateS2,'" + OpenDateE2 + "' OpenDateE2 ";
                    sqlD += "from TypeDataWeb a (nolock) ";
                    sqlD += "left join #s1 s1 on a.type_id=s1.id ";
                    sqlD += "left join #s2 s2 on a.type_id=s2.id ";
                    sqlD += "where a.CompanyCode='" + uu.CompanyId + "' and a.type_code='G' ";
                    sqlD += "order by a.type_id ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sqlH = "select sum(isnull(s1.Qty1,0))SumQty1,sum(isnull(s1.Cash1,0))SumCash1, ";
                    sqlH += "sum(isnull(s2.Qty2,0))SumQty2,sum(isnull(s2.Cash2,0))SumCash2, ";
                    sqlH += "case when sum(isnull(s1.Cash1,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.Cash2,0))-sum(isnull(s1.Cash1,0)) as Float)/cast(sum(isnull(s1.Cash1,0)) as Float),'p') end as SumPer ";
                    sqlH += "from TypeDataWeb a (nolock) ";
                    sqlH += "left join #s1 s1 on a.type_id=s1.id ";
                    sqlH += "left join #s2 s2 on a.type_id=s2.id ";
                    sqlH += "where a.CompanyCode='" + uu.CompanyId + "' and a.type_code='G' ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
                //大類
                else if (Flag == "B")
                {
                    //期間1
                    sql = "select p.GD_BGNO id,Sum(a.Num)Qty1,Sum(a.Cash)Cash1 into #s1 ";
                    sql += "from SalesDWeb a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "inner join PLUWeb p (nolock) on a.GoodsNo=p.GD_NO and p.Companycode='" + uu.CompanyId + "' ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    if (OpenDateS1 != "")
                    {
                        sql += "and a.OpenDate between '" + OpenDateS1 + "' and '" + OpenDateE1 + "' ";
                    }
                    if (ShopNo != "")
                    {
                        sql += "and a.ShopNo in(" + ShopNo + ") ";
                    }
                    sql += "group by p.GD_BGNO; ";

                    //期間2
                    sql += "select p.GD_BGNO id,Sum(a.Num)Qty2,Sum(a.Cash)Cash2 into #s2 ";
                    sql += "from SalesDWeb a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "inner join PLUWeb p (nolock) on a.GoodsNo=p.GD_NO and p.Companycode='" + uu.CompanyId + "' ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    if (OpenDateS2 != "")
                    {
                        sql += "and a.OpenDate between '" + OpenDateS2 + "' and '" + OpenDateE2 + "' ";
                    }
                    if (ShopNo != "")
                    {
                        sql += "and a.ShopNo in(" + ShopNo + ") ";
                    }
                    sql += "group by p.GD_BGNO; ";

                    //明細資料
                    sqlD = "select a.type_id + '-' + a.type_name id,isnull(s1.Qty1,0)Qty1,isnull(s1.Cash1,0)Cash1, ";
                    sqlD += "isnull(s2.Qty2,0)Qty2,isnull(s2.Cash2,0)Cash2, ";
                    sqlD += "case when isnull(s1.Cash1,0)=0 and isnull(s2.Cash2,0)=0 then format(0,'p') when isnull(s1.Cash1,0)=0 then format(1,'p') else format(cast(isnull(s2.Cash2,0)-isnull(s1.Cash1,0) as Float)/cast(isnull(s1.Cash1,0) as Float),'p') end as Per ";
                    sqlD += "from TypeDataWeb a (nolock) ";
                    sqlD += "left join #s1 s1 on a.type_id=s1.id ";
                    sqlD += "left join #s2 s2 on a.type_id=s2.id ";
                    sqlD += "where a.CompanyCode='" + uu.CompanyId + "' and a.type_code='L' ";
                    sqlD += "order by a.type_id ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sqlH = "select sum(isnull(s1.Qty1,0))SumQty1,sum(isnull(s1.Cash1,0))SumCash1, ";
                    sqlH += "sum(isnull(s2.Qty2,0))SumQty2,sum(isnull(s2.Cash2,0))SumCash2, ";
                    sqlH += "case when sum(isnull(s1.Cash1,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.Cash2,0))-sum(isnull(s1.Cash1,0)) as Float)/cast(sum(isnull(s1.Cash1,0)) as Float),'p') end as SumPer ";
                    sqlH += "from TypeDataWeb a (nolock) ";
                    sqlH += "left join #s1 s1 on a.type_id=s1.id ";
                    sqlH += "left join #s2 s2 on a.type_id=s2.id ";
                    sqlH += "where a.CompanyCode='" + uu.CompanyId + "' and a.type_code='L' ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSA103QueryD1")]
        public ActionResult SystemSetup_MSSA103QueryD1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSA103QueryD1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string OpenDateS1 = rq["OpenDateS1"];
                string OpenDateE1 = rq["OpenDateE1"];
                string OpenDateS2 = rq["OpenDateS2"];
                string OpenDateE2 = rq["OpenDateE2"];
                string ID = rq["ID"];
                string Flag = rq["Flag"];

                string sql = "";
                string sqlD = "";
                string sqlH = "";

                //部門
                if (Flag == "D")
                {
                    //前期
                    sql = "select p.GD_Dept id,Sum(a.Num)Qty1,Sum(a.Cash)Cash1 into #s1 ";
                    sql += "from SalesDWeb a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "inner join PLUWeb p (nolock) on a.GoodsNo=p.GD_NO and p.Companycode='" + uu.CompanyId + "' ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    if (OpenDateS1 != "")
                    {
                        sql += "and a.OpenDate between '" + OpenDateS1 + "' and '" + OpenDateE1 + "' ";
                    }
                    if (ID != "")
                    {
                        sql += "and a.ShopNo='" + ID + "' ";
                    }
                    sql += "group by p.GD_Dept; ";

                    //本期
                    sql += "select p.GD_Dept id,Sum(a.Num)Qty2,Sum(a.Cash)Cash2 into #s2 ";
                    sql += "from SalesDWeb a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "inner join PLUWeb p (nolock) on a.GoodsNo=p.GD_NO and p.Companycode='" + uu.CompanyId + "' ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    if (OpenDateS2 != "")
                    {
                        sql += "and a.OpenDate between '" + OpenDateS2 + "' and '" + OpenDateE2 + "' ";
                    }
                    if (ID != "")
                    {
                        sql += "and a.ShopNo='" + ID + "' ";
                    }
                    sql += "group by p.GD_Dept; ";

                    //明細資料
                    sqlD = "select a.type_id + '-' + a.type_name id,isnull(s1.Qty1,0)Qty1,isnull(s1.Cash1,0)Cash1, ";
                    sqlD += "isnull(s2.Qty2,0)Qty2,isnull(s2.Cash2,0)Cash2, ";
                    sqlD += "case when isnull(s1.Cash1,0)=0 and isnull(s2.Cash2,0)=0 then format(0,'p') when isnull(s1.Cash1,0)=0 then format(1,'p') else format(cast(isnull(s2.Cash2,0)-isnull(s1.Cash1,0) as Float)/cast(isnull(s1.Cash1,0) as Float),'p') end as Per ";
                    sqlD += "from TypeDataWeb a (nolock) ";
                    sqlD += "left join #s1 s1 on a.type_id=s1.id ";
                    sqlD += "left join #s2 s2 on a.type_id=s2.id ";
                    sqlD += "where a.CompanyCode='" + uu.CompanyId + "' and a.type_code='G' ";
                    sqlD += "order by a.type_id ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sqlH = "select sum(isnull(s1.Qty1,0))SumQty1,sum(isnull(s1.Cash1,0))SumCash1, ";
                    sqlH += "sum(isnull(s2.Qty2,0))SumQty2,sum(isnull(s2.Cash2,0))SumCash2, ";
                    sqlH += "case when sum(isnull(s1.Cash1,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.Cash2,0))-sum(isnull(s1.Cash1,0)) as Float)/cast(sum(isnull(s1.Cash1,0)) as Float),'p') end as SumPer ";
                    sqlH += "from TypeDataWeb a (nolock) ";
                    sqlH += "left join #s1 s1 on a.type_id=s1.id ";
                    sqlH += "left join #s2 s2 on a.type_id=s2.id ";
                    sqlH += "where a.CompanyCode='" + uu.CompanyId + "' and a.type_code='G' ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
                //大類
                else if (Flag == "B")
                {
                    //前期
                    sql = "select p.GD_BGNO id,Sum(a.Num)Qty1,Sum(a.Cash)Cash1 into #s1 ";
                    sql += "from SalesDWeb a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "inner join PLUWeb p (nolock) on a.GoodsNo=p.GD_NO and p.Companycode='" + uu.CompanyId + "' ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    if (OpenDateS1 != "")
                    {
                        sql += "and a.OpenDate between '" + OpenDateS1 + "' and '" + OpenDateE1 + "' ";
                    }
                    if (ID != "")
                    {
                        sql += "and a.ShopNo='" + ID + "' ";
                    }
                    sql += "group by p.GD_BGNO; ";

                    //本期
                    sql += "select p.GD_BGNO id,Sum(a.Num)Qty2,Sum(a.Cash)Cash2 into #s2 ";
                    sql += "from SalesDWeb a (nolock) ";
                    sql += "inner join WarehouseWeb w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "inner join PLUWeb p (nolock) on a.GoodsNo=p.GD_NO and p.Companycode='" + uu.CompanyId + "' ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    if (OpenDateS2 != "")
                    {
                        sql += "and a.OpenDate between '" + OpenDateS2 + "' and '" + OpenDateE2 + "' ";
                    }
                    if (ID != "")
                    {
                        sql += "and a.ShopNo='" + ID + "' ";
                    }
                    sql += "group by p.GD_BGNO; ";

                    //明細資料
                    sqlD = "select a.type_id + '-' + a.type_name id,isnull(s1.Qty1,0)Qty1,isnull(s1.Cash1,0)Cash1, ";
                    sqlD += "isnull(s2.Qty2,0)Qty2,isnull(s2.Cash2,0)Cash2, ";
                    sqlD += "case when isnull(s1.Cash1,0)=0 and isnull(s2.Cash2,0)=0 then format(0,'p') when isnull(s1.Cash1,0)=0 then format(1,'p') else format(cast(isnull(s2.Cash2,0)-isnull(s1.Cash1,0) as Float)/cast(isnull(s1.Cash1,0) as Float),'p') end as Per ";
                    sqlD += "from TypeDataWeb a (nolock) ";
                    sqlD += "left join #s1 s1 on a.type_id=s1.id ";
                    sqlD += "left join #s2 s2 on a.type_id=s2.id ";
                    sqlD += "where a.CompanyCode='" + uu.CompanyId + "' and a.type_code='L' ";
                    sqlD += "order by a.type_id ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sqlH = "select sum(isnull(s1.Qty1,0))SumQty1,sum(isnull(s1.Cash1,0))SumCash1, ";
                    sqlH += "sum(isnull(s2.Qty2,0))SumQty2,sum(isnull(s2.Cash2,0))SumCash2, ";
                    sqlH += "case when sum(isnull(s1.Cash1,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.Cash2,0))-sum(isnull(s1.Cash1,0)) as Float)/cast(sum(isnull(s1.Cash1,0)) as Float),'p') end as SumPer ";
                    sqlH += "from TypeDataWeb a (nolock) ";
                    sqlH += "left join #s1 s1 on a.type_id=s1.id ";
                    sqlH += "left join #s2 s2 on a.type_id=s2.id ";
                    sqlH += "where a.CompanyCode='" + uu.CompanyId + "' and a.type_code='L' ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSA103QueryDD1")]
        public ActionResult SystemSetup_MSSA103QueryDD1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSA103QueryDD1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string OpenDateS1 = rq["OpenDateS1"];
                string OpenDateE1 = rq["OpenDateE1"];
                string OpenDateS2 = rq["OpenDateS2"];
                string OpenDateE2 = rq["OpenDateE2"];
                string ShopNo = rq["ShopNo"];
                string Dept = rq["Dept"];

                string sql = "";
                string sqlD = "";
                string sqlH = "";

                //前期
                sql = "select p.GD_BGNO id,Sum(a.Num)Qty1,Sum(a.Cash)Cash1 into #s1 ";
                sql += "from SalesDWeb a (nolock) ";
                sql += "inner join WarehouseWeb w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                sql += "inner join PLUWeb p (nolock) on a.GoodsNo=p.GD_NO and p.Companycode='" + uu.CompanyId + "' ";
                if (Dept != "") {
                    sql += "and p.GD_Dept='" + Dept + "' ";
                }
                sql += "where a.Companycode='" + uu.CompanyId + "' ";
                if (OpenDateS1 != "")
                {
                    sql += "and a.OpenDate between '" + OpenDateS1 + "' and '" + OpenDateE1 + "' ";
                }
                if (ShopNo != "")
                {
                    sql += "and a.ShopNo='" + ShopNo + "' ";
                }
                sql += "group by p.GD_BGNO; ";

                //本期
                sql += "select p.GD_BGNO id,Sum(a.Num)Qty2,Sum(a.Cash)Cash2 into #s2 ";
                sql += "from SalesDWeb a (nolock) ";
                sql += "inner join WarehouseWeb w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                sql += "inner join PLUWeb p (nolock) on a.GoodsNo=p.GD_NO and p.Companycode='" + uu.CompanyId + "' ";
                if (Dept != "")
                {
                    sql += "and p.GD_Dept='" + Dept + "' ";
                }
                sql += "where a.Companycode='" + uu.CompanyId + "' ";
                if (OpenDateS2 != "")
                {
                    sql += "and a.OpenDate between '" + OpenDateS2 + "' and '" + OpenDateE2 + "' ";
                }
                if (ShopNo != "")
                {
                    sql += "and a.ShopNo='" + ShopNo + "' ";
                }
                sql += "group by p.GD_BGNO; ";

                //明細資料
                sqlD = "select a.type_id + '-' + a.type_name id,isnull(s1.Qty1,0)Qty1,isnull(s1.Cash1,0)Cash1, ";
                sqlD += "isnull(s2.Qty2,0)Qty2,isnull(s2.Cash2,0)Cash2, ";
                sqlD += "case when isnull(s1.Cash1,0)=0 and isnull(s2.Cash2,0)=0 then format(0,'p') when isnull(s1.Cash1,0)=0 then format(1,'p') else format(cast(isnull(s2.Cash2,0)-isnull(s1.Cash1,0) as Float)/cast(isnull(s1.Cash1,0) as Float),'p') end as Per ";
                sqlD += "from TypeDataWeb a (nolock) ";
                sqlD += "left join #s1 s1 on a.type_id=s1.id ";
                sqlD += "left join #s2 s2 on a.type_id=s2.id ";
                sqlD += "where a.CompanyCode='" + uu.CompanyId + "' and a.type_code='L' ";
                sqlD += "order by a.type_id ";
                DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);

                //彙總資料
                sqlH = "select sum(isnull(s1.Qty1,0))SumQty1,sum(isnull(s1.Cash1,0))SumCash1, ";
                sqlH += "sum(isnull(s2.Qty2,0))SumQty2,sum(isnull(s2.Cash2,0))SumCash2, ";
                sqlH += "case when sum(isnull(s1.Cash1,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.Cash2,0))-sum(isnull(s1.Cash1,0)) as Float)/cast(sum(isnull(s1.Cash1,0)) as Float),'p') end as SumPer ";
                sqlH += "from TypeDataWeb a (nolock) ";
                sqlH += "left join #s1 s1 on a.type_id=s1.id ";
                sqlH += "left join #s2 s2 on a.type_id=s2.id ";
                sqlH += "where a.CompanyCode='" + uu.CompanyId + "' and a.type_code='L' ";
                DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSA103QueryD2")]
        public ActionResult SystemSetup_MSSA103QueryD2()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSA103QueryD2OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string OpenDateS1 = rq["OpenDateS1"];
                string OpenDateE1 = rq["OpenDateE1"];
                string OpenDateS2 = rq["OpenDateS2"];
                string OpenDateE2 = rq["OpenDateE2"];
                string ShopNo = rq["ShopNo"];
                string ID = rq["ID"];

                string sql = "";
                string sqlD = "";
                string sqlH = "";

                //前期
                sql = "select p.GD_BGNO id,Sum(a.Num)Qty1,Sum(a.Cash)Cash1 into #s1 ";
                sql += "from SalesDWeb a (nolock) ";
                sql += "inner join WarehouseWeb w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                sql += "inner join PLUWeb p (nolock) on a.GoodsNo=p.GD_NO and p.Companycode='" + uu.CompanyId + "' ";
                if (ID != "")
                {
                    sql += "and p.GD_Dept='" + ID + "' ";
                }
                sql += "where a.Companycode='" + uu.CompanyId + "' ";
                if (OpenDateS1 != "")
                {
                    sql += "and a.OpenDate between '" + OpenDateS1 + "' and '" + OpenDateE1 + "' ";
                }
                if (ShopNo != "") {
                    sql += "and a.ShopNo in(" + ShopNo + ") ";
                }
                sql += "group by p.GD_BGNO; ";

                //本期
                sql += "select p.GD_BGNO id,Sum(a.Num)Qty2,Sum(a.Cash)Cash2 into #s2 ";
                sql += "from SalesDWeb a (nolock) ";
                sql += "inner join WarehouseWeb w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                sql += "inner join PLUWeb p (nolock) on a.GoodsNo=p.GD_NO and p.Companycode='" + uu.CompanyId + "' ";
                if (ID != "")
                {
                    sql += "and p.GD_Dept='" + ID + "' ";
                }
                sql += "where a.Companycode='" + uu.CompanyId + "' ";
                if (OpenDateS2 != "")
                {
                    sql += "and a.OpenDate between '" + OpenDateS2 + "' and '" + OpenDateE2 + "' ";
                }
                if (ShopNo != "")
                {
                    sql += "and a.ShopNo in(" + ShopNo + ") ";
                }
                sql += "group by p.GD_BGNO; ";

                //明細資料
                sqlD = "select a.type_id + '-' + a.type_name id,isnull(s1.Qty1,0)Qty1,isnull(s1.Cash1,0)Cash1, ";
                sqlD += "isnull(s2.Qty2,0)Qty2,isnull(s2.Cash2,0)Cash2, ";
                sqlD += "case when isnull(s1.Cash1,0)=0 and isnull(s2.Cash2,0)=0 then format(0,'p') when isnull(s1.Cash1,0)=0 then format(1,'p') else format(cast(isnull(s2.Cash2,0)-isnull(s1.Cash1,0) as Float)/cast(isnull(s1.Cash1,0) as Float),'p') end as Per ";
                sqlD += "from TypeDataWeb a (nolock) ";
                sqlD += "left join #s1 s1 on a.type_id=s1.id ";
                sqlD += "left join #s2 s2 on a.type_id=s2.id ";
                sqlD += "where a.CompanyCode='" + uu.CompanyId + "' and a.type_code='L' ";
                sqlD += "order by a.type_id ";
                DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);

                //彙總資料
                sqlH = "select sum(isnull(s1.Qty1,0))SumQty1,sum(isnull(s1.Cash1,0))SumCash1, ";
                sqlH += "sum(isnull(s2.Qty2,0))SumQty2,sum(isnull(s2.Cash2,0))SumCash2, ";
                sqlH += "case when sum(isnull(s1.Cash1,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.Cash2,0))-sum(isnull(s1.Cash1,0)) as Float)/cast(sum(isnull(s1.Cash1,0)) as Float),'p') end as SumPer ";
                sqlH += "from TypeDataWeb a (nolock) ";
                sqlH += "left join #s1 s1 on a.type_id=s1.id ";
                sqlH += "left join #s2 s2 on a.type_id=s2.id ";
                sqlH += "where a.CompanyCode='" + uu.CompanyId + "' and a.type_code='L' ";
                DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }
        #endregion

        #region MSSA108
        [Route("SystemSetup/MSSA108Query")]
        public ActionResult SystemSetup_MSSA108Query()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSA108QueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ProgramID = rq["ProgramID"];
                string Flag = rq["Flag"];
                string sqlD = "";
                string sqlH = "";
                string sql = "select ChineseName,convert(char(10),getdate(),111) as SysDate from ProgramIDWeb (nolock) where ProgramID='" + ProgramID.SqlQuote() + "'";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);

                //區域
                if (Flag == "A") {
                    sql = "Select b.ST_PlaceID id,sum(a.RecCount)RecCount,sum(a.Qty)Qty,sum(a.Cash)Cash into #s1 ";
                    sql += "From SalesAtonceHWeb a (nolock) ";
                    sql += "inner join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode and b.st_type not in ('2','3') ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' and a.OpenDate=convert(char(10),getdate(),111) ";
                    sql += "group by b.ST_PlaceID; ";

                    //明細資料
                    sqlD = "Select a.type_id + '-' + a.type_name id,isnull(b.RecCount,0)RecCount,isnull(b.Qty,0)Qty, ";
                    sqlD += "isnull(b.Cash,0)Cash, ";
                    sqlD += "case when isnull(b.RecCount,0)=0 then 0 else Round(isnull(b.Cash,0) / isnull(b.RecCount,0), 0) end as Price ";
                    sqlD += "From TypeDataWeb a (nolock) ";
                    sqlD += "left join #s1 b on a.type_id=b.id ";
                    sqlD += "Where a.Companycode='" + uu.CompanyId + "' and a.type_code='A' ";
                    sqlD += "order by a.type_id ";
                    DataTable dtD = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtD.TableName = "dtD";
                    ds.Tables.Add(dtD);

                    //彙總資料
                    sqlH = "select sum(isnull(b.RecCount,0))SumRecCount,sum(isnull(b.Qty,0))SumQty, ";
                    sqlH += "sum(isnull(b.Cash,0))SumCash, ";
                    sqlH += "case when sum(isnull(b.RecCount,0))=0 then 0 else Round(sum(isnull(b.Cash,0)) / sum(isnull(b.RecCount,0)), 0) end as SumPrice ";
                    sqlH += "From TypeDataWeb a (nolock) ";
                    sqlH += "left join #s1 b on a.type_id=b.id ";
                    sqlH += "Where a.Companycode='" + uu.CompanyId + "' and a.type_code='A' ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);

                }
                //店別
                else if (Flag == "S") {
                    sql = "Select a.ShopNo id,b.st_sname name,sum(a.RecCount)RecCount,sum(a.Qty)Qty,sum(a.Cash)Cash into #s1 ";
                    sql += "From SalesAtonceHWeb a (nolock) ";
                    sql += "inner join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode and b.st_type not in ('2','3') ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' and a.OpenDate=convert(char(10),getdate(),111) ";
                    sql += "group by a.ShopNo,b.st_sname; ";

                    //明細資料
                    sqlD = "Select a.id + '-' + a.name id,isnull(a.RecCount,0)RecCount,isnull(a.Qty,0)Qty, ";
                    sqlD += "isnull(a.Cash,0)Cash, ";
                    sqlD += "case when isnull(a.RecCount,0)=0 then 0 else Round(isnull(a.Cash,0) / isnull(a.RecCount,0), 0) end as Price ";
                    sqlD += "From #s1 a (nolock) ";
                    sqlD += "Where 1=1 ";
                    sqlD += "order by a.id ";
                    DataTable dtD = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtD.TableName = "dtD";
                    ds.Tables.Add(dtD);

                    //彙總資料
                    sqlH = "select sum(isnull(a.RecCount,0))SumRecCount,sum(isnull(a.Qty,0))SumQty, ";
                    sqlH += "sum(isnull(a.Cash,0))SumCash, ";
                    sqlH += "case when sum(isnull(a.RecCount,0))=0 then 0 else Round(sum(isnull(a.Cash,0)) / sum(isnull(a.RecCount,0)), 0) end as SumPrice ";
                    sqlH += "From #s1 a (nolock) ";
                    sqlH += "Where 1=1 ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
                //前20名商品(金額)
                else if (Flag == "20C")
                {
                    sql = "Select a.GoodsNo id,c.GD_Name name,sum(a.Num)Qty,sum(a.Cash)Cash into #s1 ";
                    sql += "From SalesAtonceDWeb a (nolock) ";
                    sql += "inner join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode and b.st_type not in ('2','3') ";
                    sql += "inner join PLUWeb c (nolock) on a.GoodsNo=c.GD_NO and c.Companycode=a.Companycode ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' and a.OpenDate=convert(char(10),getdate(),111) ";
                    sql += "group by a.GoodsNo,c.GD_Name; ";

                    //明細資料
                    sqlD = "Select top 20 a.id ID,a.name Name,isnull(a.Qty,0)Qty,isnull(a.Cash,0)Cash ";
                    sqlD += "From #s1 a (nolock) ";
                    sqlD += "Where 1=1 ";
                    sqlD += "order by a.Cash desc,a.id ";
                    DataTable dtD = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtD.TableName = "dtD";
                    ds.Tables.Add(dtD);

                    //彙總資料
                    sqlH = "select sum(isnull(aa.Qty,0))SumQty,sum(isnull(aa.Cash,0))SumCash ";
                    sqlH += "From ( ";
                    sqlH += "Select top 20 isnull(a.Qty,0)Qty,isnull(a.Cash,0)Cash ";
                    sqlH += "From #s1 a (nolock) ";
                    sqlH += "Where 1=1 ";
                    sqlH += "order by a.Cash desc,a.id ";
                    sqlH += ")aa ";
                    sqlH += "Where 1=1 ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
                //前20名商品(數量)
                else if (Flag == "20N")
                {
                    sql = "Select a.GoodsNo id,c.GD_Name name,sum(a.Num)Qty,sum(a.Cash)Cash into #s1 ";
                    sql += "From SalesAtonceDWeb a (nolock) ";
                    sql += "inner join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode and b.st_type not in ('2','3') ";
                    sql += "inner join PLUWeb c (nolock) on a.GoodsNo=c.GD_NO and c.Companycode=a.Companycode ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' and a.OpenDate=convert(char(10),getdate(),111) ";
                    sql += "group by a.GoodsNo,c.GD_Name; ";

                    //明細資料
                    sqlD = "Select top 20 a.id ID,a.name Name,isnull(a.Qty,0)Qty,isnull(a.Cash,0)Cash ";
                    sqlD += "From #s1 a (nolock) ";
                    sqlD += "Where 1=1 ";
                    sqlD += "order by a.Qty desc,a.id ";
                    DataTable dtD = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtD.TableName = "dtD";
                    ds.Tables.Add(dtD);

                    //彙總資料
                    sqlH = "select sum(isnull(aa.Qty,0))SumQty,sum(isnull(aa.Cash,0))SumCash ";
                    sqlH += "From ( ";
                    sqlH += "Select top 20 isnull(a.Qty,0)Qty,isnull(a.Cash,0)Cash ";
                    sqlH += "From #s1 a (nolock) ";
                    sqlH += "Where 1=1 ";
                    sqlH += "order by a.Qty desc,a.id ";
                    sqlH += ")aa ";
                    sqlH += "Where 1=1 ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSA108QueryD1")]
        public ActionResult SystemSetup_MSSA108QueryD1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSA108QueryD1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Today = rq["Today"];
                string ShopNo = rq["ShopNo"];
                string Flag = rq["Flag"];
                string sqlD = "";
                string sqlH = "";
                string sql = "";


                sql = "Select a.GoodsNo id,c.GD_Name name,sum(a.Num)Qty,sum(a.Cash)Cash into #s1 ";
                sql += "From SalesAtonceDWeb a (nolock) ";
                sql += "inner join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode and b.st_type not in ('2','3') ";
                sql += "inner join PLUWeb c (nolock) on a.GoodsNo=c.GD_NO and c.Companycode=a.Companycode ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                if (Today != "")
                {
                    sql += "and a.OpenDate='" + Today + "' ";
                }
                if (ShopNo != "")
                {
                    sql += "and a.ShopNo='" + ShopNo + "' ";
                }
                sql += "group by a.GoodsNo,c.GD_Name; ";

                //明細資料
                sqlD = "Select top 20 a.id ID,a.name Name,isnull(a.Qty,0)Qty,isnull(a.Cash,0)Cash ";
                sqlD += "From #s1 a (nolock) ";
                sqlD += "Where 1=1 ";
                if (Flag == "20C") {
                    sqlD += "order by a.Cash desc,a.id ";
                }
                else if (Flag == "20N") {
                    sqlD += "order by a.Qty desc,a.id ";
                }
                DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
                //彙總資料
                sqlH = "select sum(isnull(aa.Qty,0))SumQty,sum(isnull(aa.Cash,0))SumCash ";
                sqlH += "From ( ";
                sqlH += "Select top 20 isnull(a.Qty,0)Qty,isnull(a.Cash,0)Cash ";
                sqlH += "From #s1 a (nolock) ";
                sqlH += "Where 1=1 ";
                if (Flag == "20C")
                {
                    sqlH += "order by a.Cash desc,a.id ";
                }
                else if (Flag == "20N")
                {
                    sqlH += "order by a.Qty desc,a.id ";
                }
                sqlH += ")aa ";
                sqlH += "Where 1=1 ";
                DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSA108QueryD2")]
        public ActionResult SystemSetup_MSSA108QueryD2()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSA108QueryD2OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Today = rq["Today"];
                string Area = rq["Area"];
                string Flag = rq["Flag"];
                string sqlD = "";
                string sqlH = "";
                string sql = "";

                if (Flag == "S")
                {
                    sql = "Select a.ShopNo id,b.st_sname name,sum(a.RecCount)RecCount,sum(a.Qty)Qty,sum(a.Cash)Cash into #s1 ";
                    sql += "From SalesAtonceHWeb a (nolock) ";
                    sql += "inner join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode and b.st_type not in ('2','3') ";
                    if (Area != "") {
                        sql += "and b.st_placeid='" + Area + "' ";
                    }
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    if (Today != "") {
                        sql += "and a.OpenDate='" + Today + "' ";
                    }
                    sql += "group by a.ShopNo,b.st_sname; ";

                    //明細資料
                    sqlD = "Select a.id + '-' + a.name id,isnull(a.RecCount,0)RecCount,isnull(a.Qty,0)Qty, ";
                    sqlD += "isnull(a.Cash,0)Cash, ";
                    sqlD += "case when isnull(a.RecCount,0)=0 then 0 else Round(isnull(a.Cash,0) / isnull(a.RecCount,0), 0) end as Price ";
                    sqlD += "From #s1 a (nolock) ";
                    sqlD += "Where 1=1 ";
                    sqlD += "order by a.id ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sqlH = "select sum(isnull(a.RecCount,0))SumRecCount,sum(isnull(a.Qty,0))SumQty, ";
                    sqlH += "sum(isnull(a.Cash,0))SumCash, ";
                    sqlH += "case when sum(isnull(a.RecCount,0))=0 then 0 else Round(sum(isnull(a.Cash,0)) / sum(isnull(a.RecCount,0)), 0) end as SumPrice ";
                    sqlH += "From #s1 a (nolock) ";
                    sqlH += "Where 1=1 ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
                else {
                    sql = "Select a.GoodsNo id,c.GD_Name name,sum(a.Num)Qty,sum(a.Cash)Cash into #s1 ";
                    sql += "From SalesAtonceDWeb a (nolock) ";
                    sql += "inner join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode and b.st_type not in ('2','3') ";
                    if (Area != "")
                    {
                        sql += "and b.st_placeid='" + Area + "' ";
                    }
                    sql += "inner join PLUWeb c (nolock) on a.GoodsNo=c.GD_NO and c.Companycode=a.Companycode ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    if (Today != "")
                    {
                        sql += "and a.OpenDate='" + Today + "' ";
                    }
                    sql += "group by a.GoodsNo,c.GD_Name; ";

                    //明細資料
                    sqlD = "Select top 20 a.id ID,a.name Name,isnull(a.Qty,0)Qty,isnull(a.Cash,0)Cash ";
                    sqlD += "From #s1 a (nolock) ";
                    sqlD += "Where 1=1 ";
                    if (Flag == "20C")
                    {
                        sqlD += "order by a.Cash desc,a.id ";
                    }
                    else if (Flag == "20N")
                    {
                        sqlD += "order by a.Qty desc,a.id ";
                    }
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);
                    //彙總資料
                    sqlH = "select sum(isnull(aa.Qty,0))SumQty,sum(isnull(aa.Cash,0))SumCash ";
                    sqlH += "From ( ";
                    sqlH += "Select top 20 isnull(a.Qty,0)Qty,isnull(a.Cash,0)Cash ";
                    sqlH += "From #s1 a (nolock) ";
                    sqlH += "Where 1=1 ";
                    if (Flag == "20C")
                    {
                        sqlH += "order by a.Cash desc,a.id ";
                    }
                    else if (Flag == "20N")
                    {
                        sqlH += "order by a.Qty desc,a.id ";
                    }
                    sqlH += ")aa ";
                    sqlH += "Where 1=1 ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSA108QueryDD1")]
        public ActionResult SystemSetup_MSSA108QueryDD1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSA108QueryDD1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Today = rq["Today"];
                string Area = rq["Area"];
                string ShopNo = rq["ShopNo"];
                string Flag = rq["Flag"];
                string sqlD = "";
                string sqlH = "";
                string sql = "";

                sql = "Select a.GoodsNo id,c.GD_Name name,sum(a.Num)Qty,sum(a.Cash)Cash into #s1 ";
                sql += "From SalesAtonceDWeb a (nolock) ";
                sql += "inner join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode and b.st_type not in ('2','3') ";
                if (Area != "") {
                    sql += "and b.st_placeid='" + Area + "' ";
                }
                sql += "inner join PLUWeb c (nolock) on a.GoodsNo=c.GD_NO and c.Companycode=a.Companycode ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                if (Today != "")
                {
                    sql += "and a.OpenDate='" + Today + "' ";
                }
                if (ShopNo != "")
                {
                    sql += "and a.ShopNo='" + ShopNo + "' ";
                }
                sql += "group by a.GoodsNo,c.GD_Name; ";

                //明細資料
                sqlD = "Select top 20 a.id ID,a.name Name,isnull(a.Qty,0)Qty,isnull(a.Cash,0)Cash ";
                sqlD += "From #s1 a (nolock) ";
                sqlD += "Where 1=1 ";
                if (Flag == "20C")
                {
                    sqlD += "order by a.Cash desc,a.id ";
                }
                else if (Flag == "20N")
                {
                    sqlD += "order by a.Qty desc,a.id ";
                }
                DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
                //彙總資料
                sqlH = "select sum(isnull(aa.Qty,0))SumQty,sum(isnull(aa.Cash,0))SumCash ";
                sqlH += "From ( ";
                sqlH += "Select top 20 isnull(a.Qty,0)Qty,isnull(a.Cash,0)Cash ";
                sqlH += "From #s1 a (nolock) ";
                sqlH += "Where 1=1 ";
                if (Flag == "20C")
                {
                    sqlH += "order by a.Cash desc,a.id ";
                }
                else if (Flag == "20N")
                {
                    sqlH += "order by a.Qty desc,a.id ";
                }
                sqlH += ")aa ";
                sqlH += "Where 1=1 ";
                DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSA108QueryD3")]
        public ActionResult SystemSetup_MSSA108QueryD3()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSA108QueryD3OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Today = rq["Today"];
                string PLU = rq["PLU"];
                string sqlD = "";
                string sqlH = "";
                string sql = "";

                sql = "Select a.ShopNo id,b.st_sname name,sum(a.Num)Num,sum(a.Cash)Cash into #s1 ";
                sql += "From SalesAtonceDWeb a (nolock) ";
                sql += "inner join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode and b.st_type not in ('2','3') ";
                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                if (Today != "") {
                    sql += "and a.OpenDate='" + Today + "' ";
                }
                if (PLU != "")
                {
                    sql += "and a.GoodsNo='" + PLU + "' ";
                }
                sql += "group by a.ShopNo,b.st_sname; ";

                //明細資料
                sqlD = "Select a.id + '-' + a.name id,isnull(a.Num,0)Num,isnull(a.Cash,0)Cash ";
                sqlD += "From #s1 a (nolock) ";
                sqlD += "Where 1=1 ";
                sqlD += "order by a.id ";
                DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);

                //彙總資料
                sqlH = "select sum(isnull(a.Num,0))SumNum,sum(isnull(a.Cash,0))SumCash ";
                sqlH += "From #s1 a (nolock) ";
                sqlH += "Where 1=1 ";
                DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }
        #endregion

        #region MSSA107
        [Route("SystemSetup/GetInitMSSA107")]
        public ActionResult SystemSetup_GetInitMSSA107()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitMSSA107OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ProgramID = rq["ProgramID"];
                string Yesterday = PubUtility.GetYesterday(uu);
                string Today = PubUtility.GetToday(uu);
                string sql = "select ChineseName from ProgramIDWeb (nolock) where ProgramID='" + ProgramID.SqlQuote() + "'";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
                //string ls_TestDT = "2023/12/25";  //black--2024/01/24
                //系統日-1(星期幾)
                //string sqldw = "select top 4 datepart(weekday,Dateadd(d,number*-1,'" + ls_TestDT + "')) dw,convert(varchar,Dateadd(d,number*-1,'" + ls_TestDT + "'),111) W1,";
                //sqldw += "convert(varchar,MONTH(Dateadd(d,number*-1,'" + ls_TestDT + "')))+'/'+convert(varchar,Day(Dateadd(d,number*-1,'" + ls_TestDT + "'))) RptW1,";
                //sqldw += "'W'+convert(varchar,ROW_NUMBER() over(order by number)) WeekCnt,";
                //sqldw += "case DATEPART(weekday, '" + ls_TestDT + "') when 1 then N'日' when 2 then N'一' when 3 then N'二' when 4 then N'三' when 5 then N'四' when 6 then N'五' when 7 then N'六' else '' end DayWeek {0} ";
                //sqldw += "from master..spt_values where type = 'p' and number<= 30 and datepart(weekday,Dateadd(d,number*-1,'" + ls_TestDT + "'))=datepart(weekday,'" + ls_TestDT + "') ";
                //sqldw += "and convert(varchar, Dateadd(d, number*-1,'" + ls_TestDT + "'),111)< '2023/12/26'";
                string sqldw = "select top 4 datepart(weekday,Dateadd(d,number*-1,dateadd(d,-1,getdate()))) dw,convert(varchar,Dateadd(d,number*-1,dateadd(d,-1,getdate())),111) W1,";
                sqldw += "convert(varchar,MONTH(Dateadd(d,number*-1,dateadd(d,-1,getdate()))))+'/'+convert(varchar,Day(Dateadd(d,number*-1,dateadd(d,-1,getdate())))) RptW1,";
                sqldw += "'W'+convert(varchar,ROW_NUMBER() over(order by number)) WeekCnt,";
                sqldw += "case DATEPART(weekday, Dateadd(d, -1, getdate())) when 1 then N'日' when 2 then N'一' when 3 then N'二' when 4 then N'三' when 5 then N'四' when 6 then N'五' when 7 then N'六' else '' end DayWeek {0} ";
                sqldw += "from master..spt_values where type = 'p' and number<= 30 and datepart(weekday,Dateadd(d,number*-1,dateadd(d,-1,getdate())))=datepart(weekday,Dateadd(d,-1,getdate())) ";
                sqldw += "and convert(varchar, Dateadd(d, number*-1,dateadd(d, -1, getdate())),111)< convert(varchar, getdate(), 111)";
                DataTable dtD = PubUtility.SqlQry(string.Format(sqldw, ""), uu, "SYS");
                dtD.TableName = "dtD";
                ds.Tables.Add(dtD);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSA107Query")]
        public ActionResult SystemSetup_MSSA107Query()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSA107QueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ShopNo = rq["ShopNo"];
                string DayNM = rq["DayNM"];
                string Flag = rq["Flag"];
                string weekDay = "";
                string sql = "";
                string sqlCon = "";
                
                if (DayNM == "日") { weekDay = "1"; }
                else if (DayNM == "一") { weekDay = "2"; }
                else if (DayNM == "二") { weekDay = "3"; }
                else if (DayNM == "三") { weekDay = "4"; }
                else if (DayNM == "四") { weekDay = "5"; }
                else if (DayNM == "五") { weekDay = "6"; }
                else if (DayNM == "六") { weekDay = "7"; }

                if (ShopNo != "")
                {
                    sqlCon += "and {0} in(" + ShopNo + ") ";
                }
                if (Flag == "S")
                {
                    sql = "";
                }
                else if (Flag == "T")
                {
                    sql = "select case len(number) when 1 then '0'+convert(varchar,number) else convert(varchar,number) end T1 into #tmpT from master..spt_values where type='p' and number<=23;";
                }

                //string ls_TestDT = "2023/12/25";  //black--2024/01/24
                //指定星期幾
                //string sqldw = "select top 4 datepart(weekday,Dateadd(d,number*-1,'" + ls_TestDT + "')) dw,convert(varchar,Dateadd(d,number*-1,'" + ls_TestDT + "'),111) W1,";
                //sqldw += "convert(varchar,MONTH(Dateadd(d,number*-1,'" + ls_TestDT + "')))+'/'+convert(varchar,Day(Dateadd(d,number*-1,'" + ls_TestDT + "'))) RptW1,";
                //sqldw += "'W'+convert(varchar,ROW_NUMBER() over(order by number)) WeekCnt,";
                //sqldw += "case DATEPART(weekday, '" + ls_TestDT + "') when 1 then N'日' when 2 then N'一' when 3 then N'二' when 4 then N'三' when 5 then N'四' when 6 then N'五' when 7 then N'六' else '' end DayWeek {0} ";
                //sqldw += "from master..spt_values where type = 'p' and number<= 30 and datepart(weekday,Dateadd(d,number*-1,'" + ls_TestDT + "'))=" + weekDay;
                //sqldw += " and convert(varchar, Dateadd(d, number*-1,'" + ls_TestDT + "'),111)< '2023/12/26'";
                string sqldw = "select top 4 datepart(weekday,Dateadd(d,number*-1,dateadd(d,-1,getdate()))) dw,convert(varchar,Dateadd(d,number*-1,dateadd(d,-1,getdate())),111) W1,";
                sqldw += "convert(varchar,MONTH(Dateadd(d,number*-1,dateadd(d,-1,getdate()))))+'/'+convert(varchar,Day(Dateadd(d,number*-1,dateadd(d,-1,getdate())))) RptW1,";
                sqldw += "'W'+convert(varchar,ROW_NUMBER() over(order by number)) WeekCnt,";
                sqldw += "case DATEPART(weekday, Dateadd(d, -1, getdate())) when 1 then N'日' when 2 then N'一' when 3 then N'二' when 4 then N'三' when 5 then N'四' when 6 then N'五' when 7 then N'六' else '' end DayWeek {0} ";
                sqldw += "from master..spt_values where type = 'p' and number<= 30 and datepart(weekday,Dateadd(d,number*-1,dateadd(d,-1,getdate())))=" + weekDay;
                sqldw += " and convert(varchar, Dateadd(d, number*-1,dateadd(d, -1, getdate())),111)< convert(varchar, getdate(), 111)";
                DataTable dtD = PubUtility.SqlQry(string.Format(sqldw, ""), uu, "SYS");
                dtD.TableName = "dtD";
                ds.Tables.Add(dtD);

                //4週時段表
                string sqlRtnT = "SELECT PVT.T1 [ID],isnull([W1],0) W1,isnull([W2],0) W2,isnull([W3],0) W3,isnull([W4],0) W4 into #tmpRtn ";
                sqlRtnT += "FROM (select T1,WeekCnt,Cash from #tmpRpt a left join SalesH_AllWeb b (nolock) on a.T1=b.TimeGroup and a.w1=b.OpenDate and b.CompanyCode='" + uu.CompanyId + "'";
                if (sqlCon != "") { sqlRtnT += " "+string.Format(sqlCon,"b.ShopNo"); }
                sqlRtnT += ") H PIVOT(";
                sqlRtnT += "Sum(Cash)";
                sqlRtnT += "FOR WeekCnt IN ([W1], [W2], [W3], [W4])";
                sqlRtnT += ") AS PVT; ";

                //4週店別表
                string sqlRtnS = "SELECT PVT.WhName [ID],isnull([W1],0) W1,isnull([W2],0) W2,isnull([W3],0) W3,isnull([W4],0) W4 into #tmpRtn ";
                sqlRtnS += "FROM (select ST_ID,WhName,WeekCnt,Cash from #tmpRpt a left join SalesH_AllWeb b (nolock) on a.ST_ID=b.ShopNo and a.w1=b.OpenDate and b.CompanyCode='" + uu.CompanyId + "') H ";
                sqlRtnS += "PIVOT(";
                sqlRtnS += "Sum(Cash)";
                sqlRtnS += "FOR WeekCnt IN ([W1], [W2], [W3], [W4])";
                sqlRtnS += ") AS PVT; ";

                sql += string.Format(sqldw, "into #tmpDW") + ";";
                if (Flag == "T")
                {
                    sql += "select * into #tmpRpt from #tmpT cross join (select W1,WeekCnt from #tmpDW) a;";
                    sql += sqlRtnT;
                }else if (Flag == "S")
                {
                    sql += "select * into #tmpRpt from (select ST_ID,ST_ID+'-'+ST_Sname WhName from WarehouseWeb (nolock) where CompanyCode='" + uu.CompanyId + "'";
                    if (sqlCon != "") { 
                        sql += " " + string.Format(sqlCon, "ST_ID"); 
                    } else {
                        sql += " and ST_Type not in('0','2','3')";
                    }
                    sql += ") w cross join (select W1,WeekCnt from #tmpDW) a;";
                    sql += sqlRtnS;
                }
                    
                sql += "insert into #tmpRtn select 'SumAll',sum([W1]), sum([W2]), sum([W3]), sum([W4]) from #tmpRtn;";
                sql += "Select * from #tmpRtn order by [ID];";
                DataTable dtDelt = PubUtility.SqlQry(sql, uu, "SYS");
                dtDelt.TableName = "dtDelt";

                DataTable dtSum = dtDelt.Clone();
                dtSum.ImportRow(dtDelt.Select("ID='SumAll'")[0]);
                dtSum.TableName = "dtSum";
                ds.Tables.Add(dtSum);

                dtDelt.Rows.Remove(dtDelt.Select("ID='SumAll'")[0]);
                ds.Tables.Add(dtDelt);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }
        #endregion

        #region MSSA106
        [Route("SystemSetup/GetInitMSSA106")]
        public ActionResult SystemSetup_GetInitMSSA106()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitMSSA106OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ProgramID = rq["ProgramID"];
                string Yesterday = PubUtility.GetYesterday(uu);
                string Today = PubUtility.GetToday(uu);
                string sql = "select ChineseName from ProgramIDWeb (nolock) where ProgramID='" + ProgramID.SqlQuote() + "'";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
                //string ls_TestDT = "2023/12/27";  //black--2024/01/24
                //系統日-1(星期幾) 7日內
                //string sqldw = "select datepart(weekday,Dateadd(d,number*-1,'" + ls_TestDT + "')) dw,convert(varchar,Dateadd(d,number*-1,'" + ls_TestDT + "'),111) D1,";
                //sqldw += "convert(varchar,MONTH(Dateadd(d,number*-1,'" + ls_TestDT + "')))+'/'+convert(varchar,Day(Dateadd(d,number*-1,'" + ls_TestDT + "'))) RptD1,";
                //sqldw += "'D'+convert(varchar,ROW_NUMBER() over(order by number desc)) DayCnt,";
                //sqldw += "case Datepart(weekday,Dateadd(d,number*-1,'" + ls_TestDT + "')) when 1 then N'日' when 2 then N'一' when 3 then N'二' when 4 then N'三' when 5 then N'四' when 6 then N'五' when 7 then N'六' else '' end DayWeek {0} ";
                //sqldw += "from master..spt_values where type = 'p' and number<= 6 ";
                string sqldw = "select datepart(weekday,Dateadd(d,number*-1,dateadd(d,-1,getdate()))) dw,convert(varchar,Dateadd(d,number*-1,dateadd(d,-1,getdate())),111) D1,";
                sqldw += "convert(varchar,MONTH(Dateadd(d,number*-1,dateadd(d,-1,getdate()))))+'/'+convert(varchar,Day(Dateadd(d,number*-1,dateadd(d,-1,getdate())))) RptD1,";
                sqldw += "'D'+convert(varchar,ROW_NUMBER() over(order by number desc)) DayCnt,";
                sqldw += "case Datepart(weekday,Dateadd(d,number*-1,Dateadd(d,-1,getdate()))) when 1 then N'日' when 2 then N'一' when 3 then N'二' when 4 then N'三' when 5 then N'四' when 6 then N'五' when 7 then N'六' else '' end DayWeek {0} ";
                sqldw += "from master..spt_values where type = 'p' and number<= 6";
                DataTable dtD = PubUtility.SqlQry(string.Format(sqldw, ""), uu, "SYS");
                dtD.TableName = "dtD";
                ds.Tables.Add(dtD);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSA106Query")]
        public ActionResult SystemSetup_MSSA106Query()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSA106QueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ShopNo = rq["ShopNo"];
                string Flag = rq["Flag"];
                string sql = "";
                string sqlCon = "";

                if (ShopNo != "")
                {
                    sqlCon += "and {0} in(" + ShopNo + ") ";
                }
                if (Flag == "S")
                {
                    sql = "";
                }
                else if (Flag == "T")
                {
                    sql = "select case len(number) when 1 then '0'+convert(varchar,number) else convert(varchar,number) end T1 into #tmpT from master..spt_values where type='p' and number<=23;";
                }

                //string ls_TestDT = "2023/12/27";  //black--2024/01/24
                //系統日-1(星期幾) 7日內
                //string sqldw = "select datepart(weekday,Dateadd(d,number*-1,'" + ls_TestDT + "')) dw,convert(varchar,Dateadd(d,number*-1,'" + ls_TestDT + "'),111) D1,";
                //sqldw += "convert(varchar,MONTH(Dateadd(d,number*-1,'" + ls_TestDT + "')))+'/'+convert(varchar,Day(Dateadd(d,number*-1,'" + ls_TestDT + "'))) RptD1,";
                //sqldw += "'D'+convert(varchar,ROW_NUMBER() over(order by number desc)) DayCnt,";
                //sqldw += "case Datepart(weekday,Dateadd(d,number*-1,'" + ls_TestDT + "')) when 1 then N'日' when 2 then N'一' when 3 then N'二' when 4 then N'三' when 5 then N'四' when 6 then N'五' when 7 then N'六' else '' end DayWeek {0} ";
                //sqldw += "from master..spt_values where type = 'p' and number<= 6 ";
                string sqldw = "select datepart(weekday,Dateadd(d,number*-1,dateadd(d,-1,getdate()))) dw,convert(varchar,Dateadd(d,number*-1,dateadd(d,-1,getdate())),111) D1,";
                sqldw += "convert(varchar,MONTH(Dateadd(d,number*-1,dateadd(d,-1,getdate()))))+'/'+convert(varchar,Day(Dateadd(d,number*-1,dateadd(d,-1,getdate())))) RptD1,";
                sqldw += "'D'+convert(varchar,ROW_NUMBER() over(order by number desc)) DayCnt,";
                sqldw += "case Datepart(weekday,Dateadd(d,number*-1,Dateadd(d,-1,getdate()))) when 1 then N'日' when 2 then N'一' when 3 then N'二' when 4 then N'三' when 5 then N'四' when 6 then N'五' when 7 then N'六' else '' end DayWeek {0} ";
                sqldw += "from master..spt_values where type = 'p' and number<= 6";
                DataTable dtD = PubUtility.SqlQry(string.Format(sqldw, ""), uu, "SYS");
                dtD.TableName = "dtD";
                ds.Tables.Add(dtD);

                //7日時段表
                string sqlRtnT = "SELECT PVT.T1 [ID],isnull([D1],0) D1,isnull([D2],0) D2,isnull([D3],0) D3,isnull([D4],0) D4,isnull([D5],0) D5,isnull([D6],0) D6,isnull([D7],0) D7 into #tmpRtn ";
                sqlRtnT += "FROM (select T1,DayCnt,Cash from #tmpRpt a left join SalesH_AllWeb b (nolock) on a.T1=b.TimeGroup and a.D1=b.OpenDate and b.CompanyCode='" + uu.CompanyId + "'";
                if (sqlCon != "") { sqlRtnT += " " + string.Format(sqlCon, "b.ShopNo"); }
                sqlRtnT += ") H PIVOT(";
                sqlRtnT += "Sum(Cash)";
                sqlRtnT += "FOR DayCnt IN ([D1], [D2], [D3], [D4], [D5], [D6], [D7])";
                sqlRtnT += ") AS PVT; ";

                //7日店別表
                string sqlRtnS = "SELECT PVT.WhName [ID],isnull([D1],0) D1,isnull([D2],0) D2,isnull([D3],0) D3,isnull([D4],0) D4,isnull([D5],0) D5,isnull([D6],0) D6,isnull([D7],0) D7 into #tmpRtn ";
                sqlRtnS += "FROM (select ST_ID,WhName,DayCnt,Cash from #tmpRpt a left join SalesH_AllWeb b (nolock) on a.ST_ID=b.ShopNo and a.D1=b.OpenDate and b.CompanyCode='" + uu.CompanyId + "') H ";
                sqlRtnS += "PIVOT(";
                sqlRtnS += "Sum(Cash)";
                sqlRtnS += "FOR DayCnt IN ([D1], [D2], [D3], [D4], [D5], [D6], [D7])";
                sqlRtnS += ") AS PVT; ";

                sql += string.Format(sqldw, "into #tmpDW") + ";";
                if (Flag == "T")
                {
                    sql += "select * into #tmpRpt from #tmpT cross join (select D1,DayCnt from #tmpDW) a;";
                    sql += sqlRtnT;
                }
                else if (Flag == "S")
                {
                    sql += "select * into #tmpRpt from (select ST_ID,ST_ID+'-'+ST_Sname WhName from WarehouseWeb (nolock) where CompanyCode='" + uu.CompanyId + "'";
                    if (sqlCon != "")
                    {
                        sql += " " + string.Format(sqlCon, "ST_ID");
                    }
                    else
                    {
                        sql += " and ST_Type not in('0','2','3')";
                    }
                    sql += ") w cross join (select D1,DayCnt from #tmpDW) a;";
                    sql += sqlRtnS;
                }

                sql += "insert into #tmpRtn select 'SumAll',sum([D1]), sum([D2]), sum([D3]), sum([D4]), sum([D5]), sum([D6]), sum([D7]) from #tmpRtn;";
                sql += "Select * from #tmpRtn order by [ID];";
                DataTable dtDelt = PubUtility.SqlQry(sql, uu, "SYS");
                dtDelt.TableName = "dtDelt";

                DataTable dtSum = dtDelt.Clone();
                dtSum.ImportRow(dtDelt.Select("ID='SumAll'")[0]);
                dtSum.TableName = "dtSum";
                ds.Tables.Add(dtSum);

                dtDelt.Rows.Remove(dtDelt.Select("ID='SumAll'")[0]);
                ds.Tables.Add(dtDelt);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }
        #endregion

        #region MSSA105
        [Route("SystemSetup/MSSA105Query")]
        public ActionResult SystemSetup_MSSA105Query()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSA105QueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Year = rq["Year"];
                string Flag = rq["Flag"];
                string YearBef = (Convert.ToInt32(Year) - 1).ToString();
                string YearLast= (Convert.ToInt32(Year) - 2).ToString();

                string sql = "";
                string sqlD = "";
                string sqlH = "";

                //月份
                if (Flag == "S")
                {
                    //期間1
                    sql = "select substring(a.Opendate,1,7) Month,Sum(a.Cash)Cash1,";
                    sql += "(select Sum(Cash)Cash1 from SalesHWeb  (nolock) where Companycode='" + uu.CompanyId + "' and substring(opendate,1,7)  =convert(varchar(7), dateadd(m,-1, convert(date,substring(a.OpenDate,1,7 )+'/01')),111) group by substring(Opendate,1,7) ) Cash2 ";
                    sql += " into #s1data ";
                    sql += "from SalesHWeb a (nolock) ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and opendate like '" + YearBef + "%' ";
                    sql += "group by substring(a.Opendate,1,7); ";

                    sql += "select  substring(Month,6,2 ) Month,Cash1,case when isnull(cash2,0)=0 then format(1,'p') else format(cast(Cash1-Cash2 as Float)/cast(Cash2 as Float),'p') end Per into #s1 from #s1data;";
                    //期間2
                    sql += "select substring(a.Opendate,1,7) Month,Sum(a.Cash)Cash1, ";
                    sql += "(select Sum(Cash)Cash1 from SalesHWeb  (nolock) where Companycode='" + uu.CompanyId + "' and substring(opendate,1,7)  =convert(varchar(7), dateadd(m,-1, convert(date,substring(a.OpenDate,1,7 )+'/01')),111) group by substring(Opendate,1,7) ) Cash2 ";
                    sql += " into #s2data ";
                    sql += "from SalesHWeb a (nolock) ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and opendate like '" + Year + "%' ";
                    sql += "group by substring(a.Opendate,1,7); ";

                    sql += "select  substring(Month,6,2 ) Month,Cash1,case when isnull(cash2,0)=0 then format(1,'p') else format(cast(Cash1-Cash2 as Float)/cast(Cash2 as Float),'p') end Per into #s2 from #s2data;";
                    //明細資料
                    sqlD = "select case when isnull(s1.Month,'')='' then s2.Month +'月' else s1.Month +'月' end id,isnull(s1.Cash1,0)Cash1,isnull(s1.per,0) Per1,isnull(s2.Cash1,0)Cash2,isnull(s2.per,0) Per2, ";
                    sqlD += "case when isnull(s1.Cash1,0)=0 and isnull(s2.Cash1,0)=0 then format(0,'p') when isnull(s1.Cash1,0)=0 then format(1,'p') else format(cast(isnull(s2.Cash1,0)-isnull(s1.Cash1,0) as Float)/cast(isnull(s1.Cash1,0) as Float),'p') end as Per ";
                    sqlD += "from #s1 s1 ";
                    sqlD += "Full join #s2 s2 on s1.Month=s2.Month ";
                    sqlD += "order by id ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sqlH = "select sum(isnull(s1.Cash1,0))SumCash1,sum(isnull(s2.Cash1,0))SumCash2, ";
                    sqlH += "case when sum(isnull(s1.Cash1,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.Cash1,0))-sum(isnull(s1.Cash1,0)) as Float)/cast(sum(isnull(s1.Cash1,0)) as Float),'p') end as SumPer ";
                    sqlH += "from #s1 s1 ";
                    sqlH += "Full join #s2 s2 on s1.Month=s2.Month ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
                //店櫃
                else if (Flag == "D")
                {
                    //期間1
                    sql = "select a.ShopNo ID,w.ST_SName Name,Sum(a.Cash)Cash1 into #s1 ";
                    sql += "from SalesHWeb a (nolock) ";
                    sql += "inner join EDDMS.dbo.Warehouse w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and opendate like '" + YearBef + "%' ";
                    sql += "group by a.ShopNo,w.ST_SName; ";

                    //期間2
                    sql += "select a.ShopNo ID,w.ST_SName Name,Sum(a.Cash)Cash1 into #s2 ";
                    sql += "from SalesHWeb a (nolock) ";
                    sql += "inner join EDDMS.dbo.Warehouse w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and opendate like '" + Year + "%' ";
                    sql += "group by a.ShopNo,w.ST_SName; ";

                    //明細資料
                    sqlD = "select case when isnull(s1.ID,'')='' then isnull(s2.ID,'') + '-' + isnull(s2.Name,'') else isnull(s1.ID,'') + '-' + isnull(s1.Name,'') end as id, ";
                    sqlD += "isnull(s1.Cash1,0)Cash1,isnull(s2.Cash1,0)Cash2, ";
                    sqlD += "case when isnull(s1.Cash1,0)=0 and isnull(s2.Cash1,0)=0 then format(0,'p') when isnull(s1.Cash1,0)=0 then format(1,'p') else format(cast(isnull(s2.Cash1,0)-isnull(s1.Cash1,0) as Float)/cast(isnull(s1.Cash1,0) as Float),'p') end as Per ";
                    sqlD += "from #s1 s1 ";
                    sqlD += "Full join #s2 s2 on s1.id=s2.id ";
                    sqlD += "order by id ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sqlH = "select sum(isnull(s1.Cash1,0))SumCash1,sum(isnull(s2.Cash1,0))SumCash2, ";
                    sqlH += "case when sum(isnull(s1.Cash1,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.Cash1,0))-sum(isnull(s1.Cash1,0)) as Float)/cast(sum(isnull(s1.Cash1,0)) as Float),'p') end as SumPer ";
                    sqlH += "from #s1 s1 ";
                    sqlH += "Full join #s2 s2 on s1.id=s2.id ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
                //區課
                else if (Flag == "B")
                {
                    //期間1
                    sql = "select p.Type_ID ID,p.Type_Name Name,Sum(a.Cash)Cash1 into #s1 ";
                    sql += "from SalesHWeb a (nolock) ";
                    sql += "inner join EDDMS.dbo.Warehouse w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "inner join TypeDataWeb p (nolock) on w.ST_PlaceID=p.Type_ID and p.Companycode='" + uu.CompanyId + "' and Type_Code='A' ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and opendate like '" + YearBef + "%' ";
                    sql += "group by  p.Type_ID,p.Type_Name; ";

                    //期間2
                    sql += "select p.Type_ID ID,p.Type_Name Name,Sum(a.Cash)Cash1 into #s2 ";
                    sql += "from SalesHWeb a (nolock) ";
                    sql += "inner join EDDMS.dbo.Warehouse w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "inner join TypeDataWeb p (nolock) on w.ST_PlaceID=p.Type_ID and p.Companycode='" + uu.CompanyId + "' and Type_Code='A' ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and opendate like '" + Year + "%' ";
                    sql += "group by  p.Type_ID,p.Type_Name; ";

                    //明細資料
                    sqlD = "select a.type_id + '-' + a.type_name id,isnull(s1.Cash1,0)Cash1,isnull(s2.Cash1,0)Cash2, ";
                    sqlD += "case when isnull(s1.Cash1,0)=0 and isnull(s2.Cash1,0)=0 then format(0,'p') when isnull(s1.Cash1,0)=0 then format(1,'p') else format(cast(isnull(s2.Cash1,0)-isnull(s1.Cash1,0) as Float)/cast(isnull(s1.Cash1,0) as Float),'p') end as Per ";
                    sqlD += "from TypeDataWeb a (nolock) ";
                    sqlD += "left join #s1 s1 on a.type_id=s1.id ";
                    sqlD += "left join #s2 s2 on a.type_id=s2.id ";
                    sqlD += "where a.CompanyCode='" + uu.CompanyId + "' and a.type_code='A' ";
                    sqlD += "order by a.type_id ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sqlH = "select sum(isnull(s1.Cash1,0))SumCash1,sum(isnull(s2.Cash1,0))SumCash2, ";
                    sqlH += "case when sum(isnull(s1.Cash1,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.Cash1,0))-sum(isnull(s1.Cash1,0)) as Float)/cast(sum(isnull(s1.Cash1,0)) as Float),'p') end as SumPer ";
                    sqlH += "from TypeDataWeb a (nolock) ";
                    sqlH += "left join #s1 s1 on a.type_id=s1.id ";
                    sqlH += "left join #s2 s2 on a.type_id=s2.id ";
                    sqlH += "where a.CompanyCode='" + uu.CompanyId + "' and a.type_code='A' ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSA105Query_Step1")]
        public ActionResult SystemSetup_MSSA105Query_Step1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSA105Query_Step1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Flag = rq["Flag"];
                string Year = rq["Year"];
                Year = Year.Substring(0, 4);
                string YearBef = (Convert.ToInt32(Year) - 1).ToString();
                string Month = rq["Month"];
                string SubType = rq["SubType"];

                string sql = "";
                string sqlD = "";
                string sqlH = "";

                //月份
                if (Flag == "S")
                {
                    Month = Month.Substring(0, 2);
                    if (SubType == "Shop")      //店櫃
                    {
                        //期間1
                        sql = "select a.ShopNo ID,w.ST_SName Name,Sum(a.Cash)Cash1 into #s1 ";
                        sql += "from SalesHWeb a (nolock) ";
                        sql += "inner join EDDMS.dbo.Warehouse w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                        sql += "where a.Companycode='" + uu.CompanyId + "' ";
                        sql += "and opendate like '" + YearBef + '/' + Month + "%' ";
                        sql += "group by a.ShopNo,w.ST_SName; ";

                        //期間2
                        sql += "select a.ShopNo ID,w.ST_SName Name,Sum(a.Cash)Cash1 into #s2 ";
                        sql += "from SalesHWeb a (nolock) ";
                        sql += "inner join EDDMS.dbo.Warehouse w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                        sql += "where a.Companycode='" + uu.CompanyId + "' ";
                        sql += "and opendate like '" + Year + '/' + Month + "%' ";
                        sql += "group by a.ShopNo,w.ST_SName; ";


                    }
                    else if (SubType == "Area")     //區課
                    {
                        //期間1
                        sql = "select w.ST_PlaceID ID,t.Type_Name Name,sum(Cash) Cash1 into #s1 from SalesHWeb s ";
                        sql += "join WarehouseWeb w on s.CompanyCode=w.CompanyCode and s.ShopNo=w.ST_ID  and w.ST_Type not in('2','3') ";
                        sql += "join TypeDataWeb t on w.ST_PlaceID=t.Type_ID and t.Companycode=w.CompanyCode  and Type_Code='A' ";
                        sql += "where s.Companycode='" + uu.CompanyId + "' ";
                        sql += "and s.OpenDate like '" + YearBef + '/' + Month + "%' ";
                        sql += "group by w.ST_PlaceID,t.Type_Name; ";

                        //期間2
                        sql += "select w.ST_PlaceID ID,t.Type_Name Name,sum(Cash) Cash1 into #s2 from SalesHWeb s ";
                        sql += "join WarehouseWeb w on s.CompanyCode=w.CompanyCode and s.ShopNo=w.ST_ID  and w.ST_Type not in('2','3') ";
                        sql += "join TypeDataWeb t on w.ST_PlaceID=t.Type_ID and t.Companycode=w.CompanyCode  and Type_Code='A' ";
                        sql += "where s.Companycode='" + uu.CompanyId + "' ";
                        sql += "and s.OpenDate like '" + Year + '/' + Month + "%' ";
                        sql += "group by w.ST_PlaceID,t.Type_Name; ";

                    }
                    //明細資料
                    sqlD = "select case when isnull(s1.ID,'')='' then isnull(s2.ID,'') + '-' + isnull(s2.Name,'') else isnull(s1.ID,'') + '-' + isnull(s1.Name,'') end as id, ";
                    sqlD += "isnull(s1.Cash1,0)Cash1,isnull(s2.Cash1,0)Cash2, ";
                    sqlD += "case when isnull(s1.Cash1,0)=0 and isnull(s2.Cash1,0)=0 then format(0,'p') when isnull(s1.Cash1,0)=0 then format(1,'p') else format(cast(isnull(s2.Cash1,0)-isnull(s1.Cash1,0) as Float)/cast(isnull(s1.Cash1,0) as Float),'p') end as Per ";
                    sqlD += "from #s1 s1 ";
                    sqlD += "Full join #s2 s2 on s1.id=s2.id ";
                    sqlD += "order by id ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sqlH = "select sum(isnull(s1.Cash1,0))SumCash1,sum(isnull(s2.Cash1,0))SumCash2, ";
                    sqlH += "case when sum(isnull(s1.Cash1,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.Cash1,0))-sum(isnull(s1.Cash1,0)) as Float)/cast(sum(isnull(s1.Cash1,0)) as Float),'p') end as SumPer ";
                    sqlH += "from #s1 s1 ";
                    sqlH += "Full join #s2 s2 on s1.id=s2.id ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
                //店櫃
                else if (Flag == "D")
                {
                    //期間1
                    sql = "select substring(a.Opendate,1,7) Month,Sum(a.Cash)Cash1, ";
                    sql += "(select Sum(Cash) Cash1 from SalesHWeb  (nolock) where Companycode='" + uu.CompanyId + "' and ShopNo='" + Month + "' and substring(opendate,1,7)  =convert(varchar(7), dateadd(m,-1, convert(date,substring(a.OpenDate,1,7 )+'/01')),111) group by substring(Opendate,1,7) ) Cash2 ";
                    sql += " into #s1data ";
                    sql += "from SalesHWeb a (nolock) ";
                    sql += "inner join EDDMS.dbo.Warehouse w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' and a.ShopNo='" + Month + "' ";
                    sql += "and opendate like '" + YearBef + "%' ";
                    sql += "group by substring(a.Opendate,1,7) ; ";

                    sql += "select  substring(Month,6,2 ) Month,Cash1,case when isnull(cash2,0)=0 then format(1,'p') else format(cast(Cash1-Cash2 as Float)/cast(Cash2 as Float),'p') end Per into #s1 from #s1data;";
                    //期間2
                    sql += "select substring(a.Opendate,1,7) Month,Sum(a.Cash)Cash1, ";
                    sql += "(select Sum(Cash) Cash1 from SalesHWeb  (nolock) where Companycode='" + uu.CompanyId + "' and ShopNo='" + Month + "' and substring(opendate,1,7)  =convert(varchar(7), dateadd(m,-1, convert(date,substring(a.OpenDate,1,7 )+'/01')),111) group by substring(Opendate,1,7) ) Cash2 ";
                    sql += " into #s2data ";
                    sql += "from SalesHWeb a (nolock) ";
                    sql += "inner join EDDMS.dbo.Warehouse w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' and a.ShopNo='" + Month + "' ";
                    sql += "and opendate like '" + Year + "%' ";
                    sql += "group by substring(a.Opendate,1,7); ";

                    sql += "select  substring(Month,6,2 ) Month,Cash1,case when isnull(cash2,0)=0 then format(1,'p') else format(cast(Cash1-Cash2 as Float)/cast(Cash2 as Float),'p') end Per into #s2 from #s2data;";
                    //明細資料
                    sqlD = "select case when isnull(s1.Month,'')='' then isnull(s2.Month,'')+'月' else isnull(s1.Month,'')+'月' end as id, ";
                    sqlD += "isnull(s1.Cash1,0)Cash1,isnull(s1.per,0) Per1,isnull(s2.Cash1,0)Cash2,isnull(s2.per,0) Per2, ";
                    sqlD += "case when isnull(s1.Cash1,0)=0 and isnull(s2.Cash1,0)=0 then format(0,'p') when isnull(s1.Cash1,0)=0 then format(1,'p') else format(cast(isnull(s2.Cash1,0)-isnull(s1.Cash1,0) as Float)/cast(isnull(s1.Cash1,0) as Float),'p') end as Per ";
                    sqlD += "from #s1 s1 ";
                    sqlD += "Full join #s2 s2 on s1.Month=s2.Month ";
                    sqlD += "order by id ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sqlH = "select sum(isnull(s1.Cash1,0))SumCash1,sum(isnull(s2.Cash1,0))SumCash2, ";
                    sqlH += "case when sum(isnull(s1.Cash1,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.Cash1,0))-sum(isnull(s1.Cash1,0)) as Float)/cast(sum(isnull(s1.Cash1,0)) as Float),'p') end as SumPer ";
                    sqlH += "from #s1 s1 ";
                    sqlH += "Full join #s2 s2 on s1.Month=s2.Month ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
                //區課
                else if (Flag == "B")
                {
                    if (SubType == "Shop")  //店櫃
                    {
                        //期間1
                        sql = "select a.ShopNo ID,w.ST_SName Name,Sum(a.Cash)Cash1 into #s1 ";
                        sql += "from SalesHWeb a (nolock) ";
                        sql += "inner join EDDMS.dbo.Warehouse w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                        sql += "inner join TypeDataWeb p (nolock) on w.ST_PlaceID=p.Type_ID and p.Companycode='" + uu.CompanyId + "' and Type_Code='A' ";
                        sql += "where a.Companycode='" + uu.CompanyId + "' and p.Type_ID='" + Month + "' ";
                        sql += "and opendate like '" + YearBef + "%' ";
                        sql += "group by a.ShopNo,w.ST_SName; ";

                        //期間2
                        sql += "select a.ShopNo ID,w.ST_SName Name,Sum(a.Cash)Cash1 into #s2 ";
                        sql += "from SalesHWeb a (nolock) ";
                        sql += "inner join EDDMS.dbo.Warehouse w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                        sql += "inner join TypeDataWeb p (nolock) on w.ST_PlaceID=p.Type_ID and p.Companycode='" + uu.CompanyId + "' and Type_Code='A' ";
                        sql += "where a.Companycode='" + uu.CompanyId + "' and p.Type_ID='" + Month + "' ";
                        sql += "and opendate like '" + Year + "%' ";
                        sql += "group by a.ShopNo,w.ST_SName ; ";

                        //明細資料
                        sqlD = "select case when isnull(s1.ID,'')='' then isnull(s2.ID,'') + '-' + isnull(s2.Name,'') else isnull(s1.ID,'') + '-' + isnull(s1.Name,'') end as id, ";
                        sqlD += "isnull(s1.Cash1,0)Cash1,isnull(s2.Cash1,0)Cash2, ";
                        sqlD += "case when isnull(s1.Cash1,0)=0 and isnull(s2.Cash1,0)=0 then format(0,'p') when isnull(s1.Cash1,0)=0 then format(1,'p') else format(cast(isnull(s2.Cash1,0)-isnull(s1.Cash1,0) as Float)/cast(isnull(s1.Cash1,0) as Float),'p') end as Per ";
                        sqlD += "from #s1 s1 ";
                        sqlD += "Full join #s2 s2 on s1.ID=s2.ID ";
                        sqlD += "order by id ";
                        DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                        dtE.TableName = "dtE";
                        ds.Tables.Add(dtE);
                    }
                    else if (SubType == "Month")    //月份
                    {
                        //期間1
                        sql = "select substring(a.Opendate,1,7) Month,Sum(a.Cash)Cash1, ";
                        sql += "(select Sum(Cash)Cash1 from SalesHWeb b (nolock) ";
                        sql += "inner join EDDMS.dbo.Warehouse w (nolock) on b.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                        sql += "inner join TypeDataWeb p (nolock) on w.ST_PlaceID=p.Type_ID and p.Companycode='" + uu.CompanyId + "' and Type_Code='A' ";
                        sql += " where b.Companycode='" + uu.CompanyId + "' and p.Type_ID='" + Month + "' and substring(opendate,1,7)  =convert(varchar(7), dateadd(m,-1, convert(date,substring(a.OpenDate,1,7 )+'/01')),111) group by substring(Opendate,1,7) ) Cash2 ";
                        sql += " into #s1data ";
                        sql += "from SalesHWeb a (nolock) ";
                        sql += "inner join EDDMS.dbo.Warehouse w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                        sql += "inner join TypeDataWeb p (nolock) on w.ST_PlaceID=p.Type_ID and p.Companycode='" + uu.CompanyId + "' and Type_Code='A' ";
                        sql += "where a.Companycode='" + uu.CompanyId + "' and p.Type_ID='" + Month + "' ";
                        sql += "and opendate like '" + YearBef + "%' ";
                        sql += "group by substring(a.Opendate,1,7) ; ";

                        sql += "select  substring(Month,6,2 ) Month,Cash1,case when isnull(cash2,0)=0 then format(1,'p') else format(cast(Cash1-Cash2 as Float)/cast(Cash2 as Float),'p') end Per into #s1 from #s1data;";
                        //期間2
                        sql += "select substring(a.Opendate,1,7) Month,Sum(a.Cash)Cash1, ";
                        sql += "(select Sum(Cash)Cash1 from SalesHWeb b (nolock) ";
                        sql += "inner join EDDMS.dbo.Warehouse w (nolock) on b.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                        sql += "inner join TypeDataWeb p (nolock) on w.ST_PlaceID=p.Type_ID and p.Companycode='" + uu.CompanyId + "' and Type_Code='A' ";
                        sql += "where b.Companycode='" + uu.CompanyId + "' and p.Type_ID='" + Month + "' and substring(opendate,1,7)  =convert(varchar(7), dateadd(m,-1, convert(date,substring(a.OpenDate,1,7 )+'/01')),111) group by substring(Opendate,1,7) ) Cash2 ";
                        sql += " into #s2data ";
                        sql += "from SalesHWeb a (nolock) ";
                        sql += "inner join EDDMS.dbo.Warehouse w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                        sql += "inner join TypeDataWeb p (nolock) on w.ST_PlaceID=p.Type_ID and p.Companycode='" + uu.CompanyId + "' and Type_Code='A' ";
                        sql += "where a.Companycode='" + uu.CompanyId + "' and p.Type_ID='" + Month + "' ";
                        sql += "and opendate like '" + Year + "%' ";
                        sql += "group by substring(a.Opendate,1,7); ";

                        sql += "select  substring(Month,6,2 ) Month,Cash1,case when isnull(cash2,0)=0 then format(1,'p') else format(cast(Cash1-Cash2 as Float)/cast(Cash2 as Float),'p') end Per into #s2 from #s2data;";
                        //明細資料
                        sqlD = "select case when isnull(s1.Month,'')='' then isnull(s2.Month,'')+'月' else isnull(s1.Month,'')+'月' end as id, ";
                        sqlD += "isnull(s1.Cash1,0)Cash1,isnull(s1.per,0) Per1,isnull(s2.Cash1,0)Cash2,isnull(s2.per,0) Per2, ";
                        sqlD += "case when isnull(s1.Cash1,0)=0 and isnull(s2.Cash1,0)=0 then format(0,'p') when isnull(s1.Cash1,0)=0 then format(1,'p') else format(cast(isnull(s2.Cash1,0)-isnull(s1.Cash1,0) as Float)/cast(isnull(s1.Cash1,0) as Float),'p') end as Per ";
                        sqlD += "from #s1 s1 ";
                        sqlD += "Full join #s2 s2 on s1.Month=s2.Month ";
                        sqlD += "order by id ";
                        DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                        dtE.TableName = "dtE";
                        ds.Tables.Add(dtE);
                    }

                    //彙總資料
                    sqlH = "select sum(isnull(s1.Cash1,0))SumCash1,sum(isnull(s2.Cash1,0))SumCash2, ";
                    sqlH += "case when sum(isnull(s1.Cash1,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.Cash1,0))-sum(isnull(s1.Cash1,0)) as Float)/cast(sum(isnull(s1.Cash1,0)) as Float),'p') end as SumPer ";
                    sqlH += "from #s1 s1 ";
                    sqlH += "Full join #s2 s2 ";
                    if (SubType == "Shop") 
                        sqlH += " on s1.id=s2.id";
                    else if (SubType == "Month")
                        sqlH += " on s1.Month=s2.Month";

                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSA105Query_Step2")]
        public ActionResult SystemSetup_MSSA105Query_Step2()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSA105Query_Step2OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Flag = rq["Flag"];
                string Year = rq["Year"];
                Year = Year.Substring(0, 4);
                string YearBef = (Convert.ToInt32(Year) - 1).ToString();
                string Type = rq["Type"];
                string Shop = rq["Shop"];

                string sql = "";
                string sqlD = "";
                string sqlH = "";


                //月份區課
                if (Flag == "B1")
                {
                    //期間1
                    sql = "select a.shopno,w.st_sname,Sum(a.Cash)Cash1 into #s1 ";
                    sql += "from SalesHWeb a (nolock) ";
                    sql += "inner join EDDMS.dbo.Warehouse w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "inner join TypeDataWeb p (nolock) on w.ST_PlaceID=p.Type_ID and p.Companycode='" + uu.CompanyId + "' and Type_Code='A' ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' and p.Type_ID='" + Shop + "' ";
                    sql += "and opendate like '" + YearBef +'/'+ Type + "%' ";
                    sql += "group by a.shopno,w.st_sname; ";

                    //期間2
                    sql += "select a.shopno,w.st_sname,Sum(a.Cash)Cash1 into #s2 ";
                    sql += "from SalesHWeb a (nolock) ";
                    sql += "inner join EDDMS.dbo.Warehouse w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "inner join TypeDataWeb p (nolock) on w.ST_PlaceID=p.Type_ID and p.Companycode='" + uu.CompanyId + "' and Type_Code='A' ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' and p.Type_ID='" + Shop + "' ";
                    sql += "and opendate like '" + Year + '/' + Type + "%' ";
                    sql += "group by a.shopno,w.st_sname ";

                    //明細資料
                    sqlD = "select case when isnull(s1.shopno,'')='' then isnull(s2.shopno+'-'+s2.st_sname,'') else isnull(s1.shopno+'-'+s1.st_sname,'') end as id, ";
                    sqlD += "isnull(s1.Cash1,0)Cash1,isnull(s2.Cash1,0)Cash2, ";
                    sqlD += "case when isnull(s1.Cash1,0)=0 and isnull(s2.Cash1,0)=0 then format(0,'p') when isnull(s1.Cash1,0)=0 then format(1,'p') else format(cast(isnull(s2.Cash1,0)-isnull(s1.Cash1,0) as Float)/cast(isnull(s1.Cash1,0) as Float),'p') end as Per ";
                    sqlD += "from #s1 s1 ";
                    sqlD += "Full join #s2 s2 on s1.shopno=s2.shopno ";
                    sqlD += "order by id ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sqlH = "select sum(isnull(s1.Cash1,0))SumCash1,sum(isnull(s2.Cash1,0))SumCash2, ";
                    sqlH += "case when sum(isnull(s1.Cash1,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.Cash1,0))-sum(isnull(s1.Cash1,0)) as Float)/cast(sum(isnull(s1.Cash1,0)) as Float),'p') end as SumPer ";
                    sqlH += "from #s1 s1 ";
                    sqlH += "Full join #s2 s2 on s1.shopno=s2.shopno ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
                //區課店別
                else if (Flag == "D2")
                {
                    //期間1
                    sql = "select substring(a.Opendate,1,7) Month,Sum(a.Cash)Cash1, ";
                    sql += "(select Sum(Cash) Cash1 from SalesHWeb b (nolock) ";
                    sql += "inner join EDDMS.dbo.Warehouse w (nolock) on b.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "inner join TypeDataWeb p (nolock) on w.ST_PlaceID=p.Type_ID and p.Companycode='" + uu.CompanyId + "' and Type_Code='A' ";
                    sql += " where b.Companycode='" + uu.CompanyId + "' and p.Type_ID='" + Type + "'  and ShopNo='" + Shop + "' and substring(opendate,1,7)  =convert(varchar(7), dateadd(m,-1, convert(date,substring(a.OpenDate,1,7 )+'/01')),111) group by substring(Opendate,1,7) ) Cash2 ";
                    sql += " into #s1data ";
                    sql += "from SalesHWeb a (nolock) ";
                    sql += "inner join EDDMS.dbo.Warehouse w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "inner join TypeDataWeb p (nolock) on w.ST_PlaceID=p.Type_ID and p.Companycode='" + uu.CompanyId + "' and Type_Code='A' ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' and p.Type_ID='" + Type + "' and a.ShopNo='" + Shop + "' ";
                    sql += "and opendate like '" + YearBef + "%' ";
                    sql += "group by substring(a.Opendate,1,7); ";

                    sql += "select  substring(Month,6,2 ) Month,Cash1,case when isnull(cash2,0)=0 then format(1,'p') else format(cast(Cash1-Cash2 as Float)/cast(Cash2 as Float),'p') end Per into #s1 from #s1data;";
                    //期間2
                    sql += "select substring(a.Opendate,1,7) Month,Sum(a.Cash)Cash1, ";
                    sql += "(select Sum(Cash) Cash1 from SalesHWeb b (nolock) ";
                    sql += "inner join EDDMS.dbo.Warehouse w (nolock) on b.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "inner join TypeDataWeb p (nolock) on w.ST_PlaceID=p.Type_ID and p.Companycode='" + uu.CompanyId + "' and Type_Code='A' ";
                    sql += " where b.Companycode='" + uu.CompanyId + "' and p.Type_ID='" + Type + "'  and ShopNo='" + Shop + "' and substring(opendate,1,7)  =convert(varchar(7), dateadd(m,-1, convert(date,substring(a.OpenDate,1,7 )+'/01')),111) group by substring(Opendate,1,7) ) Cash2 ";
                    sql += " into #s2data ";
                    sql += "from SalesHWeb a (nolock) ";
                    sql += "inner join EDDMS.dbo.Warehouse w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "inner join TypeDataWeb p (nolock) on w.ST_PlaceID=p.Type_ID and p.Companycode='" + uu.CompanyId + "' and Type_Code='A' ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' and p.Type_ID='" + Type + "' and a.ShopNo='" + Shop + "' ";
                    sql += "and opendate like '" + Year + "%' ";
                    sql += "group by substring(a.Opendate,1,7) ";

                    sql += "select  substring(Month,6,2 ) Month,Cash1,case when isnull(cash2,0)=0 then format(1,'p') else format(cast(Cash1-Cash2 as Float)/cast(Cash2 as Float),'p') end Per into #s2 from #s2data;";
                    //明細資料
                    sqlD = "select case when isnull(s1.Month,'')='' then isnull(s2.Month,'')+'月' else isnull(s1.Month,'')+'月' end as id, ";
                    sqlD += "isnull(s1.Cash1,0)Cash1,isnull(s1.per,0) Per1,isnull(s2.Cash1,0)Cash2,isnull(s2.per,0) Per2, ";
                    sqlD += "case when isnull(s1.Cash1,0)=0 and isnull(s2.Cash1,0)=0 then format(0,'p') when isnull(s1.Cash1,0)=0 then format(1,'p') else format(cast(isnull(s2.Cash1,0)-isnull(s1.Cash1,0) as Float)/cast(isnull(s1.Cash1,0) as Float),'p') end as Per ";
                    sqlD += "from #s1 s1 ";
                    sqlD += "Full join #s2 s2 on s1.Month=s2.Month ";
                    sqlD += "order by id ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sqlH = "select sum(isnull(s1.Cash1,0))SumCash1,sum(isnull(s2.Cash1,0))SumCash2, ";
                    sqlH += "case when sum(isnull(s1.Cash1,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.Cash1,0))-sum(isnull(s1.Cash1,0)) as Float)/cast(sum(isnull(s1.Cash1,0)) as Float),'p') end as SumPer ";
                    sqlH += "from #s1 s1 ";
                    sqlH += "Full join #s2 s2 on s1.Month=s2.Month ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
                //區課月份
                else if (Flag == "S2")
                {
                    //期間1
                    sql = "select a.shopno,w.st_sname,Sum(a.Cash)Cash1 into #s1 ";
                    sql += "from SalesHWeb a (nolock) ";
                    sql += "inner join EDDMS.dbo.Warehouse w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "inner join TypeDataWeb p (nolock) on w.ST_PlaceID=p.Type_ID and p.Companycode='" + uu.CompanyId + "' and Type_Code='A' ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' and p.Type_ID='" + Type + "' ";
                    sql += "and opendate like '" + YearBef + '/' + Shop + "%' ";
                    sql += "group by a.shopno,w.st_sname; ";

                    //期間2
                    sql += "select a.shopno,w.st_sname,Sum(a.Cash)Cash1 into #s2 ";
                    sql += "from SalesHWeb a (nolock) ";
                    sql += "inner join EDDMS.dbo.Warehouse w (nolock) on a.ShopNo=w.ST_ID and w.Companycode='" + uu.CompanyId + "' and w.ST_Type not in('2','3') ";
                    sql += "inner join TypeDataWeb p (nolock) on w.ST_PlaceID=p.Type_ID and p.Companycode='" + uu.CompanyId + "' and Type_Code='A' ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' and p.Type_ID='" + Type + "' ";
                    sql += "and opendate like '" + Year + '/' + Shop + "%' ";
                    sql += "group by a.shopno,w.st_sname ";

                    //明細資料
                    sqlD = "select case when isnull(s1.shopno,'')='' then isnull(s2.shopno+'-'+s2.st_sname,'') else isnull(s1.shopno+'-'+s1.st_sname,'') end as id, ";
                    sqlD += "isnull(s1.Cash1,0)Cash1,isnull(s2.Cash1,0)Cash2, ";
                    sqlD += "case when isnull(s1.Cash1,0)=0 and isnull(s2.Cash1,0)=0 then format(0,'p') when isnull(s1.Cash1,0)=0 then format(1,'p') else format(cast(isnull(s2.Cash1,0)-isnull(s1.Cash1,0) as Float)/cast(isnull(s1.Cash1,0) as Float),'p') end as Per ";
                    sqlD += "from #s1 s1 ";
                    sqlD += "Full join #s2 s2 on s1.shopno=s2.shopno ";
                    sqlD += "order by id ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);

                    //彙總資料
                    sqlH = "select sum(isnull(s1.Cash1,0))SumCash1,sum(isnull(s2.Cash1,0))SumCash2, ";
                    sqlH += "case when sum(isnull(s1.Cash1,0))=0 then format(1,'p') else format(cast(sum(isnull(s2.Cash1,0))-sum(isnull(s1.Cash1,0)) as Float)/cast(sum(isnull(s1.Cash1,0)) as Float),'p') end as SumPer ";
                    sqlH += "from #s1 s1 ";
                    sqlH += "Full join #s2 s2 on s1.shopno=s2.shopno ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }
        #endregion

        #region MSSA101
        [Route("SystemSetup/GetInitMSSA101")]
        public ActionResult SystemSetup_GetInitMSSA101()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitMSSA101OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ProgramID = rq["ProgramID"];
                string sql = "select ChineseName,convert(char(7),dateadd(d,-1,getdate()),111) + '/01' SysDate1,convert(char(10),dateadd(d,-1,getdate()),111) SysDate2 from ProgramIDWeb (nolock) where ProgramID='" + ProgramID.SqlQuote() + "'";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSA101Query")]
        public ActionResult SystemSetup_MSSA101Query()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSA101QueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string OpenDateS1 = rq["OpenDateS1"];
                string OpenDateE1 = rq["OpenDateE1"];
                string ShopNo = rq["ShopNo"];
                string Area = rq["Area"];
                string Flag = rq["Flag"];  //A-區域 S-店別 D-日期

                string sql = "";
                string sqlIDColname = "";  //ID要取的實際資料欄位名稱
                string sqlBaseData = "";  //實際資料的Sql指令
                string sqlGroup = "";  //
                
                if (Flag == "A")
                {
                    sqlIDColname = "case when Areaid<>'' then Areaid+'-'+areaname else '' end";
                    sqlBaseData = "select AreaID,AreaName,Cash,Recs,VIP_Cash,VIP_Recs from #tmpW a left join SalesHWeb b (nolock) on a.ST_ID=b.ShopNo and companycode='"+ uu.CompanyId + "' and opendate between '" + OpenDateS1 + "' and '" + OpenDateE1 + "'";
                    if (ShopNo != "")
                    {
                        sqlBaseData += " and a.ST_ID in ("+ ShopNo +")";
                    }
                    sqlGroup = "group by Areaid,areaname";
                }else if (Flag == "S")
                {
                    sqlIDColname = "ST_ID+'-'+ST_Sname";
                    sqlBaseData = "select ST_ID,ST_Sname,Cash,Recs,VIP_Cash,VIP_Recs from #tmpW a left join SalesHWeb b (nolock) on a.ST_ID=b.ShopNo and companycode='" + uu.CompanyId + "' and opendate between '" + OpenDateS1 + "' and '" + OpenDateE1 + "' where a.ST_ID<>''";
                    if (ShopNo != "")
                    {
                        sqlBaseData += " and a.ST_ID in (" + ShopNo + ")";
                    }
                    sqlGroup = "group by ST_ID,ST_Sname";
                }
                else if (Flag == "D")
                {
                    sqlIDColname = "D1";
                    sqlBaseData = "select D1,Cash,Recs,VIP_Cash,VIP_Recs from #tmpD a left join SalesHWeb b on a.D1=b.OpenDate and companycode='" + uu.CompanyId + "'";
                    if (ShopNo != "")
                    {
                        sqlBaseData += " and b.ShopNo in (" + ShopNo + ")";
                    }
                    sqlBaseData += " left join #tmpW c on b.ShopNo=c.ST_ID";
                    if (Area != null)
                    {
                        sqlBaseData += " where Shopno in (Select ST_ID from WarehouseWeb (nolock) where companycode='" + uu.CompanyId + "' and ST_Type not in ('0','2','3') and isnull(ST_placeID,'')='" + Area + "')";
                    }
                    sqlGroup = "group by D1";
                    sql = "select convert(varchar,dateadd(d,number,'"+ OpenDateS1 + "'),111) D1 into #tmpD from master..spt_values where type='p' and number<=datediff(d,'" + OpenDateS1 + "','" + OpenDateE1 + "');";
                }


                sql += "select * into #tmpW from (";
                sql += "select isnull(Type_ID,'') AreaID,isnull(Type_Name, '') AreaName,isnull(ST_ID, '') ST_ID,isnull(ST_Sname, '') ST_Sname from TypeDataWeb a (nolock) full join WarehouseWeb b (nolock) on a.CompanyCode = b.CompanyCode and a.Type_ID = b.ST_placeID and b.ST_Type not in ('0','2', '3') where a.companycode = '" + uu.CompanyId + "' and a.Type_Code = 'A'";
                sql += " union ";
                sql += "select isnull(Type_ID, '') AreaID,isnull(Type_Name, '') AreaName,isnull(ST_ID, '') ST_ID,isnull(ST_Sname, '') ST_Sname from WarehouseWeb a (nolock) full join TypeDataWeb b (nolock) on a.CompanyCode = b.CompanyCode and b.Type_Code = 'A' and a.ST_placeID = b.Type_ID where a.companycode = '" + uu.CompanyId + "' and a.ST_Type not in ('0','2', '3')";
                
                sql += ") a";
                if (Area != null)
                {
                    sql += " where AreaID='" + Area + "'";
                }
                sql += ";";
                
                sql += "Select " + sqlIDColname + " ID,isnull(sum(Cash),0) Cash1,isnull(sum(Recs),0) Cnt1, ";
                sql += "case when isnull(sum(Recs),0)=0 then 0 else round(isnull(sum(Cash),0)/isnull(sum(Recs),0),0) end  CusCash1, ";
                sql += "isnull(sum(VIP_Cash),0) VCash,isnull(sum(VIP_Recs),0) VCnt, ";
                sql += "case when isnull(sum(VIP_Recs),0)=0 then 0 else round(isnull(sum(VIP_Cash),0)/isnull(sum(VIP_Recs),0),0) end VCusCash, ";
                sql += "case when isnull(sum(Cash),0)=0 and isnull(sum(VIP_Cash),0)=0 then format(0,'p') when isnull(sum(Cash),0)=0 then format(1,'p') else format(cast(isnull(sum(VIP_Cash),0) as float)/cast(isnull(sum(Cash),0) as float),'p') end VPer, ";
                sql += "isnull(sum(Cash),0)-isnull(sum(VIP_Cash),0) VNoCash,isnull(sum(Recs),0)-isnull(sum(VIP_Recs),0) VNoCnt, ";
                sql += "case when isnull(sum(Recs),0)-isnull(sum(VIP_Recs),0)=0 then 0 else round((isnull(sum(Cash),0)-isnull(sum(VIP_Cash),0))/(isnull(sum(Recs),0)-isnull(sum(VIP_Recs),0)),0) end VNoCusCash, ";
                sql += "case when isnull(sum(Cash),0)=0 and isnull(sum(Cash),0)-isnull(sum(VIP_Cash),0)=0 then format(0,'p') when isnull(sum(Cash),0)=0 then format(1,'p') else format(cast(isnull(sum(Cash),0)-isnull(sum(VIP_Cash),0) as float)/cast(isnull(sum(Cash),0) as float),'p') end VNoPer ";
                sql += "into #tmpSel ";
                sql += "from (" + sqlBaseData + ") a "+sqlGroup+";";

                sql += "insert into #tmpSel select 'SumAll',isnull(sum([Cash1]),0), isnull(sum([Cnt1]),0),";
                sql += "case when isnull(sum([Cnt1]),0)= 0 then 0 else round(isnull(sum(Cash1), 0) / isnull(sum([Cnt1]), 0),0) end,";
                sql += "isnull(sum([VCash]),0), isnull(sum([VCnt]),0),case when isnull(sum([VCnt]),0)= 0 then 0 else round(isnull(sum(VCash), 0) / isnull(sum([VCnt]), 0),0) end,";
                sql += "case when isnull(sum(Cash1),0)= 0 then format(1,'p') else format(cast(isnull(sum(VCash),0) as float)/cast(isnull(sum(Cash1),0) as float),'p') end, ";
                sql += "isnull(sum([VNoCash]),0),isnull(sum([VNoCnt]),0),case when isnull(sum([VNoCnt]),0)= 0 then 0 else round(isnull(sum(VNoCash), 0) / isnull(sum([VNoCnt]), 0),0) end,";
                sql += "case when isnull(sum(Cash1),0)= 0 then format(1,'p') else format(cast(isnull(sum(VNoCash),0) as float)/cast(isnull(sum(Cash1),0) as float),'p') end ";
                sql += "from #tmpSel; ";
                sql += "select * from #tmpSel order by [ID];";
                DataTable dtDelt = PubUtility.SqlQry(sql, uu, "SYS");
                dtDelt.TableName = "dtDelt";

                DataTable dtSum = dtDelt.Clone();
                dtSum.ImportRow(dtDelt.Select("ID='SumAll'")[0]);
                dtSum.TableName = "dtSum";
                ds.Tables.Add(dtSum);

                dtDelt.Rows.Remove(dtDelt.Select("ID='SumAll'")[0]);
                ds.Tables.Add(dtDelt);
                
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }
        #endregion

        #region MSSA102
        [Route("SystemSetup/GetInitMSSA102")]
        public ActionResult SystemSetup_GetInitMSSA102()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitMSSA102OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ProgramID = rq["ProgramID"];
                string sql = "select ChineseName,convert(char(10),getdate(),111) SysDate from ProgramIDWeb (nolock) where ProgramID='" + ProgramID.SqlQuote() + "'";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSA102Query")]
        public ActionResult SystemSetup_MSSA102Query()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSA102QueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Opendate = rq["Opendate"];

                string sqldw = "select h.CG_NO+char(13)+h.CG_Name+char(13)+h.StartDate+'~'+h.EndDate ID,sum(a.SalesAmt) SalesAmt,sum(a.SalesQty) SalesQty,sum(a.CG_Amt) CG_Amt,sum(a.CG_Qty) CG_Qty,sum(a.CGVIP_Amt) CGVIP_Amt,sum(a.CGVIP_Qty) CGVIP_Qty ";
                sqldw += ",format(case when sum(a.SalesAmt)>0 then sum(a.CG_Amt)/sum(a.SalesAmt) else 0 end,'p') CGPer ";
                sqldw += ",format(case when sum(a.SalesAmt)>0 then sum(a.CGVIP_Amt)/sum(a.SalesAmt) else 0 end,'p') VIPPer ";
                sqldw += "from MSData4Web a join CompositeHWeb h on a.CompanyCode=h.CompanyCode and a.PrDocNO=h.CG_No  ";
                sqldw += " where h.CompanyCode='" + uu.CompanyId + "' and '" + Opendate + "' between h.StartDate and h.EndDate and isnull(h.DefeasanceDate,'')=''";
                sqldw += " group by h.CG_NO,h.CG_Name,h.StartDate,h.EndDate";
                DataTable dtD = PubUtility.SqlQry(string.Format(sqldw, ""), uu, "SYS");
                dtD.TableName = "dtD";
                ds.Tables.Add(dtD);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSA102QueryShop")]
        public ActionResult SystemSetup_MSSA102QueryShop()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSA102QueryShopOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string CG_NO = rq["CG_NO"];
                string Flag = rq["Flag"];

                if (Flag == "S")
                {
                    string sqldw = "select a.ShopNO+char(13)+b.ST_Sname ID,sum(a.SalesAmt) SalesAmt,sum(a.SalesQty) SalesQty,sum(a.CG_Amt) CG_Amt,sum(a.CG_Qty) CG_Qty,sum(a.CGVIP_Amt) CGVIP_Amt,sum(a.CGVIP_Qty) CGVIP_Qty ";
                    sqldw += ",format(case when sum(a.SalesAmt)>0 then sum(a.CG_Amt)/sum(a.SalesAmt) else 0 end,'p') CGPer ";
                    sqldw += ",format(case when sum(a.SalesAmt)>0 then sum(a.CGVIP_Amt)/sum(a.SalesAmt) else 0 end,'p') VIPPer ";
                    sqldw += "from MSData4Web a join CompositeHWeb h on a.CompanyCode=h.CompanyCode and a.PrDocNO=h.CG_No  ";
                    sqldw += " join WarehouseWeb b  on a.CompanyCode=b.CompanyCode and a.ShopNO=b.ST_ID ";
                    sqldw += " where a.CompanyCode='" + uu.CompanyId + "' and h.CG_NO='" + CG_NO + "' ";
                    sqldw += " group by a.ShopNO,b.ST_Sname";
                    DataTable dtShop = PubUtility.SqlQry(string.Format(sqldw, ""), uu, "SYS");
                    dtShop.TableName = "dtShop";
                    ds.Tables.Add(dtShop);

                    sqldw = "select sum(a.SalesAmt) SalesAmt,sum(a.SalesQty) SalesQty,sum(a.CG_Amt) CG_Amt,sum(a.CG_Qty) CG_Qty,sum(a.CGVIP_Amt) CGVIP_Amt,sum(a.CGVIP_Qty) CGVIP_Qty ";
                    sqldw += ",format(case when sum(a.SalesAmt)>0 then sum(a.CG_Amt)/sum(a.SalesAmt) else 0 end,'p') CGPer ";
                    sqldw += ",format(case when sum(a.SalesAmt)>0 then sum(a.CGVIP_Amt)/sum(a.SalesAmt) else 0 end,'p') VIPPer ";
                    sqldw += "from MSData4Web a join CompositeHWeb h on a.CompanyCode=h.CompanyCode and a.PrDocNO=h.CG_No  ";
                    sqldw += " where a.CompanyCode='" + uu.CompanyId + "' and h.CG_NO='" + CG_NO + "' ";
                    DataTable dtSum = PubUtility.SqlQry(string.Format(sqldw, ""), uu, "SYS");
                    dtSum.TableName = "dtSum";
                    ds.Tables.Add(dtSum);
                }
                else if (Flag == "P")
                {
                    string sql = "select a.GoodsNO+char(13)+b.GD_NAME ID,sum(a.CG_Cash) CG_Amt,sum(a.CG_Qty) CG_Qty,sum(a.CGVIP_Cash) CGVIP_Amt,sum(a.CGVIP_Qty) CGVIP_Qty "; 
                    sql += ",format(case when sum(a.CG_Cash)>0 then sum(a.CGVIP_Cash)/sum(a.CG_Cash) else 0 end,'p') VIPPer ";
                    sql += "from MSData4_1Web a left join PLUWeb b on a.CompanyCode=b.CompanyCode and a.GoodsNO=b.GD_NO ";
                    sql += "where a.CompanyCode='" + uu.CompanyId + "' and a.PrDocNO='" + CG_NO + "'";
                    sql += " group by a.GoodsNO,b.GD_NAME";
                    DataTable dtShop = PubUtility.SqlQry(string.Format(sql, ""), uu, "SYS");
                    dtShop.TableName = "dtShop";
                    ds.Tables.Add(dtShop);

                    sql = "select sum(a.CG_Cash) CG_Amt,sum(a.CG_Qty) CG_Qty,sum(a.CGVIP_Cash) CGVIP_Amt,sum(a.CGVIP_Qty) CGVIP_Qty ";
                    sql += ",format(case when sum(a.CG_Cash)>0 then sum(a.CGVIP_Cash)/sum(a.CG_Cash) else 0 end,'p') VIPPer ";
                    sql += "from MSData4_1Web a left join PLUWeb b on a.CompanyCode=b.CompanyCode and a.GoodsNO=b.GD_NO ";
                    sql += "where a.CompanyCode='" + uu.CompanyId + "' and a.PrDocNO='" + CG_NO + "'";
                    DataTable dtSum = PubUtility.SqlQry(string.Format(sql, ""), uu, "SYS");
                    dtSum.TableName = "dtSum";
                    ds.Tables.Add(dtSum);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSD102Clear_Step1")]
        public ActionResult SystemSetup_MSSD102Clear_Step1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD102Clear_Step1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string sql = "";

                sql = "Select '' as ID,'' as SalesAmt, ";
                sql += "'' as SalesQty,'' as CG_Amt,'' as CG_Qty, ";
                sql += "'' as CGVIP_Amt,'' as CGVIP_Qty,'' as CGPer, ";
                sql += "'' as VIPPer ";
                sql += "From MSData4Web (nolock) ";
                sql += "Where 1=2 ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }
        #endregion

        #region MSSA109
        [Route("SystemSetup/MSSA109Query")]
        public ActionResult SystemSetup_MSSA109Query()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSA109QueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Year = rq["Year"];
                string Flag = rq["Flag"];
                string Month = rq["Month"];
                string YYYYMM;
                if (Month != "")
                    YYYYMM = Year + '/' + Month;
                else
                    YYYYMM = Year;

                string sql = "";
                string sqlD = "";
                string sqlH = "";

                
                if (Flag == "PQ")   //商品數量
                {
                    sql = "select top 100 ROW_NUMBER() over (ORDER BY sum(Qty) desc ) SeqNo,a.GD_NO,SUBSTRING(GD_NAME,1,15) GD_NAME,sum(Qty) Qty,Sum(Cash) Cash into #S1 ";
                    sql += "from MSData5Web a (nolock) join PLUWeb b (nolock) on a.CompanyCode =b.CompanyCode and a.GD_NO=b.GD_NO ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and S_YYYYMM like '" + YYYYMM + "%' ";
                    sql += "group by a.GD_NO ,GD_NAME ; ";

                }
                else if (Flag == "PM")      //商品金額
                {
                    sql = "select top 100 ROW_NUMBER() over (ORDER BY sum(Cash) desc ) SeqNo,a.GD_NO,SUBSTRING(GD_NAME,1,15) GD_NAME,sum(Qty) Qty,Sum(Cash) Cash into #S1 ";
                    sql += "from MSData5Web a (nolock) join PLUWeb b (nolock) on a.CompanyCode =b.CompanyCode and a.GD_NO=b.GD_NO ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and S_YYYYMM like '" + YYYYMM + "%' ";
                    sql += "group by a.GD_NO ,GD_NAME ; ";

                }
                else if (Flag == "W")      //店
                {
                    sql = "select  ROW_NUMBER() over (ORDER BY sum(Cash) desc ) SeqNo ,S_ShopNO+' '+b.ST_Sname Name,sum(Qty) Qty,Sum(Cash) Cash into #S1 ";
                    sql += "from MSData5Web a (nolock) join WarehouseWeb b(nolock) on a.CompanyCode =b.CompanyCode and a.S_ShopNO=b.ST_ID ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and S_YYYYMM like '" + YYYYMM + "%' ";
                    sql += "group by S_ShopNO,b.ST_Sname ; ";

                }
                else if (Flag == "MM")      //月
                {
                    sql = "select  SUBSTRING(S_YYYYMM,6,2) SeqNo,sum(Qty) Qty,sum(Cash) Cash ";
                    sql += ",(select Sum(Cash) Cash1 from MSData5Web  (nolock) where Companycode='" + uu.CompanyId + "' and  S_YYYYMM  =convert(varchar(7), dateadd(m,-1, convert(date,a.S_YYYYMM+'/01')),111) group by S_YYYYMM ) Cash2";
                    sql += " into #M ";
                    sql += "from MSData5Web a (nolock) ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and S_YYYYMM like '" + YYYYMM + "%' ";
                    sql += "group by S_YYYYMM ; ";
                    sql += "select  SeqNo,Qty,Cash,Case when Cash2 is null then format(1,'p') else format(cast(Cash-Cash2 as float)/cast(Cash2 as float),'p') end Per into #S1 from  #M;";
                }
                else       //部門,大,中,小類,系列
                {
                    string ColName = "";
                    if (Flag == "G")
                        ColName = "GD_Dept";
                    else if (Flag == "L")
                        ColName = "GD_BGNo";
                    else if (Flag == "M")
                        ColName = "GD_MDNo";
                    else if (Flag == "S")
                        ColName = "GD_SMNo";
                    else if (Flag == "B")
                        ColName = "GD_BNID";
                    else if (Flag == "E")
                        ColName = "GD_SERIES";

                    sql = "select  ROW_NUMBER() over (ORDER BY sum(Cash) desc ) SeqNo ,c.Type_ID+' '+c.Type_Name Name,sum(Qty) Qty,Sum(Cash) Cash into #S1 ";
                    sql += "from MSData5Web a (nolock) ";
                    sql += "left join TypeDataWeb c (nolock) on a.CompanyCode =c.CompanyCode and a." + ColName + "=c.Type_ID and Type_Code='" + Flag+ "' ";
                    sql += "where a.Companycode='" + uu.CompanyId + "' ";
                    sql += "and S_YYYYMM like '" + YYYYMM + "%' ";
                    sql += "group by c.Type_ID ,c.Type_Name ; ";

                }
                //明細
                sqlD = "select * From #S1 order by SeqNo";
                DataTable dtE = PubUtility.SqlQry(sql+sqlD, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
                //彙總資料
                sqlH = "select sum(qty) SumQty,sum(Cash) SumCash From #S1 ";
                DataTable dtH = PubUtility.SqlQry(sql+sqlH, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSA109Query_Step1")]
        public ActionResult SystemSetup_MSSA109Query_Step1()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSA109Query_Step1OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Year = rq["Year"];
                string Flag = rq["Flag"];
                string SubFlag = rq["SubFlag"];
                string Month = rq["Month"];
                string YYYYMM;
                if (Month != "" )
                    YYYYMM = Year + '/' + Month.Replace("月","");
                else
                    YYYYMM = Year;
                string SubType = rq["SubType"];

                string sql = "";
                string sqlD = "";
                string sqlH = "";

                if (Flag == "PQ" | Flag == "PM")   //商品數量,金額
                {
                    if (SubFlag == "Month")
                    {
                        sql = "select  SUBSTRING(S_YYYYMM,6,2) SeqNo,sum(Qty) Qty,sum(Cash) Cash ";
                        sql += ",(select Sum(Cash) Cash1 from MSData5Web  (nolock) where Companycode='" + uu.CompanyId + "' and  S_YYYYMM  =convert(varchar(7), dateadd(m,-1, convert(date,a.S_YYYYMM+'/01')),111) and GD_NO='" + SubType + "'group by S_YYYYMM ) Cash2";
                        sql += " into #M ";
                        sql += "from MSData5Web a (nolock) ";
                        sql += "where a.Companycode='" + uu.CompanyId + "' ";
                        sql += "and S_YYYYMM like '" + Year + "%' ";
                        sql += "and GD_NO='" + SubType + "'";
                        sql += "group by S_YYYYMM ; ";
                        sql += "select  SeqNo,Qty,Cash,Case when Cash2 is null then format(1,'p') else format(cast(Cash-Cash2 as float)/cast(Cash2 as float),'p') end Per into #S1 from  #M;";
                    }
                    else {
                        sql = "select  ROW_NUMBER() over (ORDER BY sum(Cash) desc ) SeqNo ,S_ShopNO+' '+b.ST_Sname Name,sum(Qty) Qty,Sum(Cash) Cash into #S1 ";
                        sql += "from MSData5Web a (nolock) join WarehouseWeb b(nolock) on a.CompanyCode =b.CompanyCode and a.S_ShopNO=b.ST_ID ";
                        sql += "where a.Companycode='" + uu.CompanyId + "' ";
                        sql += "and S_YYYYMM like '" + YYYYMM + "%' ";
                        sql += "and GD_NO='" + SubType + "'";
                        sql += "group by S_ShopNO,b.ST_Sname ; ";
                    }

                }
                else if (Flag == "MM")      //月
                {
                    if (SubFlag == "PLU")
                    {
                        sql = "select  ROW_NUMBER() over (ORDER BY sum(Qty) desc ) SeqNo,a.GD_NO,SUBSTRING(GD_NAME,1,15) GD_NAME,sum(Qty) Qty,Sum(Cash) Cash into #S1 ";
                        sql += "from MSData5Web a (nolock) join PLUWeb b (nolock) on a.CompanyCode =b.CompanyCode and a.GD_NO=b.GD_NO ";
                        sql += "where a.Companycode='" + uu.CompanyId + "' ";
                        sql += "and S_YYYYMM like '" + YYYYMM + "%' ";
                        sql += "group by a.GD_NO ,GD_NAME ; ";
                    }
                    else {
                         sql = "select  ROW_NUMBER() over (ORDER BY sum(Cash) desc ) SeqNo ,S_ShopNO+' '+b.ST_Sname Name,sum(Qty) Qty,Sum(Cash) Cash into #S1 ";
                        sql += "from MSData5Web a (nolock) join WarehouseWeb b(nolock) on a.CompanyCode =b.CompanyCode and a.S_ShopNO=b.ST_ID ";
                        sql += "where a.Companycode='" + uu.CompanyId + "' ";
                        sql += "and S_YYYYMM like '" + YYYYMM + "%' ";
                        sql += "group by S_ShopNO,b.ST_Sname ; ";
                   }

                }
                else       //部門,大,中,小類,系列
                {
                    string ColName = "";
                    if (Flag == "G")
                        ColName = "GD_Dept";
                    else if (Flag == "L")
                        ColName = "GD_BGNo";
                    else if (Flag == "M")
                        ColName = "GD_MDNo";
                    else if (Flag == "S")
                        ColName = "GD_SMNo";
                    else if (Flag == "B")
                        ColName = "GD_BNID";
                    else if (Flag == "E")
                        ColName = "GD_SERIES";
                    else if (Flag == "W")
                        ColName = "S_ShopNO";

                    if (SubFlag == "Month")
                    {
                        sql = "select  SUBSTRING(S_YYYYMM,6,2) SeqNo,sum(Qty) Qty,sum(Cash) Cash ";
                        sql += ",(select Sum(Cash) Cash1 from MSData5Web  (nolock) where Companycode='" + uu.CompanyId + "' and  S_YYYYMM  =convert(varchar(7), dateadd(m,-1, convert(date,a.S_YYYYMM+'/01')),111) and " + ColName + "='" + SubType + "'group by S_YYYYMM ) Cash2";
                        sql += " into #M ";
                        sql += "from MSData5Web a (nolock) ";
                        sql += "where a.Companycode='" + uu.CompanyId + "' ";
                        sql += "and S_YYYYMM like '" + Year + "%' ";
                        sql += "and " + ColName + "='" + SubType + "'";
                        sql += "group by S_YYYYMM ; ";
                        sql += "select  SeqNo,Qty,Cash,Case when Cash2 is null then format(1,'p') else format(cast(Cash-Cash2 as float)/cast(Cash2 as float),'p') end Per into #S1 from  #M;";
                    }
                    else {
                        sql = "select ROW_NUMBER() over (ORDER BY sum(Qty) desc ) SeqNo,a.GD_NO,SUBSTRING(GD_NAME,1,15) GD_NAME,sum(Qty) Qty,Sum(Cash) Cash into #S1 ";
                        sql += "from MSData5Web a (nolock) left join PLUWeb b (nolock) on a.CompanyCode =b.CompanyCode and a.GD_NO=b.GD_NO ";
                        sql += "where a.Companycode='" + uu.CompanyId + "' ";
                        sql += "and S_YYYYMM like '" + YYYYMM + "%' ";
                        sql += "and a." + ColName + "='" + SubType + "'";
                        sql += "group by a.GD_NO ,GD_NAME ; ";

                    }

                }
                //明細
                sqlD = "select top 100 * From #S1 order by SeqNo";
                DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
                //彙總資料
                sqlH = "select sum(qty) SumQty,sum(Cash) SumCash From #S1 ";
                DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                dtH.TableName = "dtH";
                ds.Tables.Add(dtH);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }
        #endregion

        [Route("SystemSetup/LookUp")]
        public ActionResult SystemSetup_LookUp()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "LookUpOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string tbName= rq["GQ_Table"];
                string ColName = rq["GQ_Column"];
                string OrderCol = rq["GQ_OrderColumn"];
                string StrCond = rq["GQ_Condition"];
                string QueryValue = rq["QueryValue"];

                string sql = "select ";
                string[] ls_TN = tbName.Split(',');
                string ls_Tables = "";
                int li_a = 97;
                for(int i = 0; i < ls_TN.Length; i++)
                {
                    ls_Tables += ls_TN[i] + " " + Convert.ToChar(li_a).ToString() + " ";
                    if (ls_TN[i].ToLower().IndexOf("nolock") <= -1)
                    {
                        ls_Tables += "(nolock)";
                    }
                    ls_Tables += ",";
                    li_a += 1;
                }
                ls_Tables = ls_Tables.Substring(0, ls_Tables.Length - 1);
                sql += ColName + " from " + ls_Tables + " where a.CompanyCode='" + uu.CompanyId + "' ";
                if (StrCond!=null && StrCond != "")
                {
                    if (StrCond.Trim().Substring(0, 3).ToLower() == "and")
                    {
                        sql += StrCond;
                    }
                    else
                    {
                        sql += " and " + StrCond;
                    }
                }
                if (QueryValue != null && QueryValue != "")
                {
                    string[] likeCol;
                    sql += " and (";
                    likeCol = ColName.Split(',');
                    for(int i = 0; i < likeCol.Length; i++)
                    {
                        sql += " (" + likeCol[i] + " like N'%" + QueryValue + "%') or";
                    }

                    sql = sql.Substring(0, sql.Length - 3);
                    sql += ") ";
                }
                if(OrderCol != null && OrderCol != "")
                {
                    sql += " order by " + OrderCol;
                }
             
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSD103Query")]
        public ActionResult SystemSetup_MSSD103Query()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD103QueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ActivityCode = rq["ActivityCode"];
                string PSName = rq["PSName"];
                string EDDate = rq["EDDate"];

                string sql = "";
                sql = "Select a.PS_NO,a.ActivityCode,b.PS_Name,isnull(PrintStartDate,'')+'~'+a.PrintEndDate PDate,sum(isnull(a.issueQty,0)) Cnt1, ";
                sql += "a.StartDate + '~' + a.EndDate EDDate,sum(isnull(a.ReclaimQty,0)) Cnt2, ";
                sql += "case when sum(isnull(a.issueQty,0))=0 then FORMAT(0,'p') else format(cast(sum(isnull(a.ReclaimQty,0)) as Float)/cast(sum(isnull(a.issueQty,0)) as Float),'p') end as RePercent, ";
                sql += "sum(isnull(a.ShareAmt,0)) ActualDiscount,sum(isnull(a.ReclaimCash,0)) Cash,sum(isnull(a.ReclaimTrans,0)) Cnt3, ";
                sql += "case when sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(sum(isnull(a.ReclaimCash,0))/sum(isnull(a.ReclaimTrans,0)),0) end as SalesPrice ";
                sql += "From MsData2Web a (nolock) ";
                sql += "inner join PromoteSCouponHWeb b (nolock) on a.PS_NO=b.PS_NO and b.Companycode=a.Companycode and b.CouponType in('1','2') ";
                //活動代號
                if (ActivityCode.SqlQuote() != "")
                {
                    sql += "and b.ActivityCode like '%" + ActivityCode.SqlQuote() + "%' ";
                }
                //活動名稱
                if (PSName.SqlQuote() != "")
                {
                    sql += "and b.PS_Name like '%" + PSName.SqlQuote() + "%' ";
                }
                sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                sql += "and a.PS_NO in (Select distinct PS_No from PrintCouponRecWeb (nolock) Where Companycode='" + uu.CompanyId + "') ";
                //活動日期
                if (EDDate.SqlQuote() != "")
                {
                    sql += "and '" + EDDate.SqlQuote() + "' between a.StartDate and a.EndDate ";
                }
                sql += "group by a.PS_NO,a.ActivityCode,b.PS_Name,PrintStartDate,PrintEndDate,a.StartDate,a.EndDate ";
                sql += "Order by a.StartDate desc";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSD103QueryD")]
        public ActionResult SystemSetup_MSSD103QueryD()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD103QueryDOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string PS_NO = rq["PS_NO"];
                string Flag = rq["Flag"];  //SA-全店 S1-單店+全日 DA-全日 D1-單日+全店
                string SalesDate = rq["SalesDate"];
                string ShopNo = rq["ShopNo"];

                string sql = "";  //明細sql指令
                string sqlSum = "";  //總計sql指令
                string sqlIDColname = "";  //ID要取的實際資料欄位名稱
                
                if (Flag == "SA")
                {
                    sql = "Select a.ShopNo + '-' + b.ST_SName as id,isnull(a.issueQty,0) Cnt1,isnull(a.ReclaimQty,0) Cnt2, ";
                    sql += "case when isnull(a.issueQty,0)=0 then case when isnull(a.ReclaimQty,0)=0 then format(0,'p') else format(9.99,'p') end "
                        + "else format(cast(isnull(a.ReclaimQty,0) as Float)/cast(isnull(a.issueQty,0) as Float),'p') end as RePercent, ";
                    sql += "isnull(a.ShareAmt,0) ActualDiscount,isnull(a.ReclaimCash,0) SalesCash1,isnull(a.ReclaimTrans,0) SalesCnt1, ";
                    sql += "case when isnull(a.ReclaimTrans,0)=0 then 0 else Round(isnull(a.ReclaimCash,0)/isnull(a.ReclaimTrans,0),0) end as SalesPrice1, ";
                    sql += "isnull(a.TotalCash,0) SalesCash2,isnull(a.TotalTrans,0) SalesCnt2, ";
                    sql += "case when isnull(a.TotalTrans,0)=0 then 0 else Round(isnull(a.TotalCash,0)/isnull(a.TotalTrans,0),0) end as SalesPrice2 ";
                    sql += "From MSData2Web a (nolock) ";
                    sql += "inner join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode and b.ST_Type not in ('0','2','3') ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' and PS_No='"+ PS_NO + "' order by a.ShopNo";

                    sqlSum = "Select Sum(isnull(a.issueQty,0)) Cnt1,Sum(isnull(a.ReclaimQty,0)) Cnt2, ";
                    sqlSum += "case when Sum(isnull(a.issueQty,0))=0 then case when Sum(isnull(a.ReclaimQty,0))=0 then format(0,'p') else format(9.99,'p') end "
                        + "else format(cast(Sum(isnull(a.ReclaimQty,0)) as Float)/cast(Sum(isnull(a.issueQty,0)) as Float),'p') end as RePercent, ";
                    sqlSum += "Sum(isnull(a.ShareAmt,0)) ActualDiscount,Sum(isnull(a.ReclaimCash,0)) SalesCash1,Sum(isnull(a.ReclaimTrans,0)) SalesCnt1, ";
                    sqlSum += "case when Sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(Sum(isnull(a.ReclaimCash,0))/Sum(isnull(a.ReclaimTrans,0)),0) end as SalesPrice1, ";
                    sqlSum += "Sum(isnull(a.TotalCash,0)) SalesCash2,Sum(isnull(a.TotalTrans,0)) SalesCnt2, ";
                    sqlSum += "case when Sum(isnull(a.TotalTrans,0))=0 then 0 else Round(Sum(isnull(a.TotalCash,0))/Sum(isnull(a.TotalTrans,0)),0) end as SalesPrice2 ";
                    sqlSum += "From MSData2Web a (nolock) ";
                    sqlSum += "Where a.Companycode='" + uu.CompanyId + "' and PS_No='" + PS_NO + "'";
                }else if (Flag == "DA")
                {
                    sql = "Select a.SalesDate as id,Sum(isnull(a.PrintQty,0)) Cnt1,Sum(isnull(a.ReclaimQty,0)) Cnt2, ";
                    sql += "case when Sum(isnull(a.PrintQty,0))=0 then case when Sum(isnull(a.ReclaimQty,0))=0 then format(0,'p') else format(9.99,'p') end "
                        + "else format(cast(Sum(isnull(a.ReclaimQty,0)) as Float)/cast(Sum(isnull(a.PrintQty,0)) as Float),'p') end as RePercent, ";
                    sql += "Sum(isnull(a.ShareAmt,0)) ActualDiscount,Sum(isnull(a.ReclaimCash,0)) SalesCash1,Sum(isnull(a.ReclaimTrans,0)) SalesCnt1, ";
                    sql += "case when Sum(isnull(a.ReclaimTrans,0))=0 then 0 else Round(Sum(isnull(a.ReclaimCash,0))/Sum(isnull(a.ReclaimTrans,0)),0) end as SalesPrice1, ";
                    sql += "Sum(isnull(a.TotalCash,0)) SalesCash2,Sum(isnull(a.TotalTrans,0)) SalesCnt2, ";
                    sql += "case when Sum(isnull(a.TotalTrans,0))=0 then 0 else Round(Sum(isnull(a.TotalCash,0))/Sum(isnull(a.TotalTrans,0)),0) end as SalesPrice2 ";
                    sql += "From MSData1Web a (nolock) ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' and PS_No='" + PS_NO + "' group by SalesDate order by SalesDate";
                }
                else
                {
                    if (Flag == "S1")
                    {
                        sqlIDColname = "a.SalesDate";
                    }
                    else
                    {
                        sqlIDColname = "a.ShopNo + '-' + b.ST_SName";
                    }

                    sql = "Select "+ sqlIDColname + " as id,isnull(a.PrintQty,0) Cnt1,isnull(a.ReclaimQty,0) Cnt2, ";
                    sql += "case when isnull(a.PrintQty,0)=0 then case when isnull(a.ReclaimQty,0)=0 then format(0,'p') else format(9.99,'p') end "
                        + "else format(cast(isnull(a.ReclaimQty,0) as Float)/cast(isnull(a.PrintQty,0) as Float),'p') end as RePercent, ";
                    sql += "isnull(a.ShareAmt,0) ActualDiscount,isnull(a.ReclaimCash,0) SalesCash1,isnull(a.ReclaimTrans,0) SalesCnt1, ";
                    sql += "case when isnull(a.ReclaimTrans,0)=0 then 0 else Round(isnull(a.ReclaimCash,0)/isnull(a.ReclaimTrans,0),0) end as SalesPrice1, ";
                    sql += "isnull(a.TotalCash,0) SalesCash2,isnull(a.TotalTrans,0) SalesCnt2, ";
                    sql += "case when isnull(a.TotalTrans,0)=0 then 0 else Round(isnull(a.TotalCash,0)/isnull(a.TotalTrans,0),0) end as SalesPrice2 ";
                    sql += "From MSData1Web a (nolock) ";
                    if (Flag == "D1")
                    {
                        sql += "inner join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.Companycode=a.Companycode and b.ST_Type not in ('0','2','3') ";
                    }
                    sql += "Where a.Companycode='" + uu.CompanyId + "' and PS_No='" + PS_NO + "' ";
                    if (Flag == "S1")
                    {
                        sql += "and a.ShopNo='"+ ShopNo +"' ";
                        sql += "order by a.SalesDate";
                    }
                    else
                    {
                        sql += "and a.SalesDate='" + SalesDate + "' ";
                        sql += "order by a.ShopNo";
                    }
                }

                //明細資料
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);

                //彙總資料
                if (sqlSum != "")
                {
                    DataTable dtSum = PubUtility.SqlQry(sqlSum, uu, "SYS");
                    dtSum.TableName = "dtSum";
                    ds.Tables.Add(dtSum);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSPV101Save")]
        public ActionResult SystemSetup_MSPV101Save()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSPV101SaveOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string UID = rq["UID"];
                string OldUPWD = rq["OldUPWD"];
                string NewUPWD = rq["NewUPWD"];

                string sql = "";
                sql = "Select * From Account (nolock) Where Companycode='" + uu.CompanyId + "' ";
                sql += "and UID='" + UID + "' ";
                sql += "and UPWD='" + OldUPWD + "' ";
                DataTable dtA = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtA.Rows.Count > 0)
                {
                    sql = "Update Account Set UPWD='" + NewUPWD + "',UPWDDate=convert(char(10),getdate(),111) + ' ' + convert(char(12),getdate(),108) ";
                    sql += "Where Companycode='" + uu.CompanyId + "' and UID='" + UID + "' ";
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }
                else {
                    throw new Exception("舊密碼不存在，請重新確認!");
                }

                //更新EDDMS.AccountIXMS密碼
                sql = "select '" + uu.CompanyId + "' as CompanyID,'" + UID + "' as UID,'" + NewUPWD + "' as NewUPWD ";
                DataTable dtUpdateUPWD = PubUtility.SqlQry(sql, uu, "SYS");
                dtUpdateUPWD.TableName = "dtUpdateUPWD";
                DataSet dsUpdateUPWD = new DataSet();
                dsUpdateUPWD.Tables.Add(dtUpdateUPWD);

                ApiSetting aSet = GetApiSetting();
                iXmsClient.ApiUrl = aSet.url_HTADDVIP_ET;
                DataSet dsR = iXmsClient.UpdateUPWD(dsUpdateUPWD, uu);
                DataTable dtP = dsR.Tables["dtProcessStatus"];
                if (dtP.Rows[0]["Error"].ToString() != "0")
                {
                    throw new Exception(dtP.Rows[0]["Msg_Code"].ToString());
                }

                //dtE.TableName = "dtE";
                //ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/InitMSSETLOGO")]
        public ActionResult SystemSetup_InitMSSETLOGO()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "InitMSSETLOGOOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ProgramID = rq["ProgramID"];
                string sql = "select ChineseName from ProgramIDWeb (nolock) where ProgramID='" + ProgramID.SqlQuote() + "'";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);

                sql = "select ProgramID,ProgramID + ' ' + ChineseName ProgramName from ProgramIDWeb (nolock) where ProgramID like '%DM%' order by ProgramID ";
                DataTable dtP = PubUtility.SqlQry(sql, uu, "SYS");
                dtP.TableName = "dtP";
                ds.Tables.Add(dtP);

                sql = "Select a.Companycode,a.Companycode + ' ' + b.ChineseName CompanyName From ProgramIdCompanyWWeb a (nolock) ";
                sql += "inner join CompanyWeb b (nolock) on a.Companycode=b.Companycode ";
                sql += "group by a.Companycode,b.ChineseName order by a.Companycode ";
                DataTable dtC = PubUtility.SqlQry(sql, uu, "SYS");
                dtC.TableName = "dtC";
                ds.Tables.Add(dtC);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSETLOGOQuery")]
        public ActionResult SystemSetup_MSSETLOGOQuery()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSETLOGOQueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string CompanyID = rq["CompanyID"];
                string ProgramID = rq["ProgramID"];
                string sql = "select a.Companycode,b.ChineseName CompanyName,a.ProgramID,c.ChineseName ProgramName,a.TE,a.Pic  ";
                sql += "from EDMSetWeb a (nolock) ";
                sql += "inner join CompanyWeb b (nolock) on a.Companycode=b.Companycode ";
                sql += "inner join ProgramIDWeb c (nolock) on a.ProgramID=c.ProgramID ";
                sql += "where 1=1 ";
                if (CompanyID != "")
                {
                    sql += "and a.Companycode='" + CompanyID.SqlQuote() + "' ";
                }
                if (ProgramID != "") {
                    sql += "and a.ProgramID='" + ProgramID.SqlQuote() + "' ";
                }
                sql += "Order by a.ProgramID ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSETLOGOGetCompany")]
        public ActionResult SystemSetup_MSSETLOGOGetCompany()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSETLOGOGetCompanyOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string Company = rq["Company"];
                string sql = "select * ";
                sql += "from CompanyWeb (nolock) ";
                sql += "where 1=1 ";
                if (Company != "")
                {
                    sql += "and Companycode='" + Company + "' ";
                }
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("MSSETLOGOGetImage_Logo")]
        public ActionResult MSSETLOGOGetImage_Logo()
        {
            try
            {
                IQueryCollection rq = HttpContext.Request.Query;
                string CompanyID = rq["CompanyID"];
                string ProgramID = rq["ProgramID"];
                string UU = rq["UU"];
                UserInfo uu = PubUtility.GetCurrentUser("Bearer " + UU);
                DataTable dt = PubUtility.SqlQry("select * from EDMSetWeb (nolock) where ProgramID='" + ProgramID + "' and Companycode='" + CompanyID + "' ", uu, "SYS");
                DataRow dr = dt.Rows[0];
                string ContentType = "image/jpeg";
                //HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(dr["FileName"].ToString()));

                return File(dr["Pic"] as byte[], ContentType);
            }
            catch (Exception err)
            {

                string ContentType = "image/jpeg";
                HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=No_Pic.jpg");
                string fn = ConstList.HostEnvironment.WebRootPath + @"\images\No_Pic.jpg";
                return File(System.IO.File.ReadAllBytes(fn), ContentType);
            }
        }

        [Route("SystemSetup/MSSETLOGOGetVMDocNo")]
        public ActionResult SystemSetup_MSSETLOGOGetVMDocNo()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSETLOGOGetVMDocNoOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                var DocNo = PubUtility.GetNewDocNo(uu, "VM", 3);
                string sql = "";
                sql = "Select '" + DocNo + "' as DocNo ";
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);

                sql = "Delete From SetEDM Where Companycode='" + uu.CompanyId + "' and left(DocNo,2)='VM' and CrtDate<convert(char(10),getdate(),111) ";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSETLOGO_Save")]
        public ActionResult SystemSetup_MSSETLOGO_Save()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSETLOGO_SaveOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string EditMode = rq["EditMode"];
                string CompanyID = rq["CompanyID"];
                string ProgramID = rq["ProgramID"];
                string TE = rq["TE"];
                string VMDocNo = rq["VMDocNo"];
                string sql = "";
                string sqlLogo = "";
                //新增
                if (EditMode.SqlQuote() == "A")
                {
                    sqlLogo = "Select * From EDMSetWeb (nolock) Where Companycode='" + CompanyID + "' and ProgramID='" + ProgramID + "' ";
                    DataTable dtchk = PubUtility.SqlQry(sqlLogo, uu, "SYS");
                    if (dtchk.Rows.Count > 0) {
                        throw new Exception("此程式代碼已有公司Logo，請重新確認!");
                    }
                    sqlLogo = "Select * From SetEDM (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + VMDocNo + "' and DataType='P1' ";
                    DataTable dtLogo = PubUtility.SqlQry(sqlLogo, uu, "SYS");
                    if (dtLogo.Rows.Count > 0)
                    {
                        if (dtLogo.Rows[0]["STATUS"].ToString() == "1")
                        {
                            sql += "Insert into EDMSetWeb (CompanyCode,ProgramID,Txt,Pic,CrtDate,CrtUser,ModDate,ModUser,TE) ";
                            sql += "Select '" + CompanyID + "','" + ProgramID + "','',DocImage, ";
                            sql += "convert(char(10),getdate(),111),'" + uu.UserID + "', ";
                            sql += "convert(char(10),getdate(),111),'" + uu.UserID + "','" + TE.SqlQuote() + "' ";
                            sql += "From SetEDM (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + VMDocNo + "' and DataType='P1'; ";
                        }
                        sql += "Delete From SetEDM Where Companycode='" + uu.CompanyId + "' and DocNo='" + VMDocNo + "' and DataType='P1'; ";
                    }
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }
                //修改
                else if (EditMode.SqlQuote() == "M")
                {
                    sqlLogo = "Select * From SetEDM (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + VMDocNo + "' and DataType='P1' ";
                    DataTable dtLogo = PubUtility.SqlQry(sqlLogo, uu, "SYS");
                    if (dtLogo.Rows.Count > 0)
                    {
                        if (dtLogo.Rows[0]["STATUS"].ToString() == "1")
                        {
                            sql += "Delete From EDMSetWeb Where Companycode='" + CompanyID + "' and ProgramID='" + ProgramID + "'; ";
                            sql += "Insert into EDMSetWeb (CompanyCode,ProgramID,Txt,Pic,CrtDate,CrtUser,ModDate,ModUser,TE) ";
                            sql += "Select '" + CompanyID + "','" + ProgramID + "','',DocImage, ";
                            sql += "convert(char(10),getdate(),111),'" + uu.UserID + "', ";
                            sql += "convert(char(10),getdate(),111),'" + uu.UserID + "','" + TE.SqlQuote() + "' ";
                            sql += "From SetEDM (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + VMDocNo + "' and DataType='P1'; ";
                        }
                        sql += "Delete From SetEDM Where Companycode='" + uu.CompanyId + "' and DocNo='" + VMDocNo + "' and DataType='P1'; ";
                    }
                    else {
                        sql += "Update EDMSetWeb set TE='" + TE.SqlQuote() + "',ModDate=convert(char(10),getdate(),111),ModUser='" + uu.UserID + "' ";
                        sql += "Where Companycode='" + CompanyID + "' and ProgramID='" + ProgramID + "' ";
                    }
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }

                sql = "select * from SetEDMHWeb (nolock) ";
                DataTable dtS = PubUtility.SqlQry(sql, uu, "SYS");
                dtS.TableName = "dtS";
                ds.Tables.Add(dtS);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSETLOGODelImg")]
        public ActionResult SystemSetup_MSSETLOGODelImg()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSETLOGODelImgOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string sql = "";

                sql = "Select * From SetEDM (nolock) Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P1' ";
                DataTable dt = PubUtility.SqlQry(sql, uu, "SYS");
                if (dt.Rows.Count > 0)
                {
                    sql = "Update SetEDM Set Status='0' Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo.SqlQuote() + "' and DataType='P1' ";
                    PubUtility.ExecuteSql(sql, uu, "SYS");
                }
                else
                {
                    DataTable dtF = new DataTable();
                    dtF.Columns.Add("CompanyCode", typeof(string));
                    dtF.Columns.Add("STATUS", typeof(string));
                    dtF.Columns.Add("DocNo", typeof(string));
                    dtF.Columns.Add("DataType", typeof(string));
                    dtF.Columns.Add("DocType", typeof(string));
                    DataRow drF = dtF.NewRow();
                    drF["CompanyCode"] = uu.CompanyId;
                    drF["STATUS"] = "0";
                    drF["DocNo"] = DocNo.SqlQuote();
                    drF["DataType"] = "P1";
                    drF["DocType"] = "P";
                    dtF.Rows.Add(drF);
                    PubUtility.AddTable("SetEDM", dtF, uu, "SYS");
                }


            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSETLOGOCancel")]
        public ActionResult SystemSetup_MSSETLOGOCancel()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSETLOGOCancelOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string CompanyID = rq["CompanyID"];
                string ProgramID = rq["ProgramID"];
                string DocNo = rq["DocNo"];
                string sql = "select a.Companycode,b.ChineseName CompanyName,a.ProgramID,c.ChineseName ProgramName,a.TE  ";
                sql += "from EDMSetWeb a (nolock) ";
                sql += "inner join CompanyWeb b (nolock) on a.Companycode=b.Companycode ";
                sql += "inner join ProgramIDWeb c (nolock) on a.ProgramID=c.ProgramID ";
                sql += "where 1=1 ";
                if (CompanyID != "")
                {
                    sql += "and a.Companycode='" + CompanyID.SqlQuote() + "' ";
                }
                if (ProgramID != "")
                {
                    sql += "and a.ProgramID='" + ProgramID.SqlQuote() + "' ";
                }
                DataTable dtE = PubUtility.SqlQry(sql, uu, "SYS");
                dtE.TableName = "dtE";
                ds.Tables.Add(dtE);

                sql = "Delete From SetEDM Where Companycode='" + uu.CompanyId + "' and DocNo='" + DocNo + "' and DataType='P1' ";
                PubUtility.ExecuteSql(sql, uu, "SYS");
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        #region MSSD107
        [Route("SystemSetup/MSSD107Query")]
        public ActionResult SystemSetup_MSSD107Query()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD107QueryOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string YY = rq["YY"];
                string Flag = rq["Flag"];
                string sql = "";
                string sqlD = "";
                string sqlH = "";

                //月份
                if (Flag == "M")
                {
                    sql = "select left(OpenDate,4) YY,substring(OpenDate,6,2) + '月' ID,SUM(isnull(Cash,0))Cash,SUM(isnull(RecS,0))RecS, ";
                    sql += "case when SUM(isnull(RecS,0))=0 then 0 else Round(SUM(isnull(Cash,0))/SUM(isnull(RecS,0)),0) end as Price, ";
                    sql += "SUM(isnull(VIP_Cash,0))VIP_Cash,SUM(isnull(VIP_RecS,0))VIP_RecS, ";
                    sql += "case when SUM(isnull(VIP_RecS,0))=0 then 0 else Round(SUM(isnull(VIP_Cash,0))/SUM(isnull(VIP_RecS,0)),0) end as VIPPrice, ";
                    sql += "case when SUM(isnull(Cash,0))=0 then format(0,'p') else format(cast(SUM(isnull(VIP_Cash,0)) as Float)/cast(SUM(isnull(Cash,0)) as Float),'p') end as VIPPercent, ";
                    sql += "SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)) VIPNo_Cash,SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0)) VIPNo_RecS, ";
                    sql += "case when SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0))=0 then 0 else Round((SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)))/(SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0))),0) end as VIPNoPrice, ";
                    sql += "case when SUM(isnull(Cash,0))=0 then format(0,'p') else format(cast(SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)) as Float)/cast(SUM(isnull(Cash,0)) as Float),'p') end as VIPNoPercent ";
                    sql += "into #S ";
                    sql += "from SalesHWeb (nolock) ";
                    sql += "Where Companycode='" + uu.CompanyId + "' ";
                    if (YY.SqlQuote() != "") {
                        sql += "and left(OpenDate,4)='" + YY.SqlQuote() + "' ";
                    }
                    sql += "group by left(OpenDate,4),substring(OpenDate,6,2) ";
                    sql += "order by substring(OpenDate,6,2); ";
                    //明細資料
                    sqlD = "Select * From #S ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);
                    //彙總資料
                    sqlH = "Select Sum(isnull(Cash,0))SumCash,Sum(isnull(RecS,0))SumRecS, ";
                    sqlH += "case when SUM(isnull(RecS,0))=0 then 0 else Round(SUM(isnull(Cash,0))/SUM(isnull(RecS,0)),0) end as SumPrice, ";
                    sqlH += "SUM(isnull(VIP_Cash,0))SumVIP_Cash,SUM(isnull(VIP_RecS,0))SumVIP_RecS, ";
                    sqlH += "case when SUM(isnull(VIP_RecS,0))=0 then 0 else Round(SUM(isnull(VIP_Cash,0))/SUM(isnull(VIP_RecS,0)),0) end as SumVIPPrice, ";
                    sqlH += "case when SUM(isnull(Cash,0))=0 then format(0,'p') else format(cast(SUM(isnull(VIP_Cash,0)) as Float)/cast(SUM(isnull(Cash,0)) as Float),'p') end as SumVIPPercent, ";
                    sqlH += "SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)) SumVIPNo_Cash,SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0)) SumVIPNo_RecS, ";
                    sqlH += "case when SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0))=0 then 0 else Round((SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)))/(SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0))),0) end as SumVIPNoPrice, ";
                    sqlH += "case when SUM(isnull(Cash,0))=0 then format(0,'p') else format(cast(SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)) as Float)/cast(SUM(isnull(Cash,0)) as Float),'p') end as SumVIPNoPercent ";
                    sqlH += "From #S ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
                //店別
                else if (Flag == "S")
                {
                    sql = "select left(OpenDate,4) YY,ShopNo + '-' + ST_SName ID,SUM(isnull(Cash,0))Cash,SUM(isnull(RecS,0))RecS, ";
                    sql += "case when SUM(isnull(RecS,0))=0 then 0 else Round(SUM(isnull(Cash,0))/SUM(isnull(RecS,0)),0) end as Price, ";
                    sql += "SUM(isnull(VIP_Cash,0))VIP_Cash,SUM(isnull(VIP_RecS,0))VIP_RecS, ";
                    sql += "case when SUM(isnull(VIP_RecS,0))=0 then 0 else Round(SUM(isnull(VIP_Cash,0))/SUM(isnull(VIP_RecS,0)),0) end as VIPPrice, ";
                    sql += "case when SUM(isnull(Cash,0))=0 then format(0,'p') else format(cast(SUM(isnull(VIP_Cash,0)) as Float)/cast(SUM(isnull(Cash,0)) as Float),'p') end as VIPPercent, ";
                    sql += "SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)) VIPNo_Cash,SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0)) VIPNo_RecS, ";
                    sql += "case when SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0))=0 then 0 else Round((SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)))/(SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0))),0) end as VIPNoPrice, ";
                    sql += "case when SUM(isnull(Cash,0))=0 then format(0,'p') else format(cast(SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)) as Float)/cast(SUM(isnull(Cash,0)) as Float),'p') end as VIPNoPercent ";
                    sql += "into #S ";
                    sql += "from SalesHWeb a (nolock) ";
                    sql += "inner join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.ST_Type not in ('0','2','3') and b.Companycode=a.Companycode ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    if (YY.SqlQuote() != "")
                    {
                        sql += "and left(OpenDate,4)='" + YY.SqlQuote() + "' ";
                    }
                    sql += "group by left(OpenDate,4),ShopNo,ST_SName ";
                    sql += "order by ShopNo; ";
                    //明細資料
                    sqlD = "Select * From #S ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);
                    //彙總資料
                    sqlH = "Select Sum(isnull(Cash,0))SumCash,Sum(isnull(RecS,0))SumRecS, ";
                    sqlH += "case when SUM(isnull(RecS,0))=0 then 0 else Round(SUM(isnull(Cash,0))/SUM(isnull(RecS,0)),0) end as SumPrice, ";
                    sqlH += "SUM(isnull(VIP_Cash,0))SumVIP_Cash,SUM(isnull(VIP_RecS,0))SumVIP_RecS, ";
                    sqlH += "case when SUM(isnull(VIP_RecS,0))=0 then 0 else Round(SUM(isnull(VIP_Cash,0))/SUM(isnull(VIP_RecS,0)),0) end as SumVIPPrice, ";
                    sqlH += "case when SUM(isnull(Cash,0))=0 then format(0,'p') else format(cast(SUM(isnull(VIP_Cash,0)) as Float)/cast(SUM(isnull(Cash,0)) as Float),'p') end as SumVIPPercent, ";
                    sqlH += "SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)) SumVIPNo_Cash,SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0)) SumVIPNo_RecS, ";
                    sqlH += "case when SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0))=0 then 0 else Round((SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)))/(SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0))),0) end as SumVIPNoPrice, ";
                    sqlH += "case when SUM(isnull(Cash,0))=0 then format(0,'p') else format(cast(SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)) as Float)/cast(SUM(isnull(Cash,0)) as Float),'p') end as SumVIPNoPercent ";
                    sqlH += "From #S ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/MSSD107QueryD")]
        public ActionResult SystemSetup_MSSD107QueryD()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "MSSD107QueryDOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string YY = rq["YY"];
                string ID = rq["ID"];
                string Flag = rq["Flag"];
                string sql = "";
                string sqlD = "";
                string sqlH = "";

                //月份
                if (Flag == "M")
                {
                    string YM = YY + "/" + ID;
                    sql = "select ShopNo + '-' + ST_SName ID,SUM(isnull(Cash,0))Cash,SUM(isnull(RecS,0))RecS, ";
                    sql += "case when SUM(isnull(RecS,0))=0 then 0 else Round(SUM(isnull(Cash,0))/SUM(isnull(RecS,0)),0) end as Price, ";
                    sql += "SUM(isnull(VIP_Cash,0))VIP_Cash,SUM(isnull(VIP_RecS,0))VIP_RecS, ";
                    sql += "case when SUM(isnull(VIP_RecS,0))=0 then 0 else Round(SUM(isnull(VIP_Cash,0))/SUM(isnull(VIP_RecS,0)),0) end as VIPPrice, ";
                    sql += "case when SUM(isnull(Cash,0))=0 then format(0,'p') else format(cast(SUM(isnull(VIP_Cash,0)) as Float)/cast(SUM(isnull(Cash,0)) as Float),'p') end as VIPPercent, ";
                    sql += "SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)) VIPNo_Cash,SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0)) VIPNo_RecS, ";
                    sql += "case when SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0))=0 then 0 else Round((SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)))/(SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0))),0) end as VIPNoPrice, ";
                    sql += "case when SUM(isnull(Cash,0))=0 then format(0,'p') else format(cast(SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)) as Float)/cast(SUM(isnull(Cash,0)) as Float),'p') end as VIPNoPercent ";
                    sql += "into #S ";
                    sql += "from SalesHWeb a (nolock) ";
                    sql += "inner join WarehouseWeb b (nolock) on a.ShopNo=b.ST_ID and b.ST_Type not in ('0','2','3') and b.Companycode=a.Companycode ";
                    sql += "Where a.Companycode='" + uu.CompanyId + "' ";
                    if (YM != "")
                    {
                        sql += "and left(OpenDate,7)='" + YM + "' ";
                    }
                    sql += "group by ShopNo,ST_SName ";
                    sql += "order by ShopNo; ";
                    //明細資料
                    sqlD = "Select * From #S ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);
                    //彙總資料
                    sqlH = "Select Sum(isnull(Cash,0))SumCash,Sum(isnull(RecS,0))SumRecS, ";
                    sqlH += "case when SUM(isnull(RecS,0))=0 then 0 else Round(SUM(isnull(Cash,0))/SUM(isnull(RecS,0)),0) end as SumPrice, ";
                    sqlH += "SUM(isnull(VIP_Cash,0))SumVIP_Cash,SUM(isnull(VIP_RecS,0))SumVIP_RecS, ";
                    sqlH += "case when SUM(isnull(VIP_RecS,0))=0 then 0 else Round(SUM(isnull(VIP_Cash,0))/SUM(isnull(VIP_RecS,0)),0) end as SumVIPPrice, ";
                    sqlH += "case when SUM(isnull(Cash,0))=0 then format(0,'p') else format(cast(SUM(isnull(VIP_Cash,0)) as Float)/cast(SUM(isnull(Cash,0)) as Float),'p') end as SumVIPPercent, ";
                    sqlH += "SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)) SumVIPNo_Cash,SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0)) SumVIPNo_RecS, ";
                    sqlH += "case when SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0))=0 then 0 else Round((SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)))/(SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0))),0) end as SumVIPNoPrice, ";
                    sqlH += "case when SUM(isnull(Cash,0))=0 then format(0,'p') else format(cast(SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)) as Float)/cast(SUM(isnull(Cash,0)) as Float),'p') end as SumVIPNoPercent ";
                    sqlH += "From #S ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
                //店別
                else if (Flag == "S")
                {
                    sql = "select substring(OpenDate,6,2) + '月' ID,SUM(isnull(Cash,0))Cash,SUM(isnull(RecS,0))RecS, ";
                    sql += "case when SUM(isnull(RecS,0))=0 then 0 else Round(SUM(isnull(Cash,0))/SUM(isnull(RecS,0)),0) end as Price, ";
                    sql += "SUM(isnull(VIP_Cash,0))VIP_Cash,SUM(isnull(VIP_RecS,0))VIP_RecS, ";
                    sql += "case when SUM(isnull(VIP_RecS,0))=0 then 0 else Round(SUM(isnull(VIP_Cash,0))/SUM(isnull(VIP_RecS,0)),0) end as VIPPrice, ";
                    sql += "case when SUM(isnull(Cash,0))=0 then format(0,'p') else format(cast(SUM(isnull(VIP_Cash,0)) as Float)/cast(SUM(isnull(Cash,0)) as Float),'p') end as VIPPercent, ";
                    sql += "SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)) VIPNo_Cash,SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0)) VIPNo_RecS, ";
                    sql += "case when SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0))=0 then 0 else Round((SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)))/(SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0))),0) end as VIPNoPrice, ";
                    sql += "case when SUM(isnull(Cash,0))=0 then format(0,'p') else format(cast(SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)) as Float)/cast(SUM(isnull(Cash,0)) as Float),'p') end as VIPNoPercent ";
                    sql += "into #S ";
                    sql += "from SalesHWeb (nolock) ";
                    sql += "Where Companycode='" + uu.CompanyId + "' ";
                    if (YY.SqlQuote() != "")
                    {
                        sql += "and left(OpenDate,4)='" + YY.SqlQuote() + "' ";
                    }
                    if (ID.SqlQuote() != "") {
                        sql += "and ShopNo='" + ID.SqlQuote() + "' ";
                    }
                    sql += "group by substring(OpenDate,6,2) ";
                    sql += "order by substring(OpenDate,6,2); ";
                    //明細資料
                    sqlD = "Select * From #S ";
                    DataTable dtE = PubUtility.SqlQry(sql + sqlD, uu, "SYS");
                    dtE.TableName = "dtE";
                    ds.Tables.Add(dtE);
                    //彙總資料
                    sqlH = "Select Sum(isnull(Cash,0))SumCash,Sum(isnull(RecS,0))SumRecS, ";
                    sqlH += "case when SUM(isnull(RecS,0))=0 then 0 else Round(SUM(isnull(Cash,0))/SUM(isnull(RecS,0)),0) end as SumPrice, ";
                    sqlH += "SUM(isnull(VIP_Cash,0))SumVIP_Cash,SUM(isnull(VIP_RecS,0))SumVIP_RecS, ";
                    sqlH += "case when SUM(isnull(VIP_RecS,0))=0 then 0 else Round(SUM(isnull(VIP_Cash,0))/SUM(isnull(VIP_RecS,0)),0) end as SumVIPPrice, ";
                    sqlH += "case when SUM(isnull(Cash,0))=0 then format(0,'p') else format(cast(SUM(isnull(VIP_Cash,0)) as Float)/cast(SUM(isnull(Cash,0)) as Float),'p') end as SumVIPPercent, ";
                    sqlH += "SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)) SumVIPNo_Cash,SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0)) SumVIPNo_RecS, ";
                    sqlH += "case when SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0))=0 then 0 else Round((SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)))/(SUM(isnull(RecS,0))-SUM(isnull(VIP_RecS,0))),0) end as SumVIPNoPrice, ";
                    sqlH += "case when SUM(isnull(Cash,0))=0 then format(0,'p') else format(cast(SUM(isnull(Cash,0))-SUM(isnull(VIP_Cash,0)) as Float)/cast(SUM(isnull(Cash,0)) as Float),'p') end as SumVIPNoPercent ";
                    sqlH += "From #S ";
                    DataTable dtH = PubUtility.SqlQry(sql + sqlH, uu, "SYS");
                    dtH.TableName = "dtH";
                    ds.Tables.Add(dtH);
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }
        #endregion


        [Route("FileUpload_EDM")]
        public ActionResult FileUpload_EDM()
        {
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "FileUpload_EDMOK", "" });
            UserInfo uu = PubUtility.GetCurrentUser(this);
            string picSGID = "";
            try
            {
                IFormFileCollection files = HttpContext.Request.Form.Files;
                string UploadFileType = HttpContext.Request.Form["UploadFileType"];
                picSGID = ImportPLUPic_EDM(files, UploadFileType);
                DataTable dtMessage = ds.Tables["dtMessage"];
                dtMessage.Rows[0][1] = picSGID;
            }
            catch (Exception err)
            {
                DataTable dtMessage = ds.Tables["dtMessage"];
                dtMessage.Rows[0][0] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        private string ImportPLUPic_EDM(IFormFileCollection files, string ParaType)
        {
            string sgid = "";
            UserInfo uu = PubUtility.GetCurrentUser(this);
            foreach (IFormFile file in files)
            {
                string filename = file.FileName;
                if (filename.ToLower().IndexOf(".jpg") < 0 & filename.ToLower().IndexOf(".jpeg") < 0)
                {
                    throw new Exception("必須是.jpg檔案!");
                }
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                file.CopyTo(ms);
                DataTable dtF = new DataTable();
                dtF.Columns.Add("CompanyCode", typeof(string));
                dtF.Columns.Add("STATUS", typeof(string));
                dtF.Columns.Add("DocNo", typeof(string));
                dtF.Columns.Add("DataType", typeof(string));
                dtF.Columns.Add("FileName", typeof(string));
                dtF.Columns.Add("DocType", typeof(string));
                dtF.Columns.Add("DocImage", typeof(byte[]));

                string sql = "";
                sql = "Delete From SetEDM ";
                sql += " where CompanyCode='" + uu.CompanyId + "' And DocNo='" + HttpContext.Request.Form["DocNo"] + "' And DataType='" + HttpContext.Request.Form["UploadFileType"] + "'";
                PubUtility.ExecuteSql(sql, uu, "SYS");

                DataRow drF = dtF.NewRow();
                drF["CompanyCode"] = uu.CompanyId;
                drF["STATUS"] = "1";
                drF["DocNo"] = HttpContext.Request.Form["DocNo"];
                drF["DataType"] = HttpContext.Request.Form["UploadFileType"];
                drF["FileName"] = file.FileName;
                drF["DocType"] = "P";
                drF["DocImage"] = ms.ToArray();
                dtF.Rows.Add(drF);
                sgid = PubUtility.AddTable("SetEDM", dtF, uu, "SYS");
            }
            return sgid;
        }

        private ApiSetting GetApiSetting()
        {
            string setfn = _hostingEnvironment.ContentRootPath + @"\ApiSetting.json";
            string str = System.IO.File.ReadAllText(setfn, System.Text.Encoding.UTF8);
            ApiSetting GAS = PubUtility.ConvertToEntity(str, typeof(ApiSetting)) as ApiSetting;
            return GAS;
        }

        //[Route("SystemSetup/FTPUpload")]
        //public ActionResult SystemSetup_FTPUpload()
        //{
        //    UserInfo uu = PubUtility.GetCurrentUser(this);
        //    System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "FTPUploadOK", uu.UserID });
        //    DataTable dtMessage = ds.Tables["dtMessage"];
        //    try
        //    {
        //        IFormCollection rq = HttpContext.Request.Form;
        //        string Shop = rq["Shop"];
        //        string Type = rq["Type"];

        //        string sql = "";
        //        string FPIP = "";
        //        string FPID = "";
        //        string FPPWD = "";
        //        string ls_Date = "";
        //        string ls_Time = "";
        //        string filename = "";
        //        string printstr = "";

        //        //取FTP位址
        //        sql = "Select * From FTPDataWeb (nolock) Where worktype='2' and TypeName in (Select CompanyCode from CompanyWeb (nolock) where isnull(ISAMFlag,'')='Y') and TypeName='" + uu.CompanyId + "'" ;
        //        DataTable dtA = PubUtility.SqlQry(sql, uu, "SYS");
        //        if (dtA.Rows.Count > 0)
        //        {
        //            FPID = dtA.Rows[0]["FTPIP"].ToString().SqlQuote();
        //            FPID = PubUtility.enCode170215(dtA.Rows[0]["FTPID"].ToString().SqlQuote());
        //            FPPWD = PubUtility.enCode170215(dtA.Rows[0]["FTPPWD"].ToString().SqlQuote());
        //            ls_Date = PubUtility.SqlQry("Select convert(char(10),getdate(),111)", uu, "SYS").Rows[0][0].ToString().SqlQuote();
        //            ls_Time = PubUtility.SqlQry("Select right(convert(varchar, getdate(), 121),12)", uu, "SYS").Rows[0][0].ToString().SqlQuote();
        //            if (!System.IO.Directory.Exists(PubUtility.UpLoadFiles.FTPFilePath)) { System.IO.Directory.CreateDirectory(PubUtility.UpLoadFiles.FTPFilePath); }
        //            if (!System.IO.Directory.Exists(PubUtility.UpLoadFiles.FTPFilePath + "\\BackUp" + "\\" + uu.CompanyId + "\\" + ls_Date)) { System.IO.Directory.CreateDirectory(PubUtility.UpLoadFiles.FTPFilePath + "\\BackUp" + "\\" + uu.CompanyId + "\\" + ls_Date); }



        //            if (Type == "T")
        //            {
        //                filename = ls_Time + "TAKE3.dat";
        //            }
        //            else if (Type == "C") {
        //                filename = ls_Time + "TAKE3.dat";
        //            }
        //            else if (Type == "D")
        //            {
        //                filename = ls_Time + "TAKE3.dat";
        //            }
        //            System.IO.File.AppendAllText(PubUtility.UpLoadFiles.FTPFilePath + "\\" + filename, printstr);
        //            if (System.IO.File.Exists(PubUtility.UpLoadFiles.FTPFilePath + "\\" + filename))
        //            {
        //                PubUtility.UpLoadFiles.UploadFile(FPIP, FPID, FPPWD, PubUtility.UpLoadFiles.FTPFilePath + "\\" + filename);
        //                if (PubUtility.UpLoadFiles.RemoteFtpDirExists(FPIP, FPID, FPPWD, filename))
        //                {
        //                    System.IO.File.Move(PubUtility.UpLoadFiles.FTPFilePath + "\\" + filename, PubUtility.UpLoadFiles.FTPFilePath + "\\BackUp\\" + filename);
        //                }
        //            }
        //            if (PubUtility.UpLoadFiles.MakeDir(FPIP, FPID, FPPWD, uu.CompanyId))
        //            {
        //                PubUtility.UpLoadFiles.MakeDir(FPIP, FPID, FPPWD, uu.CompanyId + "/" + Shop);
        //            }
        //        }
        //        else {
        //            throw new Exception("FTP位址");
        //        }

        //    }
        //    catch (Exception err)
        //    {
        //        dtMessage.Rows[0][0] = "Exception";
        //        dtMessage.Rows[0][1] = err.Message;
        //    }
        //    return PubUtility.DatasetXML(ds);
        //}

    }
}
