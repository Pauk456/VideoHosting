import { useState } from 'react'
import HomePage from "./pages/HomePage.jsx";
import VideoPage from "./pages/VideoPage.jsx";
import {Route, BrowserRouter, Routes} from "react-router-dom";


function App() {
  const [count, setCount] = useState(0)

  return (
      <BrowserRouter>
          <Routes>
              <Route path="/" element={<HomePage />} />
              <Route path="/anime/:title/:id" element={<VideoPage />} />
          </Routes>
      </BrowserRouter>
      )
    }

      export default App
