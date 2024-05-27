
const API_PORT = 3090;

/**
 * @type {(
 * 		method: "GET" | "POST",
 * 		path: String,
 * 		data: Object | Null
 * ) => Promise<Object>}
 */
const api = (method, path, data) => new Promise((resolve, reject) => {
	const uri = `http://localhost:${API_PORT}/api/${path}`;

	// fetch that accepts both string and object data
	const options = {
		method,
		headers: {
			'Content-Type': 'application/json'
		},
		body: data ? JSON.stringify(data) : undefined
	};

	fetch(uri, options)
		.then(async (response) => {
			if (response.ok) {
				return await response.text();
			} else {
				throw new Error(`HTTP ${response.status}\n${await response.text()}`);
			};
		})
		.then(response => {
			if (response.error) {
				reject(response.error);
			} else {
				resolve(response);
			};
		})
		.catch(error => reject(error));
});

/**
 * @type {(
 * 		title: String,
 * 		message: String,
 * 		buttons: { text: String, onClick: ( e: MouseEvent, modal: HTMLElement, background: HTMLElement ) => Void }[]
 * ) => Void}
 */
const modal = (title, message, buttons) => {
	const modal = document.createElement('div');
	modal.classList.add('modal');
	modal.innerHTML = `
<div id="header">
	<img src="/images/logo.png" alt="logo" />
	<h6>${title}</h6>
	<img src="/svg/Close Window.svg" alt="close" id="close" />
</div>

<div id="content">
	<p>${message}</p>
</div>

<div id="buttonsPanel">
</div>
`;

	document.body.appendChild(modal);

	const background = document.createElement('div');
	background.style.position = 'fixed';
	background.style.top = '0';
	background.style.left = '0';
	background.style.width = '100%';
	background.style.height = '100%';
	background.style.backgroundColor = 'rgba(0, 0, 0, 0.5)';
	document.body.appendChild(background);

	if (buttons) {
		if (buttons.length > 2) throw new Error('Only 2 buttons are allowed');

		const buttonsPanel = modal.querySelector('#buttonsPanel');
		for (const button of buttons) {
			const buttonElement = document.createElement('button');
			buttonElement.classList.add('button');
			buttonElement.innerHTML = `
<h6>
	${button.text}
</h6>
			`;
			buttonElement.addEventListener('click', e => button.onClick(e, modal, background));
			buttonsPanel.appendChild(buttonElement);
		};
	};

	const close = modal.querySelector('#close');
	close.addEventListener('click', () => {
		modal.remove();
		background.remove();
	});
	background.addEventListener('click', () => {
		modal.remove();
		background.remove();
	});
};

const getCookiesJSON = () => {
	const cookies = document.cookie.split('; ');
	const cookiesJSON = {};
	for (const cookie of cookies) {
		const [key, value] = cookie.split('=');
		cookiesJSON[key] = value;
	};
	return cookiesJSON;
};