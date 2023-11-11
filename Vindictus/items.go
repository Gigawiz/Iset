package Vindictus

import (
	"database/sql"
	"fmt"
	"time"
	"log"
	"bloodreddawn.com/IsetGo/config"
)

func findItemID() string {
	ret := ""
	dbinfo := "server=" + config.DBIP + ";user id=" + config.DBUser + ";password="+config.DBPass+";port="+config.DBPort+";database=heroes;"
	
	db, err := sql.Open("mssql", dbinfo) //try to connect
	if err != nil { //it didnt connect, send an error
		log.Fatal("Open connection failed:", err.Error())
		return ret
	}
	fmt.Printf("Connected!\n") //we connected, lets log that for now
	defer db.Close() //dont close the connection yet
	
	t1 := time.Now() //start a timer to see how long the query takes
	fmt.Printf("Start time: %s\n", t1) //log the start time to console
	
	
	
	
	stmt, err := db.Prepare("SELECT TOP 1 * FROM Item WHERE OwnerID = ? AND ItemClass LIKE ?")
	if err != nil {
		log.Fatal(err)
	}
	defer stmt.Close()
	var ItemID string
	err = stmt.QueryRow(1).Scan(&ItemID)
	if err != nil {
		log.Fatal(err)
	}
	fmt.Println(ItemID)
	ret = ItemID
	
	//calculate time taken
	t2 := time.Since(t1)
	fmt.Printf("The query took: %s\n", t2) //log to console
	
	
	
	return ret

}

func clearQueue() bool {
	dbinfo := "server=" + config.DBIP + ";user id=" + config.DBUser + ";password="+config.DBPass+";port="+config.DBPort+";database=heroes;"
	
	db, err := sql.Open("mssql", dbinfo) //try to connect
	if err != nil { //it didnt connect, send an error
		log.Fatal("Open connection failed:", err.Error())
		return false
	}
	fmt.Printf("Connected!\n") //we connected, lets log that for now
	defer db.Close() //dont close the connection yet
	
	t1 := time.Now() //start a timer to see how long the query takes
	fmt.Printf("Start time: %s\n", t1) //log the start time to console
	
	stmt, err := db.Prepare("DELETE FROM QueuedItem")
	if err != nil {
		log.Fatal(err)
		return false
	}
	defer stmt.Close()
	stmt.QueryRow()

	//calculate time taken
	t2 := time.Since(t1)
	fmt.Printf("The query took: %s\n", t2) //log to console
	
	return true
}

func SendMail(characterName string, item string, count int, mailMsg string, mailSender string, staffMember string) string {
	ret := "check console"
	var playerList []string
	if (characterName == "all") {
		playerList = PlayerList("name", "all")
	} else if (characterName == "allonline") {
		playerList = PlayerList("name", "online")
	} else {
		playerList = append(playerList, characterName)
	}
	fmt.Println(playerList)
	
	return ret
}