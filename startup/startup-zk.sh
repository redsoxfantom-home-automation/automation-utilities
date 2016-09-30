#!/bin/bash
# Installs all supporting cots to run a home automation system
# Script must be run as root on the server that will host the zookeeper instance

if [ "$(id -u)" != "0" ]; then
   echo "Script must be run as root"
   exit 1
fi

# install zookeeper
apt-get install -y zookeeperd

echo "Zookeeper installed. Set zookeeperHost to '$(hostname)' and zookeeperPort to '2181' to access" 
