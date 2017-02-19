var zookeeper = require('node-zookeeper-client');
var config = require('./config.json');
var Q = require('q');

var client = zookeeper.createClient(config.zk_host+':'+config.zk_port);
var successfullyConnected = false;

client.once('connected',function() {
	successfullyConnected = true;
});
client.connect();

getPathData = function(path) {
	var deferred = Q.defer();
	var promise = deferred.promise;

	client.getData(
		path,
		function(event){},
		function(err,data,stat) {
			if (err) throw err;

			var dataStr = data.toString('utf8');
			deferred.resolve(dataStr);
		}
	);

	return promise;
}

exports.getService = function(apiLevel, serviceName, callback) {
	var root = '/services/'+apiLevel+'/'+serviceName;
	var resolvedHost;
	var resolvedPort;	

	Q.all([getPathData(root+'/host'),getPathData(root+'/port')])
		.then(function(items) {
			callback(null, {
				host : items[0],
				port : items[1]
			})
		});
}
