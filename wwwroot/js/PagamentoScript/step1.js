import { isValidEmail } from './ultilities.js';

export function initStep1(data) {
  if (!data) return;

  const empresaEl = document.getElementById('EmpresaContratante_EmpresaContratante');
  const emailEl = document.getElementById('EmpresaContratante_Email');
  const id = sessionStorage.getItem('Pagamento_Id');
  const nomePlano = sessionStorage.getItem('Pagamento_NomePlano');
  const periodicidade = sessionStorage.getItem('Pagamento_Periodicidade');

  // Preenche campos visíveis (se existirem)
  if (empresaEl && data.EmpresaContratante) {
    empresaEl.value = data.EmpresaContratante;
  }
  if (emailEl && data.EmailEmpresa) {
    emailEl.value = data.EmailEmpresa;
  }

  // Atualiza o objeto `data` com os dados do SessionStorage
  if (id && nomePlano && periodicidade) {
    data.Id = id;
    data.NomePlano = nomePlano;
    data.Periodicidade = periodicidade;
  }
}


export function validateStep1() {
  const empresaEl = document.getElementById('EmpresaContratante_EmpresaContratante');
  const emailEl = document.getElementById('EmpresaContratante_Email');
  const emailError = document.getElementById('emailError');
  const empresa = empresaEl?.value.trim() || '';
  const email = emailEl?.value.trim() || '';

  if (!empresa || !email) {
    alert('Preencha todos os campos obrigatórios.');
    return false;
  }
  if (!isValidEmail(email)) {
    emailError.textContent = 'Formato de e-mail inválido.';
    emailError.classList.remove('d-none');
    return false;
  }
  emailError.classList.add('d-none');
  return true;
}


export function getStep1Data() {
    const empresa = document.getElementById('EmpresaContratante_EmpresaContratante').value.trim();
    const email = document.getElementById('EmpresaContratante_Email').value.trim();
    return { EmpresaContratante: empresa, EmailEmpresa: email };
}
