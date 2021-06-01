using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace SVMAdmin.Controllers
{
    [Route("")]
    [ApiController]
    public class HtmlController2 : Controller
    {


        [Route("SystemSetup/MMMachineSet")]
        public IActionResult MMMachineSet()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\MMMachineSet.html".AdjPathByOS());
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
                 "//table[@id='tbMMMachineRack']/tbody",
                 "//table[@id='tbMMMachineSet']/tbody"
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

        [Route("SystemSetup/GMInvPLUSet")]
        public IActionResult GMInvPLUSet()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\SystemSetup\GMInvPLUSet.html".AdjPathByOS());
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
                 //"//table[@id='tbMMMachineRack']/tbody",
                 //"//table[@id='tbMMMachineSet']/tbody"
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

        [Route("AIReports/VSA04P")]
        public IActionResult VSA04P()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\AIReports\VSA04P.html".AdjPathByOS());
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
                 "//table[@id='tbVSA04P']/tbody"
                 //"//table[@id='tbMMMachineSet']/tbody"
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



    }
}
