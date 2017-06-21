using System;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using RemoteControlSystem;
using Devcat.Core;
using System.Security.Cryptography;
using System.Text;
using RemoteControlSystem.ServerMessage;
using System.Data.SQLite;

namespace Iset
{
    class Program
    {
        static void Main(string[] args) => new Program().Start();
        public static DiscordClient _client;
        IniFile ini = new IniFile(Directory.GetCurrentDirectory() + @"\config.ini");
        bool customHelpCommand = false;
        public void Start()
        {
            initialize();
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
                customHelpCommand = true;
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
            _client.MessageReceived += async (s, e) =>
            {
                if (!e.Message.IsAuthor && e.Message.RawText.Contains("(╯°□°）╯︵ ┻━┻") && checkPerms(e.User, "tableflip"))
                {
                    await e.Channel.SendMessage("┬─┬﻿ ノ( ゜-゜ノ)");
                }
            };
            #region commands
            if (customHelpCommand)
            {
                _client.GetService<CommandService>().CreateCommand("help") //create command greet
                                   .Description("lists commands you can access") //add description, it will be shown when ~help is used
                                   .Do(async e =>
                                   {
                                       await processCommand(e);
                                   });
            }
            _client.GetService<CommandService>().CreateCommand("about") //create command greet
                   .Description("tells you about Iset") //add description, it will be shown when ~help is used
                   .Do(async e =>
                   {
                       for (int i = 0; i < 50; i++)
                       {
                           await e.Channel.SendIsTyping();
                       }
                       await e.Channel.SendMessage("I was created by Gigawiz to help staff administrate a vindictus server without needing hands on access to the database or server. You might see me around on the BloodRedDawn Discord under the name 'Iset' or 'Saber' (my test alias). I am currently in beta, so there will be bugs along the way. You can see my source code, issues and download the latest build at https://github.com/Gigawiz/Iset .");
                   });
            _client.GetService<CommandService>().CreateCommand("lastactive") //create command greet
                   .Description("checks last activity of a character or account") //add description, it will be shown when ~help is used
                   .Parameter("username", ParameterType.Required)
                   .Do(async e =>
                   {
                       await processCommand(e);
                   });
            _client.GetService<CommandService>().CreateCommand("fix") //create command greet
                   .Description("clears broken soaps") //add description, it will be shown when ~help is used
                   .Parameter("whatdoifix", ParameterType.Required)
                   .Parameter("announce", ParameterType.Optional)
                   .Do(async e =>
                   {
                       await processCommand(e);
                   });
            _client.GetService<CommandService>().CreateCommand("clearqueue") //create command greet
                   .Description("clears item queue") //add description, it will be shown when ~help is used
                   .Do(async e =>
                   {
                       await processCommand(e);
                   });
            _client.GetService<CommandService>().CreateCommand("checkban") //create command greet
                   .Description("checks if a player is banned") //add description, it will be shown when ~help is used
                   .Parameter("username", ParameterType.Required)
                   .Do(async e =>
                   {
                       await processCommand(e);
                   });
            _client.GetService<CommandService>().CreateCommand("restorechar") //create command greet
                  .Description("restores a deleted character") //add description, it will be shown when ~help is used
                  .Parameter("username", ParameterType.Required)
                  .Do(async e =>
                  {
                      await processCommand(e);
                  });
            _client.GetService<CommandService>().CreateCommand("deletechar") //create command greet
                  .Description("delete a character") //add description, it will be shown when ~help is used
                  .Parameter("username", ParameterType.Required)
                  .Do(async e =>
                  {
                      await processCommand(e);
                  });
            _client.GetService<CommandService>().CreateCommand("setlevel") //create command greet
                  .Description("set the level of a character") //add description, it will be shown when ~help is used
                  .Parameter("username", ParameterType.Required)
                  .Parameter("level", ParameterType.Optional)
                  .Do(async e =>
                  {
                      await processCommand(e);
                  });
            _client.GetService<CommandService>().CreateCommand("changename") //create command greet
                  .Description("change a characters name") //add description, it will be shown when ~help is used
                  .Parameter("oldname", ParameterType.Required)
                  .Parameter("newname", ParameterType.Required)
                  .Do(async e =>
                  {
                      await processCommand(e);
                  });
            _client.GetService<CommandService>().CreateCommand("deleteguild") //create command greet
                 .Description("mark a guild as deleted") //add description, it will be shown when ~help is used
                 .Parameter("guildname", ParameterType.Required)
                 .Do(async e =>
                  {
                      await processCommand(e);
                  });
            _client.GetService<CommandService>().CreateCommand("restoreguild") //create command greet
                .Description("mark a guild as deleted") //add description, it will be shown when ~help is used
                .Parameter("guildname", ParameterType.Required)
                .Do(async e =>
                {
                    await processCommand(e);
                });
            _client.GetService<CommandService>().CreateCommand("changeguildname") //create command greet
                  .Description("change the name of a guild") //add description, it will be shown when ~help is used
                  .Parameter("oldname", ParameterType.Required)
                  .Parameter("newname", ParameterType.Required)
                  .Parameter("oldid", ParameterType.Optional)
                  .Parameter("newid", ParameterType.Optional)
                  .Do(async e =>
                  {
                      await processCommand(e);
                  });
            
            _client.GetService<CommandService>().CreateCommand("spawn") //create command greet
                  .Description("Send an item to the defined players mailbox.") //add description, it will be shown when ~help is used
                  .Parameter("charactername", ParameterType.Required)
                  .Parameter("count", ParameterType.Required)
                  .Parameter("itemtospawn", ParameterType.Optional)
                  //.Parameter("message", ParameterType.Optional)
                  .Do(async e =>
                  {
                      await processCommand(e);
                  });
            //SendMailToAllOnline(string itemtospawn, int count, string mailsender)
            _client.GetService<CommandService>().CreateCommand("giveap") //create command greet
                  .Description("give ap to a character") //add description, it will be shown when ~help is used
                  .Parameter("username", ParameterType.Required)
                  .Parameter("amount", ParameterType.Optional)
                  .Do(async e =>
                  {
                      await processCommand(e);
                  });
            _client.GetService<CommandService>().CreateCommand("getaccountid") //create command greet
               .Description("get the account id for a character") //add description, it will be shown when ~help is used
               .Parameter("username", ParameterType.Required)
               .Do(async e =>
               {
                   await processCommand(e);

               });
            _client.GetService<CommandService>().CreateCommand("getcharacterid") //create command greet
   .Description("get the account id for a character") //add description, it will be shown when ~help is used
   .Parameter("username", ParameterType.Required)
   .Do(async e =>
   {
       await processCommand(e);

   });
            //getcharacterid
            _client.GetService<CommandService>().CreateCommand("getloginuser") //create command greet
               .Description("get the login string for a character") //add description, it will be shown when ~help is used
               .Parameter("username", ParameterType.Required)
               .Do(async e =>
               {
                   await processCommand(e);

               });
            _client.GetService<CommandService>().CreateCommand("reset2ndary") //create command greet
                   .Description("resets the secondary password of the specified player") //add description, it will be shown when ~help is used
                   .Parameter("username", ParameterType.Required)
                   .Do(async e =>
                   {
                       await processCommand(e);
                   });
            _client.GetService<CommandService>().CreateCommand("ban") //create command greet
                   .Description("ban a player in game") //add description, it will be shown when ~help is used
                   .Parameter("username", ParameterType.Required)
                   .Parameter("reason", ParameterType.Unparsed)
                   .Do(async e =>
                   {
                       await processCommand(e);

                   });
            _client.GetService<CommandService>().CreateCommand("unban") //create command greet
               .Description("ban a player in game") //add description, it will be shown when ~help is used
               .Parameter("username", ParameterType.Required)
               .Do(async e =>
               {
                   await processCommand(e);
               });
            _client.GetService<CommandService>().CreateCommand("online") //create command greet
               .Description("Lists online players with usernames") //add description, it will be shown when ~help is used
               .Parameter("list", ParameterType.Optional)
               .Do(async e =>
               {
                   await processCommand(e);
               });
            _client.GetService<CommandService>().CreateCommand("banlist") //create command greet
               .Description("Lists banned player usernames") //add description, it will be shown when ~help is used
               .Do(async e =>
               {
                   await processCommand(e);
               });
            _client.GetService<CommandService>().CreateCommand("findalts") //create command greet
                .Description("Lists alts of the specified character") //add description, it will be shown when ~help is used
                .Parameter("charactername", ParameterType.Required)
                .Do(async e =>
                {
                    await processCommand(e);
                });
            _client.GetService<CommandService>().CreateCommand("validateme") //create command greet
              .Description("change a characters name") //add description, it will be shown when ~help is used
              .Parameter("charname", ParameterType.Required)
              .Do(async e =>
              {
                  await processCommand(e);

              });
            _client.GetService<CommandService>().CreateCommand("restoreweapon") //create command greet
              .Description("restore a weapon for a character") //add description, it will be shown when ~help is used
              .Parameter("charactername", ParameterType.Required)
              .Parameter("enhancementlevel", ParameterType.Required)
              .Parameter("weapontype", ParameterType.Required)
              .Do(async e =>
              {
                  await processCommand(e);
              });
            _client.GetService<CommandService>().CreateCommand("forcerestoreweapon") //create command greet
             .Description("restore a weapon for a character") //add description, it will be shown when ~help is used
             .Parameter("charactername", ParameterType.Required)
             .Parameter("wepcode", ParameterType.Required)
             .Do(async e =>
             {
                 await processCommand(e);
             });
            _client.GetService<CommandService>().CreateCommand("rgb")
                .Parameter("R", ParameterType.Optional)
                .Parameter("G", ParameterType.Optional)
                .Parameter("B", ParameterType.Optional)
                .Do(async e =>
                {
                    await processCommand(e);
                });
            _client.GetService<CommandService>().CreateCommand("dye")
                .Parameter("charactername", ParameterType.Required)
                .Parameter("itemcode", ParameterType.Required)
                .Parameter("slot1", ParameterType.Required)
                .Parameter("slot2", ParameterType.Required)
                .Parameter("slot3", ParameterType.Required)
                .Do(async e =>
                {
                    await processCommand(e);
                });
            _client.GetService<CommandService>().CreateCommand("server")
                .Parameter("command", ParameterType.Required)
                .Parameter("othervars", ParameterType.Unparsed)
                .Do(async e =>
                {
                    await processCommand(e);
                });
            _client.GetService<CommandService>().CreateCommand("rng")
                .Parameter("rngcmd", ParameterType.Required)
                .Parameter("playerstochoosefrom", ParameterType.Unparsed)
                .Do(async e =>
                {
                    await processCommand(e);
                });
            #endregion
            //
            _client.ExecuteAndWait(async () =>
            {
                await _client.Connect(ini.IniReadValue("discord", "bot-token"), TokenType.Bot);
            });
            _client.SetGame("Blood Red Dawn", GameType.Default, "github.com/Gigawiz/Iset");
        }

        #region defaultconfigSetup

        public void initialize()
        {
            //create the sqlite db if its not there
             ValidationFunctions.createInitialTables();
            //next, create the config
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
            if (String.IsNullOrEmpty(inidefault.IniReadValue("discord", "enable-announcements")))
            {
                inidefault.IniWriteValue("discord", "enable-announcements", "false");
            }
            if (String.IsNullOrEmpty(inidefault.IniReadValue("discord", "announcements-channel")))
            {
                inidefault.IniWriteValue("discord", "announcements-channel", "channel id that iset should send announcements to");
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
            if (String.IsNullOrEmpty(inidefault.IniReadValue("logs", "logtofile")))
            {
                inidefault.IniWriteValue("logs", "logtofile", "true");
            }
            if (String.IsNullOrEmpty(inidefault.IniReadValue("logs", "logtoconsole")))
            {
                inidefault.IniWriteValue("logs", "logtoconsole", "true");
            }
            if (String.IsNullOrEmpty(inidefault.IniReadValue("logs", "logToDiscordChannel")))
            {
                inidefault.IniWriteValue("logs", "logToDiscordChannel", "false");
            }
            if (String.IsNullOrEmpty(inidefault.IniReadValue("logs", "logchannelid")))
            {
                inidefault.IniWriteValue("logs", "logchannelid", "channel id that iset should log to");
            }
            if (String.IsNullOrEmpty(inidefault.IniReadValue("misc", "maxitemrestoresperaccount")))
            {
                inidefault.IniWriteValue("misc", "maxitemrestoresperaccount", "1");
            }
        }
        #endregion

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
                case "custom":
                    customHelpCommand = true;
                    return HelpMode.Disabled;
                default:
                    return HelpMode.Disabled;
            }

        }

        public bool checkPerms(User discordUser, string command)
        {
            IniFile ini = new IniFile(Directory.GetCurrentDirectory() + @"\config.ini");
            string perms = ini.IniReadValue("permissions", discordUser.Id.ToString());
            if (string.IsNullOrEmpty(perms))
            {
                return false;
            }
            if (perms.Split(',').Count() > 1)
            {
                foreach (string userperm in perms.Split(','))
                {
                    if (command == userperm)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (command == perms)
                {
                    return true;
                }
            }
            Logging.LogItem("User " + discordUser.Name + " does not have permission to use given command " + command);
            return false;
        }

        public bool checkValidInput(string input)
        {
            if (input.All(char.IsLetterOrDigit))
            {
                return true;
            }
            return false;
        }

        public async Task processCommand(CommandEventArgs e)
        {
            bool hasPerms = checkPerms(e.User, e.Command.Text);
            if (!hasPerms)
            {
                await e.Channel.SendMessage("You do not have permission to use " + e.Command.Text + "!");
                return;
            }
            bool announcementsEnabled = false;
            bool.TryParse(ini.IniReadValue("discord", "enable-announcements"), out announcementsEnabled);
            ulong announcementsChannel = 0;
            if (announcementsEnabled)
            {
                ulong.TryParse(ini.IniReadValue("discord", "announcements-channel"), out announcementsChannel);
            }
            switch (e.Command.Text)
            {
                case "help":
                    if (!checkPerms(e.User, "help"))
                    {
                        await e.Channel.SendMessage("You do not have permission to use this command!");
                        return;
                    }
                    await e.Channel.SendMessage("coming soon");
                    Logging.LogItem(e.User.Name + " has used the help command.");
                    break;
                case "unban":
                    if (!checkValidInput(e.GetArg("username")))
                    {
                        await e.Channel.SendMessage("Invalid Data.");
                        return;
                    }
                    if (e.GetArg("username").ToUpper().Contains("DELT"))
                    {
                        await e.Channel.SendMessage("This user's ban is irrevocable. Please contact Gigawiz if you have an issue with this.");
                        return;
                    }
                    string unbanResult = Bans.unbanUser(e.GetArg("username"));
                    await e.Channel.SendMessage(unbanResult);
                    Logging.LogItem(e.User.Name + " has unbanned " + e.GetArg("username"));
                    break;
                case "lastactive":
                    await e.Channel.SendMessage(UserFunctions.checkActivity(e.GetArg("username")));
                    Logging.LogItem(e.User.Name + " checked the activity of " + e.GetArg("username"));
                    break;
                case "clearqueue":
                    await e.Channel.SendMessage(ItemFunctions.clearQueue());
                    Logging.LogItem(e.User.Name + " has cleared the item queue");
                    break;
                case "changeguildname":
                    if (!checkValidInput(e.GetArg("oldname")))
                    {
                        await e.Channel.SendMessage("Invalid Current Guild Name or ID.");
                        return;
                    }
                    if (!checkValidInput(e.GetArg("newname")))
                    {
                        await e.Channel.SendMessage("Invalid New Guild Name.");
                        return;
                    }
                    string oldid = null;
                    string newid = null;
                    if (!string.IsNullOrEmpty(e.GetArg("oldid")))
                    {
                        if (!checkValidInput(e.GetArg("oldid")))
                        {
                            await e.Channel.SendMessage("Invalid Old Guild ID.");
                            return;
                        }
                        oldid = e.GetArg("oldid");
                    }
                    if (!string.IsNullOrEmpty(e.GetArg("newid")))
                    {
                        if (!checkValidInput(e.GetArg("newid")))
                        {
                            await e.Channel.SendMessage("Invalid New Guild ID.");
                            return;
                        }
                        newid = e.GetArg("newid");
                    }
                    await e.Channel.SendMessage(GuildFunctions.changeGuildName(e.GetArg("oldname"), e.GetArg("newname"), oldid, newid));
                    Logging.LogItem(e.User.Name + " has changed the character name of \"" + e.GetArg("oldname") + "\" to \"" + e.GetArg("newname") + "\"");
                    break;
                case "restoreguild":
                    if (!checkValidInput(e.GetArg("guildname")))
                    {
                        await e.Channel.SendMessage("Invalid Guild Name.");
                        return;
                    }
                    await e.Channel.SendMessage(GuildFunctions.reopenGuild(e.GetArg("guildname")));
                    Logging.LogItem(e.User.Name + " has re-opened the guild \"" + e.GetArg("guildname"));
                    break;
                case "deleteguild":
                    if (!checkValidInput(e.GetArg("guildname")))
                    {
                        await e.Channel.SendMessage("Invalid Guild Name.");
                        return;
                    }
                    await e.Channel.SendMessage(GuildFunctions.closeGuild(e.GetArg("guildname")));
                    Logging.LogItem(e.User.Name + " has deleted the guild \"" + e.GetArg("guildname"));
                    break;
                case "changename":
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
                    await e.Channel.SendMessage(UserFunctions.changeUserName(e.GetArg("oldname"), e.GetArg("newname")));
                    Logging.LogItem(e.User.Name + " has changed the character name of \"" + e.GetArg("oldname") + "\" to \"" + e.GetArg("newname") + "\"");
                    break;
                case "setlevel":
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
                    await e.Channel.SendMessage(UserFunctions.setCharLevel(e.GetArg("username"), newlevel));
                    Logging.LogItem(e.User.Name + " has set " + e.GetArg("username") + "'s level to " + newlevel.ToString());
                    break;
                case "deletechar":
                    if (!checkValidInput(e.GetArg("username")))
                    {
                        await e.Channel.SendMessage("Invalid Data.");
                        return;
                    }
                    await e.Channel.SendMessage(UserFunctions.deleteCharacter(e.GetArg("username")));
                    Logging.LogItem(e.User.Name + " has deleted character " + e.GetArg("username"));
                    break;
                case "restorechar":
                    if (!checkValidInput(e.GetArg("username")))
                    {
                        await e.Channel.SendMessage("Invalid Data.");
                        return;
                    }
                    await e.Channel.SendMessage(UserFunctions.restoreDeletedCharacter(e.GetArg("username")));
                    Logging.LogItem(e.User.Name + " has restored character " + e.GetArg("username"));
                    break;
                case "checkban":
                    if (!checkValidInput(e.GetArg("username")))
                    {
                        await e.Channel.SendMessage("Invalid Data.");
                        return;
                    }
                    string msg = "The user " + e.GetArg("username") + " " + Bans.isBanned(e.GetArg("username"));
                    await e.Channel.SendMessage(msg);
                    Logging.LogItem(e.User.Name + " checked if user " + e.GetArg("username") + " was banned");
                    break;
                case "spawn":
                    if (!checkValidInput(e.GetArg("charactername")))
                    {
                        await e.Channel.SendMessage("Invalid Data.");
                        return;
                    }
                    if (e.GetArg("charactername") == "all")
                    {
                        if (!checkPerms(e.User, "spawnall"))
                        {
                            await e.Channel.SendMessage("You do not have permission to use " + e.Command.Text + " for all players!");
                            return;
                        }
                        int playerCount = UserFunctions.playerList("id", "all", true).Count();
                        await e.Channel.SendMessage("Sending item(s) to " + playerCount.ToString() + " players. Please wait as this will take a long time for a large player base. I will remain un-responsive until all items have been set in the queue!");
                    }
                    if (e.GetArg("charactername") == "allonline")
                    {
                        if (!checkPerms(e.User, "spawnallonline"))
                        {
                            await e.Channel.SendMessage("You do not have permission to use " + e.Command.Text + " for all online players!");
                            return;
                        }
                    }
                    int count = 1;
                    if (!String.IsNullOrEmpty(e.GetArg("count")))
                    {
                        if (!int.TryParse(e.GetArg("count"), out count))
                        {
                            await e.Channel.SendMessage(ItemFunctions.SendMail(e.GetArg("charactername"), e.GetArg("count"), 1, "Here is the item you requested from " + e.User.Name, "From Iset and " + e.User.Name));
                            Logging.LogItem(e.User.Name + " has spawned one " + e.GetArg("count") + " for player " + e.GetArg("charactername"));
                        }
                        else
                        {
                            await e.Channel.SendMessage(ItemFunctions.SendMail(e.GetArg("charactername"), e.GetArg("itemtospawn"), count, "Here is the item you requested from " + e.User.Name, "From Iset and " + e.User.Name));
                            Logging.LogItem(e.User.Name + " has spawned " + count.ToString() + " " + e.GetArg("itemtospawn") + " for player " + e.GetArg("charactername"));
                        }
                    }
                    break;
                case "giveap":
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
                    await e.Channel.SendMessage(UserFunctions.giveApToChar(e.GetArg("username"), amount));
                    Logging.LogItem(e.User.Name + " has given " + e.GetArg("username") + " " + amount.ToString() + " AP.");
                    break;
                case "getcharacterid":
                    if (!checkValidInput(e.GetArg("username")))
                    {
                        await e.Channel.SendMessage("Invalid data");
                        return;
                    }
                    await e.Channel.SendMessage(UserFunctions.getCharacterIdFromName(e.GetArg("username")));
                    break;
                case "getaccountid":
                    if (!checkValidInput(e.GetArg("username")))
                    {
                        await e.Channel.SendMessage("Invalid Data.");
                        return;
                    }
                    await e.Channel.SendMessage(UserFunctions.getUserIdFromCharacterName(e.GetArg("username")));
                    Logging.LogItem(e.User.Name + " has retrieved the user id for " + e.GetArg("username"));
                    break;
                case "getloginuser":
                    if (!checkValidInput(e.GetArg("username")))
                    {
                        await e.Channel.SendMessage("Invalid Data.");
                        return;
                    }
                    string charid = UserFunctions.getUserIdFromCharacterName(e.GetArg("username"));
                    if (!string.IsNullOrEmpty(charid))
                    {
                        await e.Channel.SendMessage(UserFunctions.getAccountNameFromID(charid));
                        Logging.LogItem(e.User.Name + " has retrieved the account login name for " + e.GetArg("username"));
                    }
                    else
                    {
                        await e.Channel.SendMessage("Invalid character name!");
                    }
                    break;
                case "reset2ndary":
                    if (!checkValidInput(e.GetArg("username")))
                    {
                        await e.Channel.SendMessage("Invalid Data.");
                        return;
                    }
                    string message = AccountFunctions.resetSecondary(e.GetArg("username"));
                    await e.Channel.SendMessage(message);
                    Logging.LogItem(e.User.Name + " has reset the secondary password of " + e.GetArg("username"));
                    break;
                case "ban":
                    if (!checkValidInput(e.GetArg("username")))
                    {
                        await e.Channel.SendMessage("Invalid Data.");
                        return;
                    }
                    string banResult = null;
                    if (!String.IsNullOrEmpty(e.GetArg("reason")))
                    {
                        banResult = Bans.BanUser(e.GetArg("username"), e.GetArg("reason"));
                    }
                    else
                    {
                        banResult = Bans.BanUser(e.GetArg("username"));
                    }
                    await e.Channel.SendMessage(banResult);
                    Logging.LogItem(e.User.Name + " has banned " + e.GetArg("username") + " for reason: " + e.GetArg("reason"));
                    break;
                case "online":
                    if (!String.IsNullOrEmpty(e.GetArg("list")) && e.GetArg("list") == "all")
                    {
                        await e.Channel.SendMessage(MiscFunctions.onlinePlayers2(true));
                    }
                    else
                    {
                        await e.Channel.SendMessage(MiscFunctions.onlinePlayers2());
                    }
                    Logging.LogItem(e.User.Name + " has listed online players.");
                    break;
                case "banlist":
                    List<string> bannedplayers = Bans.bannedPlayers();
                    string bannednames = null;
                    int index = 0;
                    foreach (string player in bannedplayers)
                    {
                        if (index == 0)
                        {
                            bannednames = player;
                        }
                        else
                        {
                            bannednames = bannednames + ", " + player;
                        }
                        index++;
                    }
                    await e.Channel.SendMessage("Total Players Banned: " + bannedplayers.Count().ToString() + " as of: " + DateTime.Now + Environment.NewLine + bannednames);
                    Logging.LogItem(e.User.Name + " has viewed the banlist.");
                    break;
                case "findalts":
                    List<string> altChars = UserFunctions.findAlts(e.GetArg("charactername"));
                    string altNames = null;
                    int i = 0;
                    foreach (string player in altChars)
                    {
                        if (i == 0)
                        {
                            altNames = player;
                        }
                        else
                        {
                            altNames = altNames + ", " + player;
                        }
                        i++;
                    }
                    await e.Channel.SendMessage("Total characters found: " + altChars.Count().ToString() + " as of: " + DateTime.Now + Environment.NewLine + altNames);
                    Logging.LogItem(e.User.Name + " has searched for the alts of " + e.GetArg("charactername"));
                    break;
                case "validateme":
                    if (!checkValidInput(e.GetArg("charname")))
                    {
                        await e.Channel.SendMessage("Invalid Player Name.");
                        return;
                    }
                    await e.Channel.SendMessage("Coming Soon");
                    //await e.Channel.SendMessage("test");
                    //await e.Channel.SendMessage(ValidationFunctions.storeValidationCode(e.GetArg("charname")));
                    //await e.Channel.SendMessage(ValidationFunctions.sendValidation(e.GetArg("charname"), e.User.Nickname));
                    Logging.LogItem(e.User.Name + " has tried to being the validation process to link character " + e.GetArg("charname") + "to their discord account.");
                    break;
                case "rgb":
                    await e.Channel.SendMessage(MiscFunctions.colorConvert(e.GetArg("R"), e.GetArg("G"), e.GetArg("B")));
                    break;
                case "restoreweapon":
                    await e.Channel.SendMessage(ItemFunctions.restoreItem(e.GetArg("charactername"), e.GetArg("enhancementlevel"), e.GetArg("weapontype"), e.User.Nickname + "(" + e.User.Name + ")"));
                    break;
                case "forcerestoreweapon":
                    await e.Channel.SendMessage(ItemFunctions.forceRestoreItem(e.GetArg("charactername"), e.GetArg("wepcode"), e.User.Nickname + "(" + e.User.Name + ")"));
                    break;
                case "dye":
                    await e.Channel.SendMessage(ItemFunctions.dyeItem(e.GetArg("charactername"), e.GetArg("itemcode"), e.GetArg("slot1"), e.GetArg("slot2"), e.GetArg("slot3")));
                    Logging.LogItem(e.User.Name + " has dyed " + e.GetArg("charactername") + "'s " + e.GetArg("itemcode") + " to colors: " + Environment.NewLine + "Slot 1: " + e.GetArg("slot1") + Environment.NewLine + "Slot 2: " + e.GetArg("slot2") + Environment.NewLine + "Slot 3: " + e.GetArg("slot3"));
                    break;
                case "server":
                    await e.Channel.SendMessage(ServerFunctions.parseServerAction(e.GetArg("command"), e.GetArg("othervars")));
                    break;
                /*case "rng":
                    if (e.GetArg("rngcmd") == "randomplayer")
                    {
                        await e.Channel.SendMessage(EventFunctions.pickRandomPlayer(e.GetArg("playerstochoosefrom")));
                    }
                    else
                    {
                        await e.Channel.SendMessage("This command currently only works with the \"randomplayer\" paramater");
                    }
                    break;*/
                case "fix":
                    if (e.GetArg("whatdoifix") == "market")
                    {
                        if (!checkPerms(e.User, "fixmarket"))
                        {
                            await e.Channel.SendMessage("You do not have permission to use " + e.Command.Text + " for the marketplace!");
                            return;
                        }
                        if (!string.IsNullOrEmpty(e.GetArg("announce")) && announcementsEnabled && announcementsChannel > 0)
                        {
                            Channel sndAnnounce = _client.GetChannel(announcementsChannel);
                            await sndAnnounce.SendMessage(sndAnnounce.Server.EveryoneRole.Mention + " Items that have expired on the marketplace have been sent to their respective owners! Please relog to see the items in your mailbox!");
                        }
                        await e.Channel.SendMessage(ServerFunctions.returnExpiredMarketItems());
                        Logging.LogItem(e.User.Name + " has restored expired marketplace items");
                    }
                    else if (e.GetArg("whatdoifix") == "soaps")
                    {
                        if (!checkPerms(e.User, "fixsoaps"))
                        {
                            await e.Channel.SendMessage("You do not have permission to use " + e.Command.Text + " for soaps!");
                            return;
                        }
                        if (!string.IsNullOrEmpty(e.GetArg("announce")) && announcementsEnabled && announcementsChannel > 0)
                        {
                            Channel sndAnnounce = _client.GetChannel(announcementsChannel);
                            await sndAnnounce.SendMessage(sndAnnounce.Server.EveryoneRole.Mention + " The soaps have been cleared by " + e.User.Mention + "! You may now place them down again.");
                        }
                        await e.Channel.SendMessage(MiscFunctions.clearSoaps());
                        Logging.LogItem(e.User.Name + " has cleared bath soaps");
                    }
                    break;
                default:
                    await e.Channel.SendMessage("This command either does not exist, or is still in development.");
                    break;
            }
        }
    }
}
