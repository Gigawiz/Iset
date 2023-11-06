package bot

import (
	"github.com/bwmarrin/discordgo"
	//"bloodreddawn.com/IsetGo/logging"
	"strings"
)

func displayHelp(message string) *discordgo.MessageSend {
	var cmdhlp = strings.ReplaceAll(message, "!help ", "")
	// Build Discord embed response
	embed := &discordgo.MessageSend{
		Embeds: []*discordgo.MessageEmbed{{
				Type:        discordgo.EmbedTypeRich,
				Title:       "Command Information for " +cmdhlp,
				Description: "Usage: /" +cmdhlp + " var1 var2 var3",
				Fields: []*discordgo.MessageEmbedField{
					{
						Name:   "First Item",
						Value:  "First Value",
						Inline: true,
					},
					{
						Name:   "Second Item",
						Value:  "Second Value",
						Inline: true,
					},
					{
						Name:   "Third Item",
						Value:  "Third Value",
						Inline: true,
					},
					{
						Name:   "Fourth Item",
						Value:  "Fourth Value",
						Inline: true,
					},
				},
			},
		},
	}

	return embed
}