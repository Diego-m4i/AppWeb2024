// Funzione per recuperare dati sicuri usando il token JWT
async function fetchSecureData() {
    const token = localStorage.getItem('token'); // Recupera il token dal localStorage

    const response = await fetch('/api/secure-data', {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`, // Include il token nell'header Authorization
            'Content-Type': 'application/json'
        }
    });

    if (response.ok) {
        const data = await response.json();
        console.log(data);
    } else {
        console.error('Fetch failed');
    }
}


