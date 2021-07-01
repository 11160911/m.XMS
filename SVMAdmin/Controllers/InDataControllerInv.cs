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


        private string GetNewDocNo(UserInfo uu, String DocType, Int16 Digits)
        {
            string sDocNo = "";
            string sDate;
            string sDateWithSlash = "";
            string sql = "";
            sql = "select convert(char(8),getdate(),112),convert(char(10),getdate(),111)";
            DataTable dtDate = PubUtility.SqlQry(sql, uu, "SYS");
            if (dtDate.Rows.Count == 0)
            {
                sDocNo = "";
                return sDocNo;
            }
            else
            {
                sDate = dtDate.Rows[0][0].ToString().Trim();
                sDateWithSlash = dtDate.Rows[0][1].ToString().Trim();
            }

            sql = "select SeqNo from DocumentNo a";
            sql += " where a.CompanyCode='" + uu.CompanyId.SqlQuote() + "' And Initial='" + DocType + "' And DocDate=convert(char(8),getdate(),112)";

            DataTable dtDoc = PubUtility.SqlQry(sql, uu, "SYS");
            //dtDoc.TableName = "dtDoc";

            using (DBOperator dbop = new DBOperator())

                if (dtDoc.Rows.Count == 0)
                {
                    string str = new string('0', Digits) + "1";
                    sDocNo = DocType + sDate + str.Substring(str.Length - Digits);
                    sql = "Insert Into DocumentNo (SGID, CompanyCode, CrtUser, CrtDate, CrtTime, ModUser, ModDate, ModTime, Initial, DocDate, SeqNo) ";
                    sql += " Select '" + uu.CompanyId.SqlQuote() + DocType + sDate + "', '" + uu.CompanyId.SqlQuote() + "'"
                         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                         + ", '" + DocType + "', '" + sDate + "', 1 ";
                    dbop.ExecuteSql(sql, uu, "SYS");
                }
                else
                {
                    int SeqNo = Convert.ToInt32(dtDoc.Rows[0][0]) + 1;

                    sql = "Update DocumentNo Set SeqNo=" + SeqNo + " "
                        + " ,ModUser='" + uu.UserID + "', ModDate=convert(char(10),getdate(),111), ModTime=convert(char(8),getdate(),108) "
                        + "Where CompanyCode='" + uu.CompanyId.SqlQuote() + "' And Initial='" + DocType + "' And DocDate='" + sDate + "'";
                    dbop.ExecuteSql(sql, uu, "SYS");
                    string str = new string('0', Digits) + SeqNo.ToString();
                    sDocNo = DocType + sDate + str.Substring(str.Length - Digits);
                }

            return sDocNo;
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

                    string DocNo = GetNewDocNo(uu, "VC", 6);

                    string sql = "Insert Into TempDocumentSV ";
                    sql += "(CompanyCode,ModUser,ModDate,ModTime,DocNo,SeqNo,PLU,Qty";
                    sql += ",Qty2,RNum, WhNo, CkNo, Layer, Sno, EffectiveDate, WorkType)";
                    sql += "select '" + uu.CompanyId + "','" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)";
                    sql += " ,'" + DocNo + "', Cast(Row_Number() Over(Order By a.Layer,a.Sno) As int), a.PLU, 0, "
                            + "0, 0, a.WhNo, a.CkNo, a.Layer, a.Sno,'', 'IA' ";
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


        //2021-06-21 Larry
        [Route("SystemSetup/SaveVIN47")]
        public ActionResult SystemSetup_SaveInv()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveVINOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                DataTable dtTemp = new DataTable("TempDocumentSV");
                PubUtility.AddStringColumns(dtTemp, "DocNo,WhNo,CkNo");
                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtTemp);
                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);
                DataRow dr = dtTemp.Rows[0];

                string DocNo = GetNewDocNo(uu, "AD", 3);

                string sql = "";
                string WhNoOut = "";
                string SysDate = "";
                using (DBOperator dbop = new DBOperator())
                {
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
                            sql += " Inner Join PLUSV b (Nolock) On a.CompanyCode=b.CompanyCode And a.PLU=b.GD_No ";
                            //sql += " Inner Join WarehouseDSV c (Nolock) On a.CompanyCode=c.CompanyCode And a.PLU=c.GD_No ";
                            sql += " Left Join InventorySV d (Nolock) On a.CompanyCode=d.CompanyCode And a.PLU=d.PLU "
                                + "And d.WhNo='" + dr["WhNo"].ToString().SqlQuote() + "' And d.CkNo='" + dr["CkNo"].ToString().SqlQuote() + "' ";

                            sql += " Where a.CompanyCode='" + uu.CompanyId + "' And a.DocNo='" + dr["DocNo"].ToString().SqlQuote() + "' ";
                            sql += " And (a.Qty>0 or (a.Qty>=0 And IsNull(a.EffectiveDate,'')<>''))";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //變更庫存數量及庫存增減日
                            string sqlTRIn = "";
                            sqlTRIn = "Select H.CompanyCode, H.TH_ID, H.WhNoIn, H.CkNoIn, D.LayerIn, D.SnoIn, D.SeqNo, D.PLU, D.InNum "
                                + " From TransferHSV H Inner Join TransferDSV D "
                                + " On H.CompanyCode = D.CompanyCode And H.TH_ID = D.TH_ID "
                                + " Where H.CompanyCode='" + uu.CompanyId + "' And H.TH_ID='" + DocNo + "' ";
                            //寫入調整jahoInvSV
                            sql = "Insert Into JahoInvSV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                + ", ModUser, ModDate, ModTime "
                                + ", DocType, DocNo, WhNo, SeqNo, PLU, Q1, Q2, Q3, CkNo, Layer, Sno) ";
                            sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", 'TF', TH_ID, WhNoIn, SeqNo, a.PLU, IsNull(b.PtNum,0), InNum, IsNull(b.PtNum,0)-InNum"
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
                                + ",PtNum=IsNull(PtNum,0) - InNum "
                                + ",In_Date = Case When In_Date>'" + SysDate + "' Then In_Date Else '" + SysDate + "' End "
                                + "From (" + sqlTRIn + ") a Inner Join InventorySV b "
                                + "On a.CompanyCode=b.CompanyCode and a.WhNoIn=b.WhNo and a.CkNoIn=b.CkNo And a.PLU=b.PLU "
                                + "and a.LayerIn=b.Layer And a.SnoIn=b.Sno ";
                            sql += " Where b.CompanyCode='" + uu.CompanyId + "' ";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            //沒有庫存資料-新增
                            sql = "Insert Into InventorySV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                + ", ModUser, ModDate, ModTime"
                                + ", WhNo, PLU, In_Date, CkNo, Layer, Sno) ";
                            sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                 + ", WhNoIn, a.PLU, '" + SysDate + "', CkNoIn, LayerIn, SnoIn "
                                 + " From (" + sqlTRIn + ") a Left Join InventorySV b "
                                 + " On a.CompanyCode = b.CompanyCode and a.WhNoIn = b.WhNo and a.CkNoIn = b.CkNo "
                                 + " and a.LayerIn=b.Layer And a.SnoIn=b.Sno And a.PLU=b.PLU "
                                 + "Where b.PLU Is Null ";
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
















    }
}
