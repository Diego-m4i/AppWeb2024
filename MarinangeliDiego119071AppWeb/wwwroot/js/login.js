document.getElementById("loginForm").addEventListener("submit", async function(event) {
    event.preventDefault();

    const username = document.getElementById("Username").value;
    const password = document.getElementById("Password").value;

    const response = await fetch('/api/Account/Login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ Username: username, Password: password })
    });

    if (response.ok) {
        const data = await response.json();
        localStorage.setItem('token', data.Token); // Memorizza il token nel localStorage
        alert('Login successful!');
        window.location.href = '/'; // Redirect alla home page o a una pagina sicura
    } else {
        alert('Login failed');
    }
});
