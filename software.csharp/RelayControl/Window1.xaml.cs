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
			
			InitializeUserInterface();

			
		}
		
	
		public void InitializeUserInterface(){
			ComboBox serial_select = new ComboBox();
			Grid     main_grid     = new Grid();
			main_grid.ColumnDefinitions.Add(new ColumnDefinition());
			main_grid.ColumnDefinitions.Add(new ColumnDefinition());
			main_grid.RowDefinitions.Add(new RowDefinition());
			main_grid.RowDefinitions.Add(new RowDefinition());
			
			Grid.SetRow(serial_select, 0);
			Grid.SetColumn(serial_select, 0);
			main_grid.Children.Add(serial_select);
			List<ComboData> devices = GetSerialDevices();
			serial_select.ItemsSource = devices;
			serial_select.DisplayMemberPath = "Value";
			serial_select.SelectedValuePath = "Id";
							
			this.Content = main_grid;
		}
   
   
		public List<ComboData> GetSerialDevices(){
			ManagementClass            processClass = new ManagementClass("Win32_PnPEntity");
            ManagementObjectCollection ports        = processClass.GetInstances();
            List<ComboData>            ListData     = new List<ComboData>();          
     
            int i = 0;
            foreach(ManagementObject property in ports){                    
                if (property.GetPropertyValue("Name") != null)
                    if (property.GetPropertyValue("Name").ToString().Contains("USB") && property.GetPropertyValue("Name").ToString().Contains("COM")){
                		string name = property.GetPropertyValue("Name").ToString();
                        Trace.WriteLine(name);
                        ListData.Add(new ComboData { Id = i, Value = name });
                        Trace.WriteLine(i);
                        i++;
                    }						
            }
            return ListData;
		}
	}
	
	public class ComboData{ 
		public int Id { get; set; } 
		public string Value { get; set; } 
	}
 

}