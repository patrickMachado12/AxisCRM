export function validarData(data) {
  const regex = /^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[0-2])\/\d{4}$/;
  return regex.test(data);
}

export function formatarData(data) {
  if (!data) return "";

  if (data instanceof Date) {
    return data.toLocaleDateString("pt-BR", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
    });
  }

  if (validarData(data)) {
    const [dia, mes, ano] = data.split("/");
    return `${dia.padStart(2, "0")}/${mes.padStart(2, "0")}/${ano}`;
  }

  const d = new Date(data);

  if (!isNaN(d.getTime())) {
    return d.toLocaleDateString("pt-BR", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
    });
  }

  return "";
}

export function toIsoDate(display) {
  if (!validarData(display)) return null;
  const [dia, mes, ano] = display.split("/");
  return `${ano}-${mes.padStart(2, "0")}-${dia.padStart(2, "0")}`;
}
