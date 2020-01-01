 String deviceName  = "Relay Controller";
int    latchPin    =      8;
int    clockPin    =     12;
int    dataPin     =     11;
int    baudRate    =  19200;
int    stimeOut    =     50;
String switchState =  "N/A";

void      setup(){set_serial(baudRate, stimeOut); set_pin(latchPin, clockPin, dataPin); print(device_status());}
void    set_pin(int latchpin, int clockpin, int datapin){ pinMode(latchPin, OUTPUT); pinMode(clockPin, OUTPUT); pinMode(dataPin,  OUTPUT); }
void set_serial(int rate, int timeout){Serial.begin(rate); Serial.setTimeout(timeout); while (!Serial) {;}}

String device_status(){
    return 
        String(" \n") +
        "device name:    " + String(deviceName) +"\n"+
        "buad rate:      " + String(baudRate)   +"\n"+
        "port timeout:   " + String(stimeOut)   +"\n"+
        "switch state:   " + String(switchState)+"\n"+
        "to get more functions, please use 'help' command\n";
}

void loop(){ParseSerial();}

void ParseSerial(){
    if (Serial.available()> 0){
        String command = Serial.readString();
        command.replace("\n", ""); command.trim(); command.toLowerCase(); command = String(command);
        DispatchCommand(command);
    }
}

void DispatchCommand(String command){
    bool valid   = false;
    if ( command == "device_status"){valid = true; print(device_status());}
    
    if ( command.substring(0, 11) == "set_relay ["  and  command[command.length()-1]==']'){
        valid              = true;
        String bin_state   = command.substring(11,command.length()-1 ); bin_state.replace(" ", ""); bin_state.replace("_", "");
        for (int i = 0; i < bin_state.length(); i++){ if ( bin_state[i] != '1' and bin_state[i] != '0' ) {valid = false; break;}}

        if (valid){
            digitalWrite(latchPin, LOW);
            int zfill = (bin_state.length() % 8) == 0 ? 0 : (bin_state.length() % 8);
            for (int i = 0; i < zfill; i++){bin_state = "0" + bin_state;}
            for (int i = 0; i < (bin_state.length() / 8); i++){
                int relay_state = BinStringToDec(bin_state.substring(i*8,(i+1)*8));
                shiftOut(dataPin, clockPin, MSBFIRST, relay_state);
            }
            digitalWrite(latchPin, HIGH); 
        }
    } 
    
    int result = ParseCommand( "relay_debug", command);
    if ( result>=0 ){ valid = true; set_relay(result);}

    int rate = ParseCommand( "set_serial_rate", command);
    if ( rate > 0 ){ valid = true; baudRate = rate; set_serial(baudRate, stimeOut);}
    
    int timeout = ParseCommand( "set_serial_timeout", command);
    if ( timeout > 0 ){ valid = true; stimeOut = timeout; set_serial(baudRate, stimeOut);}

    if ( command.substring(0, 4) == "help" ){
        valid = true; print(
          String("\n") +
          "commands:\n" + 
          " get device status:       device_status;\n" +
          " set relay state:         set_relay [00111011];\n" +
          " set relay debug:         relay_debug 256;\n" +          
          " set COM bual rate:       set_serial_rate 300~19200;\n"+
          " set COM time out:        set_serial_timeout >0;\n");
    }
      
    if (valid){print("command:" + command + "\n");} else { print("invalid command: " + command + "\nto get more functions, please use 'help' command\n"); }
}

void set_relay(int relay_state){if( relay_state >= 0){
        digitalWrite(latchPin, LOW);
        shiftOut(dataPin, clockPin, MSBFIRST, relay_state); 
        digitalWrite(latchPin, HIGH);
    }
}

int ParseCommand(String target, String command){
    int result = -1;
    if (command.substring(0, target.length()) == target){result = command.substring(target.length()+1, command.length()).toInt();}
    return result;
}

int BinStringToDec(String bin_str){
    int result = 0;

    for ( int i = bin_str.length()-1; i >= 0 ; i--){
        int bin_digit = String(bin_str[i]).toInt();

        if ( bin_digit == 1 or bin_digit == 0){
            int power  = -1 * (i - (int)bin_str.length()+1);
            int decval = bin_digit * (int)(pow(2, power)+0.5);
            result += decval;        
        }else{
            break;
        }
    }
    return result;
}



void print(const char*  s){ Serial.println(String(s));}
void print(String       s){ Serial.println(s);}
void print(unsigned int s){ Serial.println(String(s));}
void print(int          s){ Serial.println(String(s));}
void print(float        s){ Serial.println(String(s));}
void print(double       s){ Serial.println(String(s));}
void print(bool         s){ Serial.println((s == true) ? "true" : "false");}
