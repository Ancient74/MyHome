Prerequisites:

MyHomeWebApi:

Project must have server.pfx file to enable https in a root folder of the project

It must have an environment variables:

MyHomePath - to where .exe is

MyHomeCertPassword - password to a server.pfx certificate

There is a list of allowed mac addresses which should be filled up in order to allow the connections

IotHome:

There should be server certificates in IotHome/data: *.crt and  *.key files in order to enable https on WiFi module

Main page:

![photo_3_2023-08-16_11-54-45](https://github.com/Ancient74/MyHome/assets/44616207/0ea8f77f-4d51-4559-baf4-229ab934b297)

Monitor config:

It allows changing monitor configuration as well as openning steam big picture on change

![photo_4_2023-08-16_11-54-45](https://github.com/Ancient74/MyHome/assets/44616207/ec4549ee-78a1-4981-a030-949c76120b34)

Audio config page:

It is possible to change the volume and mute the input or output audio device

![photo_2_2023-08-16_11-54-45](https://github.com/Ancient74/MyHome/assets/44616207/1675fd9a-2605-4155-9632-38b680e08c53)

It is also possible to change input or output audio device

![photo_5_2023-08-16_11-54-45](https://github.com/Ancient74/MyHome/assets/44616207/bb31d262-bc7d-44cb-bc80-e124750cf87c)

Shutdown menu:

It is possible to restart/shutdown a PC using force or soft mechanizm 

![photo_8_2023-08-16_11-54-45](https://github.com/Ancient74/MyHome/assets/44616207/2089ff4e-7a81-4b0f-b50b-b233c9b78342)

Also it is possible to turn on a PC using "Wake on Lan" feature

![photo_2023-08-16_11-57-15](https://github.com/Ancient74/MyHome/assets/44616207/724d8c8d-2fcc-47d6-bca0-0fd3f6100737)

Here we configure ip of a pc and it's mac address

![photo_7_2023-08-16_11-54-45](https://github.com/Ancient74/MyHome/assets/44616207/ef4ac2bc-e709-4401-8188-72ff20614416)

IoT:

There is a possibility to control IoT devices that follow pattern described in IoTHomeApi.cs (and related classes) in MyHomeApp project

![photo_6_2023-08-16_11-54-45](https://github.com/Ancient74/MyHome/assets/44616207/b4f1c885-85c2-4860-bd63-1be1dff18c59)

Sharing:

It is possible to share a link with MyHome android app and it will open it on a connected PC

![photo_1_2023-08-16_11-54-45](https://github.com/Ancient74/MyHome/assets/44616207/eae2eb80-f0a0-4f08-9fa9-1c531e4aa053)
