import React, {useEffect, useState} from 'react';

import BigListElem from "../components/mainPage/BigListElem.jsx";

const GetRecommends = () => {
    const [animes, setAnimes] = useState([]);

    useEffect(() => {
         fetch('http://193.53.126.242:5004/api/getManyTitles/filter=all')
            .then(res => res.json()).then(data => setAnimes(data.all));

    }, []);

    return (
        animes.map((anime) => (
            <BigListElem animeInfo={{title: anime.titleName, year: anime.description.year, imgUrl: `http://193.53.126.242:5001/api/img/${anime.idTitle}`, id: anime.idTitle, description: anime.description.description}} />
        ))
    );
};

export default GetRecommends;