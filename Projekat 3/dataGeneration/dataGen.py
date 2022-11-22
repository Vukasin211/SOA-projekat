import requests
import json
import time
import pandas

edgexip = '127.0.0.1'
dcPower = list()
acPower = list()
dailyYield = list()


#DC_POWER,AC_POWER,DAILY_YIELD


def readSensorData():
    data = pandas.read_csv("Plant_1_Generation_Data.csv")

    global dcPower
    global acPower
    global dailyYield

    dcPower = data["DC_POWER"].tolist()
    acPower = data["AC_POWER"].tolist()
    dailyYield = data["DAILY_YIELD"].tolist()


if __name__ == "__main__":
    readSensorData()
    i = 0
    while(i < len(dcPower)):

        url = 'http://%s:49986/api/v1/resource/solar_power_sensor/dcPower' % edgexip
        payload = dcPower[i]
        headers = {'content-type': 'application/json'}
        response = requests.post(url, data=json.dumps(
            payload), headers=headers, verify=False)

        url = 'http://%s:49986/api/v1/resource/solar_power_sensor/acPower' % edgexip
        payload = acPower[i]
        headers = {'content-type': 'application/json'}
        response = requests.post(url, data=json.dumps(
            payload), headers=headers, verify=False)

        url = 'http://%s:49986/api/v1/resource/solar_power_sensor/dailyYield' % edgexip
        payload = dailyYield[i]
        headers = {'content-type': 'application/json'}
        response = requests.post(url, data=json.dumps(
            payload), headers=headers, verify=False)

        i = i+1
        print(i)
        time.sleep(1)
