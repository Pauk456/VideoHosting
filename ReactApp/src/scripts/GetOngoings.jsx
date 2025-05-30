// https://api.anilibria.tv/v3/title/updates?limit=5

import React, {useEffect, useState} from 'react';
import OngoingElem from "../components/mainPage/OngoingElem.jsx";

const GetOngoings = () => {
    const [animes, setAnimes] = useState([]);

    useEffect(() => {
        fetch(`http://localhost:5004/api/getManyTitles/filter=recent`)
            .then(res => res.json()).then(data => {setAnimes(data.recent)})
    }, []);

    return animes.map((anime) => {
        const totalEpisodes = anime.seasons?.reduce((sum, season) => sum + (season.episodes?.length || 0), 0);

        return (
                <OngoingElem animeInfo={{title: anime.titleName, imgUrl: `http://localhost:5001/api/img/${anime.idTitle}`, id: anime.idTitle, episodes: totalEpisodes}} />
        );
    });
};

export default GetOngoings;