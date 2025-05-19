function validarData(data) {
  const regex = /^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[0-2])\/\d{4}$/;
  return regex.test(data);
}

function formatarData(data) {
  if (!data) return '';
  const [dia, mes, ano] = data.split('/');
  if (validarData(data)) {
    return `${dia.padStart(2, '0')}/${mes.padStart(2, '0')}/${ano}`;
  }
  return '';
}

export default {
  formatarData,
  validarData
}