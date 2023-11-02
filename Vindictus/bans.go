package Vindictus

import (
	"database/sql"
	//"github.com/denisenkom/go-mssqldb"
	"log"
	"fmt"
	"time"
	_ "github.com/denisenkom/go-mssqldb"
)

var (
	SqlQryStr string
)

func ListBans(dbinfo string) string {
	ret := "No Bans! YAY!" //just some placeholder text
	conn, err := sql.Open("mssql", dbinfo) //try to connect
	if err != nil { //it didnt connect, send an error
		log.Fatal("Open connection failed:", err.Error())
		ret = "Open connection failed: "+ err.Error()
	}
	fmt.Printf("Connected!\n") //we connected, lets log that for now
	defer conn.Close() //dont close the connection yet
	
	t1 := time.Now() //start a timer to see how long the query takes
	fmt.Printf("Start time: %s\n", t1) //log the start time to console
	
	//execute the command
	result, err := conn.Prepare("SELECT * FROM UserBan")
	if err != nil {
		fmt.Println("Error preparing query: " + err.Error())
	}

	row := result.QueryRow()
	var sum string
	err = row.Scan(&sum)
	fmt.Printf("Sum: %s\n", sum)
	
	//calculate time taken
	t2 := time.Since(t1)
	fmt.Printf("The query took: %s\n", t2) //log to console
	
	//return our result
	return ret
}