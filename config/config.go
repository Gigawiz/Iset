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
	UseUnflip bool
	DBIP     string
	DBPort   int
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
	
	section = inidata.Section("mssql.database")  
	DBIP = section.Key("host").String()
	port, err := strconv.Atoi(section.Key("port").String())
	DBPort = port
	DBUser = section.Key("username").String()
	DBPass = section.Key("password").String()
	
	section = inidata.Section("login.server")
	boolVal, err = strconv.ParseBool(section.Key("enabled").String())
	UseLoginServer = boolVal
	
	boolVal, err = strconv.ParseBool(section.Key("pserver").String())
	PServer = boolVal
	PServerURL = section.Key("server").String()
	port, err = strconv.Atoi(section.Key("port").String())
	PServerPort = port
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

func updateSetting() {
	inidata, err := ini.Load("iset.ini")
	if err != nil {
		fmt.Printf("Fail to read file: %v", err)
		os.Exit(1)
	}
	section := inidata.Section("database")
	section.Key("host").SetValue("127.0.0.0")
	section.Key("port").SetValue("3306")

	err = inidata.SaveTo("config.ini")
	if err != nil {
		panic(err)
	}
}