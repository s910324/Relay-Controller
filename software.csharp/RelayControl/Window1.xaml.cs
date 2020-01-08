   
using System;
using System.Windows;
using System.Windows.Controls;
using System.IO.Ports;
using System.Text.RegularExpressions;                  
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Management;
using UI;

namespace RelayControl
{
     
	public partial class Window1 : Window
	{
		private ComboBox         serial_select     = new ComboBox();
		private ComboBox         bual_select       = new ComboBox();
		private Button           serial_confirm_pb = new Button();
		private ListView         serial_log_list   = new ListView();
//		private TextBox          serial_input      = new TextBox();
		private SubmitTextBox          serial_input      = new SubmitTextBox();
		private Button           serial_send_pb    = new Button();
		private SerialConnection connection        = new SerialConnection();
		public Window1(){
			InitializeComponent();
			InitializeUserInterface();
		}
		
	
		public void InitializeUserInterface(){
			this.MinWidth                   = 400;
			this.MinHeight                  = 400;
			this.Width                      = 450;
			this.Height                     = 600;
				
			Grid       serial_port_grid     = new HBox(serial_select, bual_select,  serial_confirm_pb).setGeomertries( "*", "*", "auto").setSpacing(5);
			Grid       serial_send_grid     = new HBox(serial_input, serial_send_pb).setGeomertries("*", "auto").setSpacing(5);
			VBox       main_grid            = new VBox(serial_port_grid, serial_log_list, serial_send_grid).setGeomertries("auto", "*", "auto").setSpacing(5).setMargin(5);
			
			List<BualComboData> bualrates   = new List<BualComboData>();
			this.bual_select.ItemsSource         = bualrates;
			this.bual_select.DisplayMemberPath   = "Display";
			this.bual_select.SelectedValuePath   = "Bual";
			this.serial_confirm_pb.Content       = "select";
			this.serial_send_pb.Content          = "Send";
			
			int i = 0;
			foreach (int rate in new int[] {2400, 4800, 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600}){
				bualrates.Add( new BualComboData{
					Id      = i,
					Bual    = rate,
					Status  = "",
					Display = "   "+ rate
				});
				i++;
			}
			
 
			GridView gv = new GridView();		
			foreach (string header in new string[] {"Id", "Time", "Status", "Data"}){
				GridViewColumn gvc = new GridViewColumn {
		            Header = header,      
		            CellTemplate = getDataTemplate(header) 
		        };
				gvc.Header = header;
				gv.Columns.Add(gvc);
			}
			
			this.serial_log_list.View = gv;
			
			connection = new SerialConnection("COM3", 19200);
			this.serial_log_list.ItemsSource    = connection.io_list;
			this.serial_send_pb.Click          += (o, a) => {this.SerialSend(connection, this.serial_input.Text); };
			this.serial_confirm_pb.Click       += (o, a) => {SerialConnection n = new SerialConnection(serial_select.SelectedValue.ToString(), (int)bual_select.SelectedValue);};
			this.serial_input.SubmitText       += (o, a) => {Trace.WriteLine(o.ToString());};
			this.serial_select.DropDownOpened  += (o, a) => {
				this.serial_select.ItemsSource       = GetSerialDevices();
				this.serial_select.DisplayMemberPath = "Display";
				this.serial_select.SelectedValuePath = "COM";
			};


			this.Content = main_grid;
		}
		
 		private DataTemplate getDataTemplate(string bind){                                             
			var T = new FrameworkElementFactory(typeof(TextBlock));
	        T.SetValue(TextBlock.TextProperty, new Binding(bind));
	        T.SetValue(TextBlock.FontFamilyProperty, new FontFamily("Lucida Console"));
	        T.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromRgb(50, 50, 50)));
	        T.SetValue(TextBlock.FontSizeProperty, 12.0);
	        var template = new DataTemplate();            
	        template.VisualTree = T;								 
	        return template;
		}
		
     	private void SerialSend(SerialConnection conn, string command ){
			try{
				conn.WriteLine(command);
			} catch (System.NullReferenceException){
				MessageBox.Show("Serial connection not established");
			}
			 
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
		public string Time    { get; set; }
		public string Status  { get; set; }
		public string Data    { get; set; }
	}
	
	public class SerialConnection{
		private SerialPort        connection;
		static  bool              _continue;
		public  string            port;
		public  List<SerialData>  io_list = new List<SerialData>();
		
		public  SerialConnection(){
		}
		
		public  SerialConnection(string port, int bual_rate){
			this.port               = port;
			connection              = new SerialPort(port, bual_rate, Parity.None, 8, StopBits.One);
	        connection.ReadTimeout  = 500;  
	        connection.WriteTimeout = 500; 			
			_continue               = true;
 
			try{
				if(!connection.IsOpen){connection.Open();}
				Thread readThread = new Thread(this.ReadLine);
				readThread.Start();
			} catch (System.IO.IOException) {
				MessageBox.Show(String.Format("Port '{0}' is not avaliable", this.port));
			} catch (System.UnauthorizedAccessException){
				MessageBox.Show(String.Format("Accessing Port '{0}' is not Unauthorized", this.port));
			}
  
 
		}
        
		public void ReadLine(){  
			while (_continue){  
				try{
					string messages = this.connection.ReadLine();
					foreach (string message in messages.Split(Environment.NewLine.ToCharArray())){
						io_list.Add(new SerialData{
							Id     = io_list.Count,
							Time   = DateTime.Now.ToString("HH:mm:ss"), 
							Status = "receieve", 
							Data   = message
						});
						Trace.WriteLine(message);
					}
				} catch (System.NullReferenceException){
					MessageBox.Show("Serial connection not established");					
				}catch (TimeoutException) { 
 
		        }
			}  
		} 
		
		public void WriteLine(string command){
			if (command.Length >0){
				try{
					this.connection.WriteLine(command); 
					io_list.Add(new SerialData{
						Id     = io_list.Count,
						Time   = DateTime.Now.ToString("HH:mm:ss"), 
						Status = "send", 
						Data   = command
					});
				} catch (System.NullReferenceException){
					MessageBox.Show("Serial connection not established");
				} catch (System.InvalidOperationException) {
					MessageBox.Show(String.Format("Port '{0}' is not avaliable", this.port));
				}
			}
			
		}
		
		public void Close(){this.connection.Close();}
	}

    
	
		
}