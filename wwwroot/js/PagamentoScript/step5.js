// wwwroot/js/PagamentoScript/step5.js
import {
  processPayment,
  detectBrand,
  validCardNumber,
  validExpiry,
  validCvv
} from './paymentUltils.js';
import { removeStep5EventListeners } from './ultilities.js';

const step5 = {
  // --- Public API ---
  handlePagarCartaoClick() {
    document.getElementById('nextBtn')?.click();
  },

  init(data) {
    removeStep5EventListeners();

    const planoEl = document.getElementById('PlanoPago');
    if (planoEl) planoEl.value = data.NomePlano || '';

    this._setupTabs();
    this._setupCardListeners();
    this._setupFinalizeListener();

    const savedMethod = this.getData().MetodoPagamento;
    if (savedMethod) {
      const tab = document.querySelector(`.tab[data-target="${savedMethod}"]`);
      if (tab) tab.click();
    }
  },

  validate() {
    this._clearMessages();
    const activeTab = document.querySelector('.tab.active');
    if (!activeTab) {
      this._showError('Selecione um método de pagamento.');
      return false;
    }
    return true;
  },

  getData() {
    const active = document.querySelector('.tab.active')?.dataset.target;
    return { MetodoPagamento: active };
  },

  async handleFinalizeStep() {
    const currentStep = document.querySelector('.step.bg-primary')?.textContent.trim();
    if (currentStep !== '5') return;
    if (!this.validate()) return;

    this._clearMessages();

    const formData = JSON.parse(sessionStorage.getItem('formData')) || {};
    const { MetodoPagamento } = this.getData();
    formData.metodoPagamento = MetodoPagamento;
    formData.tipoPlano = formData.NomePlano;

    if (MetodoPagamento === 'cartao') {
      const numero = document.getElementById('cartaoNumber').value.replace(/\s/g, '');
      const validade = document.getElementById('validadeCartao').value;
      const cvv = document.getElementById('cvvCartao').value;
      const brand = detectBrand(numero);

      if (!validCardNumber(numero)) return this._showError('Número do cartão inválido.');
      if (!validExpiry(validade)) return this._showError('Validade do cartão inválida.');
      if (!validCvv(cvv, brand)) return this._showError('CVV inválido.');

      formData.cartao = { numero, validade, cvv };
    }

    try {
      const result = await processPayment(formData);
      const pagamentoId = result.cartao?.pagamentoConcluidoId;

      if (MetodoPagamento === 'cartao' && pagamentoId) {
        window.location.href =
          `/ConfirmarPagamentoCartao?token=${encodeURIComponent(pagamentoId)}&plano=${encodeURIComponent(formData.tipoPlano)}`;
      }
    } catch (err) {
      this._showError(err.message);
    }
  },

  // --- Private Methods ---
  _setupTabs() {
    document.querySelectorAll('.tab').forEach(tab => {
      tab.addEventListener('click', () => {
        document.querySelectorAll('.tab-content').forEach(content => {
          content.classList.remove('active');
          content.classList.add('d-none');
        });
        document.querySelectorAll('.tab').forEach(t => t.classList.remove('active'));
        tab.classList.add('active');
        const content = document.getElementById(tab.dataset.target);
        if (content) {
          content.classList.remove('d-none');
          content.classList.add('active');
        }
        if (tab.dataset.target === 'card') this._setupCardListeners();
        this._clearMessages();
      });
    });
  },

  _setupCardListeners() {
    const numEl = document.getElementById('cartaoNumber');
    numEl?.removeEventListener('input', this._cardInputHandler.bind(this));
    numEl?.addEventListener('input', this._cardInputHandler.bind(this));

    const validadeEl = document.getElementById('validadeCartao');
    validadeEl?.removeEventListener('input', this._validadeInputHandler.bind(this));
    validadeEl?.addEventListener('input', this._validadeInputHandler.bind(this));

    const pagarBtn = document.getElementById('pagarCartao');
    pagarBtn?.removeEventListener('click', this.handlePagarCartaoClick.bind(this));
    pagarBtn?.addEventListener('click', this.handlePagarCartaoClick.bind(this));
  },

  _setupFinalizeListener() {
    const btnNext = document.getElementById('nextBtn');
    if (!btnNext) return;

    btnNext.removeEventListener('click', this.handleFinalizeStep.bind(this));
    btnNext.addEventListener('click', this.handleFinalizeStep.bind(this));
  },

  _cardInputHandler(e) {
    let v = e.target.value.replace(/\D/g, '').slice(0, 16);
    e.target.value = v.match(/.{1,4}/g)?.join(' ') || '';
    const brand = detectBrand(v);
    document.getElementById('cc-flag').style.backgroundImage = brand
      ? `url(https://cdn.jsdelivr.net/gh/lipis/flag-icons/flags/4x3/${brand}.svg)`
      : '';
  },

  _validadeInputHandler(e) {
    let v = e.target.value.replace(/\D/g, '').slice(0, 4);
    e.target.value = v.length > 2 ? `${v.slice(0, 2)}/${v.slice(2)}` : v;
  },

  _clearMessages() {
    ['resCartao'].forEach(id => {
      const el = document.getElementById(id);
      if (el) el.classList.add('d-none');
    });
  },

  _showError(msg) {
    ['resCartao'].forEach(id => {
      const el = document.getElementById(id);
      if (el) {
        el.classList.remove('d-none');
        el.classList.replace('alert-success', 'alert-danger');
        el.classList.add('alert');
        el.textContent = msg;
      }
    });
  }
};

export default step5;