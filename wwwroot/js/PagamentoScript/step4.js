// wwwroot/js/PagamentoScript/step4.js
import { isValidEmail, isValidPassword, initPasswordToggle, removeStep4EventListeners } from './ultilities.js';

const step4 = {
  currentData: {},

  // --- Public API ---
  addUserHandler() {
    const limit = parseInt(document.getElementById('userLimit').textContent, 10);
    const container = document.getElementById('usersContainer');
    if (container.children.length >= limit) {
      this._displayMessage(`Você não pode adicionar mais de ${limit} usuários comuns.`, 'error');
    } else {
      this._addUserRow({}, container.children.length);
      this._updateCount();
    }
  },

  removeUserHandler() {
    const card = this.closest('.user-card');
    if (card) {
      const userCards = document.querySelectorAll('#usersContainer .user-card');
      if (userCards.length <= 1) {
        this._displayMessage('Deve existir ao menos um usuário comum.', 'error');
      } else {
        card.remove();
        this._reindexUsers();
        this._updateCount();
      }
    }
  },

  userInputHandler() {
    this._clearMessages();
  },

  init(data) {
    removeStep4EventListeners();

    this.currentData = data;
    const container = document.getElementById('usersContainer');
    const btnAdd = document.getElementById('btnAdicionarUser');
    const countEl = document.getElementById('userCount');
    const limitEl = document.getElementById('userLimit');

    if (!container || !countEl || !limitEl) {
      console.warn('Step4: elementos não encontrados');
      return;
    }

    this._clearMessages();
    container.innerHTML = '';

    const existingUsers = Array.isArray(data.Usuarios) ? data.Usuarios : [];
    existingUsers.length ? 
      existingUsers.forEach((user, index) => this._addUserRow(user, index)) : 
      this._addUserRow({}, 0);
    
    this._updateCount();

    initPasswordToggle();
    this._setupAddButton();
  },

  validate() {
    this._clearMessages();
    const userCards = document.querySelectorAll('#usersContainer .user-card');
    
    if (!userCards.length) {
      this._displayMessage('É obrigatório criar ao menos um usuário comum.', 'error');
      return false;
    }

    const adminEmails = new Set(
      (this.currentData.Administradores || []).map(a => a.Email.toLowerCase())
    );
    const adminLogins = new Set(
      (this.currentData.Administradores || []).map(a => a.Usuario.toLowerCase())
    );
    const empresaEmail = this.currentData.EmailEmpresa?.toLowerCase();

    for (const card of userCards) {
      const { Usuario, Email, Senha } = this._getUserData(card);

      if (!Usuario || !Email || !Senha) {
        this._displayMessage('Todos os campos devem ser preenchidos.', 'error');
        return false;
      }

      if (adminLogins.has(Usuario.toLowerCase())) {
        this._displayMessage(`O usuário "${Usuario}" já está em uso por um administrador.`, 'error');
        return false;
      }

      if (!isValidEmail(Email)) {
        this._displayMessage(`E-mail inválido: ${Email}`, 'error');
        return false;
      }

      if (adminEmails.has(Email.toLowerCase())) {
        this._displayMessage(`O e-mail "${Email}" já está em uso por um administrador.`, 'error');
        return false;
      }

      if (empresaEmail && Email.toLowerCase() === empresaEmail) {
        this._displayMessage(`O e-mail "${Email}" já está em uso pela empresa.`, 'error');
        return false;
      }

      if (!isValidPassword(Senha)) {
        this._displayMessage('Senha deve ter 8+ caracteres, incluir maiúscula, minúscula e caractere especial.', 'error');
        return false;
      }
    }

    this._displayMessage('Usuários comuns validados com sucesso.', 'success');
    return true;
  },

  getData() {
    const userCards = document.querySelectorAll('#usersContainer .user-card');
    return {
      Usuarios: Array.from(userCards).map(card => this._getUserData(card))
    };
  },

  // --- Private Methods ---
  _addUserRow(dados, index) {
    const template = document.getElementById('templateUser').content.cloneNode(true);
    const card = template.querySelector('.user-card');
    
    if (!card) return;

    card.querySelector('.user-index').textContent = index + 1;
    card.querySelector('input[data-field="Usuario"]').value = dados.Usuario || '';
    card.querySelector('input[data-field="Email"]').value = dados.Email || '';
    card.querySelector('input[data-field="Senha"]').value = dados.Senha || '';

    this._setupRemoveButton(card);
    this._setupInputListeners(card);
    
    document.getElementById('usersContainer').appendChild(card);
    initPasswordToggle();
  },

  _setupAddButton() {
    const btnAdd = document.getElementById('btnAdicionarUser');
    if (!btnAdd) return;

    btnAdd.replaceWith(btnAdd.cloneNode(true));
    document.getElementById('btnAdicionarUser').addEventListener('click', this.addUserHandler.bind(this));
  },

  _getUserData(card) {
    return {
      Usuario: card.querySelector('input[data-field="Usuario"]').value.trim(),
      Email: card.querySelector('input[data-field="Email"]').value.trim(),
      Senha: card.querySelector('input[data-field="Senha"]').value.trim(),
      PermissionAccount: 'User'
    };
  },

  _setupRemoveButton(card) {
    const btnRemover = card.querySelector('.remover-user');
    if (!btnRemover) return;

    btnRemover.addEventListener('click', this.removeUserHandler.bind(this));
  },

  _setupInputListeners(card) {
    ['Usuario', 'Email', 'Senha'].forEach(field => {
      const input = card.querySelector(`input[data-field="${field}"]`);
      if (input) input.addEventListener('input', this.userInputHandler.bind(this));
    });
  },

  _reindexUsers() {
    document.querySelectorAll('#usersContainer .user-card').forEach((card, i) => {
      card.querySelector('.user-index').textContent = i + 1;
    });
  },

  _updateCount() {
    const countEl = document.getElementById('userCount');
    if (countEl) {
      countEl.textContent = document.querySelectorAll('#usersContainer .user-card').length;
    }
  },

  _displayMessage(message, type) {
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
  },

  _clearMessages() {
    ['step4Error', 'step4Success'].forEach(id => {
      const el = document.getElementById(id);
      if (el) {
        el.textContent = '';
        el.classList.add('d-none');
      }
    });
  }
};

export default step4;