document.addEventListener('DOMContentLoaded', () => {
  // Inicia animação AOS
  AOS.init({ once: true });

  // --- SELEÇÃO DE PLANO (Cartão e Pix) ---
  window.selecionarPlano = (periodicidade, preco) => {
    // Cartão
    const tipoCartaoInput = document.getElementById('tipoPlanoCartao');
    const precoSpan       = document.getElementById('precoSelecionado');
    if (tipoCartaoInput) tipoCartaoInput.value = periodicidade;
    if (precoSpan)       precoSpan.innerText = preco.toFixed(2);

    // Pix
    const tipoPixInput = document.getElementById('tipoPlanoPix');
    const valorPixText = document.querySelector('#pixInfo .valor');
    const planoPixText = document.querySelector('#pixInfo .plano');
    if (tipoPixInput)  tipoPixInput.value = periodicidade;
    if (valorPixText)  valorPixText.innerText = preco.toFixed(2);
    if (planoPixText)  planoPixText.innerText = `${periodicidade}`;
  };

  // --- CARTÃO ---
  const formCartao = document.getElementById('formCartao');
  if (formCartao) {
    formCartao.addEventListener('submit', async e => {
      e.preventDefault();
      const resEl = document.getElementById('resCartao');
      resEl?.classList.add('d-none');

      const payload = {
        CartaoNumber:    document.getElementById('cartaoNumber').value,
        ValidadeCartao:  document.getElementById('validadeCartao').value,
        CvvCartao:       document.getElementById('cvvCartao').value,
        TipoCartao:      'Credito',
        TipoPlano:       document.getElementById('tipoPlanoCartao').value
      };

      try {
        const res = await fetch('/api/PagamentoFalso/cartao', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(payload)
        });
        const data = await res.json();
        resEl.className = `alert alert-${res.ok ? 'success' : 'danger'}`;
        resEl.innerText = res.ok ? data.Mensagem : data.Erro;
      } catch {
        resEl.className = 'alert alert-danger';
        resEl.innerText = 'Erro ao enviar pagamento.';
      } finally {
        resEl.classList.remove('d-none');
      }
    });
  }

  // --- PIX (QR + Polling) ---
  const btnGerarPix = document.getElementById('btnGerarPix');
  if (btnGerarPix) {
    btnGerarPix.addEventListener('click', async () => {
      const tipoPlano = document.getElementById('tipoPlanoPix').value;
      const payload   = { tipoPlano };
      console.log('Enviando para a API:', payload);

      // exibe placeholder
      document.getElementById('qrPlaceholder').style.display = 'flex';
      document.getElementById('qrCodeContainer').innerHTML = '';
      document.getElementById('resPix').classList.add('d-none');

      try {
        const res = await fetch('/api/PagamentoFalso/pix/gerar', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(payload)
        });
        if (!res.ok) {
          const err = await res.json();
          throw err;
        }
        const data = await res.json();
        console.log('Resposta da API:', data);

        // remove placeholder e gera QR
        document.getElementById('qrPlaceholder').style.display = 'none';
        const urlPix = `${window.location.origin}${data.qrCode}`;
        new QRCode(document.getElementById("qrCodeContainer"), {
          text: urlPix,
          width: 200,
          height: 200,
        });

        // polling a cada 5s via status/{id}
        const intervalo = setInterval(async () => {
          try {
            const poll = await fetch(`/api/PagamentoFalso/pix/status/${data.pagamentoPixId}`);
            if (!poll.ok) return;
            const { confirmado } = await poll.json();
            if (confirmado) {
              clearInterval(intervalo);
              alert('Pagamento Pix confirmado!');
              window.location.href = '/';
            }
          } catch (e) {
            console.error('Erro no polling:', e);
          }
        }, 5000);

      } catch (error) {
        console.error('Erro ao chamar a API:', error);
        alert('Erro: ' + (error.Erro || error.message || 'Falha na requisição'));
      }
    });
  }

  // --- FORMATAÇÃO VALIDADE ---
  const validadeInput = document.getElementById('validadeCartao');
  if (validadeInput) {
    validadeInput.addEventListener('input', e => {
      let v = e.target.value.replace(/\D/g, '');
      if (v.length > 2) v = v.slice(0,2) + '/' + v.slice(2,4);
      e.target.value = v;
    });
  }
});
