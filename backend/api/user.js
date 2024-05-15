
const globals = require('../utils/globals.js');
const mail = require('../utils/mail.js');
const Readable = require('stream').Readable;

const bcrypt = require('bcrypt');
const csv = require('csv-parser');

const express = require('express');
const user = express.Router();

const connection = require('../utils/databaseConnection.js');



user.post('/login', async (req, res) => {
    const { email, password } = req.body;

    // Check if email exists in admins, faculties, or students
    let found = false;
    let where = '';

});



user.get('/verify/:id', async (req, res) => {
    const id = req.params.id;

    // Check if verification code exists in admins, faculties, or students
    let found = false;
    let where = '';

    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM admins WHERE verificationID = ?', [id], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else if (results.length > 0) {
                found = true;
                where = 'admins';
                resolve();
            } else {
                resolve();
            };
        });
    });

    if (!found) await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM faculties WHERE verificationID = ?', [id], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else if (results.length > 0) {
                found = true;
                where = 'faculties';
                resolve();
            } else {
                resolve();
            };
        });
    });

    if (!found) await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM students WHERE verificationID = ?', [id], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else if (results.length > 0) {
                found = true;
                where = 'students';
                resolve();
            } else {
                resolve();
            };
        });
    });

    if (!found) {
        res.status(404).send('Verification code not found');
        return;
    };

    // Set verified to true
    await new Promise((resolve, reject) => {
        connection.query(`UPDATE ${where} SET verified = true WHERE verificationID = ?`, [id], async (err, result) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                await new Promise((resolve, reject) => {
                    connection.query(`UPDATE ${where} SET verificationID = NULL WHERE verificationID = ?`, [id], (err, result) => {
                        if (err) {
                            res.status(500).send('Internal Server Error');
                            reject(err);
                        } else {
                            resolve();
                        };
                    });
                });
                res.send('Account verified');
                resolve();
            };
        });
    });
});



module.exports = user;