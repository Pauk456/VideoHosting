import { useState } from 'react'
import '../css/main.css'
import Header from "../components/header/Header.jsx";
import RecommendAnime from "../components/mainPage/RecommendAnime.jsx";
import ActualAnime from "../components/mainPage/ActualAnime.jsx";
import GetRecommends from "../scripts/GetRecommends.jsx";
import OngoingElem from "../components/mainPage/OngoingElem.jsx";
import BigListElem from "../components/mainPage/BigListElem.jsx";
import ResizeRecommends from "../scripts/ResizeRecommends.jsx";
import GetActual from "../scripts/GetActual.jsx";
import GetBigList from "../scripts/GetBigList.jsx";
import GetOngoings from "../scripts/GetOngoings.jsx";
import ResizeActual from "../scripts/ResizeActual.jsx";


function HomePage() {
  const [count, setCount] = useState(0)

  return (
      <div >
        <Header />
        <main>
          <div className="recommends-container slider container">
              <p className="container-title container-title-text">Рекомендованное</p>

             <ResizeRecommends />
          </div>
          <div className="actual-container container">
              <p className="container-title container-title-text">Смотрят сейчас</p>
              < ResizeActual />
          </div>

          <div className="big-anime-container container">
              <ul className="big-list container">
                  <GetBigList />
              </ul>
              <div className="ongoings-container container">
                  <p className="ongoings-title container-title-text">Недавние</p>
                  <ul className="ongoing-list">
                      <GetOngoings />
                  </ul>
              </div>
          </div>
        </main>
      </div>
      )
      }

      export default HomePage
