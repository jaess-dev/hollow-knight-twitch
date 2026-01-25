#

## Streamer.bot integration

Making streamer.bot accessible in the local network.

Find your local ip with:
```
192.168.0.126
```

```
netsh http add urlacl url=http://192.168.0.126:7474/ user=YOURDOMAIN\YourUsername

netsh advfirewall firewall add rule name="Streamer.bot TCP 7474" ^
 dir=in action=allow protocol=TCP localport=7474 ^
 program="C:\Path\To\Streamer.bot.exe"
```

