using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TelldusLib;
using MIG.Config;

namespace MIG.Interfaces.HomeAutomation
{
    public class Tellstick : MigInterface
    {
        public static string Event_Node_Description = "Tellstick Node";
        public static string Event_Sensor_Description = "Tellstick Sensor";

        TellstickController controller;
        List<InterfaceModule> interfaceModules = new List<InterfaceModule>();

        public Tellstick()
        {
            controller = new TellstickController();
        }

        #region MIG Interface members

        public event InterfaceModulesChangedEventHandler InterfaceModulesChanged;
        public event InterfacePropertyChangedEventHandler InterfacePropertyChanged;

        public string Domain
        {
            get
            {
                string domain = this.GetType().Namespace.ToString();
                domain = domain.Substring(domain.LastIndexOf(".") + 1) + "." + this.GetType().Name.ToString();
                return domain;
            }
        }

        public List<InterfaceModule> GetModules()
        {
            return interfaceModules;
        }

        public List<Option> Options { get; set; }

        public void OnSetOption(Option option)
        {
            if (IsEnabled)
                Connect();
        }

        public bool IsConnected
        {
            get { return controller.IsConnected; }
        }

        public bool Connect()
        {
            controller.Init();
            var n = controller.GetNumberOfDevices();
            controller.SetConnected(n >= 0);

            for (var i = 0; i < n; i++)
            {
                var id = controller.GetDeviceId(i);
                interfaceModules.Add(new InterfaceModule
                    {
                        Domain = Domain,
                        Address = id.ToString(),
                        Description = controller.GetName(id),
                        ModuleType = GetDeviceType(controller.GetProtocol(id))
                    });
                var lastCommand = controller.LastSentCommand(id, (int)(Command.DIM | Command.TURNON | Command.TURNOFF));
                if (lastCommand != (int)Command.TURNOFF)
                {
                    OnInterfacePropertyChanged(this.GetDomain(), id.ToString(), Event_Node_Description, ModuleEvents.Status_Level, "1");
                }
            }

            controller.RegisterDeviceEvent(OnDeviceUpdated, null);
            controller.RegisterSensorEvent(SensorUpdated, null);

            return true;
        }

        public void Disconnect()
        {
            controller.Close();
        }

        public bool IsDevicePresent()
        {
            return true;
        }

        public bool IsEnabled { get; set; }

        public object InterfaceControl(MigInterfaceCommand command)
        {
            string returnValue = "";
            bool raisePropertyChanged = false;
            string parameterPath = "Status.Level";
            string raiseParameter = "";
            switch (command.Command)
            {
                case "Control.On":
                    controller.TurnOn(int.Parse(command.Address));
                    raisePropertyChanged = true;
                    raiseParameter = "1";
                    break;
                case "Control.Off":
                    raisePropertyChanged = true;
                    controller.TurnOff(int.Parse(command.Address));
                    raiseParameter = "0";
                    break;
                case "Control.Level":
                    var dimValue = double.Parse(command.GetOption(0));
                    if(dimValue == 0)
                    { 
                        controller.TurnOff(int.Parse(command.Address));
                        raisePropertyChanged = true;
                        raiseParameter = "0";
                    }
                    else
                    { 
                        controller.Dim(int.Parse(command.Address), (int)Math.Round(dimValue));
                        raisePropertyChanged = true;
                        raiseParameter = (dimValue / 100.0).ToString(CultureInfo.InvariantCulture);
                    }
                    break;
                case "Control.Toggle":
                    int status = controller.LastSentCommand(int.Parse(command.Address), (int)(Command.DIM | Command.TURNON | Command.TURNOFF));

                    if (status == (int)Command.TURNOFF)
                    {
                        controller.TurnOn(int.Parse(command.Address));
                        raiseParameter = "1";
                    }
                    else
                    {
                        controller.TurnOff(int.Parse(command.Address));
                        raiseParameter = "0";
                    }
                    raisePropertyChanged = true;
                    break;
                default:
                    Console.WriteLine("TS:" + command.Command + " | " + command.Address);
                    break;
            }

            if (raisePropertyChanged)
            {
                try
                {
                    OnInterfacePropertyChanged(this.GetDomain(), command.Address, Event_Node_Description, parameterPath, raiseParameter);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception catched on OnInterfacePropertyChanged. Message: {0}", ex.Message);
                }
            }
            return returnValue;
        }

        #endregion

        #region Events

        protected virtual void OnInterfaceModulesChanged(string domain)
        {
            if (InterfaceModulesChanged == null) return;

            var args = new InterfaceModulesChangedEventArgs(domain);
            InterfaceModulesChanged(this, args);
        }

        protected virtual void OnInterfacePropertyChanged(string domain, string source, string description, string propertyPath, object propertyValue)
        {
            if (InterfacePropertyChanged == null) return;

            var args = new InterfacePropertyChangedEventArgs(domain, source, description, propertyPath, propertyValue);
            InterfacePropertyChanged(this, args);
        }

        #endregion

        private ModuleTypes GetDeviceType(string protocol)
        {
            if (protocol.IndexOf("dimmer", StringComparison.Ordinal) > -1)
                return ModuleTypes.Dimmer;
            if (protocol.IndexOf("switch", StringComparison.Ordinal) > -1)
                return ModuleTypes.Switch;
            return ModuleTypes.Generic;
        }

        private int OnDeviceUpdated(int deviceId, int method, string data, int callbackId, object obj, UnmanagedException ex)
        {
            var path = ModuleEvents.Status_Level;
            int? value = null;
            if (method == (int)TelldusLib.Command.TURNON)
            {
                path = ModuleEvents.Status_Level;
                value = 1;
            }
            else if (method == (int)TelldusLib.Command.TURNOFF)
            {
                path = ModuleEvents.Status_Level;
                value = 0;
            }

            if (value.HasValue)
            { 
                var module = interfaceModules.FirstOrDefault(i => i.Address == deviceId.ToString());
                OnInterfacePropertyChanged(this.GetDomain(), module.Address, Event_Sensor_Description, path, value);
            }

            return 1;
        }

        private int SensorUpdated(
            string protocol, string model, int id, int dataType, string val, int timestamp, int callbackId, object obj,
            UnmanagedException ex)
        {
            Console.WriteLine("TS: " + protocol + ", " + model + ", " + id + ", " + dataType + ", " + val + ", " + timestamp + ", " + callbackId);
            var module = interfaceModules.FirstOrDefault(i => i.Address == id.ToString());
            if (module == null)
            {

                module = new InterfaceModule
                {
                    Domain = Domain,
                    Address = id.ToString(),
                    Description = model + " - " + protocol,
                    ModuleType = ModuleTypes.Sensor
                };
                interfaceModules.Add(module);

                OnInterfaceModulesChanged(this.GetDomain());
            }

            var path = ModuleEvents.Status_Level;
            if (dataType == (int)TelldusLib.DataType.TEMPERATURE)
                path = ModuleEvents.Sensor_Temperature;
            else if (dataType == (int)TelldusLib.DataType.HUMIDITY)
                path = ModuleEvents.Sensor_Humidity;

            OnInterfacePropertyChanged(this.GetDomain(), module.Address, Event_Sensor_Description, path, val);

            return 1;
        }

        public static class ModuleEvents
        {
            public static string Status_Level =
                "Status.Level";
            public static string Sensor_Temperature =
                "Sensor.Temperature";
            public static string Sensor_Luminance =
                "Sensor.Luminance";
            public static string Sensor_Humidity =
                "Sensor.Humidity";
            public static string Sensor_DoorWindow =
                "Sensor.DoorWindow";
            public static string Sensor_Tamper =
                "Sensor.Tamper";
        }
    }
}
