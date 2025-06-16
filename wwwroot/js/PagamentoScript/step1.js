import { FormUtils } from './utilities.js';

export function initStep1() {
  const emailInput = document.querySelector('[name="EmailContato"]');
  emailInput?.addEventListener('input', () => {
    const errorElement = document.getElementById('emailError');
    FormUtils.showAlert(errorElement, 'E-mail inválido', !FormUtils.isValidEmail(emailInput.value));
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

export function getStep1Data() {
  return {
    EmpresaContratante: document.querySelector('[name="EmpresaContratante"]').value.trim(),
    EmailContato: document.querySelector('[name="EmailContato"]').value.trim()
  };
}