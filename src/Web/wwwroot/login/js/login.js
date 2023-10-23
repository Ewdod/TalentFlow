var button = document.getElementById('mainButton');

var openForm = function () {
    button.classList.add('active');
};

var checkInput = function (input) {
    if (input.value.length > 0) {
        input.classList.add('active');
    } else {
        input.classList.remove('active');
    }
};

var closeForm = function () {
    button.classList.remove('active');
};

document.addEventListener("key", function (e) {
    if (e.key === 'Escape' || e.key === 'Enter') {
        closeForm();
    }
});

function togglePasswordVisibility() {
    var passwordField = document.getElementById("password");
    var toggleButton = document.getElementById("togglePassword");

    if (passwordField.type === "password") {
        passwordField.type = "text";
        toggleButton.querySelector('i').classList.remove('fa-eye');
        toggleButton.querySelector('i').classList.add('fa-eye-slash');
    } else {
        passwordField.type = "password";
        toggleButton.querySelector('i').classList.remove('fa-eye-slash');
        toggleButton.querySelector('i').classList.add('fa-eye');
    }
}