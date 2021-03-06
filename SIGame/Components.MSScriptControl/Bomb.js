var _x = 0;
var _y = 0;
var _bombSpeed = 0;
var _dispatcher = null;
var _alive = false;

function asign(dispatcher, x, y, bombSpeed) {
    _x = x;
    _y = y;
    _bombSpeed = bombSpeed;
    _dispatcher = dispatcher;
}

function drop() {
    _alive = true;
    var worker = new Worker(getScriptPath(function () {
        self.addEventListener('message', function (e) {
            var value = 0;
            while (_y <= _dispatcher.height && _alive) {
                _y++;
                await sleep(_bombSpeed);
            }
            _dispatcher.destroyBomb(this);
        }, false);
    }));
    worker.postMessage(0);
}

function destroy() {
    _alive = false;
}

function draw(screen) {
    screen.draw(_x, _y, "+");
}
