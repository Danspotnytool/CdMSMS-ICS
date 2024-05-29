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

const calendarIcon = document.getElementById('calendarIcon');
calendarIcon.addEventListener('click', () => {
    window.location.href = '/dashboard_calendar';
});


const requestsPanel = document.getElementById('requests');
const displayNotifications = () => {
    requestsPanel.innerHTML = '';
    api('POST', 'user/requests', getCookiesJSON())
        .then(response => {
            /**
             * @type {{
             *      requestID: String,
             *      facultyID: String,
             *      facultyName: String,
             *      original: {
             *          facilityID: String,
             *          facilityName: String,
             *          day: 'Sunday' | 'Monday' | 'Tuesday' | 'Wednesday' | 'Thursday' | 'Friday' | 'Saturday,
             *          startTime: String,
             *          endTime: String
             *      },
             *      request: {
             *          facilityID: String,
             *          facilityName: String,
             *          day: 'Sunday' | 'Monday' | 'Tuesday' | 'Wednesday' | 'Thursday' | 'Friday' | 'Saturday,
             *          startTime: String,
             *          endTime: String
             *      },
             *      status: 'pending' | 'rejected' | 'approved',
             *      requestReason: String,
             *      rejectReason: String | Null,
             *      requestDate: String,
             *      program: 'bsit' | 'bscpe',
             *      courseCode: String,
             *      scheduleID: String
             *  }[]}
             */
            const requests = JSON.parse(response);

            for (const request of requests) {
                const requestElement = document.createElement('table');
                requestElement.classList.add('request');
                requestElement.innerHTML = `
<caption>
    <h5>${request.courseCode}</h5>
</caption>
<thead>
    <th>
        <h6>Original</h6>
    </th>
    <th>
        <h6>Request</h6>
    </th>
    <th>
        <h6>Status</h6>
    </th>
</thead>
<tbody>
    <tr>
        <td>
            <div><b>Facility:</b> <span>${request.original.facilityName}</span></div>
            <div><b>Day:</b> <span>${request.original.day}</span></div>
            <div><b>Time:</b> <span>${request.original.startTime} - ${request.original.endTime}</span></div>
        </td>
        <td>
            <div><b>Facility:</b> <span>${request.request.facilityName}</span></div>
            <div><b>Day:</b> <span>${request.request.day}</span></div>
            <div><b>Time:</b> <span>${request.request.startTime} - ${request.request.endTime}</span></div>
        </td>
        <td>
            <div><b>Status:</b> <span>${request.status.charAt(0).toUpperCase() + request.status.slice(1)}</span></div>
            <div><b>${request.status === 'rejected' ? 'Rejection Reason' : 'Reason'}:</b> <span>${request.status === 'rejected' ? request.rejectReason : request.requestReason}</span></div>
            <div><b>Created:</b> <span>${new Date(request.requestDate).toLocaleString().split(',')[0]}</span></div>
        </td>
    </tr>
</tbody>
`;

                requestsPanel.appendChild(requestElement);
            };
        })
        .catch(error => {
            console.error(error);
        });
};

window.addEventListener('load', () => {
    displayNotifications();

    const ws = new WebSocket(`ws://${window.location.host}/`.replace(window.location.port, parseInt(window.location.port) - 1));
    ws.onopen = () => {
        ws.send(JSON.stringify({ type: 'join', room: 'notifications_user' }));
    };
    ws.onmessage = () => {
        displayNotifications();
    };
});