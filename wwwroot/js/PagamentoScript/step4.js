// wwwroot/js/PagamentoScript/step4.js
import { isValidEmail, isValidPassword, initPasswordToggle, removeStep4EventListeners } from './ultilities.js';

let currentData = {};

// Exportar funções de evento para remoção
export function addUserHandler() {
  const limit = parseInt(document.getElementById('userLimit').textContent, 10);
  const container = document.getElementById('usersContainer');
  if (container.children.length >= limit) {
    displayMessage(`Você não pode adicionar mais de ${limit} usuários comuns.`, 'error');
  } else {
    _addUserRow({}, container.children.length);
    _updateCount();
  }
}

export function removeUserHandler() {
  const card = this.closest('.user-card');
  if (card) {
    const userCards = document.querySelectorAll('#usersContainer .user-card');
    if (userCards.length <= 1) {
      displayMessage('Deve existir ao menos um usuário comum.', 'error');
    } else {
      card.remove();
      _reindexUsers();
      _updateCount();
    }
  }
}

export function userInputHandler() {
  clearMessages();
}

export function clearMessages() {
  ['step4Error', 'step4Success'].forEach(id => {
    const el = document.getElementById(id);
    if (el) {
      el.textContent = '';
      el.classList.add('d-none');
    }
  });
}

export function initStep4(data) {
  // Limpar listeners antigos antes de reinicializar
  removeStep4EventListeners();

  currentData = data;
  const container = document.getElementById('usersContainer');
  const btnAdd = document.getElementById('btnAdicionarUser');
  const countEl = document.getElementById('userCount');
  const limitEl = document.getElementById('userLimit');

  if (!container || !countEl || !limitEl) {
    console.warn('Step4: elementos não encontrados');
    return;
  }

  clearMessages();
  container.innerHTML = '';

  const existingUsers = Array.isArray(data.Usuarios) ? data.Usuarios : [];
  existingUsers.length ? 
    existingUsers.forEach((user, index) => _addUserRow(user, index)) : 
    _addUserRow({}, 0);
  
  _updateCount();

  initPasswordToggle();
  setupAddButton(container, limitEl);
}

export function validateStep4() {
  clearMessages();
  const userCards = document.querySelectorAll('#usersContainer .user-card');
  
  if (!userCards.length) {
    displayMessage('É obrigatório criar ao menos um usuário comum.', 'error');
    return false;
  }

  const adminEmails = new Set(
    (currentData.Administradores || []).map(a => a.Email.toLowerCase())
  );
  const adminLogins = new Set(
    (currentData.Administradores || []).map(a => a.Usuario.toLowerCase())
  );
  const empresaEmail = currentData.EmailEmpresa?.toLowerCase();

  for (const card of userCards) {
    const { Usuario, Email, Senha } = _getUserData(card); // ✅ Fixed destructuring

    if (!Usuario || !Email || !Senha) {
      displayMessage('Todos os campos devem ser preenchidos.', 'error');
      return false;
    }

    if (adminLogins.has(Usuario.toLowerCase())) {
      displayMessage(`O usuário "${Usuario}" já está em uso por um administrador.`, 'error');
      return false;
    }

    if (!isValidEmail(Email)) {
      displayMessage(`E-mail inválido: ${Email}`, 'error');
      return false;
    }

    if (adminEmails.has(Email.toLowerCase())) {
      displayMessage(`O e-mail "${Email}" já está em uso por um administrador.`, 'error');
      return false;
    }

    if (empresaEmail && Email.toLowerCase() === empresaEmail) {
      displayMessage(`O e-mail "${Email}" já está em uso pela empresa.`, 'error');
      return false;
    }

    if (!isValidPassword(Senha)) {
      displayMessage('Senha deve ter 8+ caracteres, incluir maiúscula, minúscula e caractere especial.', 'error');
      return false;
    }
  }

  displayMessage('Usuários comuns validados com sucesso.', 'success');
  return true;
}

export function getStep4Data() {
  const userCards = document.querySelectorAll('#usersContainer .user-card');
  return {
    Usuarios: Array.from(userCards).map(card => _getUserData(card))
  };
}

// ───── Funções internas ─────

function _addUserRow(dados, index) {
  const template = document.getElementById('templateUser').content.cloneNode(true);
  const card = template.querySelector('.user-card');
  
  if (!card) return;

  card.querySelector('.user-index').textContent = index + 1;
  card.querySelector('input[data-field="Usuario"]').value = dados.Usuario || '';
  card.querySelector('input[data-field="Email"]').value = dados.Email || '';
  card.querySelector('input[data-field="Senha"]').value = dados.Senha || '';

  _setupRemoveButton(card);
  _setupInputListeners(card);
  
  document.getElementById('usersContainer').appendChild(card);
  initPasswordToggle();
}

function setupAddButton(container, limitEl) {
  const btnAdd = document.getElementById('btnAdicionarUser');
  if (!btnAdd) return;

  btnAdd.replaceWith(btnAdd.cloneNode(true));
  document.getElementById('btnAdicionarUser').addEventListener('click', addUserHandler);
}

function _getUserData(card) {
  return {
    Usuario: card.querySelector('input[data-field="Usuario"]').value.trim(),
    Email: card.querySelector('input[data-field="Email"]').value.trim(),
    Senha: card.querySelector('input[data-field="Senha"]').value.trim(),
    PermissionAccount: 'User'
  };
}

function _setupRemoveButton(card) {
  const btnRemover = card.querySelector('.remover-user');
  if (!btnRemover) return;

  btnRemover.addEventListener('click', removeUserHandler);
}

function _setupInputListeners(card) {
  ['Usuario', 'Email', 'Senha'].forEach(field => {
    const input = card.querySelector(`input[data-field="${field}"]`);
    if (input) input.addEventListener('input', userInputHandler);
  });
}

function _reindexUsers() {
  document.querySelectorAll('#usersContainer .user-card').forEach((card, i) => {
    card.querySelector('.user-index').textContent = i + 1;
  });
}

function _updateCount() {
  const countEl = document.getElementById('userCount');
  if (countEl) {
    countEl.textContent = document.querySelectorAll('#usersContainer .user-card').length;
  }
}

function displayMessage(message, type) {
  const errorEl = document.getElementById('step4Error');
  const successEl = document.getElementById('step4Success');
  
  if (errorEl && successEl) {
    if (type === 'error') {
      errorEl.textContent = message;
      errorEl.classList.remove('d-none');
      successEl.classList.add('d-none');
    } else {
      successEl.textContent = message;
      successEl.classList.remove('d-none');
      errorEl.classList.add('d-none');
    }
  }
}