
const emailInput = document.getElementById('emailInput');
const codeInput = document.getElementById('codeInput');
const newPasswordInput = document.getElementById('newPasswordInput');

const submitRequestButton = document.getElementById('submitRequestButton');
const submitCodeButton = document.getElementById('submitCodeButton');
const submitNewPasswordButton = document.getElementById('submitNewPasswordButton');

const data = {
    email: '',
    forgotPasswordCode: '',
    password: ''
};

submitRequestButton.addEventListener('click', () => {
    const email = document.getElementById('email').value;

    if (!email) {
        modal('Error', 'Please enter your email', [
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

    api('POST', 'user/forgotPassword', { email })
        .then(response => {
            data.email = email;
            modal('Success', response, [
                {
                    text: 'OK',
                    onClick: (e, modal, background) => {
                        modal.remove();
                        background.remove();
                    }
                }
            ]);
            emailInput.style.display = 'none';
            codeInput.style.display = 'flex';
        })
        .catch(error => {
            console.error(error);
            modal('Error', 'An error occurred', [
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

submitCodeButton.addEventListener('click', () => {
    const code = document.getElementById('code').value;

    if (!code) {
        modal('Error', 'Please enter the code', [
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

    api('POST', 'user/resetPassword', { email: data.email, forgotPasswordCode: code })
        .then(response => {
            data.forgotPasswordCode = code;
            modal('Success', response, [
                {
                    text: 'OK',
                    onClick: (e, modal, background) => {
                        modal.remove();
                        background.remove();
                    }
                }
            ]);
            codeInput.style.display = 'none';
            newPasswordInput.style.display = 'flex';
        })
        .catch(error => {
            console.error(error);
            modal('Error', 'An error occurred', [
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

submitNewPasswordButton.addEventListener('click', () => {
    const newPassword = document.getElementById('newPassword').value;

    data.password = newPassword;

    api('POST', 'user/changePassword', data)
        .then(response => {
            modal('Success', response, [
                {
                    text: 'OK',
                    onClick: (e, modal, background) => {
                        modal.remove();
                        background.remove();
                        window.location.href = '/login';
                    }
                }
            ]);
        })
        .catch(error => {
            console.error(error);
            modal('Error', 'An error occurred', [
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