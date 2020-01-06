   
using System;
using System.Collections.Generic;
using System.Windows;
using System.Management;
using System.Windows.Controls;
using System.IO.Ports;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Data;
using System.Windows.Media;


   
using UI;

namespace RelayControl
{
     
	public partial class Window1 : Window
	{
		private ComboBox   serial_select     = new ComboBox();
		private ComboBox   bual_select       = new ComboBox();
		private Button     serial_confirm_pb = new Button();
		private ListView   serial_log_list   = new ListView();
		private TextBox    serial_input      = new TextBox();
		private Button     serial_send_pb    = new Button();
		
		public Window1()
		{
			InitializeComponent();
			
			InitializeUserInterface();
			

			
		}
		
	
		public void InitializeUserInterface(){
			this.MinWidth  = 400;
			this.MinHeight = 400;
			this.Width     = 450;
			this.Height    = 600;
				

			
			Grid       serial_port_grid  = new HBox(serial_select, bual_select,  serial_confirm_pb).setGeomertries( "*", "*", "auto").setSpacing(5);
			Grid       serial_send_grid  = new HBox(serial_input, serial_send_pb).setGeomertries("*", "auto").setSpacing(5);
			VBox       main_grid         = new VBox(serial_port_grid, serial_log_list, serial_send_grid).setGeomertries("auto", "*", "auto").setSpacing(5).setMargin(5);
																			  
			
			List<PortComboData> devices     = GetSerialDevices();
			serial_select.ItemsSource       = devices;
			serial_select.DisplayMemberPath = "Display";
			serial_select.SelectedValuePath = "Id";
			
			List<BualComboData> bualrates   = new List<BualComboData>();
			bual_select.ItemsSource         = bualrates;
			bual_select.DisplayMemberPath   = "Display";
			bual_select.SelectedValuePath   = "Id";
			
			serial_confirm_pb.Content       = "select";
			serial_send_pb.Content          = "Send";
			
			int i = 0;
			foreach (int rate in new int[] {2400, 4800, 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600}){
				bualrates.Add( new BualComboData{
					Id      = i,
					Bual    = rate,
					Status  = "",
					Display = "   "+ rate
				});
			}
			SerialConnection n = new SerialConnection("COM4", 19200);
			this.serial_log_list.ItemsSource = n.io_list;	
			GridView gv = new GridView();
			
			
			foreach (string header in new string[] {"Id", "Status", "Time", "Data"}){
				GridViewColumn gvc = new GridViewColumn {
		            Header = header,
//		            DisplayMemberBinding = new Binding(header)
		            CellTemplate = getDataTemplate(header) 
		        };
				gvc.Header = header;
 
				gv.Columns.Add(gvc);
			}
			
			this.serial_log_list.View = gv;
			
			serial_send_pb.Click += (o, a)=>{this.SerialSend(n, "help"); };
			
	                                 
			
			this.Content = main_grid;
		}
		
 		private DataTemplate getDataTemplate(string bind){
            var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
	        textBlockFactory.SetValue(TextBlock.TextProperty, new Binding(bind));
	        textBlockFactory.SetValue(TextBlock.FontFamilyProperty, new FontFamily("Lucida Console"));
//	        textBlockFactory.SetValue(TextBlock.BackgroundProperty, Brushes.Red);
	        textBlockFactory.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromRgb(50, 50, 50)));

	
	        textBlockFactory.SetValue(TextBlock.FontSizeProperty, 12.0);
	
	        var template = new DataTemplate();            
	        template.VisualTree = textBlockFactory;
	
	        return template;
 

		}
		
     	private void SerialSend(SerialConnection conn, string command ){
			conn.WriteLine(command);
			
			
		}
   
		private List<PortComboData> GetSerialDevices(){
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
	
	public class SerialData{
		public int    Id      { get; set; }
		public string Status  { get; set; }
		public string Time    { get; set; }
		public string Data    { get; set; }
	}
	
	public class SerialConnection{
		private SerialPort        connection;
		static  bool              _continue;
		public  List<SerialData>  io_list = new List<SerialData>();
		
		public  SerialConnection(string port, int bual_rate){
			connection = new SerialPort(port, bual_rate, Parity.None, 8, StopBits.One);
	        connection.ReadTimeout  = 500;  
	        connection.WriteTimeout = 500; 			
			_continue               = true;
 
			if(!connection.IsOpen){connection.Open();}
			Thread readThread = new Thread(this.ReadLine);
			readThread.Start();
 
		}
        
		public void ReadLine(){  
			while (_continue){  
				try{
					string message = this.connection.ReadLine();
					io_list.Add(new SerialData{
						Id     = io_list.Count, 
						Status = "receieve",
						Time   = DateTime.Now.ToString("HH:mm:ss"), 
						Data   = message
					});
					Trace.WriteLine(message);						  
				}catch (TimeoutException) { 
		        
		        }
			}  
		} 
		
		public void WriteLine(string command){
			this.connection.WriteLine(command); 
			io_list.Add(new SerialData{
				Id     = io_list.Count, 
				Status = "send",
				Time   = DateTime.Now.ToString("HH:mm:ss"), 
				Data   = command
			});
		}
		
		public void Close(){this.connection.Close();}
	}

    
	
		
}