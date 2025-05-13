document.addEventListener('DOMContentLoaded', () => {
  const micBtn   = document.getElementById('mic-btn-modal');
  const btnText  = document.getElementById('btn-text');
  const subtitle = document.getElementById('subtitle-modal');
  const response = document.getElementById('response-modal');
  const spinner  = document.getElementById('spinner');

  // Evita execução se os elementos do modal não existem
  if (!micBtn || !btnText || !subtitle || !response || !spinner) return;

  let isRecording = false, timer, sec = 0;

  function startRecording() {
    isRecording = true;
    micBtn.classList.add('recording');
    sec = 0;
    btnText.textContent = `Gravando ${sec}s`;
    subtitle.textContent = 'Gravando sua pergunta…';
    response.textContent = '';
    spinner.classList.remove('visible');
    timer = setInterval(() => {
      sec++;
      btnText.textContent = `Gravando ${sec}s`;
    }, 1000);
  }

  function stopRecording() {
    isRecording = false;
    clearInterval(timer);
    micBtn.classList.remove('recording');
    micBtn.classList.add('sending');
    btnText.textContent = 'Enviando…';
    subtitle.textContent = 'Enviando áudio…';
    spinner.classList.add('visible');

    setTimeout(() => {
      micBtn.classList.remove('sending');
      spinner.classList.remove('visible');
      btnText.textContent = 'Processando…';
      subtitle.textContent = 'Processando resposta…';
      response.classList.add('loading');

      setTimeout(() => {
        response.classList.remove('loading');
        const exemplos = [
          'Documento “ABC123” localizado com sucesso.',
          'Nenhum arquivo encontrado. Tente outro termo.',
          'Documento “RELATÓRIO.pdf” arquivado.',
        ];
        const random = exemplos[Math.floor(Math.random() * exemplos.length)];
        response.textContent = `Resposta: ${random}`;
        btnText.textContent = 'Gravar';
        subtitle.textContent = 'Segure para gravar';
        sec = 0;
      }, 2000);
    }, 800);
  }

  // Evento de clique no botão do microfone
  document.body.addEventListener('click', e => {
    if (e.target.closest('#mic-btn-modal')) {
      if (!isRecording) startRecording();
      else stopRecording();
    }
  });

  // Adiciona cursor pointer às caixas de recurso (se existirem)
  document.querySelectorAll('.feature-box').forEach(box => {
    box.style.cursor = 'pointer';
  });
});
