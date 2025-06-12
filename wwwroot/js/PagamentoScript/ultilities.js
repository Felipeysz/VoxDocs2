// Importações de funções específicas por etapa
import { addPastaHandler, addSubpastaHandler, removePastaHandler, removeSubpastaHandler } from './step2.js';
import { addAdminHandler, removeAdminHandler, adminInputHandler } from './step3.js';
import { addUserHandler, removeUserHandler, userInputHandler } from './step4.js';
import { handlePagarCartaoClick } from './step5.js';
import { finalizePayment } from './paymentUltils.js';

// Função para remover listeners do Step 2
export function removeStep2EventListeners() {
  document.getElementById('btnAdicionarPastaPrincipal')?.removeEventListener('click', addPastaHandler);
  document.querySelectorAll('.btn-adicionar-subpasta').forEach(btn => {
    btn.removeEventListener('click', addSubpastaHandler);
  });
  document.querySelectorAll('.btn-remover-pasta').forEach(btn => {
    btn.removeEventListener('click', removePastaHandler);
  });
  document.querySelectorAll('.btn-remover-subpasta').forEach(btn => {
    btn.removeEventListener('click', removeSubpastaHandler);
  });
  document.querySelectorAll('#pastasPrincipaisContainer input').forEach(input => {
    input.removeEventListener('input', () => {});
  });
}

// Função para remover listeners do Step 3
export function removeStep3EventListeners() {
  document.getElementById('btnAdicionarAdmin')?.removeEventListener('click', addAdminHandler);
  document.querySelectorAll('.remover-admin').forEach(btn => {
    btn.removeEventListener('click', removeAdminHandler);
  });
  document.querySelectorAll('#adminsContainer input').forEach(input => {
    input.removeEventListener('input', adminInputHandler);
  });
}

// Função para remover listeners do Step 4
export function removeStep4EventListeners() {
  document.getElementById('btnAdicionarUser')?.removeEventListener('click', addUserHandler);
  document.querySelectorAll('.remover-user').forEach(btn => {
    btn.removeEventListener('click', removeUserHandler);
  });
  document.querySelectorAll('#usersContainer input').forEach(input => {
    input.removeEventListener('input', userInputHandler);
  });
}

// Função para remover listeners do Step 5
export function removeStep5EventListeners() {
  document.getElementById('nextBtn')?.removeEventListener('click', finalizePayment);
  document.getElementById('pagarCartao')?.removeEventListener('click', handlePagarCartaoClick);
}

// Função principal para remover listeners de todas as etapas
export function removeOldEventListeners() {
  removeStep2EventListeners();
  removeStep3EventListeners();
  removeStep4EventListeners();
  removeStep5EventListeners();
}

// Funções gerais (não específicas a etapas)
export function initPasswordToggle() {
  $(document).on('click', '.toggle-password', function() {
    const $btn = $(this);
    const $input = $btn.siblings('.password-field');
    const $icon = $btn.find('i');

    if ($input.attr('type') === 'password') {
      $input.attr('type', 'text');
      $icon.removeClass('fa-eye').addClass('fa-eye-slash');
    } else {
      $input.attr('type', 'password');
      $icon.removeClass('fa-eye-slash').addClass('fa-eye');
    }
  });
}

export function isValidEmail(email) {
  return /^[\w.-]+@[\w.-]+\.\w{2,}$/.test(email);
}

export function isValidPassword(pw) {
  return /^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$/.test(pw);
}