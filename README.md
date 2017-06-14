### COTS products to install
- zookeeper
  - sudo apt-get install zookeeper zookeeperd
- docker
  - Configure docker to also listen on tcp://0.0.0.0:2375
  - If docker is not available, can run "natively". That is, the services will run as daemons on the local system (remote deployment not possible)
    - Native deployments require additional COTS products, look at each service's build.gradle "additionalDependencies" for a list

### Template Variables
Template variables are applied at deployment time to any files that have the ".template" extension. They are used for "just in time" configuration prior to running, and set things like what port the application will listen on, or what host it will be deployed to. Variables in a ".template" file look like this: ${varName}. When the file is deployed, the ".template" will be stripped off the filename, and all variables will be replaced with the value defined by the template variable files.
   
#### Template Variable Files
Template variables are defined by files named "replacement.properties". These files contain a sequence of variable definitions of the form "\<variable name>=\<value>". The files are read in a hierarchical manner. That is, the file "lowest" is read first, then the next, then the next. If a replacement file defines a variable with the same name as one defined earlier, the earlier value is overriden.

##### "replacement.properties" Hierarchy

      $HOME/.home_automation/<application name>/replacement.properties
                  /\
      $HOME/.home_automation/replacement.properties
                  /\
      $GLOBAL/home_automation/<application name>/replacement.properties
                  /\
      $GLOBAL/home_automation/replacement.properties
                  /\
      $REPO_LOCATION/<application repo>/replacement.properties
                  /\
      $REPO_LOCATION/application-utilities/replacement.properties
      
For example, if $REPO_LOCATION/application-utilities/replacement.properties defines a variable "zookeeperPort=5000", but $HOME/.home_automation/replacement.properties defines the variable "zookeeperPort=6000", the final value of zookeeperPort will be 6000 since the second replacement.properties is higher in the hierarchy
   
$GLOBAL is the location on the host machine where global configuration goes. On Windows machines, this is wherever %ProgramData% points (usually C:\Program Data). On Linux, this is "/usr/etc". This folder will be created at build time if it doesn't already exist (the creation may require local admin priviliges). $REPO_LOCATION is the location that you cloned all the home_automation repos to. $HOME is the location of the current user's home directory.
   
Replacement files in $HOME or $GLOBAL can be used for defining host-specific properties. Replacement files in the repos define "useful defaults" and should not be modified. Ideally, you would put application specific properties in .home_automation/<application name>/replacement.properties, and "global" properties (like the zookeeper host and port) in .home_automation/replacement.properties

#### Template Variable Values
Applications can define their own specific variables, but the following are variables that all applications use:
- zookeeperHost - defines the hostname of the machine running zookeeper. This should be common to all applications, since zookeeper is how they discover each other
- zookeeperPort - defines the port number that the running instance of zookeeper is bound to.
- applicationPort - defines the port that this application will bind to.
- applicationHost - defines the host that will run this application (only applicable if you're using docker)
- dockerImage - defines the base docker image that this application will build off of (only applicable if you're using docker)
- logLevel - defines the minimum level the application logger logs at. Applications log to stdout. Acceptable levels (in order of severity) are: error, warning, info, debug
- forcenative - if set to "true", will force a native compile and deploy, instead of a docker-based one. You should probably only use this in dev to speed up testing
