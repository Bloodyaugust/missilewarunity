(function (Messaging) {
  var listeners = [];

  Messaging.trigger = function (event, data) {
    for (var i = 0; i < listeners.length; i++) {
      listeners[i](event, data);
    }
  }

  Messaging.addListener = function (callback) {
    listeners.push(callback);
  }
})(window.Messaging);
