var express = require('express');
var app = express();
var path = require("path");

var port = process.env.NODE_HOST_PORT || 8081;

var allowCrossDomain = function(req, res, next) {
  res.header('Access-Control-Allow-Origin', '*');
  res.header('Access-Control-Allow-Methods', 'GET,PUT,POST,DELETE');
  res.header('Access-Control-Allow-Headers', 'Content-Type');

  next();
}

app.use(express.static('public'));

app.listen(port, function () {
  console.log('Listening on port: ' + port);
});
