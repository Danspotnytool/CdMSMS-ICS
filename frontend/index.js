
const fs = require('fs');
const path = require('path');
const port = parseInt(
    fs.readFileSync(
        path.join(__dirname, '../backend/.env'), 'utf8'
    ).split('\n').find((line) => line.startsWith('PORT')).split('=')[1].replace('"', "")
) + 1;

const phin = require('phin');
const cookieParser = require('cookie-parser');

const express = require('express');
const app = express();

app.use(express.static(path.join(__dirname)));
app.use(express.static(path.join(__dirname, 'js')));
app.use(express.static(path.join(__dirname, 'css')));
app.use(express.static(path.join(__dirname, 'svg')));
app.use(express.static(path.join(__dirname, 'images')));

app.use(cookieParser());

app.get('/', (req, res) => {
    if (req.headers.cookie) {
        const cookie = req.cookies;
        if (cookie.token && cookie.identifier && cookie[cookie.identifier]) {
            phin({
                url: `http://localhost:${port - 1}/api/user/check`,
                method: 'POST',
                data: JSON.parse(`{
                    "token": "${cookie.token}",
                    "identifier": "${cookie.identifier}",
                    "${cookie.identifier}": "${cookie[cookie.identifier]}"
                }`),
                parse: 'json'
            })
                .then(response => {
                    if (response.body.error) {
                        res.redirect('/register');
                    } else {
                        res.redirect('/dashboard_calendar');
                    };
                })
                .catch(error => {
                    console.error(error);
                    res.redirect('/register');
                });
        } else {
            res.redirect('/register');
        };
    } else {
        res.redirect('/register');
    };
});

app.get('/login', (req, res) => {
    if (req.headers.cookie) {
        const cookie = req.cookies;
        if (cookie.token && cookie.identifier && cookie[cookie.identifier]) {
            phin({
                url: `http://localhost:${port - 1}/api/user/check`,
                method: 'POST',
                data: JSON.parse(`{
                    "token": "${cookie.token}",
                    "identifier": "${cookie.identifier}",
                    "${cookie.identifier}": "${cookie[cookie.identifier]}"
                }`),
                parse: 'json'
            })
                .then(response => {
                    if (response.body.error) {
                        res.sendFile(path.join(__dirname, 'login.html'));
                    } else {
                        res.redirect('/dashboard_calendar');
                    };
                })
                .catch(error => {
                    console.error(error);
                    res.sendFile(path.join(__dirname, 'login.html'));
                });
        } else {
            res.sendFile(path.join(__dirname, 'login.html'));
        };
    } else {
        res.sendFile(path.join(__dirname, 'login.html'));
    };
});

app.get('/register', (req, res) => {
    if (req.headers.cookie) {
        const cookie = req.cookies;
        if (cookie.token && cookie.identifier && cookie[cookie.identifier]) {
            phin({
                url: `http://localhost:${port - 1}/api/user/check`,
                method: 'POST',
                data: JSON.parse(`{
                    "token": "${cookie.token}",
                    "identifier": "${cookie.identifier}",
                    "${cookie.identifier}": "${cookie[cookie.identifier]}"
                }`),
                parse: 'json'
            })
                .then(response => {
                    if (response.body.error) {
                        res.sendFile(path.join(__dirname, 'register.html'));
                    } else {
                        res.redirect('/dashboard_calendar');
                    };
                })
                .catch(error => {
                    console.error(error);
                    res.sendFile(path.join(__dirname, 'register.html'));
                });
        } else {
            res.sendFile(path.join(__dirname, 'register.html'));
        };
    } else {
        res.sendFile(path.join(__dirname, 'register.html'));
    };
});

app.get('/forgotPassword', (req, res) => {
    if (req.headers.cookie) {
        const cookie = req.cookies;
        if (cookie.token && cookie.identifier && cookie[cookie.identifier]) {
            phin({
                url: `http://localhost:${port - 1}/api/user/check`,
                method: 'POST',
                data: JSON.parse(`{
                    "token": "${cookie.token}",
                    "identifier": "${cookie.identifier}",
                    "${cookie.identifier}": "${cookie[cookie.identifier]}"
                }`),
                parse: 'json'
            })
                .then(response => {
                    if (response.body.error) {
                        res.sendFile(path.join(__dirname, 'forgotPassword.html'));
                    } else {
                        res.redirect('/dashboard_calendar');
                    };
                })
                .catch(error => {
                    console.error(error);
                    res.sendFile(path.join(__dirname, 'forgotPassword.html'));
                });
        } else {
            res.sendFile(path.join(__dirname, 'forgotPassword.html'));
        };
    } else {
        res.sendFile(path.join(__dirname, 'forgotPassword.html'));
    };
});

app.get('/dashboard_calendar', (req, res) => {
    if (!req.headers.cookie) {
        res.redirect('/login');
        return;
    };
    const cookie = req.cookies;

    if (!cookie.token || !cookie.identifier || !cookie[cookie.identifier]) {
        res.redirect('/login');
        return;
    };

    phin({
        url: `http://localhost:${port - 1}/api/user/check`,
        method: 'POST',
        data: JSON.parse(`{
            "token": "${cookie.token}",
            "identifier": "${cookie.identifier}",
            "${cookie.identifier}": "${cookie[cookie.identifier]}"
        }`),
        parse: 'json'
    })
        .then(response => {
            if (response.body.error) {
                res.redirect('/login');
                return;
            };
            res.sendFile(path.join(__dirname, 'dashboard_calendar.html'));
        })
        .catch(error => {
            console.error(error);
            res.redirect('/login');
        });
});

app.get('/dashboard_notification', (req, res) => {
    if (!req.headers.cookie) {
        res.redirect('/login');
        return;
    };
    const cookie = req.cookies;

    if (!cookie.token || !cookie.identifier || !cookie[cookie.identifier]) {
        res.redirect('/login');
        return;
    };

    phin({
        url: `http://localhost:${port - 1}/api/user/check`,
        method: 'POST',
        data: JSON.parse(`{
            "token": "${cookie.token}",
            "identifier": "${cookie.identifier}",
            "${cookie.identifier}": "${cookie[cookie.identifier]}"
        }`),
        parse: 'json'
    })
        .then(response => {
            if (response.body.error) {
                res.redirect('/login');
                return;
            };
            res.sendFile(path.join(__dirname, 'dashboard_notification.html'));
        })
        .catch(error => {
            console.error(error);
            res.redirect('/login');
        });
});

app.listen(port, () => {
    console.log(`Server is running on port ${port}`);
});