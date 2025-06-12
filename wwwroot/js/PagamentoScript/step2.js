// step2.js - Step 2: Estrutura de Pastas
import { FormUtils } from './utilities.js';

export function initStep2(data) {
  const container = document.getElementById('pastasPrincipaisContainer');
  const btnAddPasta = document.getElementById('btnAdicionarPastaPrincipal');
  const tplPasta = document.getElementById('templatePastaPrincipal');
  const tplSubpasta = document.getElementById('templateSubpasta');

  if (!container || !btnAddPasta || !tplPasta || !tplSubpasta) return;

  // Limpa e reinicializa
  container.innerHTML = '';
  const newBtn = btnAddPasta.cloneNode(true);
  btnAddPasta.parentNode.replaceChild(newBtn, btnAddPasta);
  newBtn.addEventListener('click', addPastaHandler);

  // Carrega dados ou inicia vazio
  const pastasSalvas = data.PastasPrincipais || [];
  if (pastasSalvas.length > 0) {
    pastasSalvas.forEach(pasta => addPastaElement(pasta, tplPasta, tplSubpasta));
  } else {
    addPastaHandler();
  }
}

export function validateStep2() {
  FormUtils.clearAlerts('step2Error', 'step2Success');
  const pastas = document.querySelectorAll('#pastasPrincipaisContainer .pasta-principal');
  const nomesPastas = new Set();

  if (pastas.length === 0) {
    FormUtils.showAlert('step2Error', 'É obrigatório criar pelo menos uma pasta principal.');
    return false;
  }

  for (const pasta of pastas) {
    const nome = pasta.querySelector('.pasta-nome').value.trim();
    const subpastas = pasta.querySelectorAll('.subpasta-nome');
    const nomesSubpastas = new Set();

    if (!nome) {
      FormUtils.showAlert('step2Error', 'Todas as pastas principais devem ter um nome.');
      return false;
    }

    if (nomesPastas.has(nome.toLowerCase())) {
      FormUtils.showAlert('step2Error', `Nome de pasta duplicado: ${nome}`);
      return false;
    }
    nomesPastas.add(nome.toLowerCase());

    if (subpastas.length === 0) {
      FormUtils.showAlert('step2Error', `A pasta "${nome}" deve ter pelo menos uma subpasta.`);
      return false;
    }

    for (const sub of subpastas) {
      const subNome = sub.value.trim();
      if (!subNome) {
        FormUtils.showAlert('step2Error', `Subpasta sem nome na pasta "${nome}".`);
        return false;
      }
      if (nomesSubpastas.has(subNome.toLowerCase())) {
        FormUtils.showAlert('step2Error', `Subpasta duplicada em "${nome}": ${subNome}`);
        return false;
      }
      nomesSubpastas.add(subNome.toLowerCase());
    }
  }

  FormUtils.showAlert('step2Success', 'Estrutura de pastas validada com sucesso!', 'success');
  return true;
}

export function getStep2Data() {
  const pastas = document.querySelectorAll('#pastasPrincipaisContainer .pasta-principal');
  return {
    PastasPrincipais: Array.from(pastas).map(pasta => ({
      Nome: pasta.querySelector('.pasta-nome').value.trim(),
      Subpastas: Array.from(pasta.querySelectorAll('.subpasta-nome')).map(sub => sub.value.trim())
    }))
  };
}

// Funções auxiliares
function addPastaHandler() {
  FormUtils.clearAlerts('step2Error', 'step2Success');
  const tplPasta = document.getElementById('templatePastaPrincipal');
  const tplSub = document.getElementById('templateSubpasta');
  addPastaElement({ Nome: '', Subpastas: [] }, tplPasta, tplSub);
}

function addPastaElement(pastaObj, tplPasta, tplSub) {
  const clone = tplPasta.content.cloneNode(true);
  const pastaEl = clone.querySelector('.pasta-principal');
  
  pastaEl.querySelector('.pasta-nome').value = pastaObj.Nome || '';
  const subContainer = pastaEl.querySelector('.subpastas-container');
  
  (pastaObj.Subpastas || ['']).forEach(sub => {
    subContainer.appendChild(createSubpasta(sub, tplSub));
  });

  // Configura eventos
  pastaEl.querySelector('.btn-adicionar-subpasta').addEventListener('click', () => {
    subContainer.appendChild(createSubpasta('', tplSub));
  });

  pastaEl.querySelector('.btn-remover-pasta').addEventListener('click', function() {
    if (document.querySelectorAll('.pasta-principal').length > 1) {
      pastaEl.remove();
    } else {
      FormUtils.showAlert('step2Error', 'Deve existir pelo menos uma pasta principal.');
    }
  });

  document.getElementById('pastasPrincipaisContainer').appendChild(pastaEl);
  return pastaEl;
}

function createSubpasta(nome, tplSub) {
  const clone = tplSub.content.cloneNode(true);
  const subEl = clone.querySelector('.subpasta-item');
  subEl.querySelector('.subpasta-nome').value = nome || '';
  
  subEl.querySelector('.btn-remover-subpasta').addEventListener('click', function() {
    subEl.remove();
  });

  return subEl;
}