#!/bin/bash
# Installs all supporting cots to run a home automation system
# Script must be run as root

if [ "$(id -u)" != "0" ]; then
   echo "Script must be run as root"
   exit 1
fi
