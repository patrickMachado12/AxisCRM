import api from "@/services/api";
import utilsStorage from "@/utils/storage";

export default {
  namespaced: true,
  state: () => ({
    token: utilsStorage.obterTokenNaStorage() || null,
  }),
  getters: {
    isLoggedIn: (state) => !!state.token,
  },
  mutations: {
    SET_TOKEN(state, token) {
      state.token = token;
      utilsStorage.salvarTokenNaStorage(token);
    },
    CLEAR_TOKEN(state) {
      state.token = null;
      utilsStorage.removerTokenDaStorage();
    },
  },
  actions: {
    async login({ commit }, { email, senha }) {
      const { data } = await api.post("/usuarios/login", { email, senha });
      commit("SET_TOKEN", data.token);
    },
    logout({ commit }) {
      commit("CLEAR_TOKEN");
    },
  },
};
