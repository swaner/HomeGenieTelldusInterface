# MIG-TelldusTellstick interface
HomeGenie / MIG interface driver for TellStick. Tested on TellStick Duo.

<h3>Installation</h3>
<h4>Install telldus-core</h4><br />
This interface requires telldus-core libraries. To install these in Windows, Mac or Linux follow instructions below.
<b>Windows</b>
<p>
Download telldus center from http://www.telldus.se/products/nativesoftware. <br />
<b>Raspberry pi</b><br />
</p>
SSH into the pi and run the following commands: <br />
Update apt-get: <br />
<i>sudo nano /etc/apt/sources.list.d/telldus.list<i /> <br />
deb-src http://download.telldus.com/debian/ stable main <br />
Download the key: wget http://download.telldus.se/debian/telldus-public.key <br />
Add the key: sudo apt-key add telldus-public.key <br />
Run a update in order to add the telldus sources: sudo apt-get update <br />
This should already be installed: sudo apt-get install build-essential <br />
Install dependencies: sudo apt-get build-dep telldus-core <br />
Even more dependencies: sudo apt-get install cmake libconfuse-dev libftdi-dev help2man <br />
Make a temporary directory to compile in: mkdir -p ~/telldus-temp <br />
cd ~/telldus-temp<br />
Download the source: sudo apt-get –-compile source telldus-core <br />
Install: sudo dpkg -–install *.deb <br />
Done, if you have any sensors to test you can do so by typing: tdtool -l <br />
The above steps are fetched from the swedish blogpost: https://blogg.itslav.nu/?p=875. <br /> Here is another more official tutorial I haven't tried myself: http://elinux.org/R-Pi_Tellstick_core.

<b>Install interface to homegenie</b><br />
When the telldus-core libraries are installed you can install the interface in homegenie: <br />
<ol>
<li>Download the zip file located in the root directory <a href="https://github.com/swaner/HomeGenieTelldusInterface/raw/master/Tellstick_0_9.zip">here</a>.</li>
<li>Open HomeGenie and goto Configure->Interfaces.</li>
<li>Press Import Interface Driver and locate the file downloaded in step 1.</li>
</ol>

<h3>Features</h3>
Currently the interface supports the following:
* Turning lights on/off
* Dimming lights
* Reading temperature values
* Simulated two way communication (remote triggers homegenie status)
