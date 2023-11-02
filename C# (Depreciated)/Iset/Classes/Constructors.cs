using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Iset
{
    class Templates
    {
        public static Dictionary<string, string> getServers()
        {
            Dictionary<string, string> servers = new Dictionary<string, string>();

            /*cd bin
    start Executer.exe UnifiedNetwork.dll UnifiedNetwork.LocationService.LocationService StartService LocationService 42
    start Executer.exe AdminClientServiceCore.dll AdminClientServiceCore.AdminClientService StartService AdminService 127.0.0.1 42
    start Executer.exe FrontendServiceCore.dll FrontendServiceCore.FrontendService StartService FrontendService 127.0.0.1 42
    start Executer.exe CashShopService.dll CashShopService.CashShopService StartService CashShopService 127.0.0.1 42
    start Executer.exe RankService.dll RankService.RankService StartService RankService 127.0.0.1 42
    start Executer.exe GuildService.dll GuildService.GuildService StartService GuildService 127.0.0.1 42
    start Executer.exe PvpService.dll PvpService.PvpService StartService PvpService 127.0.0.1 42
    start Executer.exe LoginServiceCore.dll LoginServiceCore.LoginService StartService LoginService 127.0.0.1 42
    start Executer.exe MicroPlayServiceCore.dll MicroPlayServiceCore.MicroPlayService StartService MIcroPlayService 127.0.0.1 42
    start Executer.exe MMOChannelService.dll MMOChannelService.MMOChannelService StartService MMOChannelService 127.0.0.1 42
    start Executer.exe PlayerService.dll PlayerService.PlayerService StartService PlayerService 127.0.0.1 42
    start Executer.exe DSService.dll DSService.DSService StartService DSService 127.0.0.1 42
    start Executer.exe PingService.dll PingServiceCore.PingService StartService PingService 127.0.0.1 42
    start Executer.exe UserDSHostService.dll UserDSHostService.UserDSHostService StartService UserDSHostService 127.0.0.1 42*/
            return servers;
        }

        public static string generateWeaponCode(Constructors.CharacterItem itm)
        {
            string retstr = "```";
            retstr = retstr + "Item Class: " + itm.ItemClass + Environment.NewLine;
            retstr = retstr + "Item Class: " + itm.ItemClass + Environment.NewLine;
            retstr = retstr + "```";
            return null;
        }

        /*internal static void ConvertHtmlToImage()
        {
            Bitmap m_Bitmap = new Bitmap(400, 600);
            PointF point = new PointF(0, 0);
            SizeF maxSize = new System.Drawing.SizeF(500, 500);
            HtmlRenderer.HtmlRender.Render(Graphics.FromImage(m_Bitmap),
                                                    "<html><body><p>This is a shitty html code</p>"
                                                    + "<p>This is another html line</p></body>",
                                                     point, maxSize);

            m_Bitmap.Save(@"C:\Test.png", ImageFormat.Png);
        }*/

        internal static string OnlineTemplate(string online, string town, string dungeon, string parties, string boats, string fishing, string fruit, string arena, string brawl, string relic, string list = null, Dictionary<string, string> partylist = null)
        {
            string template = "```" +
                "Players Online:                  " + online + Environment.NewLine +
                "Players In Town:                 " + town + Environment.NewLine +
                "Players In Dungeon(s):  		 " + dungeon + " (" + parties + " active parties)" + Environment.NewLine +
                "Players On Boat(s):     		 " + boats + Environment.NewLine +
                Environment.NewLine +
                "Players Fishing:                 " + fishing + Environment.NewLine +
                "Players In Fruit Fight:          " + fruit + Environment.NewLine +
                "Players In Arena:                " + arena + Environment.NewLine +
                "Players In Monster Brawl:        " + brawl + Environment.NewLine +
                "Players In Capture The Relic:    " + relic + Environment.NewLine;
            if (list != null)
            {
                template = template + Environment.NewLine + "Online Players:" + Environment.NewLine + list + Environment.NewLine;
            }
            if (partylist != null)
            {
                int partynum = 1;
                foreach (KeyValuePair<string, string> kvp in partylist)
                {
                    int partyMembercount = 0;
                    foreach (string player in kvp.Value.Split(','))
                    {
                        partyMembercount++;
                    }
                    template = template + Environment.NewLine +
                     "Party " + partynum.ToString() + ":" + Environment.NewLine +
                     "        Members (" + partyMembercount.ToString() + "):" + Environment.NewLine +
                     "            " + kvp.Value + Environment.NewLine +
                     Environment.NewLine;
                    partynum++;
                }
            }
            template = template + "```";
            return template;
        }

    }
    class Constructors
    {
        [XmlRoot("Iset")]
        public class CharacterItem
        {
            [XmlAttribute]
            public string ItemID { get; set; }

            [XmlAttribute]
            public string ItemClass { get; set; }

            [XmlAttribute]
            public string Combination { get; set; }

            [XmlAttribute]
            public Tuple<string,string,string> PS0 { get; set; }

            [XmlAttribute]
            public Dictionary<string, string> PS1 { get; set; }

            [XmlAttribute]
            public Dictionary<string, string> PS2 { get; set; }

            [XmlAttribute]
            public Dictionary<string, string> PS3 { get; set; }

            [XmlAttribute]
            public Dictionary<string, string> PS4 { get; set; }

            [XmlAttribute]
            public string BindCount { get; set; }

            [XmlAttribute]
            public string Enhancement { get; set; }

            [XmlAttribute]
            public string Prefix { get; set; }

            [XmlAttribute]
            public string Suffix { get; set; }

            [XmlAttribute]
            public Dictionary<string,string> Quality { get; set; }

            [XmlAttribute]
            public string Look { get; set; }

            [XmlAttribute]
            public bool Restored { get; set; }

            [XmlAttribute]
            public Dictionary<string,string> Infusion { get; set; }
        }
    }
}
