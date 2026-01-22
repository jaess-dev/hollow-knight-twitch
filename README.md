# Modding HK

[getting started](https://prashantmohta.github.io/ModdingDocs/getting-started.html)

[first mod](https://prashantmohta.github.io/ModdingDocs/your-first-mod.html)

Local game files: `%USERPROFILE%\AppData\LocalLow\LocalLow\Team Cherry\Hollow Knight`

## Architecture

This project uses a hollow knight mod to send web requests whenever a certain event happens.
These requests can be further extended upon. 
Currently, 
I'm only sending certain events that I wanted to implement.

While the initial implementation uses a Streamer.bot instance and sends the events to it,
this is not the end goal here.
The events should be send to a different .NET server and then be distributed by that server.
This architecture is in my eyes necessary to enable different distribution methods without
relying on the mod and thus the game doing the heavy lifting.
The game should only do its normal processing of an event and then send information about the event
to the server which can then do all necessary processing.

As I develop this mod to be used in OBS, 
I will also add a React frontend which can be used to display certain things as an overlay
in the stream or better said recording.

### The Mod

The mod is based on the [getting started] guid and the [first mod] guide.
It uses the in these guides described template for mod creation.
Additionally the modding framework must be installed for this mod to work.
Otherwise, you can just place it in your games mod folder. 
(When the mods `.csproj` file is correctly configured, the project will be directly build into that folder.)

The mod uses the modding framework hooks to be notified when certain events happen and then sends a web request
to a provided server. 
Currently, that server is hard coded but I'm planning on adding a mod menu for easier changes.

### The server

.NET server (TODO)

### The frontend

React frontend (TODO)

## Future work

* A mod menu addition to the mod to make things configurable. Mainly:
  * What events are subscribed
  * To what endpoint should they be send


[getting started]: https://prashantmohta.github.io/ModdingDocs/getting-started.html
[first mod]: https://prashantmohta.github.io/ModdingDocs/your-first-mod.html