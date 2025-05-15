import React, {useEffect, useRef, useState} from 'react';
import RecommendAnime from "../components/mainPage/RecommendAnime.jsx";

const ResizeRecommends = () => {
    const containerRef = useRef(null);
    // const posterRef = useRef(null);
    const [animes, setAnimes] = useState([]);

    const idList =  [8500,8644,8700,8896,8646,8648];
    const viewport = window.innerWidth;
    // сколько элементов помещается в одну строку
    const [cols, setCols] = useState(0);

    useEffect(() => {
        fetch(`http://localhost:5006/api/title/all`)
            .then(res => res.json())
            .then(animes => setAnimes(animes));
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
        <ul className="recommend-anime-list" ref={containerRef}>
                {animes.slice(0, cols).map((anime) => (
                    <RecommendAnime animeInfo={{title: anime.name, imgUrl: `http://localhost:5001/api/img/${anime.seriesId}`, id: anime.seriesId}} />
                ))}
        </ul>
    );
            {/* При желании можно вывести отладочную информацию */}
            {/* <p>Показываем {cols} элементов из {animes.length}</p> */}
};

export default ResizeRecommends;