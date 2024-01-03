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

        public static DataTable AllFunction()
        {
            DataTable dt = new DataTable("dtAllFunction");
            dt.Columns.Add("CategoryC", typeof(string));
            dt.Columns.Add("Category", typeof(string));
            dt.Columns.Add("ItemCode", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("Page", typeof(string));
            dt.Columns.Add("MobilePC", typeof(string));
            dt.Columns.Add("Icon", typeof(string));

            dt.Rows.Add(new object[] { "分區盤點", "ISAM", "ISAM01", "分區盤點", "ISAM01", "P", "fa-cogs" });
            dt.Rows.Add(new object[] { "條碼蒐集", "BarcodeCollect", "ISAM02", "條碼蒐集", "ISAM02", "P", "fa-cubes" });
			dt.Rows.Add(new object[] { "出貨/調撥", "OperatManage", "ISAM03", "出貨/調撥", "ISAM03", "P", "fa-th-large" });
            dt.Rows.Add(new object[] { "單價查詢", "PriceRequest", "ISAMQPLU", "單價查詢", "ISAMQPLU", "P", "fa-dollar" });
            dt.Rows.Add(new object[] { "分區盤點上傳", "MachineManage", "ISAMToFTP", "分區盤點上傳", "ISAMToFTP", "P", "fa-cloud-upload" });
            dt.Rows.Add(new object[] { "上傳查詢", "UpRecordQuery", "ISAMQFTPREC", "上傳查詢", "ISAMQFTPREC", "P", "fa-list" });
            dt.Rows.Add(new object[] { "清除作業", "GoodsManage", "ISAMDelData", "清除作業", "ISAMDelData", "P", "fa-trash-o" });
            dt.Rows.Add(new object[] { "店號設定", "ShopManage", "ISAMWHSET", "店號設定", "ISAMWHSET", "P", "fa-desktop" });

            //權限管理,智販機管理,商品管理,營運管理,營運分析
            //dt.Rows.Add(new object[] { "權限管理", "Authorize", "SysUsers", "使用者維護", "SysUsers", "P", "fa-cog" });
            //dt.Rows.Add(new object[] { "權限管理", "Authorize", "VPV01", "系統權限管理", "VPV01", "P", "fa-cog" });
            //dt.Rows.Add(new object[] { "權限管理", "Authorize", "SysChangePWD", "密碼變更作業", "SysChangePWD", "P", "fa-cog" });
            //dt.Rows.Add(new object[] { "功能選單", "MachineManage", "GMMacPLUSet", "盤點作業", "GMMacPLUSet", "P", "fa-cogs" });
            ////dt.Rows.Add(new object[] { "功能選單", "MachineManage", "ISAM01", "盤點作業", "ISAM01", "P", "fa-cogs" });
            //dt.Rows.Add(new object[] { "功能選單", "MachineManage", "ISAM02", "條碼收集", "ISAM02", "P", "fa-cogs" });
            //dt.Rows.Add(new object[] { "功能選單", "MachineManage", "ISAM03", "出貨/調撥", "ISAM03", "P", "fa-cogs" });
            //dt.Rows.Add(new object[] { "功能選單", "MachineManage", "ISAMQPLU", "單價查詢", "ISAMQPLU", "P", "fa-cogs" });
            //dt.Rows.Add(new object[] { "功能選單", "MachineManage", "ISAMTOFTP", "上傳作業", "ISAMTOFTP", "P", "fa-cogs" });
            //dt.Rows.Add(new object[] { "功能選單", "MachineManage", "ISAMDELDATA", "清除作業", "ISAMDELDATA", "P", "fa-cogs" });

            //dt.Rows.Add(new object[] { "商品管理", "GoodsManage", "Inv", "庫存查詢", "Inv", "P", "fa-cubes" });
            //dt.Rows.Add(new object[] { "商品管理", "GoodsManage", "GMMacPLUSet", "智販機商品設定", "GMMacPLUSet", "P", "fa-cubes" });
            //dt.Rows.Add(new object[] { "商品管理", "GoodsManage", "GMInvPLUSet", "貨倉商品設定", "GMInvPLUSet", "P", "fa-cubes" });
            //dt.Rows.Add(new object[] { "商品管理", "GoodsManage", "GMInvoiceNoSet", "發票分配(店+機)", "GMInvoiceNoSet", "P", "fa-cubes" });
            //dt.Rows.Add(new object[] { "商品管理", "GoodsManage", "GMPicVdoSet", "圖片/影片設定", "GMPicVdoSet", "P", "fa-cubes" });
            //dt.Rows.Add(new object[] { "商品管理", "GoodsManage", "GMMacType", "智販機類別設定", "GMMacType", "P", "fa-cubes" });

            //dt.Rows.Add(new object[] { "營運管理", "OperatManage", "VIN13_1", "換店設定", "VIN13_1", "P", "fa-th-large" });
            //dt.Rows.Add(new object[] { "營運管理", "OperatManage", "VIN13_2", "商品換貨設定", "VIN13_2", "P", "fa-th-large" });
            //dt.Rows.Add(new object[] { "營運管理", "OperatManage", "VIN14_1", "商品撿貨作業", "VIN14_1", "P", "fa-th-large" });
            //dt.Rows.Add(new object[] { "營運管理", "OperatManage", "VIN14_2", "商品補貨作業", "VIN14_2", "P", "fa-th-large" });
            //dt.Rows.Add(new object[] { "營運管理", "OperatManage", "VIN14_3", "商品報廢/退貨作業", "VIN14_3", "P", "fa-th-large" });
            //dt.Rows.Add(new object[] { "營運管理", "OperatManage", "OMScrap", "商品作業", "OMScrap", "P", "fa-th-large" });
            //dt.Rows.Add(new object[] { "營運管理", "OperatManage", "VIN14_4", "換店作業", "VIN14_4", "P", "fa-th-large" });
            //dt.Rows.Add(new object[] { "營運管理", "OperatManage", "VIN14_5", "商品換貨作業", "VIN14_5", "P", "fa-th-large" });
            //dt.Rows.Add(new object[] { "營運管理", "OperatManage", "VIN47", "商品調整作業", "VIN47", "P", "fa-th-large" });
            //dt.Rows.Add(new object[] { "營運管理", "OperatManage", "VIN14_1P", "商品撿貨查詢", "VIN14_1P", "P", "fa-th-large" });

            //dt.Rows.Add(new object[] { "營運分析", "SaleAnalysis", "SAReport1", "區域別銷售分析", "SAReport1", "P", "fa-file-excel-o" });
            //dt.Rows.Add(new object[] { "營運分析", "SaleAnalysis", "SAReport2", "商品別銷售分析", "SAReport2", "P", "fa-file-excel-o" });

            //dt.Rows.Add(new object[] { "智能報表", "AIReports", "VSA04P", "交易明細", "VSA04P", "P", "fa-file-excel-o" });
            //dt.Rows.Add(new object[] { "智能報表", "AIReports", "VSA21P", "商品銷售排行", "VSA21P", "P", "fa-file-excel-o" });
            //dt.Rows.Add(new object[] { "智能報表", "AIReports", "VSA21_7P", "區域銷售排行", "VSA21_7P", "P", "fa-file-excel-o" });
            //dt.Rows.Add(new object[] { "智能報表", "AIReports", "VSA76_1P", "即時銷售查詢", "VSA76_1P", "P", "fa-file-excel-o" });
            //dt.Rows.Add(new object[] { "智能報表", "AIReports", "VSA76P", "智販機銷售排行", "VSA76P", "P", "fa-file-excel-o" });
            //dt.Rows.Add(new object[] { "智能報表", "AIReports", "VSA73P", "時段銷售排行", "VSA73P", "P", "fa-file-excel-o" });
            //dt.Rows.Add(new object[] { "智能報表", "AIReports", "VSA73_1P", "7日時段統計", "VSA73_1P", "P", "fa-file-excel-o" });
            return dt.Copy();
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



}
