
/**
 * @type {(length: Number) => String}
 */
const randomString = (length) => {
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let result = '';
    for (let i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * characters.length));
    };
    return result;
};
/**
 * @type {(length: String | Null) => String}
 */
const randomID = (length = 10) => {
    let result = '';
    while (result.length < length) {
        let char = randomString(1);
        if (char.match(/[0-9]/)) {
            result += char;
        };
    };
    return result;
};

module.exports = {
    randomString,
    randomID
};