// wwwroot/js/PagamentoScript/step1.js

import { FormUtils } from './ultilities.js';

export function initStep1() {
    const emailInput = document.querySelector('[name="EmailContato"]');
    emailInput?.addEventListener('input', () => {
        const errorElement = document.getElementById('emailError');
        if (!errorElement) return;

        if (!FormUtils.isValidEmail(emailInput.value)) {
            FormUtils.showAlert(errorElement, 'E-mail inválido');
        } else {
            errorElement.classList.add('d-none');
        }
    });
}

export function validateStep1() {
    const empresa = document.querySelector('[name="EmpresaContratante"]')?.value.trim();
    const email = document.querySelector('[name="EmailContato"]')?.value.trim();
    const errorElement = document.getElementById('step1Error');

    if (!empresa || !email) {
        FormUtils.showAlert(errorElement, 'Preencha todos os campos');
        return false;
    }

    if (!FormUtils.isValidEmail(email)) {
        FormUtils.showAlert(errorElement, 'E-mail inválido');
        return false;
    }

    return true;
}