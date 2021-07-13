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

                string sql = "select a.*, a.WhNo+b.ST_SNAME WhName, c.Man_Name , d.GD_SName OldName, e.GD_SName NewName, a.Layer+a.Sno LayerSno ,b.ST_OpenDay, f.PtNum ";
                sql += " ,Case When IsNull(a.AppDate,'')='' Then '未批核' Else '已批核' End AppStatus";
                sql += " ,Case When IsNull(a.FinishDate,'')='' Then (Case When IsNull(DefeasanceDate,'')<>'' Then '作廢' Else '未完成' End) Else '完成' End FinStatus";
                sql += " from ChangePLUSV a (Nolock) ";
                sql += " inner join WarehouseSV b (Nolock) on a.WhNo=b.ST_ID And a.CompanyCode=b.CompanyCode";
                sql += " left join EmployeeSV c (Nolock) on a.DocUser=c.Man_ID And a.CompanyCode=c.CompanyCode ";
                sql += " left join PLUSV d (Nolock) on a.PLUOld=d.GD_No And a.CompanyCode=d.CompanyCode  ";
                sql += " left join PLUSV e (Nolock) on a.PLUNew=e.GD_No And a.CompanyCode=e.CompanyCode ";
                sql += " left join InventorySV f (Nolock) " 
                    +  " on a.WhNo=f.WhNo And a.CkNo=f.CkNo And a.Layer=f.Layer And a.Sno=f.Sno And a.CompanyCode=f.CompanyCode ";
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
                    sql += "(CompanyCode,ModUser,ModDate,ModTime,DocNo,SeqNo,PLU,Qty";
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
        public ActionResult SystemSetup_SaveInv()
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
                                sql += " And a.Qty>0 ";
                                //sql += " And (a.Qty>0 or (a.Qty>=0 And IsNull(a.EffectiveDate,'')<>''))";
                                dbop.ExecuteSql(sql, uu, "SYS");

                                //變更庫存數量及庫存增減日
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
