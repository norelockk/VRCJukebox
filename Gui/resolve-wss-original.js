// deprecated

// description: little idea to implement a kinda encrypted library that allows to resolve websocket url without pointless implementing it
// in a client-side of gui, but it's gonna be open source so it is pointless to make it encrypted so yeah, have a look
// how the origin idea for resolving websocket url directly from the engine goes.

(async function resolveWebSocketPath(global) {
  function stringContains(string, substring, position = 0) {
    return string.indexOf(substring, position) !== -1;
  }

  const request = await fetch('/api/resolve-wss');

  if (request.ok) {
    const method = global.DEV_MODE ? 'text' : 'json';
    const response = await request[method]();

    switch (method) {
      case 'text': {
        let jsonFromText;

        try {
          jsonFromText = JSON.parse(response);
        } catch (e) {
          throw new TypeError('json parse fail: ' + e.message);
        }

        if (jsonFromText)
          global.WSS_URL = stringContains(jsonFromText.url, '0.0.0.0') && (location.hostname !== 'localhost' || location.hostname !== '127.0.0.1') ? location.hostname : jsonFromText.url;
        break;
      }

      case 'json': {
        global.WSS_URL = stringContains(response.url, '0.0.0.0') && (location.hostname !== 'localhost'  || location.hostname !== '127.0.0.1') ? location.hostname : response.url;
        break;
      }

      default: throw new ReferenceError('method unknown');
    }

    console.log('resolved websocket path', global.WSS_URL);
  }
})(window);