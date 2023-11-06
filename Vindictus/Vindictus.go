package Vindictus

import (
	"fmt"
	"strings"
	//"bloodreddawn.com/IsetGo/logging"
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

func ProcessCmd(command string) (string, string) {
	ret := "The command you seek does not seem to exist in the Royal Libraries...."
	dmsgt := "default"
	switch {
		case strings.Contains(command, "scroll"):
			dmsgt = "scrollembed"
			ret = FindScroll(command)
		case strings.Contains(command, "banlist"):
			ret = ListBans(setUrl("heroes"))
		case strings.Contains(command, "infusions"):
			ret = "```Infusions: \nATK \nPVP_ATK \nMATK \nPVP_MATK \nBalance \nCritical \nATK_Speed \nATK_Range \nATK_Absolute \nDEF \nPVP_DEF \nDEF_Absolute \nDEF_Destroyed \nSTR \nDEX \nINT \nWILL \nLUCK \nHP \nSTAMINA \nRes_Critical \nTOWN_SPEED \n```"
	}
	return dmsgt, ret
}