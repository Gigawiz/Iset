package Vindictus

import (
	"database/sql"
	"log"
	"fmt"
	"time"
	"strings"
	_ "github.com/denisenkom/go-mssqldb"
	"bloodreddawn.com/IsetGo/config"
)


func ResetSecondary(userName string) bool {
	userName = strings.ReplaceAll(userName, "reset2ndary ", "")
	if (!CheckExists(userName)) {
		return false
	}
	
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

	stmt, err := db.Prepare("UPDATE [User] SET SecondPassword=NULL WHERE Name=?")
	if err != nil {
		log.Fatal(err)
		return false
	}
	res, err := stmt.Exec(userName)
	if err != nil {
		log.Fatal(err)
		return false
	}
	
	_ = res
	//calculate time taken
	t2 := time.Since(t1)
	fmt.Printf("The query took: %s\n", t2) //log to console
	return true
}

func CheckExists(userName string) bool {

	dbinfo := "server=" + config.DBIP + ";user id=" + config.DBUser + ";password="+config.DBPass+";port="+config.DBPort+";database=heroes;"
	db, err := sql.Open("mssql", dbinfo) //try to connect
	if err != nil { //it didnt connect, send an error
		log.Fatal("Open connection failed:", err.Error())
		return false
	}
	defer db.Close() //dont close the connection yet
	
	var name string
	err = db.QueryRow("SELECT Name FROM [User] WHERE Name = ?", userName).Scan(&name)
	if err != nil {
		return false
	}
	return true
}