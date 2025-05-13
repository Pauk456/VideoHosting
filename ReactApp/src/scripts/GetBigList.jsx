import React, {useEffect, useState} from 'react';
import RecommendAnime from "../components/mainPage/RecommendAnime.jsx";
import ActualAnime from "../components/mainPage/ActualAnime.jsx";
import BigListElem from "../components/mainPage/BigListElem.jsx";

const GetRecommends = () => {
    const [animes, setAnimes] = useState([]);

    useEffect(() => {
        fetch('https://api.anilibria.tv/v3/title/list?id_list=8500,8644,8700,8645,8646,8648&filter=names.ru,names.en,description,id,posters.original.url')
            .then(res => res.json())
            .then(data => setAnimes(data))

    }, []);

    return (
        animes.map((anime) => (
            <BigListElem animeInfo={{title: anime.names.ru, titleEng: anime.names.en, imgUrl: `https://anilibria.tv/${anime.posters.original.url}`, id: anime.id, description: anime.description}} />
        ))
    );
};

export default GetRecommends;