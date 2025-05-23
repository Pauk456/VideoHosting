import React, {useEffect, useState} from 'react';

import BigListElem from "../components/mainPage/BigListElem.jsx";

const GetRecommends = () => {
    const [animes, setAnimes] = useState([]);

    useEffect(() => {
        async function fetchAnimeData() {
            const animeList = await fetch('http://localhost:5006/api/title/all')
                .then(res => res.json());
            const animeData = await Promise.all(
                animeList.map(async anime => {
                    const cfg = await fetch(`http://localhost:5006/api/title/getConfig/${anime.id}`)
                        .then(res => res.json());
                    return {
                        id: anime?.id,
                        name: anime?.name,           // если есть поле name
                        description: cfg?.description,
                        year: cfg?.year,
                        studio: cfg?.year,

                    };
                })
            );
            setAnimes(animeData);
        }
        fetchAnimeData();
    }, []);

    return (
        animes.map((anime) => (
            <BigListElem animeInfo={{title: anime.name, year: anime.year, imgUrl: `http://localhost:5001/api/img/${anime.id}`, id: anime.id, description: anime.description}} />
        ))
    );
};

export default GetRecommends;