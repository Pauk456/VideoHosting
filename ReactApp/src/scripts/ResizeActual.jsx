import React, {useEffect, useRef, useState} from 'react';
import GetRecommends from "./GetRecommends.jsx";
import RecommendAnime from "../components/mainPage/RecommendAnime.jsx";
import ActualAnime from "../components/mainPage/ActualAnime.jsx";

const ResizeRecommends = () => {
    const containerRef = useRef(null);
    // const posterRef = useRef(null);
    const [animes, setAnimes] = useState([]);

    const idList =  [8500,8644,8700,8645,8646,8648,8896,8998];

    // сколько элементов помещается в одну строку
    const [cols, setCols] = useState(idList.length);

    useEffect(() => {
        fetch(`https://api.anilibria.tv/v3/title/list?id_list=${idList.join(",")}&filter=names.ru,id,posters.medium.url,type.string,type.episodes`)
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
                <ActualAnime animeInfo={{title: anime.names.ru, imgUrl: `https://anilibria.tv/${anime.posters.medium.url}`, id: anime.id, type : anime.type.string, episodes: anime.type.episodes}} />
            ))}
        </div>
    );
    {/* При желании можно вывести отладочную информацию */}
    {/* <p>Показываем {cols} элементов из {animes.length}</p> */}
};

export default ResizeRecommends;