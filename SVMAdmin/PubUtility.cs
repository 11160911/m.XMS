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

namespace SVMAdmin
{
    public static class PubUtility
    {
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
            ds.WriteXml(ms);
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

            //權限管理,智販機管理,商品管理,營運管理,營運分析
            dt.Rows.Add(new object[] { "權限管理", "Authorize", "SysUsers", "使用者維護", "SysUsers", "P", "fa-cog" });
            dt.Rows.Add(new object[] { "權限管理", "Authorize", "SysSecurity", "角色權限維護", "SysSecurity", "P", "fa-cog" });
            dt.Rows.Add(new object[] { "權限管理", "Authorize", "SysChangePWD", "密碼變更作業", "SysChangePWD", "P", "fa-cog" });

            dt.Rows.Add(new object[] { "智販機管理", "MachineManage", "MMMachineSet", "智販機設定", "MMMachineSet", "P", "fa-cogs" });

            dt.Rows.Add(new object[] { "商品管理", "GoodsManage", "Inv", "庫存查詢", "Inv", "P", "fa-cubes" });
            dt.Rows.Add(new object[] { "商品管理", "GoodsManage", "GMMacPLUSet", "智販機商品設定", "GMMacPLUSet", "P", "fa-cubes" });
            dt.Rows.Add(new object[] { "商品管理", "GoodsManage", "GMInvPLUSet", "貨倉商品設定", "GMInvPLUSet", "P", "fa-cubes" });
            dt.Rows.Add(new object[] { "商品管理", "GoodsManage", "GMInvoiceNoSet", "發票分配(店+機)", "GMInvoiceNoSet", "P", "fa-cubes" });
            dt.Rows.Add(new object[] { "商品管理", "GoodsManage", "GMPicVdoSet", "圖片/影片設定", "GMPicVdoSet", "P", "fa-cubes" });
            dt.Rows.Add(new object[] { "商品管理", "GoodsManage", "GMMacType", "智販機類別設定", "GMMacType", "P", "fa-cubes" });

            dt.Rows.Add(new object[] { "營運管理", "OperatManage", "OMPick", "商品撿貨設定", "OMPick", "P", "fa-th-large" });
            dt.Rows.Add(new object[] { "營運管理", "OperatManage", "OMFill", "商品補貨設定", "OMFill", "P", "fa-th-large" });
            dt.Rows.Add(new object[] { "營運管理", "OperatManage", "OMReturn", "商品退貨設定", "OMReturn", "P", "fa-th-large" });
            dt.Rows.Add(new object[] { "營運管理", "OperatManage", "OMScrap", "商品報廢設定", "OMScrap", "P", "fa-th-large" });
            dt.Rows.Add(new object[] { "營運管理", "OperatManage", "OMChange", "商品換貨設定", "OMChange", "P", "fa-th-large" });
            dt.Rows.Add(new object[] { "營運管理", "OperatManage", "OMAdjust", "商品調整設定", "OMAdjust", "P", "fa-th-large" });
            dt.Rows.Add(new object[] { "營運管理", "OperatManage", "OMChangeST", "商品換店設定", "OMChangeST", "P", "fa-th-large" });
            dt.Rows.Add(new object[] { "營運管理", "OperatManage", "OMChangeGood", "換貨設定", "OMChangeGood", "P", "fa-th-large" });
            dt.Rows.Add(new object[] { "營運管理", "OperatManage", "OMChangeST", "換店設定", "OMChangeST", "P", "fa-th-large" });

            dt.Rows.Add(new object[] { "營運分析", "SaleAnalysis", "SAReport1", "區域別銷售分析", "SAReport1", "P", "fa-file-excel-o" });
            dt.Rows.Add(new object[] { "營運分析", "SaleAnalysis", "SAReport2", "商品別銷售分析", "SAReport2", "P", "fa-file-excel-o" });

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
