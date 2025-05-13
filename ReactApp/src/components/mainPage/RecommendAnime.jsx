import React from 'react';
import {Link} from "react-router-dom";
import {getAnimeUrl} from "../../utils/UrlFormat.jsx";

const RecommendAnime = (props) => {
    return (
        <li className="recommend-anime-elem hover-effect">
            <Link to={getAnimeUrl(props.animeInfo.title, props.animeInfo.id)}>
                <img className="recommend-poster-img" src={props.animeInfo.imgUrl} alt=""/>
                <p className="recommend-anime-link elem-text-big anime-link">{props.animeInfo.title}</p>
            </Link>
        </li>
    );
};

export default RecommendAnime;