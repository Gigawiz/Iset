// snippet from: bot/bot.go
package bot

import (
	"log"
	"os"
	"os/signal"
	"strings"
	//"fmt"
	
	"bloodreddawn.com/IsetGo/config"
	"bloodreddawn.com/IsetGo/logging"
	"bloodreddawn.com/IsetGo/LoginServer"
	"bloodreddawn.com/IsetGo/Vindictus"
	"github.com/bwmarrin/discordgo"
)

var (
	BotToken string
	BotPrefix string
	BotVersion string
	UseUnflip bool
	UseLoginServer bool
	LogToConsole bool
)

func init() {
	BotToken = config.BotToken
	BotPrefix = config.BotPrefix
	BotVersion = config.BotVersion
	UseUnflip = config.UseUnflip
	UseLoginServer = config.UseLoginServer
	LogToConsole = config.LogToConsole
}

func Run() {
	// Create new Discord Session
	logging.Log("Creating new Discord Bot Instance...");
	discord, err := discordgo.New("Bot " + BotToken)
	if err != nil {
		log.Fatal(err)
	}

	// Add event handler
	discord.AddHandler(newMessage)
	logging.Log("Opening session...")
	// Open session
	discord.Open()
	defer discord.Close()
	
	if (UseLoginServer) {
		//launch NMServer too :D
		logging.Log("Login server enabled.... Launching....")
		LoginServer.LoginStart()
		logging.Log("Login Server Running!")
	}
	// Run until code is terminated
	logging.Log("Bot Running!")
	c := make(chan os.Signal, 1)
	signal.Notify(c, os.Interrupt)
	<-c

}

func newMessage(discord *discordgo.Session, message *discordgo.MessageCreate) {

	// Ignore bot messaage
	if message.Author.ID == discord.State.User.ID {
		return
	}
	
	if (strings.Contains(message.Content, "(╯°□°）╯︵ ┻━┻") || strings.Contains(message.Content, "(╯°□°)")) {
		if (UseUnflip) {
			discord.ChannelMessageSend(message.ChannelID, Unflip())
		}
	}
	
	if (!strings.Contains(message.Content, BotPrefix)) {
		return
	}
	
	if (!strings.Contains(message.Content[0:len(BotPrefix)], BotPrefix)) {
		return
	}
	
	var msgContent = strings.ReplaceAll(message.Content, BotPrefix + " ", "")
	
	words := strings.Fields(msgContent)
	if (!hasPermission(message.Author.ID, words[0])) {
		discord.ChannelMessageSend(message.ChannelID, "You do not have permission to use this command!");
		return
	}
	
	var commandSent = words[0]
	
	// Respond to messages
	switch {
		case commandSent == "hello":
			discord.ChannelMessageSend(message.ChannelID, "Hi there!")
		case commandSent == "addperm":
			permdata := addPermission(words[1], words[2])
			if (permdata) {
				discord.ChannelMessageSend(message.ChannelID, "Permissions added!")
			} else {
				discord.ChannelMessageSend(message.ChannelID, "Failed to add permissions!")
			}
		case commandSent == "removeperm":
			permdata := removePermission(words[1], words[2])
			if (permdata) {
				discord.ChannelMessageSend(message.ChannelID, "Permissions removed!")
			} else {
				discord.ChannelMessageSend(message.ChannelID, "Failed to remove permissions!")
			}
		
		case commandSent == "version":
			discord.ChannelMessageSend(message.ChannelID, "I am currently running on Version " + BotVersion + "!" + CheckUpdate(config.AutoDLUpdate))
			
		case commandSent == "help":
			commandHelp := displayHelp(msgContent)
			discord.ChannelMessageSendComplex(message.ChannelID, commandHelp)
		default:
			dmsgt, data := Vindictus.ProcessCmd(msgContent)
			if (data == "" || data == "error") {
				discord.ChannelMessageSend(message.ChannelID, "An error has occoured!")
				return
			}
			if (dmsgt == "scrollembed") {
				discord.ChannelMessageSendComplex(message.ChannelID, Vindictus.ScrollEmbed(data))
			} else if (dmsgt == "banembed") {
				discord.ChannelMessageSendComplex(message.ChannelID, Vindictus.BanEmbed(data))
			}	else {
				discord.ChannelMessageSend(message.ChannelID, data)
			}
	}
	logging.Log("Command '" + message.Content + "' run by " + message.Author.Username)
}

func firstN(s string, n int) string {
     if len(s) > n {
          return s[:n]
     }
     return s
}
