EESchema Schematic File Version 4
EELAYER 30 0
EELAYER END
$Descr A4 11693 8268
encoding utf-8
Sheet 1 1
Title ""
Date ""
Rev ""
Comp ""
Comment1 ""
Comment2 ""
Comment3 ""
Comment4 ""
$EndDescr
$Comp
L 74xx:74HC595 U?
U 1 1 5E0CDFFE
P 2950 2350
F 0 "U?" H 2950 3131 50  0000 C CNN
F 1 "74HC595" H 2950 3040 50  0000 C CNN
F 2 "" H 2950 2350 50  0001 C CNN
F 3 "http://www.ti.com/lit/ds/symlink/sn74hc595.pdf" H 2950 2350 50  0001 C CNN
	1    2950 2350
	1    0    0    -1  
$EndComp
$Comp
L 74xx:74HC595 U?
U 1 1 5E0CEAE9
P 2950 4100
F 0 "U?" H 2950 4881 50  0000 C CNN
F 1 "74HC595" H 2950 4790 50  0000 C CNN
F 2 "" H 2950 4100 50  0001 C CNN
F 3 "http://www.ti.com/lit/ds/symlink/sn74hc595.pdf" H 2950 4100 50  0001 C CNN
	1    2950 4100
	1    0    0    -1  
$EndComp
$Comp
L 74xx:74HC595 U?
U 1 1 5E0CFCC1
P 4650 2350
F 0 "U?" H 4650 3131 50  0000 C CNN
F 1 "74HC595" H 4650 3040 50  0000 C CNN
F 2 "" H 4650 2350 50  0001 C CNN
F 3 "http://www.ti.com/lit/ds/symlink/sn74hc595.pdf" H 4650 2350 50  0001 C CNN
	1    4650 2350
	1    0    0    -1  
$EndComp
$Comp
L 74xx:74HC595 U?
U 1 1 5E0D0664
P 4700 4100
F 0 "U?" H 4700 4881 50  0000 C CNN
F 1 "74HC595" H 4700 4790 50  0000 C CNN
F 2 "" H 4700 4100 50  0001 C CNN
F 3 "http://www.ti.com/lit/ds/symlink/sn74hc595.pdf" H 4700 4100 50  0001 C CNN
	1    4700 4100
	1    0    0    -1  
$EndComp
$Comp
L Device:C_Small C?
U 1 1 5E0D1A98
P 5850 1850
F 0 "C?" H 5942 1896 50  0000 L CNN
F 1 "C_Small" H 5942 1805 50  0000 L CNN
F 2 "" H 5850 1850 50  0001 C CNN
F 3 "~" H 5850 1850 50  0001 C CNN
	1    5850 1850
	1    0    0    -1  
$EndComp
$Comp
L MCU_Module:Arduino_Nano_v2.x A?
U 1 1 5E0D286E
P 7000 2750
F 0 "A?" H 7000 1661 50  0000 C CNN
F 1 "Arduino_Nano_v2.x" H 7000 1570 50  0000 C CNN
F 2 "Module:Arduino_Nano" H 7000 2750 50  0001 C CIN
F 3 "https://www.arduino.cc/en/uploads/Main/ArduinoNanoManual23.pdf" H 7000 2750 50  0001 C CNN
	1    7000 2750
	1    0    0    -1  
$EndComp
$Comp
L Connector_Generic:Conn_02x10_Odd_Even J?
U 1 1 5E0D8B01
P 2950 6850
F 0 "J?" V 3046 6262 50  0000 R CNN
F 1 "Conn_02x10_Odd_Even" V 2955 6262 50  0000 R CNN
F 2 "" H 2950 6850 50  0001 C CNN
F 3 "~" H 2950 6850 50  0001 C CNN
	1    2950 6850
	0    -1   -1   0   
$EndComp
$Comp
L Connector_Generic:Conn_02x10_Odd_Even J?
U 1 1 5E0DD231
P 4850 6250
F 0 "J?" V 4946 5662 50  0000 R CNN
F 1 "Conn_02x10_Odd_Even" V 4855 5662 50  0000 R CNN
F 2 "" H 4850 6250 50  0001 C CNN
F 3 "~" H 4850 6250 50  0001 C CNN
	1    4850 6250
	0    -1   -1   0   
$EndComp
Wire Wire Line
	2950 3050 4650 3050
Wire Wire Line
	4650 3050 6200 3050
Wire Wire Line
	6200 3050 6200 4800
Wire Wire Line
	6200 4800 4700 4800
Connection ~ 4650 3050
Wire Wire Line
	2950 4800 4700 4800
Connection ~ 4700 4800
$EndSCHEMATC
