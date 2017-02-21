from kazoo.client import KazooClient
import os
import socket
import json
import logging

class ServiceAccessor(object):
   def __init__(self):
      logging.info("Initializing service accessor")
      root_path = os.path.realpath(os.path.dirname(__file__))
      config_location = os.path.join(root_path,"config.json")
      config_data = json.load(open(config_location))

      self.zkhost = config_data['zk_host']
      self.port = str(config_data['zk_port'])
      logging.info("Creating zookeeper client to connect to instance at %s:%s" % (self.zkhost,self.port))
      self.hostname = socket.gethostname()
      self.root_name = "services"
      self.zk = KazooClient(hosts="%s:%s" % (self.zkhost,self.port))
      logging.info("Successfully initialized")

   def connect(self):
      logging.info("Attempting to connect to zookeeper instance...")
      self.zk.start()
      logging.info("Successfully connected")

   def register_service(self, api_level, service_name, port):
      self.api_level = api_level
      self.service_name = service_name
      self.port = port

      root_path = "%s/%s/%s" % (self.root_name,api_level,service_name)
      host_name_path = "%s/host" % root_path
      port_path = "%s/port" % root_path

      logging.info("Registering %s service (version %s) to %s:%s" % (service_name,api_level,self.hostname,port))

      self.__create_node(host_name_path,self.hostname)
      self.__create_node(port_path,port)

   def disconnect(self):
      self.zk.stop()
   
   def get_service(self,api_level,service_name):
      root_path = "%s/%s/%s" % (self.root_name,api_level,service_name)
      host_name_path = "%s/host" % root_path
      port_path = "%s/port" % root_path
      
      hostname = self.__get_node(host_name_path)
      port = self.__get_node(port_path)

      return (hostname,port)

   def __get_node(self, path_name):
      if(not self.zk.exists(path_name)):
         raise ZookeeperException("Node %s not found" % path_name)

      data, stat = self.zk.get(path_name)
      return data.decode("utf-8")

   def __create_node(self, path_name, path_value):
      if(self.zk.exists(path_name)):
         self.zk.delete(path_name)

      self.zk.create(path_name,path_value,ephemeral=True,makepath=True)

class ZookeeperException(Exception):
   def __init__(self,value):
      self.value = value

   def __str__(self):
      return repr(self.value)
