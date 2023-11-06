package Vindictus

import (
  "os"
  "io"
  "github.com/antchfx/xmlquery"
  "strings"
  "github.com/bwmarrin/discordgo"
  "regexp"
)

func FindScroll(enchant string) string {
	f, err := os.Open("locale/enchantments.xml")
	if err == io.EOF {
		return "ERROR OPENING ENCHANTMENTS.XML"
	}
	doc, err := xmlquery.Parse(f)
	if err != nil {
		return "ERROR PARSING ENCHANTMENTS.XML"
	}
	enchant = strings.ReplaceAll(enchant, "scroll ", "")
	var ret = ""
	list, err := xmlquery.QueryAll(doc, "//scroll[@id='"+enchant+"']")
	if (err != nil || len(list) <= 0) {
		ret = "error"
		return ret
	}
	var enchantData = xmlquery.Find(doc, "//scroll[@id='"+enchant+"']")
	ret = enchantData[0].InnerText()
	return ret
}

func ScrollEmbed(message string) *discordgo.MessageSend {
	if (!strings.Contains(message, "!!")) {
		message = strings.ReplaceAll(message, " ", "")
	}
	message = strings.ReplaceAll(message, "	", "")
	re := regexp.MustCompile(`\r?\n`)
	message = re.ReplaceAllString(message[1:], ">")
	//lets get the variables
	parts := strings.Split(message, ">")
	// Build Discord embed response
	embed := &discordgo.MessageSend{
		Embeds: []*discordgo.MessageEmbed{{
				Type:        discordgo.EmbedTypeRich,
				Title:       strings.Title(strings.ToLower(parts[0])) +" Enchant Scroll",
				Description: "This Enchant Scroll infuses magical power into your equipment.\r\n Take this scroll to Brynn to unleash its potential.",
				Fields: []*discordgo.MessageEmbedField{
					{
						Name:   "Enchant Type",
						Value:  parts[1] + " " + parts[3],
						Inline: true,
					},
					{
						Name:   "Equipment Type",
						Value:  parts[2],
						Inline: true,
					},
					{
						Name:   "Enchant Effect",
						Value:  strings.ReplaceAll(parts[5], "!!", "\n"),
						Inline: false,
					},
					{
						Name:   "Spawn Code",
						Value:  parts[4],
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