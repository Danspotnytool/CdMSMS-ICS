
const globals = require('../utils/globals.js');

const bcrypt = require('bcrypt');

const express = require('express');
const admin = express.Router();

const connection = require('../utils/databaseConnection.js');

// Program Info
admin.get('/dashboard/program/:program', async (req, res) => {
    const program = req.params['program'].toLowerCase();
    if (program !== 'bsit' && program !== 'bscpe') {
        res.status(400).send('Invalid program');
        return;
    };

    const data = {
        programHead: '',
        courses: 0,
        faculties: 0,
        facilities: 0,
        students: 0
    };
    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM admins WHERE role = ?', [program], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else if (results.length === 0) {
                data.programHead = 'Not yet set';
            } else {
                data.programHead = `${results[0].firstName} ${results[0].lastName}`;
            };
            resolve();
        });
    });
    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM courses', (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                data.courses = results.length;
            };
            resolve();
        });
    });
    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM faculties WHERE programs = ? OR programs = ?', [program, 'both'], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                data.faculties = results.length;
            };
            resolve();
        });
    });
    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM facilities', (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                data.facilities = results.length;
            };
            resolve();
        });
    });
    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM students WHERE program = ?', [program], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                data.students = results.length;
            };
            resolve();
        });
    });

    res.send(data);
});

// Program Year Levels
admin.get('/dashboard/program/:program/yearlevels/', async (req, res) => {
    const program = req.params['program'].toLowerCase();
    if (program !== 'bsit' && program !== 'bscpe') {
        res.status(400).send('Invalid program');
        return;
    };

    const data = [];
    await new Promise((resolve, reject) => {
        connection.query('SELECT yearLevel FROM courses WHERE courseCode LIKE ?', [`${program.replace('bs', '').toUpperCase()}%`], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                for (const result of results) {
                    if (!data.includes(result.yearLevel)) {
                        data.push(result.yearLevel);
                    };
                };
            };
            resolve();
        });
    });

    data.sort((a, b) => a - b);

    const stringsToReturn = {};

    for (const yearLevel of data) {
        const postfixes = ['st', 'nd', 'rd', 'th'];
        let postfix = '';
        if (yearLevel < 4) {
            postfix = postfixes[yearLevel - 1];
        } else {
            postfix = postfixes[3];
        };
        stringsToReturn[yearLevel] = `${yearLevel}${postfix} Year`;
    };

    res.send(stringsToReturn);
});

// Program Courses
admin.get('/dashboard/program/:program/courses/:yearLevel', async (req, res) => {
    const program = req.params['program'].toLowerCase();
    if (program !== 'bsit' && program !== 'bscpe') {
        res.status(400).send('Invalid program');
        return;
    };

    const yearLevel = parseInt(req.params['yearLevel']);
    if (isNaN(yearLevel)) {
        res.status(400).send('Invalid year level');
        return;
    };

    const data = [];

    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM courses WHERE courseCode LIKE ? AND yearLevel = ?', [`${program.replace('bs', '').toUpperCase()}%`, yearLevel], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                for (const result of results) {
                    data.push(result);
                };
            };
            resolve();
        });
    });

    const toReturn = {};

    for (const course of data) {
        toReturn[course.courseCode] = course.description;
    };

    res.send(toReturn);
});

// Program Faculties
admin.get('/dashboard/program/:program/faculties/', async (req, res) => {
    const program = req.params['program'].toLowerCase();
    if (program !== 'bsit' && program !== 'bscpe') {
        res.status(400).send('Invalid program');
        return;
    };

    const data = [];

    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM faculties WHERE programs = ? OR programs = ?', [program, 'both'], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                for (const result of results) {
                    data.push(result);
                };
            };
            resolve();
        });
    });

    const toReturn = {};

    for (const faculty of data) {
        toReturn[faculty.facultyID] = `${faculty.firstName} ${faculty.lastName}`;
    };

    res.send(toReturn);
});

// Program Facilities
admin.get('/dashboard/program/:program/facilities/', async (req, res) => {
    const program = req.params['program'].toLowerCase();
    if (program !== 'bsit' && program !== 'bscpe') {
        res.status(400).send('Invalid program');
        return;
    };

    const data = [];

    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM facilities', (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                for (const result of results) {
                    data.push(result);
                };
            };
            resolve();
        });
    });

    const toReturn = {};

    for (const facility of data) {
        toReturn[facility.facilityID] = facility.name;
    };

    res.send(toReturn);
});

// Program Sections
admin.get('/dashboard/program/:program/sections/:yearLevel', async (req, res) => {
    const program = req.params['program'].toLowerCase();
    if (program !== 'bsit' && program !== 'bscpe') {
        res.status(400).send('Invalid program');
        return;
    };

    const yearLevel = parseInt(req.params['yearLevel']);
    if (isNaN(yearLevel)) {
        res.status(400).send('Invalid year level');
        return;
    };

    const data = [];

    await new Promise((resolve, reject) => {
        connection.query('SELECT section FROM students WHERE program = ?', [program], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                for (const result of results) {
                    if (!data.includes(result.section)) {
                        data.push(result.section);
                    };
                };
            };
            resolve();
        });
    });

    const filtered = data.filter(section => section.startsWith(`${yearLevel}`));

    const sorted = filtered.sort((a, b) => a.localeCompare(b));

    const toReturn = {};

    for (const section of sorted) {
        toReturn[section] = section;
    };

    res.send(toReturn);
});

// Create Schedule
admin.post('/dashboard/program/:program/schedule/', async (req, res) => {
    const program = req.params['program'].toLowerCase();
    if (program !== 'bsit' && program !== 'bscpe') {
        res.status(400).send('Invalid program');
        return;
    };

    /** @type {import('../utils/docs.js').Schedule} */
    const schedule = {
        scheduleID: '',
        courseCode: '',
        facultyID: '',
        facilityID: '',
        section: '',
        day: '',
        startTime: '',
        endTime: '',
    };

    schedule.scheduleID = `${globals.randomString(10)}`;
    schedule.courseCode = `${req.body['course'].split(' - ')[0]}`.trim();
    schedule.facultyID = `${req.body['faculty'].split(' - ')[0]}`.trim();
    schedule.facilityID = `${req.body['facility'].split(' - ')[0]}`.trim();
    schedule.day = req.body['day'].trim();
    schedule.startTime = globals.convertTime12to24(req.body['startTime'].trim());
    schedule.endTime = globals.convertTime12to24(req.body['endTime'].trim());
    schedule.section = req.body['section'].trim();

    // check for conflicts
    const conflicts = [];
    await new Promise((resolve, reject) => {
        connection.query(`
        SELECT * FROM schedules
        WHERE facultyID = '${schedule.facultyID}'
        AND day = '${schedule.day}'
        AND ((startTime < '${schedule.endTime}') AND (endTime > '${schedule.startTime}'))`,
            (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    for (const result of results) {
                        conflicts.push({
                            courseCode: result.courseCode,
                            day: result.day,
                            startTime: result.startTime,
                            endTime: result.endTime,
                            section: result.section,
                            reason: 'Faculty conflict'
                        });
                    };
                };
                resolve();
            });
    });
    await new Promise((resolve, reject) => {
        connection.query(`
        SELECT * FROM schedules
            WHERE facilityID = '${schedule.facilityID}'
            AND day = '${schedule.day}'
            AND ((startTime < '${schedule.endTime}') AND (endTime > '${schedule.startTime}'))`,
            (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    for (const result of results) {
                        conflicts.push({
                            courseCode: result.courseCode,
                            day: result.day,
                            startTime: result.startTime,
                            endTime: result.endTime,
                            section: result.section,
                            reason: 'Facility conflict'
                        });
                    };
                };
                resolve();
            });
    });
    await new Promise((resolve, reject) => {
        connection.query(`
        SELECT * FROM schedules
        WHERE section = '${schedule.section}'
        AND day = '${schedule.day}'
        AND ((startTime < '${schedule.endTime}') AND (endTime > '${schedule.startTime}'))`,
            (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    for (const result of results) {
                        conflicts.push({
                            courseCode: result.courseCode,
                            day: result.day,
                            startTime: result.startTime,
                            endTime: result.endTime,
                            section: result.section,
                            reason: 'Section conflict'
                        });
                    };
                };
                resolve();
            });
    });

    if (conflicts.length > 0) {
        // Show conflictive schedules
        res.status(409).send(`Conflicts detected with the following schedules:\n\n${conflicts.map(conflict => `${conflict.reason}:\n| ${conflict.courseCode} | ${program === 'bsit' ? 'BSIT' : 'BSCpE'} ${conflict.section} | ${conflict.day} |\n| ${conflict.startTime} - ${conflict.endTime} |`).join('\n\n')
            }`);
        return;
    } else {
        await new Promise((resolve, reject) => {
            connection.query('INSERT INTO schedules SET ?', schedule, (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                };
                resolve();
            });
        });

        // update the scheduleID of the Facility, add this scheduleID to the list
        await new Promise((resolve, reject) => {
            connection.query('SELECT schedules FROM facilities WHERE facilityID = ?', [schedule.facilityID], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    const schedules = results[0].schedules ? results[0].schedules.split(',') : [];
                    schedules.push(schedule.scheduleID);
                    connection.query('UPDATE facilities SET schedules = ? WHERE facilityID = ?', [schedules.join(','), schedule.facilityID], (err, results) => {
                        if (err) {
                            res.status(500).send('Internal Server Error');
                            reject(err);
                        };
                        resolve();
                    });
                };
            });
        });
        // update the scheduleID of the Faculty, add this scheduleID to the list
        await new Promise((resolve, reject) => {
            connection.query('SELECT schedules FROM faculties WHERE facultyID = ?', [schedule.facultyID], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    const schedules = results[0].schedules ? results[0].schedules.split(',') : [];
                    schedules.push(schedule.scheduleID);
                    connection.query('UPDATE faculties SET schedules = ? WHERE facultyID = ?', [schedules.join(','), schedule.facultyID], (err, results) => {
                        if (err) {
                            res.status(500).send('Internal Server Error');
                            reject(err);
                        };
                        resolve();
                    });
                };
            });
        });
        // update the scheduleID of the Section, add this scheduleID to the list
        await new Promise((resolve, reject) => {
            connection.query('SELECT schedules FROM students WHERE section = ?', [schedule.section], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    const schedules = results[0].schedules ? results[0].schedules.split(',') : [];
                    schedules.push(schedule.scheduleID);
                    connection.query('UPDATE students SET schedules = ? WHERE section = ?', [schedules.join(','), schedule.section], (err, results) => {
                        if (err) {
                            res.status(500).send('Internal Server Error');
                            reject(err);
                        };
                        resolve();
                    });
                };
            });
        });
    };

    res.send('Success');
});

// Get Schedule
admin.get('/dashboard/program/:program/schedule/:identifier/:key', async (req, res) => {
    const program = req.params['program'].toLowerCase();
    if (program !== 'bsit' && program !== 'bscpe') {
        res.status(400).send('Invalid program');
        return;
    };

    const identifier = req.params['identifier'];
    if (identifier !== 'facultyID' && identifier !== 'facilityID' && identifier !== 'section') {
        res.status(400).send('Invalid identifier');
        return;
    };
    const key = req.params['key'];
    if (!key) {
        res.status(400).send('Invalid key');
        return;
    };

    const data = [];

    await new Promise((resolve, reject) => {
        connection.query(`SELECT * FROM schedules WHERE ${identifier} = ?`, [key], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                for (const result of results) {
                    data.push(result);
                };
            };
            resolve();
        });
    });

    const toReturn = {};

    for (const datum of data) {
        toReturn[datum.scheduleID] = {
            course: datum.courseCode,
            faculty: datum.facultyID,
            facility: datum.facilityID,
            section: datum.section,
            day: datum.day,
            startTime: datum.startTime,
            endTime: datum.endTime,
            description: '',
            identifier: ''
        };
        if (identifier === 'facultyID') {
            toReturn[datum.scheduleID].description = program === 'bsit' ? `BSIT ${datum.section}` : `BSCpE ${datum.section}`;
            // Get Facility Name
            await new Promise((resolve, reject) => {
                connection.query('SELECT name FROM facilities WHERE facilityID = ?', [datum.facilityID], (err, results) => {
                    if (err) {
                        res.status(500).send('Internal Server Error');
                        reject(err);
                    } else {
                        toReturn[datum.scheduleID].identifier += `${results[0].name}`;
                    };
                    resolve();
                });
            });
        } else if (identifier === 'facilityID') {
            toReturn[datum.scheduleID].description = program === 'bsit' ? `BSIT ${datum.section}` : `BSCpE ${datum.section}`;
            // Get Faculty Name
            await new Promise((resolve, reject) => {
                connection.query('SELECT firstName, lastName FROM faculties WHERE facultyID = ?', [datum.facultyID], (err, results) => {
                    if (err) {
                        res.status(500).send('Internal Server Error');
                        reject(err);
                    } else {
                        toReturn[datum.scheduleID].identifier += `${results[0].firstName} ${results[0].lastName}`;
                    };
                    resolve();
                });
            });
        } else if (identifier === 'section') {
            // Get Faculty Name
            await new Promise((resolve, reject) => {
                connection.query('SELECT firstName, lastName FROM faculties WHERE facultyID = ?', [datum.facultyID], (err, results) => {
                    if (err) {
                        res.status(500).send('Internal Server Error');
                        reject(err);
                    } else {
                        toReturn[datum.scheduleID].description = `${results[0].firstName} ${results[0].lastName}`;
                    };
                    resolve();
                });
            });
            // Get Facility Name
            await new Promise((resolve, reject) => {
                connection.query('SELECT name FROM facilities WHERE facilityID = ?', [datum.facilityID], (err, results) => {
                    if (err) {
                        res.status(500).send('Internal Server Error');
                        reject(err);
                    } else {
                        toReturn[datum.scheduleID].identifier += `${results[0].name}`;
                    };
                    resolve();
                });
            });
        };
    };

    res.send(toReturn);
});

admin.get('/dashboard/program/:program/calendars/', async (req, res) => {
    const program = req.params['program'].toLowerCase();
    if (program !== 'bsit' && program !== 'bscpe') {
        res.status(400).send('Invalid program');
        return;
    };

    const data = [];

    await new Promise((resolve, reject) => {
        connection.query('SELECT section FROM schedules', (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                for (const result of results) {
                    if (!data.includes(result.section)) {
                        data.push({
                            key: result.section,
                            name: `${program === 'bsit' ? 'BSIT' : 'BSCpE'} ${result.section}`,
                            identifier: 'section'
                        });
                    };
                };
            };
            resolve();
        });
    });
    await new Promise((resolve, reject) => {
        connection.query('SELECT facilityID FROM schedules', async (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                for (const result of results) {
                    await new Promise((resolve, reject) => {
                        connection.query('SELECT name FROM facilities WHERE facilityID = ?', [result.facilityID], (err, results) => {
                            if (err) {
                                res.status(500).send('Internal Server Error');
                                reject(err);
                            } else {
                                data.push({
                                    key: result.facilityID,
                                    name: results[0].name,
                                    identifier: 'facilityID'
                                });
                            };
                            resolve();
                        });
                    });
                };
            };
            resolve();
        });
    });
    await new Promise((resolve, reject) => {
        connection.query('SELECT facultyID FROM schedules', async (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                for (const result of results) {
                    await new Promise((resolve, reject) => {
                        connection.query('SELECT firstName, lastName FROM faculties WHERE facultyID = ?', [result.facultyID], (err, results) => {
                            if (err) {
                                res.status(500).send('Internal Server Error');
                                reject(err);
                            } else {
                                data.push({
                                    key: result.facultyID,
                                    name: `${results[0].firstName} ${results[0].lastName}`,
                                    identifier: 'facultyID'
                                });
                            };
                            resolve();
                        });
                    });
                };
            };
            resolve();
        });
    });

    const toReturn = {};

    for (const datum of data) {
        toReturn[datum.name] = {
            name: datum.name,
            key: datum.key,
            identifier: datum.identifier
        };
    };

    res.send(toReturn);
});

// Update Schedule
admin.post('/dashboard/program/:program/update/schedule/:scheduleID', async (req, res) => {
    const program = req.params['program'].toLowerCase();
    if (program !== 'bsit' && program !== 'bscpe') {
        res.status(400).send('Invalid program');
        return;
    };

    const scheduleID = req.params['scheduleID'];
    if (!scheduleID) {
        res.status(400).send('Invalid schedule ID');
        return;
    };

    const schedule = {
        facultyID: '',
        facilityID: '',
        section: '',
        day: '',
        startTime: '',
        endTime: ''
    };

    schedule.facultyID = `${req.body['facultyID'].split(' - ')[0]}`.trim();
    schedule.facilityID = `${req.body['facilityID'].split(' - ')[0]}`.trim();
    schedule.section = req.body['section'].trim();
    schedule.day = req.body['day'].trim();
    schedule.startTime = globals.convertTime12to24(req.body['startTime'].trim());
    schedule.endTime = globals.convertTime12to24(req.body['endTime'].trim());



    // Check for conflicts
    const conflicts = [];
    // Faculty
    await new Promise((resolve, reject) => {
        connection.query(`
        SELECT * FROM schedules
        WHERE facultyID = '${schedule.facultyID}'
        AND day = '${schedule.day}'
        AND ((startTime < '${schedule.endTime}') AND (endTime > '${schedule.startTime}'))
        AND NOT scheduleID = '${scheduleID}'`,
            (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    for (const result of results) {
                        conflicts.push({
                            courseCode: result.courseCode,
                            day: result.day,
                            startTime: result.startTime,
                            endTime: result.endTime,
                            section: result.section,
                            scheduleID: result.scheduleID,
                            reason: 'Faculty conflict'
                        });
                    };
                };
                resolve();
            });
    });
    // Facility
    await new Promise((resolve, reject) => {
        connection.query(`
        SELECT * FROM schedules
        WHERE facilityID = '${schedule.facilityID}'
        AND day = '${schedule.day}'
        AND ((startTime < '${schedule.endTime}') AND (endTime > '${schedule.startTime}'))
        AND NOT scheduleID = '${scheduleID}'`,
            (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    for (const result of results) {
                        conflicts.push({
                            courseCode: result.courseCode,
                            day: result.day,
                            startTime: result.startTime,
                            endTime: result.endTime,
                            section: result.section,
                            scheduleID: result.scheduleID,
                            reason: 'Facility conflict'
                        });
                    };
                };
                resolve();
            });
    });
    // Section
    await new Promise((resolve, reject) => {
        connection.query(`
        SELECT * FROM schedules
        WHERE section = '${schedule.section}'
        AND day = '${schedule.day}'
        AND ((startTime < '${schedule.endTime}') AND (endTime > '${schedule.startTime}'))
        AND NOT scheduleID = '${scheduleID}'`,
            (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    for (const result of results) {
                        conflicts.push({
                            courseCode: result.courseCode,
                            day: result.day,
                            startTime: result.startTime,
                            endTime: result.endTime,
                            section: result.section,
                            scheduleID: result.scheduleID,
                            reason: 'Section conflict'
                        });
                    };
                };
                resolve();
            });
    });

    // Remove this schedule from the conflicts
    conflicts.filter(conflict => conflict.scheduleID !== scheduleID);

    if (conflicts.length > 0) {
        // Show conflictive schedules
        res.status(409).send(`Conflicts detected with the following schedules:\n\n${conflicts.map(conflict => `${conflict.reason}:\n| ${conflict.courseCode} | ${program === 'bsit' ? 'BSIT' : 'BSCpE'} ${conflict.section} | ${conflict.day} |\n| ${conflict.startTime} - ${conflict.endTime} |`).join('\n\n')
            }`);
        return;
    } else {
        await new Promise((resolve, reject) => {
            connection.query('UPDATE schedules SET ? WHERE scheduleID = ?', [schedule, scheduleID], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                };
                resolve();
            });
        });
    };

    res.send('Schedule updated');
});

// Delete Schedule
admin.post('/dashboard/program/:program/delete/schedule/:scheduleID', async (req, res) => {
    const program = req.params['program'].toLowerCase();
    if (program !== 'bsit' && program !== 'bscpe') {
        res.status(400).send('Invalid program');
        return;
    };

    const scheduleID = req.params['scheduleID'];
    if (!scheduleID) {
        res.status(400).send('Invalid schedule ID');
        return;
    };

    const data = {};

    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM schedules WHERE scheduleID = ?', [scheduleID], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                data.facultyID = results[0].facultyID;
                data.facilityID = results[0].facilityID;
                data.section = results[0].section;
            };
            resolve();
        });
    });

    await new Promise((resolve, reject) => {
        connection.query('DELETE FROM schedules WHERE scheduleID = ?', [scheduleID], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            };
            resolve();
        });
    });

    // Remove this scheduleID from the Faculty
    await new Promise((resolve, reject) => {
        connection.query('SELECT schedules FROM faculties WHERE facultyID = ?', [data.facultyID], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                const schedules = results[0].schedules.split(',');
                schedules.splice(schedules.indexOf(scheduleID), 1);
                connection.query('UPDATE faculties SET schedules = ? WHERE facultyID = ?', [schedules.join(','), data.facultyID], (err, results) => {
                    if (err) {
                        res.status(500).send('Internal Server Error');
                        reject(err);
                    };
                    resolve();
                });
            };
        });
    });

    // Remove this scheduleID from the Facility
    await new Promise((resolve, reject) => {
        connection.query('SELECT schedules FROM facilities WHERE facilityID = ?', [data.facilityID], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                const schedules = results[0].schedules.split(',');
                schedules.splice(schedules.indexOf(scheduleID), 1);
                connection.query('UPDATE facilities SET schedules = ? WHERE facilityID = ?', [schedules.join(','), data.facilityID], (err, results) => {
                    if (err) {
                        res.status(500).send('Internal Server Error');
                        reject(err);
                    };
                    resolve();
                });
            };
        });
    });

    // Remove this scheduleID from the Section
    await new Promise((resolve, reject) => {
        connection.query('SELECT schedules FROM students WHERE section = ?', [data.section], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                const schedules = results[0].schedules.split(',');
                schedules.splice(schedules.indexOf(scheduleID), 1);
                connection.query('UPDATE students SET schedules = ? WHERE section = ?', [schedules.join(','), data.section], (err, results) => {
                    if (err) {
                        res.status(500).send('Internal Server Error');
                        reject(err);
                    };
                    resolve();
                });
            };
        });
    });

    res.send('Schedule deleted');
});

module.exports = admin;