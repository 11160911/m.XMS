using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace SVMAdmin.Controllers
{
    [Route("")]
    public class HtmlControllerInv : Controller
    {


        //2021-05-21 Larry
        [Route("VIN13_2")]
        public IActionResult VIN13_2()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\INV\VIN13_2.html".AdjPathByOS());
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
                 "//table[@id='tbVIN13_2']/tbody"
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


        //Larry 2021/06/15
        [Route("VIN14_3")]
        public IActionResult VIN14_3()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\INV\VIN14_3.html".AdjPathByOS());
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
                 "//table[@id='tbVIN14_3']/tbody"
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



        //Larry 2021/06/28
        [Route("VIN14_4")]
        public IActionResult VIN14_4()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\INV\VIN14_4.html".AdjPathByOS());
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
                 "//table[@id='tbVIN14_4']/tbody"
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



        //Larry 2021/06/28
        [Route("VIN14_5")]
        public IActionResult VIN14_5()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\INV\VIN14_5.html".AdjPathByOS());
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
                 "//table[@id='tbVIN14_5']/tbody"
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



        //Larry 2021/06/28
        [Route("VIN47")]
        public IActionResult VIN47()
        {
            HtmlAgilityPack.HtmlDocument doc1 = new HtmlAgilityPack.HtmlDocument();
            string strHtml = System.IO.File.ReadAllText(ConstList.HostEnvironment.WebRootPath + @"\INV\VIN47.html".AdjPathByOS());
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
                 "//table[@id='tbVIN47']/tbody"
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
