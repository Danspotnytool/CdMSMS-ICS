
const connection = require('./utils/databaseConnection.js');
const logger = require('./utils/logger.js');
const globals = require('./utils/globals.js');

const express = require('express');
const WebSocket = require('ws');

const bodyParser = require('body-parser');
const app = express();

// Allow CORS from all origins
app.use((req, res, next) => {
	res.header('Access-Control-Allow-Origin', '*');
	res.header('Access-Control-Allow-Methods', 'GET, POST, PUT, DELETE');
	res.header('Access-Control-Allow-Headers', 'Content-Type');
	next();
});

app.use(bodyParser.json({ limit: '50mb' }));
app.use(bodyParser.urlencoded({ extended: true }));

// Middleware
app.use('*', (req, res, next) => {
	logger.log(`${req.method} ${req.originalUrl}`);
	next();
});

app.get('/', (req, res) => {
	res.send('Hello World!');
});

// Routers
app.use('/api/setup', require('./api/setup.js'));
app.use('/api/user', require('./api/user.js'));
app.use('/api/admin', require('./api/admin.js'));

const port = process.env.PORT || 3090;
const server = app.listen(port, () => {
	logger.log(`Server running on port ${port}`);
});



// Websocket
const wss = new WebSocket.Server({ server });



wss.on('connection', (ws, req) => {
	logger.log('New Connection');
	ws.on('message', (message) => {
		const data = JSON.parse(message);

		if (data.type === 'join') {
			globals.rooms[data.room].push(ws);
			logger.log(`User joined room: ${logger.colors.green(data.room)} with ${globals.rooms[data.room].length} users`);
		};
		if (data.type === 'leave') {
			globals.rooms[data.room] = globals.rooms[data.room].filter((room) => room !== ws);
			logger.log(`User left room: ${logger.colors.red(data.room)} with ${globals.rooms[data.room].length} users`);
		};
	});

	ws.on('close', () => {
		for (const room in globals.rooms) {
			// if user exists in room
			// remove user from room
			if (globals.rooms[room].includes(ws)) {
				globals.rooms[room] = globals.rooms[room].filter((room) => room !== ws);
				logger.log(`User left room: ${logger.colors.red(room)}`);
			};
		};
	});
});



process.stdin.on('data', (data) => {
	const input = data.toString();
	const args = input.trim().split(/ +/g);
	const commandName = args.shift().toLowerCase();
	switch (commandName) {
		case 'reset':
			connection.query('DROP DATABASE IF EXISTS cdmsms_ics', (err, results, fields) => {
				if (err) {
					logger.error(err);
					return;
				};
				logger.log(`Dropped Database: ${logger.colors.red('cdmsms_ics')}`);
				process.exit(0);
			});
			break;
		case 'eval':
			// Get the content inside quotes
			const quote = input.match(/"(.*?)"/g);
			if (!quote) {
				logger.error('No quote found');
				return;
			};
			const code = quote[0].replace(/"/g, '');
			eval(code);
			break;
		case 'ping':
			logger.log('pong');
			for (const connection of globals.connections) {
				connection.ws.send('ping');
			};
			break;
		case 'exit':
			process.exit(0);
			break;
	};
});