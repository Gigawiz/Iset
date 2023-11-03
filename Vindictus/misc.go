package Vindictus

import (
  "github.com/nicksnyder/go-i18n/v2/i18n"
  "golang.org/x/text/language"
  "encoding/json"
  //"bloodreddawn.com/IsetGo/logging"
)

var localizer *i18n.Localizer  //1
var bundle *i18n.Bundle  //2

func init() {  //3
  bundle = i18n.NewBundle(language.English)  //4
  bundle.RegisterUnmarshalFunc("json", json.Unmarshal)  //5
  bundle.LoadMessageFile("locale/en.json")  //6
  localizer = i18n.NewLocalizer(bundle, language.English.String(), language.French.String())  //8
}

func getEnchants() string {
	localizeConfigWelcome := i18n.LocalizeConfig{
	  MessageID: "enchants",  //1
	}
	localizationUsingJson, _ := localizer.Localize(&localizeConfigWelcome)  //2
	return localizationUsingJson
}

func translateEnchant(enchant string) string {
	localizeConfigWelcome := i18n.LocalizeConfig{
		MessageID: enchant,  //1
	}
	localizationUsingJson, _ := localizer.Localize(&localizeConfigWelcome)  //2
	return localizationUsingJson
}