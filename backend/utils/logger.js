
const fs = require('fs');

let index = 0;

const logger = {
	colors: {
		/** @type { (str: String) => String } */
		red(str) { return `\x1b[31m${str}\x1b[0m` },

		/** @type { (str: String) => String } */
		green(str) { return `\x1b[32m${str}\x1b[0m` },

		/** @type { (str: String) => String } */
		yellow(str) { return `\x1b[33m${str}\x1b[0m` },

		/** @type { (str: String) => String } */
		blue(str) { return `\x1b[34m${str}\x1b[0m` },

		/** @type { (str: String) => String } */
		magenta(str) { return `\x1b[35m${str}\x1b[0m` },

		/** @type { (str: String) => String } */
		cyan(str) { return `\x1b[36m${str}\x1b[0m` },

		/** @type { (str: String) => String } */
		white(str) { return `\x1b[37m${str}\x1b[0m` },

		/** @type { (str: String) => String } */
		gray(str) { return `\x1b[90m${str}\x1b[0m` },

		/** @type { (str: String) => String } */
		black(str) { return `\x1b[30m${str}\x1b[0m` }
	},
	/** @type { (withColor: Boolean) => String } */
	getTimestamp(withColor) {
		const time = new Date().toLocaleTimeString('en-US', {
			timeZone: 'Asia/Manila',
			hour: 'numeric',
			hour12: true,
			minute: 'numeric',
			second: 'numeric'
		});
		return withColor ? this.colors.magenta(`${time} PHT`) : `${time} PHT`;
	},

	/** @type { (...strs: Any) => void } */
	log(...strs) {
		const timestamp = this.getTimestamp(true);
		console.log(`[${this.colors.green(index)}] ${timestamp}`, ...strs);

		index = index + 1;
	},

	/** @type { (...strs: Any) => void } */
	warn(...strs) {
		const timestamp = this.getTimestamp(true);
		console.warn(`[${this.colors.yellow(index)}] ${timestamp}`, ...strs);

		index = index + 1;
	},

	/** @type { (...strs: Any) => void } */
	error(...strs) {
		const timestamp = this.getTimestamp(true);
		console.error(`[${this.colors.red(index)}] ${timestamp}`, ...strs);

		index = index + 1;
	},

	/** @type { (...strs: Any) => void } */
	write(...strs) {
		console.log(...strs);
	}
};

module.exports = logger;