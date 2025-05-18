import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'

// TODO: Рейтинг на аниме постерах главной страницы
//  Красивая адаптивность Хэдера
//  Тест выпадающего списка окна поиска
//  Окно регистрации
//  Улучшение видеоплеера
//  Бесконечный список аниме на главной странице



import App from './App.jsx'

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <App />
  </StrictMode>,
)


