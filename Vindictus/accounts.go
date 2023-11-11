package Vindictus

import (
	"database/sql"
	"log"
	"fmt"
	"time"
	"strings"
	"strconv"
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

func PlayerList(returnDataType string, searchType string) []string {
	var ret []string
	
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
	
	var (
		ID string
		Name string
	)
	var qry = "SELECT ID, Name FROM CharacterInfo WHERE Status = 0"
	if (searchType == "online") {
		qry = qry + " AND IsConnected=1"
	}
	rows, err := db.Query(qry)
	if err != nil {
		log.Fatal(err)
	}
	defer rows.Close()
	for rows.Next() {
		err := rows.Scan(&ID, &Name)
		if err != nil {
			log.Fatal(err)
		}
		//log.Println(ID, Reason)
		if (returnDataType == "id") {
			ret = append(ret, ID)
		} else {
			ret = append(ret, Name)
		}
	}
	err = rows.Err()
	if err != nil {
		log.Fatal(err)
	}
	
	//calculate time taken
	t2 := time.Since(t1)
	fmt.Printf("The query took: %s\n", t2) //log to console
	
	return ret
}

func PlayerAlts(returnDataType string, characterName string) []string {
	var ret []string
	userID := getUserIDFromCharName(characterName)
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
	
	var (
		ID string
		Name string
	)

	stmt, err := db.Prepare("SELECT ID, Name FROM CharacterInfo WHERE UID = ? ORDER BY STATUS ASC")
	if err != nil {
		log.Fatal(err)
	}
	defer stmt.Close()
	
	rows, err := stmt.Query(userID)
	if err != nil {
		log.Fatal(err)
	}
	defer rows.Close()
	
	
	for rows.Next() {
		err := rows.Scan(&ID, &Name)
		if err != nil {
			log.Fatal(err)
		}
		//log.Println(ID, Reason)
		if (returnDataType == "id") {
			ret = append(ret, ID)
		} else {
			ret = append(ret, Name)
		}
	}
	err = rows.Err()
	if err != nil {
		log.Fatal(err)
	}
	
	//calculate time taken
	t2 := time.Since(t1)
	fmt.Printf("The query took: %s\n", t2) //log to console
	
	return ret
}


func getUserIDFromCharName(charName string) string {
	var ret = "Invalid Character Name!"
	if _, err := strconv.Atoi(charName); err == nil {
		return charName
	}
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
	
	
	stmt, err := db.Prepare("Select UID from CharacterInfo where Name = ?")
	if err != nil {
		log.Fatal(err)
	}
	defer stmt.Close()
	var name string
	err = stmt.QueryRow(charName).Scan(&name)
	if err != nil {
		log.Fatal(err)
	}

	ret = name
	
	
	//calculate time taken
	t2 := time.Since(t1)
	fmt.Printf("The query took: %s\n", t2) //log to console
	
	return ret
}

func getCharacterIdFromName(charName string) string {
	var ret = "Invalid Character Name!"
	if _, err := strconv.Atoi(charName); err == nil {
		return charName
	}
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
	
	
	stmt, err := db.Prepare("Select ID from CharacterInfo where Name = ?")
	if err != nil {
		log.Fatal(err)
	}
	defer stmt.Close()
	var name string
	err = stmt.QueryRow(charName).Scan(&name)
	if err != nil {
		log.Fatal(err)
	}
	
	ret = name
	
	
	//calculate time taken
	t2 := time.Since(t1)
	fmt.Printf("The query took: %s\n", t2) //log to console
	
	return ret
}

func getAccountNameFromID(accountID string) string {
	var ret = "Invalid Character Name!"
	if _, err := strconv.Atoi(accountID); err == nil {
		return accountID
	}
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
	
	
	stmt, err := db.Prepare("SELECT Name FROM [User] WHERE ID = ?")
	if err != nil {
		log.Fatal(err)
	}
	defer stmt.Close()
	var name string
	err = stmt.QueryRow(accountID).Scan(&name)
	if err != nil {
		log.Fatal(err)
	}
	
	ret = name
	
	
	//calculate time taken
	t2 := time.Since(t1)
	fmt.Printf("The query took: %s\n", t2) //log to console
	
	return ret
}

func getCharacterNameFromID(characterID string) string {
	var ret = "Invalid Character Name!"
	if _, err := strconv.Atoi(characterID); err != nil {
		return characterID
	}
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
	
	
	stmt, err := db.Prepare("Select Name from CharacterInfo where ID = ?")
	if err != nil {
		log.Fatal(err)
	}
	defer stmt.Close()
	var name string
	err = stmt.QueryRow(characterID).Scan(&name)
	if err != nil {
		log.Fatal(err)
	}
	
	ret = name
	
	
	//calculate time taken
	t2 := time.Since(t1)
	fmt.Printf("The query took: %s\n", t2) //log to console
	
	return ret
}

func deleteCharacter(characterName string) string {
	ret := "Failed to delete " + characterName
	charName := getCharacterNameFromID(characterName)
	
	dbinfo := "server=" + config.DBIP + ";user id=" + config.DBUser + ";password="+config.DBPass+";port="+config.DBPort+";database=heroes;"
	
	db, err := sql.Open("mssql", dbinfo) //try to connect
	if err != nil { //it didnt connect, send an error
		log.Fatal("Open connection failed:", err.Error())
		ret = err.Error()
		return ret
	}
	fmt.Printf("Connected!\n") //we connected, lets log that for now
	defer db.Close() //dont close the connection yet
	
	t1 := time.Now() //start a timer to see how long the query takes
	fmt.Printf("Start time: %s\n", t1) //log the start time to console
	
	stmt, err := db.Prepare("UPDATE CharacterInfo SET Status=1, DeleteTime=CURRENT_TIMESTAMP WHERE Name = ?")
	if err != nil {
		ret = err.Error()
		return ret
	}
	defer stmt.Close()
	_, err = stmt.Query(charName)
	if err != nil {
		ret = err.Error()
		return ret
	}
	
	ret = "Successfully deleted the character named '" + charName + "' (If it existed)!"
	
	//calculate time taken
	t2 := time.Since(t1)
	fmt.Printf("The query took: %s\n", t2) //log to console
	
	return ret
}

func restoreCharacter(characterName string) string {
	ret := "Failed to delete " + characterName
	charName := getCharacterNameFromID(characterName)
	
	dbinfo := "server=" + config.DBIP + ";user id=" + config.DBUser + ";password="+config.DBPass+";port="+config.DBPort+";database=heroes;"
	
	db, err := sql.Open("mssql", dbinfo) //try to connect
	if err != nil { //it didnt connect, send an error
		log.Fatal("Open connection failed:", err.Error())
		ret = err.Error()
		return ret
	}
	fmt.Printf("Connected!\n") //we connected, lets log that for now
	defer db.Close() //dont close the connection yet
	
	t1 := time.Now() //start a timer to see how long the query takes
	fmt.Printf("Start time: %s\n", t1) //log the start time to console
	
	stmt, err := db.Prepare("UPDATE CharacterInfo SET Status=0, DeleteTime=NULL WHERE Name = ?")
	if err != nil {
		ret = err.Error()
		return ret
	}
	defer stmt.Close()
	_, err = stmt.Query(charName)
	if err != nil {
		ret = err.Error()
		return ret
	}
	
	ret = "Successfully restored the character named '" + charName + "' (If it existed)!"
	
	//calculate time taken
	t2 := time.Since(t1)
	fmt.Printf("The query took: %s\n", t2) //log to console
	
	return ret
}