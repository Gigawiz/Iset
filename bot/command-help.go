package bot

import (
	"github.com/bwmarrin/discordgo"
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

func scrollEmbed(enchant string, message string) *discordgo.MessageSend {
	//lets get the variables
	parts := strings.Split(message, ">")
	// Build Discord embed response
	embed := &discordgo.MessageSend{
		Embeds: []*discordgo.MessageEmbed{{
				Type:        discordgo.EmbedTypeRich,
				Title:       strings.Title(strings.ToLower(enchant)) +" Enchant Scroll",
				Description: "This Enchant Scroll infuses magical power into your equipment.\r\n Take this scroll to Brynn to unleash its potential.",
				Fields: []*discordgo.MessageEmbedField{
					{
						Name:   "Enchant Type",
						Value:  parts[1] + " " + parts[2],
						Inline: true,
					},
					{
						Name:   "Equipment Type",
						Value:  parts[3],
						Inline: true,
					},
					{
						Name:   "Enchant Effect",
						Value:  parts[4],
						Inline: false,
					},
					{
						Name:   "Spawn Code",
						Value:  "enchant_scroll[" + strings.ToUpper(parts[1]) + ":" + strings.ToLower(parts[0]) + "]",
						Inline: false,
					},
				},
				Thumbnail: &discordgo.MessageEmbedThumbnail{
					URL: "https://i.imgur.com/yWD33tg.png",
				},
				Footer: &discordgo.MessageEmbedFooter {
					Text: "This item can be traded",
					IconURL: "https://i.imgur.com/ERrgglj.png",
				},
			},
		},
	}
	return embed
}