// jsonWebSocket客户端
function jsonWebSocket(url) {
    var ws;
    var packetId = 0;
    var callbackTable = [];
    var selfApi = {};

    // 是否连接
    this.connected = false;

    // 关闭时触发
    this.onclose = function (e) {
    };

    // 握手成功后触发
    this.onopen = function (e) {
    };

    // 绑定给服务器来调用的api，返回是否绑定成功
    // name：api名称
    // func: api实现函数
    this.bindApi = function (name, func) {
        if (typeof name !== "string" || !name || !isFunction(func)) {
            return false;
        }
        selfApi[name.toLowerCase()] = func;
        return true;
    }

    function isFunction(func) {
        return !!func && typeof func === "function";
    }

    // 调用服务器实现api，返回是否可以正常调用
    // api：远程端api名，不区分大小写
    // parameters：参数值数组，注意参数顺序(可选)
    // doneFunc：服务端返回api结果后触发的回调(可选)
    // exFunc：服务端返回异常信息后触发的回调(可选)    
    this.invokeApi = function (api, parameters, doneFunc, exFunc) {
        if (this.connected === false) {
            return false;
        }

        packetId = packetId + 1;
        var packet = { api: api, id: packetId, body: parameters || [] };
        var json = JSON.stringify(packet);

        if (isFunction(doneFunc) || isFunction(exFunc)) {
            callbackTable.push({ id: packetId, callback: { doneFunc: doneFunc, exFunc: exFunc } });
        }
        ws.send(json);
        return true;
    };

    // 获取回调
    function getCallback(id) {
        for (var i = 0; i < callbackTable.length; i++) {
            var item = callbackTable[i];
            if (item.id === id) {
                callbackTable.splice(i, 1);
                return item.callback;
            }
        }
    }

    // 收到文本消息时
    function onmessage(e) {
        var packet = JSON.parse(e.data);
        if (!packet.fromClient) {
            callApi.apply(this, [packet]);
            return;
        }

        var callback = getCallback(packet.id);
        if (!callback) {
            return;
        }

        var func = packet.state ? callback.doneFunc : callback.exFunc;
        if (isFunction(func)) {
            func(packet.body);
        }
    }

    // 调用自身实现的api
    function callApi(packet) {
        var api = selfApi[packet.api.toLowerCase()];
        if (!isFunction(api)) {
            setRemoteException(packet, '请求的api不存在：' + api);
            return;
        }

        try {
            var result = api.apply(this, packet.body);
            if (result !== undefined) {
                setApiResult(packet, result);
            }
        } catch (ex) {
            setRemoteException(packet, ex.message);
        }
    }

    // 将异常信息发送到服务端
    function setRemoteException(packet, message) {
        packet.state = false;
        packet.body = message;
        var json = JSON.stringify(packet);
        ws.send(json);
    }

    // 将api调用结果发送到服务端
    function setApiResult(packet, result) {
        packet.body = result;
        var json = JSON.stringify(packet);
        ws.send(json);
    }

    // 初始化
    function init() {
        var $this = this;
        ws = new (window.WebSocket || window.MozWebSocket)(url);
        ws.onclose = function (e) {
            $this.connected = false;
            $this.onclose(e);
        };
        ws.onopen = function (e) {
            $this.connected = true;
            $this.onopen(e);
        };
        ws.onmessage = function (e) {
            onmessage.apply($this, [e]);
        };
    }

    init.apply(this);
}
