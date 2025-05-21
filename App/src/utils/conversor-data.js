export function validarData(data) {
  const regex = /^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[0-2])\/\d{4}$/;
  return regex.test(data);
}

export function formatarData(data) {
  if (!data) return "";

  if (validarData(data)) {
    const [dia, mes, ano] = data.split("/");
    return `${dia.padStart(2, "0")}/${mes.padStart(2, "0")}/${ano}`;
  }

  const d = new Date(data);
  if (!isNaN(d)) {
    return d.toLocaleDateString("pt-BR", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
    });
  }

  return "";
}
