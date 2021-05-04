using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

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
            HtmlAgilityPack.HtmlDocument doc1 = LoadHtmlDoc("login.html");
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

            PubUtility.AppendScriptAtBodyEnd(doc1, "lib/jQuery-File-Upload-10.2.0/vendor/jquery.ui.widget.js");
            PubUtility.AppendScriptAtBodyEnd(doc1, "lib/jQuery-File-Upload-10.2.0/jquery.iframe-transport.js");
            PubUtility.AppendScriptAtBodyEnd(doc1, "lib/jQuery-File-Upload-10.2.0/jquery.fileupload.js");
            PubUtility.AppendScriptAtBodyEnd(doc1, "lib/jquery-ui-1.11.4/jquery-ui.min.js");
            PubUtility.AppendScriptAtBodyEnd(doc1, "js/JSUtility.js");
            PubUtility.AppendScriptAtBodyEnd(doc1, "Menu.js");
            //PubUtility.AppendScriptAtBodyEnd(doc1, "js/custom.js");


            PubUtility.SetCssVer(doc1, "css/custom.css");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            string strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }


        //
        [Route("SystemSetup/GMMacPLUSet")]
        public IActionResult GMMacPLUSet()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\GMMacPLUSet.html".AdjPathByOS());
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
                 "//table[@id='tbGMMacPLUSet']/tbody",
                 "//table[@id='tbSecurity']/tbody"
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


        [Route("test")]
        public IActionResult test()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\test.html".AdjPathByOS());
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
                 "//table[@id='tbtest']/tbody",
                 "//table[@id='tbSecurity']/tbody"
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


        //2021-04-28
        [Route("Inv")]
        public IActionResult Inv()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\Inv.html".AdjPathByOS());
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
                 "//table[@id='tbInv']/tbody",
                 "//table[@id='tbSecurity']/tbody"
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
