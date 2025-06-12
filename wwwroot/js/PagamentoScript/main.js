// main.js
// Imports de Steps
import { initStep1, validateStep1, getStep1Data } from './step1.js';
import { initStep2, validateStep2, getStep2Data } from './step2.js';
import { initStep3, validateStep3, getStep3Data } from './step3.js';
import { initStep4, validateStep4, getStep4Data } from './step4.js';
import { initStep5, validateStep5, getStep5Data } from './step5.js';
import { removeOldEventListeners } from './ultilities.js';

let currentStep = 1;
const totalSteps = 5;
let formData = JSON.parse(sessionStorage.getItem('formData')) || {};

// Função para mesclar dados do ViewBag com o formData existente
function mergeFormDataWithViewBag() {
  const id = document.getElementById('idPlano')?.value;
  const nomePlano = document.getElementById('nomePlano')?.value;
  const periodicidade = document.getElementById('periodicidadePlano')?.value;

  formData = {
    ...formData,
    Id: id,
    NomePlano: nomePlano,
    Periodicidade: periodicidade,
    EmpresaContratante: formData.EmpresaContratante || '',
    EmailEmpresa: formData.EmailEmpresa || '',
    PastasPrincipais: formData.PastasPrincipais || [],
    Administradores: formData.Administradores || [],
    Usuarios: formData.Usuarios || []
  };
}

// Função para mostrar uma etapa específica
function showStep(step) {
  // Esconde todos os steps
  document.querySelectorAll('.step-content').forEach(el => {
    el.classList.add('d-none');
  });

  // Exibe o atual
  const content = document.querySelector(`.step-content[data-step="${step}"]`);
  if (content) content.classList.remove('d-none');

  // Atualiza o indicador do stepper
  document.querySelectorAll('.step').forEach((el, idx) => {
    const active = idx + 1 === step;
    el.classList.toggle('bg-primary', active);
    el.classList.toggle('text-white', active);
    el.classList.toggle('bg-light', !active);
    el.classList.toggle('text-muted', !active);
  });

  updateNavigationButtons(step);

  // Inicializa o step atual
  switch (step) {
    case 1:
      initStep1(formData);
      break;
    case 2:
      initStep2(formData);
      break;
    case 3:
      initStep3(formData);
      break;
    case 4:
      initStep4(formData);
      break;
    case 5:
      initStep5(formData);
      break;
  }
}

// Atualiza os botões de navegação
function updateNavigationButtons(step) {
  const prevBtn = document.getElementById('prevBtn');
  const nextBtn = document.getElementById('nextBtn');

  prevBtn.disabled = (step === 1);

  if (step === totalSteps) {
    nextBtn.style.display = 'none';
  } else {
    nextBtn.style.display = '';
    nextBtn.textContent = 'Próximo »';
  }
}

// Valida a etapa atual
function validateCurrentStep() {
  switch (currentStep) {
    case 1: return validateStep1();
    case 2: return validateStep2();
    case 3: return validateStep3();
    case 4: return validateStep4();
    case 5: return validateStep5();
    default: return true;
  }
}

// Salva os dados da etapa atual no formData
function saveStepData(step) {
  const data = {
    1: getStep1Data(),
    2: getStep2Data(),
    3: getStep3Data(),
    4: getStep4Data(),
    5: getStep5Data()
  }[step];

  if (data) {
    formData = {
      ...formData,
      ...data
    };
    sessionStorage.setItem('formData', JSON.stringify(formData));
  }
}

// Navega para a próxima etapa
function goToNextStep() {
  if (!validateCurrentStep()) return;

  saveStepData(currentStep);

  if (currentStep < totalSteps) {
    currentStep++;
    showStep(currentStep);
  }
}

// Navega para a etapa anterior
function goToPrevStep() {
  if (currentStep > 1) {
    currentStep--;
    showStep(currentStep);
  }
}

// Inicialização ao carregar a página
document.addEventListener('DOMContentLoaded', () => {
  // Mescla dados do ViewBag com o formData existente
  mergeFormDataWithViewBag();

  // Configura eventos
  document.getElementById('nextBtn').addEventListener('click', goToNextStep);
  document.getElementById('prevBtn').addEventListener('click', goToPrevStep);

  // Exibe a primeira etapa
  showStep(currentStep);
});