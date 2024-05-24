
const globals = require('../utils/globals.js');

const bcrypt = require('bcrypt');

const express = require('express');
const user = express.Router();

const connection = require('../utils/databaseConnection.js');



user.post('/login', async (req, res) => {
    const { email, password } = req.body;

    // Check if email exists in admins, faculties, or students
    let found = false;
    let where = '';

    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM admins WHERE email = ?', [email], (err, results) => {
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
        connection.query('SELECT * FROM faculties WHERE email = ?', [email], (err, results) => {
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
        connection.query('SELECT * FROM students WHERE email = ?', [email], (err, results) => {
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
        res.status(404).send('Email not found');
        return;
    };

    // Check if password is correct
    await new Promise((resolve, reject) => {
        connection.query(`SELECT * FROM ${where} WHERE email = ?`, [email], async (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                /**
                 * @type {import('../utils/docs.js').Admin | import('../utils/docs.js').Faculty | import('../utils/docs.js').Student}
                 */
                const user = results[0];
                const comparison = await bcrypt.compare(password, user.password);
                if (comparison) {
                    // Check if user is verified
                    if (user.verified) {
                        console.log(user);
                        res.send(user);
                    } else {
                        res.status(401).send('Account not verified. Please check your email for the verification code');
                    };
                } else {
                    res.status(401).send('Incorrect password');
                };
                resolve();
            };
        });
    });
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