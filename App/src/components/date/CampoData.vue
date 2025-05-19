<template>
  <v-text-field
    v-model="dataFormatada"
    :label="label"
    :rules="rules"
    placeholder="DD/MM/AAAA"
    v-mask="'##/##/####'"
  ></v-text-field>
</template>

<script>
import moment from 'moment';

export default {
  name: 'CampoData',
  props: {
    value: {
      type: String,
      required: false,
      default: '',
    },
    label: {
      type: String,
      required: false,
      default: 'Data',
    },
    rules: {
      type: Array,
      required: false,
      default: () => [],
    }
  },
  computed: {
    dataFormatada: {
      get() {
        return this.value ? moment(this.value).format('DD/MM/YYYY') : '';
      },
      set(newValue) {
        const date = moment(newValue, 'DD/MM/YYYY', true);
        const formattedValue = date.isValid() ? date.toISOString() : '';
        this.$emit('input', formattedValue);
      },
    },
  },

};
</script>