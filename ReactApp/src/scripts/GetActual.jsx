import React, {useEffect, useState} from 'react';
import RecommendAnime from "../components/mainPage/RecommendAnime.jsx";
import ActualAnime from "../components/mainPage/ActualAnime.jsx";

const GetRecommends = () => {
    const [animes, setAnimes] = useState([]);

    useEffect(() => {
        fetch('https://api.anilibria.tv/v3/title/list?id_list=8500,8644,8700,8645,8646,8648,8896,8998&filter=names.ru,id,posters.medium.url,type.string,type.episodes')
            .then(res => res.json())
            .then(data => setAnimes(data))
    }, []);

    return (
        animes.map((anime) => (
            <ActualAnime animeInfo={{title: anime.names.ru, imgUrl: `https://anilibria.tv/${anime.posters.medium.url}`, id: anime.id, type : anime.type.string, episodes: anime.type.episodes}} />
        ))
    );
};

export default GetRecommends;