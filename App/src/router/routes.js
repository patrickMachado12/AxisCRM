import Login from '@/views/Login.vue'
import ControleCliente from '@/views/ControleCliente.vue';
import ControleUsuario from '@/views/ControleUsuario.vue';
import Dashboard from '@/views/ControleDashboard.vue';
import ControleAdministrador from '@/views/ControleAdministrador.vue';
import ControleAtendimentos from '@/views/ControleAtendimentos.vue';

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
		path: '/dashboard',
		name: 'Dashboard',
		component: Dashboard,
		title: 'Dashboard',
		meta: { layout: 'auth', requiredAuth: true }
	},
	{
		path: '/administrador',
		name: 'Administrador',
		component: ControleAdministrador,
		title: 'Administrador',
		meta: { layout: 'auth', requiredAuth: true }
	},
	{
		path: '/atendimentos',
		name: 'Atendimentos',
		component: ControleAtendimentos,
		title: 'Atendimentos',
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