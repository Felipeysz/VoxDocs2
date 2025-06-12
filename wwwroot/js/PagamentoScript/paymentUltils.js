// --- HELPERS DE VALIDAÇÃO DE CARTÃO ---
export function validCardNumber(num) {
  const sanitized = num.replace(/\D/g, '');
  let sum = 0, shouldDouble = false;
  for (let i = sanitized.length - 1; i >= 0; i--) {
    let digit = parseInt(sanitized.charAt(i), 10);
    if (shouldDouble) {
      digit *= 2;
      if (digit > 9) digit -= 9;
    }
    sum += digit;
    shouldDouble = !shouldDouble;
  }
  return sum % 10 === 0;
}

export function detectBrand(num) {
  const sanitized = num.replace(/\D/g, '');
  if (/^4/.test(sanitized)) return 'visa';
  if (/^5[1-5]/.test(sanitized)) return 'mastercard';
  if (/^3[47]/.test(sanitized)) return 'amex';
  return '';
}

export function validExpiry(v) {
  if (!/^\d{2}\/\d{2}$/.test(v)) return false;
  const [mm, yy] = v.split('/').map(n => parseInt(n, 10));
  if (mm < 1 || mm > 12) return false;
  const now = new Date();
  const exp = new Date(2000 + yy, mm, 0, 23, 59, 59);
  return exp > now;
}

export function validCvv(cvv, brand = '') {
  const sanitized = cvv.replace(/\D/g, '');
  return brand === 'amex'
    ? /^\d{4}$/.test(sanitized)
    : /^\d{3}$/.test(sanitized);
}

// --- PAYLOAD & API ---
export function buildPaymentPayload(formData) {
  const pastasPrincipais = (formData.PastasPrincipais || []).map(pasta => ({
    nomePastaPrincipal: pasta.Nome || '',
    empresaContratante: formData.EmpresaContratante || ''
  }));

  const subPastas = (formData.PastasPrincipais || []).flatMap(pasta =>
    (pasta.Subpastas || []).map(sub => ({
      nomeSubPasta: sub,
      nomePastaPrincipal: pasta.Nome || '',
      empresaContratante: formData.EmpresaContratante || ''
    }))
  );

  const usuarios = [
    ...(formData.Administradores || []).map(u => ({
      usuario: u.Usuario || '',
      email: u.Email || '',
      senha: u.Senha || '',
      permissionAccount: u.PermissionAccount || '',
      empresaContratante: u.EmpresaContratante || formData.EmpresaContratante || '',
      planoPago: formData.NomePlano || ''
    })),
    ...(formData.Usuarios || []).map(u => ({
      usuario: u.Usuario || '',
      email: u.Email || '',
      senha: u.Senha || '',
      permissionAccount: u.PermissionAccount || '',
      empresaContratante: u.EmpresaContratante || formData.EmpresaContratante || '',
      planoPago: formData.NomePlano || ''
    }))
  ];

  return {
    id: formData.Id || '',
    metodoPagamento: formData.metodoPagamento || '',
    empresaContratantePlano: formData.EmpresaContratante || '',
    dataPagamento: new Date().toISOString(),
    nomePlano: formData.NomePlano || '',
    periodicidade: formData.Periodicidade || '',
    empresaContratante: {
      empresaContratante: formData.EmpresaContratante || '',
      email: formData.EmailEmpresa || ''
    },
    pastasPrincipais,
    subPastas,
    usuarios
  };
}


// --- Se houver outras funções de cartão, mantenha-as aqui ---
export async function processPayment(formData) {
  // validações de cartão
  if (formData.metodoPagamento === 'cartao') {
    const { numero, validade, cvv } = formData.cartao || {};
    if (!validCardNumber(numero)) throw new Error('Cartão inválido');
    if (!validExpiry(validade)) throw new Error('Validade inválida');
    if (!validCvv(cvv, detectBrand(numero))) throw new Error('CVV inválido');
  }
  const payload = buildPaymentPayload(formData);
  const result = await finalizePayment(payload);
  return { cartao: result };
}

export async function finalizePayment(payload) {
  const response = await fetch('/api/Pagamentos/finalizar', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(payload)
  });

  const text = await response.text();
  let data;
  try {
    data = JSON.parse(text);
  } catch {
    data = text;
  }

  if (!response.ok) {
    let message = 'Falha ao finalizar pagamento';

    // Se for texto puro com exceção, tenta extrair a 1ª linha útil
    if (typeof data === 'string') {
      const match = data.match(/^.*?: (.+)$/m); // Pega primeira linha com formato "TipoExcecao: Mensagem"
      if (match) message = match[1];
    } else if (data?.message) {
      message = data.message;
    }
    throw new Error(message);
  }

  return data;
}
