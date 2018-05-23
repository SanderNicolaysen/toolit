import requests

apikey = ""

with open("apikey.txt", "r") as f:
    apikey = f.readline()


while True:
    print(apikey)
    userId = input("Please scan your card")
    toolId = input("Please scan your tool")
    r = requests.post('http://localhost:5000/api/checkout', json = {
        'key': apikey,
        'userIdentifierCode': userId,
        'toolIdentifierCode': toolId
        })
    print(r.text)
    break
