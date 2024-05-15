require('dotenv').config();

const nodemailer = require('nodemailer');

const config = {
    host: 'smtp.gmail.com',
    port: 465,
    secure: true,
    auth: {
        user: process.env['GMAIL_ADDRESS'],
        pass: process.env['GMAIL_PASSWORD']
    }
};

const transporter = nodemailer.createTransport(config);

/**
 * @type { ( to: String, subject: String, text: String, html: String  ) => Promise<SMTPTransport.SentMessageInfo>}
 */
const send = (to, subject, text, html) => {
    return new Promise((resolve, reject) => {
        const mailOptions = {
            from: process.env['GMAIL_ADDRESS'],
            to,
            subject,
            text,
            html
        };

        transporter.sendMail(mailOptions, (err, info) => {
            if (err) {
                reject(err);
            };
            resolve(info);
        });
    });
};
module.exports = send;