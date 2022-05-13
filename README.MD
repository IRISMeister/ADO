# ADO

## 実行方法

ADO.exe SleepTime(ms) host port

```
> ADO.exe 100 192.168.11.2 1972
```

## 結果
Windows10 Client PCから遠隔のWindows10ホスト上のIRISサーバに対して実行。
> 使用環境は計測に「理想」とは言えない環境。通信は無線LAN経由。クライアント、サーバ共にセキュリティ対策ソフトあり。

|IRIS環境|計測値|備考|
|:---|:---|:---|
|Windows版IRIS|[実行結果](results/sleep100-win.txt)||
|コンテナ版(同Windows上のWSL2)|[実行結果](results/sleep100-docker.txt)||

Windows版散布図

![Windows版散布図](results/sleep100-docker.png)|

コンテナ版散布図

![コンテナ版散布図](results/sleep100-docker.png)|
## how to forward wsl2 port to an external host.

```
netsh.exe interface portproxy add v4tov4 listenaddress=192.168.11.2 listenport=1972 connectaddress=172.25.29.22 connectport=1972
```
