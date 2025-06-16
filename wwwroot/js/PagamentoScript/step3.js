import { FormUtils } from './ultilities.js';

export function initStep3() {
    const container = document.getElementById('adminsContainer');
    const btnAdd = document.getElementById('btnAdicionarAdmin');
    
    if (!container) return;

    // Adicionar primeiro admin se não existir
    if (container.children.length === 0) {
        addAdmin();
    }

    btnAdd?.addEventListener('click', addAdmin);
    setupRemoveButtons(container);
    FormUtils.initPasswordToggle();
}

export function validateStep3() {
    const admins = document.querySelectorAll('#adminsContainer .admin-card');
    const errorElement = document.getElementById('step3Error');
    
    if (admins.length === 0) {
        FormUtils.showAlert(errorElement, 'Adicione pelo menos um administrador');
        return false;
    }

    const emails = new Set();
    for (const admin of admins) {
        const email = admin.querySelector('[data-field="Email"]')?.value.trim();
        const user = admin.querySelector('[data-field="Usuario"]')?.value.trim();
        const pass = admin.querySelector('[data-field="Senha"]')?.value.trim();

        if (!email || !user || !pass) {
            FormUtils.showAlert(errorElement, 'Preencha todos os campos do administrador');
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

export function getStep3Data() {
    return {
        Administradores: Array.from(document.querySelectorAll('#adminsContainer .admin-card')).map(admin => ({
            Usuario: admin.querySelector('[data-field="Usuario"]').value.trim(),
            Email: admin.querySelector('[data-field="Email"]').value.trim(),
            Senha: admin.querySelector('[data-field="Senha"]').value.trim(),
            PermissaoConta: 'Admin'
        }))
    };
}

function addAdmin() {
    const container = document.getElementById('adminsContainer');
    const template = document.getElementById('templateAdmin');
    
    if (!container || !template) return;

    const clone = template.content.cloneNode(true);
    container.appendChild(clone);
    setupRemoveButtons(container);
    updateAdminCount();
}

function setupRemoveButtons(container) {
    container.querySelectorAll('.remover-admin').forEach(btn => {
        btn.addEventListener('click', function() {
            this.closest('.admin-card').remove();
            updateAdminCount();
        });
    });
}

function updateAdminCount() {
    const countElement = document.getElementById('adminCount');
    if (countElement) {
        countElement.textContent = `Administradores: ${document.querySelectorAll('#adminsContainer .admin-card').length}`;
    }
}