/*
 * Created by SharpDevelop.
 * User: rawr
 * Date: 2019/12/25
 * Time: 上午 1:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Management;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.IO.Ports;
using System.Diagnostics;

namespace RelayControl
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
			ManagementClass processClass = new ManagementClass("Win32_PnPEntity");


            ManagementObjectCollection Ports = processClass.GetInstances();
            string device = "No recognized";
            foreach (ManagementObject property in Ports)
            {
                if (property.GetPropertyValue("Name") != null)
                    if (property.GetPropertyValue("Name").ToString().Contains("USB") &&
                        property.GetPropertyValue("Name").ToString().Contains("COM"))
                    {
                        Trace.WriteLine(property.GetPropertyValue("Name").ToString());
                        device = property.GetPropertyValue("Name").ToString();
                    }

                    }
			
		}
	}
}