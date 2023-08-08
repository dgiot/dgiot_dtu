# dgiot_dtu

 [dgiot_dtu](http://dgiot-1253666439.cos.ap-shanghai-fsi.myqcloud.com/dgiot4.0/dgiot_dtu.zip) は、dgiot産業用IoT全体ソリューションのエッジ側にあるデスクトップユーティリティです。
 +複数のdgiot_dtumqtt / tcp / udp階層メソッドを介して企業イントラネットにデプロイでき、企業イントラネットとクラウドサーバー間の通信チャネルを開きます
 +企業イントラネットのシリアルポート/ PLC / OPC / BACNet / UI / Access / SqlServerなどの古い産業用システムからのデータをクラウドdgiotサービスに統合します
 + OPC / OPC / BACNet / UI / Access / SqlServerなどの自動スキャンツールを実現し、リモートの構築および運用と保守サービスを実現します
 +クラウドdgiotサーバーのチャネルと協力して、自動データ収集、プロトコル分析、データストレージ、および表示機能を実現します

# インタラクティブプロセス

![dgiot_dtu_bus.png](http://dgiot-1253666439.cos.ap-shanghai-fsi.myqcloud.com/dgiot4.0/dgiot_dtu.png)

# インターフェースプレビュー

![dgiot_dtu_demo.png](http://dgiot-1253666439.cos.ap-shanghai-fsi.myqcloud.com/dgiot4.0/dgiot_dtu_demo.png)

# 動作環境
dgiot_dtuは、[.net4.5](https://dgiot-dev-1306147891.cos.ap-nanjing.myqcloud.com/dgiot_dtu/dotNetFx45.rar)の動作環境に依存します。
window7以下の環境に.net4.5をインストールする必要があります

# コンパイラ環境
dgiot_dtuは、[vc2019](https://dgiot-dev-1306147891.cos.ap-nanjing.myqcloud.com/dgiot_dtu/visualstudio2019.zip)を使用してコンパイルおよびデバッグします。


# テスト環境

## OPCシミュレーションテスト
+ [opcserver](https://dgiot-dev-1306147891.cos.ap-nanjing.myqcloud.com/dgiot_dtu/MatrikonOPCSimulation.zip)

+ [opcclient](https://dgiot-dev-1306147891.cos.ap-nanjing.myqcloud.com/dgiot_dtu/MatrikonOPCSimulationV_1.5.zip)

##シリアルポートシミュレーションテスト

+ [仮想メーター]（https://gitee.com/dgiiot/dgiot/wikis/%E5%BF%AB%E9%80%9F%E6%8E%A5%E5%85%A5/%E8%99%9A%E6%8B%9F%E7%94%B5%E8%A1%A8%E6%8E%A5%E5%85%A5/%E6%A6%82%E8%BF%B0）
+ [仮想modbus]（https://gitee.com/dgiiot/dgiot/wikis/%E5%BF%AB%E9%80%9F%E6%8E%A5%E5%85%A5/Modbus%E8%AE%BE%E5%A4%87%E6%8E%A5%E5%85%A5/modbus%20slave%E9%85%8D%E7%BD%AE）

## Bacnetシミュレーションテスト

## PLCシミュレーションテスト

##制御シミュレーションテスト

##模擬テストにアクセスする

## SqlSeverシミュレーションテスト

## mqttブリッジテスト

## tcpブリッジテスト

## udpブリッジテスト
