import axios from "axios";
import utilsStorage from "@/utils/storage";

const api = axios.create({
  baseURL: "https://localhost:7021",
});

api.interceptors.request.use((config) => {
  const token = utilsStorage.obterTokenNaStorage();
  if (token) {
    config.headers.Authorization = token;
  }
  return config;
});

export default api;
