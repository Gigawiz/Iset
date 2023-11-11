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

func characterExists(characterName string) bool {

	dbinfo := "server=" + config.DBIP + ";user id=" + config.DBUser + ";password="+config.DBPass+";port="+config.DBPort+";database=heroes;"
	db, err := sql.Open("mssql", dbinfo) //try to connect
	if err != nil { //it didnt connect, send an error
		log.Fatal("Open connection failed:", err.Error())
		return false
	}
	defer db.Close() //dont close the connection yet
	
	var name string
	err = db.QueryRow("Select Name from CharacterInfo where Name = ?", characterName).Scan(&name)
	if err != nil {
		return false
	}
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

func changeUserName(inputStr string) string {
	ret := "Unable to change character name!"
	//split the input on whitespace
	words := strings.Fields(inputStr)
	fmt.Println(words, len(words))
	//check that the correct number of parameters exists in the new array
	if (len(words) <= 1 || len(words) > 2) {
		ret = "You have not supplied the correct amount of values!\n command syntax: changecharname <old name> <new name>"
		return ret
	}
	
	//check that the character actually exists in the database?
	exists := characterExists(words[0])
	
	if (!exists) {
		ret = "Could not find the character " + words[0] + "! Check that you have spelled the username correctly and try again!"
		return ret
	}
	
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
	
	stmt, err := db.Prepare("UPDATE CharacterInfo SET Name = ? WHERE Name = ?")
	if err != nil {
		ret = err.Error()
		return ret
	}
	defer stmt.Close()
	_, err = stmt.Query(words[1], words[0])
	if err != nil {
		ret = err.Error()
		return ret
	}
	
	ret = "Successfully changed the character named '" + words[0] + "' to '" + words[1] + "'!"
	
	//calculate time taken
	t2 := time.Since(t1)
	fmt.Printf("The query took: %s\n", t2) //log to console
	
	return ret
}

func giveAPToChar(inputStr string) string {
	ret := "Unable to give AP to player!"
	//split the input on whitespace
	words := strings.Fields(inputStr)
	fmt.Println(words, len(words))
	//check that the correct number of parameters exists in the new array
	if (len(words) <= 1 || len(words) > 2) {
		ret = "You have not supplied the correct amount of values!\n command syntax: giveap <character name> <amount>"
		return ret
	}
	
	//check that the character actually exists in the database?
	exists := characterExists(words[0])
	
	if (!exists) {
		ret = "Could not find the character " + words[0] + "! Check that you have spelled the username correctly and try again!"
		return ret
	}
	
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
	
	stmt, err := db.Prepare("UPDATE CharacterInfo SET AP=AP + ? WHERE Name = ?")
	if err != nil {
		ret = err.Error()
		return ret
	}
	defer stmt.Close()
	_, err = stmt.Query(words[1], words[0])
	if err != nil {
		ret = err.Error()
		return ret
	}
	
	ret = "Successfully gave " + words[1] + "AP to '" + words[0] + "'!"
	
	//calculate time taken
	t2 := time.Since(t1)
	fmt.Printf("The query took: %s\n", t2) //log to console
	
	return ret
}

func setCharLevel(inputStr string) string {
	ret := "Unable to set players level!"
	//split the input on whitespace
	words := strings.Fields(inputStr)
	fmt.Println(words, len(words))
	//check that the correct number of parameters exists in the new array
	if (len(words) <= 1 || len(words) > 2) {
		ret = "You have not supplied the correct amount of values!\n command syntax: setlevel <character name> <level>"
		return ret
	}
	
	//check that the character actually exists in the database?
	exists := characterExists(words[0])
	
	if (!exists) {
		ret = "Could not find the character " + words[0] + "! Check that you have spelled the username correctly and try again!"
		return ret
	}
	
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
	
	stmt, err := db.Prepare("UPDATE CharacterInfo SET Level = ? WHERE Name = ?")
	if err != nil {
		ret = err.Error()
		return ret
	}
	defer stmt.Close()
	_, err = stmt.Query(words[1], words[0])
	if err != nil {
		ret = err.Error()
		return ret
	}
	
	ret = "Successfully set " + words[0] + "'s level to " + words[1] + "!"
	
	//calculate time taken
	t2 := time.Since(t1)
	fmt.Printf("The query took: %s\n", t2) //log to console
	
	return ret
}

func setTrans(inputStr string) string {
	ret := "Unable to set players level!"
	//split the input on whitespace
	words := strings.Fields(inputStr)
	fmt.Println(words, len(words))
	//check that the correct number of parameters exists in the new array
	if (len(words) <= 1 || len(words) > 2) {
		ret = "You have not supplied the correct amount of values!\n command syntax: settrans <character name> <paladin|knight>"
		return ret
	}
	
	//check that the character actually exists in the database?
	exists := characterExists(words[0])
	
	if (!exists) {
		ret = "Could not find the character " + words[0] + "! Check that you have spelled the username correctly and try again!"
		return ret
	}
	
	charID := getCharacterIdFromName(words[0])
	
	transType := 0
	
	if (words[1] == "paladin") {
		transType = 0
	} else if (words[1] == "knight") {
		transType = 1
	}
	
	if (hasTrans(charID)) {
		ret = "The player " + words[0] + " already has a transformation!"
		return ret
	}
	
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
	
	stmt, err := db.Prepare("INSERT INTO Vocation ([CID] ,[VocationClass] ,[VocationLevel] ,[VocationExp] ,[LastTransform]) VALUES (?,?,40,0,GETDATE())")
	if err != nil {
		ret = err.Error()
		return ret
	}
	defer stmt.Close()
	_, err = stmt.Query(charID, transType)
	if err != nil {
		ret = err.Error()
		return ret
	}
	transtp := "Paladin"
	if (words[1] == "knight") {
		transtp = "Dark Knight"
	}
	ret = words[0] + " is now a level 40 " + transtp + "!"
	
	//calculate time taken
	t2 := time.Since(t1)
	fmt.Printf("The query took: %s\n", t2) //log to console
	
	return ret
}

func hasTrans(charID string) bool {
	dbinfo := "server=" + config.DBIP + ";user id=" + config.DBUser + ";password="+config.DBPass+";port="+config.DBPort+";database=heroes;"
	db, err := sql.Open("mssql", dbinfo) //try to connect
	if err != nil { //it didnt connect, send an error
		log.Fatal("Open connection failed:", err.Error())
		return false
	}
	defer db.Close() //dont close the connection yet
	
	var name string
	err = db.QueryRow("Select CID from Vocation where CID = ?", charID).Scan(&name)
	if err != nil {
		return false
	}
	return true
}

func transType(charID string) string {
	dbinfo := "server=" + config.DBIP + ";user id=" + config.DBUser + ";password="+config.DBPass+";port="+config.DBPort+";database=heroes;"
	db, err := sql.Open("mssql", dbinfo) //try to connect
	if err != nil { //it didnt connect, send an error
		log.Fatal("Open connection failed:", err.Error())
		return "error"
	}
	defer db.Close() //dont close the connection yet
	
	var VocationClass string
	err = db.QueryRow("Select VocationClass from Vocation where CID = ?", charID).Scan(&VocationClass)
	if err != nil {
		return "error"
	}
	return VocationClass
}

func giveTransSkills(charName string) string {
	ret := "Unable to give skills to " + charName + "!"
	
	//check that the character actually exists in the database?
	exists := characterExists(charName)
	
	if (!exists) {
		ret = "Could not find the character " + charName + "! Check that you have spelled the username correctly and try again!"
		return ret
	}
	
	charID := getCharacterIdFromName(charName)
	
	if (!hasTrans(charID)) {
		ret = "The player " + charName + " does not have any transformation! Please set that first with the settrans command!"
		return ret
	}
	
	playerTrans := transType(charID)
	
	darkKnight := [13]string{"voc_skill_dk_hp_up", "voc_skill_dk_atk_up", "voc_skill_dk_down", "voc_skill_dk_transform_01", "voc_skill_dk_life_drain", "voc_skill_dk_party_aura", "voc_skill_dk_atk_up_2", "voc_skill_dk_hp_up_2", "voc_skill_dk_active_tentacle", "voc_skill_dk_stamina_recovery", "voc_skill_dk_hp_recovery", "voc_skill_dk_stat_up_2", "voc_skill_dk_active_tentacle_plus" }
	paladin := [10]string { "voc_skill_pl_hp_up", "voc_skill_pl_def_up", "voc_skill_pl_party_aura", "voc_skill_pl_down", "voc_skill_pl_homing_missile", "voc_skill_pl_homing_missile_plus", "voc_skill_pl_stone", "voc_skill_pl_stat_up", "voc_skill_pl_transform_01", "voc_skill_pl_hp_up_2"}
	
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
	
	if (playerTrans == "0") {
		for _, value := range paladin { 
				stmt, err := db.Prepare("INSERT INTO VocationSkill ([CID] ,[SkillID] ,[Rank]) VALUES (?,?,5)")
				if err != nil {
					ret = err.Error()
					return ret
				}
				defer stmt.Close()
				_, err = stmt.Query(charID, value)
				if err != nil {
					ret = err.Error()
					return ret
				}
			} 
	} else {
		for _, value := range darkKnight { 
				stmt, err := db.Prepare("INSERT INTO VocationSkill ([CID] ,[SkillID] ,[Rank]) VALUES (?,?,5)")
				if err != nil {
					ret = err.Error()
					return ret
				}
				defer stmt.Close()
				_, err = stmt.Query(charID, value)
				if err != nil {
					ret = err.Error()
					return ret
				}
		} 
	}
	
	ret = charName + " has been given all transformation skills appropriate to their transformation type!"
	
	//calculate time taken
	t2 := time.Since(t1)
	fmt.Printf("The query took: %s\n", t2) //log to console
	
	return ret
}