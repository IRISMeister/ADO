# ADO

## how to run

ADO.exe SleepTime(ms) host port

```
> ADO.exe 100 192.168.11.2 51774
```

## how to forward wsl2 port to an external host.

```
netsh.exe interface portproxy add v4tov4 listenaddress=192.168.11.2 listenport=1972 connectaddress=172.25.29.22 connectport=1972
```
