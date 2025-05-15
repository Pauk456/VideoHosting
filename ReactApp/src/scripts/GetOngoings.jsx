// https://api.anilibria.tv/v3/title/updates?limit=5

import React, {useEffect, useState} from 'react';
import OngoingElem from "../components/mainPage/OngoingElem.jsx";

const GetOngoings = () => {
    const [animes, setAnimes] = useState([]);

    useEffect(() => {
        fetch(`http://localhost:5006/api/title/getRecentEpisodes`)
            .then(res => res.json())
            .then(listData => {
                const idList = listData.map(item => item.titleId);
                return Promise.all(
                    idList.map(id =>
                        fetch(`http://localhost:5006/api/title/getAnimeName/${id}`)
                            .then(res => res.json())
                            .then(detail => ({
                                id,
                                title: detail.name,
                                episodes: detail.episodeNumber,
                            }))
                    )
                );
            }).then(animes => setAnimes(animes));
    }, []);

    return (
        animes.map((anime) => (
            <OngoingElem animeInfo={{title: anime.title, imgUrl: `http://localhost:5001/api/img/${anime.id}`, id: anime.episodes}} />
        ))
    );
};

export default GetOngoings;