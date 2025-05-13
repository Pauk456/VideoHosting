// https://api.anilibria.tv/v3/title/updates?limit=5

import React, {useEffect, useState} from 'react';
import RecommendAnime from "../components/mainPage/RecommendAnime.jsx";
import OngoingElem from "../components/mainPage/OngoingElem.jsx";

const GetRecommends = () => {
    const [animes, setAnimes] = useState([]);

    useEffect(() => {
        fetch('https://api.anilibria.tv/v3/title/updates?limit=7&filter=names.ru,id,posters.small.url,player.episodes')
            .then(res => res.json())
            .then(data => setAnimes(data.list))

    }, []);

    return (
        animes.map((anime) => (
            <OngoingElem animeInfo={{title: anime.names.ru, imgUrl: `https://anilibria.tv/${anime.posters.small.url}`, id: anime.id, episodes: 2}} />
        ))
    );
};

export default GetRecommends;