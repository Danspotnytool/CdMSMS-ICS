
const globals = require('../utils/globals.js');
const mail = require('../utils/mail.js');

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
                if (email !== user.email) {
                    res.status(404).send('Email not found');
                    return;
                };
                if (!user.password) {
                    res.status(401).send('User hasn\'t set a registered yet');
                    return;
                };
                const comparison = await bcrypt.compare(password, user.password);
                if (comparison) {
                    // Check if user is verified
                    if (user.verified) {
                        const userToReturn = {
                            ...user,
                            password: undefined,
                            studentID: undefined,
                            facultyID: undefined,
                            adminID: undefined
                        };
                        if (where === 'admins') userToReturn['identifier'] = 'adminID';
                        if (where === 'faculties') userToReturn['identifier'] = 'facultyID';
                        if (where === 'students') userToReturn['identifier'] = 'studentID';

                        userToReturn[userToReturn['identifier']] = user[userToReturn['identifier']];

                        res.send(userToReturn);
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

user.post('/forgotPassword/', async (req, res) => {
    const email = req.body['email'];

    if (!email) {
        res.status(400).send('Email is required');
        return;
    };

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

    // Get user
    /** @type {import('../utils/docs.js').Admin | import('../utils/docs.js').Faculty | import('../utils/docs.js').Student} */
    let user = null;
    await new Promise((resolve, reject) => {
        connection.query(`SELECT * FROM ${where} WHERE email = ?`, [email], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                user = results[0];
                resolve();
            };
        });
    });

    // Generate new forgotPasswordCode
    // 000-000-000
    const forgotPasswordCode = `${Math.floor(Math.random() * 1000).toString().padStart(3, '0')}-${Math.floor(Math.random() * 1000).toString().padStart(3, '0')}-${Math.floor(Math.random() * 1000).toString().padStart(3, '0')}`;

    // Set forgotPasswordCode
    await new Promise((resolve, reject) => {
        connection.query(`UPDATE ${where} SET forgotPasswordCode = ? WHERE email = ?`, [forgotPasswordCode, email], (err, result) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                resolve();
            };
        });
    });

    // Send email
    const subject = 'Forgot Password';
    const text = `Your forgot password code is ${forgotPasswordCode}`;
    await mail(email, subject, text);

    res.send('Email sent');
});

user.post('/resetPassword/', async (req, res) => {
    const email = req.body['email'];
    const forgotPasswordCode = req.body['forgotPasswordCode'];

    if (!email) {
        res.status(400).send('Email is required');
        return;
    };

    if (!forgotPasswordCode) {
        res.status(400).send('Forgot password code is required');
        return;
    };

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

    // Get user
    /** @type {import('../utils/docs.js').Admin | import('../utils/docs.js').Faculty | import('../utils/docs.js').Student} */
    let user = null;

    await new Promise((resolve, reject) => {
        connection.query(`SELECT * FROM ${where} WHERE email = ?`, [email], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                user = results[0];
                resolve();
            };
        });
    });

    // Check if forgotPasswordCode is correct
    if (forgotPasswordCode !== user.forgotPasswordCode) {
        res.status(401).send('Incorrect forgot password code');
        return;
    };

    res.send('Correct forgot password code');
});

user.post('/changePassword/', async (req, res) => {
    const email = req.body['email'];
    const forgotPasswordCode = req.body['forgotPasswordCode'];
    const password = req.body['password'];

    if (!email) {
        res.status(400).send('Email is required');
        return;
    };
    if (!forgotPasswordCode) {
        res.status(400).send('Forgot password code is required');
        return;
    };
    if (!password) {
        res.status(400).send('Password is required');
        return;
    };

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

    // Get user
    /** @type {import('../utils/docs.js').Admin | import('../utils/docs.js').Faculty | import('../utils/docs.js').Student} */
    let user = null;

    await new Promise((resolve, reject) => {
        connection.query(`SELECT * FROM ${where} WHERE email = ?`, [email], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                user = results[0];
                resolve();
            };
        });
    });

    // Check if forgotPasswordCode is correct
    if (forgotPasswordCode !== user.forgotPasswordCode) {
        res.status(401).send('Incorrect forgot password code');
        return;
    };

    // Hash password
    const hashedPassword = await bcrypt.hash(password, 10);

    // Set password
    await new Promise((resolve, reject) => {
        connection.query(`UPDATE ${where} SET password = ? WHERE email = ?`, [hashedPassword, email], (err, result) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                resolve();
            };
        });
    });

    // Set forgotPasswordCode to null
    await new Promise((resolve, reject) => {
        connection.query(`UPDATE ${where} SET forgotPasswordCode = NULL WHERE email = ?`, [email], (err, result) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                resolve();
            };
        });
    });

    res.send('Password changed');
});

user.post('/register/', async (req, res) => {
    const user = {
        firstName: req.body['firstName'],
        lastName: req.body['lastName'],
        userID: req.body['userID'],
        password: req.body['password'],
        email: req.body['email']
    };

    if (!user.firstName) {
        res.status(400).send('First name is required');
        return;
    };
    if (!user.lastName) {
        res.status(400).send('Last name is required');
        return;
    };
    if (!user.userID) {
        res.status(400).send('User ID is required');
        return;
    };
    if (!user.password) {
        res.status(400).send('Password is required');
        return;
    };
    if (!user.email) {
        res.status(400).send('Email is required');
        return;
    };

    // Check if user exists in faculties or students
    let found = false;
    let where = '';
    let identifier = '';

    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM faculties WHERE facultyID = ?', [user.userID], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                if (results.length > 0) {
                    found = true;
                    where = 'faculties';
                    identifier = 'facultyID';
                };
                resolve();
            };
        });
    });
    if (!found) await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM students WHERE studentID = ?', [user.userID], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                if (results.length > 0) {
                    found = true;
                    where = 'students';
                    identifier = 'studentID';
                };
                resolve();
            };
        });
    });

    if (!found) {
        // User does not exist in the database
        // Reject registration
        res.status(401).send('User ID not found');
        return;
    };

    // Get user
    /** @type {import('../utils/docs.js').Faculty | import('../utils/docs.js').Student}*/
    let userDB = null;
    await new Promise((resolve, reject) => {
        connection.query(`SELECT * FROM ${where} WHERE ${identifier} = ?`, [user.userID], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                userDB = results[0];
                resolve();
            };
        });
    });
    console.log(userDB);

    // Check if user is already verified or pending for verification
    if (userDB.verified) {
        res.status(401).send('User already exists and is verified');
        return;
    };
    if (userDB.verificationID) {
        res.status(401).send('User already exists and is pending for verification');
        return;
    };

    // Generate verificationID
    // userID.0000000000.0000000000
    const verificationID = `${user.userID}.${globals.randomString(10)}.${globals.randomString(10)}`;

    // Hash password
    const hashedPassword = await bcrypt.hash(user.password, 10);

    // GenerateToken
    const token = `${user.userID}.${globals.randomString(20)}.${globals.randomString(10)}`;

    // Set verificationID and password
    await new Promise((resolve, reject) => {
        connection.query(`UPDATE ${where} SET verificationID = ?, password = ?, token = ? WHERE ${identifier} = ?`, [verificationID, hashedPassword, token, user.userID], (err, result) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                resolve();
            };
        });
    });

    // Send email
    const subject = 'Verification';
    const text = `Click this link to verify your account: ${req.protocol}://${req.get('host')}/api/user/verify/${verificationID}`;
    await mail(user.email, subject, text);

    res.send('Verification email sent');
});

user.post('/check/', async (req, res) => {
    const data = {
        token: req.body['token'],
        identifier: req.body['identifier'],
        [req.body['identifier']]: req.body[req.body['identifier']]
    };

    if (!data.token) {
        res.status(400).send('Token is required');
        return;
    };
    if (!data.identifier) {
        res.status(400).send('Identifier is required');
        return;
    };

    const where = data.identifier === 'adminID' ? 'admins' : data.identifier === 'facultyID' ? 'faculties' : data.identifier === 'studentID' ? 'students' : null;
    if (!where) {
        res.status(400).send('Invalid identifier');
        return;
    };

    // Get user
    /** @type {import('../utils/docs.js').Admin | import('../utils/docs.js').Faculty | import('../utils/docs.js').Student}*/
    let user = null;
    await new Promise((resolve, reject) => {
        connection.query(`SELECT * FROM ${where} WHERE token = ? AND ${data.identifier} = ?`, [data.token, data[data.identifier]], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                user = results[0];
                resolve();
            };
        });
    });

    if (!user) {
        res.status(401).send('Unauthorized');
        return;
    };

    res.send(user);
});

user.post('/schedules/', async (req, res) => {
    const data = {
        token: req.body['token'],
        identifier: req.body['identifier'],
        [req.body['identifier']]: req.body[req.body['identifier']]
    };

    if (!data.token) {
        res.status(400).send('Token is required');
        return;
    };
    if (!data.identifier) {
        res.status(400).send('Identifier is required');
        return;
    };

    // if identifier is studentID, change it to the student's section
    if (data.identifier === 'studentID') {
        await new Promise((resolve, reject) => {
            connection.query('SELECT section FROM students WHERE studentID = ?', [data.studentID], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    data.studentID = undefined;
                    data.section = results[0].section;
                    data.identifier = 'section';
                    resolve();
                };
            });
        });
    };

    // Check if token exists in faculties or students
    let found = false;
    let where = '';
    let user = null;
    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM faculties WHERE token = ?', [data.token], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                if (results.length > 0) {
                    found = true;
                    where = 'faculties';
                    user = results[0];
                };
                resolve();
            };
        });
    });
    if (!found) await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM students WHERE token = ?', [data.token], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                if (results.length > 0) {
                    found = true;
                    where = 'students';
                    user = results[0];
                };
                resolve();
            };
        });
    });
    // if user is not found, reject
    if (!found) {
        res.status(401).send('Unauthorized');
        return;
    };

    // Get schedules
    const schedules = [];
    await new Promise((resolve, reject) => {
        connection.query(`SELECT * FROM schedules WHERE ${data.identifier} = ?`, [data[data.identifier]], async (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                // If identifier is section, get faculty name using facultyID
                for (const schedule of results) {
                    if (data.identifier === 'section') {
                        await new Promise((resolve, reject) => {
                            connection.query('SELECT firstName, lastName FROM faculties WHERE facultyID = ?', [schedule.facultyID], (err, results) => {
                                if (err) {
                                    res.status(500).send('Internal Server Error');
                                    reject(err);
                                } else {
                                    schedule.facultyName = `${results[0].firstName} ${results[0].lastName}`;
                                    resolve();
                                };
                            });
                        });
                    };
                    if (data.identifier === 'facultyID') {
                        schedule.section = schedule.courseCode.split('-')[0] === 'IT' ? `BSIT ${schedule.section}` : `BSCpE ${schedule.section}`;
                    }
                    schedules.push(schedule);
                };
                resolve();
            };
        });
    });

    // Get facility name using facilityID
    for (const schedule of schedules) {
        await new Promise((resolve, reject) => {
            connection.query('SELECT name FROM facilities WHERE facilityID = ?', [schedule.facilityID], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    schedule.facilityName = results[0].name;
                    resolve();
                };
            });
        });
    };

    res.send(schedules);
});

user.post('/identifierHeader/', async (req, res) => {
    const data = {
        token: req.body['token'],
        identifier: req.body['identifier'],
        [req.body['identifier']]: req.body[req.body['identifier']]
    };

    if (!data.token) {
        res.status(400).send('Token is required');
        return;
    };
    if (!data.identifier) {
        res.status(400).send('Identifier is required');
        return;
    };

    // if identifier is studentID, change it to the student's section
    if (data.identifier === 'studentID') {
        await new Promise((resolve, reject) => {
            connection.query('SELECT section FROM students WHERE studentID = ?', [data.studentID], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    data.studentID = undefined;
                    data.section = results[0].section;
                    data.identifier = 'section';
                    resolve();
                };
            });
        });
    };

    // Check if token exists in faculties or students
    let found = false;
    let where = '';
    let user = null;
    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM faculties WHERE token = ?', [data.token], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                if (results.length > 0) {
                    found = true;
                    where = 'faculties';
                    user = results[0];
                };
                resolve();
            };
        });
    });
    if (!found) await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM students WHERE token = ?', [data.token], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                if (results.length > 0) {
                    found = true;
                    where = 'students';
                    user = results[0];
                };
                resolve();
            };
        });
    });
    // if user is not found, reject
    if (!found) {
        res.status(401).send('Unauthorized');
        return;
    };

    // Get Header for the Identifier
    if (data.identifier === 'section') {
        await new Promise((resolve, reject) => {
            connection.query('SELECT * FROM students WHERE section = ?', [data.section], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    res.send(results[0].program === 'bsit' ? `BSIT ${data.section}` : `BSCpE ${data.section}`);
                    resolve();
                };
            });
        });
    };
    if (data.identifier === 'facultyID') {
        await new Promise((resolve, reject) => {
            connection.query('SELECT * FROM faculties WHERE facultyID = ?', [data.facultyID], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    res.send(`${results[0].firstName} ${results[0].lastName}`);
                    resolve();
                };
            });
        });
    };
});

user.post('/facilities/', async (req, res) => {
    const data = {
        token: req.body['token'],
        identifier: req.body['identifier'],
        [req.body['identifier']]: req.body[req.body['identifier']]
    };

    if (!data.token) {
        res.status(400).send('Token is required');
        return;
    };
    if (!data.identifier) {
        res.status(400).send('Identifier is required');
        return;
    };

    // if identifier is studentID, change it to the student's section
    if (data.identifier === 'studentID') {
        await new Promise((resolve, reject) => {
            connection.query('SELECT section FROM students WHERE studentID = ?', [data.studentID], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    data.studentID = undefined;
                    data.section = results[0].section;
                    data.identifier = 'section';
                    resolve();
                };
            });
        });
    };

    // Check if token exists in faculties
    let found = false;
    let where = '';
    let user = null;
    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM faculties WHERE token = ?', [data.token], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                if (results.length > 0) {
                    found = true;
                    where = 'faculties';
                    user = results[0];
                };
                resolve();
            };
        });
    });
    // if user is not found, reject
    if (!found) {
        res.status(401).send('Unauthorized');
        return;
    };

    // Get facilities
    const facilities = [];
    await new Promise((resolve, reject) => {
        connection.query(`SELECT * FROM facilities`, async (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                for (const facility of results) {
                    facilities.push(facility);
                };
                resolve();
            };
        });
    });

    res.send(facilities);
});

user.post('/request/', async (req, res) => {
    const data = {
        token: req.body['token'],
        identifier: req.body['identifier'],
        [req.body['identifier']]: req.body[req.body['identifier']]
    };

    if (!data.token) {
        res.status(400).send('Token is required');
        return;
    };
    if (!data.identifier) {
        res.status(400).send('Identifier is required');
        return;
    };

    // if identifier is studentID, change it to the student's section
    if (data.identifier === 'studentID') {
        await new Promise((resolve, reject) => {
            connection.query('SELECT section FROM students WHERE studentID = ?', [data.studentID], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    data.studentID = undefined;
                    data.section = results[0].section;
                    data.identifier = 'section';
                    resolve();
                };
            });
        });
    };

    // Check if token exists in faculties
    let found = false;
    let where = '';
    let user = null;
    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM faculties WHERE token = ?', [data.token], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                if (results.length > 0) {
                    found = true;
                    where = 'faculties';
                    user = results[0];
                };
                resolve();
            };
        });
    });
    // if user is not found, reject
    if (!found) {
        res.status(401).send('Unauthorized');
        return;
    };

    const requestData = {
        scheduleID: req.body['scheduleID'],
        facultyID: data[data['identifier']],
        facilityID: req.body['facilityID'],
        day: req.body['day'],
        startTime: `${globals.convertTime12to24(req.body['startTime'])}:00`,
        endTime: `${globals.convertTime12to24(req.body['endTime'])}:00`,
        reason: req.body['reason'],
        section: req.body['section']
    };

    if (!requestData.scheduleID || !requestData.facultyID || !requestData.facilityID || !requestData.day || !requestData.startTime || !requestData.endTime || !requestData.reason) {
        res.status(400).send('All fields are required');
        return;
    };

    let schedule = {};

    // Get schedule
    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM schedules WHERE scheduleID = ?', [requestData.scheduleID], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                if (results.length > 0) {
                    schedule = results[0];
                };
                resolve();
            };
        });
    });

    if (!schedule) {
        res.status(404).send('Schedule not found');
        return;
    };

    if (
        (requestData.facilityID === schedule.facilityID)
        & (requestData.day === schedule.day)
        & (requestData.startTime === schedule.startTime)
        & (requestData.endTime === schedule.endTime)
    ) {
        res.status(409).send('Schedule unchanged');
        return;
    };

    // Check if schedule is already requested
    let requested = false;
    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM requests WHERE scheduleID = ? AND status = ?', [requestData.scheduleID, 'pending'], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                if (results.length > 0) {
                    requested = true;
                };
                resolve();
            };
        });
    });

    if (requested) {
        res.status(409).send('Schedule already requested');
        return;
    };

    // Check for conflicts
    let conflicts = [];
    // Faculty
    await new Promise((resolve, reject) => {
        connection.query(`
            SELECT * FROM schedules
            WHERE facultyID = ?
            AND day = ?
            AND (
                (startTime BETWEEN ? AND ?)
                OR (endTime BETWEEN ? AND ?)
            )`, [requestData.facultyID, requestData.day, requestData.startTime, requestData.endTime, requestData.startTime, requestData.endTime], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                for (const result of results) {
                    conflicts.push(result);
                };
                resolve();
            };
        });
    });
    // Facility
    await new Promise((resolve, reject) => {
        connection.query(`
            SELECT * FROM schedules
            WHERE facilityID = ?
            AND day = ?
            AND (
                (startTime BETWEEN ? AND ?)
                OR (endTime BETWEEN ? AND ?)
            )`, [requestData.facilityID, requestData.day, requestData.startTime, requestData.endTime, requestData.startTime, requestData.endTime], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                for (const result of results) {
                    conflicts.push(result);
                };
                resolve();
            };
        });
    });
    // Section
    await new Promise((resolve, reject) => {
        connection.query(`
            SELECT * FROM schedules
            WHERE section = ?
            AND day = ?
            AND (
                (startTime BETWEEN ? AND ?)
                OR (endTime BETWEEN ? AND ?)
            )`, [requestData.section, requestData.day, requestData.startTime, requestData.endTime, requestData.startTime, requestData.endTime], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                for (const result of results) {
                    conflicts.push(result);
                };
                resolve();
            };
        });
    });

    // Remove this schedule from conflicts
    conflicts = conflicts.filter(conflict => conflict.scheduleID !== requestData.scheduleID);

    if (conflicts.length > 0) {
        res.status(409).send('Conflicts found');
        return;
    };

    const request = {
        requestID: globals.randomString(10),
        facultyID: requestData.facultyID,
        original: {
            facilityID: schedule.facilityID,
            day: schedule.day,
            startTime: schedule.startTime,
            endTime: schedule.endTime
        },
        request: {
            facilityID: requestData.facilityID,
            day: requestData.day,
            startTime: requestData.startTime,
            endTime: requestData.endTime
        },
        status: 'pending',
        requestReason: requestData.reason,
        rejectReason: null,
        requestDate: new Date().toISOString(),
        program: schedule.courseCode.split('-')[0] === 'IT' ? 'bsit' : 'bscpe',
        courseCode: schedule.courseCode,
        scheduleID: requestData.scheduleID
    };

    // Insert request
    await new Promise((resolve, reject) => {
        connection.query(`
            INSERT INTO requests (
                requestID,
                facultyID,
                original_facilityID,
                original_day,
                original_startTime,
                original_endTime,
                request_facilityID,
                request_day,
                request_startTime,
                request_endTime,
                status,
                requestReason,
                rejectReason,
                requestDate,
                program,
                courseCode,
                scheduleID
            )
            VALUES (?,
                    ?,
                    ?,
                    ?,
                    ?,
                    ?,
                    ?,
                    ?,
                    ?,
                    ?,
                    ?,
                    ?,
                    ?,
                    ?,
                    ?,
                    ?,
                    ?)`, [
            request.requestID,
            request.facultyID,
            request.original.facilityID,
            request.original.day,
            request.original.startTime,
            request.original.endTime,
            request.request.facilityID,
            request.request.day,
            request.request.startTime,
            request.request.endTime,
            request.status,
            request.requestReason,
            request.rejectReason,
            request.requestDate,
            request.program,
            request.courseCode,
            request.scheduleID
        ], (err, result) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                resolve();
            };
        });
    });

    res.send('Request sent');
    console.log(globals.rooms);
    for (const ws of globals.rooms['notifications_admin']) {
        ws.send("refresh");
    };
    for (const ws of globals.rooms['notifications_user']) {
        ws.send("refresh");
    };
});

user.post('/requests/', async (req, res) => {
    const data = {
        token: req.body['token'],
        identifier: req.body['identifier'],
        [req.body['identifier']]: req.body[req.body['identifier']]
    };

    if (!data.token) {
        res.status(400).send('Token is required');
        return;
    };
    if (!data.identifier) {
        res.status(400).send('Identifier is required');
        return;
    };

    // if identifier is studentID, change it to the student's section
    if (data.identifier === 'studentID') {
        await new Promise((resolve, reject) => {
            connection.query('SELECT section FROM students WHERE studentID = ?', [data.studentID], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    data.studentID = undefined;
                    data.section = results[0].section;
                    data.identifier = 'section';
                    resolve();
                };
            });
        });
    };

    // Check if token exists in faculties
    let found = false;
    let where = '';
    let user = null;
    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM faculties WHERE token = ?', [data.token], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                if (results.length > 0) {
                    found = true;
                    where = 'faculties';
                    user = results[0];
                };
                resolve();
            };
        });
    });
    // if user is not found, reject
    if (!found) {
        res.status(401).send('Unauthorized');
        return;
    };

    // Get requests
    const requests = [];
    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM requests WHERE facultyID = ?', [data[data.identifier]], async (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                for (const request of results) {
                    requests.push(request);
                };
                resolve();
            };
        });
    });

    const toReturn = [];

    for (const request of requests) {
        const requestToReturn = {
            requestID: request.requestID,
            facultyID: request.facultyID,
            facultyName: '',
            original: {
                facilityID: request.original_facilityID,
                facilityName: '',
                day: request.original_day,
                startTime: request.original_startTime,
                endTime: request.original_endTime
            },
            request: {
                facilityID: request.request_facilityID,
                facilityName: '',
                day: request.request_day,
                startTime: request.request_startTime,
                endTime: request.request_endTime
            },
            status: request.status,
            requestReason: request.requestReason,
            rejectReason: request.rejectReason,
            requestDate: request.requestDate,
            program: request.program,
            courseCode: request.courseCode,
            scheduleID: request.scheduleID
        };

        // Get faculty name
        await new Promise((resolve, reject) => {
            connection.query('SELECT firstName, lastName FROM faculties WHERE facultyID = ?', [request.facultyID], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    requestToReturn.facultyName = `${results[0].firstName} ${results[0].lastName}`;
                    resolve();
                };
            });
        });

        // Get facility name
        await new Promise((resolve, reject) => {
            connection.query('SELECT name FROM facilities WHERE facilityID = ?', [request.original_facilityID], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    requestToReturn.original.facilityName = results[0].name;
                    resolve();
                };
            });
        });
        await new Promise((resolve, reject) => {
            connection.query('SELECT name FROM facilities WHERE facilityID = ?', [request.request_facilityID], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    requestToReturn.request.facilityName = results[0].name;
                    resolve();
                };
            });
        });

        toReturn.push(requestToReturn);
    };

    // Sort requests by requestDate
    toReturn.sort((a, b) => new Date(b.requestDate) - new Date(a.requestDate));

    res.send(toReturn);
});
module.exports = user;