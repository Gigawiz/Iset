// snippet from: bot/bot.go
package bot

import (
	"log"
	"os"
	"os/signal"
	"strings"

	"bloodreddawn.com/IsetGo/config"
	"bloodreddawn.com/IsetGo/logging"
	"bloodreddawn.com/IsetGo/LoginServer"
	"bloodreddawn.com/IsetGo/Vindictus"
	"github.com/bwmarrin/discordgo"
)

var (
	BotToken string
	UseLoginServer bool
	LogToConsole bool
)

func init() {
	BotToken = config.BotToken
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
	//discord.ChannelMessageSend(message.ChannelID, "You Said: " + message.Content)

	// Respond to messages
	switch {
	case strings.Contains(message.Content, "$hello"):
		discord.ChannelMessageSend(message.ChannelID, "Hi there!")
	case strings.Contains(message.Content, "$help"):
		commandHelp := displayHelp(message.Content)
		discord.ChannelMessageSendComplex(message.ChannelID, commandHelp)
	case strings.Contains(message.Content, "$vindictus"):
		var data = getData(message.Content)
		if (data == "") {
			data = "That doesn't appear to exist!"
			discord.ChannelMessageSend(message.ChannelID, data)
		} else {
			if (strings.Contains(message.Content, "scroll")) {
				var cmdCln = strings.ReplaceAll(message.Content, "$vindictus ", "")
				embedScroll := scrollEmbed(strings.ReplaceAll(cmdCln, "scroll ", ""), data)
				discord.ChannelMessageSendComplex(message.ChannelID, embedScroll)
			} else {
				discord.ChannelMessageSend(message.ChannelID, data)
			}
		}
	}
	logging.Log("Command '" + message.Content + "' run by " + message.Author.Username)
}

func getData(command string) string {
	var cmdCln = strings.ReplaceAll(command, "$vindictus ", "")
	return Vindictus.ProcessCmd(cmdCln)
}