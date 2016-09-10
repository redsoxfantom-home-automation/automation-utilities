var zookeeper = require('node-zookeeper-client');
var process = require('process');
var Q = require('q');

var zk_host = process.env.ZK_HOST;
var zk_port = process.env.ZK_PORT;

var client = zookeeper.createClient(zk_host+':'+zk_port);
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
