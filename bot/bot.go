// snippet from: bot/bot.go
package bot

import (
	"fmt"
	"log"
	"os"
	"os/signal"
	"strings"

	"bloodreddawn.com/IsetGo/LoginServer"
	"bloodreddawn.com/IsetGo/Vindictus"
	"github.com/bwmarrin/discordgo"
)

var (
	BotToken string
	DBIP     string
	DBPort   int
	DBUser   string
	DBPass   string
)

func Run() {
	// Create new Discord Session
	discord, err := discordgo.New("Bot " + BotToken)
	if err != nil {
		log.Fatal(err)
	}

	// Add event handler
	discord.AddHandler(newMessage)

	// Open session
	discord.Open()
	defer discord.Close()

	//launch NMServer too :D
	LoginServer.LoginStart()
	fmt.Println("Login Server running...")

	// Run until code is terminated
	fmt.Println("Bot running...")
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
}

func getData(command string) string {
	Vindictus.DBIP = DBIP
	Vindictus.DBPort = DBPort
	Vindictus.DBUser = DBUser
	Vindictus.DBPass = DBPass
	var cmdCln = strings.ReplaceAll(command, "$vindictus ", "")
	return Vindictus.ProcessCmd(cmdCln)
}
