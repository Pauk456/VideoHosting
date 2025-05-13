import React, {useEffect, useRef, useState} from 'react';
import GetRecommends from "./GetRecommends.jsx";
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
        fetch(`https://api.anilibria.tv/v3/title/list?id_list=${idList.join(",")}&filter=names.ru,id,posters.original.url`)
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
        <ul className="recommend-anime-list" ref={containerRef}>
                {animes.slice(0, cols).map((anime) => (
                    <RecommendAnime animeInfo={{title: anime.names.ru, imgUrl: `https://anilibria.tv/${anime.posters.original.url}`, id: anime.id}} />
                ))}
        </ul>
    );
            {/* При желании можно вывести отладочную информацию */}
            {/* <p>Показываем {cols} элементов из {animes.length}</p> */}
};

export default ResizeRecommends;