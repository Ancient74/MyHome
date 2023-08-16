Prerequisites:

MyHomeWebApi:
Project must have server.pfx file to enable https in a root folder of the project

It must have an environment variables:
MyHomePath - to where .exe is
MyHomeCertPassword - password to a server.pfx certificate

There is a list of allowed mac addresses which should be filled up in order to allow the connections

IotHome:
There should be server certificates in IotHome/data: *.crt and  *.key files in order to enable https on WiFi module
