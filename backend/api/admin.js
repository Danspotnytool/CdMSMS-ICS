
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

    for (const ws of globals.rooms['calendar_admin']) {
        ws.send("refresh");
    };
    for (const ws of globals.rooms['calendar_user']) {
        ws.send("refresh");
    };
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

// Get Calendar
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

    for (const ws of globals.rooms['calendar_admin']) {
        ws.send("refresh");
    };
    for (const ws of globals.rooms['calendar_user']) {
        ws.send("refresh");
    };
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

    for (const ws of globals.rooms['calendar_admin']) {
        ws.send("refresh");
    };
    for (const ws of globals.rooms['calendar_user']) {
        ws.send("refresh");
    };
});

// Get Requests
admin.get('/dashboard/program/:program/requests/', async (req, res) => {
    const program = req.params['program'].toLowerCase();
    if (program !== 'bsit' && program !== 'bscpe') {
        res.status(400).send('Invalid program');
        return;
    };

    const data = [];

    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM requests WHERE program = ?', [program], (err, results) => {
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

    // Sort by date
    data.sort((a, b) => new Date(b.requestDate) - new Date(a.requestDate));

    const toReturn = {};

    for (const datum of data) {
        const request = {
            requestID: datum.requestID,
            facultyID: datum.facultyID,
            facultyName: '',
            original: {
                facilityID: datum.original_facilityID,
                facilityName: '',
                day: datum.original_day,
                startTime: datum.original_startTime,
                endTime: datum.original_endTime
            },
            request: {
                facilityID: datum.request_facilityID,
                facilityName: '',
                day: datum.request_day,
                startTime: datum.request_startTime,
                endTime: datum.request_endTime
            },
            status: datum.status,
            requestReason: datum.requestReason,
            rejectReason: datum.rejectReason,
            requestDate: datum.requestDate,
            program: datum.program,
            courseCode: datum.courseCode,
            scheduleID: datum.scheduleID
        };

        // Get faculty name
        await new Promise((resolve, reject) => {
            connection.query('SELECT firstName, lastName FROM faculties WHERE facultyID = ?', [datum.facultyID], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    request.facultyName = `${results[0].firstName} ${results[0].lastName}`;
                };
                resolve();
            });
        });
        // Get original facility name
        await new Promise((resolve, reject) => {
            connection.query('SELECT name FROM facilities WHERE facilityID = ?', [request.original.facilityID], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    request.original.facilityName = results[0].name;
                };
                resolve();
            });
        });
        // Get request facility name
        await new Promise((resolve, reject) => {
            connection.query('SELECT name FROM facilities WHERE facilityID = ?', [request.request.facilityID], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                } else {
                    request.request.facilityName = results[0].name;
                };
                resolve();
            });
        });
        toReturn[datum.requestID] = request;
    };

    res.send(toReturn);
});

// Reject Request
admin.post('/dashboard/program/:program/requests/:requestID/reject/', async (req, res) => {
    const program = req.params['program'].toLowerCase();
    const requestID = req.params['requestID'];

    const rejectReason = req.body['rejectReason'];

    if (!rejectReason) {
        res.status(400).send('Invalid reject reason');
        return;
    };

    await new Promise((resolve, reject) => {
        connection.query('UPDATE requests SET status = ?, rejectReason = ? WHERE requestID = ?', ['rejected', rejectReason, requestID], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            };
            resolve();
        });
    });

    res.send('Request rejected');
    for (const ws of globals.rooms['notifications_admin']) {
        ws.send("refresh");
    };
    for (const ws of globals.rooms['notifications_user']) {
        ws.send("refresh");
    };
});

// Approve Request
admin.post('/dashboard/program/:program/requests/:requestID/approve/', async (req, res) => {
    const program = req.params['program'].toLowerCase();
    const requestID = req.params['requestID'];

    // Get the request
    const request = {
        requestID: '',
        facultyID: '',
        facultyName: '',
        original: {
            facilityID: '',
            facilityName: '',
            day: '',
            startTime: '',
            endTime: ''
        },
        request: {
            facilityID: '',
            facilityName: '',
            day: '',
            startTime: '',
            endTime: ''
        },
        status: '',
        requestReason: '',
        rejectReason: '',
        requestDate: '',
        program: '',
        courseCode: '',
        scheduleID: ''
    };

    await new Promise((resolve, reject) => {
        connection.query('SELECT * FROM requests WHERE requestID = ?', [requestID], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                request.requestID = results[0].requestID;
                request.facultyID = results[0].facultyID;
                request.original.facilityID = results[0].original_facilityID;
                request.original.day = results[0].original_day;
                request.original.startTime = results[0].original_startTime;
                request.original.endTime = results[0].original_endTime;
                request.request.facilityID = results[0].request_facilityID;
                request.request.day = results[0].request_day;
                request.request.startTime = results[0].request_startTime;
                request.request.endTime = results[0].request_endTime;
                request.status = results[0].status;
                request.requestReason = results[0].requestReason;
                request.rejectReason = results[0].rejectReason;
                request.requestDate = results[0].requestDate;
                request.program = results[0].program;
                request.courseCode = results[0].courseCode;
                request.scheduleID = results[0].scheduleID;
            };
            resolve();
        });
    });

    // Get section
    let section = '';
    await new Promise((resolve, reject) => {
        connection.query('SELECT section FROM schedules WHERE scheduleID = ?', [request.scheduleID], (err, results) => {
            if (err) {
                res.status(500).send('Internal Server Error');
                reject(err);
            } else {
                section = results[0].section;
            };
            resolve();
        });
    });

    // Check for conflicts
    let conflicts = [];
    // Faculty
    await new Promise((resolve, reject) => {
        connection.query(`
        SELECT * FROM schedules
        WHERE facultyID = '${request.facultyID}'
        AND day = '${request.request.day}'
        AND ((startTime < '${request.request.endTime}') AND (endTime > '${request.request.startTime}'))`,
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
        WHERE facilityID = '${request.request.facilityID}'
        AND day = '${request.request.day}'
        AND ((startTime < '${request.request.endTime}') AND (endTime > '${request.request.startTime}'))`,
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
        WHERE section = '${section}'
        AND day = '${request.request.day}'
        AND ((startTime < '${request.request.endTime}') AND (endTime > '${request.request.startTime}'))`,
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
    conflicts = conflicts.filter(conflict => conflict.scheduleID !== request.scheduleID);

    if (conflicts.length > 0) {
        // Show conflictive schedules
        res.status(409).send(`Conflicts detected with the following schedules:\n\n${conflicts.map(conflict => `${conflict.reason}:\n| ${conflict.courseCode} | ${program === 'bsit' ? 'BSIT' : 'BSCpE'} ${conflict.section} | ${conflict.day} |\n| ${conflict.startTime} - ${conflict.endTime} |`).join('\n\n')
            }`);
        return;
    } else {
        // Update the schedule
        await new Promise((resolve, reject) => {
            connection.query('UPDATE schedules SET facultyID = ?, facilityID = ?, day = ?, startTime = ?, endTime = ? WHERE scheduleID = ?', [request.facultyID, request.request.facilityID, request.request.day, request.request.startTime, request.request.endTime, request.scheduleID], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                };
                resolve();
            });
        });

        // Update the request status
        await new Promise((resolve, reject) => {
            connection.query('UPDATE requests SET status = ? WHERE requestID = ?', ['approved', requestID], (err, results) => {
                if (err) {
                    res.status(500).send('Internal Server Error');
                    reject(err);
                };
                resolve();
            });
        });
    };

    res.send('Request approved');

    for (const ws of globals.rooms['calendar_admin']) {
        ws.send("refresh");
    };
    for (const ws of globals.rooms['calendar_user']) {
        ws.send("refresh");
    };
    for (const ws of globals.rooms['notifications_admin']) {
        ws.send("refresh");
    };
    for (const ws of globals.rooms['notifications_user']) {
        ws.send("refresh");
    };
});

module.exports = admin;