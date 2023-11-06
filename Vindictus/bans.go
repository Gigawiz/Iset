package Vindictus

import (
	"database/sql"
	"log"
	"fmt"
	"time"
	_ "github.com/denisenkom/go-mssqldb"
	"bloodreddawn.com/IsetGo/config"
	"github.com/bwmarrin/discordgo"
)


func ListBans() string {
	dbinfo := "server=" + config.DBIP + ";user id=" + config.DBUser + ";password="+config.DBPass+";port="+config.DBPort+";database=heroes;"
	var ret string //just some placeholder text
	db, err := sql.Open("mssql", dbinfo) //try to connect
	if err != nil { //it didnt connect, send an error
		log.Fatal("Open connection failed:", err.Error())
		return ret
	}
	fmt.Printf("Connected!\n") //we connected, lets log that for now
	defer db.Close() //dont close the connection yet
	
	t1 := time.Now() //start a timer to see how long the query takes
	fmt.Printf("Start time: %s\n", t1) //log the start time to console
	
	var (
		ID string
		Reason string
	)
	rows, err := db.Query("select ID, Reason from UserBan WHERE Status = 4")
	if err != nil {
		log.Fatal(err)
	}
	defer rows.Close()
	for rows.Next() {
		err := rows.Scan(&ID, &Reason)
		if err != nil {
			log.Fatal(err)
		}
		//log.Println(ID, Reason)
		ret = ret + ID + " - " + Reason + "\n"
	}
	err = rows.Err()
	if err != nil {
		log.Fatal(err)
	}
	
	//calculate time taken
	t2 := time.Since(t1)
	fmt.Printf("The query took: %s\n", t2) //log to console
	
	if (len(ret) == 0) {
		ret = "No Bans! YAY!"
	}
	
	//return our result
	return ret
}

func BanEmbed(banlist string) *discordgo.MessageSend {
	// Build Discord embed response
	embed := &discordgo.MessageSend{
		Embeds: []*discordgo.MessageEmbed{{
				Type:        discordgo.EmbedTypeRich,
				Title:       "Ban List",
				Description: banlist,
			},
		},
	}

	return embed
}