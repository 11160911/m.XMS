using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace SVMAdmin.Controllers
{
    [Route("")]
    public class HtmlController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HtmlController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [Route("Login")]
        public IActionResult Login()
        {
            IQueryCollection rq = HttpContext.Request.Query;
            string BeforeCompanyID = rq["company"];
            string CompanyID = PubUtility.enCode170215(BeforeCompanyID);

            HtmlAgilityPack.HtmlDocument doc1 = LoadHtmlDoc("login2.html");
            string [] NodeRemove = new string[] {
                "//input[@id='CompanyID']"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].SetAttributeValue("value",CompanyID);

                }
            }
            //PubUtility.SetCssVer(doc1, "css/custom.css");
            //PubUtility.SetScriptVer(doc1, "lib/bootstrap/dist/js/bootstrap.bundle.min.js");
            PubUtility.AppendScriptAtBodyEnd(doc1, "js/JSUtility.js");
            PubUtility.AppendScriptAtBodyEnd(doc1, "Login.js");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            string strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }

        [Route("Menu")]
        public IActionResult Menu()
        {
            HtmlAgilityPack.HtmlDocument doc1 = LoadHtmlDoc("Menu.html");

            string[] NodeRemove = new string[] {
                 "//div[@id='sidebar-menu']//ul[contains(@class,'side-menu')]" 
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].RemoveAllChildren();
                }
            }
            NodeRemove = new string[] {
                "//script[@src='js/custom.min.js']"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].Remove();
                }
            }
            PubUtility.AppendCssAtHeadEnd(doc1, "lib/jsTree/themes/default/style.min.css");

            PubUtility.AppendScriptAtBodyEnd(doc1, "lib/jQuery-File-Upload-10.2.0/vendor/jquery.ui.widget.js");
            PubUtility.AppendScriptAtBodyEnd(doc1, "lib/jQuery-File-Upload-10.2.0/jquery.iframe-transport.js");
            PubUtility.AppendScriptAtBodyEnd(doc1, "lib/jQuery-File-Upload-10.2.0/jquery.fileupload.js");
            PubUtility.AppendScriptAtBodyEnd(doc1, "lib/jquery-ui-1.11.4/jquery-ui.min.js");
            PubUtility.AppendScriptAtBodyEnd(doc1, "lib/jsTree/jstree.min.js");
            PubUtility.AppendScriptAtBodyEnd(doc1, "js/JSUtility.js");
            PubUtility.AppendScriptAtBodyEnd(doc1, "build/ckeditor.js");
            //PubUtility.AppendScriptAtBodyEnd(doc1, "ckeditor5-master/packages/ckeditor5-build-classic/build/ckeditor.js");
            PubUtility.AppendScriptAtBodyEnd(doc1, "Menu.js");

            PubUtility.SetCssVer(doc1, "css/custom.css");
            PubUtility.AppendCssAtHeadEnd(doc1, "css/DyForm3.css");
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            string strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }
       
        [Route("SystemSetup/MSSA101")]
        public IActionResult MSSA101()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\MSSA101.html".AdjPathByOS());
            doc1.LoadHtml(strHtml);

            //Remove Node
            string[] NodeRemove = new string[] {
                "//script",
                "//link"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].Remove();
                }
            }

            //RemoveAllChildren
            NodeRemove = new string[] {
                 "//ul[contains(@class,'app-menu')]",
                 "//table[@id='tbMSSA101Mod']/tbody"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].RemoveAllChildren();
                }

            }

            HtmlAgilityPack.HtmlNode ndh = doc1.DocumentNode.SelectSingleNode("//head");
            //PubUtility.AppendCss(ndh, "css/main.css");
            //PubUtility.AppendCss(ndh, "css/font-awesome.css");


            ndh = doc1.DocumentNode.SelectSingleNode("//body");
            //PubUtility.AppendScriptAtBodyEnd(doc1, "SystemSetup/GMMacPLUSet.js");


            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }

        [Route("SystemSetup/MSDM101")]
        public IActionResult MSDM101()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\MSDM101.html".AdjPathByOS());
            doc1.LoadHtml(strHtml);

            //Remove Node
            string[] NodeRemove = new string[] {
                "//script",
                "//link"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].Remove();
                }
            }

            //RemoveAllChildren
            NodeRemove = new string[] {
                 "//ul[contains(@class,'app-menu')]",
                 "//table[@id='tbMSDM101Mod']/tbody"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].RemoveAllChildren();
                }

            }

            HtmlAgilityPack.HtmlNode ndh = doc1.DocumentNode.SelectSingleNode("//head");
            //PubUtility.AppendCss(ndh, "css/main.css");
            //PubUtility.AppendCss(ndh, "css/font-awesome.css");


            ndh = doc1.DocumentNode.SelectSingleNode("//body");
            //PubUtility.AppendScriptAtBodyEnd(doc1, "SystemSetup/GMMacPLUSet.js");


            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }

        [Route("SystemSetup/MSPV102")]
        public IActionResult MSPV102()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\MSPV102.html".AdjPathByOS());
            doc1.LoadHtml(strHtml);

            //Remove Node
            string[] NodeRemove = new string[] {
                "//script",
                "//link"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].Remove();
                }
            }

            //RemoveAllChildren
            NodeRemove = new string[] {
                 "//ul[contains(@class,'app-menu')]",
                 "//table[@id='tbmsPV102']/tbody"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].RemoveAllChildren();
                }

            }

            HtmlAgilityPack.HtmlNode ndh = doc1.DocumentNode.SelectSingleNode("//head");
            //PubUtility.AppendCss(ndh, "css/main.css");
            //PubUtility.AppendCss(ndh, "css/font-awesome.css");


            ndh = doc1.DocumentNode.SelectSingleNode("//body");
            //PubUtility.AppendScriptAtBodyEnd(doc1, "SystemSetup/GMMacPLUSet.js");


            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }

        [Route("SystemSetup/MSDM104")]
        public IActionResult MSDM104()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\MSDM104.html".AdjPathByOS());
            doc1.LoadHtml(strHtml);

            //Remove Node
            string[] NodeRemove = new string[] {
                "//script",
                "//link"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].Remove();
                }
            }

            //RemoveAllChildren
            NodeRemove = new string[] {
                 "//ul[contains(@class,'app-menu')]",
                 "//table[@id='tbMSDM104Mod']/tbody"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].RemoveAllChildren();
                }

            }

            HtmlAgilityPack.HtmlNode ndh = doc1.DocumentNode.SelectSingleNode("//head");
            //PubUtility.AppendCss(ndh, "css/main.css");
            //PubUtility.AppendCss(ndh, "css/font-awesome.css");
            ndh = doc1.DocumentNode.SelectSingleNode("//body");
            //PubUtility.AppendCssAtHeadEnd(doc1, "../css/DyForm3.css");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }

        [Route("SystemSetup/MSDM106")]
        public IActionResult MSDM106()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\MSDM106.html".AdjPathByOS());
            doc1.LoadHtml(strHtml);

            //Remove Node
            string[] NodeRemove = new string[] {
                "//script",
                "//link"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].Remove();
                }
            }

            //RemoveAllChildren
            NodeRemove = new string[] {
                 "//ul[contains(@class,'app-menu')]",
                 "//table[@id='tbMSDM106Mod']/tbody"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].RemoveAllChildren();
                }

            }

            HtmlAgilityPack.HtmlNode ndh = doc1.DocumentNode.SelectSingleNode("//head");
            //PubUtility.AppendCss(ndh, "css/main.css");
            //PubUtility.AppendCss(ndh, "css/font-awesome.css");
            ndh = doc1.DocumentNode.SelectSingleNode("//body");
            //PubUtility.AppendCssAtHeadEnd(doc1, "../css/DyForm3.css");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }

        [Route("SystemSetup/MSPV101")]
        public IActionResult MSPV101()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\MSPV101.html".AdjPathByOS());
            doc1.LoadHtml(strHtml);

            //Remove Node
            string[] NodeRemove = new string[] {
                "//script",
                "//link"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].Remove();
                }
            }

            //RemoveAllChildren
            NodeRemove = new string[] {
                 "//ul[contains(@class,'app-menu')]",
                 "//table[@id='tbMSPV101Mod']/tbody"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].RemoveAllChildren();
                }

            }

            HtmlAgilityPack.HtmlNode ndh = doc1.DocumentNode.SelectSingleNode("//head");
            //PubUtility.AppendCss(ndh, "css/main.css");
            //PubUtility.AppendCss(ndh, "css/font-awesome.css");
            ndh = doc1.DocumentNode.SelectSingleNode("//body");
            //PubUtility.AppendCssAtHeadEnd(doc1, "../css/DyForm3.css");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }
        [Route("SystemSetup/MSSD101")]
        public IActionResult MSSD101()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\MSSD101.html".AdjPathByOS());
            doc1.LoadHtml(strHtml);

            //Remove Node
            string[] NodeRemove = new string[] {
                "//script",
                "//link"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].Remove();
                }
            }

            //RemoveAllChildren
            NodeRemove = new string[] {
                 "//ul[contains(@class,'app-menu')]",
                 "//table[@id='tbMSSD101Mod']/tbody"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].RemoveAllChildren();
                }

            }

            HtmlAgilityPack.HtmlNode ndh = doc1.DocumentNode.SelectSingleNode("//head");
            //PubUtility.AppendCss(ndh, "css/main.css");
            //PubUtility.AppendCss(ndh, "css/font-awesome.css");
            ndh = doc1.DocumentNode.SelectSingleNode("//body");
            //PubUtility.AppendCssAtHeadEnd(doc1, "../css/DyForm3.css");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }

        [Route("SystemSetup/MSSD102")]
        public IActionResult MSSD102()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\MSSD102.html".AdjPathByOS());
            doc1.LoadHtml(strHtml);

            //Remove Node
            string[] NodeRemove = new string[] {
                "//script",
                "//link"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].Remove();
                }
            }

            //RemoveAllChildren
            NodeRemove = new string[] {
                 "//ul[contains(@class,'app-menu')]",
                 "//table[@id='tbMSSD102Mod']/tbody"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].RemoveAllChildren();
                }

            }

            HtmlAgilityPack.HtmlNode ndh = doc1.DocumentNode.SelectSingleNode("//head");
            //PubUtility.AppendCss(ndh, "css/main.css");
            //PubUtility.AppendCss(ndh, "css/font-awesome.css");
            ndh = doc1.DocumentNode.SelectSingleNode("//body");
            //PubUtility.AppendCssAtHeadEnd(doc1, "../css/DyForm3.css");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }

        [Route("SystemSetup/MSSD105")]
        public IActionResult MSSD105()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\MSSD105.html".AdjPathByOS());
            doc1.LoadHtml(strHtml);

            //Remove Node
            string[] NodeRemove = new string[] {
                "//script",
                "//link"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].Remove();
                }
            }

            //RemoveAllChildren
            NodeRemove = new string[] {
                 "//ul[contains(@class,'app-menu')]",
                 "//table[@id='tbMSSD105Mod']/tbody"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].RemoveAllChildren();
                }

            }

            HtmlAgilityPack.HtmlNode ndh = doc1.DocumentNode.SelectSingleNode("//head");
            //PubUtility.AppendCss(ndh, "css/main.css");
            //PubUtility.AppendCss(ndh, "css/font-awesome.css");
            ndh = doc1.DocumentNode.SelectSingleNode("//body");
            //PubUtility.AppendCssAtHeadEnd(doc1, "../css/DyForm3.css");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }

        [Route("SystemSetup/MSSD106")]
        public IActionResult MSSD106()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\MSSD106.html".AdjPathByOS());
            doc1.LoadHtml(strHtml);

            //Remove Node
            string[] NodeRemove = new string[] {
                "//script",
                "//link"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].Remove();
                }
            }

            //RemoveAllChildren
            NodeRemove = new string[] {
                 "//ul[contains(@class,'app-menu')]",
                 "//table[@id='tbMSSD106Mod']/tbody"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].RemoveAllChildren();
                }

            }

            HtmlAgilityPack.HtmlNode ndh = doc1.DocumentNode.SelectSingleNode("//head");
            //PubUtility.AppendCss(ndh, "css/main.css");
            //PubUtility.AppendCss(ndh, "css/font-awesome.css");
            ndh = doc1.DocumentNode.SelectSingleNode("//body");
            //PubUtility.AppendCssAtHeadEnd(doc1, "../css/DyForm3.css");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }

        [Route("SystemSetup/MSDM107")]
        public IActionResult MSDM107()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\MSDM107.html".AdjPathByOS());
            doc1.LoadHtml(strHtml);

            //Remove Node
            string[] NodeRemove = new string[] {
                "//script",
                "//link"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].Remove();
                }
            }

            //RemoveAllChildren
            NodeRemove = new string[] {
                 "//ul[contains(@class,'app-menu')]",
                 "//table[@id='tbMSDM107Mod']/tbody"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].RemoveAllChildren();
                }

            }

            HtmlAgilityPack.HtmlNode ndh = doc1.DocumentNode.SelectSingleNode("//head");
            //PubUtility.AppendCss(ndh, "css/main.css");
            //PubUtility.AppendCss(ndh, "css/font-awesome.css");
            ndh = doc1.DocumentNode.SelectSingleNode("//body");
            //PubUtility.AppendCssAtHeadEnd(doc1, "../css/DyForm3.css");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }

        [Route("SystemSetup/MSVP101")]
        public IActionResult MSVP101()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\MSVP101.html".AdjPathByOS());
            doc1.LoadHtml(strHtml);

            //Remove Node
            string[] NodeRemove = new string[] {
                "//script",
                "//link"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].Remove();
                }
            }

            //RemoveAllChildren
            NodeRemove = new string[] {
                 "//ul[contains(@class,'app-menu')]",
                 "//table[@id='tbMSVP101Mod']/tbody"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].RemoveAllChildren();
                }

            }

            HtmlAgilityPack.HtmlNode ndh = doc1.DocumentNode.SelectSingleNode("//head");
            //PubUtility.AppendCss(ndh, "css/main.css");
            //PubUtility.AppendCss(ndh, "css/font-awesome.css");
            ndh = doc1.DocumentNode.SelectSingleNode("//body");
            //PubUtility.AppendCssAtHeadEnd(doc1, "../css/DyForm3.css");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }

        [Route("SystemSetup/MSVP102")]
        public IActionResult MSVP102()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\MSVP102.html".AdjPathByOS());
            doc1.LoadHtml(strHtml);

            //Remove Node
            string[] NodeRemove = new string[] {
                "//script",
                "//link"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].Remove();
                }
            }

            //RemoveAllChildren
            NodeRemove = new string[] {
                 "//ul[contains(@class,'app-menu')]",
                 "//table[@id='tbMSVP102Mod']/tbody"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].RemoveAllChildren();
                }

            }

            HtmlAgilityPack.HtmlNode ndh = doc1.DocumentNode.SelectSingleNode("//head");
            //PubUtility.AppendCss(ndh, "css/main.css");
            //PubUtility.AppendCss(ndh, "css/font-awesome.css");
            ndh = doc1.DocumentNode.SelectSingleNode("//body");
            //PubUtility.AppendCssAtHeadEnd(doc1, "../css/DyForm3.css");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }
        
        [Route("SystemSetup/MSSA107")]
        public IActionResult MSSA107()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\MSSA107.html".AdjPathByOS());
            doc1.LoadHtml(strHtml);

            //Remove Node
            string[] NodeRemove = new string[] {
                "//script",
                "//link"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].Remove();
                }
            }

            //RemoveAllChildren
            NodeRemove = new string[] {
                 "//ul[contains(@class,'app-menu')]",
                 //"//table[@id='tbMSSD106Mod']/tbody"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].RemoveAllChildren();
                }

            }

            HtmlAgilityPack.HtmlNode ndh = doc1.DocumentNode.SelectSingleNode("//head");
            //PubUtility.AppendCss(ndh, "css/main.css");
            //PubUtility.AppendCss(ndh, "css/font-awesome.css");
            ndh = doc1.DocumentNode.SelectSingleNode("//body");
            //PubUtility.AppendCssAtHeadEnd(doc1, "../css/DyForm3.css");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }

        [Route("SystemSetup/MSSD104")]
        public IActionResult MSSD104()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\MSSD104.html".AdjPathByOS());
            doc1.LoadHtml(strHtml);

            //Remove Node
            string[] NodeRemove = new string[] {
                "//script",
                "//link"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].Remove();
                }
            }

            //RemoveAllChildren
            NodeRemove = new string[] {
                 "//ul[contains(@class,'app-menu')]",
                 "//table[@id='tbMSSD104Mod']/tbody"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].RemoveAllChildren();
                }

            }

            HtmlAgilityPack.HtmlNode ndh = doc1.DocumentNode.SelectSingleNode("//head");
            //PubUtility.AppendCss(ndh, "css/main.css");
            //PubUtility.AppendCss(ndh, "css/font-awesome.css");
            ndh = doc1.DocumentNode.SelectSingleNode("//body");
            //PubUtility.AppendCssAtHeadEnd(doc1, "../css/DyForm3.css");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }

        [Route("SystemSetup/MSSA103")]
        public IActionResult MSSA103()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\MSSA103.html".AdjPathByOS());
            doc1.LoadHtml(strHtml);

            //Remove Node
            string[] NodeRemove = new string[] {
                "//script",
                "//link"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].Remove();
                }
            }

            //RemoveAllChildren
            NodeRemove = new string[] {
                 "//ul[contains(@class,'app-menu')]",
                 "//table[@id='tbMSSA103Mod']/tbody"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].RemoveAllChildren();
                }

            }

            HtmlAgilityPack.HtmlNode ndh = doc1.DocumentNode.SelectSingleNode("//head");
            //PubUtility.AppendCss(ndh, "css/main.css");
            //PubUtility.AppendCss(ndh, "css/font-awesome.css");
            ndh = doc1.DocumentNode.SelectSingleNode("//body");
            //PubUtility.AppendCssAtHeadEnd(doc1, "../css/DyForm3.css");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }

        [Route("SystemSetup/MSSA108")]
        public IActionResult MSSA108()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\MSSA108.html".AdjPathByOS());
            doc1.LoadHtml(strHtml);

            //Remove Node
            string[] NodeRemove = new string[] {
                "//script",
                "//link"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].Remove();
                }
            }

            //RemoveAllChildren
            NodeRemove = new string[] {
                 "//ul[contains(@class,'app-menu')]",
                 "//table[@id='tbMSSA108Mod']/tbody"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].RemoveAllChildren();
                }

            }

            HtmlAgilityPack.HtmlNode ndh = doc1.DocumentNode.SelectSingleNode("//head");
            //PubUtility.AppendCss(ndh, "css/main.css");
            //PubUtility.AppendCss(ndh, "css/font-awesome.css");
            ndh = doc1.DocumentNode.SelectSingleNode("//body");
            //PubUtility.AppendCssAtHeadEnd(doc1, "../css/DyForm3.css");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }

        [Route("SystemSetup/MSSA106")]
        public IActionResult MSSA106()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\MSSA106.html".AdjPathByOS());
            doc1.LoadHtml(strHtml);

            //Remove Node
            string[] NodeRemove = new string[] {
                "//script",
                "//link"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].Remove();
                }
            }

            //RemoveAllChildren
            NodeRemove = new string[] {
                 "//ul[contains(@class,'app-menu')]",
                 //"//table[@id='tbMSSD106Mod']/tbody"
            };
            for (int i = 0; i < NodeRemove.Length; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection ndm = doc1.DocumentNode.SelectNodes(NodeRemove[i]);
                if (ndm != null)
                {
                    for (int j = 0; j < ndm.Count; j++)
                        ndm[j].RemoveAllChildren();
                }

            }

            HtmlAgilityPack.HtmlNode ndh = doc1.DocumentNode.SelectSingleNode("//head");
            //PubUtility.AppendCss(ndh, "css/main.css");
            //PubUtility.AppendCss(ndh, "css/font-awesome.css");
            ndh = doc1.DocumentNode.SelectSingleNode("//body");
            //PubUtility.AppendCssAtHeadEnd(doc1, "../css/DyForm3.css");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }

        private HtmlAgilityPack.HtmlDocument LoadHtmlDoc(string FileOnWebRoot)
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string file = _hostingEnvironment.WebRootPath + @"\" + FileOnWebRoot;
            string strHtml = System.IO.File.ReadAllText(file.AdjPathByOS());
            doc1.LoadHtml(strHtml);
            return doc1;
        }

    }





}
