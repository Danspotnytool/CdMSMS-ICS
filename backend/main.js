
const mysql = require('mysql2');
const connection = mysql.createConnection({
	host: 'localhost',
	user: 'root',
	password: ''
});

connection.connect((err) => {
	if (err) throw err;
	console.log('Connected!');
});

// Check if database exists, if not create it
connection.query('CREATE DATABASE IF NOT EXISTS cdmsms_ics', (err) => {
	if (err) throw err;
	console.log('Created Database: cdmsms_ics');
});

// Select the database
connection.query('USE cdmsms_ics', (err) => {
	if (err) throw err;
	console.log('Selected Database: cdmsms_ics');
});

// Database Structure
// Tables: admins, courses, faculties, facilities, students, schedules
// courses: courseCode: VARCHAR(10), description: TEXT, units: INT
// faculties: facultyID: VARCHAR(10), firstName: VARCHAR(50), lastName: VARCHAR(50), email: VARCHAR(50), password: VARCHAR(50), courses: TEXT, schedules: TEXT
// facilities: facilityID: VARCHAR(10), name: VARCHAR(50), description: TEXT, schedules: TEXT
// students: studentID: VARCHAR(10), firstName: VARCHAR(50), lastName: VARCHAR(50), courses: TEXT, regular: BOOLEAN, email: VARCHAR(50), password: VARCHAR(50), schedules: TEXT
// schedules: scheduleID: VARCHAR(10), courseCode: VARCHAR(10), facultyID: VARCHAR(10), facilityID: VARCHAR(10), day: VARCHAR(10), startTime: TIME, endTime: TIME
connection.query('CREATE TABLE IF NOT EXISTS courses (courseCode VARCHAR(10), description TEXT, units INT)', (err) => {
	if (err) throw err;
	console.log('\tCreated Table: courses');
});
connection.query('CREATE TABLE IF NOT EXISTS faculties (facultyID VARCHAR(10), firstName VARCHAR(50), lastName VARCHAR(50), email VARCHAR(50), password VARCHAR(50), courses TEXT, schedules TEXT)', (err) => {
	if (err) throw err;
	console.log('\tCreated Table: faculties');
});
connection.query('CREATE TABLE IF NOT EXISTS facilities (facilityID VARCHAR(10), name VARCHAR(50), description TEXT, schedules TEXT)', (err) => {
	if (err) throw err;
	console.log('\tCreated Table: facilities');
});
connection.query('CREATE TABLE IF NOT EXISTS students (studentID VARCHAR(10), firstName VARCHAR(50), lastName VARCHAR(50), courses TEXT, regular BOOLEAN, email VARCHAR(50), password VARCHAR(50), schedules TEXT)', (err) => {
	if (err) throw err;
	console.log('\tCreated Table: students');
});
connection.query('CREATE TABLE IF NOT EXISTS schedules (scheduleID VARCHAR(10), courseCode VARCHAR(10), facultyID VARCHAR(10), facilityID VARCHAR(10), day VARCHAR(10), startTime TIME, endTime TIME)', (err) => {
	if (err) throw err;
	console.log('\tCreated Table: schedules');
});



const express = require('express');
const app = express();

app.get('/', (req, res) => {
	res.send('Hello World!');
});
const port = process.env.PORT || 3090;
app.listen(port, () => {
	console.log(`Server started on http://localhost:${port}`);
});