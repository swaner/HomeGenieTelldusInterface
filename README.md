<style>
.code {
    font: italic bold 12px Georgia, serif;
    display: inline;
}
    .code:before {
         content:"\A"; white-space:pre;
    }
</style>
# MIG-TelldusTellstick interface
HomeGenie / MIG interface driver for TellStick. Tested on TellStick Duo.

<div id="installation">
<h3>Installation</h3>
<h4>Install telldus-core</h4>
This interface requires telldus-core libraries. To install these in Windows, Mac or Linux follow instructions below.
<h5>Windows</h5>
<p>
Download telldus center from http://www.telldus.se/products/nativesoftware.
</p>
<h5>Raspberry pi</h5>
SSH into the pi and run the following commands:
    <p>
Update apt-get:
<span class="code">sudo nano /etc/apt/sources.list.d/telldus.list</span>
<span class="code">deb-src http://download.telldus.com/debian/ stable main</span>
<p/>
        <p>
            
Download the key:<span class="code">wget http://download.telldus.se/debian/telldus-public.key 
            </span>
        </p>
        <p>
            Add the key: <span class="code">sudo apt-key add telldus-public.key </span>
        </p>
        <p>
            Run a update in order to add the telldus sources: <span class="code">sudo apt-get update </span>
        </p>
        <p>
            This should already be installed: <span class="code">sudo apt-get install build-essential </span>
        </p>
        <p>
            Install dependencies: <span class="code">sudo apt-get build-dep telldus-core </code>
        </p>
        <p>
            Even more dependencies: <span class="code">sudo apt-get install cmake libconfuse-dev libftdi-dev help2man </span>
        </p>
        <p>
            Make a temporary directory to compile in: <span class="code">mkdir -p ~/telldus-temp </span>
            <span class="code">cd ~/telldus-temp</span>
        </p>
        <p>
            Download the source: <span class="code">sudo apt-get –-compile source telldus-core </span>
        </p>
        <p>
            Install: <span class="code">sudo dpkg -–install *.deb </span>
        </p>
        <p>
            Done, if you have any sensors to test you can now do so by typing: <span class="code">tdtool -l </span>
            </p>
  <p>          
The above steps are fetched from the swedish blogpost: https://blogg.itslav.nu/?p=875. <br /> Here is another more official tutorial I haven't tried myself: http://elinux.org/R-Pi_Tellstick_core.
        </p>
</div>

<h4>Install interface to homegenie</h4>
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
