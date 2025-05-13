import React, {useEffect, useState} from 'react';
import RecommendAnime from "../components/mainPage/RecommendAnime.jsx";

const GetRecommends = (props) => {
    const [animes, setAnimes] = useState([]);
    const limit = props.limit;
    const idList =  [8500,8644,8700,8645,8646,8648,8998];
    useEffect(() => {

        fetch(`https://api.anilibria.tv/v3/title/list?id_list=${idList.join(",")}&filter=names.ru,id,posters.original.url`)
            .then(res => res.json())
            .then(data => setAnimes(data))

    }, []);

    return (
        animes.slice(0, limit).map((anime) => (
            <RecommendAnime animeInfo={{title: anime.names.ru, imgUrl: `https://anilibria.tv/${anime.posters.original.url}`, id: anime.id}} />
        ))
    );
};

export default GetRecommends;