<template>
  <v-container fluid>
    <!-- Título -->
    <v-row>
      <v-col cols="12">
        <h2 class="titulo">Dashboard de Atendimentos</h2>
      </v-col>
    </v-row>

    <!-- Filtro de período -->
    <v-row justify="end" class="mb-6">
      <v-col cols="12" md="3">
        <v-menu
          v-model="menuDataInicial"
          :close-on-content-click="false"
          transition="scale-transition"
          offset-y
          min-width="auto"
        >
          <template #activator="{ props }">
            <v-text-field
              v-model="formatarDataInicial"
              label="Data Inicial"
              prepend-icon="mdi-calendar"
              readonly
              v-bind="props"
            />
          </template>
          <v-date-picker
            v-model="filters.dataInicial"
            locale="pt-br"
            @update:model-value="onDataInicialChange"
          />
        </v-menu>
      </v-col>

      <v-col cols="12" md="3">
        <v-menu
          v-model="menuDataFinal"
          :close-on-content-click="false"
          transition="scale-transition"
          offset-y
          min-width="auto"
        >
          <template #activator="{ props }">
            <v-text-field
              v-model="formatarDataFinal"
              label="Data Final"
              prepend-icon="mdi-calendar"
              readonly
              v-bind="props"
            />
          </template>
          <v-date-picker
            v-model="filters.dataFinal"
            locale="pt-br"
            @update:model-value="onDataFinalChange"
          />
        </v-menu>
      </v-col>

      <v-col cols="12" md="2" class="d-flex align-center">
        <v-btn color="primary" @click="filtrar">
          Filtrar
        </v-btn>
      </v-col>
    </v-row>    
  </v-container>
</template>

<script setup>

import atendimentoService from '@/services/atendimento-service.js'

</script>

<style scoped>
.titulo {
  margin-bottom: 16px;
}
</style>
