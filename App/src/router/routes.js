import Login from '@/views/Login.vue'
import ControleCliente from '@/views/ControleCliente.vue';
import ControleUsuario from '@/views/ControleUsuario.vue';
import ControleAdministrador from '@/views/ControleAdministrador.vue';
import ControleAtendimento from '@/views/ControleAtendimento.vue';

export default [
	{
		path: '/login',
		name: 'Login',
		component: Login,
		title: 'Login',
		meta: { layout: 'auth', requiredAuth: false }
	},
	{
    path: '/',
    redirect: '/login'
  },
	{
		path: '/administrador',
		name: 'Administrador',
		component: ControleAdministrador,
		title: 'Administrador',
		meta: { layout: 'auth', requiredAuth: true }
	},
	{
		path: '/atendimento',
		name: 'Atendimento',
		component: ControleAtendimento,
		title: 'Atendimento',
		meta: { layout: 'auth', requiredAuth: true }
	},
	{
		path: '/cliente',
		name: 'ControleCliente',
		component: ControleCliente,
		title: 'Cadastro de Cliente',
		meta: { layout: 'auth', requiredAuth: true }
	},
	{
		path: '/usuario',
		name: 'ControleUsuario',
		component: ControleUsuario,
		title: 'Cadastro de Usuario',
		meta: { layout: 'auth', requiredAuth: true }
	},
];