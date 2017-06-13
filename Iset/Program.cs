using System;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace Iset
{
    class Program
    {
        static void Main(string[] args) => new Program().Start();
        SqlConnection conn;
        private DiscordClient _client;
        IniFile ini = new IniFile(Directory.GetCurrentDirectory() + @"\config.ini");
        public void Start()
        {
            setupConfigBase();
            _client = new DiscordClient();
            char cmd = '!';
            HelpMode hlpmode = HelpMode.Disabled;
            bool AllowMentionPrefix = false;
            if (!String.IsNullOrEmpty(ini.IniReadValue("discord", "PrefixChar")))
            {
                cmd = Convert.ToChar(ini.IniReadValue("discord", "PrefixChar"));
            }
            if (!String.IsNullOrEmpty(ini.IniReadValue("discord", "HelpMode")))
            {
                hlpmode = getHlpMode(ini.IniReadValue("discord", "HelpMode"));
            }
            if (!String.IsNullOrEmpty(ini.IniReadValue("discord", "AllowMentionPrefix")))
            {
                bool.TryParse(ini.IniReadValue("discord", "AllowMentionPrefix"), out AllowMentionPrefix);
            }
            _client.UsingCommands(x =>
            {
                x.PrefixChar = cmd;
                x.HelpMode = hlpmode;
                x.AllowMentionPrefix = AllowMentionPrefix;
            });

            //this is used to read ALL text chat, not just commands with the prefix
            /*_client.MessageReceived += async (s, e) =>
            {
                if (!e.Message.IsAuthor)
                {
                    await e.Channel.SendMessage("you said something that wasnt a command");
                }
            };*/
            _client.GetService<CommandService>().CreateCommand("soaps") //create command greet
                   .Description("clears broken soaps") //add description, it will be shown when ~help is used
                   .Do(async e =>
                   {
                       if (!checkPerms(e.User, "soaps"))
                       {
                           await e.Channel.SendMessage("You do not have permission to use this command!");
                           return;
                       }
                       await e.Channel.SendMessage(clearSoaps());
                   });
            _client.GetService<CommandService>().CreateCommand("clearqueue") //create command greet
                   .Description("clears item queue") //add description, it will be shown when ~help is used
                   .Do(async e =>
                   {
                       if (!checkPerms(e.User, "clearqueue"))
                       {
                           await e.Channel.SendMessage("You do not have permission to use this command!");
                           return;
                       }
                       await e.Channel.SendMessage(clearQueue());
                   });
            _client.GetService<CommandService>().CreateCommand("checkban") //create command greet
                   .Description("checks if a player is banned") //add description, it will be shown when ~help is used
                   .Parameter("username", ParameterType.Required)
                   .Do(async e =>
                   {
                       if (!checkPerms(e.User, "checkban"))
                       {
                           await e.Channel.SendMessage("You do not have permission to use this command!");
                           return;
                       }
                       if (!checkValidInput(e.GetArg("username")))
                       {
                           await e.Channel.SendMessage("Invalid Data.");
                           return;
                       }
                       string msg = "The user " + e.GetArg("username") + " " + isBanned(e.GetArg("username"));
                       await e.Channel.SendMessage(msg);
                   });
            _client.GetService<CommandService>().CreateCommand("restorechar") //create command greet
                  .Description("restores a deleted character") //add description, it will be shown when ~help is used
                  .Parameter("username", ParameterType.Required)
                  .Do(async e =>
                  {
                      if (!checkPerms(e.User, "restorechar"))
                      {
                          await e.Channel.SendMessage("You do not have permission to use this command!");
                          return;
                      }
                      if (!checkValidInput(e.GetArg("username")))
                      {
                          await e.Channel.SendMessage("Invalid Data.");
                          return;
                      }
                      await e.Channel.SendMessage(restoreDeletedCharacter(e.GetArg("username")));
                  });
            _client.GetService<CommandService>().CreateCommand("deletechar") //create command greet
                  .Description("delete a character") //add description, it will be shown when ~help is used
                  .Parameter("username", ParameterType.Required)
                  .Do(async e =>
                  {
                      if (!checkPerms(e.User, "deletechar"))
                      {
                          await e.Channel.SendMessage("You do not have permission to use this command!");
                          return;
                      }
                      if (!checkValidInput(e.GetArg("username")))
                      {
                          await e.Channel.SendMessage("Invalid Data.");
                          return;
                      }
                      await e.Channel.SendMessage(deleteCharacter(e.GetArg("username")));
                  });
            _client.GetService<CommandService>().CreateCommand("setlevel") //create command greet
                  .Description("set the level of a character") //add description, it will be shown when ~help is used
                  .Parameter("username", ParameterType.Required)
                  .Parameter("level", ParameterType.Optional)
                  .Do(async e =>
                  {
                      if (!checkPerms(e.User, "setlevel"))
                      {
                          await e.Channel.SendMessage("You do not have permission to use this command!");
                          return;
                      }
                      if (!checkValidInput(e.GetArg("username")))
                      {
                          await e.Channel.SendMessage("Invalid Data.");
                          return;
                      }
                      int newlevel = 90;
                      if (!String.IsNullOrEmpty(e.GetArg("level")))
                      {
                          if (!int.TryParse(e.GetArg("level"), out newlevel))
                          {
                              await e.Channel.SendMessage("Invalid level given! The value _MUST BE A NUMBER_");
                              return;
                          }
                      }
                      await e.Channel.SendMessage(setCharLevel(e.GetArg("username"), newlevel));
                  });
            _client.GetService<CommandService>().CreateCommand("changename") //create command greet
                  .Description("set the level of a character") //add description, it will be shown when ~help is used
                  .Parameter("oldname", ParameterType.Required)
                  .Parameter("newname", ParameterType.Required)
                  .Do(async e =>
                  {
                      if (!checkPerms(e.User, "changename"))
                      {
                          await e.Channel.SendMessage("You do not have permission to use this command!");
                          return;
                      }
                      if (!checkValidInput(e.GetArg("oldname")))
                      {
                          await e.Channel.SendMessage("Invalid Current Player Name.");
                          return;
                      }
                      if (!checkValidInput(e.GetArg("newname")))
                      {
                          await e.Channel.SendMessage("Invalid New Player Name.");
                          return;
                      }
                      await e.Channel.SendMessage(changeUserName(e.GetArg("oldname"), e.GetArg("newname")));
                  });
            _client.GetService<CommandService>().CreateCommand("spawnitem") //create command greet
      .Description("Send an item to the defined players mailbox.") //add description, it will be shown when ~help is used
      .Parameter("charactername", ParameterType.Required)
      .Parameter("count", ParameterType.Required)
      .Parameter("itemtospawn", ParameterType.Optional)
      //.Parameter("message", ParameterType.Optional)
      .Do(async e =>
      {
          //SendMail(string charactername, string itemtospawn, int count, string mailcontent, string mailsender)
          if (!checkPerms(e.User, "spawnitem"))
          {
              await e.Channel.SendMessage("You do not have permission to use this command!");
              return;
          }
         if (!checkValidInput(e.GetArg("charactername")))
          {
              await e.Channel.SendMessage("Invalid Data.");
              return;
          }
          int count = 1;
          if (!String.IsNullOrEmpty(e.GetArg("count")))
          {
              if (!int.TryParse(e.GetArg("count"), out count))
              {
                  await e.Channel.SendMessage(SendMail(e.GetArg("charactername"), e.GetArg("count"), 1, "", e.User.Name));
              }
              else
              {
                  await e.Channel.SendMessage(SendMail(e.GetArg("charactername"), e.GetArg("itemtospawn"), count, "", e.User.Name));
              }
          }
      });

            _client.GetService<CommandService>().CreateCommand("spawnforall") //create command greet
.Description("Send an item to the all online players.") //add description, it will be shown when ~help is used
.Parameter("count", ParameterType.Required)
.Parameter("itemtospawn", ParameterType.Optional)
//.Parameter("message", ParameterType.Optional)
.Do(async e =>
{
          //SendMail(string charactername, string itemtospawn, int count, string mailcontent, string mailsender)
          if (!checkPerms(e.User, "spawnforall"))
    {
        await e.Channel.SendMessage("You do not have permission to use this command!");
        return;
    }
    int count = 1;
    if (!String.IsNullOrEmpty(e.GetArg("count")))
    {
        if (!int.TryParse(e.GetArg("count"), out count))
        {
            await e.Channel.SendMessage(SendMailToAllOnline(e.GetArg("count"), 1, e.User.Name));
        }
        else
        {
            await e.Channel.SendMessage(SendMailToAllOnline(e.GetArg("itemtospawn"), count, e.User.Name));
        }
    }
});
            //SendMailToAllOnline(string itemtospawn, int count, string mailsender)
            _client.GetService<CommandService>().CreateCommand("giveap") //create command greet
                  .Description("give ap to a character") //add description, it will be shown when ~help is used
                  .Parameter("username", ParameterType.Required)
                  .Parameter("amount", ParameterType.Optional)
                  .Do(async e =>
                  {
                      if (!checkPerms(e.User, "giveap"))
                      {
                          await e.Channel.SendMessage("You do not have permission to use this command!");
                          return;
                      }
                      if (!checkValidInput(e.GetArg("username")))
                      {
                          await e.Channel.SendMessage("Invalid Data.");
                          return;
                      }
                      int amount = 90;
                      if (!String.IsNullOrEmpty(e.GetArg("amount")))
                      {
                          if (!int.TryParse(e.GetArg("amount"), out amount))
                          {
                              await e.Channel.SendMessage("Invalid level given! The value _MUST BE A NUMBER_");
                              return;
                          }
                      }
                      await e.Channel.SendMessage(giveApToChar(e.GetArg("username"), amount));
                  });
            _client.GetService<CommandService>().CreateCommand("getaccountid") //create command greet
       .Description("get the account id for a character") //add description, it will be shown when ~help is used
       .Parameter("username", ParameterType.Required)
       .Do(async e =>
       {
           if (!checkPerms(e.User, "getaccountid"))
           {
               await e.Channel.SendMessage("You do not have permission to use this command!");
               return;
           }
           if (!checkValidInput(e.GetArg("username")))
           {
               await e.Channel.SendMessage("Invalid Data.");
               return;
           }
           await e.Channel.SendMessage(getUserIdFromCharacterName(e.GetArg("username")));
       });
       _client.GetService<CommandService>().CreateCommand("getloginuser") //create command greet
       .Description("get the login string for a character") //add description, it will be shown when ~help is used
       .Parameter("username", ParameterType.Required)
       .Do(async e =>
       {
           if (!checkPerms(e.User, "getloginuser"))
           {
               await e.Channel.SendMessage("You do not have permission to use this command!");
               return;
           }
           if (!checkValidInput(e.GetArg("username")))
           {
               await e.Channel.SendMessage("Invalid Data.");
               return;
           }
           string charid = getUserIdFromCharacterName(e.GetArg("username"));
           if (!string.IsNullOrEmpty(charid))
           {
               await e.Channel.SendMessage(getAccountNameFromID(charid));
           }
           else
           {
               await e.Channel.SendMessage("Invalid character name!");
           }
       });
            _client.GetService<CommandService>().CreateCommand("reset2ndary") //create command greet
                   .Description("resets the secondary password of the specified player") //add description, it will be shown when ~help is used
                   .Parameter("username", ParameterType.Required)
                   .Do(async e =>
                   {
                       if (!checkPerms(e.User, "reset2ndary"))
                       {
                           await e.Channel.SendMessage("You do not have permission to use this command!");
                           return;
                       }
                       if (!checkValidInput(e.GetArg("username")))
                       {
                           await e.Channel.SendMessage("Invalid Data.");
                           return;
                       }
                       string msg = resetSecondary(e.GetArg("username"));
                       await e.Channel.SendMessage(msg);
                   });
            _client.GetService<CommandService>().CreateCommand("ban") //create command greet
                   .Description("ban a player in game") //add description, it will be shown when ~help is used
                   .Parameter("username", ParameterType.Required)
                   .Parameter("reason", ParameterType.Unparsed)
                   .Do(async e =>
                   {
                       if (!checkPerms(e.User, "ban"))
                       {
                           await e.Channel.SendMessage("You do not have permission to use this command!");
                           return;
                       }
                       if (!checkValidInput(e.GetArg("username")))
                       {
                           await e.Channel.SendMessage("Invalid Data.");
                           return;
                       }
                       string banResult = null;
                       if (!String.IsNullOrEmpty(e.GetArg("reason")))
                       {
                           banResult = BanUser(e.GetArg("username"), e.GetArg("reason"));
                       }
                       else
                       {
                           banResult = BanUser(e.GetArg("username"));
                       }
                       await e.Channel.SendMessage(banResult);
                   });
            _client.GetService<CommandService>().CreateCommand("unban") //create command greet
       .Description("ban a player in game") //add description, it will be shown when ~help is used
       .Parameter("username", ParameterType.Required)
       .Do(async e =>
       {
           if (!checkPerms(e.User, "unban"))
           {
               await e.Channel.SendMessage("You do not have permission to use this command!");
               return;
           }
           if (!checkValidInput(e.GetArg("username")))
           {
               await e.Channel.SendMessage("Invalid Data.");
               return;
           }
           string banResult = unbanUser(e.GetArg("username"));
           await e.Channel.SendMessage(banResult);
       });
            _client.GetService<CommandService>().CreateCommand("online") //create command greet
       .Description("Lists online players with usernames") //add description, it will be shown when ~help is used
       .Parameter("list", ParameterType.Optional)
       .Do(async e =>
       {
           if (!checkPerms(e.User, "online"))
           {
               await e.Channel.SendMessage("You do not have permission to use this command!");
               return;
           }
           List<string> players = onlinePlayers();
           string playernames = null;
           if (!String.IsNullOrEmpty(e.GetArg("list")) && e.GetArg("list") == "all")
           {
               int i = 0;
               foreach (string player in players)
               {
                   if (i == 0)
                   {
                       playernames = player;
                   }
                   else
                   {
                       playernames = playernames + ", " + player;
                   }
                   i++;
               }
               await e.Channel.SendMessage("Players Online: " + players.Count().ToString() + " at time: " + DateTime.Now + Environment.NewLine + playernames);
           }
           else
           {
               await e.Channel.SendMessage("Players Online: " + players.Count().ToString() + " at time: " + DateTime.Now);
           }
       });
            _client.GetService<CommandService>().CreateCommand("banlist") //create command greet
       .Description("Lists banned player usernames") //add description, it will be shown when ~help is used
       .Do(async e =>
       {
           if (!checkPerms(e.User, "banlist"))
           {
               await e.Channel.SendMessage("You do not have permission to use this command!");
               return;
           }
           List<string> players = bannedPlayers();
           string playernames = null;
               int i = 0;
               foreach (string player in players)
               {
                   if (i == 0)
                   {
                       playernames = player;
                   }
                   else
                   {
                       playernames = playernames + ", " + player;
                   }
                   i++;
               }
               await e.Channel.SendMessage("Total Players Banned: " + players.Count().ToString() + " as of: " + DateTime.Now + Environment.NewLine + playernames);
       });
            _client.GetService<CommandService>().CreateCommand("findalts") //create command greet
.Description("Lists alts of the specified character") //add description, it will be shown when ~help is used
.Parameter("charactername", ParameterType.Required)
.Do(async e =>
{
    if (!checkPerms(e.User, "findalts"))
    {
        await e.Channel.SendMessage("You do not have permission to use this command!");
        return;
    }
    List<string> players = findAlts(e.GetArg("charactername"));
    string playernames = null;
    int i = 0;
    foreach (string player in players)
    {
        if (i == 0)
        {
            playernames = player;
        }
        else
        {
            playernames = playernames + ", " + player;
        }
        i++;
    }
    await e.Channel.SendMessage("Total characters found: " + players.Count().ToString() + " as of: " + DateTime.Now + Environment.NewLine + playernames);
});
            //
            _client.ExecuteAndWait(async () =>
            {
                await _client.Connect(ini.IniReadValue("discord", "bot-token"), TokenType.Bot);
            });
        }

        public void setupConfigBase()
        {
            IniFile inidefault = new IniFile(Directory.GetCurrentDirectory() + @"\config.ini");
            if (String.IsNullOrEmpty(inidefault.IniReadValue("discord", "bot-token")))
            {
                inidefault.IniWriteValue("discord", "bot-token", "enter your bot token here!");
            }
            if (String.IsNullOrEmpty(inidefault.IniReadValue("discord", "PrefixChar")))
            {
                inidefault.IniWriteValue("discord", "PrefixChar", "!");
            }
            if (String.IsNullOrEmpty(inidefault.IniReadValue("discord", "HelpMode")))
            {
                inidefault.IniWriteValue("discord", "HelpMode", "public");
            }
            if (String.IsNullOrEmpty(inidefault.IniReadValue("discord", "AllowMentionPrefix")))
            {
                inidefault.IniWriteValue("discord", "AllowMentionPrefix", "true");
            }
            if (String.IsNullOrEmpty(ini.IniReadValue("botconfig", "allowedgroups")))
            {
                inidefault.IniWriteValue("botconfig", "allowedgroups", "000000000000000001,000000000000000002");
                foreach (string group in inidefault.IniReadValue("botconfig", "allowedgroups").Split(','))
                {
                    inidefault.IniWriteValue("permissions", group, "online,banlist,soaps,checkban,reset2ndary,ban,unban,getaccountid,getloginuser,findalts");
                }
            }
            if (String.IsNullOrEmpty(inidefault.IniReadValue("mssql", "username")))
            {
                inidefault.IniWriteValue("mssql", "username", "sa");
            }
            if (String.IsNullOrEmpty(inidefault.IniReadValue("mssql", "password")))
            {
                inidefault.IniWriteValue("mssql", "password", "yourpasswordhere");
            }
            if (String.IsNullOrEmpty(inidefault.IniReadValue("mssql", "ipandport")))
            {
                inidefault.IniWriteValue("mssql", "ipandport", "127.0.0.1,1433");
            }
        }

        public HelpMode getHlpMode(string mode)
        {
            switch (mode)
            {
                case "disabled":
                    return HelpMode.Disabled;
                case "private":
                    return HelpMode.Private;
                case "public":
                    return HelpMode.Public;
                default:
                    return HelpMode.Disabled;
            }

        }

        public string SendMailToAllOnline(string itemtospawn, int count, string mailsender)
        {
            List<string> mailRecepients = onlinePlayers();
            string mailTo = null;
            int i = 0;
            try
            {
                foreach (string user in mailRecepients)
                {
                    string mailReciever = getCharacterIdFromName(user);
                    using (conn = new SqlConnection())
                    {
                        conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                        string oString = "INSERT INTO QueuedItem (CID, ItemClassEx, IsCharacterBinded, Count, MailContent, MailTitle, Color1, Color2, Color3, ReducedDurability, MaxDurabilityBonus)  " + "VALUES (@CID, @ItemClassEx, 0, @Count, @MailContent, @MailTitle,-1 ,-1 ,-1 ,0 ,0)";
                        SqlCommand oCmd = new SqlCommand(oString, conn);
                        oCmd.Parameters.AddWithValue("@CID", mailReciever);
                        oCmd.Parameters.AddWithValue("@ItemClassEx", itemtospawn);
                        oCmd.Parameters.AddWithValue("@Count", count);
                        oCmd.Parameters.AddWithValue("@MailContent", "Here is the item you requested from " + mailsender);
                        oCmd.Parameters.AddWithValue("@MailTitle", "From Iset and " + mailsender);
                        conn.Open();
                        oCmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    if (i==0)
                    {
                        mailTo = user;
                    }
                    else
                    {
                        mailTo = mailTo + ", " + user;
                    }
                    i++;
                }

                return "The items have been sent to " + mailTo + "! They will need to relog or run a quest to see the mail!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string SendMail(string charactername, string itemtospawn, int count, string mailcontent, string mailsender)
        {
            string characterId = getCharacterIdFromName(charactername);
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "INSERT INTO QueuedItem (CID, ItemClassEx, IsCharacterBinded, Count, MailContent, MailTitle, Color1, Color2, Color3, ReducedDurability, MaxDurabilityBonus)  " + "VALUES (@CID, @ItemClassEx, 0, @Count, @MailContent, @MailTitle,-1 ,-1 ,-1 ,0 ,0)";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@CID", characterId);
                    oCmd.Parameters.AddWithValue("@ItemClassEx", itemtospawn);
                    oCmd.Parameters.AddWithValue("@Count", count);
                    oCmd.Parameters.AddWithValue("@MailContent", "Here is the item you requested from " +mailsender);
                    oCmd.Parameters.AddWithValue("@MailTitle", "From Iset and "+ mailsender);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                    return "The item has been sent to " + charactername + "! They will need to relog or run a quest to see the mail!";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            } 
        }

        public bool checkPerms(User discordUser, string command)
        {
            var userRoles = discordUser.Roles;
            IniFile ini = new IniFile(Directory.GetCurrentDirectory() + @"\config.ini");
            string allowed = ini.IniReadValue("botconfig", "allowedgroups");
            if (allowed.Split(',').Count() > 0)
            {
                foreach (string cnfuser in allowed.Split(','))
                {
                    ulong userid = 0;
                    ulong.TryParse(cnfuser, out userid);
                    if (userRoles.Any(input => input.Id == userid))
                    {
                        foreach (string userperm in ini.IniReadValue("permissions", userid.ToString()).Split(','))
                        {
                            if (command == userperm)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public string clearQueue()
        {
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "DELETE FROM QueuedItem";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                    return "Queue has been cleared!";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string clearSoaps()
        {
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "DELETE FROM ChannelBuff";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                    return "Soaps have been cleared!";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string unbanUser(string userName)
        {
            string banResult = null;
            string characterId = getUserIdFromCharacterName(userName);
            string accountName = null;
            if (!string.IsNullOrEmpty(characterId))
            {
                accountName = getAccountNameFromID(characterId);
                if (string.IsNullOrEmpty(accountName))
                {
                    accountName = userName;
                }
            }
            else
            {
                accountName = userName;
            }
            string preBanCheck = isBanned(accountName);
            if (string.IsNullOrEmpty(preBanCheck) || preBanCheck.Contains("is _not_ banned."))
            {
                return "The user " + userName + " is not banned.";
            }
            //INSERT INTO UserBan ([ID], [Status], [ExpireTime], [Reason]) VALUES (N'Gigawiz', '4', '2099-03-25 17:00:00.000', N'Testing');
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server="+ini.IniReadValue("mssql", "ipandport")+"; Database=heroes; User Id="+ ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "DELETE FROM UserBan WHERE ID = @fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", accountName);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                //"The user " + userName + " was successfully banned.";
                string banStatus = isBanned(accountName);
                if (string.IsNullOrEmpty(banStatus) || banStatus.Contains("is _not_ banned."))
                {
                    banResult = "The user " + userName + " was successfully unbanned.";
                }
                else
                {
                    banResult = "The user " + userName + " was _NOT_ unbanned.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return banResult;
        }

        public string restoreDeletedCharacter(string charactername)
        {
            string retmsg = null;
            string verifyChar = getUserIdFromCharacterName(charactername);
            if (String.IsNullOrEmpty(verifyChar))
            {
                return "That is not a valid character name!";
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "UPDATE CharacterInfo SET Status=@fStatus, DeleteTime=@fDeleteTime WHERE Name=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", charactername);
                    oCmd.Parameters.AddWithValue("@fStatus", 0);
                    oCmd.Parameters.AddWithValue("@fDeleteTime", DBNull.Value);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                retmsg = "The character " + charactername + " has been restored. Please go back to the home screen to see it!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return retmsg;
        }

        public string setCharLevel(string charactername, int newlevel)
        {
            string retmsg = null;
            string verifyChar = getUserIdFromCharacterName(charactername);
            if (String.IsNullOrEmpty(verifyChar))
            {
                return "That is not a valid character name!";
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "UPDATE CharacterInfo SET Level=@fLevel WHERE Name=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", charactername);
                    oCmd.Parameters.AddWithValue("@fLevel", newlevel);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                retmsg = charactername + "'s level has been set to " + newlevel.ToString() + ". Please relog to see your new level!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return retmsg;
        }


        public string giveApToChar(string charactername, int amount)
        {
            string retmsg = null;
            string verifyChar = getUserIdFromCharacterName(charactername);
            if (String.IsNullOrEmpty(verifyChar))
            {
                return "That is not a valid character name!";
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "UPDATE CharacterInfo SET AP=AP + @fNewAP WHERE Name=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", charactername);
                    oCmd.Parameters.AddWithValue("@fNewAP", amount);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                retmsg = "Successfully gave " + amount.ToString() + " ap to " + charactername + "! They will need to relog to see the AP changes.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return retmsg;
        }

        public string deleteCharacter(string charactername)
        {
            string retmsg = null;
            string verifyChar = getUserIdFromCharacterName(charactername);
            if (String.IsNullOrEmpty(verifyChar))
            {
                return "That is not a valid character name!";
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "UPDATE CharacterInfo SET Status=1, DeleteTime=CURRENT_TIMESTAMP WHERE Name=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", charactername);
                    oCmd.Parameters.AddWithValue("@fStatus", 0);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                retmsg = "The character " + charactername + " has been deleted.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return retmsg;
        }

        public string changeUserName(string currentname, string newname)
        {
            string retmsg = null;
            string verifyChar = getUserIdFromCharacterName(currentname);
            if (String.IsNullOrEmpty(verifyChar))
            {
                return "That is not a valid character name!";
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "UPDATE CharacterInfo SET Name=@fNewName WHERE Name=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", currentname);
                    oCmd.Parameters.AddWithValue("@fNewName", newname);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                retmsg = "Character Name \"" + currentname + "\" has been changed to '" + newname + "'!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return retmsg;
        }

        public string resetSecondary(string playername)
        {
            string retmsg = null;
            string characterName = getUserIdFromCharacterName(playername);
            string accountName = null;
            if (!string.IsNullOrEmpty(characterName))
            {
                accountName = getAccountNameFromID(characterName);
                if (string.IsNullOrEmpty(accountName))
                {
                    accountName = playername;
                }
            }
            else
            {
                accountName = playername;
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server="+ini.IniReadValue("mssql", "ipandport")+"; Database=heroes; User Id="+ ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "UPDATE [User] SET SecondPassword=@fSecondary WHERE Name=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", accountName);
                    oCmd.Parameters.AddWithValue("@fSecondary", DBNull.Value);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                retmsg =  "The secondary password for " + playername + " has been reset.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return retmsg;
        }

        public bool checkValidInput(string input)
        {
            if (input.All(char.IsLetterOrDigit))
            {
                return true;
            }
            return false;
        }

        public string BanUser(string userName, string banMessage = "You have been permanently banned. Contact staff for additional info.", int duration = 0)
        {
            string banResult = null;
            string characterId = getUserIdFromCharacterName(userName);
            string accountName = null;
            if (!string.IsNullOrEmpty(characterId))
            {
                accountName = getAccountNameFromID(characterId);
                if (string.IsNullOrEmpty(accountName))
                {
                    accountName = userName;
                }
            }
            else
            {
                accountName = userName;
            }
            string preBanCheck = isBanned(accountName);
            if (!string.IsNullOrEmpty(preBanCheck) && preBanCheck.Contains("is banned."))
            {
                return "The user " + userName + " is all ready banned.";
            }
            //INSERT INTO UserBan ([ID], [Status], [ExpireTime], [Reason]) VALUES (N'Gigawiz', '4', '2099-03-25 17:00:00.000', N'Testing');
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server="+ini.IniReadValue("mssql", "ipandport")+"; Database=heroes; User Id="+ ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "INSERT INTO UserBan ([ID], [Status], [ExpireTime], [Reason]) VALUES (@fName, '4', '2099-03-25 17:00:00.000', @fReason);";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", accountName);
                    oCmd.Parameters.AddWithValue("@fReason", banMessage);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                //"The user " + userName + " was successfully banned.";
                string banStatus = isBanned(accountName);
                if (!string.IsNullOrEmpty(banStatus) && banStatus.Contains("is banned."))
                {
                    banResult = "The user " + userName + " was successfully banned.";
                }
                else
                {
                    banResult = "The user " + userName + " was _NOT_ banned.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return banResult;
        }
       
        public string getAccountNameFromID(string accountID)
        {
            string userid = null;
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server="+ini.IniReadValue("mssql", "ipandport")+"; Database=heroes; User Id="+ ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "SELECT Name FROM [User] WHERE ID=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", accountID);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            if (!string.IsNullOrEmpty(oReader["Name"].ToString()))
                            {
                                userid = oReader["Name"].ToString();
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return userid;
        }

        public string getCharacterIdFromName(string characterName)
        {
            string userid = null;
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "Select * from CharacterInfo where Name=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", characterName);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            if (!string.IsNullOrEmpty(oReader["UID"].ToString()))
                            {
                                userid = oReader["ID"].ToString();
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return userid;
        }

        public string getUserIdFromCharacterName(string characterName)
        {
            string userid = null;
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server="+ini.IniReadValue("mssql", "ipandport")+"; Database=heroes; User Id="+ ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "Select * from CharacterInfo where Name=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", characterName);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            if (!string.IsNullOrEmpty(oReader["UID"].ToString()))
                            {
                                userid = oReader["UID"].ToString();
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return userid;
        }

        public string isBanned(string playername)
        {
            string bannedStatus = "is _not_ banned.";
            string characterName = getUserIdFromCharacterName(playername);
            string accountName = null;
            if (!string.IsNullOrEmpty(characterName))
            {
                accountName = getAccountNameFromID(characterName);
                if (string.IsNullOrEmpty(accountName))
                {
                    accountName = playername;
                }
            }
            else
            {
                accountName = playername;
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server="+ini.IniReadValue("mssql", "ipandport")+"; Database=heroes; User Id="+ ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "Select * from UserBan where ID=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", accountName);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            if (!string.IsNullOrEmpty(oReader["ID"].ToString()))
                            {
                                bannedStatus = "is banned." + Environment.NewLine + "Ban Expires on: " + oReader["ExpireTime"].ToString() + Environment.NewLine + "Ban Reason: " + oReader["Reason"].ToString();
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return bannedStatus;
        }

        public List<string> onlinePlayers()
        {
            List<string> onlineNames = new List<string>();
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server=" + ini.IniReadValue("mssql", "ipandport") + "; Database=heroes; User Id=" + ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "SELECT * FROM CharacterInfo WHERE IsConnected=1 ORDER BY Name ASC";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            onlineNames.Add(oReader["Name"].ToString());
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                onlineNames.Add("Error in query!");
                onlineNames.Add(ex.Message);
            }
            return onlineNames;
        }

        public List<string> findAlts(string charactername)
        {
            List<string> characterNames = new List<string>();
            string submittedCharacterAccID = getUserIdFromCharacterName(charactername);
            if (String.IsNullOrEmpty(submittedCharacterAccID))
            {
                return null;
            }
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server="+ini.IniReadValue("mssql", "ipandport")+"; Database=heroes; User Id="+ ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "SELECT * FROM CharacterInfo WHERE UID=@fName";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    oCmd.Parameters.AddWithValue("@fName", submittedCharacterAccID);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            characterNames.Add(oReader["Name"].ToString());
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                characterNames.Add("Error in query!");
                characterNames.Add(ex.Message);
            }
            return characterNames;
        }

        public List<string> bannedPlayers()
        {
            List<string> onlineNames = new List<string>();
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = "Server="+ini.IniReadValue("mssql", "ipandport")+"; Database=heroes; User Id="+ ini.IniReadValue("mssql", "username") + "; password=" + ini.IniReadValue("mssql", "password");
                    string oString = "SELECT * FROM UserBan";
                    SqlCommand oCmd = new SqlCommand(oString, conn);
                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            onlineNames.Add(oReader["ID"].ToString());
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                onlineNames.Add("Error in query!");
                onlineNames.Add(ex.Message);
            }
            return onlineNames;
        }
    }
}
