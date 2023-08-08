# dgiot_dtu

 [dgiot_dtu](http://dgiot-1253666439.cos.ap-shanghai-fsi.myqcloud.com/dgiot4.0/dgiot_dtu.zip) is a desktop utility on the edge side of the dgiot industrial IoT overall solution
 + Can be deployed on the corporate intranet, through multiple dgiot_dtu mqtt/tcp/udp hierarchical methods, open up the communication channel between the corporate intranet and the cloud server
 + Converge data from old industrial systems such as serial port/PLC/OPC/BACNet/UI/Access/SqlServer in the corporate intranet into the cloud dgiot service
 + Realize automatic scanning tools for OPC/OPC/BACNet/UI/Access/SqlServer, etc. to realize remote construction and operation and maintenance services
 + Cooperate with the channel in the cloud dgiot server to realize automatic data collection, protocol analysis, data storage and display functions

# Interactive process

![dgiot_dtu_bus.png](http://dgiot-1253666439.cos.ap-shanghai-fsi.myqcloud.com/dgiot4.0/dgiot_dtu.png)

# Interface preview

![dgiot_dtu_demo.png](http://dgiot-1253666439.cos.ap-shanghai-fsi.myqcloud.com/dgiot4.0/dgiot_dtu_demo.png)

# Operating environment
dgiot_dtu depends on [.net4.5](https://dgiot-dev-1306147891.cos.ap-nanjing.myqcloud.com/dgiot_dtu/dotnetfx45.zip) operating environment,
Need to install .net4.5 in the environment of window7 and below

# Compiler Environment
dgiot_dtu compile and debug with [vc2019](https://dgiot-dev-1306147891.cos.ap-nanjing.myqcloud.com/dgiot_dtu/visualstudio2019.zip)


# test environment

## OPC simulation test
+ [opcserver](https://dgiot-dev-1306147891.cos.ap-nanjing.myqcloud.com/dgiot_dtu/MatrikonOPCSimulation.zip)

+ [opcclient](https://dgiot-dev-1306147891.cos.ap-nanjing.myqcloud.com/dgiot_dtu/MatrikonOPCSimulationV_1.5.zip)

## Serial port simulation test

+ [Virtual Meter](https://gitee.com/dgiiot/dgiot/wikis/%E5%BF%AB%E9%80%9F%E6%8E%A5%E5%85%A5/%E8%99%9A%E6%8B%9F%E7%94%B5%E8%A1%A8%E6%8E%A5%E5%85%A5/%E6%A6%82%E8%BF%B0)
+ [Virtual modbus](https://gitee.com/dgiiot/dgiot/wikis/%E5%BF%AB%E9%80%9F%E6%8E%A5%E5%85%A5/Modbus%E8%AE%BE%E5%A4%87%E6%8E%A5%E5%85%A5/modbus%20slave%E9%85%8D%E7%BD%AE)

## Bacnet simulation test

## PLC simulation test

## Control simulation test

## Access mock test

## SqlSever simulation test

## mqtt bridge test

## tcp bridge test

## udp bridge test
