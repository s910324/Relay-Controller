// Lab11 使用 74HC595 和三支腳位控制 8 顆 LED

// 接 74HC595 的 ST_CP (pin 12,latch pin)
int latchPin = 8;
// 接 74HC595 的 SH_CP (pin 11, clock pin)
int clockPin = 12;
// 接 74HC595 的 DS (pin 14)
int dataPin = 11;

void setup() {
  // 將 latchPin, clockPin, dataPin 設置為輸出
  pinMode(latchPin, OUTPUT);
  pinMode(clockPin, OUTPUT);
  pinMode(dataPin, OUTPUT);
}

void loop() {
  // 在 8 顆 LED 上計數數字，從 0 計數到 255
  for (int numberToDisplay = 0; numberToDisplay < 8; numberToDisplay++) {
    // 送資料前要先把 latchPin 拉成低電位
    digitalWrite(latchPin, LOW);
    
    // 使用 shiftOut 函式送出資料
    shiftOut(dataPin, clockPin, MSBFIRST, numberToDisplay);  

    // 送完資料後要把 latchPin 拉回成高電位
    digitalWrite(latchPin, HIGH);
    
    // 隔 500ms 後換下一個數字
    delay(100);

  }
}
