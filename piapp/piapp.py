import requests
from lcd_16x2 import *

lcd_setup()
apikey = ""

with open("apikey.txt", "r") as f:
    apikey = f.readline().strip()


while True:
    lcd_string("ToolIt Solutions", LCD_LINE_1)
    lcd_string("Please scan card", LCD_LINE_2)
    userId = input("Please scan your card\n")
    lcd_string("Please scan tool", LCD_LINE_2)
    toolId = input("Please scan your tool\n")
    r = requests.post('http://159.89.1.137/api/checkout', json = {
        'key': apikey,
        'userIdentifierCode': userId,
        'toolIdentifierCode': toolId
        })
    lcd_string(r.text.strip("\""), LCD_LINE_2)
    print(r.text)

