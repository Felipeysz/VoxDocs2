// wwwroot/js/PagamentoScript/step3.js
import { isValidEmail, initPasswordToggle, removeStep3EventListeners } from './ultilities.js';

// Exportar funções de evento para remoção
export function addAdminHandler() {
  const limit = parseInt(document.getElementById('adminLimit').textContent, 10);
  const container = document.getElementById('adminsContainer');
  if (container.children.length >= limit) {
    displayMessage('Limite de administradores atingido.', 'error');
  } else {
    _addAdminRow({}, container.children.length);
    _updateCount();
  }
}

export function removeAdminHandler() {
  const card = this.closest('.admin-card');
  if (card) {
    card.remove();
    _reindexAdmins();
    _updateCount();
  }
}

export function adminInputHandler() {
  clearMessages();
}

export function clearMessages() {
  ['step3Error', 'step3Success'].forEach(id => {
    const el = document.getElementById(id);
    if (el) el.classList.add('d-none');
  });
}

export function initStep3(data) {
  // Limpar listeners antigos antes de reinicializar
  removeStep3EventListeners();

  const container = document.getElementById('adminsContainer');
  const btnAdd = document.getElementById('btnAdicionarAdmin');
  const tplAdmin = document.getElementById('templateAdmin');
  const countSpan = document.getElementById('adminCount');
  const limitSpan = document.getElementById('adminLimit');

  if (!container || !tplAdmin || !countSpan || !limitSpan) {
    console.warn('Step 3: elementos não encontrados');
    return;
  }

  clearMessages();
  container.innerHTML = '';

  // Se já houver administradores em formData, usa-os; senão, cria um vazio
  const existing = data.Administradores || [];
  if (existing.length) {
    existing.forEach((u, idx) => _addAdminRow(u, idx));
  } else {
    _addAdminRow({}, 0);
  }

  _updateCount();

  // Inicializa o toggle de mostrar/ocultar senha para cards já existentes
  initPasswordToggle();

  // Configura botão “Adicionar Administrador”
  if (btnAdd) {
    const newBtn = btnAdd.cloneNode(true);
    btnAdd.parentNode.replaceChild(newBtn, btnAdd);
    newBtn.addEventListener('click', addAdminHandler);
  }
}

export function validateStep3() {
  clearMessages();
  const cards = Array.from(document.querySelectorAll('#adminsContainer .admin-card'));
  if (cards.length === 0) {
    displayMessage('É obrigatório ter ao menos um administrador.', 'error');
    return false;
  }

  const seen = new Set();
  for (const card of cards) {
    const { Usuario, Email, Senha } = _getAdminData(card);

    if (!Usuario || !Email || !Senha) {
      displayMessage('Todos os campos dos administradores são obrigatórios.', 'error');
      return false;
    }

    if (!isValidEmail(Email)) {
      displayMessage(`E-mail inválido: ${Email}`, 'error');
      return false;
    }

    if (seen.has(Email.toLowerCase())) {
      displayMessage(`E-mail duplicado: ${Email}`, 'error');
      return false;
    }

    seen.add(Email.toLowerCase());
  }

  displayMessage('Administradores validados com sucesso!', 'success');
  return true;
}

export function getStep3Data() {
  return {
    Administradores: Array.from(
      document.querySelectorAll('#adminsContainer .admin-card')
    ).map(card => _getAdminData(card))
  };
}

// —————— Funções privadas ——————

function _addAdminRow(dados, index) {
  const tpl = document.getElementById('templateAdmin').content.cloneNode(true);
  const card = tpl.querySelector('.admin-card');
  if (!card) return;

  card.querySelector('.admin-index').textContent = index + 1;
  card.querySelector('input[data-field="Usuario"]').value = dados.Usuario || '';
  card.querySelector('input[data-field="Email"]').value = dados.Email || '';
  card.querySelector('input[data-field="Senha"]').value = dados.Senha || '';

  _setupRemoveButton(card);
  _setupInputListeners(card);

  document.getElementById('adminsContainer').appendChild(card);
}

function _setupRemoveButton(card) {
  const btnRemover = card.querySelector('.remover-admin');
  if (btnRemover) {
    btnRemover.addEventListener('click', removeAdminHandler);
  }
}

function _setupInputListeners(card) {
  ['Usuario', 'Email', 'Senha'].forEach(field => {
    const input = card.querySelector(`input[data-field="${field}"]`);
    if (input) input.addEventListener('input', adminInputHandler);
  });
}

function _reindexAdmins() {
  document.querySelectorAll('#adminsContainer .admin-card').forEach((card, i) => {
    card.querySelector('.admin-index').textContent = i + 1;
  });
}

function _updateCount() {
  document.getElementById('adminCount').textContent =
    document.querySelectorAll('#adminsContainer .admin-card').length;
}

function _getAdminData(card) {
  return {
    Usuario: card.querySelector('input[data-field="Usuario"]').value.trim(),
    Email: card.querySelector('input[data-field="Email"]').value.trim(),
    Senha: card.querySelector('input[data-field="Senha"]').value.trim(),
    PermissionAccount: 'Admin'
  };
}

// —————— Mensagens ——————

function displayMessage(message, type) {
  const errorEl = document.getElementById('step3Error');
  const successEl = document.getElementById('step3Success');

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