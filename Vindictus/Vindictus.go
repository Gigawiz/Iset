package Vindictus

import (
	"strings"
	//"fmt"
	//"bloodreddawn.com/IsetGo/logging"
)

var (
	DBIP string
	DBPort int
	DBUser string
	DBPass string
	retstr string
)

func ProcessCmd(command string) (string, string) {
	ret := "The command you seek does not seem to exist in the Royal Libraries...."
	dmsgt := "default"
	switch {
		case strings.Contains(command, "findalts") :
			var accountID = strings.ReplaceAll(command, "findalts ", "") //if no data is entered, this will not remove the command from the string because of the space. will need this check for each switch statement
			if (accountID == "findalts") {
				ret = "You have not entered a character name or ID!"
			} else {
				ret = strings.Join(PlayerAlts("name", accountID),"\n")
				if (len(ret) <= 0) {
					ret = "Unable to locate any alternate characters!"
				}
			}
			return dmsgt, ret
		case strings.Contains(command, "restorechar") :
			var accountID = strings.ReplaceAll(command, "restorechar ", "") //if no data is entered, this will not remove the command from the string because of the space. will need this check for each switch statement
			if (accountID == "restorechar") {
				ret = "You have not entered a character name or ID!"
			} else {
				ret = restoreCharacter(accountID)
			}
			return dmsgt, ret
		case strings.Contains(command, "deletechar") :
			var accountID = strings.ReplaceAll(command, "deletechar ", "") //if no data is entered, this will not remove the command from the string because of the space. will need this check for each switch statement
			if (accountID == "deletechar") {
				ret = "You have not entered a character name or ID!"
			} else {
				ret = deleteCharacter(accountID)
			}
			return dmsgt, ret
		case strings.Contains(command, "getcharname") :
			var accountID = strings.ReplaceAll(command, "getcharname ", "") //if no data is entered, this will not remove the command from the string because of the space. will need this check for each switch statement
			if (accountID == "getcharname") {
				ret = "You have not entered a user ID!"
			} else {
				ret = getCharacterNameFromID(accountID)
			}
			return dmsgt, ret
		case strings.Contains(command, "getlogin") :
			var accountID = strings.ReplaceAll(command, "getlogin ", "") //if no data is entered, this will not remove the command from the string because of the space. will need this check for each switch statement
			if (accountID == "getlogin") {
				ret = "You have not entered a user ID!"
			} else {
				ret = getAccountNameFromID(accountID)
			}
			return dmsgt, ret
		case strings.Contains(command, "getcharid") :
			var charName = strings.ReplaceAll(command, "getcharid ", "") //if no data is entered, this will not remove the command from the string because of the space. will need this check for each switch statement
			if (charName == "getcharid") {
				ret = "You have not entered a username!"
			} else {
				ret = getCharacterIdFromName(charName)
			}
			return dmsgt, ret
		case strings.Contains(command, "getuserid") :
			var charName = strings.ReplaceAll(command, "getuserid ", "") //if no data is entered, this will not remove the command from the string because of the space. will need this check for each switch statement
			if (charName == "getuserid") {
				ret = "You have not entered a username!"
			} else {
				ret = getUserIDFromCharName(charName)
			}
			return dmsgt, ret
		case strings.Contains(command, "spawn"):
			ret = SendMail("allonline", "test", 1, "test message", "Iset", "Gigawiz")
			return dmsgt, ret
		case strings.Contains(command, "online"):
			ret := strings.Join(PlayerList("name", "online"),"\n")
			if (len(ret) <= 0) {
				ret = "There are no players online at this time."
			}
			return dmsgt, ret
		case strings.Contains(command, "scroll"):
			dmsgt = "scrollembed"
			ret = FindScroll(command)
			return dmsgt, ret
		case strings.Contains(command, "banlist"):
			tmp := ListBans()
			dmsgt = "banembed"
			ret = tmp
			return dmsgt, ret
		case strings.Contains(command, "infusions"):
			ret = "```Infusions: \nATK \nPVP_ATK \nMATK \nPVP_MATK \nBalance \nCritical \nATK_Speed \nATK_Range \nATK_Absolute \nDEF \nPVP_DEF \nDEF_Absolute \nDEF_Destroyed \nSTR \nDEX \nINT \nWILL \nLUCK \nHP \nSTAMINA \nRes_Critical \nTOWN_SPEED \n```"
			return dmsgt, ret
		case strings.Contains(command, "reset2ndary"):
			ret = "Secondary password should be reset! Please contact an admin if you continue to have issues!"
			if (!ResetSecondary(command)) {
				ret = "Failed to reset secondary password! Please ensure you have entered the correct username. If this issue persists, please contact a staff member!"
			}
			return dmsgt, ret
		default:
			return dmsgt, ret
	}

}