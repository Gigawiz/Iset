# Iset - A Discord Administration tool for Vindictus Servers

Iset was created while I was running a "public" private vindictus server. There was a need for the ability for my staff members to run commands without having direct access to the hardware or RDP. For example, Banning players, Restoring broken items from enhancment/enchantment, etc. Originally, these actions would have been done by logging in to RDP and either editing the databases directly, or using HeroesOPTool to run commands. Unfortunately, I learned the hard way that more people having access to the live files of your server is not a good thing. After Delteros and Schitelle had a falling out with me on discord, Delteros attempted to wipe all my data assuming it would ruin the server. Fortunately, I had a two day old backup, and not much came of that. However it pointed out the need to make some changes to how I ran the server. It was at this point that Iset became a staple tool for running the server. At the time it had a permissions system that allowed my staff members to spawn items, ban/unban players, upload changes to the heroesContents file (it even made a backup that it could restore if something got screwed!), and eventually even had player runnable commands.

  After the server closed down, I let the project die out. Recently, one of my loyal players asked if I could revive the project (iset) for another private server that he plays on. Thus was the rebirth of Iset.... Iset 2.0. She's a work in progress. With all of the changes to .Net between then and now, I was basically starting from scratch, so I figured I'd start it in GoLang. This was mainly done so that I could integrate NMServer into the source code, with the intent to allow user management from not only discord, but web access too. In any case, that was a rant and you're here for.... well I have no idea but you've read this far, so why not read some features?

  ## Features

Currently, there's not a ton of features. This is because I've been working on this for a collective total of.... 1 week.... But I'm making progress daily, so keep checking back!
- Integrated NMServer source
- Added enchants and scroll commands that have external translations
- Bot now loads all settings from ini file
- Allows disabling login server
- Can get details about a scroll using $iset scroll <scroll name>. If scroll does not exist, returns error instead of crashing like it used to
- Added variable to INI to change bot prefix (instead of calling commands using $vindictus, you can now use your own like !whydidichangethis)
- Added unflip command
- Added infusions command with hard-coded list of infusions
- Ban List! - Actually gets the list of bans from the mssql database and Formats it in an embed

### Conclusion

Keep an eye to this if you're interested in using Iset for your private server, I plan to keep adding features as often as I can, with the hope of having a stable release within the quarter!