// JavaScript source code

var app = require('express')();
var http = require('http').Server(app);
var io = require('socket.io')(http);

var mqWarehouse = require('../mq/mqWarehouse');

var warehouseSocket;

http.listen(4000, function () {
    console.log('listening on *:4000');
});


io.on('connection', function (socket) {
    console.log('a user connected');

    warehouseSocket = socket;

    warehouseSocket.on('order', function (msg) {
        console.log('message received: ', 'order\n', msg);
        warehouseSocket.emit("info", msg);
    });

    warehouseSocket.on('shipping', function (msg) {
        console.log('message received: ', 'shipping\n', msg);
        //warehouseSocket.emit("ack", "received ship order");

        //rabbitmq
        mqWarehouse.sendMsg(msg);
    });

    warehouseSocket.on('getOrders', function (msg) {
        console.log('message received: ', 'getOrders\n', msg);

        //TODO
        //getOrderslist from database

        warehouseSocket.emit("orderList", "");
    });
    
    /* socket.disconnect() or socket.close() triggers disconnect event */
    warehouseSocket.on('disconnect', function () {
        console.log("user disconnected");
        io.emit('user disconnected');
    });
});

function sendMsg(msg, data) {
    console.log('check 1 >>>>>>>>>>');

    io.emit(msg, data);
}

module.exports.sendMsg = sendMsg;