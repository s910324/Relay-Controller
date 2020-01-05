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
using System.Text.RegularExpressions;
using System.Threading;

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
//			SerialConnection n = new SerialConnection("COM4", 19200) ;

			
		}
		
	
		public void InitializeUserInterface(){
			ComboBox   serial_select     = new ComboBox();
			ComboBox   bual_select       = new ComboBox();
			Button     serial_confirm_pb = new Button();
			TabControl tab_page          = new TabControl();
			Grid       main_grid         = new Grid();
			Grid       serial_grid       = new Grid();
						
			main_grid.ColumnDefinitions.Add(new ColumnDefinition());		  
			main_grid.RowDefinitions.Add(new RowDefinition());
			main_grid.RowDefinitions.Add(new RowDefinition());
			main_grid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Auto);
			main_grid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);

			serial_grid.ColumnDefinitions.Add(new ColumnDefinition());
			serial_grid.ColumnDefinitions.Add(new ColumnDefinition());
			serial_grid.ColumnDefinitions.Add(new ColumnDefinition());
			serial_grid.RowDefinitions.Add(new RowDefinition());
			
			Grid.SetRow(serial_grid, 0);
			Grid.SetColumn(serial_grid, 0);
			main_grid.Children.Add(serial_grid); 
			
			Grid.SetRow(serial_select, 0);
			Grid.SetColumn(serial_select, 0);
			serial_grid.Children.Add(serial_select);
			
			Grid.SetRow(bual_select, 0);
			Grid.SetColumn(bual_select, 1);
			serial_grid.Children.Add(bual_select);
			
			Grid.SetRow(serial_confirm_pb, 0);
			Grid.SetColumn(serial_confirm_pb, 2);
			serial_grid.Children.Add(serial_confirm_pb);
			
			List<PortComboData> devices     = GetSerialDevices();
			serial_select.ItemsSource       = devices;
			serial_select.DisplayMemberPath = "Display";
			serial_select.SelectedValuePath = "Id";
			
			List<BualComboData> bualrates   = new List<BualComboData>();
			bual_select.ItemsSource         = bualrates;
			bual_select.DisplayMemberPath   = "Display";
			bual_select.SelectedValuePath   = "Id";
			
			serial_confirm_pb.Content       = "select";
			
			int i = 0;
			foreach (int rate in new int[] {2400, 4800, 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600}){
				bualrates.Add( new BualComboData{
					Id      = i,
					Bual    = rate,
					Status  = "",
					Display = "   "+ rate
				});
			}
			
			this.Content = main_grid;
		}
   
   
		public List<PortComboData> GetSerialDevices(){
			ManagementClass            processClass = new ManagementClass("Win32_PnPEntity");
            ManagementObjectCollection ports        = processClass.GetInstances();
            List<PortComboData>        ListData     = new List<PortComboData>();          
     
            int i = 0;
            foreach(ManagementObject property in ports){                    
                if (property.GetPropertyValue("Name") != null)
                	if (property.GetPropertyValue("Name").ToString().Contains("USB") && property.GetPropertyValue("Name").ToString().Contains("COM")){
                		string c_name    = property.GetPropertyValue("Name").ToString();
						Match  match     = Regex.Match(c_name, @"([\w|\W]+)\((COM([0-9])+)\)");
			   
						if (match.Success){
							ListData.Add(new PortComboData { 
				             	Id      = i, 
				             	Name    = match.Groups[1].Value, 
				             	COM     = match.Groups[2].Value,
				             	Display = String.Format("[{0}] -- {1}", match.Groups[2].Value, match.Groups[1].Value)
							});
							Trace.WriteLine(String.Format("index: {0} name: {1} COM: {2}", i, match.Groups[1].Value, match.Groups[2].Value));                                                                      
						}
                        i++;
                    }						
            }
            return ListData;
		}
		
   
	}
	
	public class PortComboData{ 
		public int    Id      { get; set; } 
		public string Name    { get; set; }
		public string COM     { get; set; }
		public string Display { get; set; }
	}
	
	public class BualComboData{ 
		public int    Id      { get; set; } 
		public int    Bual    { get; set; }
		public string Status  { get; set; }
		public string Display { get; set; }
	}
	
	public class SerialConnection{
		private SerialPort connection;
		static  bool       _continue;
		
		public  SerialConnection(string port, int bual_rate){
			connection = new SerialPort(port, bual_rate, Parity.None, 8, StopBits.One);
	        connection.ReadTimeout  = 500;  
	        connection.WriteTimeout = 500; 			
			_continue = true;
 
			if(!connection.IsOpen){connection.Open(); Trace.WriteLine("open");}
			Thread readThread = new Thread(Read);
			readThread.Start();
	        while (_continue){
				connection.WriteLine("help");
				Thread.Sleep(1000);
	        }
	        connection.Close();
	        
		}
        
		public  void Read(){  
			while (_continue){  
				try{
					string message = connection.ReadLine();  
					Trace.WriteLine(message);  
				}catch (TimeoutException) { 
		        
		        }
			}  
		} 
	}

    
	
		
}