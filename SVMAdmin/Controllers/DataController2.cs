using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace SVMAdmin.Controllers
{
    [Route("api")]
    [ApiController]
    public class DataController2 : ControllerBase
    {

        [Route("SystemSetup/GetInitMMMachineSet")]
        public ActionResult GetInitMMMachineSet()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitMMMachineSetOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "select * from Rack  order by Type_ID";
                DataTable dtR = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtR);
                dtR.TableName = "dtRack";
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }


        private string SearchMachineSQL (UserInfo uu)
        {
            string sql = "select a.*,b.Lyaers,b.Channels";
            sql += " from MachineList a";
            sql += " inner join";
            sql += " (select CompanyCode,SNno,count(distinct LayerNo) as Lyaers, count(*)  as Channels";
            sql += " from  MachineListSpec group by CompanyCode,SNno";
            sql += " ) b on a.CompanyCode=b.CompanyCode and a.SNno=b.SNno";
            sql += " where a.CompanyCode='" + uu.CompanyId.SqlQuote() + "'";
            return sql;
        }

        [Route("SystemSetup/SearchMachine")]
        public ActionResult SystemSetup_SearchMachine()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SearchMachineOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string KeyWord = rq["KeyWord"];
                //string sql = "select a.*,b.Lyaers,b.Channels";
                //sql += " from MachineList a";
                //sql += " inner join";
                //sql += " (select CompanyCode,SNno,count(distinct LayerNo) as Lyaers, count(*)  as Channels";
                //sql += " from  MachineListSpec group by CompanyCode,SNno";
                //sql += " ) b on a.CompanyCode=b.CompanyCode and a.SNno=b.SNno";
                //sql += " where 1=1";
                string sql = SearchMachineSQL(uu);
                if (KeyWord != "")
                {
                    sql += " and (";
                    sql += " a.SNno like '" + KeyWord + "%'";
                    //sql += " or a.GD_NO='" + KeyWord + "'";
                    //sql += " or a.GD_Sname='" + KeyWord + "'";
                    sql += ")";
                }
                sql += "order by a.SNno";
                DataTable dtMachine = PubUtility.SqlQry(sql, uu, "SYS");
                dtMachine.TableName = "dtMachine";
                ds.Tables.Add(dtMachine);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SaveMachineList")]
        public ActionResult AddMachineList()
        {
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveMachineListOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                UserInfo uu = PubUtility.GetCurrentUser(this);
                DataTable dtH = new DataTable("MachineList");
                PubUtility.AddStringColumns(dtH, "EditMode,CompanyCode,SNno,StartDay,StopDay");
                dtH.Columns.Add("Temperature", typeof(double));

                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtH);

                DataTable dtD = new DataTable("MachineListSpec");
                PubUtility.AddStringColumns(dtD, "CompanyCode,SNno,LayerNo,ChannelType");
                dtD.Columns.Add("ChannelQty", typeof(double));
                dsRQ.Tables.Add(dtD);

                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);

                dtH.Rows[0]["CompanyCode"] = uu.CompanyId;
                DataTable dtS = new DataTable();
                PubUtility.AddStringColumns(dtS, "CompanyCode,SNno,LayerNo,ChannelNo,ChannelType");
                
                foreach (DataRow dr in dtD.Rows)
                {
                    dr["CompanyCode"] = uu.CompanyId;
                    int ch = 1;
                    for (int i=0; i < PubUtility.CB( dr["ChannelQty"]); i++)
                    {
                        DataRow drS = dtS.NewRow();
                        dtS.Rows.Add(drS);
                        List<string> fds = "CompanyCode,SNno,LayerNo,ChannelType".Split(',').ToList();
                        foreach (string fd in fds)
                        {
                            drS[fd] = dr[fd];
                        }
                        drS["ChannelNo"] = ch.ToString().PadLeft(2, '0');
                        ch++;
                    }
                }
                string EditMode = dtH.Rows[0]["EditMode"].ToString();
                string CompanyCode = dtH.Rows[0]["CompanyCode"].ToString();
                string SNno = dtH.Rows[0]["SNno"].ToString();
                dsRQ.Tables.Add(dtS);
                string sql;
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            if (EditMode == "Add")
                            {
                                dbop.Add("MachineList", dtH, uu, "SYS");
                                dbop.Add("MachineListSpec", dtS, uu, "SYS");
                            }
                            if (EditMode == "Modify")
                            {

                                dbop.Update("MachineList", dtH, new string [] { "CompanyCode", "SNno" }, uu, "SYS");
                                sql = "delete from MachineListSpec where CompanyCode='" + CompanyCode.SqlQuote() + "'";
                                sql += " and SNno='" + SNno.SqlQuote() + "'";
                                dbop.ExecuteSql(sql, uu, "SYS");
                                dbop.Add("MachineListSpec", dtS, uu, "SYS");
                            }


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
                sql = SearchMachineSQL(uu);
                sql += " and a.CompanyCode='" + CompanyCode.SqlQuote() + "'";
                sql += " and a.SNno='" + SNno.SqlQuote() + "'";
                DataTable dtMachine = PubUtility.SqlQry(sql, uu, "SYS");
                dtMachine.TableName = "dtMachine";
                ds.Tables.Add(dtMachine);
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetMachineDetail")]
        public ActionResult SystemSetup_GetMachineDetail()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetMachineDetailOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string CompanyCode = rq["CompanyCode"];
                string SNno = rq["SNno"];
                string sql = "select a.*,b.Lyaers,b.Channels";
                sql += " from MachineList a";
                sql += " inner join";
                sql += " (select CompanyCode,SNno,count(distinct LayerNo) as Lyaers, count(*)  as Channels";
                sql += " from  MachineListSpec group by CompanyCode,SNno";
                sql += " ) b on a.CompanyCode=b.CompanyCode and a.SNno=b.SNno";
                sql += " where a.CompanyCode='" + CompanyCode.SqlQuote() + "'";
                sql += " and a.SNno='" + SNno.SqlQuote() + "'";
                DataTable dtMachine = PubUtility.SqlQry(sql, uu, "SYS");
                dtMachine.TableName = "MachineList";
                ds.Tables.Add(dtMachine);

                sql = "select LayerNo,ChannelType,max(ChannelNo) as ChannelNo  from MachineListSpec a";
                sql += " where a.CompanyCode='" + CompanyCode.SqlQuote() + "'";
                sql += " and a.SNno='" + SNno.SqlQuote() + "'";
                sql += " group by LayerNo,ChannelType";
                sql += " order by LayerNo";
                DataTable MachineListSpec = PubUtility.SqlQry(sql, uu, "SYS");
                MachineListSpec.TableName = "MachineListSpec";
                ds.Tables.Add(MachineListSpec);

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
