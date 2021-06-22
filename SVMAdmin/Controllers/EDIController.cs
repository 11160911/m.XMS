using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;


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
                dsS.ReadXml( HttpContext.Request.Body);
                DataTable dtH = dsS.Tables["PosSalesH"];
                DataTable dtD = dsS.Tables["PosSalesD"];
                DataTable dtC = dsS.Tables["Company"];

                string sql = "select * from SalesH where CompanyCode='" + uu.CompanyId.SqlQuote() + "'";
                sql += " and ShopNo='" + dtC.Rows[0]["Face_ID"].ToString().SqlQuote() + "'";
                sql += " and OpenDate='"+ dtH.Rows[0]["SalesDate"].ToString().SqlQuote() + "'";
                sql += " and CKNo='" + dtH.Rows[0]["MachineNo"].ToString().SqlQuote() + "'";
                sql += " and ChrNo='" + dtH.Rows[0]["TransSeq"].ToString().PadLeft(6, '0').SqlQuote() + "'";
                DataTable dtA = PubUtility.SqlQry(sql, uu, "SYS");

                sql = "select * from SalesD where 1=2";
                DataTable dtB = PubUtility.SqlQry(sql, uu, "SYS");

                bool isExists = true;
                if (dtA.Rows.Count == 0)
                {
                    isExists = false;
                    dtA.Rows.Add(dtA.NewRow());
                }

                #region data mapping
                DataRow drA = dtA.Rows[0];
                drA["Status"] = "";
                drA["operCode"] = "";
                drA["ShopNo"] = dtC.Rows[0]["Face_ID"];
                drA["OpenDate"] = dtH.Rows[0]["SalesDate"];
                drA["CKNo"] = dtH.Rows[0]["MachineNo"];
                drA["ChrNo"] = dtH.Rows[0]["TransSeq"].ToString().PadLeft(6, '0');
                drA["OpenTime"] = dtH.Rows[0]["TransTime"];
                drA["InvType"] = dtH.Rows[0]["TaxType"];
                drA["Inv"] = dtH.Rows[0]["InvTYpe"];
                drA["InvNum"] = dtH.Rows[0]["InvNum"];
                drA["InvNo"] = dtH.Rows[0]["InvBegNo"];
                drA["InvNoE"] = dtH.Rows[0]["InvEndNo"];
                drA["CkerNo"] = dtH.Rows[0]["SalesMan"];
                drA["CUID"] = dtH.Rows[0]["SerialNo"];
                drA["IfVIP"] = dtH.Rows[0]["VIPNo"].ToString() == "" ? "0" : "1";
                drA["VIPNo"] = dtH.Rows[0]["VIPNo"];
                drA["InvTitle"] = dtH.Rows[0]["InvTitle"];
                drA["InvAddress"] = dtH.Rows[0]["InvAddress"];
                drA["PeopleNum"] = dtH.Rows[0]["Person"];
                drA["TableNum"] = dtH.Rows[0]["TableNo"];
                drA["SalesMan1"] = dtH.Rows[0]["Salesman1"];
                drA["SalesMan2"] = dtH.Rows[0]["Salesman2"];
                drA["SalesMan3"] = dtH.Rows[0]["Salesman3"];
                drA["SalesMan4"] = dtH.Rows[0]["Salesman4"];
                drA["SalesMan5"] = dtH.Rows[0]["Salesman5"];
                drA["SalesMan6"] = dtH.Rows[0]["Salesman6"];
                drA["Cash"] = dtH.Rows[0]["TotalAmt"];
                drA["Discount"] = -1* PubUtility.CB(dtH.Rows[0]["ItemDiscount"]) + PubUtility.CB(dtH.Rows[0]["Discount"]);
                drA["InvCash"] = PubUtility.CB(dtH.Rows[0]["TotalAmt"]) + PubUtility.CB(dtH.Rows[0]["TaxAmt"]);
                drA["Tax"] = dtH.Rows[0]["TaxAmt"];
                drA["FaxCash"] = dtH.Rows[0]["TotalTax"];
                drA["PriceDiscount"] = dtH.Rows[0]["PriceDiscount"];
                drA["HandDiscount"] = dtH.Rows[0]["HandDiscount"];
                drA["IfPass"] = "N";
                drA["PassUser"] = "";
                drA["PassDate"] = "";
                drA["VIP_Export"] = "";
                drA["SYSDate"] = dtH.Rows[0]["OpenDate"];
                drA["VIP_ID2"] = "";
                drA["IssuedCash"] = dtH.Rows[0]["IssuedCash"];
                drA["BOpenDate"] = "";
                drA["BCKNo"] = "";
                drA["BChrNo"] = "";
                drA["ComputerDate"] = dtH.Rows[0]["ModDate"];
                drA["RndCode"] = dtH.Rows[0]["RndCode"];
                drA["SubDiscount"] = dtH.Rows[0]["SubDiscount"];
                drA["SubHandDiscount"] = dtH.Rows[0]["SubHandDiscount"];
                drA["HeartCode"] = dtH.Rows[0]["HeartCode"];
                drA["MobileCode"] = dtH.Rows[0]["MobileCode"];
                drA["InvAmt"] = dtH.Rows[0]["InvAmt"];
                drA["InvPrint"] = dtH.Rows[0]["InvPrint"];
                drA["CopyPrint"] = dtH.Rows[0]["CopyPrint"];
                drA["SeqNo"] = dtH.Rows[0]["SeqNo"];
                drA["BComputerDate"] = "";
                drA["BOpenTime"] = "";
                drA["PS_NO"] = ""; // dtH.Rows[0]["PS_NO"];
                drA["PS_Point"] = DBNull.Value; // dtH.Rows[0]["PS_Point"];
                drA["SalesFlag"] = dtH.Rows[0]["SalesFlag"];
                drA["DocNO_Buy"] = "";
                drA["DocNO_Get"] = "";
                drA["PassDateSV"] = "";
                

                foreach(DataRow drD in dtD.Rows)
                {
                    DataRow drB = dtB.NewRow();
                    drB["Status"] = "";
                    drB["operCode"] = "";
                    drB["operCode"] = "";
                    drB["ShopNo"] = dtC.Rows[0]["Face_ID"];
                    drB["OpenDate"] = dtH.Rows[0]["SalesDate"];
                    drB["CkNo"] = dtH.Rows[0]["MachineNo"];
                    drB["ChrNo"] = dtH.Rows[0]["TransSeq"].ToString().PadLeft(6, '0');
                    drB["No"] = drD["ItemSeq"];
                    drB["GoodsNo"] = drD["GoodsNo"];
                    drB["Goods"] = "";
                    drB["Unit"] = drD["Unit"];
                    drB["Prices"] = drD["UnitPrice"];
                    drB["Num"] = drD["SalesQty"];
                    drB["Cash"] = PubUtility.CB(drD["SalesAmt"]) + PubUtility.CB(drD["SPDisc"]);
                    drB["Tax"] = drD["TaxType"];
                    drB["Discount"] = -1*( PubUtility.CB(drD["SplitDiscount"]) + PubUtility.CB(drD["Discount"]) + PubUtility.CB(drD["SPDisc"]));
                    drB["ItemType"] = "P";//看不懂
                    drB["DiscountRate"] = 0;//看不懂
                    drB["Specialdisc"] = drD["SpecialDisc"];
                    drB["Dpid"] ="";
                    drB["Promoteprice"] = DBNull.Value; 
                    drB["PromoteDiscount1"] = DBNull.Value; 
                    drB["PromoteDiscount"] = DBNull.Value; 
                    drB["GD_CodeType"] = drD["ItemType"];
                    drB["PriceDiscount"] = drD["PriceDiscount"];
                    drB["HandDiscount"] = drD["HandDiscount"];
                    drB["ProductSerialNo"] = drD["ProductSerialNo"];
                    drB["HSGID"] = "";
                    drB["issuedCash"] = DBNull.Value; 
                    drB["SubDiscount"] = DBNull.Value; //看不懂
                    drB["SubHandDiscount"] = DBNull.Value;//看不懂
                    drB["PLUType1"] = "1";
                    drB["GiftReason"] = "";
                    drB["Layer"] = DBNull.Value; 
                    drB["Sno"] = DBNull.Value; 
                    dtB.Rows.Add(drB);
                }
                #endregion

                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            sql = "delete from SalesD";
                            sql += " where CompanyCode='" + uu.CompanyId.SqlQuote() + "'";
                            sql += " and ShopNo='" + dtC.Rows[0]["Face_ID"].ToString().SqlQuote() + "'";
                            sql += " and OpenDate='" + dtH.Rows[0]["SalesDate"].ToString().SqlQuote() + "'";
                            sql += " and CKNo='" + dtH.Rows[0]["MachineNo"].ToString().SqlQuote() + "'";
                            sql += " and ChrNo='" + dtH.Rows[0]["TransSeq"].ToString().PadLeft(6, '0').SqlQuote() + "'";
                            dbop.ExecuteSql(sql, uu, "SYS");
                            string HSGID = dbop.Add("SalesH", dtA, uu, "SYS");
                            foreach (DataRow drB in dtB.Rows)
                                drB["HSGID"] = HSGID;
                            dbop.Add("SalesD", dtB, uu, "SYS");
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
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

    }


}
