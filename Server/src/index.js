import '@babel/polyfill';
import React from 'react';
import ReactDOM from 'react-dom';
import App from './js/App.jsx';

const wrapper = document.getElementById('app');
wrapper ? ReactDOM.render(<App />, wrapper) : false;