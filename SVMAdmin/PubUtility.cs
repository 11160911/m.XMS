using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;
using System.Net.NetworkInformation;
using System.Drawing.Printing;
using System.Drawing;

using ZXing;
using ZXing.Common;
using ZXing.Rendering;
using ZXing.QrCode.Internal;



namespace SVMAdmin
{
    public static class PubUtility
    {

        public static string GetNewDocNo(UserInfo uu, String DocType, Int16 Digits)
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

            sql = "select SeqNo from DocumentNoWeb a";
            sql += " where a.CompanyCode='" + uu.CompanyId.SqlQuote() + "' And Initial='" + DocType + "' And DocDate=convert(char(8),getdate(),112)";

            DataTable dtDoc = PubUtility.SqlQry(sql, uu, "SYS");
            //dtDoc.TableName = "dtDoc";

            using (DBOperator dbop = new DBOperator())

                if (dtDoc.Rows.Count == 0)
                {
                    string str = new string('0', Digits) + "1";
                    sDocNo = DocType + sDate + str.Substring(str.Length - Digits);
                    sql = "Insert Into DocumentNoWeb (SGID, CompanyCode, CrtUser, CrtDate, CrtTime, ModUser, ModDate, ModTime, Initial, DocDate, SeqNo) ";
                    sql += " Select '" + uu.CompanyId.SqlQuote() + DocType + sDate + "', '" + uu.CompanyId.SqlQuote() + "'"
                         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                         + ", '" + uu.UserID + "',convert(char(10),getdate(),111),convert(char(8),getdate(),108)"
                         + ", '" + DocType + "', '" + sDate + "', 1 ";
                    dbop.ExecuteSql(sql, uu, "SYS");
                }
                else
                {
                    int SeqNo = Convert.ToInt32(dtDoc.Rows[0][0]) + 1;

                    sql = "Update DocumentNoWeb Set SeqNo=" + SeqNo + " "
                        + " ,ModUser='" + uu.UserID + "', ModDate=convert(char(10),getdate(),111), ModTime=convert(char(8),getdate(),108) "
                        + "Where CompanyCode='" + uu.CompanyId.SqlQuote() + "' And Initial='" + DocType + "' And DocDate='" + sDate + "'";
                    dbop.ExecuteSql(sql, uu, "SYS");
                    string str = new string('0', Digits) + SeqNo.ToString();
                    sDocNo = DocType + sDate + str.Substring(str.Length - Digits);
                }

            return sDocNo;
        }


        public static string PadLeft(String Str, char PadStr, Int16 Digits)
        {
            string sRes = new(PadStr, Digits);
            sRes = StrRigth(sRes + Str, Digits);
            return sRes;
        }

        public static string StrRigth(string s, int length)
        {
            return s.Substring(s.Length - length);
        }

        public static string StrLeft(string s, int length)
        {
            return s.Substring(0, length);
        }

        public static string StrMid(string s, int start, int length)
        {
            return s.Substring(start, length);
        }


        public static void AppendScriptAtBodyEnd(HtmlAgilityPack.HtmlDocument doc, string srcJsFile)
        {
            HtmlAgilityPack.HtmlNode script1 = doc.CreateElement("script");
            script1.Attributes.Add("type", "text/javascript");
            script1.Attributes.Add("src", srcJsFile + "?v=" + DateTime.Now.ToString("yyMMddHHmmss"));
            HtmlAgilityPack.HtmlNode ndh = doc.DocumentNode.SelectSingleNode("//body");
            ndh.AppendChild(script1);
        }

        public static void AppendCssAtHeadEnd(HtmlAgilityPack.HtmlDocument doc, string srcCssFile)
        {
            HtmlAgilityPack.HtmlNode css = doc.CreateElement("link");
            css.Attributes.Add("type", "text/css");
            css.Attributes.Add("rel", "stylesheet");
            css.Attributes.Add("href", srcCssFile + "?v=" + DateTime.Now.ToString("yyMMddHHmmss"));
            HtmlAgilityPack.HtmlNode ndh = doc.DocumentNode.SelectSingleNode("//head");
            ndh.AppendChild(css);
        }

        public static DataSet GetApiReturn(string[] message)
        {
            DataTable dtRt = new DataTable();
            dtRt.TableName = "dtMessage";
            for (int i = 0; i < message.Length; i++)
            {
                string fd = "Msg" + i.ToString();
                dtRt.Columns.Add(fd, typeof(string));
            }
            DataRow drRt = dtRt.NewRow();
            dtRt.Rows.Add(drRt);
            for (int i = 0; i < message.Length; i++)
            {
                string fd = "Msg" + i.ToString();
                dtRt.Rows[0][fd] = message[i];
            }
            dtRt.AcceptChanges();
            DataSet ds = new DataSet();
            ds.Tables.Add(dtRt);
            return ds;
        }

        internal static Microsoft.AspNetCore.Mvc.ContentResult DatasetXML(DataSet ds)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ds.WriteXml(ms, XmlWriteMode.WriteSchema);
            Microsoft.AspNetCore.Mvc.ContentResult CR = new Microsoft.AspNetCore.Mvc.ContentResult();
            CR.ContentType = "text/xml";
            CR.Content = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return CR;
        }

        public static bool IsNumericColumn(this DataColumn col)
        {
            if (col == null)
                return false;
            // Make this const
            var numericTypes = new[] { typeof(Byte), typeof(Decimal), typeof(Double),
        typeof(Int16), typeof(Int32), typeof(Int64), typeof(SByte),
        typeof(Single), typeof(UInt16), typeof(UInt32), typeof(UInt64)};
            return numericTypes.Contains(col.DataType);
        }

        /// <summary>
        /// 產生 JWT Token
        /// </summary>
        /// <param name="user">使用者</param>
        /// <returns></returns>
        internal static string GenerateJwtToken(UserInfo user)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Runtime.Serialization.Json.DataContractJsonSerializer js = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(UserInfo));
            js.WriteObject(ms, user);

            // 準備登入資訊
            var claims = new List<Claim>
            {
                //new Claim(JwtRegisteredClaimNames.Sub, user.Account),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("USEREMTITY",System.Text.Encoding.UTF8.GetString(ms.ToArray()))
                //new Claim(ClaimTypes.Role, user.Role)
            };

            // 製作 Token
            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            //var token = new JwtSecurityToken(
            //    _configuration["JwtIssuer"],
            //    _configuration["JwtIssuer"],
            //    claims,
            //    expires: expires,
            //    signingCredentials: creds
            //);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConstList.ThisSiteConfig.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(1));

            var token = new JwtSecurityToken(
                "http://yourdomain.com",
                "http://yourdomain.com",
                claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        internal static bool TryValidateToken(string token, out ClaimsPrincipal principal)
        {
            string Secret = ConstList.ThisSiteConfig.SecurityKey;
            principal = null;
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var handler = new JwtSecurityTokenHandler();

            try
            {
                var jwt = handler.ReadJwtToken(token);

                if (jwt == null)
                {
                    return false;
                }

                var secretBytes = Encoding.UTF8.GetBytes(Secret);

                var validationParameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(secretBytes),
                    ClockSkew = TimeSpan.Zero
                };
                SecurityToken securityToken;
                principal = handler.ValidateToken(token, validationParameters, out securityToken);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static UserInfo GetCurrentUser(string token)
        {
            UserInfo uu = new UserInfo();
            token = token.Substring(7);
            return DecodeUser(token);
        }

        public static UserInfo GetCurrentUser(ControllerBase _controler)
        {
            string token = _controler.HttpContext.Request.Headers["authorization"];
            //GetCurrentUser(token)
            //UserInfo uu = new UserInfo();
            //token = token.Substring(7);
            return GetCurrentUser(token);
        }

        private static UserInfo DecodeUser(string token)
        {
            UserInfo uu = new UserInfo();
            ClaimsPrincipal principal = null;
            if (TryValidateToken(token, out principal))
            {
                string kk = principal.FindFirstValue("USEREMTITY");
                System.Runtime.Serialization.Json.DataContractJsonSerializer js = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(UserInfo));
                uu = js.ReadObject(new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(kk))) as UserInfo;
            }
            else
            {

            }
            return uu;
        }

        public static void AddStringColumns(DataTable dt, string columnname)
        {
            string[] fds = columnname.Split(new char[] { ',' });
            for (int i = 0; i < fds.Length; i++)
                dt.Columns.Add(fds[i], typeof(string));
        }

        public static void FillDataFromRequest(DataSet ds, Microsoft.AspNetCore.Http.IFormCollection form)
        {
            foreach (DataTable dt in ds.Tables)
            {
                string tbname = dt.TableName;
                int r = dt.Rows.Count;
                string keyH = tbname + "[" + r.ToString() + "]";
                System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>> qrTb = form.Where(k => k.Key.StartsWith(keyH));
                int keycount = qrTb.Count();
                while (keycount > 0)
                {
                    DataRow dr = dt.NewRow();
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        string fd = dt.Columns[c].ColumnName;
                        string keyF = keyH + "[" + fd + "]";
                        if (form.ContainsKey(keyF))
                        {
                            string valueString = form[keyF];

                            Type tp = dt.Columns[fd].DataType;
                            if (tp == typeof(string))
                                dr[fd] = valueString;
                            else if (tp == typeof(double) | tp == typeof(decimal))
                                dr[fd] = PubUtility.CB(valueString);
                            else if (tp == typeof(Int32) | tp == typeof(Int16) | tp == typeof(int))
                                dr[fd] = PubUtility.CI(valueString);
                        }
                    }
                    dt.Rows.Add(dr);
                    r++;
                    keyH = tbname + "[" + r.ToString() + "]";
                    qrTb = form.Where(k => k.Key.StartsWith(keyH));
                    keycount = qrTb.Count();
                }
            }
        }


        public static object ConvertToEntity(string json, Type type)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            ms.Position = 0;
            System.Runtime.Serialization.Json.DataContractJsonSerializer ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(type);
            return ser.ReadObject(ms) as object;
        }

        public static T JsonFileToEntity<T>(string JsonFileName)
        {
            string strJ = System.IO.File.ReadAllText(JsonFileName, System.Text.Encoding.UTF8);
            T AP = (T)ConvertToEntity(strJ,typeof(T)) ;
            return AP;
        }

        private static string secKey = ConstList.ThisSiteConfig.SecurityKey;

        public static string GetSerString(object ob, Type T)
        {
            System.Runtime.Serialization.Json.DataContractJsonSerializer js = new System.Runtime.Serialization.Json.DataContractJsonSerializer(T);
            MemoryStream ms = new MemoryStream();
            js.WriteObject(ms, ob);
            string str = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            string a = aesEncryptBase64(str, secKey);
            return a;
        }

        /// <summary>
        /// 字串加密(非對稱式)
        /// </summary>
        /// <param name="Source">加密前字串</param>
        /// <param name="CryptoKey">加密金鑰</param>
        /// <returns>加密後字串</returns>
        public static string aesEncryptBase64(string SourceStr, string CryptoKey)
        {
            string encrypt = "";
            try
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
                byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
                byte[] iv = md5.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
                aes.Key = key;
                aes.IV = iv;

                byte[] dataByteArray = Encoding.UTF8.GetBytes(SourceStr);
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(dataByteArray, 0, dataByteArray.Length);
                    cs.FlushFinalBlock();
                    encrypt = Convert.ToBase64String(ms.ToArray());
                }
            }
            catch (Exception e)
            {
                //System.Windows.Forms.MessageBox.Show(e.Message);
            }
            return encrypt;
        }

        /// <summary>
        /// 字串解密(非對稱式)
        /// </summary>
        /// <param name="Source">解密前字串</param>
        /// <param name="CryptoKey">解密金鑰</param>
        /// <returns>解密後字串</returns>
        public static string aesDecryptBase64(string SourceStr, string CryptoKey)
        {
            string decrypt = "";
            try
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
                byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
                byte[] iv = md5.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
                aes.Key = key;
                aes.IV = iv;

                byte[] dataByteArray = Convert.FromBase64String(SourceStr);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(dataByteArray, 0, dataByteArray.Length);
                        cs.FlushFinalBlock();
                        decrypt = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
            catch (Exception e)
            {
                //System.Windows.Forms.MessageBox.Show(e.Message);
            }
            return decrypt;
        }

        //加解密(ixms)
        public static string enCode170215(String bb)
        {
            string k1 = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%*";
            string k2 = "qMd5UPwX76E4Wn9oZH8OKYTrV0aB!@#Ijtc*m2syzAiL1pFgGefDSxQv$%hbNJCRklu3";
            string tt = bb;
            int n6 = 68;
            string dd = "";
            string ch = "";
            int i = 0; int j = 0; int ex = 0;
            if (tt != null)
            {
                if (tt.Substring(0, 2) != "n$")
                {
                    for (i = 0; i < tt.Length; i++)
                    {
                        ch = tt.Substring(i, 1); ex = 0;
                        for (j = 0; j < n6; j++)
                        {
                            if (ch == k1.Substring(j, 1))
                            { dd = dd + k2.Substring(j, 1); }
                            ex = 1;
                        }
                        if (ex == 0)
                        { dd = dd + ch; }
                    }
                    return "n$" + dd;
                }
                else
                {
                    for (i = 2; i < tt.Length; i++)
                    {
                        ch = tt.Substring(i, 1); ex = 0;
                        for (j = 0; j < n6; j++)
                        {
                            if (ch == k2.Substring(j, 1))
                            { dd = dd + k1.Substring(j, 1); }
                            ex = 1;
                        }
                        if (ex == 0)
                        { dd = dd + ch; }
                    }
                    return dd;
                }
            }
            return dd;
        }


        public static void SetScriptVer(HtmlAgilityPack.HtmlDocument doc, string src)
        {
            string[] xPaths = new string[] {
                "//html//script[@src='" + src +"']"
            };
            for (int j = 0; j < xPaths.Length; j++)
            {
                HtmlAgilityPack.HtmlNodeCollection nds = doc.DocumentNode.SelectNodes(xPaths[j]);
                if (nds == null)
                    continue;
                for (int i = 0; i < nds.Count; i++)
                {
                    HtmlAgilityPack.HtmlNode nd = nds[i];
                    string NewSrc = nd.GetAttributeValue("src", "");
                    NewSrc += "?v=" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    nd.SetAttributeValue("src", NewSrc);
                }
            }
        }

        public static void SetCssVer(HtmlAgilityPack.HtmlDocument doc, string href)
        {
            string[] xPaths = new string[] {
                "//html//link[@href='" + href +"']"
            };
            for (int j = 0; j < xPaths.Length; j++)
            {
                HtmlAgilityPack.HtmlNodeCollection nds = doc.DocumentNode.SelectNodes(xPaths[j]);
                if (nds == null)
                    continue;
                for (int i = 0; i < nds.Count; i++)
                {
                    HtmlAgilityPack.HtmlNode nd = nds[i];
                    string Newhref = nd.GetAttributeValue("href", "");
                    Newhref += "?v=" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    nd.SetAttributeValue("href", Newhref);
                }
            }

        }

        public static DataTable SqlQry(string sql, UserInfo uu, string ModuleID)
        {
            DataTable dt = null;
            using (DBOperator dbop = new DBOperator())
            {
                try
                {
                    dt = dbop.Query(sql, uu, ModuleID);
                    dbop.Dispose();
                }
                catch (Exception error)
                {
                    dbop.Dispose();
                    throw error;
                }
            }
            return dt;
        }

        public static int ExecuteSql(string sql, UserInfo uu, string ModuleID)
        {
            int ir = 0;
            using (DBOperator dbop = new DBOperator())
            {
                try
                {
                    ir = dbop.ExecuteSql(sql, uu, ModuleID);
                    dbop.Dispose();
                }
                catch (Exception error)
                {
                    dbop.Dispose();
                    throw error;
                }
            }

            return ir;
        }

        public static string AddTable(string TableName, DataTable dt, UserInfo uu, string ModuleID)
        {
            string strSGID = "";
            using (DBOperator dbop = new DBOperator())
            {
                try
                {
                    strSGID = dbop.Add(TableName, dt, uu, ModuleID);
                    dbop.Dispose();
                }
                catch (Exception error)
                {
                    dbop.Dispose();
                    throw error;
                }
            }
            return strSGID;

        }

        public static int UpdateTable(string TableName, DataTable dt, UserInfo uu, string ModuleID)
        {
            int iEx = 0;
            using (DBOperator dbop = new DBOperator())
            {
                try
                {
                    iEx = dbop.Update(TableName, dt, uu, ModuleID);
                    dbop.Dispose();
                }
                catch (Exception error)
                {
                    dbop.Dispose();
                    throw new Exception(error.Message);
                }
            }
            return iEx;
        }

        public static int UpdateTable(string TableName, DataTable dt, string[] UniqueKey, UserInfo uu, string ModuleID)
        {
            int iEx = 0;
            using (DBOperator dbop = new DBOperator())
            {
                try
                {
                    iEx = dbop.Update(TableName, dt, UniqueKey, uu, ModuleID);
                    dbop.Dispose();
                }
                catch (Exception error)
                {
                    dbop.Dispose();
                    throw new Exception(error.Message);
                }
            }
            return iEx;
        }


        public static string DecodeSGID(string strSGID)
        {
            string strRt = "";
            string xx = strSGID;
            byte[] aa = new byte[xx.Length / 2];
            int p = 0;
            for (int i = 0; i < xx.Length - 1; i = i + 2)
            {
                string b = @"0x" + xx.Substring(i, 2);
                aa[p] = Convert.ToByte(b, 16);
                p++;
            }

            System.Text.Encoding en = System.Text.Encoding.ASCII;
            strRt = en.GetString(aa);
            return strRt;
        }
        public static int CI(object ob)
        {
            int b = 0;
            try
            {
                b = Convert.ToInt32(ob);
            }
            catch
            {

            }
            return b;
        }

        public static double CB(object ob)
        {
            double b = 0;
            try
            {
                b = Convert.ToDouble(ob);
            }
            catch
            {

            }
            return b;
        }

        public static string ConvertInsertSql(string tablename , DataRow dr ,UserInfo uu)
        {
            DataTable dt = dr.Table;
            string sql = "insert into "+ tablename + " (";
            string strval = "";
            for (int i=0; i<dt.Columns.Count; i++)
            {
                string fd = dt.Columns[i].ColumnName;
                sql += fd + ",";
                if (fd== "CompanyCode")
                    strval += "'" +uu.CompanyId+ "',";
                else if (fd == "CrtUser" | fd == "ModUser")
                    strval += "'" + uu.UserID + "',";
                else if (fd == "CrtDate" | fd == "ModDate")
                    strval += "'" + DateTime.Now.ToString("yyyy/MM/dd") + "',";
                else if (fd == "CrtTime" | fd == "ModTime")
                    strval += "'" + DateTime.Now.ToString("HH:mm:ss") + "',";
                else
                {
                    if (dr[fd] == DBNull.Value)
                        strval += "null,";
                    else
                    {
                        Type tp = dt.Columns[i].DataType;
                        if (tp == typeof(string))
                            strval += "N'" + dr[fd].ToString().SqlQuote() + "',";
                        else if (tp == typeof(double) | tp == typeof(decimal) | tp == typeof(int) | tp == typeof(byte))
                            strval += dr[fd].ToString() + ",";
                        else
                        {
                            strval += dr[fd].ToString() + ",";
                        }
                    }
                }
            }
            strval = "(" + strval.Substring(0, strval.Length - 1) + ")";
            sql = sql.Substring(0, sql.Length - 1) + ") values " + strval;
            return sql;
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public static iXmsApiParameter GetiXmsApiParameter(ControllerBase CB , string CrypKey)
        {
            iXmsApiParameter AP = null;
            try
            {
                string secKey = CrypKey;
                //string apikey = HttpContext.Request.Headers["ParaKey"];
                string apikey = CB.HttpContext.Request.Headers["ParaKey"];
                string strSer = StringEncrypt.StringEncrypt.aesDecryptBase64(apikey, secKey);
                //AP = Newtonsoft.Json.JsonConvert.DeserializeObject<iXmsApiParameter>(strSer);
                AP = ConvertToEntity(strSer, typeof(iXmsApiParameter)) as iXmsApiParameter;
            }
            catch { }
            return AP;
        }

        public static class UpLoadFiles
        {
            public static string FTPFilePath = ConstList.ThisSiteConfig.LogPath.Replace("Log", "FTPFile").Trim();
            //private static string FTPIP = ConstList.ThisSiteConfig.Companys
            //            .Where<Config.Company>(C => C.CompanyID == "OK").ToList()[0].FTPIP ;
            //private static string FTPID = PubUtility.enCode170215(ConstList.ThisSiteConfig.Companys
            //            .Where<Config.Company>(C => C.CompanyID == "OK").ToList()[0].FTPID);
            //private static string FTPPWD = PubUtility.enCode170215(ConstList.ThisSiteConfig.Companys
            //    .Where<Config.Company>(C => C.CompanyID == "OK").ToList()[0].FTPPWD);
            public static string FTPIP = "";//FTP的服务器地址，格式为ftp://192.168.1.234:8021/。ip地址和端口换成自己的，这些建议写在配置文件中，方便修改
            public static string FTPID = "";//FTP服务器的用户名
            public static string FTPPWD = "";//FTP服务器的密码

            #region 本地文件上传到FTP服务器
            /// <summary>
            /// 上传文件到远程ftp
            /// </summary>
            /// <param name="ftpPath">ftp上的文件路径</param>
            /// <param name="path">本地的文件目录</param>
            /// <param name="id">文件名</param>
            /// <returns></returns>
            public static bool UploadFile(string fpip, string fpid, string fppwd, string filepath)
            {
                FTPIP = fpip;
                FTPID = fpid;
                FTPPWD = fppwd;
                //string erroinfo = "";
                FileInfo f = new FileInfo(filepath);
                string a = "ftp://" + FTPIP;
                if (PubUtility.StrRigth(a, 1) != "/")
                {
                    a = a + "/";
                }
                try
                {
                    string FileName = "";
                    FileName = f.Name;

                    if (FileName.IndexOf("TAKE3") > -1)
                    {
                    }
                    else {
                        FileName = FileName.Split("_")[FileName.Split("_").GetUpperBound(0)];
                    }


                    FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(a + FileName));
                    reqFtp.UseBinary = true;
                    reqFtp.UsePassive = true; //false-PORT主動模式
                    reqFtp.Credentials = new NetworkCredential(FTPID, FTPPWD);
                    reqFtp.KeepAlive = false;
                    reqFtp.Method = WebRequestMethods.Ftp.UploadFile;
                    reqFtp.ContentLength = f.Length;
                    int buffLength = 1024;
                    byte[] buff = new byte[buffLength];
                    int contentLen;
                    FileStream fs = f.OpenRead();

                    Stream strm = reqFtp.GetRequestStream();
                    contentLen = fs.Read(buff, 0, buffLength);
                    while (contentLen != 0)
                    {
                        strm.Write(buff, 0, contentLen);
                        contentLen = fs.Read(buff, 0, buffLength);
                    }
                    strm.Close();
                    fs.Close();
                    //erroinfo = "完成";
                    return true;
                }
                catch (Exception ex)
                {
                    //erroinfo = string.Format("因{0},无法完成上传", ex.Message);
                    throw new Exception(string.Format("因{0},檔案無法完成上傳", ex.Message));
                    //return false;
                }
            }
            #endregion

            #region 从ftp服务器下载文件
            //private delegate void updateui(long rowCount, int i, ProgressBar PB);
            //public static void upui(long rowCount, int i, ProgressBar PB)
            //{
            //    try
            //    {
            //        PB.Value = i;
            //    }
            //    catch { }
            //}
            //////上面的代码实现了从ftp服务器下载文件的功能
            //public static Stream Download(string ftpfilepath)
            //{
            //    Stream ftpStream = null;
            //    FtpWebResponse response = null;
            //    try
            //    {
            //        ftpfilepath = ftpfilepath.Replace("\\", "/");
            //        string url = FTPIP + ftpfilepath;
            //        FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
            //        reqFtp.UseBinary = true;
            //        reqFtp.UsePassive = false; //false-PORT主動模式
            //        reqFtp.Credentials = new NetworkCredential(FTPID, FTPPWD);
            //        response = (FtpWebResponse)reqFtp.GetResponse();
            //        ftpStream = response.GetResponseStream();
            //    }
            //    catch (Exception ee)
            //    {
            //        if (response != null)
            //        {
            //            response.Close();
            //        }
            //        MessageBox.Show("文件读取出错，请确认FTP服务器服务开启并存在该文件");
            //    }
            //    return ftpStream;
            //}




            ///// <summary>
            ///// 从ftp服务器下载文件的功能
            ///// </summary>
            ///// <param name="ftpfilepath">ftp下载的地址</param>
            ///// <param name="filePath">存放到本地的路径</param>
            ///// <param name="fileName">保存的文件名称</param>
            ///// <returns></returns>
            //public static bool Download(string ftpfilepath, string filePath, string fileName)
            //{
            //    try
            //    {
            //        filePath = filePath.Replace("我的电脑\\", "");
            //        String onlyFileName = Path.GetFileName(fileName);
            //        string newFileName = filePath + onlyFileName;
            //        if (File.Exists(newFileName))
            //        {
            //            //errorinfo = string.Format("本地文件{0}已存在,无法下载", newFileName);                   
            //            File.Delete(newFileName);
            //            //return false;
            //        }
            //        ftpfilepath = ftpfilepath.Replace("\\", "/");
            //        string url = FTPIP + ftpfilepath;
            //        FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
            //        reqFtp.UseBinary = true;
            //        reqFtp.UsePassive = false; //false-PORT主動模式
            //        reqFtp.Credentials = new NetworkCredential(FTPID, FTPPWD);
            //        FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();
            //        Stream ftpStream = response.GetResponseStream();
            //        long cl = response.ContentLength;
            //        int bufferSize = 2048;
            //        int readCount;
            //        byte[] buffer = new byte[bufferSize];
            //        readCount = ftpStream.Read(buffer, 0, bufferSize);
            //        FileStream outputStream = new FileStream(newFileName, FileMode.Create);
            //        while (readCount > 0)
            //        {
            //            outputStream.Write(buffer, 0, readCount);
            //            readCount = ftpStream.Read(buffer, 0, bufferSize);
            //        }
            //        ftpStream.Close();
            //        outputStream.Close();
            //        response.Close();
            //        return true;
            //    }
            //    catch (Exception ex)
            //    {
            //        //errorinfo = string.Format("因{0},无法下载", ex.Message);
            //        return false;
            //    }
            //}
            ////
            ///// <summary>
            ///// 从ftp服务器下载文件的功能----带进度条
            ///// </summary>
            ///// <param name="ftpfilepath">ftp下载的地址</param>
            ///// <param name="filePath">保存本地的地址</param>
            ///// <param name="fileName">保存的名字</param>
            ///// <param name="pb">进度条引用</param>
            ///// <returns></returns>
            //public static bool Download(string ftpfilepath, string filePath, string fileName, ProgressBar pb)
            //{
            //    FtpWebRequest reqFtp = null;
            //    FtpWebResponse response = null;
            //    Stream ftpStream = null;
            //    FileStream outputStream = null;
            //    try
            //    {
            //        filePath = filePath.Replace("我的电脑\\", "");
            //        String onlyFileName = Path.GetFileName(fileName);
            //        string newFileName = filePath + onlyFileName;
            //        if (File.Exists(newFileName))
            //        {
            //            try
            //            {
            //                File.Delete(newFileName);
            //            }
            //            catch { }

            //        }
            //        ftpfilepath = ftpfilepath.Replace("\\", "/");
            //        string url = FTPIP + ftpfilepath;
            //        reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
            //        reqFtp.UseBinary = true;
            //        reqFtp.UsePassive = false; //false-PORT主動模式
            //        reqFtp.Credentials = new NetworkCredential(FTPID, FTPPWD);
            //        response = (FtpWebResponse)reqFtp.GetResponse();
            //        ftpStream = response.GetResponseStream();
            //        long cl = GetFileSize(url);
            //        int bufferSize = 2048;
            //        int readCount;
            //        byte[] buffer = new byte[bufferSize];
            //        readCount = ftpStream.Read(buffer, 0, bufferSize);
            //        outputStream = new FileStream(newFileName, FileMode.Create);

            //        float percent = 0;
            //        while (readCount > 0)
            //        {
            //            outputStream.Write(buffer, 0, readCount);
            //            readCount = ftpStream.Read(buffer, 0, bufferSize);
            //            percent = (float)outputStream.Length / (float)cl * 100;
            //            if (percent <= 100)
            //            {
            //                if (pb != null)
            //                {
            //                    pb.Invoke(new updateui(upui), new object[] { cl, (int)percent, pb });
            //                }
            //            }
            //            // pb.Invoke(new updateui(upui), new object[] { cl, outputStream.Length, pb });

            //        }

            //        //MessageBoxEx.Show("Download0");
            //        return true;
            //    }
            //    catch (Exception ex)
            //    {
            //        //errorinfo = string.Format("因{0},无法下载", ex.Message);
            //        //MessageBoxEx.Show("Download00");
            //        return false;
            //    }
            //    finally
            //    {
            //        //MessageBoxEx.Show("Download2");
            //        if (reqFtp != null)
            //        {
            //            reqFtp.Abort();
            //        }
            //        if (response != null)
            //        {
            //            response.Close();
            //        }
            //        if (ftpStream != null)
            //        {
            //            ftpStream.Close();
            //        }
            //        if (outputStream != null)
            //        {
            //            outputStream.Close();
            //        }
            //    }
            //}
            #endregion

            #region 获得文件的大小
            ///// <summary>
            ///// 获得文件大小
            ///// </summary>
            ///// <param name="url">FTP文件的完全路径</param>
            ///// <returns></returns>
            //public static long GetFileSize(string url)
            //{

            //    long fileSize = 0;
            //    try
            //    {
            //        FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
            //        reqFtp.UseBinary = true;
            //        reqFtp.UsePassive = false; //false-PORT主動模式
            //        reqFtp.Credentials = new NetworkCredential(FTPID, FTPPWD);
            //        reqFtp.Method = WebRequestMethods.Ftp.GetFileSize;
            //        FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();
            //        fileSize = response.ContentLength;

            //        response.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //    return fileSize;
            //}
            #endregion

            #region FTP新建資料夾
            /// <summary>
            ///在ftp服务器上创建文件目录
            /// </summary>
            /// <param name="dirName">文件目录</param>
            /// <returns></returns>
            public static bool MakeDir(string fpip, string fpid, string fppwd, string dirName)
            {
                FTPIP = fpip;
                FTPID = fpid;
                FTPPWD = fppwd;
                //string erroinfo = "";
                try
                {
                    bool b = false;
                    b = RemoteFtpDirExists(FTPIP, FTPID, FTPPWD, dirName);

                    if (b)
                    {
                        return true;
                    }
                    string a = "ftp://" + FTPIP;
                    if (a.Substring(a.Length - 1, 1) != "/")
                    {
                        a = a + "/";
                    }
                    string url = a + dirName;
                    FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
                    reqFtp.UseBinary = true;
                    reqFtp.UsePassive = true; //false-PORT主動模式
                                               // reqFtp.KeepAlive = false;
                    reqFtp.Method = WebRequestMethods.Ftp.MakeDirectory;
                    reqFtp.Credentials = new NetworkCredential(FTPID, FTPPWD);
                    FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();
                    response.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("因{0},FTP資料夾無法建立", ex.Message));
                    //return false;
                }

            }
            #endregion

            #region FTP資料夾/檔案是否存在
            /// <summary>
            /// 取得ftp上的文件目录List
            /// </summary>
            /// <param name="path"></param> 
            /// <returns></returns>
            public static bool RemoteFtpDirExists(string fpip, string fpid, string fppwd, string path)
            {
                FTPIP = fpip;
                FTPID = fpid;
                FTPPWD = fppwd;
                string a = "";
                a = "ftp://" + FTPIP;
                if (a.Substring(a.Length - 1, 1) != "/")
                {
                    a = a + "/";
                }

                if (path.IndexOf("TAKE3") > -1)
                {
                }
                else
                {
                    path = path.Split("_")[path.Split("_").GetUpperBound(0)];
                }


                try
                {
                    FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(a));
                    reqFtp.UseBinary = true;
                    reqFtp.UsePassive = true; //false-PORT主動模式
                    reqFtp.Credentials = new NetworkCredential(FTPID, FTPPWD);
                    reqFtp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                    FtpWebResponse resFtp = null;
                    StringBuilder result = new StringBuilder();

                    resFtp = (FtpWebResponse)reqFtp.GetResponse();
                    StreamReader reader = new StreamReader(resFtp.GetResponseStream(), Encoding.Default);
                    string line = reader.ReadLine();
                    int fileCnt = 0;
                    int gotfile = 0;
                    while (line != null)
                    {
                        //result.Append(line);
                        //result.Append("\n");
                        fileCnt += 1;
                        string file = line.Split(" ")[line.Split(" ").GetUpperBound(0)];
                        if (file.ToLower() == path.ToLower())
                        {
                            gotfile = 1;
                            break;
                        }

                        line = reader.ReadLine();
                    }
                    // to remove the trailing '\n'
                    //result.Remove(result.ToString().LastIndexOf('\n'), 1);
                    reader.Close();
                    resFtp.Close();
                    //return result.ToString().Split('\n');
                    if (fileCnt == 0) { return false; }
                    if (gotfile == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }


                }
                catch (Exception ex)
                {
                    throw new Exception("檢查資料夾/檔案是否存在失敗。原因： " + ex.Message);
                    //return false;
                }
            }
            #endregion

            #region 从ftp服务器删除文件的功能
            ///// <summary>
            ///// 从ftp服务器删除文件的功能
            ///// </summary>
            ///// <param name="fileName"></param>
            ///// <returns></returns>
            //public static bool DeleteFile(string fpip, string fpid, string fppwd, string fileName)
            //{
            //    try
            //    {
            //        FTPIP = fpip;
            //        FTPID = fpid;
            //        FTPPWD = fppwd;
            //        string url = FTPIP + fileName;
            //        FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
            //        reqFtp.UseBinary = true;
            //        reqFtp.UsePassive = false; //false-PORT主動模式
            //        reqFtp.KeepAlive = false;
            //        reqFtp.Method = WebRequestMethods.Ftp.DeleteFile;
            //        reqFtp.Credentials = new NetworkCredential(FTPID, FTPPWD);
            //        FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();
            //        response.Close();
            //        return true;
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception(string.Format("因{0},FTP檔案無法刪除", ex.Message));
            //        //return false;
            //    }
            //}
            #endregion
        }

        public static string GetSerString(object ob, Type T, string type = "")
        {
            System.Runtime.Serialization.Json.DataContractJsonSerializer js = new System.Runtime.Serialization.Json.DataContractJsonSerializer(T);
            MemoryStream ms = new MemoryStream();
            js.WriteObject(ms, ob);
            string str = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            string a = "";
            if (type == "")
            {
                a = aesEncryptBase64(str, secKey);
            }
            else if (type == "api")
            {
                a = aesEncryptBase64(str, "HelloKitty");
            }

            return a;
        }

        public static string GetYesterday(UserInfo uu)
        {
            string sDate;
            string sql = "";
            sql = "select convert(char,dateadd(DD,-1,getdate()),111)";
            DataTable dtDate = PubUtility.SqlQry(sql, uu, "SYS");
            sDate = dtDate.Rows[0][0].ToString();
            return sDate.Trim();
        }

        public static string GetToday(UserInfo uu)
        {
            string sDate;
            string sql = "";
            sql = "select convert(char(10),getdate(),111)";
            DataTable dtDate = PubUtility.SqlQry(sql, uu, "SYS");
            sDate = dtDate.Rows[0][0].ToString();
            return sDate.Trim();
        }
    }

    public static class ConstList
    {
        private static string _ConfigSetPath = "";

        private static Config.ThisSiteConfig _SiteConfig = null;

        private static IWebHostEnvironment _HostEnvironment = null;

        public static IWebHostEnvironment HostEnvironment
        {
            set { _HostEnvironment = value; }
            get { return _HostEnvironment; }
        }

        public static string ConfigSetPath
        {
            get
            {
                if (_ConfigSetPath == "")
                {
                    string fn = HostEnvironment.ContentRootPath + @"\ConfigSetPath.ini";
                    _ConfigSetPath = System.IO.File.ReadAllText(fn, System.Text.Encoding.UTF8);
                }
                return _ConfigSetPath;
            }
            

        }


        public static Config.ThisSiteConfig ThisSiteConfig
        {
            get
            {
                if (_SiteConfig == null)
                {
                    string fn = ConfigSetPath + @"\ThisSiteConfig.json";
                    _SiteConfig = PubUtility.JsonFileToEntity<Config.ThisSiteConfig>(fn);
                }
                return _SiteConfig;
            }
        }

        //public static DataTable AllFunction(DataTable dtA)
        //{
        //    //DataTable dt = new DataTable("dtAllFunction");
        //    //dt.Columns.Add("CategoryC", typeof(string));
        //    //dt.Columns.Add("Category", typeof(string));
        //    //dt.Columns.Add("ItemCode", typeof(string));
        //    //dt.Columns.Add("Description", typeof(string));
        //    //dt.Columns.Add("Page", typeof(string));
        //    //dt.Columns.Add("MobilePC", typeof(string));
        //    //dt.Columns.Add("Icon", typeof(string));

        //    //for (int i = 0; i < dtA.Rows.Count; i++) {
        //    //    dt.Rows.Add(new object[] { dtA.Rows[i]["SystemName"].ToString(), dtA.Rows[i]["SectionID"].ToString(), dtA.Rows[i]["ProgramID"].ToString(), dtA.Rows[i]["ProgramName"].ToString(), dtA.Rows[i]["ProgramID"].ToString(), "P", "" });
        //    //}



        //    //dt.Rows.Add(new object[] { "銷售查詢", "SalesQuery", "msSA101", "銷售分析", "msSA101", "P", "fa-cogs" });
        //    //dt.Rows.Add(new object[] { "銷售查詢", "SalesQuery", "msSA102", "組促銷售分析", "msSA102", "P", "fa-cubes" });
        //    //dt.Rows.Add(new object[] { "資料查詢", "DataQuery", "msDM101", "EDM發送設定作業", "msDM101", "P", "fa-th-large" });
        //    //dt.Rows.Add(new object[] { "其他", "Authorize", "MS99", "密碼變更", "MSPV101", "P", "fa-cog" });
        //    //dt.Rows.Add(new object[] { "其他", "Authorize", "MS99", "群組權限設定", "MSPV102", "P", "fa-cog" });
        //    //dt.Rows.Add(new object[] { "其他", "Authorize", "MS99", "公佈欄", "MSPV103", "P", "fa-cog" });
        //    //dt.Rows.Add(new object[] { "單價查詢", "PriceRequest", "ISAMQPLU", "單價查詢", "ISAMQPLU", "P", "fa-dollar" });
        //    //dt.Rows.Add(new object[] { "分區盤點上傳", "MachineManage", "ISAMToFTP", "分區盤點上傳", "ISAMToFTP", "P", "fa-cloud-upload" });
        //    //dt.Rows.Add(new object[] { "上傳查詢", "UpRecordQuery", "ISAMQFTPREC", "上傳查詢", "ISAMQFTPREC", "P", "fa-list" });
        //    //dt.Rows.Add(new object[] { "清除作業", "GoodsManage", "ISAMDelData", "清除作業", "ISAMDelData", "P", "fa-trash-o" });
        //    //dt.Rows.Add(new object[] { "店號設定", "ShopManage", "ISAMWHSET", "店號設定", "ISAMWHSET", "P", "fa-desktop" });




        //    //權限管理,智販機管理,商品管理,營運管理,營運分析
        //    //dt.Rows.Add(new object[] { "權限管理", "Authorize", "SysUsers", "使用者維護", "SysUsers", "P", "fa-cog" });
        //    //dt.Rows.Add(new object[] { "權限管理", "Authorize", "VPV01", "系統權限管理", "VPV01", "P", "fa-cog" });
        //    //dt.Rows.Add(new object[] { "權限管理", "Authorize", "SysChangePWD", "密碼變更作業", "SysChangePWD", "P", "fa-cog" });
        //    //dt.Rows.Add(new object[] { "功能選單", "MachineManage", "GMMacPLUSet", "盤點作業", "GMMacPLUSet", "P", "fa-cogs" });
        //    ////dt.Rows.Add(new object[] { "功能選單", "MachineManage", "ISAM01", "盤點作業", "ISAM01", "P", "fa-cogs" });
        //    //dt.Rows.Add(new object[] { "功能選單", "MachineManage", "ISAM02", "條碼收集", "ISAM02", "P", "fa-cogs" });
        //    //dt.Rows.Add(new object[] { "功能選單", "MachineManage", "ISAM03", "出貨/調撥", "ISAM03", "P", "fa-cogs" });
        //    //dt.Rows.Add(new object[] { "功能選單", "MachineManage", "ISAMQPLU", "單價查詢", "ISAMQPLU", "P", "fa-cogs" });
        //    //dt.Rows.Add(new object[] { "功能選單", "MachineManage", "ISAMTOFTP", "上傳作業", "ISAMTOFTP", "P", "fa-cogs" });
        //    //dt.Rows.Add(new object[] { "功能選單", "MachineManage", "ISAMDELDATA", "清除作業", "ISAMDELDATA", "P", "fa-cogs" });

        //    //return dt.Copy();
        //}

        private static System.Drawing.Bitmap imgBackBmp;
        private static System.Drawing.Bitmap imgBackG;
        private static int Resolution = 1000;
        private static int margin = 0;
    

        public static System.Drawing.Bitmap[] GetBitmap_Barcode(string Barcode)
        {
            try
            {
                System.Drawing.Bitmap[] bmps;
                bmps = new System.Drawing.Bitmap[1];

                string fontA = "Courier New";
                fontA = "標楷體";
                //fontA = "新細明體";
                string fontC = "Courier New";
                fontC = "Calibri";
                string fontT = "Times New Roman";


                System.Drawing.SolidBrush BrushB = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                System.Drawing.SolidBrush BrushW = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                System.Drawing.Pen LineP1 = new System.Drawing.Pen(System.Drawing.Color.Black, 40);
                System.Drawing.Pen LineP = new System.Drawing.Pen(System.Drawing.Color.Black, 20);
                System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(0, 0, 0));

                System.Drawing.Font drawFontC12 = new System.Drawing.Font(fontC, 12, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFontC26 = new System.Drawing.Font(fontC, 26, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFontC20 = new System.Drawing.Font(fontC, 20, System.Drawing.FontStyle.Bold);

                System.Drawing.Font drawFont6 = new System.Drawing.Font(fontA, 6, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont7 = new System.Drawing.Font(fontA, 7, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont8 = new System.Drawing.Font(fontA, 8, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont9 = new System.Drawing.Font(fontA, 9, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont10 = new System.Drawing.Font(fontA, 10, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont12 = new System.Drawing.Font(fontA, 12, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont16 = new System.Drawing.Font(fontA, 15, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont18 = new System.Drawing.Font(fontA, 18, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont14 = new System.Drawing.Font(fontA, 14, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont20 = new System.Drawing.Font(fontA, 20, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont24 = new System.Drawing.Font(fontA, 24, System.Drawing.FontStyle.Bold);

                System.Drawing.Font drawFontHeader = new System.Drawing.Font(fontA, 12, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFontHeader10 = new System.Drawing.Font(fontA, 10, System.Drawing.FontStyle.Bold);

                int iWidth = mm2px(15);
                int iHeight = mm2px(5);
                imgBackG = new System.Drawing.Bitmap(iWidth, iHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                imgBackG.SetResolution(Resolution, Resolution);     //列印解析度
                System.Drawing.Graphics grLabel = System.Drawing.Graphics.FromImage(imgBackG);
                grLabel.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                grLabel.Clear(System.Drawing.Color.White);

                string strCode39 = Convert.ToString(Barcode);
                grLabel.DrawImage(Generate2(strCode39, 0, 50), mm2px(2), mm2px(2), mm2px(10), mm2px(3));

                //qrcode
                //ZXing.BarcodeWriter bw = new ZXing.BarcodeWriter();
                //bw.Format = ZXing.BarcodeFormat.QR_CODE;
                //ZXing.QrCode.QrCodeEncodingOptions op = new ZXing.QrCode.QrCodeEncodingOptions();
                //op.Width = mm2px(25);
                //op.Height = mm2px(25);
                //op.Margin = 1;
                //op.CharacterSet = "UTF-8";
                //bw.Options = op;
                //QREncrypter qre = new QREncrypter();
                //string strQR = dt.Rows[0]["QR_CODE"].ToString();
                //System.Drawing.Bitmap bitmapQR = bw.Write(strQR);
                //grLabel.DrawImage(bitmapQR, mm2px(51), mm2px(50), mm2px(18), mm2px(18));

                grLabel.Dispose();
                System.Drawing.Imaging.BitmapData bmpData = imgBackG.LockBits(new System.Drawing.Rectangle(0, 0, iWidth, iHeight), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
                imgBackBmp = new System.Drawing.Bitmap(iWidth, iHeight, bmpData.Stride, System.Drawing.Imaging.PixelFormat.Format1bppIndexed, bmpData.Scan0);
                imgBackBmp.SetResolution(Resolution, Resolution);
                bmps[0] = imgBackBmp;
                return bmps;
            }
            catch (IOException e)
            {
                return null;
            }

        }

        public static System.Drawing.Bitmap[] GetBitmap_QRCode(string QRCode)
        {
            try
            {
                System.Drawing.Bitmap[] bmps;
                bmps = new System.Drawing.Bitmap[1];

                string fontA = "Courier New";
                fontA = "標楷體";
                //fontA = "新細明體";
                string fontC = "Courier New";
                fontC = "Calibri";
                string fontT = "Times New Roman";


                System.Drawing.SolidBrush BrushB = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                System.Drawing.SolidBrush BrushW = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                System.Drawing.Pen LineP1 = new System.Drawing.Pen(System.Drawing.Color.Black, 40);
                System.Drawing.Pen LineP = new System.Drawing.Pen(System.Drawing.Color.Black, 20);
                System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(0, 0, 0));

                System.Drawing.Font drawFontC12 = new System.Drawing.Font(fontC, 12, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFontC26 = new System.Drawing.Font(fontC, 26, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFontC20 = new System.Drawing.Font(fontC, 20, System.Drawing.FontStyle.Bold);

                System.Drawing.Font drawFont6 = new System.Drawing.Font(fontA, 6, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont7 = new System.Drawing.Font(fontA, 7, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont8 = new System.Drawing.Font(fontA, 8, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont9 = new System.Drawing.Font(fontA, 9, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont10 = new System.Drawing.Font(fontA, 10, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont12 = new System.Drawing.Font(fontA, 12, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont16 = new System.Drawing.Font(fontA, 15, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont18 = new System.Drawing.Font(fontA, 18, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont14 = new System.Drawing.Font(fontA, 14, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont20 = new System.Drawing.Font(fontA, 20, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont24 = new System.Drawing.Font(fontA, 24, System.Drawing.FontStyle.Bold);

                System.Drawing.Font drawFontHeader = new System.Drawing.Font(fontA, 12, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFontHeader10 = new System.Drawing.Font(fontA, 10, System.Drawing.FontStyle.Bold);

                int iWidth = mm2px(10);
                int iHeight = mm2px(10);
                imgBackG = new System.Drawing.Bitmap(iWidth, iHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                imgBackG.SetResolution(Resolution, Resolution);     //列印解析度
                System.Drawing.Graphics grLabel = System.Drawing.Graphics.FromImage(imgBackG);
                grLabel.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                grLabel.Clear(System.Drawing.Color.White);

                //qrcode
                ZXing.BarcodeWriter bw = new ZXing.BarcodeWriter();
                bw.Format = ZXing.BarcodeFormat.QR_CODE;
                ZXing.QrCode.QrCodeEncodingOptions op = new ZXing.QrCode.QrCodeEncodingOptions();
                op.Width = mm2px(25);
                op.Height = mm2px(25);
                op.Margin = 1;
                op.CharacterSet = "UTF-8";
                bw.Options = op;
                QREncrypter qre = new QREncrypter();
                string strQR = QRCode;
                System.Drawing.Bitmap bitmapQR = bw.Write(strQR);
                grLabel.DrawImage(bitmapQR, mm2px(2), mm2px(2), mm2px(5), mm2px(5));

                grLabel.Dispose();
                System.Drawing.Imaging.BitmapData bmpData = imgBackG.LockBits(new System.Drawing.Rectangle(0, 0, iWidth, iHeight), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
                imgBackBmp = new System.Drawing.Bitmap(iWidth, iHeight, bmpData.Stride, System.Drawing.Imaging.PixelFormat.Format1bppIndexed, bmpData.Scan0);
                imgBackBmp.SetResolution(Resolution, Resolution);
                bmps[0] = imgBackBmp;
                return bmps;
            }
            catch (IOException e)
            {
                return null;
            }

        }

        public static System.Drawing.Bitmap[] GetBitmap_QRCodeandBarcode(string Code)
        {
            try
            {
                System.Drawing.Bitmap[] bmps;
                bmps = new System.Drawing.Bitmap[1];

                string fontA = "Courier New";
                fontA = "標楷體";
                //fontA = "新細明體";
                string fontC = "Courier New";
                fontC = "Calibri";
                string fontT = "Times New Roman";


                System.Drawing.SolidBrush BrushB = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                System.Drawing.SolidBrush BrushW = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                System.Drawing.Pen LineP1 = new System.Drawing.Pen(System.Drawing.Color.Black, 40);
                System.Drawing.Pen LineP = new System.Drawing.Pen(System.Drawing.Color.Black, 20);
                System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(0, 0, 0));

                System.Drawing.Font drawFontC12 = new System.Drawing.Font(fontC, 12, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFontC26 = new System.Drawing.Font(fontC, 26, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFontC20 = new System.Drawing.Font(fontC, 20, System.Drawing.FontStyle.Bold);

                System.Drawing.Font drawFont6 = new System.Drawing.Font(fontA, 6, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont7 = new System.Drawing.Font(fontA, 7, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont8 = new System.Drawing.Font(fontA, 8, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont9 = new System.Drawing.Font(fontA, 9, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont10 = new System.Drawing.Font(fontA, 10, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont12 = new System.Drawing.Font(fontA, 12, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont16 = new System.Drawing.Font(fontA, 15, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont18 = new System.Drawing.Font(fontA, 18, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont14 = new System.Drawing.Font(fontA, 14, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont20 = new System.Drawing.Font(fontA, 20, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFont24 = new System.Drawing.Font(fontA, 24, System.Drawing.FontStyle.Bold);

                System.Drawing.Font drawFontHeader = new System.Drawing.Font(fontA, 12, System.Drawing.FontStyle.Bold);
                System.Drawing.Font drawFontHeader10 = new System.Drawing.Font(fontA, 10, System.Drawing.FontStyle.Bold);

                int iWidth = mm2px(10);
                int iHeight = mm2px(9);
                imgBackG = new System.Drawing.Bitmap(iWidth, iHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                imgBackG.SetResolution(Resolution, Resolution);     //列印解析度
                System.Drawing.Graphics grLabel = System.Drawing.Graphics.FromImage(imgBackG);
                grLabel.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                grLabel.Clear(System.Drawing.Color.White);

                //qrcode
                ZXing.BarcodeWriter bw = new ZXing.BarcodeWriter();
                bw.Format = ZXing.BarcodeFormat.QR_CODE;
                ZXing.QrCode.QrCodeEncodingOptions op = new ZXing.QrCode.QrCodeEncodingOptions();
                op.Width = mm2px(25);
                op.Height = mm2px(25);
                op.Margin = 1;
                op.CharacterSet = "UTF-8";
                bw.Options = op;
                QREncrypter qre = new QREncrypter();
                string strQR = Code;
                System.Drawing.Bitmap bitmapQR = bw.Write(strQR);
                grLabel.DrawImage(bitmapQR, mm2px(2), mm2px(0), mm2px(5), mm2px(5));

                string strCode39 = Convert.ToString(Code);
                grLabel.DrawImage(Generate2(strCode39, 0, 50), mm2px(1), mm2px(6), mm2px(10), mm2px(3));

                grLabel.Dispose();
                System.Drawing.Imaging.BitmapData bmpData = imgBackG.LockBits(new System.Drawing.Rectangle(0, 0, iWidth, iHeight), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
                imgBackBmp = new System.Drawing.Bitmap(iWidth, iHeight, bmpData.Stride, System.Drawing.Imaging.PixelFormat.Format1bppIndexed, bmpData.Scan0);
                imgBackBmp.SetResolution(Resolution, Resolution);
                bmps[0] = imgBackBmp;
                return bmps;
            }
            catch (IOException e)
            {
                return null;
            }

        }



        private static int mm2px(int mm)
        {
            return Convert.ToInt32(Math.Round((mm - 2 * margin) * Resolution / 25.4));
        }

        private static int mm2px(double mm)
        {
            return Convert.ToInt32(Math.Round((mm - 2 * margin) * Resolution / 25.4));
        }

        public static Bitmap Generate2(string text, int width, int height)
        {
            BarcodeWriter writer = new BarcodeWriter();
            //使用ITF 格式，不能被現在常用的支付寶、微信掃出來
            //如果想生成可識別的可以使用 CODE_128 格式
            //writer.Format = BarcodeFormat.ITF;
            writer.Format = BarcodeFormat.CODE_128;
            EncodingOptions options = new EncodingOptions()
            {
                Width = width,
                Height = height,
                Margin = 1
            };
            System.Drawing.Font drawFontC8 = new System.Drawing.Font("Consolas", 9, System.Drawing.FontStyle.Bold);
            BitmapRenderer Renderer = new BitmapRenderer
            {
                TextFont = drawFontC8
            };
            writer.Renderer = Renderer;
            writer.Options = options;
            Bitmap map = writer.Write(text);
            return map;
        }

        public class QREncrypter
        {
            /// <summary>
            /// 將發票資訊文字加密成驗證文字
            /// </summary>
            /// <param name="plainText">發票資訊</param>
            /// <param name="AESKey">種子密碼(QRcode)</param>
            /// <returns>加密後的HEX字串</returns>
            public string AESEncrypt(string plainText, string AESKey)
            {
                byte[] bytes = Encoding.Default.GetBytes(plainText);
                ICryptoTransform transform = new RijndaelManaged
                {
                    KeySize = 0x80,
                    Key = this.convertHexToByte(AESKey),
                    BlockSize = 0x80,
                    IV = Convert.FromBase64String("Dt8lyToo17X/XkXaQvihuA==")
                }.CreateEncryptor();
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
                stream2.Write(bytes, 0, bytes.Length);
                stream2.FlushFinalBlock();
                stream2.Close();
                return Convert.ToBase64String(stream.ToArray());
            }
            /// <summary>
            /// 轉換HEX值為 Binaries
            /// </summary>
            /// <param name="hexString">HEX字串</param>
            /// <returns>Binaries值</returns>
            private byte[] convertHexToByte(string hexString)
            {
                byte[] buffer = new byte[hexString.Length / 2];
                int index = 0;
                for (int i = 0; i < hexString.Length; i += 2)
                {
                    int num3 = Convert.ToInt32(hexString.Substring(i, 2), 0x10);
                    buffer[index] = BitConverter.GetBytes(num3)[0];
                    index++;
                }
                return buffer;
            }
            /// <summary>
            /// 檢查發票輸入資訊
            /// </summary>
            /// <param name="InvoiceNumber">發票字軌號碼共 10 碼</param>
            /// <param name="InvoiceDate">發票開立年月日(中華民國年份月份日期)共 7 碼</param>
            /// <param name="InvoiceTime">發票開立時間 (24 小時制) 共 6 碼</param>
            /// <param name="RandomNumber">4碼隨機碼</param>
            /// <param name="SalesAmount">以整數方式載入銷售額 (未稅)，若無法分離稅項則記載為0</param>
            /// <param name="TaxAmount">以整數方式載入稅額，若無法分離稅項則記載為0</param>
            /// <param name="TotalAmount">整數方式載入總計金額(含稅)</param>
            /// <param name="BuyerIdentifier">買受人統一編號，若買受人為一般消費者，請填入 00000000 8位字串</param>
            /// <param name="RepresentIdentifier">代表店統一編號，電子發票證明聯二維條碼規格已不使用代表店，請填入00000000 8位字串</param>
            /// <param name="SellerIdentifier">銷售店統一編號</param>
            /// <param name="BusinessIdentifier">總機構統一編號，如無總機構請填入銷售店統一編號</param>
            /// <param name="productArray">單項商品資訊</param>
            /// <param name="AESKey">加解密金鑰(QR種子密碼)</param>
            private void inputValidate(string InvoiceNumber,
                string InvoiceDate,
                string InvoiceTime,
                string RandomNumber,
                decimal SalesAmount,
                decimal TaxAmount,
                decimal TotalAmount,
                string BuyerIdentifier,
                string RepresentIdentifier,
                string SellerIdentifier,
                string BusinessIdentifier,
                Array[] productArray,
                string AESKey)
            {
                if (string.IsNullOrEmpty(InvoiceNumber) || (InvoiceNumber.Length != 10))
                {
                    throw new Exception("Invaild InvoiceNumber: " + InvoiceNumber);
                }
                if (string.IsNullOrEmpty(InvoiceDate) || (InvoiceDate.Length != 7))
                {
                    throw new Exception("Invaild InvoiceDate: " + InvoiceDate);
                }
                try
                {
                    long num = long.Parse(InvoiceDate);
                    int num2 = int.Parse(InvoiceDate.Substring(3, 2));
                    int num3 = int.Parse(InvoiceDate.Substring(5));
                    if ((num2 < 1) || (num2 > 12))
                    {
                        throw new Exception();
                    }
                    if ((num3 < 1) || (num3 > 0x1f))
                    {
                        throw new Exception();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Invaild InvoiceDate: " + InvoiceDate);
                }
                if (string.IsNullOrEmpty(InvoiceTime))
                {
                    throw new Exception("Invaild InvoiceTime: " + InvoiceTime);
                }
                if (string.IsNullOrEmpty(RandomNumber) || (RandomNumber.Length != 4))
                {
                    throw new Exception("Invaild RandomNumber: " + RandomNumber);
                }
                if (SalesAmount < 0M)
                {
                    throw new Exception("Invaild SalesAmount: " + SalesAmount);
                }
                if (TotalAmount < 0M)
                {
                    throw new Exception("Invaild TotalAmount: " + TotalAmount);
                }
                if (string.IsNullOrEmpty(BuyerIdentifier) || (BuyerIdentifier.Length != 8))
                {
                    throw new Exception("Invaild BuyerIdentifier: " + BuyerIdentifier);
                }
                if (string.IsNullOrEmpty(RepresentIdentifier))
                {
                    throw new Exception("Invaild RepresentIdentifier: " + RepresentIdentifier);
                }
                if (string.IsNullOrEmpty(SellerIdentifier) || (SellerIdentifier.Length != 8))
                {
                    throw new Exception("Invaild SellerIdentifier: " + SellerIdentifier);
                }
                if (string.IsNullOrEmpty(BusinessIdentifier))
                {
                    throw new Exception("Invaild BusinessIdentifier: " + BusinessIdentifier);
                }
                if ((productArray == null) || (productArray.Length == 0))
                {
                    throw new Exception("Invaild ProductArray");
                }
                if (string.IsNullOrEmpty(AESKey))
                {
                    throw new Exception("Invaild AESKey");
                }
            }
            /// <summary>
            /// 產生發票左邊QR碼
            /// </summary>
            /// <param name="InvoiceNumber">發票字軌號碼共 10 碼</param>
            /// <param name="InvoiceDate">發票開立年月日(中華民國年份月份日期)共 7 碼</param>
            /// <param name="InvoiceTime">發票開立時間 (24 小時制) 共 6 碼</param>
            /// <param name="RandomNumber">4碼隨機碼</param>
            /// <param name="SalesAmount">以整數方式載入銷售額 (未稅)，若無法分離稅項則記載為0</param>
            /// <param name="TaxAmount">以整數方式載入稅額，若無法分離稅項則記載為0</param>
            /// <param name="TotalAmount">整數方式載入總計金額(含稅)</param>
            /// <param name="BuyerIdentifier">買受人統一編號，若買受人為一般消費者，請填入 00000000 8位字串</param>
            /// <param name="RepresentIdentifier">代表店統一編號，電子發票證明聯二維條碼規格已不使用代表店，請填入00000000 8位字串</param>
            /// <param name="SellerIdentifier">銷售店統一編號</param>
            /// <param name="BusinessIdentifier">總機構統一編號，如無總機構請填入銷售店統一編號</param>
            /// <param name="productArray">單項商品資訊</param>
            /// <param name="AESKey">加解密金鑰(QR種子密碼)</param>
            /// <returns></returns>
            public string QRCodeINV(string InvoiceNumber,
                string InvoiceDate,
                string InvoiceTime,
                string RandomNumber,
                decimal SalesAmount,
                decimal TaxAmount,
                decimal TotalAmount,
                string BuyerIdentifier,
                string RepresentIdentifier,
                string SellerIdentifier,
                string BusinessIdentifier,
                string[][] productArray,
                string AESKey)
            {
                try
                {
                    this.inputValidate(InvoiceNumber,
                        InvoiceDate,
                        InvoiceTime,
                        RandomNumber,
                        SalesAmount,
                        TaxAmount,
                        TotalAmount,
                        BuyerIdentifier,
                        RepresentIdentifier,
                        SellerIdentifier,
                        BusinessIdentifier,
                        productArray,
                        AESKey);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                return ((InvoiceNumber +
                    InvoiceDate +
                    RandomNumber +
                    Convert.ToInt32(SalesAmount).ToString("x8") +
                    Convert.ToInt32(TotalAmount).ToString("x8") +
                    BuyerIdentifier + SellerIdentifier) +
                    this.AESEncrypt(InvoiceNumber + RandomNumber, AESKey).PadRight(0x18));
            }
        }
    }

    public static class StringExtensions
    {
        public static string SqlQuote(this String str)
        {
            return str.Replace("'", "''");
        }

        public static string AdjPathByOS(this String str)
        {
            string strR = str;
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                strR = str.Replace("/", @"\");
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                strR = str.Replace(@"\", "/");
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
                strR = str.Replace(@"\", "/");
            return strR;
        }


        public static string CrLf(this String str)
        {
            if (str == null)
                str = "\r\n";
            return str + "\r\n";
        }

    }

    public class iXmsClient
    {
        public static string ApiUrl;
        public static System.Data.DataSet LockedCode(System.Data.DataSet ds, UserInfo vUserInfo)
        {
            if (ApiUrl.ToLower().IndexOf("https:") == 0)
                SSLValidator.OverrideValidation();
            iXmsApiParameter AP = new iXmsApiParameter();
            AP.user = vUserInfo;
            AP.Method = "LockedCode";
            AP.ObjName = "";
            AP.UnknowName = "";
            string strPara = PubUtility.GetSerString(AP, typeof(iXmsApiParameter), "api");

            Uri aUri = new Uri(ApiUrl);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(aUri);
            httpWebRequest.Headers.Add("ParaKey", strPara);
            //httpWebRequest.Headers.Add("apikeysecret", ApiPara.apikeysecret);
            //httpWebRequest.Headers.Add("Proxy-Authorization", ApiPara.Proxy_Authorization);
            httpWebRequest.ContentType = "text/xml";
            httpWebRequest.Accept = "text/xml";
            httpWebRequest.Method = "POST";
            //string strReturn = "";
            ds.WriteXml(httpWebRequest.GetRequestStream());
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            DataSet dsR = null;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                //strReturn = streamReader.ReadToEnd();
                dsR = new DataSet();
                dsR.ReadXml(streamReader);
                //AR.Msg_Code = strReturn;
            }
            return dsR;
        }
        public static System.Data.DataSet UpdateUPWD(System.Data.DataSet ds, UserInfo vUserInfo)
        {
            if (ApiUrl.ToLower().IndexOf("https:") == 0)
                SSLValidator.OverrideValidation();
            iXmsApiParameter AP = new iXmsApiParameter();
            AP.user = vUserInfo;
            AP.Method = "UpdateUPWD";
            AP.ObjName = "";
            AP.UnknowName = "";
            string strPara = PubUtility.GetSerString(AP, typeof(iXmsApiParameter), "api");

            Uri aUri = new Uri(ApiUrl);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(aUri);
            httpWebRequest.Headers.Add("ParaKey", strPara);
            //httpWebRequest.Headers.Add("apikeysecret", ApiPara.apikeysecret);
            //httpWebRequest.Headers.Add("Proxy-Authorization", ApiPara.Proxy_Authorization);
            httpWebRequest.ContentType = "text/xml";
            httpWebRequest.Accept = "text/xml";
            httpWebRequest.Method = "POST";
            //string strReturn = "";
            ds.WriteXml(httpWebRequest.GetRequestStream());
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            DataSet dsR = null;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                //strReturn = streamReader.ReadToEnd();
                dsR = new DataSet();
                dsR.ReadXml(streamReader);
                //AR.Msg_Code = strReturn;
            }
            return dsR;
        }
    }

    public static class SSLValidator
    {
        private static bool OnValidateCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain,
                                                    System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        public static void OverrideValidation()
        {
            System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072;
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                OnValidateCertificate;
            System.Net.ServicePointManager.Expect100Continue = true;
        }
    }
}
