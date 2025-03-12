import { createContext} from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'

import 'bootstrap/dist/css/bootstrap.min.css'
import UserStore from './stores/UserStore.ts'
import { BrowserRouter } from 'react-router-dom'

export const Context = createContext<{ userStore: UserStore } | null>(null);


createRoot(document.getElementById('root')!).render(
  <Context.Provider value={{
    userStore: new UserStore()
  }}>
  <BrowserRouter>
    <App />
  </BrowserRouter>
  </Context.Provider>,
)