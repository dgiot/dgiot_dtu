`# dgiot_dtu

 [dgiot_dtu](http://dgiot-1253666439.cos.ap-shanghai-fsi.myqcloud.com/dgiot4.0/dgiot_dtu.zip) 是dgiot工业物联网整体解决方案中边缘侧的桌面实用工具
 + 可以部署在企业内网,通过多个dgiot_dtu的mqtt/tcp/udp等级联的方式,打通企业内网与云端服务器的通讯通道
 + 将企业内网中的串口/PLC/OPC/BACNet/UI/Access/SqlServer等老的工业系统中的数据汇聚到云端dgiot服务中
 + 对OPC/OPC/BACNet/UI/Access/SqlServer等实现自动扫描工具，实现远程施工与运维服务
 + 与云端dgiot服务器中的通道配合实现数据自动采集、协议解析，数据存储和展示功能

# 交互流程

![dgiot_dtu_bus.png](http://dgiot-1253666439.cos.ap-shanghai-fsi.myqcloud.com/dgiot4.0/dgiot_dtu.png)

# 界面预览

![dgiot_dtu_demo.png](http://dgiot-1253666439.cos.ap-shanghai-fsi.myqcloud.com/dgiot4.0/dgiot_dtu_demo.png)

# 运行环境
dgiot_dtu 依赖[.net4.5](https://dgiot-dev-1306147891.cos.ap-nanjing.myqcloud.com/dgiot_dtu/dotNetFx45.rar)运行环境，
在window7及以下环境下需要安装.net4.5

# 编译环境
dgiot_dtu 用[vc2019](https://dgiot-dev-1306147891.cos.ap-nanjing.myqcloud.com/dgiot_dtu/visualstudio2019.zip)编译调试

# 数据映射

|  TreeNode | TAG     | Text   | Name | Level   | Index  | FullPath  | Action   |
| --------  | ------  | ----   |----- |  -----  | -----  | --------  | -------- |
| OPCDA     | Proctol | ItemId | Name |  Type   | {Id}    | Path     |  API    |


## 设备树Level映射到设备Type
| TreeNode | Level0  | Level1  | Level2  | Level3  |  Level4 | Level5 |  Level6 |
| -------- | ------- | ------- | ------- | ------- | ------- | ------ | ------- |
| OPCDA    | OPCDA   | HOST    | Service | Device  | Group   |Item    | Property|


# 测试环境

## OPC模拟测试
+ [opcserver](https://dgiot-dev-1306147891.cos.ap-nanjing.myqcloud.com/dgiot_dtu/MatrikonOPCSimulation.zip)

+ [opcclient](https://dgiot-dev-1306147891.cos.ap-nanjing.myqcloud.com/dgiot_dtu/MatrikonOPCSimulationV_1.5.zip)

## 串口模拟测试

+ [虚拟电表](http://tech.iotn2n.com/w/docs/details?id=9)
+ [虚拟modbus](http://tech.iotn2n.com/w/docs/details?id=9)

## Bacnet模拟测试

## PLC模拟测试

## Control模拟测试

## Access模拟测试

## SqlSever模拟测试

## mqtt桥接测试

## tcp桥接测试

## udp桥接测试

