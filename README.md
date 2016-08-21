# [[ Grievous Bodily Harm ]]

Build artifacts available [here](https://ci.appveyor.com/project/avail/gbhm/build/artifacts)

## How to play

### Hosting a lobby

When the game opens, press F11 to open the console and write 'a'. This will put 
you in a local session people can join. (The 'a' command is an alias to 'map mp1-comp')


### Joining a lobby

When the game opens, press F11 to open the console and write 'connect [ip/hostname]',
your buddy will have to tell you the address.

## Variables

The game console isn't only used for hosting a lobby and joining it, there's 
various variables that can be used. The variables are pretty straight-forward:

- cheats - enables cheat-protected commands [host only]
- net_forceLatency - force network packets to be sent/arrive with a delay
- com_maxfps - maximum allowed fps
- timescale - scale time by an amount [cheat protected]
- sv_running - is the server running? [read-only]
- cl_running - is the client running? [read-only]
- sv_paused - is the server paused? [read-only]
- cl_paused - is the client paused? [read-only]
- net_showpackets - show network packets
- mapname - current map name [read-only]
- nickname - your nickname
- fov - field of view in degrees [cheat protected]
- snd_volume - game audio volume
- cl_maxpackets - max packets sent per frame
- cl_snapDelay - amount of time to delay the rendering by
- sv_gravity - gravity value
- vid_fullscreen - switch to borderless fullscreen mode
- vid_xpos - horizontal position on screen
- vid_ypos - vertical position on screen
- vid_width - game window width
- vid_height - game window height
- r_sunIntensity - sun intensity
- r_hdr_enable - enable high dynamic range rendering
- sm_enable - enable shadow mapping
- sm_filterType - define filtering algorythm to use for shadow mapping

# Commands

- connect - connect to specified ip/hostname
- map - load a map (see map names in Data/Maps/ directory
- say - send a chat message (only displayed in console currently)
- quit - exit the game
- seta - set a config variable. (example: seta "username" "avail")
- exec - execute a configuration file (example: exec config.cfg)
- status - show a list of players connected [host only]
- kick - kick a player by name [host only]
- clear - clear the console output
- kill - kill a user [host only]

more to come...

# TODO

- [ ] libcef UI framework
- [ ] server-side resources
- [ ] python scripting framework
- [ ] ai implementation
- [ ] proper linux support

