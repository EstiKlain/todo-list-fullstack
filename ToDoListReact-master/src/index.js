import React from 'react';
import { createRoot } from 'react-dom/client'; // ייבוא ה-API החדש
import App from './App';

const container = document.getElementById('root');
const root = createRoot(container); // יצירת השורש בצורה החדשה

root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);