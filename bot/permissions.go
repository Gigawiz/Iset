// snippet from: bot/permissions.go
package bot

import (
	"bloodreddawn.com/IsetGo/config"
	"strings"
)

//help,version,givetransskills,settrans,setlevel,giveap,changecharname,findalts,restorechar,deletechar,getcharname,getlogin,getcharid,getuserid,spawn,online,scroll,banlist,infusions,reset2ndary

func hasPermission (userID string, command string) bool {
	perms := config.GetSetting("command.permissions", userID)
	permSlice := strings.Split(perms, ",")
	if (len(permSlice) == 1 && perms == "all") {
		return true
	}
	for _, value := range permSlice { 
		if (command == value) {
			return true
		}
	}
	return false
}

func addPermission (userID string, command string) bool {
	config.UpdateSetting("command.permissions", userID, command)
	if (hasPermission(userID, command)) {
		return true
	}
	return false
}

func removePermission (userID string, command string) bool {
	perms := config.GetSetting("command.permissions", userID)
	permSlice := strings.Split(perms, ",")
	var newPerms []string
	
	for _, value := range permSlice { 
		if (command != value) {
			newPerms = append(newPerms, value)
		}
	}
	
	config.UpdateSetting("command.permissions", userID, strings.Join(newPerms[:], ","))
	
	if (!hasPermission(userID, command)) {
		return true
	}
	return false
}