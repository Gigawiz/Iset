//this config file code was ripped from
//https://www.kelche.co/blog/go/ini/
//and modified for use in Iset


package config

import (
	"fmt"
	"strconv"
	"os"
	"gopkg.in/ini.v1"
)

var (
	BotToken string
	BotPrefix string
	BotVersion string
	BotUpdateUrl string
	GithubLatestUrl string
	AutoDLUpdate bool
	UpdateDLUrl string
	
	UseUnflip bool
	DBIP     string
	DBPort   string
	DBUser   string
	DBPass   string
	
	LogToConsole bool
	
	LocalizationFolder string
	
	UseLoginServer bool
	PServer bool
	PServerURL string
	PServerPort int
)

func init() {
	loadSettings()
}

func loadSettings() {

	inidata, err := ini.Load("iset.ini")
	if err != nil {
		fmt.Printf("Fail to read file: %v", err)
		os.Exit(1)
	}
	
	section := inidata.Section("logging")  
	boolVal, err := strconv.ParseBool(section.Key("logToConsole").String())
	LogToConsole = boolVal
	
	section = inidata.Section("localization")  
	LocalizationFolder = section.Key("folder").String()
	
	section = inidata.Section("discord.bot")  
	BotToken = section.Key("token").String()
	BotPrefix = section.Key("prefix").String()
	boolVal, err = strconv.ParseBool(section.Key("use-unflip").String())
	UseUnflip = boolVal
	BotVersion = "2.0.18"
	BotUpdateUrl = "https://raw.githubusercontent.com/Gigawiz/Iset/master/bot/update.dat"
	GithubLatestUrl = "https://github.com/Gigawiz/Iset/releases/latest"
	
	boolVal, err = strconv.ParseBool(section.Key("auto-download-updates").String())
	AutoDLUpdate = boolVal
	
	UpdateDLUrl = "https://github.com/Gigawiz/Iset/releases/latest/download/Iset."
	
	section = inidata.Section("mssql.database")  
	DBIP = section.Key("host").String()
	DBPort = section.Key("port").String()
	DBUser = section.Key("username").String()
	DBPass = section.Key("password").String()
	
	section = inidata.Section("login.server")
	boolVal, err = strconv.ParseBool(section.Key("enabled").String())
	UseLoginServer = boolVal
	
	boolVal, err = strconv.ParseBool(section.Key("pserver").String())
	PServer = boolVal
	PServerURL = section.Key("server").String()
	port, err := strconv.Atoi(section.Key("port").String())
	PServerPort = port
}

func GetSetting(secNm string, settingNm string) string {
	inidata, err := ini.Load("iset.ini")
	if err != nil {
		fmt.Printf("Fail to read file: %v", err)
		os.Exit(1)
	}
	section := inidata.Section(secNm)  
	return section.Key(settingNm).String()
}

func writeSetting() {
	inidata := ini.Empty()
	sec, err := inidata.NewSection("database")
	if err != nil {
		panic(err)
	}
	_, err = sec.NewKey("host", "localhost")
	if err != nil {
		panic(err)
	}

	sec, err = inidata.NewSection("database.options")
	if err != nil {
		panic(err)
	}

	_, err = sec.NewKey("sslmode", "disable")
	if err != nil {
		panic(err)
	}

	err = inidata.SaveTo("config2.ini")
	if err != nil {
		panic(err)
	}
}

func UpdateSetting(sec string, keydat string, valdat string) {
	inidata, err := ini.Load("iset.ini")
	if err != nil {
		fmt.Printf("Fail to read file: %v", err)
		os.Exit(1)
	}
	section := inidata.Section(sec)
	section.Key(keydat).SetValue(valdat)

	err = inidata.SaveTo("iset.ini")
	if err != nil {
		panic(err)
	}
}