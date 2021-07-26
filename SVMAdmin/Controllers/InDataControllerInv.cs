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


namespace SVMAdmin.Controllers
{
    [Route("api")]
    [ApiController]
    public class DataControllerInv : ControllerBase
    {


        //private string GetNewDocNo(UserInfo uu, String DocType, Int16 Digits)
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


        //2021-05-26 Larry
        [Route("SystemSetup/SearchVIN13_2")]
        public ActionResult SystemSetup_SearchVIN13_2()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchVIN13_2OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;

                string WhNo = rq["WhNo"];
                string CkNo = rq["CkNo"];
                string Layer = rq["Layer"];
                string exDate = rq["exDate"];

                string sql = "select a.*, a.WhNo+b.ST_SNAME WhName,c.Man_Name , d.GD_SName OldName, e.GD_SName NewName, a.Layer+Sno LayerSno ,b.ST_OpenDay ";
                sql += " ,Case When IsNull(a.AppDate,'')='' Then '未批核' Else '已批核' End AppStatus";
                sql += " ,Case When IsNull(a.FinishDate,'')='' Then (Case When IsNull(DefeasanceDate,'')<>'' Then '作廢' Else '未完成' End) Else '完成' End FinStatus";
                sql += " from ChangePLUSV a";
                sql += " inner join WarehouseSV b on a.WhNo=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " left join EmployeeSV c on a.DocUser=c.Man_ID And a.CompanyCode=c.CompanyCode ";
                sql += " left join PLUSV d on a.PLUOld=d.GD_No And a.CompanyCode=d.CompanyCode  ";
                sql += " left join PLUSV e on a.PLUNew=e.GD_No And a.CompanyCode=e.CompanyCode ";
                sql += " where a.CompanyCode='" + uu.CompanyId + "'";
                if (WhNo != "")
                {
                    sql += " and a.WhNo='" + WhNo + "'";
                }
                if (CkNo != "")
                {
                    sql += " and a.CkNo='" + CkNo + "'";
                }
                if (Layer != "")
                {
                    sql += " and a.Layer='" + Layer + "'";
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


        //2021-05-26 Larry
        [Route("SystemSetup/UpdateChgPLU")]
        public ActionResult SystemSetup_UpdateChgPLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateChgPLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                DataTable dtChgPLU = new DataTable("ChangePLUSV");
                PubUtility.AddStringColumns(dtChgPLU, "DocNo,PLUNew,Num,DisplayNum,ExchangeDate");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtChgPLU);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtChgPLU.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {

                            sql = "update ChangePLUSV set ";
                            sql += " PLUNew='" + dr["PLUNew"].ToString().SqlQuote() + "'";
                            sql += ",Num=" + dr["Num"].ToString().SqlQuote() + "";
                            sql += ",DisplayNum=" + dr["DisplayNum"].ToString().SqlQuote() + "";
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

                sql = "select a.*, a.WhNo+b.ST_SNAME WhName,c.Man_Name , d.GD_SName OldName, e.GD_SName NewName, a.Layer+Sno LayerSno ,b.ST_OpenDay ";
                sql += " ,Case When IsNull(a.AppDate,'')='' Then '未批核' Else '已批核' End AppStatus";
                sql += " ,Case When IsNull(a.FinishDate,'')='' Then (Case When IsNull(DefeasanceDate,'')<>'' Then '作廢' Else '未完成' End) Else '完成' End FinStatus";
                sql += " from ChangePLUSV a";
                sql += " inner join WarehouseSV b on a.WhNo=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " left join EmployeeSV c on a.DocUser=c.Man_ID And a.CompanyCode=c.CompanyCode ";
                sql += " left join PLUSV d on a.PLUOld=d.GD_No And a.CompanyCode=d.CompanyCode  ";
                sql += " left join PLUSV e on a.PLUNew=e.GD_No And a.CompanyCode=e.CompanyCode ";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";

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
        [Route("SystemSetup/DelChgPLU")]
        public ActionResult SystemSetup_DelChgShop()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "DelChgPLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtChgPLU = new DataTable("ChangePLUSV");
                PubUtility.AddStringColumns(dtChgPLU, "DocNo");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtChgPLU);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtChgPLU.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {

                            sql = "Delete From ChangePLUSV ";
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

                sql = "select a.*, a.WhNo+b.ST_SNAME WhName,c.Man_Name , d.GD_SName OldName, e.GD_SName NewName, a.Layer+Sno LayerSno ,b.ST_OpenDay ";
                sql += " ,Case When IsNull(a.AppDate,'')='' Then '未批核' Else '已批核' End AppStatus";
                sql += " ,Case When IsNull(a.FinishDate,'')='' Then (Case When IsNull(DefeasanceDate,'')<>'' Then '作廢' Else '未完成' End) Else '完成' End FinStatus";
                sql += " from ChangePLUSV a";
                sql += " inner join WarehouseSV b on a.WhNo=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " left join EmployeeSV c on a.DocUser=c.Man_ID And a.CompanyCode=c.CompanyCode ";
                sql += " left join PLUSV d on a.PLUOld=d.GD_No And a.CompanyCode=d.CompanyCode  ";
                sql += " left join PLUSV e on a.PLUNew=e.GD_No And a.CompanyCode=e.CompanyCode ";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' ";

                //sql = "select a.*";
                //sql += " from ChangePLUSV a";
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



        //2021-05-26 Larry
        [Route("SystemSetup/AppChgPLU")]
        public ActionResult SystemSetup_AppChgPLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            //批核完成後，同樣執行異動完成，故回傳UpdateChgPLUOK
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateChgPLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtChgPLU = new DataTable("ChangePLUSV");
                PubUtility.AddStringColumns(dtChgPLU, "DocNo");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtChgPLU);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtChgPLU.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {

                            sql = "update ChangePLUSV set ";
                            sql += " AppDate=convert(char(10),getdate(),111)";
                            sql += ",AppUser='" + uu.UserID + "'";
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

                sql = "select a.*, a.WhNo+b.ST_SNAME WhName,c.Man_Name , d.GD_SName OldName, e.GD_SName NewName, a.Layer+Sno LayerSno ,b.ST_OpenDay ";
                sql += " ,Case When IsNull(a.AppDate,'')='' Then '未批核' Else '已批核' End AppStatus";
                sql += " ,Case When IsNull(a.FinishDate,'')='' Then (Case When IsNull(DefeasanceDate,'')<>'' Then '作廢' Else '未完成' End) Else '完成' End FinStatus";
                sql += " from ChangePLUSV a";
                sql += " inner join WarehouseSV b on a.WhNo=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " left join EmployeeSV c on a.DocUser=c.Man_ID And a.CompanyCode=c.CompanyCode ";
                sql += " left join PLUSV d on a.PLUOld=d.GD_No And a.CompanyCode=d.CompanyCode  ";
                sql += " left join PLUSV e on a.PLUNew=e.GD_No And a.CompanyCode=e.CompanyCode ";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";

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
        [Route("SystemSetup/DefChgPLU")]
        public ActionResult SystemSetup_DefChgPLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            //作廢完成後，同樣執行異動完成，故回傳UpdateChgPLUOK
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "UpdateChgPLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataTable dtChgPLU = new DataTable("ChangePLUSV");
                PubUtility.AddStringColumns(dtChgPLU, "DocNo");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtChgPLU);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtChgPLU.Rows[0];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {

                            sql = "update ChangePLUSV set ";
                            sql += " DefeasanceDate=convert(char(10),getdate(),111)";
                            sql += ",Defeasance='" + uu.UserID + "'";
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

                sql = "select a.*, a.WhNo+b.ST_SNAME WhName,c.Man_Name , d.GD_SName OldName, e.GD_SName NewName, a.Layer+Sno LayerSno ,b.ST_OpenDay ";
                sql += " ,Case When IsNull(a.AppDate,'')='' Then '未批核' Else '已批核' End AppStatus";
                sql += " ,Case When IsNull(a.FinishDate,'')='' Then (Case When IsNull(DefeasanceDate,'')<>'' Then '作廢' Else '未完成' End) Else '完成' End FinStatus";
                sql += " from ChangePLUSV a";
                sql += " inner join WarehouseSV b on a.WhNo=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " left join EmployeeSV c on a.DocUser=c.Man_ID And a.CompanyCode=c.CompanyCode ";
                sql += " left join PLUSV d on a.PLUOld=d.GD_No And a.CompanyCode=d.CompanyCode  ";
                sql += " left join PLUSV e on a.PLUNew=e.GD_No And a.CompanyCode=e.CompanyCode ";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "'";

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
        [Route("SystemSetup/AddChgPLU")]
        public ActionResult SystemSetup_AddChgPLU()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "AddChgPLUOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                string DocNo = PubUtility.GetNewDocNo(uu, "CA", 3);

                DataTable dtRec = new DataTable("ChangePLUSV");
                PubUtility.AddStringColumns(dtRec, "WhNo,CkNo,Layer,Sno,OldPLU,NewPLU,Num,DisplayNum,ExchangeDate");
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
                            sql = "Insert Into ChangePLUSV (CompanyCode, CrtUser, CrtDate, CrtTime ";
                            sql += ", ModUser, ModDate, ModTime ";
                            sql += ", DocNo, DocUser, DocDate ";
                            sql += ", ExchangeDate, WhNo, CkNo " 
                                +  ", Layer, Sno "
                                +  ", PLUOld, PLUNew, Num, DisplayNum) Values ";
                            sql += " ('" + uu.CompanyId + "', '" + uu.UserID + "', convert(char(10),getdate(),111), convert(char(12),getdate(),108) ";
                            sql += ", '" + uu.UserID + "', convert(char(10),getdate(),111), convert(char(12),getdate(),108) ";
                            sql += ", '" + DocNo + "', '" + uu.UserID + "',convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(12),getdate(),108),1,5) ";
                            sql += ", '" + dr["ExchangeDate"].ToString().SqlQuote() + "','" + dr["WhNo"].ToString().SqlQuote() + "','" + dr["CkNo"].ToString().SqlQuote() + "'"
                                + ", '" + dr["Layer"].ToString().SqlQuote() + "', '" + dr["Sno"].ToString().SqlQuote() + "'" 
                                + ", '" + dr["OldPLU"].ToString().SqlQuote() + "', '" + dr["NewPLU"].ToString().SqlQuote() + "', " + dr["Num"].ToString().SqlQuote() + " , " + dr["DisplayNum"].ToString().SqlQuote() + ")";
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
                sql = "select a.*, a.WhNo+b.ST_SNAME WhName,c.Man_Name , d.GD_SName OldName, e.GD_SName NewName, a.Layer+Sno LayerSno ,b.ST_OpenDay ";
                sql += " ,Case When IsNull(a.AppDate,'')='' Then '未批核' Else '已批核' End AppStatus";
                sql += " ,Case When IsNull(a.FinishDate,'')='' Then (Case When IsNull(DefeasanceDate,'')<>'' Then '作廢' Else '未完成' End) Else '完成' End FinStatus";
                sql += " from ChangePLUSV a";
                sql += " inner join WarehouseSV b on a.WhNo=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " left join EmployeeSV c on a.DocUser=c.Man_ID And a.CompanyCode=c.CompanyCode ";
                sql += " left join PLUSV d on a.PLUOld=d.GD_No And a.CompanyCode=d.CompanyCode  ";
                sql += " left join PLUSV e on a.PLUNew=e.GD_No And a.CompanyCode=e.CompanyCode ";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + DocNo + "'";
                DataTable dtChgPLU = PubUtility.SqlQry(sql, uu, "SYS");
                dtChgPLU.TableName = "dtChgPLU";
                ds.Tables.Add(dtChgPLU);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        //GetInitVIN14_2-->VIN14_2、VIN14_3共用
        [Route("SystemSetup/GetInitVIN14_2")]
        public ActionResult SystemSetup_GetInitVIN14_2()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitVIN14_2OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "select ST_ID,ST_ID+ST_SName ST_SName";
                sql += " from WarehouseSV ";
                sql += " where CompanyCode='" + uu.CompanyId + "' And ST_Type='6' Order By ST_ID";
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


        [Route("SystemSetup/SearchVIN14_2")]
        public ActionResult SystemSetup_SearchVIN14_2()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchVIN14_2OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            using (DBOperator dbop = new DBOperator())
                try
                {
                    IFormCollection rq = HttpContext.Request.Form;
                    string WhNo = rq["WhNo"];
                    string CkNo = rq["CkNo"];
                    string LayerNo = rq["LayerNo"];

                    string DocNo = PubUtility.GetNewDocNo(uu, "VC", 6);

                    string sql = "Insert Into TempDocumentSV ";
                    sql += "(CompanyCode,CrtUser,CrtDate,CrtTime,DocNo,SeqNo,PLU,Qty";
                    sql += ",Qty2,RNum, WhNo, CkNo, Layer, Sno, EffectiveDate, WorkType)";
                    sql += "select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)";
                    sql += " ,'" + DocNo + "', Cast(Row_Number() Over(Order By a.Layer,a.Sno) As int), a.PLU, 0, "
                            + "0, 0, a.WhNo, a.CkNo, a.Layer, a.Sno, a.EffectiveDate, 'IN' ";
                    sql += " from InventorySV a (Nolock) ";
                    sql += " inner join PLUSV b (Nolock) on a.CompanyCode=b.CompanyCode And a.PLU=b.GD_NO";
                    sql += " where a.CompanyCode='" + uu.CompanyId + "' And b.GD_Flag1='1' ";

                    if (WhNo != "" & WhNo != null)
                    {
                        sql += " and a.WhNo='" + WhNo + "'";
                    }
                    if (CkNo != "" & CkNo != null)
                    {
                        sql += " and a.CkNo='" + CkNo + "'";
                    }
                    if (LayerNo != "" & LayerNo != null)
                    {
                        sql += " and a.Layer='" + LayerNo + "'";
                    }
                    dbop.ExecuteSql(sql, uu, "SYS");

                    sql = "select a.*,a.Layer+a.Sno Channel,c.GD_SName,c.Photo1 ";
                    sql += " , Cast(b.PtNum As VarChar(5))+'/'+Cast(b.DisplayNum As VarChar(5)) ShowQty, b.DisplayNum-b.PtNum ShortQty, d.ST_SName, b.DisplayNum, b.PtNum ";
                    sql += " from tempdocumentsv a (Nolock) ";
                    sql += " inner join InventorySV b (Nolock) on a.WhNo=b.WhNo and a.CkNo=b.CkNo And a.Layer=b.Layer And a.Sno=b.Sno and a.CompanyCode=b.CompanyCode";
                    sql += " inner join PLUSV c (Nolock) on a.PLU=c.GD_NO and a.CompanyCode=c.CompanyCode";
                    sql += " inner join WarehouseSV d (Nolock) on a.WhNo=d.ST_ID and a.CompanyCode=d.CompanyCode";
                    sql += " where a.CompanyCode='" + uu.CompanyId + "' ";
                    sql += " and a.DocNo='" + DocNo + "' ";
                    sql += " Order By a.SeqNo ";

                    DataTable dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                    dtPLU.TableName = "dtPLU";
                    ds.Tables.Add(dtPLU);

                    DataTable dtDocNo = new DataTable("dtDocNo");
                    dtDocNo.Columns.Add("DocNo", typeof(string));

                    DataRow dr = dtDocNo.NewRow();
                    dr["DocNo"] = DocNo;

                    dtDocNo.Rows.Add(dr);
                    ds.Tables.Add(dtDocNo);

                }
                catch (Exception err)
                {
                    dtMessage.Rows[0][0] = "Exception";
                    dtMessage.Rows[0][1] = err.Message;
                }
            return PubUtility.DatasetXML(ds);
        }



        [Route("SystemSetup/SearchVIN14_2Saved")]
        public ActionResult SystemSetup_SearchVIN14_2Saved()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchVIN14_2SavedOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            using (DBOperator dbop = new DBOperator())
                try
                {
                    IFormCollection rq = HttpContext.Request.Form;
                    string DocNo = rq["DocNo"];

                    string sql = "";

                    sql = "select a.*,a.Layer+a.Sno Channel,c.GD_SName,c.Photo1 ";
                    sql += " , Cast(b.PtNum As VarChar(5))+'/'+Cast(b.DisplayNum As VarChar(5)) ShowQty, b.DisplayNum-b.PtNum ShortQty, d.ST_SName, b.DisplayNum, b.PtNum ";
                    sql += " from tempdocumentsv a (Nolock) ";
                    sql += " inner join InventorySV b (Nolock) on a.WhNo=b.WhNo and a.CkNo=b.CkNo And a.Layer=b.Layer And a.Sno=b.Sno and a.CompanyCode=b.CompanyCode";
                    sql += " inner join PLUSV c (Nolock) on a.PLU=c.GD_NO and a.CompanyCode=c.CompanyCode";
                    sql += " inner join WarehouseSV d (Nolock) on a.WhNo=d.ST_ID and a.CompanyCode=d.CompanyCode";
                    sql += " where a.CompanyCode='" + uu.CompanyId + "' ";
                    sql += " and a.DocNo='" + DocNo + "' ";
                    sql += " Order By a.SeqNo ";

                    DataTable dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                    dtPLU.TableName = "dtPLU";
                    ds.Tables.Add(dtPLU);

                    sql = "Delete From TempDocumentSV Where CompanyCode='" + uu.CompanyId + "' and DocNo='" + DocNo + "' ";
                    dbop.ExecuteSql(sql, uu, "SYS");

                }
                catch (Exception err)
                {
                    dtMessage.Rows[0][0] = "Exception";
                    dtMessage.Rows[0][1] = err.Message;
                }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/SearchVIN14_3")]
        public ActionResult SystemSetup_SearchVIN14_3()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchVIN14_3OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            using (DBOperator dbop = new DBOperator())
                try
                {
                    IFormCollection rq = HttpContext.Request.Form;
                    string WhNo = rq["WhNo"];
                    string CkNo = rq["CkNo"];
                    //string LayerNo = rq["LayerNo"];

                    string DocNo = PubUtility.GetNewDocNo(uu, "VC", 6);

                    string sql = "Insert Into TempDocumentSV ";
                    sql += "(CompanyCode,CrtUser,CrtDate,CrtTime,DocNo,SeqNo,PLU,Qty";
                    sql += ",Qty2,RNum, WhNo, CkNo, Layer, Sno, EffectiveDate, WorkType)";
                    sql += "select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)";
                    sql += " ,'" + DocNo + "', Cast(Row_Number() Over(Order By a.Layer,a.Sno) As int), a.PLU, 0, "
                            + "0, 0, a.WhNo, a.CkNo, a.Layer, a.Sno, a.EffectiveDate, 'IN' ";
                    sql += " from InventorySV a (Nolock) ";
                    sql += " inner join PLUSV b (Nolock) on a.CompanyCode=b.CompanyCode And a.PLU=b.GD_NO";
                    sql += " where a.CompanyCode='" + uu.CompanyId + "' And b.GD_Flag1='1' ";

                    if (WhNo != "" & WhNo != null)
                    {
                        sql += " and a.WhNo='" + WhNo + "'";
                    }
                    if (CkNo != "" & CkNo != null)
                    {
                        sql += " and a.CkNo='" + CkNo + "'";
                    }
                    //if (LayerNo != "" & LayerNo != null)
                    //{
                    //    sql += " and a.Layer='" + LayerNo + "'";
                    //}
                    dbop.ExecuteSql(sql, uu, "SYS");

                    sql = "select a.*,a.Layer+a.Sno Channel,c.GD_SName,c.Photo1 ";
                    sql += " , Cast(b.PtNum As VarChar(5))+'/'+Cast(b.DisplayNum As VarChar(5)) ShowQty";
                    sql += ", d.ST_SName, b.DisplayNum, b.PtNum ";
                    sql += " from tempdocumentsv a (Nolock) ";
                    sql += " inner join InventorySV b (Nolock) on a.WhNo=b.WhNo and a.CkNo=b.CkNo And a.Layer=b.Layer And a.Sno=b.Sno and a.CompanyCode=b.CompanyCode";
                    sql += " inner join PLUSV c (Nolock) on a.PLU=c.GD_NO and a.CompanyCode=c.CompanyCode";
                    sql += " inner join WarehouseSV d (Nolock) on a.WhNo=d.ST_ID and a.CompanyCode=d.CompanyCode";
                    sql += " where a.CompanyCode='" + uu.CompanyId + "' ";
                    sql += " and a.DocNo='" + DocNo + "' ";
                    sql += " Order By a.SeqNo ";

                    DataTable dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                    dtPLU.TableName = "dtPLU";
                    ds.Tables.Add(dtPLU);

                    DataTable dtDocNo = new DataTable("dtDocNo");
                    dtDocNo.Columns.Add("DocNo", typeof(string));

                    DataRow dr = dtDocNo.NewRow();
                    dr["DocNo"] = DocNo;

                    dtDocNo.Rows.Add(dr);
                    ds.Tables.Add(dtDocNo);

                }
                catch (Exception err)
                {
                    dtMessage.Rows[0][0] = "Exception";
                    dtMessage.Rows[0][1] = err.Message;
                }
            return PubUtility.DatasetXML(ds);
        }



        //2021-06-21 Larry
        [Route("SystemSetup/SaveVIN14_3")]
        public ActionResult SystemSetup_SaveVIN14_3()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveVIN14_3OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                DataTable dtTemp = new DataTable("TempDocumentSV");
                PubUtility.AddStringColumns(dtTemp, "DocNo,WhNo,CkNo");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtTemp);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtTemp.Rows[0];

                string tDocNo = PubUtility.GetNewDocNo(uu, "TH", 6);
                string uDocNo = PubUtility.GetNewDocNo(uu, "UH", 4);

                string sql = "";
                string WhNoIn = "";
                string SysDate = "";


                sql = "select convert(char(10),getdate(),111) SysDate";

                DataTable dtSysDate = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtSysDate.Rows.Count > 0)
                {
                    SysDate = dtSysDate.Rows[0][0].ToString();
                }


                sql = "Select * From TempDocumentSV a ";
                sql += " Where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
                sql += " And IsNull(Qty,0) <> 0 ";
                sql += " And IsNull(ModDate,'')<>'' ";
                DataTable dtChkQty = PubUtility.SqlQry(sql, uu, "SYS");

                bool bSameWh = false;
                string CkNoIn = ""; string LayerIn = "";

                if (dtChkQty.Rows.Count > 0)
                {

                    sql = "Select WhNoIn From WarehouseDSV "
                        + "Where CompanyCode='" + uu.CompanyId + "' "
                        + "And ST_ID='" + dr["WhNo"].ToString().SqlQuote() + "' "
                        + "And CkNo='" + dr["CkNo"].ToString().SqlQuote() + "' ";
                    DataTable dtWhNoOut = PubUtility.SqlQry(sql, uu, "SYS");

                    if (dtWhNoOut.Rows.Count > 0)
                    {
                        WhNoIn = dtWhNoOut.Rows[0][0].ToString();
                    }


                    if (WhNoIn == dr["WhNo"].ToString())
                    {
                        bSameWh = true; CkNoIn = "00"; LayerIn = "Z";
                    }
                    else
                    {
                        bSameWh = false; CkNoIn = "XX"; LayerIn = "";
                    }

                }



                sql = "Select * From TempDocumentSV a ";
                sql += " Where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
                sql += " And IsNull(Qty2,0) <> 0 ";
                sql += " And IsNull(ModDate,'')<>'' ";
                DataTable dtChkQty2 = PubUtility.SqlQry(sql, uu, "SYS");



                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {

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
                                    + ", '" + tDocNo + "', convert(char(10),getdate(),111), '" + dr["WhNo"].ToString().SqlQuote() + "', '" + uu.UserID + "'"
                                    + ", convert(char(10),getdate(),111),'" + uu.UserID + "','" + WhNoIn + "'"
                                    + ", '', '" + uu.UserID + "', convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5)"
                                    + ", '" + uu.UserID + "', convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5),'V' "
                                    + ",'" + dr["CkNo"].ToString().SqlQuote() + "', '" + CkNoIn + "', 'IB')";
                                dbop.ExecuteSql(sql, uu, "SYS");

                                //調撥表身
                                sql = "Insert Into TransferDSV (CompanyCode, CrtUser, CrtDate, CrtTime, ModUser, ModDate, ModTime, " +
                                    "TH_ID, SeqNo, PLU, OutNum, InNum, " +
                                    "GD_Retail, Amt, LayerIn, SnoIn, LayerOut, EffectiveDate, SnoOut) ";
                                sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", '" + tDocNo + "', Cast(Row_Number() Over(Order By a.Layer,a.Sno) As int), a.PLU, Qty, Qty"
                                     + ", b.GD_Retail, Qty*GD_Retail, '" + LayerIn + "' ";
                                if (LayerIn == "Z")
                                {
                                    sql += ", d.Sno ";
                                }
                                else
                                {
                                    sql += ", '' ";
                                }
                                sql += ",a.Layer, a.EffectiveDate, a.Sno ";
                                sql += " From TempDocumentSV a (Nolock) ";
                                sql += " Inner Join PLUSV b (Nolock) On a.CompanyCode=b.CompanyCode And a.PLU=b.GD_No ";
                                //sql += " Inner Join WarehouseDSV c (Nolock) On a.CompanyCode=c.CompanyCode And a.PLU=c.GD_No ";
                                sql += " Left Join InventorySV d (Nolock) On a.CompanyCode=d.CompanyCode And a.PLU=d.PLU "
                                    + "And d.WhNo='" + WhNoIn + "' And d.CkNo='" + CkNoIn + "' And d.Layer='" + LayerIn + "' ";

                                sql += " Where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
                                sql += " And IsNull(a.Qty,0)>0 ";
                                sql += " And IsNull(a.ModDate,'')<>'' ";
                                dbop.ExecuteSql(sql, uu, "SYS");



                                //變更庫存數量及庫存增減日
                                //調出方
                                string sqlTROut = "";
                                sqlTROut = "Select H.CompanyCode, H.TH_ID, H.WhNoOut, H.CkNoOut, D.LayerOut, D.SnoOut, D.SeqNo, D.PLU, D.OutNum, D.EffectiveDate "
                                    + " From TransferHSV H Inner Join TransferDSV D "
                                    + " On H.CompanyCode = D.CompanyCode And H.TH_ID = D.TH_ID "
                                    + " Where H.CompanyCode='" + uu.CompanyId + "' And H.TH_ID='" + tDocNo + "' ";


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
                                    + ",EffectiveDate=a.EffectiveDate "
                                    + "From (" + sqlTROut + ") a Inner Join InventorySV b "
                                    + "On a.CompanyCode=b.CompanyCode and a.WhNoOut=b.WhNo and a.CkNoOut=b.CkNo "
                                    + "and a.LayerOut=b.Layer And a.SnoOut=b.Sno And a.PLU=b.PLU ";
                                sql += " Where b.CompanyCode='" + uu.CompanyId + "' ";
                                dbop.ExecuteSql(sql, uu, "SYS");

                                //沒有庫存資料-新增
                                sql = "Insert Into InventorySV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                    + ", ModUser, ModDate, ModTime "
                                    + ", WhNo, a.PLU, Out_Date, CkNo, Layer, Sno, PtNum, SafeNum, EffectiveDate ) ";
                                sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", WhNoOut, PLU, '" + SysDate + "', CkNoOut, LayerOut, SnoOut, -1*OutNum, 1, a.EffectiveDate "
                                     + " From (" + sqlTROut + ") a Left Join InventorySV b "
                                     + " On a.CompanyCode = b.CompanyCode and a.WhNoOut = b.WhNo and a.CkNoOut = b.CkNo "
                                     + "and a.LayerOut=b.Layer And a.SnoOut=b.Sno And a.PLU=b.PLU "
                                     + "Where b.PLU Is Null ";
                                dbop.ExecuteSql(sql, uu, "SYS");


                                //調入方
                                if (CkNoIn != "XX")
                                {

                                    string sqlTRIn = "";
                                    sqlTRIn = "Select H.CompanyCode, H.TH_ID, H.WhNoIn, H.CkNoIn, D.LayerIn, D.SnoIn, D.SeqNo, D.PLU, D.InNum, T.EffectiveDate "
                                        + " From TransferHSV H Inner Join TransferDSV D "
                                        + " On H.CompanyCode = D.CompanyCode And H.TH_ID = D.TH_ID "
                                        + " Inner Join TempDocumentSV T On H.CompanyCode = T.CompanyCode And H.WhNoIn=T.WhNo And H.CkNoIn=T.CkNo And D.LayerIn=T.Layer And D.SnoIn=T.Sno "
                                        + " Where H.CompanyCode='" + uu.CompanyId + "' And H.TH_ID='" + tDocNo + "' And T.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
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

                                


                            }


                            //有報廢數量時，才轉出報廢單
                            if (dtChkQty2.Rows.Count > 0)
                            {

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




        //2021-05-26 Larry
        [Route("SystemSetup/SearchVIN14_4")]
        public ActionResult SystemSetup_SearchVIN14_4()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchVIN14_4OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;

                string WhNo = rq["WhNo"];
                string CkNo = rq["CkNo"];
                string exDate = rq["exDate"];
                string FinStatus = rq["FinStatus"];

                string sql = "select a.*,a.WhNoOut+b.ST_SNAME WhOut,b.ST_SNAME WhOutName, a.WhNoIn+c.ST_SName WhIn,c.ST_SName WhInName,d.Man_Name";
                sql += " ,Case When IsNull(a.AppDate,'')='' Then '未批核' Else '已批核' End AppStatus";
                sql += " ,Case When IsNull(a.FinishDate,'')='' Then '未完成' Else '完成' End FinStatus";
                sql += " ,c.ST_OpenDay";
                sql += " from ChangeShopSV a";
                sql += " inner join WarehouseSV b on a.WhNoOut=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " inner join WarehouseSV c on a.WhNoIn=c.ST_ID And a.CompanyCode=c.CompanyCode";
                sql += " left  join EmployeeSV d on a.DocUser=d.Man_ID And a.CompanyCode=d.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "'";
                sql += " and IsNull(a.AppDate,'')<>''";
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
                if (FinStatus == "Y")
                {
                    sql += " and IsNull(a.FinishDate,'')<>''";
                }
                else if (FinStatus == "N")
                {
                    sql += " and IsNull(a.FinishDate,'')=''";
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


        //
        [Route("SystemSetup/ChkChangeShop")]
        public ActionResult SystemSetup_ChkChangeShop()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ChkChangeShopOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string WhNo = rq["WhNo"];

                //檢查是否有調撥單未過帳
                string sql = "Select * from TransferHSV Where CompanyCode='" + uu.CompanyId + "' ";
                sql += " And IsNull(PostDate,'')='' ";
                sql +=" And (WhNoOut='" + WhNo + "' Or WhNoIn='" + WhNo + "') ";

                DataTable dtChkTF = PubUtility.SqlQry(sql, uu, "SYS");
                dtChkTF.TableName = "dtChkTF";
                ds.Tables.Add(dtChkTF);

                //檢查是否有報廢單未過帳
                sql = "Select * from UselessHSV Where CompanyCode='" + uu.CompanyId + "' ";
                sql += " And IsNull(PostDate,'')='' ";
                sql += " And (WhNoOut='" + WhNo + "' ) ";

                DataTable dtChkUseless = PubUtility.SqlQry(sql, uu, "SYS");
                dtChkUseless.TableName = "dtChkUseless";
                ds.Tables.Add(dtChkUseless);

                //檢查是否有調整單未過帳
                sql = "Select * from AdjustHSV Where CompanyCode='" + uu.CompanyId + "' ";
                sql += " And IsNull(PostDate,'')='' ";
                sql += " And (WhNo='" + WhNo + "' ) ";

                DataTable dtChkAdj = PubUtility.SqlQry(sql, uu, "SYS");
                dtChkAdj.TableName = "dtChkAdj";
                ds.Tables.Add(dtChkAdj);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }



        [Route("SystemSetup/FinishChgShop")]
        public ActionResult SystemSetup_FinishChgShop()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "FinishChgShopOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                //DataTable dtRec = new DataTable("Rack");
                //PubUtility.AddStringColumns(dtRec, "OldType_ID,Type_ID,Type_Name,DisplayNum");
                //DataSet dsRQ = new DataSet();
                //dsRQ.Tables.Add(dtRec);
                //PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                //DataRow dr = dtRec.Rows[0];

                IFormCollection rq = HttpContext.Request.Form;
                string DocNo = rq["DocNo"];
                string WhNoOut = rq["WhNoOut"];
                string CkNoOut = rq["CkNoOut"];
                string WhNoIn = rq["WhNoIn"];
                string CkNoIn = rq["CkNoIn"];

                
                string sql = "";
                string SysDate = "";

                string OldWareDSVIn = ""; string NewWareDSVIn = "";

                sql = "select convert(char(10),getdate(),111) SysDate";
                DataTable dtSysDate = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtSysDate.Rows.Count > 0)
                {
                    SysDate = dtSysDate.Rows[0][0].ToString();
                }

                sql = "select WhNoIn From WarehouseDSV (Nolock) " 
                    + "Where CompanyCode='" + uu.CompanyId.SqlQuote() + "' And ST_ID='" + WhNoOut + "' And CkNo='" + CkNoOut + "' ";
                DataTable dtOldWareDSVIn = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtOldWareDSVIn.Rows.Count > 0)
                {
                    OldWareDSVIn = dtOldWareDSVIn.Rows[0][0].ToString();
                }

                sql = "select WhNoIn From WarehouseDSV (Nolock) "
                    + "Where CompanyCode='" + uu.CompanyId.SqlQuote() + "' And ST_ID='" + WhNoOut + "' And CkNo='" + CkNoOut + "' ";
                DataTable dtNewWareDSVIn = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtNewWareDSVIn.Rows.Count > 0)
                {
                    NewWareDSVIn = dtNewWareDSVIn.Rows[0][0].ToString();
                }

                string OldDocNo = PubUtility.GetNewDocNo(uu, "TH", 6);
                string NewDocNo = PubUtility.GetNewDocNo(uu, "TH", 6);


                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {

                            //寫入調撥資料
                            //原智販店調撥表頭

                            string sCkNo = ""; string sLayer = "";
                            if (OldWareDSVIn != WhNoOut)
                            {
                                sCkNo = "XX";
                                sLayer = "";
                            }
                            else
                            {
                                sCkNo = "00";
                                sLayer = "Z";
                            }
                            sql = "Insert Into TransferHSV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                + ",ModUser, ModDate, ModTime "
                                + ",TH_ID, DocDate, WhNoOut, OutUser"
                                + ", InDate, InUser, WhNoIn"
                                + ",ExpressNo, ChkUser, ChkDate, "
                                + "PostUser, PostDate, DocType"
                                + ", CkNoOut, CkNoIn, WorkType) Values ";
                            sql += " ('" + uu.CompanyId.SqlQuote() + "', '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                + ", '" + OldDocNo + "', convert(char(10),getdate(),111), '" + WhNoOut + "', '" + uu.UserID + "'"
                                + ", '" + SysDate + "','" + uu.UserID + "','" + OldWareDSVIn + "'"
                                + ", '" + DocNo + "', '" + uu.UserID + "', convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5)"
                                + ", '" + uu.UserID + "', convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5),'V' "
                                + ",'" + CkNoOut + "', '" + sCkNo + "', 'IS') ";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //原智販店調撥表身
                            sql = "Insert Into TransferDSV (CompanyCode, CrtUser, CrtDate, CrtTime, ModUser, ModDate, ModTime, " +
                                "TH_ID, SeqNo, PLU, OutNum, InNum, " +
                                "GD_Retail, Amt, LayerIn, SnoIn, LayerOut, EffectiveDate, SnoOut) ";
                            sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", '" + OldDocNo + "', Cast(Row_Number() Over(Order By a.Layer,a.Sno) As int), a.PLU, a.PtNum, a.PtNum"
                                 + ", b.GD_Retail, Qty*GD_Retail, '" + sLayer + "', a.Sno, a.Layer"
                                 + ", a.EffectiveDate, a.Sno";
                            sql += " From InventorySV a (Nolock) ";
                            sql += " Inner Join PLUSV b (Nolock) On a.CompanyCode=b.CompanyCode And a.PLU=b.GD_No ";
                            //sql += " Left Join InventorySV d (Nolock) On a.CompanyCode=d.CompanyCode And a.PLU=d.PLU "
                                //+ "And d.WhNo='" + WhNoOut + "' And d.CkNo='" + CkNoOut + "' And d.Layer='Z' ";
                            sql += " Where a.CompanyCode='" + uu.CompanyId + "' "
                                + "And a.WhNo='" + WhNoOut + "' And a.CkNo='" + CkNoOut + "' ";
                            dbop.ExecuteSql(sql, uu, "SYS");


                            //新智販店調撥表頭#########
                            if (NewWareDSVIn != WhNoIn)
                            {
                                sCkNo = "XX"; sLayer = "";
                            }
                            else
                            {
                                sCkNo = "00"; sLayer = "Z";
                            }
                            sql = "Insert Into TransferHSV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                + ",ModUser, ModDate, ModTime "
                                + ",TH_ID, DocDate, WhNoOut, OutUser"
                                + ", InDate, InUser, WhNoIn"
                                + ",ExpressNo, ChkUser, ChkDate, "
                                + "PostUser, PostDate, DocType"
                                + ", CkNoOut, CkNoIn, WorkType) Values ";
                            sql += " ('" + uu.CompanyId.SqlQuote() + "', '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                + ", '" + NewDocNo + "', convert(char(10),getdate(),111), '" + NewWareDSVIn + "', '" + uu.UserID + "'"
                                + ", '" + SysDate + "','" + uu.UserID + "','" + WhNoIn + "'"
                                + ", '" + DocNo + "', '" + uu.UserID + "', convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5)"
                                + ", '" + uu.UserID + "', convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5),'V' "
                                + ",'" + CkNoIn + "', '" + sCkNo + "', 'IS') ";
                            dbop.ExecuteSql(sql, uu, "SYS");



                            ////變更庫存數量及庫存增減日
                            ////調出方
                            //string sqlTROut = "";
                            //sqlTROut = "Select H.CompanyCode, H.TH_ID, H.WhNoOut, H.CkNoOut, D.LayerOut, D.SnoOut, D.SeqNo, D.PLU, D.OutNum "
                            //    + " From TransferHSV H Inner Join TransferDSV D "
                            //    + " On H.CompanyCode = D.CompanyCode And H.TH_ID = D.TH_ID "
                            //    + " Where H.CompanyCode='" + uu.CompanyId + "' And H.TH_ID='" + DocNo + "' ";


                            //    //寫入調出方jahoInvSV
                            //    sql = "Insert Into JahoInvSV (CompanyCode, CrtUser, CrtDate, CrtTime "
                            //        + ", ModUser, ModDate, ModTime "
                            //        + ", DocType, DocNo, WhNo, SeqNo, PLU, Q1, Q2, Q3, CkNo, Layer, Sno) ";
                            //    sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                            //         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                            //         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                            //         + ", 'TF', TH_ID, WhNoOut, SeqNo, a.PLU, IsNull(b.PtNum,0), -1*OutNum, IsNull(b.PtNum,0)-OutNum"
                            //         + ", CkNoOut, LayerOut, SnoOut "
                            //         + " From (" + sqlTROut + ") a Left Join InventorySV b "
                            //         + " On a.CompanyCode = b.CompanyCode And a.WhNoOut = b.WhNo and a.CkNoOut = b.CkNo "
                            //         + " and a.LayerOut=b.Layer And a.SnoOut=b.Sno And a.PLU=b.PLU ";
                            //    dbop.ExecuteSql(sql, uu, "SYS");

                            //    //已有庫存資料
                            //    sql = "Update InventorySV "
                            //        + "Set ModUser='" + uu.UserID + "' "
                            //        + ",ModDate=convert(char(10),getdate(),111) "
                            //        + ",ModTime=convert(char(8),getdate(),108) "
                            //        + ",PtNum=IsNull(PtNum,0) - OutNum "
                            //        + ",Out_Date = Case When Out_Date>'" + SysDate + "' Then Out_Date Else '" + SysDate + "' End "
                            //        + "From (" + sqlTROut + ") a Inner Join InventorySV b "
                            //        + "On a.CompanyCode=b.CompanyCode and a.WhNoOut=b.WhNo and a.CkNoOut=b.CkNo "
                            //        + "and a.LayerOut=b.Layer And a.SnoOut=b.Sno And a.PLU=b.PLU ";
                            //    sql += " Where b.CompanyCode='" + uu.CompanyId + "' ";
                            //    dbop.ExecuteSql(sql, uu, "SYS");

                            //    //沒有庫存資料-新增
                            //    sql = "Insert Into InventorySV (CompanyCode, CrtUser, CrtDate, CrtTime "
                            //        + ", ModUser, ModDate, ModTime "
                            //        + ", WhNo, a.PLU, Out_Date, CkNo, Layer, Sno, PtNum) ";
                            //    sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                            //         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                            //         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                            //         + ", WhNoOut, PLU, '" + SysDate + "', CkNoOut, LayerOut, SnoOut, -1*OutNum "
                            //         + " From (" + sqlTROut + ") a Left Join InventorySV b "
                            //         + " On a.CompanyCode = b.CompanyCode and a.WhNoOut = b.WhNo and a.CkNoOut = b.CkNo "
                            //         + "and a.LayerOut=b.Layer And a.SnoOut=b.Sno And a.PLU=b.PLU "
                            //         + "Where b.PLU Is Null ";
                            //    dbop.ExecuteSql(sql, uu, "SYS");





                            ////調入方
                            //string sqlTRIn = "";
                            //sqlTRIn = "Select H.CompanyCode, H.TH_ID, H.WhNoIn, H.CkNoIn, D.LayerIn, D.SnoIn, D.SeqNo, D.PLU, D.InNum "
                            //    + " From TransferHSV H Inner Join TransferDSV D "
                            //    + " On H.CompanyCode = D.CompanyCode And H.TH_ID = D.TH_ID "
                            //    + " Where H.CompanyCode='" + uu.CompanyId + "' And H.TH_ID='" + DocNo + "' ";
                            ////寫入調入方jahoInvSV
                            //sql = "Insert Into JahoInvSV (CompanyCode, CrtUser, CrtDate, CrtTime "
                            //    + ", ModUser, ModDate, ModTime "
                            //    + ", DocType, DocNo, WhNo, SeqNo, PLU, Q1, Q2, Q3, CkNo, Layer, Sno) ";
                            //sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                            //     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                            //     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                            //     + ", 'TF', TH_ID, WhNoIn, SeqNo, a.PLU, IsNull(b.PtNum,0), InNum, IsNull(b.PtNum,0)+InNum"
                            //     + ", CkNoIn, LayerIn, SnoIn "
                            //     + " From (" + sqlTRIn + ") a Left Join InventorySV b "
                            //     + " On a.CompanyCode = b.CompanyCode And a.WhNoIn = b.WhNo and a.CkNoIn = b.CkNo "
                            //     + " and a.LayerIn = b.Layer And a.SnoIn = b.Sno And a.PLU=b.PLU ";
                            //dbop.ExecuteSql(sql, uu, "SYS");

                            ////已有庫存資料
                            //sql = "Update InventorySV "
                            //    + "Set ModUser='" + uu.UserID + "' "
                            //    + ",ModDate=convert(char(10),getdate(),111) "
                            //    + ",ModTime=convert(char(8),getdate(),108) "
                            //    + ",PtNum=IsNull(PtNum,0) + InNum "
                            //    + ",In_Date = Case When In_Date>'" + SysDate + "' Then In_Date Else '" + SysDate + "' End "
                            //    + "From (" + sqlTRIn + ") a Inner Join InventorySV b "
                            //    + "On a.CompanyCode=b.CompanyCode and a.WhNoIn=b.WhNo and a.CkNoIn=b.CkNo And a.PLU=b.PLU "
                            //    + "and a.LayerIn=b.Layer And a.SnoIn=b.Sno ";
                            //sql += " Where b.CompanyCode='" + uu.CompanyId + "' ";
                            //dbop.ExecuteSql(sql, uu, "SYS");

                            ////沒有庫存資料-新增
                            //sql = "Insert Into InventorySV (CompanyCode, CrtUser, CrtDate, CrtTime "
                            //    + ", ModUser, ModDate, ModTime"
                            //    + ", WhNo, PLU, In_Date, CkNo, Layer, Sno, PtNum) ";
                            //sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                            //     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                            //     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                            //     + ", WhNoIn, a.PLU, '" + SysDate + "', CkNoIn, LayerIn, SnoIn, InNum "
                            //     + " From (" + sqlTRIn + ") a Left Join InventorySV b "
                            //     + " On a.CompanyCode = b.CompanyCode and a.WhNoIn = b.WhNo and a.CkNoIn = b.CkNo "
                            //     + " and a.LayerIn=b.Layer And a.SnoIn=b.Sno And a.PLU=b.PLU "
                            //     + "Where b.PLU Is Null ";
                            //dbop.ExecuteSql(sql, uu, "SYS");

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
                sql += " ,c.ST_OpenDay";
                sql += " from ChangeShopSV a";
                sql += " inner join WarehouseSV b on a.WhNoOut=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " inner join WarehouseSV c on a.WhNoIn=c.ST_ID And a.CompanyCode=c.CompanyCode";
                sql += " left  join EmployeeSV d on a.DocUser=d.Man_ID And a.CompanyCode=d.CompanyCode";
                sql += " where a.CompanyCode='" + uu.CompanyId + "'";
                sql += " And a.DocNo='" + DocNo + "'";
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



        [Route("SystemSetup/SearchVIN14_5")]
        public ActionResult SystemSetup_SearchVIN14_5()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchVIN14_5OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;

                string WhNo = rq["WhNo"];
                string CkNo = rq["CkNo"];
                string Layer = rq["Layer"];
                string exDate = rq["exDate"];

                string sql = "select a.*, a.WhNo+b.ST_SNAME WhName, IsNull(c.st_StopDay,'') ST_StopDay , d.GD_SName OldName, e.GD_SName NewName, a.Layer+a.Sno LayerSno , b.ST_OpenDay , f.PtNum , g.FlagUse, e.GD_Flag1 ";
                sql += " ,Case When IsNull(a.AppDate,'')='' Then '未批核' Else '已批核' End AppStatus";
                sql += " ,Case When IsNull(a.FinishDate,'')='' Then (Case When IsNull(DefeasanceDate,'')<>'' Then '作廢' Else '未完成' End) Else '完成' End FinStatus";
                sql += " from ChangePLUSV a (Nolock) ";
                sql += " inner join WarehouseSV b (Nolock) on a.WhNo=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " left join WarehouseDSV c (Nolock) on a.CompanyCode=c.CompanyCode And a.WhNo=c.ST_ID And a.CkNo=c.CkNo ";
                sql += " left join PLUSV d (Nolock) on a.PLUOld=d.GD_No And a.CompanyCode=d.CompanyCode  ";
                sql += " left join PLUSV e (Nolock) on a.PLUNew=e.GD_No And a.CompanyCode=e.CompanyCode ";
                sql += " left join InventorySV f (Nolock) " 
                    +  " on a.WhNo=f.WhNo And a.CkNo=f.CkNo And a.Layer=f.Layer And a.Sno=f.Sno And a.CompanyCode=f.CompanyCode ";
                sql += " left join MachineList g (Nolock) on c.CompanyCode=g.CompanyCode And c.SNno=g.SNno  ";
                sql += " where a.CompanyCode='" + uu.CompanyId + "' ";
                sql += " And IsNull(a.FinishDate,'')='' And IsNull(a.DefeasanceDate,'')='' And IsNull(a.AppDate,'')<>'' ";
                if (WhNo != "")
                {
                    sql += " and a.WhNo='" + WhNo + "'";
                }
                if (CkNo != "")
                {
                    sql += " and a.CkNo='" + CkNo + "'";
                }
                if (Layer != "")
                {
                    sql += " and a.Layer='" + Layer + "'";
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


        //2021-06-21 Larry
        [Route("SystemSetup/SaveVIN14_5")]
        public ActionResult SystemSetup_SaveVIN14_5()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveVIN14_5OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                DataTable dtRec = new DataTable("ChangePLUSV");
                PubUtility.AddStringColumns(dtRec, "DocNo,WhNo,CkNo,Layer,Sno,OldPLU,PtNum,NewPLU,Num,ExpDate");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtRec);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtRec.Rows[0];

                string BackDocNo = PubUtility.GetNewDocNo(uu, "TH", 6);
                string InDocNo = PubUtility.GetNewDocNo(uu, "TH", 6);

                string sql = "";
                string WhNoDsv = "";
                string SysDate = "";
                using (DBOperator dbop = new DBOperator())
                {


                    int Q3;
                    sql = "Select IsNull(b.PtNum,0)-" + Convert.ToInt32(dr["PtNum"].ToString()) + " Q3 From ChangePLUSV a (Nolock) ";
                    sql += "Left Join InventorySV b (Nolock) "
                        + "On a.CompanyCode=b.CompanyCode And a.PLUOld=b.PLU "
                        + "And a.WhNo=b.WhNo And a.CkNo=b.CkNo And a.Layer=b.Layer And a.Sno=b.Sno "
                        + "Where a.CompanyCode='" + uu.CompanyId + "' "
                        + "And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
                    DataTable dtQ3 = PubUtility.SqlQry(sql, uu, "SYS");

                    if (dtQ3.Rows.Count > 0)
                    {
                        Q3 = Convert.ToInt32(dtQ3.Rows[0][0]);
                    }
                    else { Q3 = 0; }


                    int maxSno;
                    sql = "Select Isnull(MAX(Sno),0) MaxSno From InventorySV "
                        + "Where CompanyCode='" + uu.CompanyId + "' "
                        + "And WhNo='" + dr["WhNo"].ToString().SqlQuote() + "' And CkNo='" + dr["CkNo"].ToString().SqlQuote() + "' "
                        + "And Layer='' ";
                    DataTable dtSno = PubUtility.SqlQry(sql, uu, "SYS");

                    if (dtSno.Rows.Count > 0)
                    {
                        maxSno = Convert.ToInt32(dtSno.Rows[0][0]) + 1;
                    }
                    else { 
                        maxSno = 1; 
                    }




                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            sql = "select convert(char(10),getdate(),111) SysDate";

                            DataTable dtSysDate = PubUtility.SqlQry(sql, uu, "SYS");
                            if (dtSysDate.Rows.Count > 0)
                            {
                                SysDate = dtSysDate.Rows[0][0].ToString();
                            }

                            sql = "Select WhNoIn From WarehouseDSV "
                                + "Where CompanyCode='" + uu.CompanyId + "' "
                                + "And ST_ID='" + dr["WhNo"].ToString().SqlQuote() + "' "
                                + "And CkNo='" + dr["CkNo"].ToString().SqlQuote() + "' ";
                            DataTable dtWhNoDsv = PubUtility.SqlQry(sql, uu, "SYS");

                            if (dtWhNoDsv.Rows.Count > 0)
                            {
                                WhNoDsv = dtWhNoDsv.Rows[0][0].ToString();
                            }

                            bool bSameWh = false;string CkNoDsv = ""; string LayerDsv = "";
                            if (WhNoDsv == dr["WhNo"].ToString())
                            {
                                bSameWh = true; CkNoDsv = "00"; LayerDsv = "Z";
                            }
                            else
                            {
                                bSameWh = false; CkNoDsv = "XX"; LayerDsv = "";
                            }




                            //寫入調撥資料：退貨、補貨各一張單

                            //#####退貨 調撥表頭
                            sql = "Insert Into TransferHSV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                + ",ModUser, ModDate, ModTime "
                                + ",TH_ID, DocDate, WhNoOut, OutUser"
                                + ", InDate, InUser, WhNoIn"
                                + ",ExpressNo, ChkUser, ChkDate, "
                                + "PostUser, PostDate, DocType"
                                + ", CkNoOut, CkNoIn, WorkType) Values ";
                            sql += " ('" + uu.CompanyId.SqlQuote() + "', '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                + ", '" + BackDocNo + "', convert(char(10),getdate(),111), '" + dr["WhNo"].ToString().SqlQuote() + "', '" + uu.UserID + "'"
                                + ", convert(char(10),getdate(),111),'" + uu.UserID + "','" + WhNoDsv + "'"
                                + ", '" + dr["DocNo"].ToString().SqlQuote() + "', '" + uu.UserID + "', convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5)"
                                + ", '" + uu.UserID + "', convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5),'V' "
                                + ",'" + dr["CkNo"].ToString().SqlQuote() + "', '" + CkNoDsv + "', 'IG')";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //退貨 調撥表身
                            sql = "Insert Into TransferDSV (CompanyCode, CrtUser, CrtDate, CrtTime, ModUser, ModDate, ModTime, " +
                                "TH_ID, SeqNo, PLU, OutNum, InNum, " +
                                "GD_Retail, Amt, LayerIn, SnoIn, LayerOut, SnoOut) ";
                            sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", '" + BackDocNo + "', 1, a.PLUOld, " + dr["PtNum"].ToString().SqlQuote() + ", " + dr["PtNum"].ToString().SqlQuote() + " "
                                 + ", b.GD_Retail, " + dr["PtNum"].ToString().SqlQuote() + "*b.GD_Retail, '" + LayerDsv + "' ";
                            if (LayerDsv == "Z")
                            {
                                sql += ", d.Sno ";
                            }
                            else
                            {
                                sql += ", '' ";
                            }
                            sql += " , a.Layer, a.Sno ";
                            sql += " From ChangePLUSV a (Nolock) ";
                            sql += " Inner Join PLUSV b (Nolock) On a.CompanyCode=b.CompanyCode And a.PLUOld=b.GD_No ";
                            //sql += " Inner Join WarehouseDSV c (Nolock) On a.CompanyCode=c.CompanyCode And a.PLU=c.GD_No ";
                            sql += " Left Join InventorySV d (Nolock) On a.CompanyCode=d.CompanyCode And a.PLUOld=d.PLU "
                                + "And d.WhNo='" + WhNoDsv + "' And d.CkNo='" + CkNoDsv + "' And d.Layer='Z' ";

                            sql += " Where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
                            //sql += " And (a.Qty>0 or (a.Qty>=0 And IsNull(a.EffectiveDate,'')<>''))";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //變更庫存數量及庫存增減日
                            //調出方
                            string sqlTROut = "";
                            sqlTROut = "Select H.CompanyCode, H.TH_ID, H.WhNoOut, H.CkNoOut, D.LayerOut, D.SnoOut, D.SeqNo, D.PLU, D.OutNum, C.DisplayNum "
                                + " From TransferHSV H Inner Join TransferDSV D "
                                + " On H.CompanyCode = D.CompanyCode And H.TH_ID = D.TH_ID "
                                + " Inner Join ChangePLUSV C "
                                + " On H.CompanyCode = C.CompanyCode And H.WhNoOut = C.WhNo And H.CkNoOut=C.CkNo And D.LayerOut=C.Layer And D.SnoOut=C.Sno "
                                + " Where H.CompanyCode='" + uu.CompanyId + "' And H.TH_ID='" + BackDocNo + "' ";

                            //if (CkNoDsv != "XX")
                            //{
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

                            //##### 退貨調出 特殊邏輯 異動調出方InventorySV
                            //0、如調出方異動後數量=0，則刪除該筆 InventorySV 資料
                            //1、如調出方異動後數量<>0，則執行下面動作
                            //1.1、取得該 店、機、Layer=''的最大Sno
                            //1.2、如有取得資料，則以最大Sno+1回寫原商品的Sno
                            //1.3、如未取得資料，則以回寫原商品的Sno=

                            if (Q3 == 0)
                            {
                                sql = "Delete From InventorySV Where CompanyCode='" + uu.CompanyId + "' "
                                    + "And WhNo='" + dr["WhNo"].ToString().SqlQuote() + "' And CkNo='" + dr["CkNo"].ToString().SqlQuote() + "' "
                                    + "And Layer='" + dr["Layer"].ToString().SqlQuote() + "' And Sno='" + dr["Sno"].ToString().SqlQuote() + "' And PLU='" + dr["OldPLU"].ToString().SqlQuote() + "' ";
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }
                            else
                            {

                                //已有庫存資料
                                sql = "Update InventorySV "
                                    + "Set ModUser='" + uu.UserID + "' "
                                    + ",ModDate=convert(char(10),getdate(),111) "
                                    + ",ModTime=convert(char(8),getdate(),108) "
                                    + ",PtNum=IsNull(PtNum,0) - OutNum "
                                    + ",Layer='' ,LayerOld=Layer "
                                    + ",Sno='" + Convert.ToString(maxSno) + "' ,SnoOld=Sno "
                                    + ",Out_Date = '" + SysDate + "' "
                                    + "From (" + sqlTROut + ") a Inner Join InventorySV b "
                                    + "On a.CompanyCode=b.CompanyCode and a.WhNoOut=b.WhNo and a.CkNoOut=b.CkNo "
                                    + "and a.LayerOut=b.Layer And a.SnoOut=b.Sno And a.PLU=b.PLU ";
                                sql += " Where b.CompanyCode='" + uu.CompanyId + "' ";
                                dbop.ExecuteSql(sql, uu, "SYS");

                                //沒有庫存資料-新增
                                sql = "Insert Into InventorySV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                    + ", ModUser, ModDate, ModTime "
                                    + ", WhNo, a.PLU, Out_Date, CkNo, Layer, Sno, PtNum, SafeNum, DisplayNum) ";
                                sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", WhNoOut, a.PLU, '" + SysDate + "', CkNoOut, LayerOut, SnoOut, -1*OutNum, 1, DisplayNum "
                                     + " From (" + sqlTROut + ") a Left Join InventorySV b "
                                     + " On a.CompanyCode = b.CompanyCode and a.WhNoOut = b.WhNo and a.CkNoOut = b.CkNo "
                                     + "and a.LayerOut=b.Layer And a.SnoOut=b.Sno And a.PLU=b.PLU "
                                     + "Where b.PLU Is Null ";
                                dbop.ExecuteSql(sql, uu, "SYS");

                            }


                            string sqlTRIn = "";
                            //#####退貨單 調入方 要判別是否 調入機號 != "XX" 才作
                            if (CkNoDsv != "XX")
                            {

                                sqlTRIn = "Select H.CompanyCode, H.TH_ID, H.WhNoIn, H.CkNoIn, D.LayerIn, D.SnoIn, D.SeqNo, D.PLU, D.InNum, C.DisplayNum "
                                    + " From TransferHSV H Inner Join TransferDSV D "
                                    + " On H.CompanyCode = D.CompanyCode And H.TH_ID = D.TH_ID "
                                    + " Inner Join ChangePLUSV C "
                                    + " On H.CompanyCode = C.CompanyCode And H.WhNoIn = C.WhNo And H.CkNoIn=C.CkNo And D.LayerIn=C.Layer And D.SnoIn=C.Sno "
                                    + " Where H.CompanyCode='" + uu.CompanyId + "' And H.TH_ID='" + BackDocNo + "' ";

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

                                //異動調入方InventorySV
                                //已有庫存資料
                                sql = "Update InventorySV "
                                    + "Set ModUser='" + uu.UserID + "' "
                                    + ",ModDate=convert(char(10),getdate(),111) "
                                    + ",ModTime=convert(char(8),getdate(),108) "
                                    + ",PtNum=IsNull(PtNum,0) + InNum "
                                    + ",In_Date = '" + SysDate + "' "
                                    + ",EffectiveDate = a.EffectiveDate "
                                    + "From (" + sqlTRIn + ") a Inner Join InventorySV b "
                                    + "On a.CompanyCode=b.CompanyCode and a.WhNoIn=b.WhNo and a.CkNoIn=b.CkNo And a.PLU=b.PLU "
                                    + "and a.LayerIn=b.Layer And a.SnoIn=b.Sno ";
                                sql += " Where b.CompanyCode='" + uu.CompanyId + "' ";
                                dbop.ExecuteSql(sql, uu, "SYS");

                                //沒有庫存資料-新增
                                sql = "Insert Into InventorySV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                    + ", ModUser, ModDate, ModTime"
                                    + ", WhNo, PLU, In_Date, CkNo, Layer, Sno, PtNum, DisplayNum, SafeNum, EffectiveDate) ";
                                sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", WhNoIn, a.PLU, '" + SysDate + "', CkNoIn, LayerIn, SnoIn, InNum, a.DisplayNum, 1, '" + dr["ExpDate"].ToString().SqlQuote() + "' "
                                     + " From (" + sqlTRIn + ") a Left Join InventorySV b "
                                     + " On a.CompanyCode = b.CompanyCode and a.WhNoIn = b.WhNo and a.CkNoIn = b.CkNo "
                                     + " and a.LayerIn=b.Layer And a.SnoIn=b.Sno And a.PLU=b.PLU "
                                     + "Where b.PLU Is Null ";
                                dbop.ExecuteSql(sql, uu, "SYS");

                            }








                            //#####補貨 調撥表頭
                            sql = "Insert Into TransferHSV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                + ",ModUser, ModDate, ModTime "
                                + ",TH_ID, DocDate, WhNoOut, OutUser"
                                + ", InDate, InUser, WhNoIn"
                                + ",ExpressNo, ChkUser, ChkDate, "
                                + "PostUser, PostDate, DocType"
                                + ", CkNoOut, CkNoIn, WorkType) Values ";
                            sql += " ('" + uu.CompanyId.SqlQuote() + "', '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                + ", '" + InDocNo + "', convert(char(10),getdate(),111), '" + WhNoDsv + "', '" + uu.UserID + "'"
                                + ", convert(char(10),getdate(),111),'" + uu.UserID + "','" + dr["WhNo"].ToString().SqlQuote() + "'"
                                + ", '" + dr["DocNo"].ToString().SqlQuote() + "', '" + uu.UserID + "', convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5)"
                                + ", '" + uu.UserID + "', convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5),'V' "
                                + ",'" + CkNoDsv + "', '" + dr["CkNo"].ToString().SqlQuote() + "', 'IG')";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //補貨 調撥表身
                            sql = "Insert Into TransferDSV (CompanyCode, CrtUser, CrtDate, CrtTime, ModUser, ModDate, ModTime, " +
                                "TH_ID, SeqNo, PLU, OutNum, InNum, " +
                                "GD_Retail, Amt, LayerIn, SnoIn, LayerOut, SnoOut, EffectiveDate) ";
                            sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", '" + InDocNo + "', 1, a.PLUNew, " + dr["Num"].ToString().SqlQuote() + ", " + dr["Num"].ToString().SqlQuote() + " "
                                 + ", b.GD_Retail, " + dr["Num"].ToString().SqlQuote() + "*b.GD_Retail, a.Layer, a.Sno, '" + LayerDsv + "' ";
                            if (LayerDsv == "Z")
                            {
                                sql += ", d.Sno ";
                            }
                            else
                            {
                                sql += ", '' ";
                            }
                            sql += " , '" + dr["ExpDate"].ToString().SqlQuote() + "' ";
                            sql += " From ChangePLUSV a (Nolock) ";
                            sql += " Inner Join PLUSV b (Nolock) On a.CompanyCode=b.CompanyCode And a.PLUNew=b.GD_No ";
                            //sql += " Inner Join WarehouseDSV c (Nolock) On a.CompanyCode=c.CompanyCode And a.PLU=c.GD_No ";
                            sql += " Left Join InventorySV d (Nolock) On a.CompanyCode=d.CompanyCode And a.PLUNew=d.PLU "
                                + "And d.WhNo='" + WhNoDsv + "' And d.CkNo='" + CkNoDsv + "' And d.Layer='Z' ";

                            sql += " Where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
                            //sql += " And (a.Qty>0 or (a.Qty>=0 And IsNull(a.EffectiveDate,'')<>''))";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //補貨 變更庫存數量及庫存增減日
                            sqlTROut = "Select H.CompanyCode, H.TH_ID, H.WhNoOut, H.CkNoOut, D.LayerOut, D.SnoOut, D.SeqNo, D.PLU, D.OutNum, C.DisplayNum "
                                + " From TransferHSV H Inner Join TransferDSV D "
                                + " On H.CompanyCode = D.CompanyCode And H.TH_ID = D.TH_ID "
                                + " Inner Join ChangePLUSV C "
                                + " On H.CompanyCode = C.CompanyCode And H.WhNoOut = C.WhNo And H.CkNoOut=C.CkNo And D.LayerOut=C.Layer And D.SnoOut=C.Sno "
                                + " Where H.CompanyCode='" + uu.CompanyId + "' And H.TH_ID='" + InDocNo + "' ";

                            // 補貨 調出
                            if (CkNoDsv != "XX")
                            {
                                //補貨 寫入調出方jahoInvSV
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

                                //##### 退貨調出 
                                //已有庫存資料
                                sql = "Update InventorySV "
                                    + "Set ModUser='" + uu.UserID + "' "
                                    + ",ModDate=convert(char(10),getdate(),111) "
                                    + ",ModTime=convert(char(8),getdate(),108) "
                                    + ",PtNum=IsNull(PtNum,0) - OutNum "
                                    + ",Layer='' ,LayerOld=Layer "
                                    + ",Sno='" + Convert.ToString(maxSno) + "' ,SnoOld=Sno "
                                    + ",Out_Date = '" + SysDate + "' "
                                    + ",EffectiveDate = a.EffectiveDate "
                                    + "From (" + sqlTROut + ") a Inner Join InventorySV b "
                                    + "On a.CompanyCode=b.CompanyCode and a.WhNoOut=b.WhNo and a.CkNoOut=b.CkNo "
                                    + "and a.LayerOut=b.Layer And a.SnoOut=b.Sno And a.PLU=b.PLU ";
                                sql += " Where b.CompanyCode='" + uu.CompanyId + "' ";
                                dbop.ExecuteSql(sql, uu, "SYS");

                                //沒有庫存資料-新增
                                sql = "Insert Into InventorySV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                    + ", ModUser, ModDate, ModTime "
                                    + ", WhNo, a.PLU, Out_Date, CkNo, Layer, Sno, PtNum, SafeNum, DisplayNum) ";
                                sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", WhNoOut, a.PLU, '" + SysDate + "', CkNoOut, LayerOut, SnoOut, -1*OutNum, 1, DisplayNum "
                                     + " From (" + sqlTROut + ") a Left Join InventorySV b "
                                     + " On a.CompanyCode = b.CompanyCode and a.WhNoOut = b.WhNo and a.CkNoOut = b.CkNo "
                                     + "and a.LayerOut=b.Layer And a.SnoOut=b.Sno And a.PLU=b.PLU "
                                     + "Where b.PLU Is Null ";
                                dbop.ExecuteSql(sql, uu, "SYS");

                            }



                            //#####補貨單 
                            //if (CkNoDsv != "XX")
                            //{
                            sqlTRIn = "Select H.CompanyCode, H.TH_ID, H.WhNoIn, H.CkNoIn, D.LayerIn, D.SnoIn, D.SeqNo, D.PLU, D.InNum, C.DisplayNum "
                                + " From TransferHSV H Inner Join TransferDSV D "
                                + " On H.CompanyCode = D.CompanyCode And H.TH_ID = D.TH_ID "
                                + " Inner Join ChangePLUSV C "
                                + " On H.CompanyCode = C.CompanyCode And H.WhNoIn = C.WhNo And H.CkNoIn=C.CkNo And D.LayerIn=C.Layer And D.SnoIn=C.Sno "
                                + " Where H.CompanyCode='" + uu.CompanyId + "' And H.TH_ID='" + InDocNo + "' ";

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

                            //異動調入方InventorySV
                            //已有庫存資料
                            sql = "Update InventorySV "
                                + "Set ModUser='" + uu.UserID + "' "
                                + ",ModDate=convert(char(10),getdate(),111) "
                                + ",ModTime=convert(char(8),getdate(),108) "
                                + ",PtNum=IsNull(PtNum,0) + InNum "
                                + ",In_Date = '" + SysDate + "' "
                                + "From (" + sqlTRIn + ") a Inner Join InventorySV b "
                                + "On a.CompanyCode=b.CompanyCode and a.WhNoIn=b.WhNo and a.CkNoIn=b.CkNo And a.PLU=b.PLU "
                                + "and a.LayerIn=b.Layer And a.SnoIn=b.Sno ";
                            sql += " Where b.CompanyCode='" + uu.CompanyId + "' ";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //沒有庫存資料-新增
                            sql = "Insert Into InventorySV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                + ", ModUser, ModDate, ModTime"
                                + ", WhNo, PLU, In_Date, CkNo, Layer, Sno, PtNum, DisplayNum, SafeNum, EffectiveDate) ";
                            sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", WhNoIn, a.PLU, '" + SysDate + "', CkNoIn, LayerIn, SnoIn, InNum, a.DisplayNum, 1, '" + dr["ExpDate"].ToString().SqlQuote() + "' "
                                 + " From (" + sqlTRIn + ") a Left Join InventorySV b "
                                 + " On a.CompanyCode = b.CompanyCode and a.WhNoIn = b.WhNo and a.CkNoIn = b.CkNo "
                                 + " and a.LayerIn=b.Layer And a.SnoIn=b.Sno And a.PLU=b.PLU "
                                 + "Where b.PLU Is Null ";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //}


                            sql = "Update ChangePLUSV Set FinishUser='" + uu.UserID + "' , "
                                + "FinishDate=convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5) "
                                + ",ModDate=convert(char(10),getdate(),111) ,ModTime=convert(char(8),getdate(),108) "
                                + "Where CompanyCode='" + uu.CompanyId.SqlQuote() + "' And DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
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

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }




        [Route("SystemSetup/GetInitVIN47")]
        public ActionResult SystemSetup_GetInitVIN47()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitVIN47OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "select ST_ID,ST_ID+ST_SName ST_SName";
                sql += " from WarehouseSV ";
                sql += " where CompanyCode='" + uu.CompanyId + "' And ST_Type='6' Order By ST_ID";
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



        [Route("SystemSetup/SearchVIN47")]
        public ActionResult SystemSetup_SearchVIN47()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchVIN47OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            using (DBOperator dbop = new DBOperator())
                try
                {
                    IFormCollection rq = HttpContext.Request.Form;
                    string WhNo = rq["WhNo"];
                    string CkNo = rq["CkNo"];
                    string LayerNo = rq["LayerNo"];

                    string DocNo = PubUtility.GetNewDocNo(uu, "VC", 6);

                    string sql = "Insert Into TempDocumentSV ";
                    sql += "(CompanyCode,CrtUser,CrtDate,CrtTime,DocNo,SeqNo,PLU,Qty";
                    sql += ",Qty2,RNum, WhNo, CkNo, Layer, Sno, EffectiveDate, WorkType)";
                    sql += "select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)";
                    sql += " ,'" + DocNo + "', Cast(Row_Number() Over(Order By a.Layer,a.Sno) As int), a.PLU, 0, "
                            + "0, 0, a.WhNo, a.CkNo, a.Layer, a.Sno, EffectiveDate, 'IA' ";
                    sql += " from InventorySV a (Nolock) ";
                    //sql += " inner join PLUSV b (Nolock) on a.CompanyCode=b.CompanyCode And a.PLU=b.GD_NO";
                    sql += " where a.CompanyCode='" + uu.CompanyId + "' ";
                    //sql += "And b.GD_Flag1='1' ";

                    if (WhNo != "" & WhNo != null)
                    {
                        sql += " and a.WhNo='" + WhNo + "'";
                    }
                    if (CkNo != "" & CkNo != null)
                    {
                        sql += " and a.CkNo='" + CkNo + "'";
                    }
                    if (LayerNo != "" & LayerNo != null)
                    {
                        sql += " and a.Layer='" + LayerNo + "'";
                    }
                    dbop.ExecuteSql(sql, uu, "SYS");

                    sql = "select a.*,a.Layer+a.Sno Channel,c.GD_SName,c.Photo1 ";
                    sql += " , Cast(b.PtNum As VarChar(5))+'/'+Cast(b.DisplayNum As VarChar(5)) ShowQty, b.DisplayNum-b.PtNum ShortQty, d.ST_SName, b.DisplayNum, b.PtNum ";
                    sql += " from tempdocumentsv a (Nolock) ";
                    sql += " inner join InventorySV b (Nolock) on a.WhNo=b.WhNo and a.CkNo=b.CkNo And a.Layer=b.Layer And a.Sno=b.Sno and a.CompanyCode=b.CompanyCode";
                    sql += " inner join PLUSV c (Nolock) on a.PLU=c.GD_NO and a.CompanyCode=c.CompanyCode";
                    sql += " inner join WarehouseSV d (Nolock) on a.WhNo=d.ST_ID and a.CompanyCode=d.CompanyCode";
                    sql += " where a.CompanyCode='" + uu.CompanyId + "' ";
                    sql += " and a.DocNo='" + DocNo + "' ";
                    sql += " Order By a.SeqNo ";

                    DataTable dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                    dtPLU.TableName = "dtPLU";
                    ds.Tables.Add(dtPLU);

                    DataTable dtDocNo = new DataTable("dtDocNo");
                    dtDocNo.Columns.Add("DocNo", typeof(string));

                    DataRow dr = dtDocNo.NewRow();
                    dr["DocNo"] = DocNo;

                    dtDocNo.Rows.Add(dr);
                    ds.Tables.Add(dtDocNo);

                }
                catch (Exception err)
                {
                    dtMessage.Rows[0][0] = "Exception";
                    dtMessage.Rows[0][1] = err.Message;
                }
            return PubUtility.DatasetXML(ds);
        }


        //2021-06-21 Larry
        [Route("SystemSetup/SaveVIN47")]
        public ActionResult SystemSetup_SaveVIN47()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveVIN47OK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                DataTable dtTemp = new DataTable("TempDocumentSV");
                PubUtility.AddStringColumns(dtTemp, "DocNo,WhNo,CkNo");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtTemp);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtTemp.Rows[0];

                string DocNo = PubUtility.GetNewDocNo(uu, "AD", 3);

                string sql = "";
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

                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            //處理只異動日期的資料
                            string sqlAdj = "";
                            //異動庫存最近有效日期
                            sqlAdj = "Select * From TempDocumentSV a (Nolock) ";
                            sqlAdj += " Where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
                            sqlAdj += " And IsNull(RNum,0) = 0 And IsNull(EffectiveDate,'')<>''" ;
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
                                //寫入調整資料
                                //調整表頭
                                sql = "Insert Into AdjustHSV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                    + ", ModUser, ModDate, ModTime "
                                    + ", ADocNo, DocDate, Man_ID "
                                    + ", WhNo, CkNo, DocType"
                                    + ", ChkUser, ChkDate"
                                    + ", PostUser, PostDate) Values ";
                                sql += " ('" + uu.CompanyId.SqlQuote() + "', '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                    + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                    + ", '" + DocNo + "', convert(char(10),getdate(),111), '" + uu.UserID + "'"
                                    + ", '" + dr["WhNo"].ToString().SqlQuote() + "', '" + dr["CkNo"].ToString().SqlQuote() + "', '0'"
                                    + ", '" + uu.UserID + "', convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5)"
                                    + ", '" + uu.UserID + "', convert(char(10),getdate(),111)+ ' ' +Substring(convert(char(8),getdate(),108),1,5))";
                                dbop.ExecuteSql(sql, uu, "SYS");

                                //調整表身
                                sql = "Insert Into AdjustDSV (CompanyCode, CrtUser, CrtDate, CrtTime"
                                    + ", ModUser, ModDate, ModTime"
                                    + ", ADocNo, SeqNo, PLU, Qty "
                                    + ", Layer, Sno, EffectiveDate, BefQty, AfterQty) ";
                                sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", '" + DocNo + "', Cast(Row_Number() Over(Order By a.Layer,a.Sno) As int), a.PLU, RNum "
                                     + ", a.Layer, a.Sno, a.EffectiveDate, a.Qty, a.Qty2 ";
                                sql += " From TempDocumentSV a (Nolock) ";
                                //sql += " Inner Join PLUSV b (Nolock) On a.CompanyCode=b.CompanyCode And a.PLU=b.GD_No ";
                                //sql += " Inner Join WarehouseDSV c (Nolock) On a.CompanyCode=c.CompanyCode And a.PLU=c.GD_No ";
                                sql += " Left Join InventorySV d (Nolock) On a.CompanyCode=d.CompanyCode And a.PLU=d.PLU "
                                    + "And d.WhNo='" + dr["WhNo"].ToString().SqlQuote() + "' And d.CkNo='" + dr["CkNo"].ToString().SqlQuote() + "' ";

                                sql += " Where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
                                //sql += " And a.Qty<>0 ";
                                sql += " And IsNull(a.RNum,0) <> 0 ";
                                sql += " And IsNull(a.ModDate,'')<>'' ";
                                //sql += " And (a.Qty>0 or (a.Qty>=0 And IsNull(a.EffectiveDate,'')<>''))";
                                dbop.ExecuteSql(sql, uu, "SYS");


                                //#####變更庫存數量及庫存增減日
                                sqlAdj = "Select H.CompanyCode, H.ADocNo, H.WhNo, H.CkNo, D.Layer, D.Sno, D.SeqNo, D.PLU, D.Qty "
                                    + " From AdjustHSV H Inner Join AdjustDSV D "
                                    + " On H.CompanyCode = D.CompanyCode And H.ADocNo = D.ADocNo "
                                    + " Where H.CompanyCode='" + uu.CompanyId + "' And H.ADocNo='" + DocNo + "' ";
                                //寫入調整jahoInvSV
                                sql = "Insert Into JahoInvSV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                    + ", ModUser, ModDate, ModTime "
                                    + ", DocType, DocNo, WhNo, SeqNo, PLU, Q1, Q2, Q3, CkNo, Layer, Sno) ";
                                sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", 'TF', ADocNo, a.WhNo, SeqNo, a.PLU, IsNull(b.PtNum,0), a.Qty, IsNull(b.PtNum,0)+a.Qty"
                                     + ", a.CkNo, a.Layer, a.Sno "
                                     + " From (" + sqlAdj + ") a Left Join InventorySV b "
                                     + " On a.CompanyCode = b.CompanyCode And a.WhNo = b.WhNo and a.CkNo = b.CkNo "
                                     + " and a.Layer = b.Layer And a.Sno = b.Sno And a.PLU=b.PLU ";
                                dbop.ExecuteSql(sql, uu, "SYS");

                                //已有庫存資料
                                sql = "Update InventorySV "
                                    + "Set ModUser='" + uu.UserID + "' "
                                    + ",ModDate=convert(char(10),getdate(),111) "
                                    + ",ModTime=convert(char(8),getdate(),108) "
                                    + ",PtNum=IsNull(PtNum,0) + Qty "
                                    + ",In_Date = Case When In_Date>'" + SysDate + "' Then In_Date Else '" + SysDate + "' End "
                                    + "From (" + sqlAdj + ") a Inner Join InventorySV b "
                                    + "On a.CompanyCode=b.CompanyCode and a.WhNo=b.WhNo and a.CkNo=b.CkNo And a.PLU=b.PLU "
                                    + "and a.Layer=b.Layer And a.Sno=b.Sno ";
                                sql += " Where b.CompanyCode='" + uu.CompanyId + "' ";
                                dbop.ExecuteSql(sql, uu, "SYS");

                                //沒有庫存資料-新增
                                sql = "Insert Into InventorySV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                    + ", ModUser, ModDate, ModTime"
                                    + ", WhNo, PLU, CkNo, Layer, Sno, "
                                    + "In_Date, Out_Date) ";
                                sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                     + ", a.WhNo, a.PLU, a.CkNo, a.Layer, a.Sno"
                                     + ", Case When a.Qty>0 Then '" + SysDate + "' Else '' End "
                                     + ", Case When a.Qty<0 Then '" + SysDate + "' Else '' End "
                                     + " From (" + sqlAdj + ") a Left Join InventorySV b "
                                     + " On a.CompanyCode = b.CompanyCode and a.WhNo = b.WhNo and a.CkNo = b.CkNo "
                                     + " and a.Layer=b.Layer And a.Sno=b.Sno And a.PLU=b.PLU "
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

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("SystemSetup/SearchVIN47Saved")]
        public ActionResult SystemSetup_SearchVIN47Saved()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchVIN47SavedOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            using (DBOperator dbop = new DBOperator())
                try
                {
                    IFormCollection rq = HttpContext.Request.Form;
                    string DocNo = rq["DocNo"];

                    string sql = "select a.*,a.Layer+a.Sno Channel,c.GD_SName,c.Photo1 ";
                    sql += " , Cast(b.PtNum As VarChar(5))+'/'+Cast(b.DisplayNum As VarChar(5)) ShowQty, b.DisplayNum-b.PtNum ShortQty, d.ST_SName, b.DisplayNum, b.PtNum ";
                    sql += " from tempdocumentsv a (Nolock) ";
                    sql += " inner join InventorySV b (Nolock) on a.WhNo=b.WhNo and a.CkNo=b.CkNo And a.Layer=b.Layer And a.Sno=b.Sno and a.CompanyCode=b.CompanyCode";
                    sql += " inner join PLUSV c (Nolock) on a.PLU=c.GD_NO and a.CompanyCode=c.CompanyCode";
                    sql += " inner join WarehouseSV d (Nolock) on a.WhNo=d.ST_ID and a.CompanyCode=d.CompanyCode";
                    sql += " where a.CompanyCode='" + uu.CompanyId + "' ";
                    sql += " and a.DocNo='" + DocNo + "' ";
                    sql += " Order By a.SeqNo ";

                    DataTable dtPLU = PubUtility.SqlQry(sql, uu, "SYS");
                    dtPLU.TableName = "dtPLU";
                    ds.Tables.Add(dtPLU);

                    sql = "Delete From TempDocumentSV Where CompanyCode='" + uu.CompanyId + "' and DocNo='" + DocNo + "' ";
                    dbop.ExecuteSql(sql, uu, "SYS");

                }
                catch (Exception err)
                {
                    dtMessage.Rows[0][0] = "Exception";
                    dtMessage.Rows[0][1] = err.Message;
                }
            return PubUtility.DatasetXML(ds);
        }














    }
}
