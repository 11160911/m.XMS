using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Microsoft.VisualBasic;

namespace SVMAdmin.Controllers
{
    [Route("edi")]
    [ApiController]
    public class EDIController : ControllerBase
    {
        [Route("ReceivePosSalesFromVM")]
        public ActionResult GetInitMMMachineSet()
        {
            iXmsApiParameter AP = PubUtility.GetiXmsApiParameter(this, ConstList.ThisSiteConfig.SecurityKey);
            UserInfo uu = AP.user;
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ReceivePosSalesFromVMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataSet dsS = new DataSet();
                dsS.ReadXml(HttpContext.Request.Body);
                DataTable dtH = dsS.Tables["PosSalesH"];
                DataTable dtD = dsS.Tables["PosSalesD"];
                DataTable dtP = dsS.Tables["PosSalesP"];
                DataTable dtC = dsS.Tables["Company"];
                bool isExists = false;
                for (int i = 0; i < dtH.Rows.Count; i++)
                {
                    string sql = "select * from SalesH where CompanyCode='" + uu.CompanyId.SqlQuote() + "'";
                    sql += " and ShopNo='" + dtC.Rows[0]["Face_ID"].ToString().SqlQuote() + "'";
                    sql += " and OpenDate='" + dtH.Rows[i]["SalesDate"].ToString().SqlQuote() + "'";
                    sql += " and CKNo='" + dtH.Rows[i]["MachineNo"].ToString().SqlQuote() + "'";
                    sql += " and ChrNo='" + dtH.Rows[i]["TransSeq"].ToString().PadLeft(6, '0').SqlQuote() + "'";
                    DataTable dtA = PubUtility.SqlQry(sql, uu, "SYS");

                    sql = "select * from SalesD where 1=2";
                    DataTable dtB = PubUtility.SqlQry(sql, uu, "SYS");

                    sql = "select * from PaymentD where 1=2";
                    DataTable dtPD = PubUtility.SqlQry(sql, uu, "SYS");

                    if (dtA.Rows.Count == 0)
                    {
                        dtA.Rows.Add(dtA.NewRow());
                        if (!isExists)
                        {
                            sql = "insert into xSToWConnectRecSV (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime,ShopNo,CkNo,TranDate,UPFlag)".CrLf();
                            sql += "select '" + uu.CompanyId.SqlQuote() + "','001',Convert(varchar,getdate(),111),substring(convert(varchar,getdate(),121),12,12)".CrLf();
                            sql += ",'001',Convert(varchar,getdate(),111),substring(convert(varchar,getdate(),121),12,12),'" + dtC.Rows[0]["Face_ID"].ToString().SqlQuote() + "'".CrLf();
                            sql += ",'" + dtC.Rows[0]["POSID"].ToString().SqlQuote() + "','" + dtH.Rows[0]["SalesDate"].ToString().SqlQuote() + "'".CrLf();
                            sql += ",'v'";
                            PubUtility.ExecuteSql(sql, uu, "SYS");
                            isExists = true;
                        }

                        #region data mapping
                        DataRow drA = dtA.Rows[0];
                        drA["Status"] = "";
                        drA["operCode"] = "";
                        drA["ShopNo"] = dtC.Rows[0]["Face_ID"];
                        drA["OpenDate"] = dtH.Rows[i]["SalesDate"];
                        drA["CKNo"] = dtH.Rows[i]["MachineNo"];
                        drA["ChrNo"] = dtH.Rows[i]["TransSeq"].ToString().PadLeft(6, '0');
                        drA["OpenTime"] = dtH.Rows[i]["TransTime"];
                        drA["InvType"] = dtH.Rows[i]["TaxType"];
                        drA["Inv"] = dtH.Rows[i]["InvTYpe"];
                        drA["InvNum"] = dtH.Rows[i]["InvNum"];
                        drA["InvNo"] = dtH.Rows[i]["InvBegNo"];
                        drA["InvNoE"] = dtH.Rows[i]["InvEndNo"];
                        drA["CkerNo"] = dtH.Rows[i]["SalesMan"];
                        drA["CUID"] = dtH.Rows[i]["SerialNo"];
                        drA["IfVIP"] = dtH.Rows[i]["VIPNo"].ToString() == "" ? "0" : "1";
                        drA["VIPNo"] = dtH.Rows[i]["VIPNo"];
                        drA["InvTitle"] = dtH.Rows[i]["InvTitle"];
                        drA["InvAddress"] = dtH.Rows[i]["InvAddress"];
                        drA["PeopleNum"] = dtH.Rows[i]["Person"];
                        drA["TableNum"] = dtH.Rows[i]["TableNo"];
                        drA["SalesMan1"] = dtH.Rows[i]["Salesman1"];
                        drA["SalesMan2"] = dtH.Rows[i]["Salesman2"];
                        drA["SalesMan3"] = dtH.Rows[i]["Salesman3"];
                        drA["SalesMan4"] = dtH.Rows[i]["Salesman4"];
                        drA["SalesMan5"] = dtH.Rows[i]["Salesman5"];
                        drA["SalesMan6"] = dtH.Rows[i]["Salesman6"];
                        drA["Cash"] = dtH.Rows[i]["TotalAmt"];
                        drA["Discount"] = -1 * PubUtility.CB(dtH.Rows[i]["ItemDiscount"]) + PubUtility.CB(dtH.Rows[i]["Discount"]);
                        drA["InvCash"] = PubUtility.CB(dtH.Rows[i]["TotalAmt"]) + PubUtility.CB(dtH.Rows[i]["TaxAmt"]);
                        drA["Tax"] = dtH.Rows[i]["TaxAmt"];
                        drA["FaxCash"] = dtH.Rows[i]["TotalTax"];
                        drA["PriceDiscount"] = dtH.Rows[i]["PriceDiscount"];
                        drA["HandDiscount"] = dtH.Rows[i]["HandDiscount"];
                        drA["IfPass"] = "N";
                        drA["PassUser"] = "";
                        drA["PassDate"] = "";
                        drA["VIP_Export"] = "";
                        drA["SYSDate"] = dtH.Rows[i]["OpenDate"];
                        drA["VIP_ID2"] = "";
                        drA["IssuedCash"] = dtH.Rows[i]["IssuedCash"];
                        drA["BOpenDate"] = "";
                        drA["BCKNo"] = "";
                        drA["BChrNo"] = "";
                        drA["ComputerDate"] = dtH.Rows[i]["ModDate"];
                        drA["RndCode"] = dtH.Rows[i]["RndCode"];
                        drA["SubDiscount"] = dtH.Rows[i]["SubDiscount"];
                        drA["SubHandDiscount"] = dtH.Rows[i]["SubHandDiscount"];
                        drA["HeartCode"] = dtH.Rows[i]["HeartCode"];
                        drA["MobileCode"] = dtH.Rows[i]["MobileCode"];
                        drA["InvAmt"] = dtH.Rows[i]["InvAmt"];
                        drA["InvPrint"] = dtH.Rows[i]["InvPrint"];
                        drA["CopyPrint"] = dtH.Rows[i]["CopyPrint"];
                        drA["SeqNo"] = dtH.Rows[i]["SeqNo"];
                        drA["BComputerDate"] = "";
                        drA["BOpenTime"] = "";
                        drA["PS_NO"] = ""; // dtH.Rows[i]["PS_NO"];
                        drA["PS_Point"] = DBNull.Value; // dtH.Rows[i]["PS_Point"];
                        drA["SalesFlag"] = dtH.Rows[i]["SalesFlag"];
                        drA["DocNO_Buy"] = "";
                        drA["DocNO_Get"] = "";
                        drA["PassDateSV"] = "";


                        DataTable dtDtmp = dtD.Select("SalesDate='" + dtH.Rows[i]["SalesDate"] + "' and TransSeq=" + dtH.Rows[i]["TransSeq"].ToString()).CopyToDataTable();
                        foreach (DataRow drD in dtDtmp.Rows)
                        {
                            DataRow drB = dtB.NewRow();
                            drB["Status"] = drD["FlagPickup"];
                            drB["operCode"] = "";
                            drB["ShopNo"] = dtC.Rows[0]["Face_ID"];
                            drB["OpenDate"] = dtH.Rows[i]["SalesDate"];
                            drB["CkNo"] = dtH.Rows[i]["MachineNo"];
                            drB["ChrNo"] = dtH.Rows[i]["TransSeq"].ToString().PadLeft(6, '0');
                            drB["No"] = drD["ItemSeq"];
                            drB["GoodsNo"] = drD["GoodsNo"];
                            drB["Goods"] = "";
                            drB["Unit"] = drD["Unit"];
                            drB["Prices"] = drD["UnitPrice"];
                            drB["Num"] = drD["SalesQty"];
                            drB["Cash"] = PubUtility.CB(drD["SalesAmt"]) + PubUtility.CB(drD["SPDisc"]);
                            drB["Tax"] = drD["TaxType"];
                            drB["Discount"] = -1 * (PubUtility.CB(drD["SplitDiscount"]) + PubUtility.CB(drD["Discount"]) + PubUtility.CB(drD["SPDisc"]));
                            string strStatus = drD["Status"].ToString();
                            switch (strStatus)
                            {
                                case "P":
                                    drB["ItemType"] = "1";
                                    break;
                                case "C":
                                    drB["ItemType"] = "1";
                                    break;
                                case "Z":
                                    drB["ItemType"] = "1";
                                    break;
                                case "R":
                                    drB["ItemType"] = "10";
                                    break;
                                case "G":
                                    if (PubUtility.CB(drD["SalesQty"]) < 0)
                                    {
                                        drB["ItemType"] = "10";
                                    }
                                    else
                                    {
                                        drB["ItemType"] = "2";
                                    }
                                    break;
                                case "F":
                                    if (PubUtility.CB(drD["SalesQty"]) < 0)
                                    {
                                        drB["ItemType"] = "10";
                                    }
                                    else if (PubUtility.CB(drD["SalesQty"]) > 0 & PubUtility.CB(drB["Cash"]) == 0)
                                    {
                                        drB["ItemType"] = "2";
                                    }
                                    else
                                    {
                                        drB["ItemType"] = "1";
                                    }
                                    break;
                                default:
                                    drB["ItemType"] = "1";
                                    break;
                            }
                            if (PubUtility.CB(drD["UnitPrice"]) * PubUtility.CB(drD["SalesQty"]) == 0)
                            {
                                drB["DiscountRate"] = 0;
                            }
                            else
                            {
                                drB["DiscountRate"] = Math.Round((PubUtility.CB(drD["SalesAmt"]) + PubUtility.CB(drD["SPDisc"])) / (PubUtility.CB(drD["UnitPrice"]) * PubUtility.CB(drD["SalesQty"])) * 100, 2, MidpointRounding.AwayFromZero);//看不懂
                            }
                            if (PubUtility.CB(drB["DiscountRate"]) > 100) { drB["DiscountRate"] = 100; }
                            drB["Specialdisc"] = drD["SpecialDisc"];
                            drB["Dpid"] = "";
                            drB["Promoteprice"] = DBNull.Value;
                            drB["PromoteDiscount1"] = DBNull.Value;
                            drB["PromoteDiscount"] = DBNull.Value;
                            drB["GD_CodeType"] = drD["ItemType"];
                            drB["PriceDiscount"] = drD["PriceDiscount"];
                            drB["HandDiscount"] = drD["HandDiscount"];
                            drB["ProductSerialNo"] = drD["ProductSerialNo"];
                            drB["HSGID"] = "";
                            drB["issuedCash"] = DBNull.Value;
                            drB["SubDiscount"] = drD["SubDiscount"];
                            drB["SubHandDiscount"] = drD["SubHandDiscount"];
                            drB["PLUType1"] = "1";
                            drB["GiftReason"] = "";
                            drB["Layer"] = drD["Layer"];
                            drB["Sno"] = drD["Sno"];
                            dtB.Rows.Add(drB);
                        }

                        DataTable dtPtmp = dtP.Select("SalesDate='" + dtH.Rows[i]["SalesDate"] + "' and TransSeq=" + dtH.Rows[i]["TransSeq"].ToString()).CopyToDataTable();
                        foreach (DataRow drP in dtPtmp.Rows)
                        {
                            DataRow drPD = dtPD.NewRow();
                            drPD["Status"] = "";
                            drPD["operCode"] = "";
                            drPD["ShopNo"] = dtC.Rows[0]["Face_ID"];
                            drPD["OpenDate"] = dtH.Rows[i]["SalesDate"];
                            drPD["CkNo"] = dtH.Rows[i]["MachineNo"];
                            drPD["ChrNo"] = dtH.Rows[i]["TransSeq"].ToString().PadLeft(6, '0');
                            drPD["Pay_ID"] = drP["PayID"];
                            drPD["CurrencyType"] = drP["Currency"];
                            drPD["TaxFlag"] = "N";
                            drPD["Pay_Money"] = drP["PayAmt"];
                            drPD["CurrencyPay_Money"] = drP["CurrencyPayAmt"];
                            drPD["FEF"] = PubUtility.CB(drPD["Pay_Money"]) / PubUtility.CB(drPD["CurrencyPay_Money"]);
                            drPD["HSGID"] = "";
                            dtPD.Rows.Add(drPD);
                        }
                        #endregion

                        using (DBOperator dbop = new DBOperator())
                        {   //銷售回傳
                            using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                            {
                                try
                                {
                                    sql = "delete from SalesD";
                                    sql += " where CompanyCode='" + uu.CompanyId.SqlQuote() + "'";
                                    sql += " and ShopNo='" + dtC.Rows[0]["Face_ID"].ToString().SqlQuote() + "'";
                                    sql += " and OpenDate='" + dtH.Rows[i]["SalesDate"].ToString().SqlQuote() + "'";
                                    sql += " and CKNo='" + dtH.Rows[i]["MachineNo"].ToString().SqlQuote() + "'";
                                    sql += " and ChrNo='" + dtH.Rows[i]["TransSeq"].ToString().PadLeft(6, '0').SqlQuote() + "'";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    sql = "delete from PaymentD";
                                    sql += " where CompanyCode='" + uu.CompanyId.SqlQuote() + "'";
                                    sql += " and ShopNo='" + dtC.Rows[0]["Face_ID"].ToString().SqlQuote() + "'";
                                    sql += " and OpenDate='" + dtH.Rows[i]["SalesDate"].ToString().SqlQuote() + "'";
                                    sql += " and CKNo='" + dtH.Rows[i]["MachineNo"].ToString().SqlQuote() + "'";
                                    sql += " and ChrNo='" + dtH.Rows[i]["TransSeq"].ToString().PadLeft(6, '0').SqlQuote() + "'";
                                    dbop.ExecuteSql(sql, uu, "SYS");
                                    string HSGID = dbop.Add("SalesH", dtA, uu, "SYS");
                                    foreach (DataRow drB in dtB.Rows)
                                        drB["HSGID"] = HSGID;
                                    dbop.Add("SalesD", dtB, uu, "SYS");
                                    foreach (DataRow drPD in dtPD.Rows)
                                        drPD["HSGID"] = HSGID;
                                    dbop.Add("PaymentD", dtPD, uu, "SYS");

                                    //計算庫存
                                    string SysDate = "";
                                    sql = "select convert(char(10),getdate(),111) SysDate";

                                    DataTable dtSysDate = dbop.Query(sql, uu, "SYS");
                                    if (dtSysDate.Rows.Count > 0)
                                    {
                                        SysDate = dtSysDate.Rows[0][0].ToString();
                                    }

                                    string sqlSA = "Select H.CompanyCode, H.ShopNo, H.OpenDate, H.CkNo, H.ChrNo, D.Layer, D.Sno, D.[No], D.GoodsNo, D.Num "
                                            + " From SalesH H (nolock) Inner Join SalesD D (nolock) "
                                            + " On H.CompanyCode = D.CompanyCode And H.Shopno = D.Shopno and h.opendate=d.opendate and h.ckno=d.ckno and h.chrno=d.chrno "
                                            + " Where H.CompanyCode='" + uu.CompanyId + "'";
                                    sqlSA += " and h.ShopNo='" + dtC.Rows[0]["Face_ID"].ToString().SqlQuote() + "'";
                                    sqlSA += " and h.OpenDate='" + dtH.Rows[i]["SalesDate"].ToString().SqlQuote() + "'";
                                    sqlSA += " and h.CKNo='" + dtH.Rows[i]["MachineNo"].ToString().SqlQuote() + "'";
                                    sqlSA += " and h.ChrNo='" + dtH.Rows[i]["TransSeq"].ToString().PadLeft(6, '0').SqlQuote() + "'";
                                    //寫入銷售jahoInvSV
                                    sql = "Insert Into JahoInvSV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                        + ", ModUser, ModDate, ModTime "
                                        + ", DocType, DocNo, WhNo, SeqNo, PLU, Q1, Q2, Q3, CkNo, Layer, Sno) ";
                                    sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                         + ", 'S2', OpenDate+a.Ckno+Chrno+[No], a.ShopNo, [No], a.GoodsNo, IsNull(b.PtNum,0), -1*a.Num, IsNull(b.PtNum,0)+(-1*a.Num)"
                                         + ", a.CkNo, a.Layer, a.Sno "
                                         + " From (" + sqlSA + ") a Left Join InventorySV b "
                                         + "On a.CompanyCode = b.CompanyCode And a.ShopNo = b.WhNo and a.CkNo = b.CkNo "
                                         + "and a.Layer = b.Layer And a.Sno = b.Sno And a.GoodsNo=b.PLU ";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    //已有庫存資料
                                    sql = "Update InventorySV "
                                        + "Set ModUser='" + uu.UserID + "' "
                                        + ",ModDate=convert(char(10),getdate(),111) "
                                        + ",ModTime=convert(char(8),getdate(),108) "
                                        + ",PtNum=IsNull(PtNum,0) - Num "
                                        + ",StartSalesDate = Case When OpenDate<StartSalesDate Then OpenDate Else StartSalesDate End "
                                        + ",EndSalesDate = Case When OpenDate>EndSalesDate Then OpenDate Else EndSalesDate End "
                                        + "From (" + sqlSA + ") a Inner Join InventorySV b "
                                        + "On a.CompanyCode = b.CompanyCode And a.ShopNo = b.WhNo and a.CkNo = b.CkNo "
                                        + "and a.Layer = b.Layer And a.Sno = b.Sno And a.GoodsNo=b.PLU ";
                                    sql += " Where b.CompanyCode='" + uu.CompanyId + "' ";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    //沒有庫存資料-新增
                                    sql = "Insert Into InventorySV (CompanyCode, CrtUser, CrtDate, CrtTime "
                                        + ", ModUser, ModDate, ModTime"
                                        + ", WhNo, PLU, CkNo, Layer, Sno, "
                                        + "StartSalesDate, EndSalesDate,PtNum) ";
                                    sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                         + ", a.ShopNo, a.Goodsno, a.CkNo, a.Layer, a.Sno,a.OpenDate,a.OpenDate,-1*a.Num "
                                         + " From (" + sqlSA + ") a Left Join InventorySV b "
                                         + "On a.CompanyCode = b.CompanyCode And a.ShopNo = b.WhNo and a.CkNo = b.CkNo "
                                         + "and a.Layer = b.Layer And a.Sno = b.Sno And a.GoodsNo=b.PLU "
                                         + "Where b.PLU Is Null ";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    sql = "Update SalesH set PassDateSV=Replace(Convert(varchar(16),getdate(),121),'-','/')";
                                    sql += " where CompanyCode='" + uu.CompanyId.SqlQuote() + "'";
                                    sql += " and ShopNo='" + dtC.Rows[0]["Face_ID"].ToString().SqlQuote() + "'";
                                    sql += " and OpenDate='" + dtH.Rows[i]["SalesDate"].ToString().SqlQuote() + "'";
                                    sql += " and CKNo='" + dtH.Rows[i]["MachineNo"].ToString().SqlQuote() + "'";
                                    sql += " and ChrNo='" + dtH.Rows[i]["TransSeq"].ToString().PadLeft(6, '0').SqlQuote() + "'";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    ts.Complete();
                                }
                                catch (Exception err)
                                {
                                    ts.Dispose();
                                    dbop.Dispose();
                                    throw new Exception(err.Message);
                                }
                            }
                            dbop.Dispose();
                        }
                    }

                }


            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("ReceiveMachineErrFromVM")]
        public ActionResult GetMachineErrData()
        {
            iXmsApiParameter AP = PubUtility.GetiXmsApiParameter(this, ConstList.ThisSiteConfig.SecurityKey);
            UserInfo uu = AP.user;
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ReceiveMachineErrFromVMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataSet dsM = new DataSet();
                dsM.ReadXml(HttpContext.Request.Body);
                DataTable dtM = dsM.Tables["MachineErrLog"];
                DataTable dtC = dsM.Tables["Company"];

                string sql = "";
                Boolean IsinsertmaErr = false;
                using (DBOperator dbop = new DBOperator())
                {   //智販機硬體錯誤回傳
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            for (int i = 0; i < dtM.Rows.Count; i++)
                            {
                                sql = "select * from MachineErrLog where CompanyCode='" + uu.CompanyId.SqlQuote() + "'";
                                sql += " and ShopNo='" + dtC.Rows[0]["Face_ID"].ToString().SqlQuote() + "'";
                                sql += " and CKNo='" + dtC.Rows[0]["POSID"].ToString().SqlQuote() + "'";
                                sql += " and ErrDate='" + dtM.Rows[i]["ErrDate"].ToString().SqlQuote() + "'";
                                sql += " and ErrType='" + dtM.Rows[i]["ErrType"].ToString().SqlQuote() + "'";
                                sql += " and ErrCode='" + dtM.Rows[i]["ErrCode"].ToString().SqlQuote() + "'";
                                DataTable dtA = dbop.Query(sql, uu, "SYS");

                                if (dtA.Rows.Count == 0)
                                {
                                    if (!IsinsertmaErr)
                                    {
                                        sql = "insert into xSToWConnectRecSV (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime,ShopNo,CkNo,TranDate,UPFlag)".CrLf();
                                        sql += "select '" + uu.CompanyId.SqlQuote() + "','001',Convert(varchar,getdate(),111),substring(convert(varchar,getdate(),121),12,12)".CrLf();
                                        sql += ",'001',Convert(varchar,getdate(),111),substring(convert(varchar,getdate(),121),12,12),'" + dtC.Rows[0]["Face_ID"].ToString().SqlQuote() + "'".CrLf();
                                        sql += ",'" + dtC.Rows[0]["POSID"].ToString().SqlQuote() + "','" + dtC.Rows[0]["OpenDate"].ToString().SqlQuote() + "'".CrLf();
                                        sql += ",'v'";
                                        dbop.ExecuteSql(sql, uu, "SYS");
                                        IsinsertmaErr = true;
                                    }

                                    //寫入智販機硬體錯誤
                                    sql = "Insert Into MachineErrLog (CompanyCode, CrtUser, CrtDate, CrtTime "
                                       + ", ModUser, ModDate, ModTime "
                                       + ", ShopNo,ErrDate,ErrType,ErrCode,Ckno,ChrNo) ";
                                    sql += " Select '" + uu.CompanyId.SqlQuote() + "'"
                                        + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                        + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                                        + ", '" + dtC.Rows[0]["Face_ID"].ToString().SqlQuote() + "'";
                                    sql += ",'" + dtM.Rows[i]["ErrDate"].ToString() + "','" + dtM.Rows[i]["ErrType"].ToString() + "','" + dtM.Rows[i]["ErrCode"].ToString() + "'";
                                    sql += ",'" + (dtM.Rows[i]["MachineNo"].ToString() == "" ? dtC.Rows[0]["POSID"].ToString() : dtM.Rows[i]["MachineNo"].ToString()) + "'";
                                    sql += ",'" + (dtM.Rows[i]["TransSeq"].ToString() == "" ? "" : dtM.Rows[i]["TransSeq"].ToString().PadLeft(6, '0')) + "'";
                                    dbop.ExecuteSql(sql, uu, "SYS");

                                    sql = "Update MachineList Set FlagM='N',ModUser='" + uu.UserID + "' "
                                        + ",ModDate=convert(char(10),getdate(),111),ModTime=convert(char(8),getdate(),108) "
                                        + " where SNno='" + dtC.Rows[0]["SNno"].ToString().SqlQuote() + "'";
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
                    }
                    dbop.Dispose();
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("ReceiveMachineListFromVM")]
        public ActionResult GetMachineList()
        {
            iXmsApiParameter AP = PubUtility.GetiXmsApiParameter(this, ConstList.ThisSiteConfig.SecurityKey);
            UserInfo uu = AP.user;
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "ReceiveMachineListFromVMOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DataSet dsM = new DataSet();
                dsM.ReadXml(HttpContext.Request.Body);
                DataTable dtMa = dsM.Tables["MachineList"];
                DataTable dtC = dsM.Tables["Company"];

                string sql = "";
                using (DBOperator dbop = new DBOperator())
                {   //智販機硬體檔回傳
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            sql = "insert into xSToWConnectRecSV (CompanyCode,CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime,ShopNo,CkNo,TranDate,UPFlag)".CrLf();
                            sql += "select '" + uu.CompanyId.SqlQuote() + "','001',Convert(varchar,getdate(),111),substring(convert(varchar,getdate(),121),12,12)".CrLf();
                            sql += ",'001',Convert(varchar,getdate(),111),substring(convert(varchar,getdate(),121),12,12),'" + dtC.Rows[0]["Face_ID"].ToString().SqlQuote() + "'".CrLf();
                            sql += ",'" + dtC.Rows[0]["POSID"].ToString().SqlQuote() + "','" + dtC.Rows[0]["OpenDate"].ToString().SqlQuote() + "'";
                            sql += ",'v'";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            sql = "Update MachineList Set Temperature='" + dtMa.Rows[0]["Temperature"].ToString().SqlQuote() + "'";
                            sql += ",FlagT='" + dtMa.Rows[0]["FlagT"].ToString().SqlQuote() + "',FlagNet='Y'";
                            sql += ",LastTransDate=replace(convert(varchar(19),getdate(),121),'-','/')";
                            sql += ",FlagM='" + dtMa.Rows[0]["FlagM"].ToString().SqlQuote() + "',ModUser='" + uu.UserID + "' ";
                            sql += ",ModDate=convert(char(10),getdate(),111),ModTime=convert(char(8),getdate(),108) ";
                            sql += " where SNno='" + dtC.Rows[0]["SNno"].ToString().SqlQuote() + "'";
                            dbop.ExecuteSql(sql, uu, "SYS");

                            sql = "select * from MachineList (nolock) where SNno='" + dtC.Rows[0]["SNno"].ToString().SqlQuote() + "'";
                            DataTable dtR = dbop.Query(sql, uu, "SYS");
                            dtR.TableName = "RMachineList";
                            ds.Tables.Add(dtR);
                            ts.Complete();
                        }
                        catch (Exception err)
                        {
                            ts.Dispose();
                            dbop.Dispose();
                            throw new Exception(err.Message);
                        }
                    }
                    dbop.Dispose();
                }
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("DownLoadData")]
        public ActionResult DownLoadData()
        {
            iXmsApiParameter AP = PubUtility.GetiXmsApiParameter(this, ConstList.ThisSiteConfig.SecurityKey);
            UserInfo uu = AP.user;
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "DownLoadDataOK", "", "", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                DateTime lastDT = new DateTime();
                DataSet dsS = new DataSet();
                dsS.ReadXml(HttpContext.Request.Body);
                DataTable dtC = dsS.Tables["Company"];
                string LastTransDate = "";
                string SNno = dtC.Rows[0]["SNno"].ToString();
                string sql = "select LastTransDate from WarehouseDSV where CompanyCode='" + uu.CompanyId + "'".CrLf();
                sql += " and SNno='" + SNno.SqlQuote() + "'".CrLf();
                DataTable dt = PubUtility.SqlQry(sql, uu, "SYS");
                if (dt.Rows.Count != 0)
                {
                    if (dt.Rows[0][0].ToString() != "")
                    {
                        lastDT = Convert.ToDateTime(dt.Rows[0][0]);
                        lastDT = DateAndTime.DateAdd("m", -2, lastDT);
                        LastTransDate = lastDT.ToString("yyyy/MM/dd HH:mm");
                    }

                }
                if (LastTransDate == "")
                    LastTransDate = "0000/00/00 00:00:00.000";
                //PLUSV
                sql = "select * from PLUSV (nolock) where CompanyCode='" + uu.CompanyId + "'";
                sql += " and ModDate+' '+ModTime>'" + LastTransDate.SqlQuote() + "'";
                DataTable dtP = PubUtility.SqlQry(sql, uu, "SYS");
                dtP.TableName = "PLUSV";
                ds.Tables.Add(dtP);
                //ImageTable
                sql = "select SGID,DataType,FileName,DocType,DocImage from ImageTable (nolock) where CompanyCode='" + uu.CompanyId + "'";
                sql += " and ModDate+' '+ModTime>'" + LastTransDate.SqlQuote() + "'";
                dt = PubUtility.SqlQry(sql, uu, "SYS");
                dt.TableName = "ImageTable";
                ds.Tables.Add(dt);
                //InventorySV
                sql = "select CrtUser,CrtDate,CrtTime,ModUser,ModDate,ModTime,WhNO,PLU,PTNum,SafeNum,";
                sql += "In_Date,Out_Date,StartSalesDate,EndSalesDate,DisPlayNum,CkNo,Layer,Sno from InventorySV (nolock) ";
                sql += "where CompanyCode='" + uu.CompanyId + "' and ModDate+' '+ModTime>'" + LastTransDate.SqlQuote() + "'";
                dt = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dt);
                dt.TableName = "dtInventorySV";
                //EmployeeSV
                sql = "select * from EmployeeSV (nolock) ";
                sql += "where CompanyCode='" + uu.CompanyId + "' and ModDate+' '+ModTime>'" + LastTransDate.SqlQuote() + "'";
                dt = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dt);
                dt.TableName = "dtEmployeeSV";
                //Payment
                sql = "select Pay_ID,Pay_Type,PayPN,IsCash,CardNoEntry,CreditCardCheck,Change,Exceed,CanUse,Pay_EType,";
                sql += "Inv,CouponCheck,CouponValid,CouponUsed,InvBit,CouponValue,StandardCoupon,AllowPaySign,";
                sql += "UnitAmount,AllowDelete,CouponBit,RSalesFlag,AllRSalesFlag,APPFlag, from Payment (nolock) ";
                sql += "where CompanyCode='" + uu.CompanyId + "' and ModDate+' '+ModTime>'" + LastTransDate.SqlQuote() + "'";
                dt = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dt);
                dt.TableName = "dtPayment";
                //WarehouseSV/D
                sql = "select ST_ID,ST_Sname,ST_Name,ST_Type,Face_Tel,Face_Fax,ST_Cperson,ST_Ctel,Face_AreaMan,"
                    + "Face_Man1,ST_OpenDay,ST_StopDay,CanNotDelete,LastTransDate,InvGetQty,InvSaveQty,InvType,FlagInv,"
                    + "WhnoIn,ST_DeliArea from WarehouseSV where CompanyCode='" + uu.CompanyId + "'";
                sql += " and ModDate+' '+ModTime>'" + LastTransDate.SqlQuote() + "'";
                DataTable dtW = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtW);
                dtW.TableName = "dtWarehouseSV";

                sql = "select ST_ID,ST_Address,ST_OpenDay,ST_StopDay,CanNotDelete,LastTransDate,InvGetQty,InvSaveQty,"
                    + "InvType,CkNo,SNno,WhnoIn from WarehouseDSV where CompanyCode='" + uu.CompanyId + "'";
                sql += " and SNno='" + SNno.SqlQuote() + "'";
                sql += " and ModDate+' '+ModTime>'" + LastTransDate.SqlQuote() + "'";
                DataTable dtWD = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtWD);
                dtWD.TableName = "dtWarehouseDSV";
                //MachineList
                sql = "select ModUser,ModDate,ModTime,SNno,StartDay,StopDay,Temperature,FlagT,LastTransDate,FlagNet,FlagUse,MCSeq,FlagM from MachineList where CompanyCode='" + uu.CompanyId + "'";
                sql += " and SNno='" + SNno.SqlQuote() + "'";
                sql += " and ModDate+' '+ModTime>'" + LastTransDate.SqlQuote() + "'";
                DataTable dtM = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtM);
                dtM.TableName = "dtMachineList";
                //Currency
                sql = "select MID,FEF,CrtDate,CrtUser,ModDate,ModUser,PointNum1,PointNum2 from Currency where CompanyCode='" + uu.CompanyId + "'";
                sql += " and ModDate+' '+ModTime>'" + LastTransDate.SqlQuote() + "'";
                dt = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dt);
                dt.TableName = "dtCurrency";
                //InvdistributeSV

                //SystemCode/SystemValue/SystemParameter (保留)

                //MachineErrCode (保留)



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
