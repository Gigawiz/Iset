// snippet from: main.go
package main

import (
	"bloodreddawn.com/IsetGo/bot"
	
	//"log"
	//"os"
)

func main() {
	// Start the bot
	bot.BotToken = "Your Discord Bot Token"
	bot.DBIP = "127.0.0.1"
	bot.DBPort = 1433
	bot.DBUser = "sa"
	bot.DBPass = "Password"
	bot.Run()
}
