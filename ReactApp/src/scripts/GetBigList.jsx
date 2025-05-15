import React, {useEffect, useState} from 'react';

import BigListElem from "../components/mainPage/BigListElem.jsx";

const GetRecommends = () => {
    const [animes, setAnimes] = useState([]);

    useEffect(() => {
        fetch('http://localhost:5006/api/title/all')
            .then(res => res.json())
            .then(data => setAnimes(data))

    }, []);

    return (
        animes.map((anime) => (
            <BigListElem animeInfo={{title: anime.name, titleEng: "Not available", imgUrl: `http://localhost:5001/api/img/${anime.id}`, id: anime.id, description: "Очень крутое аниме, да-да"}} />
        ))
    );
};

export default GetRecommends;