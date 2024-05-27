
const connection = require('./utils/databaseConnection.js');
const logger = require('./utils/logger.js');

const express = require('express');
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
app.listen(port, () => {
	logger.log(`Server is running on port ${logger.colors.blue(port)}`);
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
		case 'exit':
			process.exit(0);
			break;
	};
});