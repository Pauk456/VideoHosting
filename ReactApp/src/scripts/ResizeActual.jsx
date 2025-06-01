import React, {useEffect, useRef, useState} from 'react';

import ActualAnime from "../components/mainPage/ActualAnime.jsx";

const ResizeRecommends = () => {
    const containerRef = useRef(null);
    // const posterRef = useRef(null);
    const [animes, setAnimes] = useState([]);

    const idList =  [8500,8644,8700,8645,8646,8648,8896,8998];

    // сколько элементов помещается в одну строку
    const [cols, setCols] = useState(idList.length);

    useEffect(() => {
        fetch(`http://193.53.126.242:5006/api/title/all`)
            .then(res => res.json())
            .then(setAnimes)
    }, []);

    // измеряем при монтировании и на ресайзе
    useEffect(() => {
        const container = containerRef.current;
        if (!container) return;

        const updateCols = () => {
            const gridStyle = getComputedStyle(container);

            // Вариант 1: Через grid-template-columns
            const templateColumns = gridStyle.gridTemplateColumns;
            const columnsCount = templateColumns.split(' ').length;
            setCols(columnsCount);
        };

        const observer = new ResizeObserver(updateCols);
        observer.observe(container);

        return () => observer.disconnect();
    }, []);

    return (
        <div className="actual-grid-container" ref={containerRef}>
            {animes.slice(0, cols * 2).map((anime) => (
                <ActualAnime animeInfo={{title: anime.name, imgUrl: `http://193.53.126.242:5001/api/img/${anime.seriesId}`, id: anime.seriesId, type : "tv", episodes: 12}} />
            ))}
        </div>
    );
    {/* При желании можно вывести отладочную информацию */}
    {/* <p>Показываем {cols} элементов из {animes.length}</p> */}
};

export default ResizeRecommends;