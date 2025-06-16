import { FormUtils } from './utilities.js';

export function initStep4() {
    const container = document.getElementById('usersContainer');
    const btnAdd = document.getElementById('btnAdicionarUser');
    
    if (!container) return;

    // Adicionar primeiro usuário se não existir
    if (container.children.length === 0) {
        addUser();
    }

    btnAdd?.addEventListener('click', addUser);
    setupRemoveButtons(container);
    FormUtils.initPasswordToggle();
}

export function validateStep4() {
    const users = document.querySelectorAll('#usersContainer .user-card');
    const errorElement = document.getElementById('step4Error');
    
    if (users.length === 0) {
        FormUtils.showAlert(errorElement, 'Adicione pelo menos um usuário');
        return false;
    }

    const emails = new Set();
    for (const user of users) {
        const email = user.querySelector('[data-field="Email"]')?.value.trim();
        const username = user.querySelector('[data-field="Usuario"]')?.value.trim();
        const pass = user.querySelector('[data-field="Senha"]')?.value.trim();

        if (!email || !username || !pass) {
            FormUtils.showAlert(errorElement, 'Preencha todos os campos do usuário');
            return false;
        }

        if (!FormUtils.isValidEmail(email)) {
            FormUtils.showAlert(errorElement, `E-mail inválido: ${email}`);
            return false;
        }

        if (emails.has(email.toLowerCase())) {
            FormUtils.showAlert(errorElement, `E-mail duplicado: ${email}`);
            return false;
        }

        emails.add(email.toLowerCase());
    }

    return true;
}

export function getStep4Data() {
    return {
        Usuarios: Array.from(document.querySelectorAll('#usersContainer .user-card')).map(user => ({
            Usuario: user.querySelector('[data-field="Usuario"]').value.trim(),
            Email: user.querySelector('[data-field="Email"]').value.trim(),
            Senha: user.querySelector('[data-field="Senha"]').value.trim(),
            PermissaoConta: 'User'
        }))
    };
}

function addUser() {
    const container = document.getElementById('usersContainer');
    const template = document.getElementById('templateUser');
    
    if (!container || !template) return;

    const clone = template.content.cloneNode(true);
    container.appendChild(clone);
    setupRemoveButtons(container);
    updateUserCount();
}

function setupRemoveButtons(container) {
    container.querySelectorAll('.remover-user').forEach(btn => {
        btn.addEventListener('click', function() {
            this.closest('.user-card').remove();
            updateUserCount();
        });
    });
}

function updateUserCount() {
    const countElement = document.getElementById('userCount');
    if (countElement) {
        countElement.textContent = `Usuários: ${document.querySelectorAll('#usersContainer .user-card').length}`;
    }
}