from kazoo.client import KazooClient
import os
import socket

class ServiceAccessor(object):
	def __init__(self):
		self.zkhost = os.environ['ZK_HOST']
		self.port = os.environ['ZK_PORT']
		self.hostname = socket.gethostname()
		self.root_name = "services"
		self.zk = KazooClient(hosts='%s:%s' % (self.zkhost,self.port))

	def connect(self):
		self.zk.start()

	def register_service(self, api_level, service_name, port):
		self.api_level = api_level
		self.service_name = service_name
		self.port = port

		root_path = "%s/%s/%s" % (self.root_name,api_level,service_name)
		host_name_path = "%s/host" % root_path
		port_path = "%s/port" % root_path

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
