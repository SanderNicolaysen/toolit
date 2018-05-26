#!/usr/bin/python
#--------------------------------------
#    ___  ___  _ ____
#   / _ \/ _ \(_) __/__  __ __
#  / , _/ ___/ /\ \/ _ \/ // /
# /_/|_/_/  /_/___/ .__/\_, /
#                /_/   /___/
#
#  lcd_16x2.py
#  16x2 LCD Test Script
#
# Author : Matt Hawkins
# Date   : 06/04/2015
#
# http://www.raspberrypi-spy.co.uk/
#
# Copyright 2015 Matt Hawkins
#
# This program is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program.  If not, see <http://www.gnu.org/licenses/>.
#
#--------------------------------------

# The wiring for the LCD is as follows:
# 1 : GND
# 2 : 5V
# 3 : Contrast (0-5V)*
# 4 : RS (Register Select)
# 5 : R/W (Read Write)       - GROUND THIS PIN
# 6 : Enable or Strobe
# 7 : Data Bit 0             - NOT USED
# 8 : Data Bit 1             - NOT USED
# 9 : Data Bit 2             - NOT USED
# 10: Data Bit 3             - NOT USED
# 11: Data Bit 4
# 12: Data Bit 5
# 13: Data Bit 6
# 14: Data Bit 7
# 15: LCD Backlight +5V**
# 16: LCD Backlight GND

#import
import RPi.GPIO as GPIO
import time
import _thread

# Define GPIO to LCD mapping
LCD_RS = 26
LCD_E  = 19
LCD_D4 = 13
LCD_D5 = 6
LCD_D6 = 5
LCD_D7 = 11


# Define some device constants
LCD_WIDTH = 16    # Maximum characters per line
LCD_CHR = True
LCD_CMD = False

LCD_LINE_1 = 0x80 # LCD RAM address for the 1st line
LCD_LINE_2 = 0xC0 # LCD RAM address for the 2nd line

# Timing constants
E_PULSE = 0.0005
E_DELAY = 0.0005

# LCD line busy
LINE_1_BUSY = False
LINE_2_BUSY = False

LINE_1_WAITING = False
LINE_2_WAITING = False

LCD_BUSY = False

def lcd_setup():
  # Main program block
  
  GPIO.setwarnings(False)
  GPIO.setmode(GPIO.BCM)       # Use BCM GPIO numbers
  GPIO.setup(LCD_E, GPIO.OUT)  # E
  GPIO.setup(LCD_RS, GPIO.OUT) # RS
  GPIO.setup(LCD_D4, GPIO.OUT) # DB4
  GPIO.setup(LCD_D5, GPIO.OUT) # DB5
  GPIO.setup(LCD_D6, GPIO.OUT) # DB6
  GPIO.setup(LCD_D7, GPIO.OUT) # DB7


  # Initialise display
  lcd_init()

def lcd_init():
  # Initialise display
  lcd_byte(0x33,LCD_CMD) # 110011 Initialise
  lcd_byte(0x32,LCD_CMD) # 110010 Initialise
  lcd_byte(0x06,LCD_CMD) # 000110 Cursor move direction
  lcd_byte(0x0C,LCD_CMD) # 001100 Display On,Cursor Off, Blink Off
  lcd_byte(0x28,LCD_CMD) # 101000 Data length, number of lines, font size
  lcd_byte(0x01,LCD_CMD) # 000001 Clear display
  time.sleep(E_DELAY)

def lcd_byte(bits, mode):
  # Send byte to data pins
  # bits = data
  # mode = True  for character
  #        False for command

  GPIO.output(LCD_RS, mode) # RS

  # High bits
  GPIO.output(LCD_D4, False)
  GPIO.output(LCD_D5, False)
  GPIO.output(LCD_D6, False)
  GPIO.output(LCD_D7, False)
  if bits&0x10==0x10:
    GPIO.output(LCD_D4, True)
  if bits&0x20==0x20:
    GPIO.output(LCD_D5, True)
  if bits&0x40==0x40:
    GPIO.output(LCD_D6, True)
  if bits&0x80==0x80:
    GPIO.output(LCD_D7, True)

  # Toggle 'Enable' pin
  lcd_toggle_enable()

  # Low bits
  GPIO.output(LCD_D4, False)
  GPIO.output(LCD_D5, False)
  GPIO.output(LCD_D6, False)
  GPIO.output(LCD_D7, False)
  if bits&0x01==0x01:
    GPIO.output(LCD_D4, True)
  if bits&0x02==0x02:
    GPIO.output(LCD_D5, True)
  if bits&0x04==0x04:
    GPIO.output(LCD_D6, True)
  if bits&0x08==0x08:
    GPIO.output(LCD_D7, True)

  # Toggle 'Enable' pin
  lcd_toggle_enable()

def lcd_toggle_enable():
  # Toggle enable
  time.sleep(E_DELAY)
  GPIO.output(LCD_E, True)
  time.sleep(E_PULSE)
  GPIO.output(LCD_E, False)
  time.sleep(E_DELAY)

def lcd_string(message,line):
  # Send string to display
  
  global LINE_1_BUSY, LINE_1_WAITING
  global LINE_2_BUSY, LINE_2_WAITING
  # some thread things
  if (line == LCD_LINE_1):
    LINE_1_WAITING = True
    while (LINE_1_BUSY):
      time.sleep(0.01)
    LINE_1_WAITING = False
    LINE_1_BUSY = True
  else:
    LINE_2_WAITING = True
    while (LINE_2_BUSY):
      time.sleep(0.01)
    LINE_1_WAITING = False
    LINE_2_BUSY = True
    
  try:
    _thread.start_new_thread(lcd_thread, (message, line, ))

  except:
    print ("ERROR: UNABLE TO PRINT TO LCD")

def lcd_thread(message,line):

  global LINE_1_BUSY, LINE_1_WAITING
  global LINE_2_BUSY, LINE_2_WAITING

  if len(message) > LCD_WIDTH:
    str_to_byte_print(message[0:LCD_WIDTH], line)
    time.sleep(1.5)
    for i in range(0, len(message) - (LCD_WIDTH - 1)):
      str_to_byte_print(message[i:i+LCD_WIDTH], line)
      time.sleep(0.2)
    time.sleep(1.3)

    if line == LCD_LINE_1:
      replay_scroll(message, line)
    else:
      replay_scroll(message, line)

  else:
    str_to_byte_print(message, line)
    time.sleep(2)

  if line == LCD_LINE_1:
    LINE_1_BUSY = False
  else:
    LINE_2_BUSY = False

def replay_scroll(message, line):

  global LINE_1_WAITING, LINE_2_WAITING, LINE_1_BUSY, LINE_2_BUSY

  first_replay = True

  if line == LCD_LINE_1:
    while not LINE_1_WAITING:
      if first_replay:
        first_replay = False
      else:
        time.sleep(1.3)
      i = 0
      str_to_byte_print(message[i:LCD_WIDTH], line)
      time.sleep(1.5)
      while i < len(message) - (LCD_WIDTH - 1) and not LINE_1_WAITING:
        str_to_byte_print(message[i:i + LCD_WIDTH], line)
        time.sleep(0.2)
        i += 1
  else:
    while not LINE_2_WAITING:
      if first_replay:
        first_replay = False
      else:
        time.sleep(1.3)
      i = 0
      str_to_byte_print(message[i:LCD_WIDTH], line)
      time.sleep(1.5)
      while i < len(message) - (LCD_WIDTH - 1) and not LINE_2_WAITING:
        str_to_byte_print(message[i:i + LCD_WIDTH], line)
        time.sleep(0.2)
        i += 1


  
def str_to_byte_print(message, line):
  message = message.ljust(LCD_WIDTH," ")

  global LCD_BUSY

  while LCD_BUSY:
    time.sleep(0.001)

  LCD_BUSY = True

  lcd_byte(line, LCD_CMD)
  for i in range(LCD_WIDTH):
    lcd_byte(ord(message[i]),LCD_CHR)
  
  LCD_BUSY = False

if __name__ == '__main__':

  try:
    print("don't run this file")
  except KeyboardInterrupt:
    pass
  finally:
    lcd_byte(0x01, LCD_CMD)
    lcd_string("Goodbye!",LCD_LINE_1)
    GPIO.cleanup()
