import requests
from lcd_16x2 import *

# prints HTTP responses to console and lcd
def printresponse(response):
  lcd_string("HTTP " + str(response.status_code), LCD_LINE_1) 
  lcd_string(response.text.strip("\""), LCD_LINE_2)
  print(str(r.status_code) + '\n' + r.text.strip("\""))

# prints to console and lcd
def printlcd(str, line):
  lcd_string(str, line)
  print(str)

# prints to lcd and console, reads user input
# you might not want to use strings longer than 16 characters, as input won't be read until the lcd is finished printing.
def inputlcd(str, line):
  lcd_string(str, line)
  user_input = input(str + '\n')
  return user_input

if (__name__ == "__main__"):

  lcd_setup()

  apikey = ""

  # retrieve api key from file
  with open("apikey.txt", "r") as f:
    apikey = f.readline().strip()
  
  # validate key with server
  r = requests.post('http://159.89.1.137/api/echo', json = {'key': apikey})
  printresponse(r)

  # if key is invalid, inform user on how to get a proper key, then exit program
  if (r.status_code == 400):
    print("ERROR: please update key in apikey.txt\nNew keys can be generated in the admin-panel in your browser.")
    lcd_string("ERROR: BAD KEY", LCD_LINE_1)
    lcd_string("Please update API key in apikey.txt. New keys can be generated in the admin-panel in your browser.", LCD_LINE_2)
    lcd_init()
    exit()

  # keep running until manually shut down
  while True:

    # need to keep track of user and toolid
    userId = None
    toolId = None

    # resets request to avoid skipping loop below
    r = None

    # keep asking for userId until a valid one is found
    while (r == None or r.status_code != 200):
      printlcd("ToolIt Solutions", LCD_LINE_1)
      userId = inputlcd("Please scan card", LCD_LINE_2)
      r = requests.post('http://159.89.1.137/api/checkuser', json = {
        'key': apikey,
        'userIdentifierCode': userId})
      printresponse(r)

    username = r.text.strip("\"")

    # resets request to avoid skipping loop below
    r = None

    t = time.time()
    # greet user and ask for toolId until a valid one is found, or timer runs out
    while time.time() < t + 60 and (r == None or r.status_code != 200):
      printlcd("Hello, " + username, LCD_LINE_1)
      toolId = inputlcd("Please scan tool", LCD_LINE_2)
      r = requests.post('http://159.89.1.137/api/checktool', json={
        'key': apikey,
        'toolIdentifierCode': toolId})
      printresponse(r)
      if r.status_code == 200:
        # send checkout request
        r = requests.post('http://159.89.1.137/api/checkout', json = {
          'key': apikey,
          'userIdentifierCode': userId,
          'toolIdentifierCode': toolId
          })
        printresponse(r)
        break