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

        
        [Route("AIReports/GetInitVSA76_1P")]
        public ActionResult GetInitVSA76_1P()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitVSA76_1POK", "" });
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


        [Route("AIReports/GetSales")]
        public ActionResult GetSales()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetSalesOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string OpenDate = rq["OpenDate"];

                string sql = "select a.ShopNo,a.CkNo,c.ST_SName+a.CkNo+'機' as ST_SName, ";
                sql += "CONVERT(int,sum(a.Num))Num,CONVERT(int,sum(a.cash))Cash, ";
                sql += "ROUND(sum(a.cash)/(select count(*) from salesh (nolock) where companycode='" + uu.CompanyId + "' and shopno=a.shopno and ckno=a.ckno ";
                if (OpenDate != "")
                {
                    sql += "and OpenDate='" + OpenDate + "' ";
                }
                sql += "group by shopno,ckno),0) as Cnt ";
                sql += "from SalesD a (nolock) ";
                sql += "left join WarehouseDSV b (nolock) on a.shopno=b.ST_ID and a.CKNo=b.CkNo and b.CompanyCode=a.CompanyCode and b.whnoin in (select whno from employeeSV (nolock) where companycode='" + uu.CompanyId + "' and man_id='" + uu.UserID + "') ";
                sql += "left join WarehouseSV c (nolock) on a.shopno=c.st_id and c.companycode=a.companycode ";
                sql += "Where a.CompanyCode='" + uu.CompanyId + "' ";
                if (OpenDate != "")
                {
                    sql += "and a.OpenDate='" + OpenDate + "' ";
                }
                sql += "group by a.ShopNo,a.CkNo,c.ST_SName ";
                sql += "order by a.shopno,a.ckno ";

                DataTable dtSales = PubUtility.SqlQry(sql, uu, "SYS");
                dtSales.TableName = "dtSales";
                ds.Tables.Add(dtSales);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("AIReports/GetSalesSearch")]
        public ActionResult GetSalesSearch()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetSalesSearchOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ShopNo = rq["ShopNo"];
                string OpenDate = rq["OpenDate"];
                string CkNo = rq["CkNo"];

                string sql = "select a.chrno,a.opendate + ' ' + a.opentime as opendate,b.layer+b.sno as layer, ";
                sql += "b.goodsno + ' ' + c.gd_sname as goodsno,CONVERT(int,b.num)num,CONVERT(int,b.cash)cash,e.pay_type ";
                sql += "from SalesH a (nolock) ";
                sql += "left join SalesD b (nolock) on a.shopno=b.shopno and a.opendate=b.opendate and a.ckno=b.ckno and a.chrno=b.chrno and b.companycode=a.companycode ";
                sql += "left join plusv c (nolock) on b.goodsno=c.gd_no and c.companycode=a.companycode ";
                sql += "left join paymentd d (nolock) on a.shopno=d.shopno and a.opendate=d.opendate and a.ckno=d.ckno and a.chrno=d.chrno and d.companycode=a.companycode ";
                sql += "left join payment e (nolock) on d.pay_id=e.pay_id and e.companycode=a.companycode ";
                sql += "Where a.CompanyCode='" + uu.CompanyId + "' ";
                if (ShopNo != "")
                {
                    sql += "and a.ShopNo='" + ShopNo + "' ";
                }
                if (OpenDate != "")
                {
                    sql += "and a.OpenDate='" + OpenDate + "' ";
                }
                if (CkNo != "")
                {
                    sql += "and a.CkNo='" + CkNo + "' ";
                }
                sql += "order by a.opendate,a.opentime,a.chrno ";

                DataTable dtSalesSearch = PubUtility.SqlQry(sql, uu, "SYS");
                dtSalesSearch.TableName = "dtSalesSearch";
                ds.Tables.Add(dtSalesSearch);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        [Route("AIReports/GetInitVSA76P")]
        public ActionResult GetInitVSA76P()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitVSA76POK", "" });
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

      
        [Route("AIReports/GetVSA76P")]
        public ActionResult GetVSA76P()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVSA76POK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string OpenDateS = rq["OpenDateS"];
                string OpenDateE = rq["OpenDateE"];

                string sql = "select a.ShopNo,a.CkNo,c.ST_SName+a.CkNo+'機' as ST_SName, ";
                sql += "CONVERT(int,sum(a.Num))Num,CONVERT(int,sum(a.cash))Cash,RANK() over(order by sum(a.cash) desc) as SeqNo ";
                sql += "from SalesD a (nolock) ";
                sql += "left join WarehouseDSV b (nolock) on a.shopno=b.ST_ID and a.CKNo=b.CkNo and b.CompanyCode=a.CompanyCode and b.whnoin in (select whno from employeeSV (nolock) where companycode='" + uu.CompanyId + "' and man_id='" + uu.UserID + "') ";
                sql += "left join WarehouseSV c (nolock) on a.shopno=c.st_id and c.companycode=a.companycode ";
                sql += "Where a.CompanyCode='" + uu.CompanyId + "' ";
                if (OpenDateS != "" && OpenDateE != "")
                {
                    sql += "and a.OpenDate between'" + OpenDateS + "' and '" + OpenDateE + "' ";
                }
                sql += "group by a.ShopNo,a.CkNo,c.ST_SName ";

                DataTable dtVSA76P = PubUtility.SqlQry(sql, uu, "SYS");
                dtVSA76P.TableName = "dtVSA76P";
                ds.Tables.Add(dtVSA76P);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

      
        [Route("AIReports/GetVSA76PSearch")]
        public ActionResult GetVSA76PSearch()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVSA76PSearchOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ShopNo = rq["ShopNo"];
                string OpenDateS = rq["OpenDateS"];
                string OpenDateE = rq["OpenDateE"];
                string CkNo = rq["CkNo"];

                string sql = "select a.chrno,a.opendate + ' ' + a.opentime as opendate,b.layer+b.sno as layer, ";
                sql += "b.goodsno + ' ' + c.gd_sname as goodsno,CONVERT(int,b.num)num,CONVERT(int,b.cash)cash,e.pay_type ";
                sql += "from SalesH a (nolock) ";
                sql += "left join SalesD b (nolock) on a.shopno=b.shopno and a.opendate=b.opendate and a.ckno=b.ckno and a.chrno=b.chrno and b.companycode=a.companycode ";
                sql += "left join plusv c (nolock) on b.goodsno=c.gd_no and c.companycode=a.companycode ";
                sql += "left join paymentd d (nolock) on a.shopno=d.shopno and a.opendate=d.opendate and a.ckno=d.ckno and a.chrno=d.chrno and d.companycode=a.companycode ";
                sql += "left join payment e (nolock) on d.pay_id=e.pay_id and e.companycode=a.companycode ";
                sql += "Where a.CompanyCode='" + uu.CompanyId + "' ";
                if (ShopNo != "")
                {
                    sql += "and a.ShopNo='" + ShopNo + "' ";
                }
                if (OpenDateS != "" && OpenDateE != "")
                {
                    sql += "and a.OpenDate between'" + OpenDateS + "' and '" + OpenDateE + "' ";
                }
                if (CkNo != "")
                {
                    sql += "and a.CkNo='" + CkNo + "' ";
                }
                sql += "order by a.opendate,a.opentime,a.chrno ";

                DataTable dtVSA76PSearch = PubUtility.SqlQry(sql, uu, "SYS");
                dtVSA76PSearch.TableName = "dtVSA76PSearch";
                ds.Tables.Add(dtVSA76PSearch);

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        
        [Route("AIReports/GetInitVSA73P")]
        public ActionResult GetInitVSA73P()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitVSA73POK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "Select ST_ID,ST_Sname from WarehouseSV where Companycode='" + uu.CompanyId.SqlQuote() + "' and ST_Type='6' order by ST_ID";
                DataTable dtSV = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtSV);
                dtSV.TableName = "dtWarehouse";
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

       
        [Route("AIReports/GetVSA73P")]
        public ActionResult GetVSA73P()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVSA73POK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string OpenDateS = rq["OpenDateS"];
                string OpenDateE = rq["OpenDateE"];
                string ShopNo = rq["ShopNo"];
                string CkNo = rq["CkNo"];
                string GoodsNo = rq["GoodsNo"];

                string ls_Cond = "";
                if (OpenDateS != "" && OpenDateE != "")
                    ls_Cond += "and a.OpenDate between '" + OpenDateS + "' and '" + OpenDateE + "' ";
                if (ShopNo != "")
                    ls_Cond += "and a.ShopNo='" + ShopNo + "' ";
                if (CkNo != "")
                    ls_Cond += "and a.CkNo='" + CkNo + "' ";
                string ls_PLU = "";
                if (GoodsNo != "")
                {
                    ls_PLU += "and (c.GD_NAME like '" + GoodsNo + "%' ";
                    ls_PLU += "or c.GD_Sname like '" + GoodsNo + "%' ";
                    ls_PLU += "or c.GD_NO like '" + GoodsNo + "%') ";
                }


                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            string sql = "select shopno,opendate,ckno,chrno,'  ' as part into #H ";
                            sql += "from SalesH (nolock) where 1=0";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            for (int i = 0; i < 24; i++)
                            {
                                sql = "insert into #H ";
                                sql += "select a.shopno,a.opendate,a.ckno,a.chrno, ";
                                if (i < 10)
                                    sql += "'0" + i + "' ";
                                else
                                    sql += "'" + i + "' ";
                                sql += "from SalesH a (nolock) ";
                                sql += "where a.companycode='" + uu.CompanyId + "' ";
                                if (i < 10)
                                    sql += "and a.opentime between '0" + i + ":00' and '0" + i + ":59' ";
                                else
                                    sql += "and a.opentime between '" + i + ":00' and '" + i + ":59' ";
                                sql += ls_Cond;
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }

                            sql = "select RANK() over(order by sum(a.cash) desc) as SeqNo,h.part + '點' as part, ";
                            sql += "CONVERT(int,sum(a.Num))Num,CONVERT(int,sum(a.cash))Cash ";
                            sql += "from SalesD a (nolock) ";
                            sql += "left join #H h on a.shopno=h.shopno and a.opendate=h.opendate and a.ckno=h.ckno and a.chrno=h.chrno ";
                            sql += "left join WarehouseDSV b (nolock) on a.shopno=b.ST_ID and a.CKNo=b.CkNo and b.CompanyCode=a.CompanyCode and b.whnoin in (select whno from employeeSV (nolock) where companycode='" + uu.CompanyId + "' and man_id='" + uu.UserID + "') ";
                            sql += "left join PLUSV c on a.CompanyCode=c.CompanyCode and a.GoodsNo=c.GD_NO ";
                            sql += "Where a.CompanyCode='" + uu.CompanyId + "' ";
                            sql += ls_Cond;
                            sql += ls_PLU;
                            sql += "group by h.part ";

                            DataTable dtVSA73P = dbop.Query(sql, uu, "SYS");
                            dtVSA73P.TableName = "dtVSA73P";
                            ds.Tables.Add(dtVSA73P);
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

        
        [Route("AIReports/GetInitVSA73_1P")]
        public ActionResult GetInitVSA73_1P()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitVSA73_1POK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "Select ST_ID,ST_Sname from WarehouseSV where Companycode='" + uu.CompanyId.SqlQuote() + "' and ST_Type='6' order by ST_ID";
                DataTable dtSV = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtSV);
                dtSV.TableName = "dtWarehouse";
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        
        [Route("AIReports/GetVSA73_1P")]
        public ActionResult GetVSA73_1P()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetVSA73_1POK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string OpenDate1 = rq["OpenDate1"];
                string OpenDate2 = rq["OpenDate2"];
                string OpenDate3 = rq["OpenDate3"];
                string OpenDate4 = rq["OpenDate4"];
                string OpenDate5 = rq["OpenDate5"];
                string OpenDate6 = rq["OpenDate6"];
                string OpenDate7 = rq["OpenDate7"];
                string ShopNo = rq["ShopNo"];
                string CkNo = rq["CkNo"];
                string GoodsNo = rq["GoodsNo"];

                string ls_Cond = "";
                if (OpenDate1 != "" && OpenDate7 != "")
                    ls_Cond += "and a.OpenDate between '" + OpenDate1 + "' and '" + OpenDate7 + "' ";
                if (ShopNo != "")
                    ls_Cond += "and a.ShopNo='" + ShopNo + "' ";
                if (CkNo != "")
                    ls_Cond += "and a.CkNo='" + CkNo + "' ";
                string ls_PLU = "";
                if (GoodsNo != "")
                {
                    ls_PLU += "and (c.GD_NAME like '" + GoodsNo + "%' ";
                    ls_PLU += "or c.GD_Sname like '" + GoodsNo + "%' ";
                    ls_PLU += "or c.GD_NO like '" + GoodsNo + "%') ";
                }
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            string sql = "select shopno,opendate,ckno,chrno,'  ' as part, ";
                            sql += "0 as Day1,0 as Day2,0 as Day3,0 as Day4,0 as Day5,0 as Day6, 0 as Day7 ";
                            sql += "into #H ";
                            sql += "from SalesH (nolock) where 1=0 ";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            for (int i = 0; i < 24; i++)
                            {
                                sql = "insert into #H ";
                                sql += "select a.shopno,a.opendate,a.ckno,a.chrno, ";
                                if (i < 10)
                                    sql += "'0" + i + "', ";
                                else
                                    sql += "'" + i + "', ";
                                sql += "0,0,0,0,0,0,0";
                                sql += "from SalesH a (nolock) ";
                                sql += "where a.companycode='" + uu.CompanyId + "' ";
                                if (i < 10)
                                    sql += "and a.opentime between '0" + i + ":00' and '0" + i + ":59' ";
                                else
                                    sql += "and a.opentime between '" + i + ":00' and '" + i + ":59' ";
                                sql += ls_Cond;
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }

                            for (int k = 1; k < 8; k++)
                            {
                                sql = "update #H set Day" + k + "=d.Cash from ";
                                sql += "(select a.shopno,a.opendate,a.ckno,a.chrno,sum(a.cash)cash from SalesD a (nolock) ";
                                sql += "inner join PLUSV c (nolock) on a.GoodsNo=c.GD_NO and c.CompanyCode=a.CompanyCode ";
                                sql += "where a.companycode='" + uu.CompanyId + "' ";
                                sql += ls_PLU;
                                sql += "group by a.shopno,a.opendate,a.ckno,a.chrno)d ";
                                sql += "where #H.shopno=d.shopno and #H.opendate=d.opendate and #H.ckno=d.ckno and #H.chrno=d.chrno ";

                                if (k == 1)
                                    sql += "and #H.OpenDate='" + OpenDate1 + "' ";
                                else if (k == 2)
                                    sql += "and #H.OpenDate='" + OpenDate2 + "' ";
                                else if (k == 3)
                                    sql += "and #H.OpenDate='" + OpenDate3 + "' ";
                                else if (k == 4)
                                    sql += "and #H.OpenDate='" + OpenDate4 + "' ";
                                else if (k == 5)
                                    sql += "and #H.OpenDate='" + OpenDate5 + "' ";
                                else if (k == 6)
                                    sql += "and #H.OpenDate='" + OpenDate6 + "' ";
                                else if (k == 7)
                                    sql += "and #H.OpenDate='" + OpenDate7 + "' ";
                                dbop.ExecuteSql(sql, uu, "SYS");
                            }

                            sql = "select a.part + '點' as part, ";
                            sql += "CONVERT(int,sum(a.Day1))Day1,CONVERT(int,sum(a.Day2))Day2,CONVERT(int,sum(a.Day3))Day3, ";
                            sql += "CONVERT(int,sum(a.Day4))Day4,CONVERT(int,sum(a.Day5))Day5,CONVERT(int,sum(a.Day6))Day6, ";
                            sql += "CONVERT(int,sum(a.Day7))Day7 ";
                            sql += "from #H a Where 1=1 ";
                            sql += "group by a.part ";
                            sql += "order by a.part ";
                            DataTable dtVSA73_1P = dbop.Query(sql, uu, "SYS");
                            dtVSA73_1P.TableName = "dtVSA73_1P";
                            ds.Tables.Add(dtVSA73_1P);
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
