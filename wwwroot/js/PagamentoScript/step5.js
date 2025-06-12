import {
  processPayment,
  detectBrand,
  validCardNumber,
  validExpiry,
  validCvv
} from './paymentUltils.js';
import { removeStep5EventListeners } from './ultilities.js';

// Exportar funções de evento para remoção
export function handlePagarCartaoClick() {
  document.getElementById('nextBtn')?.click();
}

export function initStep5(data) {
  removeStep5EventListeners();

  const planoEl = document.getElementById('PlanoPago');
  if (planoEl) planoEl.value = data.NomePlano || '';

  setupTabs();
  setupCardListeners();
  setupFinalizeListener();

  const savedMethod = getStep5Data().MetodoPagamento;
  if (savedMethod) {
    const tab = document.querySelector(`.tab[data-target="${savedMethod}"]`);
    if (tab) tab.click();
  }
}

export function validateStep5() {
  clearStep5Messages();
  const activeTab = document.querySelector('.tab.active');
  if (!activeTab) {
    showStep5Error('Selecione um método de pagamento.');
    return false;
  }
  return true;
}

export function getStep5Data() {
  const active = document.querySelector('.tab.active')?.dataset.target;
  return { MetodoPagamento: active };
}

// ─── Helpers de UI ───
function setupTabs() {
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
      if (tab.dataset.target === 'card') setupCardListeners();
      clearStep5Messages();
    });
  });
}

function clearStep5Messages() {
  ['resCartao'].forEach(id => {
    const el = document.getElementById(id);
    if (el) el.classList.add('d-none');
  });
}

function showStep5Error(msg) {
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

// ─── Listener de Conclusão ───

function setupFinalizeListener() {
  const btnNext = document.getElementById('nextBtn');
  if (!btnNext) return;

  btnNext.removeEventListener('click', handleFinalizeStep);
  btnNext.addEventListener('click', handleFinalizeStep);
}

export async function handleFinalizeStep() {
  const currentStep = document.querySelector('.step.bg-primary')?.textContent.trim();
  if (currentStep !== '5') return;
  if (!validateStep5()) return;

  clearStep5Messages();

  const formData = JSON.parse(sessionStorage.getItem('formData')) || {};
  const { MetodoPagamento } = getStep5Data();
  formData.metodoPagamento = MetodoPagamento;
  formData.tipoPlano = formData.NomePlano;

  if (MetodoPagamento === 'cartao') {
    const numero = document.getElementById('cartaoNumber').value.replace(/\s/g, '');
    const validade = document.getElementById('validadeCartao').value;
    const cvv = document.getElementById('cvvCartao').value;
    const brand = detectBrand(numero);

    // Validações manuais antes de enviar
    if (!validCardNumber(numero)) return showStep5Error('Número do cartão inválido.');
    if (!validExpiry(validade)) return showStep5Error('Validade do cartão inválida.');
    if (!validCvv(cvv, brand)) return showStep5Error('CVV inválido.');

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
    showStep5Error(err.message);
  }
}


// ─── Lógica de Cartão ───
function setupCardListeners() {
  const numEl = document.getElementById('cartaoNumber');
  numEl?.removeEventListener('input', cardInputHandler);
  numEl?.addEventListener('input', cardInputHandler);

  const validadeEl = document.getElementById('validadeCartao');
  validadeEl?.removeEventListener('input', validadeInputHandler);
  validadeEl?.addEventListener('input', validadeInputHandler);

  const pagarBtn = document.getElementById('pagarCartao');
  pagarBtn?.removeEventListener('click', handlePagarCartaoClick);
  pagarBtn?.addEventListener('click', handlePagarCartaoClick);
}
function cardInputHandler(e) {
  let v = e.target.value.replace(/\D/g, '').slice(0, 16);
  e.target.value = v.match(/.{1,4}/g)?.join(' ') || '';
  const brand = detectBrand(v);
  document.getElementById('cc-flag').style.backgroundImage = brand
    ? `url(https://cdn.jsdelivr.net/gh/lipis/flag-icons/flags/4x3/${brand}.svg)`
    : '';
}
function validadeInputHandler(e) {
  let v = e.target.value.replace(/\D/g, '').slice(0, 4);
  e.target.value = v.length > 2 ? `${v.slice(0, 2)}/${v.slice(2)}` : v;
}