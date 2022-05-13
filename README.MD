# ADO

## 実行方法

ADO.exe SleepTime(ms) host port

```
> ADO.exe 100 192.168.11.2 1972
```

## 結果
Windows10 Client PCから遠隔のIRISサーバに対して実行。

Windows版IRISの場合の[実行結果](results/sleep100-win.txt)
コンテナ版(同Windows上のWSL2)の場合の[実行結果](results/sleep100-docker.txt)

## how to forward wsl2 port to an external host.

```
netsh.exe interface portproxy add v4tov4 listenaddress=192.168.11.2 listenport=1972 connectaddress=172.25.29.22 connectport=1972
```
