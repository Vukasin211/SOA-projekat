cd "$(dirname "$0")"
cd ./deviceCreation
pip install -r requirements.txt
python createSensorCluster.py &
python createRESTDevice.py -ip 127.0.0.1 -devip color-changer &
cd ../dataGeneration
pip install -r requirements.txt
python dataGen.py