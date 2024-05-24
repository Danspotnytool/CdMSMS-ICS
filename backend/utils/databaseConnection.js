
const logger = require('./logger.js');

const mysql = require('mysql2');

const connection = mysql.createConnection({
	host: 'localhost',
	user: 'root',
	password: ''
});

connection.connect((err) => {
	if (err) throw err;
	logger.log('Connected to MySQL Server');
});

// Check if database exists, if not create it
connection.query('CREATE DATABASE IF NOT EXISTS cdmsms_ics', (err) => {
	if (err) throw err;
});

// Select the database
connection.query('USE cdmsms_ics', (err) => {
	if (err) throw err;
	logger.log(`Database: ${logger.colors.blue('cdmsms_ics')}`);
});

// Database Structure
// 	Tables: admins, courses, faculties, facilities, students, schedules
// 		admins: adminID: VARCHAR(10), firstName: VARCHAR(50), lastName: VARCHAR(50), email: VARCHAR(50), password: VARCHAR(50), role: VARCHAR(10), verificationID: VARCHAR(50), verified: BOOLEAN, forgotPasswordCode: VARCHAR(50), token: VARCHAR(50)
//          primary key: adminID
// 		courses: courseCode: VARCHAR(20), description: TEXT, units: INT, yearLevel: INT
//			primary key: courseCode
// 		faculties: facultyID: VARCHAR(10), firstName: VARCHAR(50), lastName: VARCHAR(50), email: VARCHAR(50), password: VARCHAR(50), courses: TEXT, schedules: TEXT, programs: TEXT, verificationID: VARCHAR(50), verified: BOOLEAN, forgotPasswordCode: VARCHAR(50), token: VARCHAR(50)
// 			primary key: facultyID
// 		facilities: facilityID: VARCHAR(10), name: VARCHAR(50), description: TEXT, schedules: TEXT
// 			primary key: facilityID
// 		students: studentID: VARCHAR(10), firstName: VARCHAR(50), lastName: VARCHAR(50), courses: TEXT, email: VARCHAR(50), password: VARCHAR(50), program: VARCHAR(10), schedules: TEXT, section: VARCHAR(10), verificationID: VARCHAR(50), verified: BOOLEAN, forgotPasswordCode: VARCHAR(50), token: VARCHAR(50)
// 			primary key: studentID
// 		schedules: scheduleID: VARCHAR(10), courseCode: VARCHAR(10), facultyID: VARCHAR(10), facilityID: VARCHAR(10), day: VARCHAR(10), startTime: TIME, endTime: TIME, section: VARCHAR(10)
// 			primary key: scheduleID
// 		requests: requestID: VARCHAR(10), facultyID: VARCHAR(10), facilityID: VARCHAR(10), day: VARCHAR(10), startTime: TIME, endTime: TIME, status: VARCHAR(10)

connection.query(`
	CREATE TABLE IF NOT EXISTS admins (
		adminID VARCHAR(10) PRIMARY KEY NOT NULL,
		firstName VARCHAR(50),
		lastName VARCHAR(50),
		email VARCHAR(50),
		password VARCHAR(200),
		role VARCHAR(10),
		verificationID VARCHAR(50),
		verified BOOLEAN,
		forgotPasswordCode VARCHAR(50),
		token VARCHAR(50)
	)`, (err) => {
	if (err) throw err;
	logger.log(`\tTable: ${logger.colors.green('admins')}`);
});
connection.query(`
	CREATE TABLE IF NOT EXISTS courses (
		courseCode VARCHAR(20) PRIMARY KEY NOT NULL,
		description TEXT,
		yearLevel INT,
		units INT
	)`, (err) => {
	if (err) throw err;
	logger.log(`\tTable: ${logger.colors.green('courses')}`);
});
connection.query(`
	CREATE TABLE IF NOT EXISTS faculties (
		facultyID VARCHAR(10) PRIMARY KEY NOT NULL,
		firstName VARCHAR(50),
		lastName VARCHAR(50),
		email VARCHAR(50),
		password VARCHAR(200),
		courses TEXT,
		schedules TEXT,
		programs TEXT,
		verificationID VARCHAR(50),
		verified BOOLEAN,
		forgotPasswordCode VARCHAR(50),
		token VARCHAR(50)
	)`, (err) => {
	if (err) throw err;
	logger.log(`\tTable: ${logger.colors.green('faculties')}`);
});
connection.query(`
	CREATE TABLE IF NOT EXISTS facilities (
		facilityID VARCHAR(10) PRIMARY KEY NOT NULL,
		name VARCHAR(50),
		description TEXT,
		schedules TEXT
	)`, (err) => {
	if (err) throw err;
	logger.log(`\tTable: ${logger.colors.green('facilities')}`);
});
connection.query(`
	CREATE TABLE IF NOT EXISTS students (
		studentID VARCHAR(10) PRIMARY KEY NOT NULL,
		firstName VARCHAR(50),
		lastName VARCHAR(50),
		email VARCHAR(50),
		password VARCHAR(200),
		program VARCHAR(10),
		schedules TEXT,
		section VARCHAR(10),
		verificationID VARCHAR(50),
		verified BOOLEAN,
		forgotPasswordCode VARCHAR(50),
		token VARCHAR(50)
	)`, (err) => {
	if (err) throw err;
	logger.log(`\tTable: ${logger.colors.green('students')}`);
});

connection.query(`
	CREATE TABLE IF NOT EXISTS schedules (
		scheduleID VARCHAR(10) PRIMARY KEY NOT NULL,
		courseCode VARCHAR(10) NOT NULL,
		facultyID VARCHAR(10) NOT NULL,
		facilityID VARCHAR(10) NOT NULL,
		day VARCHAR(10),
		startTime TIME,
		endTime TIME,
		section VARCHAR(10)
	)`, (err) => {
	if (err) throw err;
	logger.log(`\tTable: ${logger.colors.green('schedules')}`);
});

connection.query(`
	CREATE TABLE IF NOT EXISTS requests (
		requestID VARCHAR(10) PRIMARY KEY NOT NULL,
		facultyID VARCHAR(10) NOT NULL,
		facilityID VARCHAR(10) NOT NULL,
		day VARCHAR(10),
		startTime TIME,
		endTime TIME,
		status VARCHAR(10)
	)`, (err) => {
	if (err) throw err;
	logger.log(`\tTable: ${logger.colors.green('requests')}`);
});

module.exports = connection;