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
    public class DataControllerSA : ControllerBase
    {

        [Route("AIReports/GetInitVSA21P")]
        public ActionResult GetInitVSA21P()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitVSA21POK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "Select ST_ID,ST_ID+ST_Sname ST_Sname from WarehouseSV where Companycode='" + uu.CompanyId.SqlQuote() + "' and ST_Type='6' order by ST_ID";
                DataTable dtSV = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtSV);
                dtSV.TableName = "dtWarehouse";

                sql = "Select CkNo,SNno from WarehouseDSV where Companycode='" + uu.CompanyId.SqlQuote() + "' and 1=2";
                DataTable dtDS = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtDS);
                dtDS.TableName = "dtWarehouseDSV";



            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("AIReports/SearchVSA21P")]
        public ActionResult SearchVSA21P()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchVSA21POK", "", "", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                IFormCollection rq = HttpContext.Request.Form;
                string OpenDateS = rq["OpenDateS"];
                string OpenDateE = rq["OpenDateE"];
                string ST_ID = rq["ST_ID"];
                string Ckno = rq["Ckno"];
                string KeyWord = rq["KeyWord"];
                string SortAmt = rq["SortAmt"];
                string sortcol = "cash";
                if (SortAmt == "N")
                    sortcol = "num";
                string sql = "select ROW_NUMBER() over(order by " + sortcol + " desc,goodsno) Seq".CrLf();
                sql += ",case isnull(GD_Sname,'') when '' then GoodsNo else GoodsNo+' '+GD_Sname end PLUNAME".CrLf();
                sql += ",CAST(GD_RETAIL as int) GD_RETAIL,Num,CAST(Cash as int) Cash from (".CrLf();
                sql += "select Goodsno,isnull(GD_Sname,'') GD_Sname,GD_RETAIL,SUM(Num) Num,Sum(Cash) Cash from SalesDSV a ".CrLf();
                sql += "left join PLUSV d on a.CompanyCode=d.CompanyCode and a.GoodsNo=d.GD_NO".CrLf();
                sql += "where a.CompanyCode='" + uu.CompanyId + "'".CrLf();
                sql += " and a.OpenDate>='" + OpenDateS.SqlQuote() + "'";
                sql += " and a.OpenDate<='" + OpenDateE.SqlQuote() + "'";
                if (ST_ID != "")
                    sql += " and a.ShopNo='" + ST_ID.SqlQuote() + "'";
                if (Ckno != "")
                    sql += " and a.CKNo='" + Ckno.SqlQuote() + "'";
                if (KeyWord != "")
                {
                    sql += " and (d.GD_NAME like '%" + KeyWord.SqlQuote() + "%'";
                    sql += " or d.GD_Sname like '%" + KeyWord.SqlQuote() + "%'";
                    sql += " or d.GD_NO like '" + KeyWord.SqlQuote() + "%')";
                }
                sql += " group by GoodsNo,GD_sname,GD_RETAIL) b";
                DataTable dtSalesDSV = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtSalesDSV);
                dtSalesDSV.TableName = "dtSalesDSV";
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("AIReports/SearchDVSA21P")]
        public ActionResult SearchDVSA21P()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchDVSA21POK", "", "", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                IFormCollection rq = HttpContext.Request.Form;
                string OpenDateS = rq["OpenDateS"];
                string OpenDateE = rq["OpenDateE"];
                string PLU = rq["PLU"];
                string SortAmt = rq["SortAmt"];
                string sortcol = "cash";
                if (SortAmt == "N")
                    sortcol = "num";
                string sql = "select ROW_NUMBER() over(order by " + sortcol + " desc,shopno,ckno) Seq".CrLf();
                sql += ",Shopno,Ckno,case isnull(ST_Sname,'') when '' then Shopno else ST_Sname+CkNo+N'機' end WhNAME".CrLf();
                sql += ",Num,CAST(Cash as int) Cash from (".CrLf();
                sql += "select shopno,CkNo,SUM(num) Num,SUM(Cash) Cash,isnull(ST_Sname,'') ST_Sname from SalesDSV a ".CrLf();
                sql += "left join Warehousesv w on a.companycode=w.companycode and a.shopno=w.st_id".CrLf();
                sql += "where a.CompanyCode='" + uu.CompanyId + "'".CrLf();
                sql += " and a.OpenDate>='" + OpenDateS.SqlQuote() + "'";
                sql += " and a.OpenDate<='" + OpenDateE.SqlQuote() + "'";
                sql += " and a.GoodsNo='" + PLU.SqlQuote() + "'";
                sql += " group by shopno,CkNo,ST_Sname) b";
                DataTable dtSalesD = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtSalesD);
                dtSalesD.TableName = "dtSalesD";
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("AIReports/SearchVSA21_7P")]
        public ActionResult SearchVSA21_7P()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchVSA21_7POK", "", "", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                IFormCollection rq = HttpContext.Request.Form;
                string OpenDateS = rq["OpenDateS"];
                string OpenDateE = rq["OpenDateE"];
                string SortAmt = rq["SortAmt"];
                string sortcol = "cash";
                if (SortAmt == "N")
                    sortcol = "num";
                string sql = "select ROW_NUMBER() over(order by " + sortcol + " desc,ST_DeliArea) Seq".CrLf();
                sql += ",ST_DeliArea,ISNULL([Type_Name],'') AreaName".CrLf();
                sql += ",Num,CAST(Cash as int) Cash from (".CrLf();
                sql += "select ST_DeliArea,ISNULL([Type_Name],'') [Type_Name],SUM(GoodsNum) Num,Sum(Cash) Cash from SalesHSV a ".CrLf();
                sql += "inner join WarehouseSV w on a.CompanyCode=w.CompanyCode and a.ShopNo=w.ST_ID".CrLf();
                sql += "left join TypeData d on a.CompanyCode=d.CompanyCode and d.Type_Code='DA' and w.ST_DeliArea=d.[Type_ID]".CrLf();
                sql += "where a.CompanyCode='" + uu.CompanyId + "'".CrLf();
                sql += " and a.OpenDate>='" + OpenDateS.SqlQuote() + "'";
                sql += " and a.OpenDate<='" + OpenDateE.SqlQuote() + "'";
                sql += " group by ST_DeliArea,[Type_name]) b";
                DataTable dtSalesHSV = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtSalesHSV);
                dtSalesHSV.TableName = "dtSalesHSV";
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("AIReports/SearchDVSA21_7P")]
        public ActionResult SearchDVSA21_7P()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchDVSA21_7POK", "", "", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {

                IFormCollection rq = HttpContext.Request.Form;
                string OpenDateS = rq["OpenDateS"];
                string OpenDateE = rq["OpenDateE"];
                string DeliArea = rq["DeliArea"];
                string SortAmt = rq["SortAmt"];
                string sortcol = "cash";
                if (SortAmt == "N")
                    sortcol = "num";
                string sql = "select ROW_NUMBER() over(order by " + sortcol + " desc,shopno,ckno) Seq".CrLf();
                sql += ",Shopno,Ckno,case isnull(ST_Sname,'') when '' then Shopno else ST_Sname+CkNo+N'機' end WhNAME".CrLf();
                sql += ",Num,CAST(Cash as int) Cash from (".CrLf();
                sql += "select shopno,CkNo,SUM(goodsnum) Num,SUM(Cash) Cash,isnull(ST_Sname,'') ST_Sname from SalesHSV a ".CrLf();
                sql += "left join Warehousesv w on a.companycode=w.companycode and a.shopno=w.st_id".CrLf();
                sql += "where a.CompanyCode='" + uu.CompanyId + "'".CrLf();
                sql += " and a.OpenDate>='" + OpenDateS.SqlQuote() + "'";
                sql += " and a.OpenDate<='" + OpenDateE.SqlQuote() + "'";
                sql += " and w.ST_DeliArea='" + DeliArea.SqlQuote() + "'";
                sql += " group by shopno,CkNo,ST_Sname) b";
                DataTable dtSHSV = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtSHSV);
                dtSHSV.TableName = "dtSHSV";
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }
















    }
}
