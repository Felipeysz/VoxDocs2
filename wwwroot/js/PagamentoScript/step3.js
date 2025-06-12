// wwwroot/js/PagamentoScript/step3.js
import { isValidEmail, initPasswordToggle, removeStep3EventListeners } from './ultilities.js';

const step3 = {
  // --- Public API ---
  addAdminHandler() {
    const limit = parseInt(document.getElementById('adminLimit').textContent, 10);
    const container = document.getElementById('adminsContainer');
    if (container.children.length >= limit) {
      this._displayMessage('Limite de administradores atingido.', 'error');
    } else {
      this._addAdminRow({}, container.children.length);
      this._updateCount();
    }
  },

  removeAdminHandler() {
    const card = this.closest('.admin-card');
    if (card) {
      card.remove();
      this._reindexAdmins();
      this._updateCount();
    }
  },

  adminInputHandler() {
    this._clearMessages();
  },

  init(data) {
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

    this._clearMessages();
    container.innerHTML = '';

    const existing = data.Administradores || [];
    if (existing.length) {
      existing.forEach((u, idx) => this._addAdminRow(u, idx));
    } else {
      this._addAdminRow({}, 0);
    }

    this._updateCount();
    initPasswordToggle();

    if (btnAdd) {
      const newBtn = btnAdd.cloneNode(true);
      btnAdd.parentNode.replaceChild(newBtn, btnAdd);
      newBtn.addEventListener('click', this.addAdminHandler.bind(this));
    }
  },

  validate() {
    this._clearMessages();
    const cards = Array.from(document.querySelectorAll('#adminsContainer .admin-card'));
    if (cards.length === 0) {
      this._displayMessage('É obrigatório ter ao menos um administrador.', 'error');
      return false;
    }

    const seen = new Set();
    for (const card of cards) {
      const { Usuario, Email, Senha } = this._getAdminData(card);

      if (!Usuario || !Email || !Senha) {
        this._displayMessage('Todos os campos dos administradores são obrigatórios.', 'error');
        return false;
      }

      if (!isValidEmail(Email)) {
        this._displayMessage(`E-mail inválido: ${Email}`, 'error');
        return false;
      }

      if (seen.has(Email.toLowerCase())) {
        this._displayMessage(`E-mail duplicado: ${Email}`, 'error');
        return false;
      }

      seen.add(Email.toLowerCase());
    }

    this._displayMessage('Administradores validados com sucesso!', 'success');
    return true;
  },

  getData() {
    return {
      Administradores: Array.from(
        document.querySelectorAll('#adminsContainer .admin-card')
      ).map(card => this._getAdminData(card))
    };
  },

  // --- Private Methods ---
  _addAdminRow(dados, index) {
    const tpl = document.getElementById('templateAdmin').content.cloneNode(true);
    const card = tpl.querySelector('.admin-card');
    if (!card) return;

    card.querySelector('.admin-index').textContent = index + 1;
    card.querySelector('input[data-field="Usuario"]').value = dados.Usuario || '';
    card.querySelector('input[data-field="Email"]').value = dados.Email || '';
    card.querySelector('input[data-field="Senha"]').value = dados.Senha || '';

    this._setupRemoveButton(card);
    this._setupInputListeners(card);

    document.getElementById('adminsContainer').appendChild(card);
  },

  _setupRemoveButton(card) {
    const btnRemover = card.querySelector('.remover-admin');
    if (btnRemover) {
      btnRemover.addEventListener('click', this.removeAdminHandler.bind(this));
    }
  },

  _setupInputListeners(card) {
    ['Usuario', 'Email', 'Senha'].forEach(field => {
      const input = card.querySelector(`input[data-field="${field}"]`);
      if (input) input.addEventListener('input', this.adminInputHandler.bind(this));
    });
  },

  _reindexAdmins() {
    document.querySelectorAll('#adminsContainer .admin-card').forEach((card, i) => {
      card.querySelector('.admin-index').textContent = i + 1;
    });
  },

  _updateCount() {
    document.getElementById('adminCount').textContent =
      document.querySelectorAll('#adminsContainer .admin-card').length;
  },

  _getAdminData(card) {
    return {
      Usuario: card.querySelector('input[data-field="Usuario"]').value.trim(),
      Email: card.querySelector('input[data-field="Email"]').value.trim(),
      Senha: card.querySelector('input[data-field="Senha"]').value.trim(),
      PermissionAccount: 'Admin'
    };
  },

  _displayMessage(message, type) {
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
  },

  _clearMessages() {
    ['step3Error', 'step3Success'].forEach(id => {
      const el = document.getElementById(id);
      if (el) el.classList.add('d-none');
    });
  }
};

export default step3;