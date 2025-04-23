    fetch('/api/User/GetUserBearerToken', {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('Bearer_Token')}`
        }
    })
    .then(response => {
        
        if (!response.ok) {
            throw new Error('Falha na requisição');
        }
        return response.json();
    })
    .then(data => {

        const navBarInfo = data[0]; 
        
        if (navBarInfo.permissionAccount && navBarInfo.permissionAccount.trim().toLowerCase() === 'admin') {
            document.getElementById("dashboardButton").style.display = "block";
        } else {
            document.getElementById("dashboardButton").style.display = "none";
        }
    })
    .catch(error => {
        console.error('[DEBUG] Erro ao fazer requisição GET:', error);
    });
