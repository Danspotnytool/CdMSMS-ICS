const logoutIcon = document.getElementById('logoutIcon');

logoutIcon.addEventListener('click', () => {
    // Remove all cookies
    const cookies = document.cookie.split(';');
    for (const cookie of cookies) {
        const eqPos = cookie.indexOf('=');
        const name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
        document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
    };
    window.location.href = '/login';
});

const calendar = document.getElementById('calendar');
const timeStamps = document.getElementById('timeStamps');

const displaySchedules = () => {
    // Remove all schedules
    const schedules = document.querySelectorAll('.schedule');
    for (const schedule of schedules) {
        schedule.remove();
    };
    api('POST', 'user/schedules/', getCookiesJSON())
        .then((res) => {
            /**
             * @type {{
             *      scheduleID: String,
             *      courseCode: String,
             *      facilityID: String,
             *      facultyID: String,
             *      section: String,
             *      day: 'Monday' | 'Tuesday' | 'Wednesday' | 'Thursday' | 'Friday' | 'Saturday' | 'Sunday',
             *      startTime: String,
             *      endTime: String,
             *      facultyName: String,
             *      facilityName: String
             * }[]}
             */
            const schedules = JSON.parse(res);
            const daysPanel = document.getElementById('daysPanel');
            const identifier = getCookiesJSON().identifier;

            const possibleTimes = [
                '07:00:00', '07:30:00',
                '08:00:00', '08:30:00',
                '09:00:00', '09:30:00',
                '10:00:00', '10:30:00',
                '11:00:00', '11:30:00',
                '12:00:00', '12:30:00',
                '13:00:00', '13:30:00',
                '14:00:00', '14:30:00',
                '15:00:00', '15:30:00',
                '16:00:00', '16:30:00',
                '17:00:00', '17:30:00',
                '18:00:00', '18:30:00',
                '19:00:00'
            ];

            for (const schedule of schedules) {
                const scheduleElement = document.createElement('div');
                scheduleElement.classList.add('schedule');
                scheduleElement.classList.add(schedule.day);
                scheduleElement.classList.add('odd');
                scheduleElement.id = schedule.scheduleID;
                scheduleElement.innerHTML = `
<h6>${schedule.courseCode}</h6>
<p>${identifier === 'studentID' ? schedule.facultyName : schedule.section}</p>
<p>${schedule.facilityName}</p>
`;

                // Get Top
                const startTime = schedule.startTime;
                const startTimeIndex = possibleTimes.indexOf(startTime);
                const startTimeElement = timeStamps.children[startTimeIndex];
                const top = startTimeElement.offsetTop;
                scheduleElement.style.top = `${(top / timeStamps.offsetHeight) * 100}%`;

                // Get Height
                const endTime = schedule.endTime;
                const endTimeIndex = possibleTimes.indexOf(endTime) - 1;
                const endTimeElement = timeStamps.children[endTimeIndex];
                const bottom = endTimeElement.offsetTop + endTimeElement.offsetHeight;
                const height = bottom - top;
                scheduleElement.style.height = `${(height / timeStamps.offsetHeight) * 100}%`;

                Array.from(daysPanel.children).find(day => day.id === schedule.day).appendChild(scheduleElement);

                scheduleElement.addEventListener('click', async () => {
                    if (identifier === 'studentID') return;

                    for (const requestForm of document.querySelectorAll('.requestForm')) {
                        requestForm.remove();
                    };

                    const requestForm = document.createElement('form');
                    requestForm.classList.add('requestForm');
                    requestForm.classList.add(schedule.day);
                    requestForm.classList.add(scheduleElement.classList.contains('even') ? 'even' : 'odd');
                    requestForm.style.top = scheduleElement.style.top;

                    requestForm.innerHTML = `
<h6>Request Changes</h6>

<div class="dropDown">
    <label for="facilityDropDown">
        <b>Facility</b>
    </label>
    <select id="facilityDropDown" name="facility" required>
    </select>
</div>

<div class="dropDown">
    <label for="dayDropDown">
        <b>Day</b>
    </label>
    <select id="dayDropDown" name="day" required>
        <option value="Sunday" ${schedule.day === 'Sunday' ? 'selected' : ''}>Sunday</option>
        <option value="Monday" ${schedule.day === 'Monday' ? 'selected' : ''}>Monday</option>
        <option value="Tuesday" ${schedule.day === 'Tuesday' ? 'selected' : ''}>Tuesday</option>
        <option value="Wednesday" ${schedule.day === 'Wednesday' ? 'selected' : ''}>Wednesday</option>
        <option value="Thursday" ${schedule.day === 'Thursday' ? 'selected' : ''}>Thursday</option>
        <option value="Friday" ${schedule.day === 'Friday' ? 'selected' : ''}>Friday</option>
        <option value="Saturday" ${schedule.day === 'Saturday' ? 'selected' : ''}>Saturday</option>
    </select>
</div>

<div class="dropDown">
    <label for="startTimeDropDown">
        <b>Start Time</b>
    </label>
    <select id="startTimeDropDown" name="startTime" required>
        <option value="07:00 AM">07:00 AM</option>
        <option value="07:30 AM">07:30 AM</option>
        <option value="08:00 AM">08:00 AM</option>
        <option value="08:30 AM">08:30 AM</option>
        <option value="09:00 AM">09:00 AM</option>
        <option value="09:30 AM">09:30 AM</option>
        <option value="10:00 AM">10:00 AM</option>
        <option value="10:30 AM">10:30 AM</option>
        <option value="11:00 AM">11:00 AM</option>
        <option value="11:30 AM">11:30 AM</option>
        <option value="12:00 PM">12:00 PM</option>
        <option value="12:30 PM">12:30 PM</option>
        <option value="01:00 PM">01:00 PM</option>
        <option value="01:30 PM">01:30 PM</option>
        <option value="02:00 PM">02:00 PM</option>
        <option value="02:30 PM">02:30 PM</option>
        <option value="03:00 PM">03:00 PM</option>
        <option value="03:30 PM">03:30 PM</option>
        <option value="04:00 PM">04:00 PM</option>
        <option value="04:30 PM">04:30 PM</option>
        <option value="05:00 PM">05:00 PM</option>
        <option value="05:30 PM">05:30 PM</option>
        <option value="06:00 PM">06:00 PM</option>
        <option value="06:30 PM">06:30 PM</option>
        <option value="07:00 PM">07:00 PM</option>
    </select>
</div>

<div class="dropDown">
    <label for="endTimeDropDown">
        <b>End Time</b>
    </label>
    <select id="endTimeDropDown" name="endTime" required>
    </select>
</div>

<div class="dropDown">
    <label for="reason">
        <b>Reason</b>
    </label>
    <textarea id="reason" name="reason" required></textarea>
</div>

<button class="button" id="submitButton">
    <h6>
        Submit
    </h6>
</button>

<button class="button" id="cancelButton">
    <h6>
        Cancel
    </h6>
</button>
`;

                    const facilities = [];

                    await new Promise((resolve, reject) =>
                        api('POST', 'user/facilities/', getCookiesJSON())
                            .then(res => {
                                const result = JSON.parse(res);
                                for (const facility of result) {
                                    facilities.push(facility);
                                };
                                resolve();
                            })
                            .catch(reject)
                    );

                    const facilityDropDown = requestForm.querySelector('#facilityDropDown');
                    for (const facility of facilities) {
                        const option = document.createElement('option');
                        option.value = facility.facilityID;
                        option.innerText = facility.name;
                        facilityDropDown.appendChild(option);
                    };
                    facilityDropDown.value = schedule.facilityID;

                    const startTimeDropDown = requestForm.querySelector('#startTimeDropDown');

                    const changeListener = () => {
                        // Add to the endTimesDropDown the remaining times starting from the selected startTime
                        const endTimeDropDown = requestForm.querySelector('#endTimeDropDown');
                        endTimeDropDown.innerHTML = '';
                        const selectedStartTime = startTimeDropDown.value;
                        const startTimeItems = Array.from(startTimeDropDown.children);
                        const selectedStartTimeIndex = startTimeItems.findIndex(item => item.value === selectedStartTime);
                        const remainingTimes = startTimeItems.slice(selectedStartTimeIndex + 1);
                        for (const remainingTime of remainingTimes) {
                            endTimeDropDown.appendChild(remainingTime.cloneNode(true));
                        };
                    };
                    startTimeDropDown.addEventListener('change', changeListener);

                    // Convert schedule's start time to 12-hour format
                    const startTime12Hour = new Date(`1970-01-01T${schedule.startTime}`).toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' });
                    startTimeDropDown.value = startTime12Hour;

                    changeListener();

                    // Select the schedule's end time
                    const endTimeDropDown = requestForm.querySelector('#endTimeDropDown');
                    const endTime12Hour = new Date(`1970-01-01T${schedule.endTime}`).toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' });
                    endTimeDropDown.value = endTime12Hour;

                    const cancelButton = requestForm.querySelector('#cancelButton');
                    cancelButton.addEventListener('click', () => {
                        requestForm.remove();
                    });

                    if (schedule.day === 'Sunday')
                        daysPanel.children[1].appendChild(requestForm);
                    else if (schedule.day === 'Monday')
                        daysPanel.children[2].appendChild(requestForm);
                    else if (schedule.day === 'Tuesday')
                        daysPanel.children[3].appendChild(requestForm);
                    else if (schedule.day === 'Wednesday')
                        daysPanel.children[4].appendChild(requestForm);
                    else if (schedule.day === 'Thursday')
                        daysPanel.children[5].appendChild(requestForm);
                    else if (schedule.day === 'Friday')
                        daysPanel.children[6].appendChild(requestForm);
                    else if (schedule.day === 'Saturday')
                        daysPanel.children[5].appendChild(requestForm);
                });
            };

            // Sort scheduleElements per day
            for (const day of daysPanel.children) {
                const scheduleElements = Array.from(day.children).sort((a, b) => {
                    const aTop = a.offsetTop;
                    const bTop = b.offsetTop;
                    return aTop - bTop;
                });
                for (const scheduleElement of scheduleElements) {
                    day.appendChild(scheduleElement);
                };
            };

            // Odd and Even
            for (const day of daysPanel.children) {
                const scheduleElements = Array.from(day.children);
                for (let i = 0; i < scheduleElements.length; i++) {
                    if (i % 2 === 0) {
                        scheduleElements[i].classList.remove('even');
                        scheduleElements[i].classList.add('odd');
                    } else {
                        scheduleElements[i].classList.remove('odd');
                        scheduleElements[i].classList.add('even');
                    };
                };
            };
        })
        .catch(err => modal('Error', err.message, [{ text: 'OK', onClick: (e, modal, background) => { modal.remove(); background.remove(); } }]));
};

window.addEventListener('load', () => {
    displaySchedules();
    api('POST', 'user/identifierHeader/', getCookiesJSON())
        .then(res => {
            const calendarHeader = document.getElementById('calendarHeader');
            calendarHeader.innerText = res;
        })
        .catch(err => console.error(err));

    if (getCookiesJSON().identifier === 'facultyID') {
        const notificationIcon = document.getElementById('notificationIcon');
        notificationIcon.style.display = 'block';
    };
});