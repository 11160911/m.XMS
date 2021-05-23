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
                string sql = "select * from Rack where CompanyCode='" + uu.CompanyId.SqlQuote() + "' order by Type_ID";
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
                    sql += " a.SNno like '" + KeyWord.SqlQuote() + "%'";
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


        [Route("SystemSetup/GetInitGMInvPLUSet")]
        public ActionResult GetInitGMInvPLUSet()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInitGMInvPLUSetOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                string sql = "Select ST_ID,ST_Sname from WarehouseSV where Companycode='" + uu.CompanyId.SqlQuote() + "' and ST_Type='6' order by ST_ID";
                DataTable dtSV = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtSV);
                dtSV.TableName = "dtWarehouse";

                sql = "Select CkNo,SNno from WarehouseDSV where Companycode='" + uu.CompanyId.SqlQuote() + "' and 1=2";
                DataTable dtDS = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtDS);
                dtDS.TableName = "dtWarehouseDSV";

                sql = "Select Distinct LayerNo from MachineListSpec where Companycode='" + uu.CompanyId.SqlQuote() + "' and 1=2";
                DataTable dtLS = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtLS);
                dtLS.TableName = "dtMachineListSpec";

            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetCkNoByST_ID")]
        public ActionResult GetCkNoByST_ID()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetCkNoByST_IDOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string ST_ID = rq["ST_ID"];
                string sql = "Select CkNo,SNno from WarehouseDSV where Companycode='" + uu.CompanyId + "' and ST_ID='" + ST_ID.SqlQuote() + "'";
                sql += " order by CkNo";
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

        [Route("SystemSetup/GetLayerNoBySNno")]
        public ActionResult GetLayerNoBySNno()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetLayerNoByCkNoOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string SNno = rq["SNno"];
                string sql = "Select Distinct LayerNo from MachineListSpec where Companycode='" + uu.CompanyId.SqlQuote() + "'";
                sql += " and SNno='" + SNno + "'";
                sql += " order by LayerNo";
                DataTable dtDS = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtDS);
                dtDS.TableName = "dtMachineListSpec";
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = "Exception";
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/GetInvtInfo")]
        public ActionResult GetInvtInfo()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInvtInfoOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string SNno = rq["SNno"];
                string LayerNo = rq["LayerNo"];

                string sql = SqlForGetInventorySVData(SNno, LayerNo, uu);

                DataTable dtIN = PubUtility.SqlQry(sql, uu, "SYS");
                if (dtIN.Rows.Count > 0)
                    throw new Exception("AlreadySetInventorySV");


                sql = "select a.*,b.Type_Name,d.ST_Sname,d.ST_ID,c.CkNo";
                sql += " from MachineListSpec a";
                sql += " inner join Rack b on a.CompanyCode=b.CompanyCode and a.ChannelType=b.Type_ID";
                sql += " inner join WarehouseDSV c on c.CompanyCode=a.CompanyCode and c.SNno=a.SNno";
                sql += " inner join WarehouseSV d on d.CompanyCode=c.CompanyCode and d.ST_ID=c.ST_ID";
                sql += " where a.CompanyCode='" + uu.CompanyId.SqlQuote() + "'";
                sql += " and a.SNno='" + SNno.SqlQuote() + "'";
                sql += " and a.LayerNo='" + LayerNo.SqlQuote() + "'";
                sql += " order by a.ChannelNo";
                DataTable dtDS = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtDS);
                dtDS.TableName = "dtMachineListSpec";




            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("SystemSetup/SaveNewInventorySV")]
        public ActionResult SaveNewInventorySV()
        {
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "SaveNewInventorySVOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                UserInfo uu = PubUtility.GetCurrentUser(this);
                DataTable dtH = new DataTable("InventorySV");
                PubUtility.AddStringColumns(dtH, "CompanyCode,WhNO,PLU,In_Date,Out_Date,StartSalesDate,EndSalesDate,CkNo,Layer,Sno,EffectiveDate");
                dtH.Columns.Add("PTNum", typeof(int));
                dtH.Columns.Add("SafeNum", typeof(int));
                dtH.Columns.Add("DisPlayNum", typeof(int));

                DataSet dsRQ = new DataSet();
                dsRQ.Tables.Add(dtH);

                PubUtility.FillDataFromRequest(dsRQ, HttpContext.Request.Form);

                dtH.Rows[0]["CompanyCode"] = uu.CompanyId;
                DataTable dtS = new DataTable();
                PubUtility.AddStringColumns(dtS, "CompanyCode,SNno,LayerNo,ChannelNo,ChannelType");

                foreach (DataRow dr in dtH.Rows)
                    dr["CompanyCode"] = uu.CompanyId;
                    
                using (DBOperator dbop = new DBOperator())
                {
                    using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required))
                    {
                        try
                        {
                            dbop.Add("InventorySV", dtH, uu, "SYS");
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
            }
            return PubUtility.DatasetXML(ds);
        }

        private string SqlForGetInventorySVData (string SNno , string LayerNo , UserInfo uu)
        {
            string sql = "select a.*,b.Type_Name,d.ST_Sname,d.ST_ID,c.CkNo,e.PLU,e.DisPlayNum";
            sql += ",f.GD_PRICES,f.Photo1,f.GD_NAME,f.GD_Sname";
            sql += " from MachineListSpec a";
            sql += " inner join Rack b on a.CompanyCode=b.CompanyCode and a.ChannelType=b.Type_ID";
            sql += " inner join WarehouseDSV c on c.CompanyCode=a.CompanyCode and c.SNno=a.SNno";
            sql += " inner join WarehouseSV d on d.CompanyCode=c.CompanyCode and d.ST_ID=c.ST_ID";
            sql += " inner join InventorySV e on e.CompanyCode=a.CompanyCode and e.WhNO=d.ST_ID and e.CkNo=c.CkNo and e.Layer=a.LayerNo and e.Sno=a.ChannelNo";
            sql += " inner join PLUSV f on f.CompanyCode=e.CompanyCode and f.GD_NO=e.PLU";
            sql += " where a.CompanyCode='" + uu.CompanyId.SqlQuote() + "'";
            sql += " and a.SNno='" + SNno.SqlQuote() + "'";
            sql += " and a.LayerNo='" + LayerNo.SqlQuote() + "'";
            sql += " order by a.ChannelNo";
            return sql;
        }

        [Route("SystemSetup/GetInventorySVData")]
        public ActionResult GetInventorySVData()
        {
            UserInfo uu = PubUtility.GetCurrentUser(this);
            System.Data.DataSet ds = PubUtility.GetApiReturn(new string[] { "GetInvtInfoOK", "" });
            DataTable dtMessage = ds.Tables["dtMessage"];
            try
            {
                IFormCollection rq = HttpContext.Request.Form;
                string SNno = rq["SNno"];
                string LayerNo = rq["LayerNo"];
                string sql = SqlForGetInventorySVData(SNno, LayerNo, uu);
                DataTable dtDS = PubUtility.SqlQry(sql, uu, "SYS");
                ds.Tables.Add(dtDS);
                dtDS.TableName = "dtMachineListSpec";
            }
            catch (Exception err)
            {
                dtMessage.Rows[0][0] = err.Message;
                dtMessage.Rows[0][1] = err.Message;
            }
            return PubUtility.DatasetXML(ds);
        }

        [Route("GetImageResize")]
        public ActionResult GetImageResize()
        {
            try
            {
                IQueryCollection rq = HttpContext.Request.Query;
                string SGID = PubUtility.DecodeSGID(rq["SGID"]);
                string UU = rq["UU"];
                string MaxPix = "0";
                if (rq.ContainsKey("MaxPix"))
                    MaxPix = rq["MaxPix"];
                UserInfo uu = PubUtility.GetCurrentUser("Bearer " + UU);
                DataTable dt = PubUtility.SqlQry("select * from ImageTable where SGID='" + SGID + "'", uu, "SYS");
                DataRow dr = dt.Rows[0];
                if (MaxPix != "0")
                {
                    byte[] bb = dr["DocImage"] as byte[];
                    System.IO.MemoryStream ms = new System.IO.MemoryStream(bb, 0, bb.Length);
                    System.Drawing.Image bm = System.Drawing.Image.FromStream(ms, true, false);
                    int imgW = bm.Width;
                    int imgH = bm.Height;
                    int maxP = PubUtility.CI(MaxPix);
                    decimal ratio = 1;
                    if (maxP !=0)
                    {
                        if (imgW > imgH & imgW > maxP)
                            ratio = imgW / maxP;
                        else if (imgH > imgW & imgH > maxP)
                            ratio = imgH / maxP;
                        if (ratio != 1)
                        {
                            int w = Convert.ToInt32(imgW / ratio);
                            int h = Convert.ToInt32(imgH / ratio);
                            System.Drawing.Bitmap bmS = ResizeImage(bm, w, h);
                            System.IO.MemoryStream msS = new System.IO.MemoryStream();
                            bmS.Save(msS, System.Drawing.Imaging.ImageFormat.Jpeg);
                            dr["DocType"] = "image/jpeg";
                            dr["DocImage"] = msS.ToArray();
                        }
                    }
                }
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

        private System.Drawing.Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new System.Drawing.Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = System.Drawing.Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, System.Drawing.GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }


    }
}
