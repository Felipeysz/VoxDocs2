import { removeStep2EventListeners } from './ultilities.js';

// Exportar funções de evento para remoção
export function addPastaHandler() {
  clearStep2Messages();
  const el = createPastaElement({ Nome: '', Subpastas: [] }, document.getElementById('templatePastaPrincipal'), document.getElementById('templateSubpasta'));
  document.getElementById('pastasPrincipaisContainer').appendChild(el);
  configurarEventosPasta(el, document.getElementById('templateSubpasta'));
}

export function addSubpastaHandler(pastaEl) {
  clearStep2Messages();
  pastaEl.querySelector('.subpastas-container').appendChild(createSubpastaElement('', document.getElementById('templateSubpasta')));
}

export function removePastaHandler() {
  const pastaEl = this.closest('.pasta-principal');
  if (document.querySelectorAll('#pastasPrincipaisContainer .pasta-principal').length <= 1) {
    showStep2Error('Deve existir pelo menos uma pasta principal.');
  } else {
    clearStep2Messages();
    pastaEl.remove();
  }
}

export function removeSubpastaHandler() {
  const subpastaEl = this.closest('.subpasta-item');
  subpastaEl.remove();
}

export function initStep2(data) {
  const container = document.getElementById('pastasPrincipaisContainer');
  const btnAddPasta = document.getElementById('btnAdicionarPastaPrincipal');
  const tplPasta = document.getElementById('templatePastaPrincipal');
  const tplSubpasta = document.getElementById('templateSubpasta');

  if (!container || !btnAddPasta || !tplPasta || !tplSubpasta) {
    console.warn('Step 2: Elementos essenciais da UI não foram encontrados.');
    return;
  }

  // Limpa listeners antigos e o container
  removeStep2EventListeners();
  clearStep2Messages();
  container.innerHTML = '';

  // Garante que o listener seja adicionado apenas uma vez
  const newBtn = btnAddPasta.cloneNode(true);
  btnAddPasta.parentNode.replaceChild(newBtn, btnAddPasta);
  newBtn.addEventListener('click', addPastaHandler);

  // Carrega do formData ou cria uma pasta inicial
  const pastasSalvas = data.PastasPrincipais || [];
  if (pastasSalvas.length > 0) {
    pastasSalvas.forEach(pastaObj => {
      const el = createPastaElement(pastaObj, tplPasta, tplSubpasta);
      container.appendChild(el);
      configurarEventosPasta(el, tplSubpasta);
    });
  } else {
    // Inicia com uma pasta principal vazia
    addPastaHandler();
  }
}


export function validateStep2() {
  clearStep2Messages();
  const cards = document.querySelectorAll('#pastasPrincipaisContainer .pasta-principal');
  const nomesPastasPrincipais = new Set();

  if (cards.length === 0) {
    showStep2Error('É obrigatório criar pelo menos uma pasta principal.');
    return false;
  }

  for (const card of cards) {
    const nomeInput = card.querySelector('.pasta-nome');
    const nome = nomeInput.value.trim();

    if (!nome) {
      showStep2Error('Todas as pastas principais devem ter um nome.');
      nomeInput.focus();
      return false;
    }

    if (nomesPastasPrincipais.has(nome.toLowerCase())) {
      showStep2Error(`O nome da pasta principal "${nome}" está duplicado.`);
      nomeInput.focus();
      return false;
    }
    nomesPastasPrincipais.add(nome.toLowerCase());

    const subpastaInputs = card.querySelectorAll('.subpasta-nome');
    const nomesSubpastas = new Set();

    if (subpastaInputs.length === 0) {
      showStep2Error(`A pasta "${nome}" deve ter pelo menos uma subpasta.`);
      return false;
    }

    for (const subInput of subpastaInputs) {
      const subNome = subInput.value.trim();
      if (!subNome) {
        showStep2Error(`A pasta "${nome}" tem uma subpasta sem nome.`);
        subInput.focus();
        return false;
      }
      if (nomesSubpastas.has(subNome.toLowerCase())) {
        showStep2Error(`A subpasta "${subNome}" está duplicada dentro de "${nome}".`);
        subInput.focus();
        return false;
      }
      nomesSubpastas.add(subNome.toLowerCase());
    }
  }

  showStep2Success('Estrutura de pastas validada com sucesso!');
  return true;
}


export function getStep2Data() {
  const pastasData = [];
  const cards = document.querySelectorAll('#pastasPrincipaisContainer .pasta-principal');

  cards.forEach(card => {
    const nomePasta = card.querySelector('.pasta-nome').value.trim();
    const subpastas = [];
    card.querySelectorAll('.subpasta-nome').forEach(subInput => {
      const nomeSub = subInput.value.trim();
      if (nomeSub) {
        subpastas.push(nomeSub);
      }
    });

    if (nomePasta) {
      pastasData.push({
        Nome: nomePasta,
        Subpastas: subpastas
      });
    }
  });

  return pastasData;
}

// --- Funções Auxiliares ---

function createPastaElement(obj, tplPasta, tplSub) {
  const clone = tplPasta.content.cloneNode(true);
  const el = clone.querySelector('.pasta-principal');
  el.querySelector('.pasta-nome').value = obj.Nome || '';
  const subContainer = el.querySelector('.subpastas-container');
  (obj.Subpastas || []).forEach(nomeSub => {
    subContainer.appendChild(createSubpastaElement(nomeSub, tplSub));
  });
  return el;
}

function createSubpastaElement(nome, tplSubpasta) {
  const clone = tplSubpasta.content.cloneNode(true);
  const item = clone.querySelector('.subpasta-item');
  item.querySelector('.subpasta-nome').value = nome || '';
  item.querySelector('.btn-remover-subpasta').addEventListener('click', removeSubpastaHandler);
  return item;
}

function configurarEventosPasta(pastaEl, tplSubpasta) {
  pastaEl.querySelector('.btn-adicionar-subpasta').addEventListener('click', () => addSubpastaHandler(pastaEl));
  pastaEl.querySelector('.btn-remover-pasta').addEventListener('click', removePastaHandler);
}

function showStep2Error(msg) {
  const e = document.getElementById('step2Error'), s = document.getElementById('step2Success');
  if (e) { e.textContent = msg; e.classList.remove('d-none'); }
  if (s) { s.classList.add('d-none'); }
}

function showStep2Success(msg) {
  const s = document.getElementById('step2Success'), e = document.getElementById('step2Error');
  if (s) { s.textContent = msg; s.classList.remove('d-none'); }
  if (e) { e.classList.add('d-none'); }
}

function clearStep2Messages() {
  ['step2Error', 'step2Success'].forEach(id => {
    const el = document.getElementById(id);
    if (el) { el.textContent = ''; el.classList.add('d-none'); }
  });
}