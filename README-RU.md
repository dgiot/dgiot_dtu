# dgiot_dtu

 [dgiot_dtu](http://dgiot-1253666439.cos.ap-shanghai-fsi.myqcloud.com/dgiot4.0/dgiot_dtu.zip)  - это настольная утилита на периферии общего решения dgiot Industrial IoT.
 + Может быть развернут в корпоративной интрасети с помощью нескольких иерархических методов dgiot_dtu mqtt / tcp / udp, открывая канал связи между корпоративной интрасетью и облачным сервером
 + Преобразование данных из старых промышленных систем, таких как последовательный порт / PLC / OPC / BACNet / UI / Access / SqlServer в корпоративной интрасети, в облачную службу dgiot
 + Реализовать инструменты автоматического сканирования для OPC / OPC / BACNet / UI / Access / SqlServer и т. Д. Для реализации удаленного строительства, эксплуатации и обслуживания
 + Сотрудничать с каналом в облачном сервере dgiot для реализации автоматического сбора данных, анализа протокола, хранения данных и функций отображения

# Интерактивный процесс

![dgiot_dtu_bus.png](http://dgiot-1253666439.cos.ap-shanghai-fsi.myqcloud.com/dgiot4.0/dgiot_dtu.png)

# Предварительный просмотр интерфейса

![dgiot_dtu_demo.png](http://dgiot-1253666439.cos.ap-shanghai-fsi.myqcloud.com/dgiot4.0/dgiot_dtu_demo.png)

# Рабочая среда
dgiot_dtu зависит от операционной среды [.net4.5](https://dgiot-dev-1306147891.cos.ap-nanjing.myqcloud.com/dgiot_dtu/dotNetFx45.rar),
Необходимо установить .net4.5 в среде window7 и ниже

# Среда компилятора
dgiot_dtu компилируется и отлаживается с помощью [vc2019] (https://dgiot-dev-1306147891.cos.ap-nanjing.myqcloud.com/dgiot_dtu/visualstudio2019.zip)


# тестовая среда

## Тест моделирования OPC
+ [opcserver](https://dgiot-dev-1306147891.cos.ap-nanjing.myqcloud.com/dgiot_dtu/MatrikonOPCSimulation.zip)

+ [opcclient](https://dgiot-dev-1306147891.cos.ap-nanjing.myqcloud.com/dgiot_dtu/MatrikonOPCSimulationV_1.5.zip)

## Тест имитации последовательного порта

+ [Виртуальный счетчик](https://gitee.com/dgiiot/dgiot/wikis/%E5%BF%AB%E9%80%9F%E6%8E%A5%E5%85%A5/%E8%99%9A%E6%8B%9F%E7%94%B5%E8%A1%A8%E6%8E%A5%E5%85%A5/%E6%A6%82%E8%BF%B0)
+ [Виртуальный Modbus](https://gitee.com/dgiiot/dgiot/wikis/%E5%BF%AB%E9%80%9F%E6%8E%A5%E5%85%A5/Modbus%E8%AE%BE%E5%A4%87%E6%8E%A5%E5%85%A5/modbus%20slave%E9%85%8D%E7%BD%AE)

## Тест имитации бакнета

## Тест моделирования ПЛК

## Тест имитации управления

## Пробный тест доступа

## Тест моделирования SqlSever

## Тест моста mqtt

## тест моста tcp

## тест моста udp
