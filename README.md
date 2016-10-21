COTS products to install:
zookeeper
	sudo apt-get install zookeeper zookeeperd
docker
   Configure docker to also listen on tcp://0.0.0.0:2376 to allow remote deployments
   If docker is not available, can run "natively". That is, the services will run as daemons on the local system (remote deployment not possible)
      Native deployments require additional COTS products, look at each service's build.gradle "additionalDependencies" for a list
