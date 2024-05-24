
/**
 * @typedef {{
 *		adminID: String,
 *		firstName: String,
 *		lastName: String,
 *		email: String,
 *		password: String,
 *		role: String,
 *		verificationID: String,
 *		verified: Boolean,
 *		forgotPasswordCode: String,
 *		token: String
 * }} Admin
 */

/**
 * @typedef {{
 * 		courseCode: String,
 * 		description: String,
 * 		yearLevel: Number,
 * 		units: Number
 * }} Course
 */

/**
 * @typedef {{
 * 		facultyID: String,
 * 		firstName: String,
 * 		lastName: String,
 * 		email: String,
 * 		password: String,
 * 		schedules: String,
 * 		programs: String,
 * 		verificationID: String,
 * 		verified: Boolean,
 * 		forgotPasswordCode: String,
 * 		token: String
 * }} Faculty
 */

/**
 * @typedef {{
 * 		facilityID: String,
 * 		name: String,
 * 		description: String,
 * 		schedules: String
 * }} Facility
 */

/**
 * @typedef {{
 * 		studentID: String,
 * 		firstName: String,
 * 		lastName: String,
 * 		email: String,
 * 		password: String,
 * 		program: String,
 * 		schedules: String,
 * 		section: String,
 * 		verificationID: String,
 * 		verified: Boolean,
 * 		forgotPasswordCode: String,
 * 		token: String
 * }} Student
 */

/**
 * @typedef {{
 * 		scheduleID: String,
 * 		courseCode: String,
 * 		facultyID: String,
 * 		facilityID: String,
 * 		day: 'Sunday' | 'Monday' | 'Tuesday' | 'Wednesday' | 'Thursday' | 'Friday' | 'Saturday',
 * 		startTime: String,
 * 		endTime: String,
 *      section: String
 * }} Schedule
 */

/**
 * @typedef {{
 * 		requestID: String,
 * 		facultyID: String,
 * 		facilityID: String,
 * 		day: 'Sunday' | 'Monday' | 'Tuesday' | 'Wednesday' | 'Thursday' | 'Friday' | 'Saturday',
 * 		startTime: String,
 * 		endTime: String,
 * 		status: String
 * }} Request
 */

module.exports = {};