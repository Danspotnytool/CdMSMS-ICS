
const registerButton = document.getElementById('registerButton');
registerButton.addEventListener('click', (e) => {
    e.preventDefault();
    const user = {
        firstName: document.getElementById('firstName').value,
        lastName: document.getElementById('lastName').value,
        userID: document.getElementById('userID').value,
        password: document.getElementById('password').value,
        email: document.getElementById('email').value,
        password: document.getElementById('password').value,
        confirmPassword: document.getElementById('confirmPassword').value
    };

    if (
        user.firstName === ''
        || user.lastName === ''
        || user.userID === ''
        || user.password === ''
        || user.email === ''
        || user.confirmPassword === ''
        || user.firstName === undefined
        || user.lastName === undefined
        || user.userID === undefined
        || user.password === undefined
        || user.email === undefined
        || user.confirmPassword === undefined
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
    if (user.password !== user.confirmPassword) {
        modal('Error', 'Passwords do not match', [
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
    if (user.password.length < 8) {
        modal('Error', 'Password must be at least 8 characters long', [
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
    if (/^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/.test(user.email) === false) {
        modal('Error', 'Invalid email address', [
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

    api('POST', 'user/register', user)
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
                modal('Success', 'User registered successfully', [
                    {
                        text: 'OK',
                        onClick: (e, modal, background) => {
                            modal.remove();
                            background.remove();
                        }
                    }
                ]);
                window.location.href = '/login';
            };
        })
        .catch(err => {
            modal('Error', err, [
                {
                    text: 'OK',
                    onClick: (e, modal, background) => {
                        modal.remove();
                        background.remove();
                    }
                }
            ]);
            console.error(err);
        });
});