// step1.js - Step 1: Informações da Empresa
import { FormUtils } from './utilities.js';

export function initStep1(data) {
  if (!data) return;

  const empresaEl = document.getElementById('EmpresaContratante_EmpresaContratante');
  const emailEl = document.getElementById('EmpresaContratante_Email');

  if (empresaEl && data.EmpresaContratante) {
    empresaEl.value = data.EmpresaContratante;
  }
  if (emailEl && data.EmailEmpresa) {
    emailEl.value = data.EmailEmpresa;
  }

  // Configura validação em tempo real
  emailEl?.addEventListener('input', () => {
    if (!FormUtils.isValidEmail(emailEl.value.trim())) {
      document.getElementById('emailError').classList.remove('d-none');
    } else {
      document.getElementById('emailError').classList.add('d-none');
    }
  });
}

export function validateStep1() {
  const empresa = document.getElementById('EmpresaContratante_EmpresaContratante')?.value.trim() || '';
  const email = document.getElementById('EmpresaContratante_Email')?.value.trim() || '';
  const emailError = document.getElementById('emailError');

  FormUtils.clearAlerts('step1Error', 'step1Success');

  if (!empresa || !email) {
    FormUtils.showAlert('step1Error', 'Preencha todos os campos obrigatórios.');
    return false;
  }

  if (!FormUtils.isValidEmail(email)) {
    emailError.classList.remove('d-none');
    FormUtils.showAlert('step1Error', 'Formato de e-mail inválido.');
    return false;
  }

  emailError.classList.add('d-none');
  FormUtils.showAlert('step1Success', 'Informações válidas!', 'success');
  return true;
}

export function getStep1Data() {
  return {
    EmpresaContratante: document.getElementById('EmpresaContratante_EmpresaContratante').value.trim(),
    EmailEmpresa: document.getElementById('EmpresaContratante_Email').value.trim()
  };
}