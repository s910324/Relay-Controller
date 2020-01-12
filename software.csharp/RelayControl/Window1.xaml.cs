   
using System;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;                  
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Management;
using UI;
using SerialConn;
using  System.Windows.Data;
namespace RelayControl
{
     
	public partial class Window1 : Window
	{
		private ComboBox                       serial_select     = new ComboBox();
		private ComboBox                       bual_select       = new ComboBox();
		private Button                         serial_confirm_pb = new Button();
		private UIListView<SerialData>         serial_log_list   = new UIListView<SerialData>();
		private SubmitTextBox                  serial_input      = new SubmitTextBox();
		private Button                         serial_send_pb    = new Button();
		private SerialConnection               connection        = new SerialConnection();
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
			this.serial_confirm_pb.Content       = "Connect";
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
			
			this.serial_send_pb.Click          += (o, a) => {this.SerialSend(this.connection,  this.serial_input.Text); this.serial_input.Clear();};
			this.serial_input.SubmitText       += (o, a) => {this.SerialSend(this.connection, ((SubmitTextBox)o).Text); this.serial_input.Clear();};
			this.serial_confirm_pb.Click       += (o, a) => {
				if (this.connection.IsConnected){
					this.connection.Close();
					this.serial_select.IsEnabled = !(this.serial_select.IsEnabled);
					this.bual_select.IsEnabled   = !(this.bual_select.IsEnabled);
				} else {
				
					if ((this.serial_select.SelectedIndex != -1) && (this.bual_select.SelectedIndex != -1)){
						this.connection = new SerialConnection(serial_select.SelectedValue.ToString(), (int)bual_select.SelectedValue);
						this.serial_log_list.ItemsSource = this.connection.io_list;
						this.serial_select.IsEnabled = !(this.serial_select.IsEnabled);
						this.bual_select.IsEnabled   = !(this.bual_select.IsEnabled);
					}
				}
			};
 
			this.serial_select.DropDownOpened  += (o, a) => {
				this.serial_select.ItemsSource       = GetSerialDevices();
				this.serial_select.DisplayMemberPath = "Display";
				this.serial_select.SelectedValuePath = "COM";
			};
			this.Content = main_grid;
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
	
		
}