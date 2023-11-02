package Vindictus

import (
	"fmt"
	"strings"
)

var (
	DBIP string
	DBPort int
	DBUser string
	DBPass string
	retstr string
)

func setUrl(database string) string {  //i cant set a default value, so the database name always needs to be passed to this functon
	connString := fmt.Sprintf("server=%s;user id=%s;password=%s;port=%d;database=%s;",
		DBIP, DBUser, DBPass, DBPort, database)
	return connString
}

func TestFunc() string {
	fmt.Println("Vindi Package Called!")
	retstr = "Vindi Package Called"
	return retstr
}

func ProcessCmd(command string) string {
	ret := "The command you seek does not seem to exist in the Royal Libraries...."
	var cmdCln = strings.ReplaceAll(command, "$vindictus ", "")
	fmt.Println("Command: " + cmdCln)
	switch {
		case strings.Contains(cmdCln, "enchants"):
			ret = getEnchants()
		case strings.Contains(cmdCln, "scroll"):
			ret = translateEnchant(strings.ReplaceAll(cmdCln, "scroll ", ""))
		case strings.Contains(cmdCln, "banlist"):
			ret = ListBans(setUrl("heroes"))
	}
	return ret
}