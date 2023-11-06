package Vindictus

import (
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

func ProcessCmd(command string) (string, string) {
	ret := "The command you seek does not seem to exist in the Royal Libraries...."
	dmsgt := "default"
	switch {
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