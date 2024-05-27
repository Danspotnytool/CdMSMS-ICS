
const loginButton = document.getElementById('loginButton');

loginButton.addEventListener('click', () => {
    const user = {
        email: document.getElementById('email').value,
        password: document.getElementById('password').value
    };

    if (
        user.email === ''
        || user.password === ''
        || user.email === undefined
        || user.password === undefined
    ) {
        modal('Error', 'All fields are required', [
            {
                text: 'OK',
                onClick: (e, modal, background) => {
                    modal.remove();
                    background.remove();
                }
            }
        ]);
        return;
    };

    api('POST', 'user/login', user)
        .then(res => {
            if (res.error) {
                modal('Error', res.error, [
                    {
                        text: 'OK',
                        onClick: (e, modal, background) => {
                            modal.remove();
                            background.remove();
                        }
                    }
                ]);
            } else {
                modal('Success', 'You have successfully logged in', [
                    {
                        text: 'OK',
                        onClick: (e, modal, background) => {
                            modal.remove();
                            background.remove();
                        }
                    }
                ]);
                const user = JSON.parse(res);
                document.cookie = `token=${user.token}; path=/;`;
                document.cookie = `identifier=${user.identifier}; path=/;`;
                document.cookie = `${user.identifier}=${user[user.identifier]}; path=/;`;
                window.location.href = '/dashboard_calendar';
            };
        })
        .catch(error => {
            console.error(error);
            modal('Error', error, [
                {
                    text: 'OK',
                    onClick: (e, modal, background) => {
                        modal.remove();
                        background.remove();
                    }
                }
            ]);
        });
});