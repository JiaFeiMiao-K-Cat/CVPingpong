#include <Servo.h>

Servo myServo[4]; 
int num = 0, pos = 0;

int buffer_cnt = 0, buffer_arr[4];

void setup() {
  myServo[0].attach(0); myServo[0].write(90); delay(100); 
  myServo[1].attach(4); myServo[1].write(90); delay(100); 
  myServo[2].attach(2); myServo[2].write(90); delay(100); 
  myServo[3].attach(5); myServo[3].write(90); delay(100); 
  
  pinMode(LED_BUILTIN, OUTPUT);
  Serial.begin(115200);
  while(!Serial);
}

void loop() { 
  while (Serial.available() > 0){
    buffer_arr[buffer_cnt++] = Serial.read();
    if (buffer_cnt == 4){
      buffer_cnt = 0;
      for (int i = 0; i < 4; i++){
        myServo[i].write(buffer_arr[i]);
      }
      delay(50); 
    }
  }
}
