var config = require('./config.json')
var winston = require('winston')

module.exports = function(module) {

var logger = new (winston.Logger)({
   transports: [
      new (winston.transports.Console)({
         level: config.logLevel,
         formatter: function(options) {
            return options.level.toUpperCase() + ' ['+ module +'] : ' + options.message
         }
      })
   ]
});

return logger;
}
