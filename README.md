# MIG-TelldusTellstick interface
HomeGenie / MIG interface driver for TellStick. Tested on TellStick Duo.
This requires telldus-core libraries. To install these in windows download telldus center from http://www.telldus.se/products/nativesoftware.
In linux follow step 4 - 7 in this tutorial (please note last comment): https://blogg.itslav.nu/?p=875.

When the telldus-core libraries are installed you can install the interface in homegenie: <br />
1. Download the zip file located in the root directory <a href="https://github.com/swaner/HomeGenieTelldusInterface/blob/master/Tellstick_0_9.zip">here</a>.<br />
2. Open HomeGenie and goto Configure->Interfaces.<br />
3. Press Import Interface Driver and locate the file downloaded in step 1.<br />

If someone manages to install this by just copying the telldus-core binaries let me know!

Currently the interface supports the following:
* Turning lights on/off
* Dimming lights
* Reading temperature values
* Simulated two way communication (remote triggers homegenie status)
