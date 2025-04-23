
function logoutUser(event) {
    event.preventDefault(); // Impede o form de ser enviado imediatamente

    const token = localStorage.getItem('Bearer_Token');

    if (!token) {
        console.error('[DEBUG] Nenhum token encontrado para logout.');
        document.getElementById('logoutForm').submit(); // Mesmo assim envia o form para limpar session
        return false;
    }

    fetch('/api/User/Logout', {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${token}`
        }
    })
    .then(response => {
        if (response.ok) {
            console.log('[DEBUG] Logout feito com sucesso.');
        } else {
            console.error('[DEBUG] Erro no logout:', response.statusText);
        }
    })
    .catch(error => {
        console.error('[DEBUG] Erro ao fazer logout:', error);
    })
    .finally(() => {
        // Sempre limpa o token e envia o form
        localStorage.removeItem('Bearer_Token');
        localStorage.removeItem('TokenExpiration');
        document.getElementById('logoutForm').submit();
    });

    return false; // Garante que o form só envie via JavaScript
}