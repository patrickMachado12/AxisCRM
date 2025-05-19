<template>
	<button @click="callback" color="primary" dark class="mb-2">{{value}}</button>
</template>

<script>
export default {
  name:'ButtonPrimary',
	props: {
		value: { type: String, required: true },
		callback: { type: Function, default: () => {} }
	}
}
</script>

<style scoped>
	ButtonPrimary {
		width: 100%;
		background-color: var(--cor-primaria);
		color: #FFF;
		padding: 8px 20px;
		margin: 8px 0;
		border: none;
		border-radius: 4px;
		cursor: pointer;
	}

	ButtonPrimary:hover{
		background-color: var(--cor-secundaria);
	}
</style>