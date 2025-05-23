export function formatarTipoPessoa(valor) {
  return valor === 1
    ? 'Física'
    : valor === 2
      ? 'Jurídica'
      : '—';
}