using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace SVMAdmin.Controllers
{
    [Route("")]
    public class HtmlControllerSa : Controller
    {

        [Route("AIReports/VSA21P")]
        public IActionResult VSA21P()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\AIReports\VSA21P.html".AdjPathByOS());
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
                 //"//ul[contains(@class,'app-menu')]",
                 "//table[@id='tbVSA21P']/tbody"
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

            ndh = doc1.DocumentNode.SelectSingleNode("//body");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }


        [Route("AIReports/VSA21_7P")]
        public IActionResult VSA21_7P()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\AIReports\VSA21_7P.html".AdjPathByOS());
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
                 //"//ul[contains(@class,'app-menu')]",
                 "//table[@id='tbVSA21_7P']/tbody"
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

            ndh = doc1.DocumentNode.SelectSingleNode("//body");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            doc1.Save(ms);
            strHtml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return Content(strHtml, "text/html", System.Text.Encoding.UTF8);
        }


        [Route("AIReports/VSA76_1P")]
        public IActionResult VSA76_1P()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\AIReports\VSA76_1P.html".AdjPathByOS());
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
                 "//table[@id='tbVSA76_1P']/tbody"
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

        
        [Route("AIReports/VSA76P")]
        public IActionResult VSA76P()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\AIReports\VSA76P.html".AdjPathByOS());
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
                 "//table[@id='tbVSA76P']/tbody"
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

       
        [Route("AIReports/VSA73P")]
        public IActionResult VSA73P()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\AIReports\VSA73P.html".AdjPathByOS());
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
                 "//table[@id='tbVSA73P']/tbody"
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

        
        [Route("AIReports/VSA73_1P")]
        public IActionResult VSA73_1P()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\AIReports\VSA73_1P.html".AdjPathByOS());
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
                 "//table[@id='tbVSA73_1P']/tbody"
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








    }





}
