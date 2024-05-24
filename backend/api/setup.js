
const globals = require('../utils/globals.js');
const mail = require('../utils/mail.js');
const Readable = require('stream').Readable;

const bcrypt = require('bcrypt');
const csv = require('csv-parser');

const express = require('express');
const setup = express.Router();

const connection = require('../utils/databaseConnection.js');

setup.get('/', async (req, res) => {
	// Check if all table exists and if they have data
	let setup = true;
	// Admin
	await new Promise((resolve, reject) => {
		connection.query('SELECT * FROM admins', (err, results) => {
			if (err) {
				res.status(500).send('Internal Server Error');
				reject(err);
			} else if (results.length === 0) {
				setup = false;
			};
			resolve();
		});
	});
	// Courses
	await new Promise((resolve, reject) => {
		connection.query('SELECT * FROM courses', (err, results) => {
			if (err) {
				res.status(500).send('Internal Server Error');
				reject(err);
			} else if (results.length === 0) {
				setup = false;
			};
			resolve();
		});
	});
	// Faculties
	await new Promise((resolve, reject) => {
		connection.query('SELECT * FROM faculties', (err, results) => {
			if (err) {
				res.status(500).send('Internal Server Error');
				reject(err);
			} else if (results.length === 0) {
				setup = false;
			};
			resolve();
		});
	});
	// Facilities
	await new Promise((resolve, reject) => {
		connection.query('SELECT * FROM facilities', (err, results) => {
			if (err) {
				res.status(500).send('Internal Server Error');
				reject(err);
			} else if (results.length === 0) {
				setup = false;
			};
			resolve();
		});
	});
	// Students
	await new Promise((resolve, reject) => {
		connection.query('SELECT * FROM students', (err, results) => {
			if (err) {
				res.status(500).send('Internal Server Error');
				reject(err);
			} else if (results.length === 0) {
				setup = false;
			};
			resolve();
		});
	});

	if (setup) {
		res.send('Database is already setup. Login to the system to continue');
	} else {
		res.status(400).send('Not yet setup. Please setup first');
		// Empty all tables
		await new Promise((resolve, reject) => {
			connection.query('DELETE FROM admins', (err) => {
				if (err) {
					res.status(500).send('Internal Server Error');
					reject(err);
				} else {
					resolve();
				};
			});
		});
		await new Promise((resolve, reject) => {
			connection.query('DELETE FROM courses', (err) => {
				if (err) {
					res.status(500).send('Internal Server Error');
					reject(err);
				} else {
					resolve();
				};
			});
		});
		await new Promise((resolve, reject) => {
			connection.query('DELETE FROM faculties', (err) => {
				if (err) {
					res.status(500).send('Internal Server Error');
					reject(err);
				} else {
					resolve();
				};
			});
		});
		await new Promise((resolve, reject) => {
			connection.query('DELETE FROM facilities', (err) => {
				if (err) {
					res.status(500).send('Internal Server Error');
					reject(err);
				} else {
					resolve();
				};
			});
		});
		await new Promise((resolve, reject) => {
			connection.query('DELETE FROM students', (err) => {
				if (err) {
					res.status(500).send('Internal Server Error');
					reject(err);
				} else {
					resolve();
				};
			});
		});
	};
});

setup.post('/admin', async (req, res) => {
	/**
	 * @type {{
	 *		firstName: String,
	 *		lastName: String,
	 *		email: String,
	 *		password: String,
	 *		role: 'dean' | 'bsit' | 'bscpe'
	 * }}
	 */
	const admin = req.body;

	if (admin.firstName === undefined || admin.lastName === undefined || admin.email === undefined || admin.password === undefined || admin.role === undefined) {
		res.status(400).send('Missing required fields');
		return;
	};

	if (admin.firstName.length < 3) {
		res.status(400).send('First Name too short');
		return;
	};
	if (admin.lastName.length < 2) {
		res.status(400).send('Last Name too short');
		return;
	};
	if (!/^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/.test(admin.email)) {
		res.status(400).send('Invalid Email');
		return;
	};
	if (admin.password.length < 8) {
		res.status(400).send('Password too short');
		return;
	};

	admin.password = await bcrypt.hash(admin.password, 10);

	const adminID = globals.randomID();

	// Check if email is already in use
	await new Promise((resolve, reject) => {
		connection.query('SELECT * FROM admins WHERE email = ?', [admin.email], (err, results) => {
			if (err) {
				res.status(500).send('Internal Server Error');
				reject(err);
			} else if (results.length > 0) {
				res.status(400).send('Email already in use');
			} else {
				resolve();
			};
		});
	});

	// Generate verification code
	let verificationID = `${adminID}-${globals.randomString(30)}`;

	// Send verification email
	await mail(admin.email, 'Verify your account', `Click this link to verify your account: ${req.protocol}://${req.get('host')}/api/user/verify/${verificationID}`, `Click this link to verify your account: <a href="${req.protocol}://${req.get('host')}/api/user/verify/${verificationID}">Verify</a>`);

	// Generate token
	const token = `${globals.randomString(10)}.${globals.randomString(30)}`;

	connection.query('INSERT INTO admins (adminID, firstName, lastName, email, password, role, verificationID, verified, token) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)', [adminID, admin.firstName, admin.lastName, admin.email, admin.password, admin.role, verificationID, false, token], (err) => {
		if (err) {
			res.status(500).send('Internal Server Error');
			return;
		};
		res.send('success');
	});
});

/**
 * @type {(string: String) => Readable}
 */
const stringToStream = (string) => {
	const stream = new Readable();
	stream.push(string);
	stream.push(null);
	return stream;
};

setup.post('/program', async (req, res) => {
	const data = req.body;

	/**
	 * @type {{
	 * 		courses: {
	 * 			courseCode: String,
	 * 			description: String,
	 * 			units: Number,
	 * 			yearLevel: Number
	 * 		}[],
	 * 		faculties: {
	 * 			facultyId: String,
	 * 			firstName: String,
	 * 			lastName: String,
	 * 			email: String,
	 * 			programs: 'bsit' | 'bscpe' | 'both'
	 * 		}[],
	 * 		facilities: {
	 * 			facilityID: String,
	 * 			name: String,
	 * 			description: String
	 * 		}[],
	 * 		students: {
	 * 			studentID: String,
	 *			firstName: String,
	 *			lastName: String,
	 *			email: String,
	 *			section: String
	 * 		}[],
	 * 		program: 'bsit' | 'bscpe'
	 * }}
	 */
	const jsonData = {};

	for (const key in data) {
		const row = [];
		const promise = new Promise((resolve, reject) => {
			stringToStream(data[key])
				.pipe(csv())
				.on('data', (data) => {
					row.push(data);
				})
				.on('end', () => {
					resolve();
				})
				.on('error', (err) => {
					res.status(400).send('Invalid CSV');
					reject(err);
				});
		});
		await promise;
		jsonData[key] = row;
	};

	for (const faculty of jsonData.faculties) {
		if (faculty.facultyId === undefined || faculty.firstName === undefined || faculty.lastName === undefined || faculty.email === undefined) {
			console.log(faculty);
			res.status(400).send(`Missing required fields in faculties: ${JSON.stringify(faculty)}`);
			return;
		};
	};

	for (const facility of jsonData.facilities) {
		if (facility.facilityID === undefined || facility.name === undefined || facility.description === undefined) {
			console.log(facility);
			res.status(400).send(`Missing required fields in facilities: ${JSON.stringify(facility)}`);
			return;
		};
	};

	for (const student of jsonData.students) {
		if (student.studentID === undefined || student.firstName === undefined || student.lastName === undefined || student.email === undefined) {
			console.log(student);
			res.status(400).send(`Missing required fields in students: ${JSON.stringify(student)}`);
			return;
		};
	};

	jsonData.program = data.program;
	if (jsonData.program !== 'bsit' && jsonData.program !== 'bscpe') {
		res.status(400).send('Invalid program');
		return;
	};

	await new Promise((resolve, reject) => {
		for (const course of jsonData.courses) {
			connection.query('INSERT INTO courses (courseCode, description, units, yearLevel) VALUES (?, ?, ?, ?)', [course.courseCode, course.description, course.units, course.yearLevel], (err) => {
				if (err) {
					res.status(500).send('Internal Server Error');
					reject(err);
				} else {
					resolve();
				};
			});
		};
	});

	await new Promise((resolve, reject) => {
		for (const faculty of jsonData.faculties) {
			// Check if faculty already exists in the database
			// If it does, this means the faculty is already in the bsit program
			// Just change it to 'both'
			// If it doesn't, insert it as a new faculty
			// Then set the programs field to 'bsit' or 'bscpe' depending on the jsonData.program
			connection.query('SELECT * FROM faculties WHERE facultyID = ?', [faculty.facultyId], (err, results) => {
				if (err) {
					res.status(500).send('Internal Server Error');
					reject(err);
				} else if (results.length === 0) {
					connection.query('INSERT INTO faculties (facultyID, firstName, lastName, email, programs) VALUES (?, ?, ?, ?, ?)', [faculty.facultyId, faculty.firstName, faculty.lastName, faculty.email, jsonData.program], (err) => {
						if (err) {
							res.status(500).send('Internal Server Error');
							reject(err);
						} else {
							resolve();
						};
					});
				} else {
					connection.query('UPDATE faculties SET programs = ? WHERE facultyID = ?', [results[0].programs === 'bsit' ? 'both' : results[0].programs, faculty.facultyId], (err) => {
						if (err) {
							res.status(500).send('Internal Server Error');
							reject(err);
						} else {
							resolve();
						};
					});
				};
			});
		};
	});

	await new Promise((resolve, reject) => {
		for (const facility of jsonData.facilities) {
			// Check if facility already exists in the database
			// If it does, skip
			// If it doesn't, insert it as a new facility
			connection.query('SELECT * FROM facilities WHERE facilityID = ?', [facility.facilityID], (err, results) => {
				if (err) {
					res.status(500).send('Internal Server Error');
					reject(err);
				} else if (results.length === 0) {
					connection.query('INSERT INTO facilities (facilityID, name, description) VALUES (?, ?, ?)', [facility.facilityID, facility.name, facility.description], (err) => {
						if (err) {
							res.status(500).send('Internal Server Error');
							reject(err);
						} else {
							resolve();
						};
					});
				} else {
					resolve();
				};
			});
		};
	});

	await new Promise((resolve, reject) => {
		for (const student of jsonData.students) {
			connection.query('INSERT INTO students (studentID, firstName, lastName, email, program, section) VALUES (?, ?, ?, ?, ?, ?)', [student.studentID, student.firstName, student.lastName, student.email, jsonData.program, student.section], (err) => {
				if (err) {
					res.status(500).send('Internal Server Error');
					reject(err);
				} else {
					resolve();
				};
			});
		};
	});

	res.send('success');
});

module.exports = setup;
