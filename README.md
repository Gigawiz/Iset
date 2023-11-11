# Iset - A Discord Administration tool for Vindictus Servers

Iset was created while I was running a "public" private vindictus server. There was a need for the ability for my staff members to run commands without having direct access to the hardware or RDP. For example, Banning players, Restoring broken items from enhancment/enchantment, etc. Originally, these actions would have been done by logging in to RDP and either editing the databases directly, or using HeroesOPTool to run commands. Unfortunately, I learned the hard way that more people having access to the live files of your server is not a good thing. After Delteros and Schitelle had a falling out with me on discord, Delteros attempted to wipe all my data assuming it would ruin the server. Fortunately, I had a two day old backup, and not much came of that. However it pointed out the need to make some changes to how I ran the server. It was at this point that Iset became a staple tool for running the server. At the time it had a permissions system that allowed my staff members to spawn items, ban/unban players, upload changes to the heroesContents file (it even made a backup that it could restore if something got screwed!), and eventually even had player runnable commands.

After the server closed down, I let the project die out. Recently, one of my loyal players asked if I could revive the project (iset) for another private server that he plays on. Thus was the rebirth of Iset.... Iset 2.0. She's a work in progress. With all of the changes to .Net between then and now, I was basically starting from scratch, so I figured I'd start it in GoLang. This was mainly done so that I could integrate NMServer into the source code, with the intent to allow user management from not only discord, but web access too. In any case, that was a rant and you're here for.... well I have no idea but you've read this far, so why not read some features?

## Features

Been working hard on porting all of Iset 1.0's features, here's what is working currently;
- Config to set custom variables
- Can specify your own bot prefix
- Have iset unflip tables if you like (or disable it in config)
- NMServer login server working, can be disabled in config
- Can notify of available updates, as well as download them (run $iset version)
- Permissions system, decide what discord users can run what commands, or give them the "all" permission and let them run wild!
- Finished porting all functions from ItemFunctions.cs

### Commands:
- Online : List online players
  - Syntax: $iset online
- FindAlts : List all characters that belong to the same account as the given character name
  - Syntax: $iset findalts [character name]
- RestoreChar : Un-delete a character that has been marked for deletion
  - Syntax: $iset restorechar [character name]
- DeleteChar : Soft-delete a character from a user's account
  - Syntax: $iset deletechar [character name]
- GetCharName : Get the name of a character from a given Character ID
  - Syntax: $iset getcharname [character ID]
- GetLogin : Get the login username of an account based on a given character name
  - Syntax: $iset getlogin [character name]
- GetCharID : Get the character ID of a given character
  - Syntax: $iset getcharid [character name]
- GetUserID : Get the UserID for the account of a specified character
  - Syntax: $iset getuserid [character name]
- ChangeCharName : Change the name of a character
  - Syntax: $iset changecharname [old character name] [new character name]
- GiveAP : Give the specified player a specific amount of AP. Stacks on top of the AP they already have. Can be negative to remove AP
  - Syntax: $iset giveap [character name] [amount]
- SetLevel : Set the level of the specified character
  - Syntax: $iset setlevel [character name] [level]
- SetTrans : Give the specified player transformation (paladin or dark knight) if they don't already have it
  - Syntax: $iset settrans [character name] [paladin|knight]
- GiveTransSkills : Give the specified player max skills based on their given transformation in the database
  - Syntax: $iset givetransskills [character name]
- Scroll : find information about a specific scroll
  - Syntax: $iset scroll [scroll name]
- Infusions : List available infusions
  - Syntax: $iset infusions
- BanList : List all banned players
  - Syntax: $iset banlist
- Reset2ndary : Reset the secondary password for a specific player
  - Syntax: $iset reset2ndary [character name]


### Conclusion

Keep an eye to this if you're interested in using Iset for your private server, I plan to keep adding features as often as I can, with the hope of having a stable release within the quarter!