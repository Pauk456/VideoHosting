import React from 'react';
import {Link} from "react-router-dom";
import {getAnimeUrl} from "../../utils/UrlFormat.jsx";

const ActualAnime = (props) => {
    var type = props.animeInfo?.type?.toUpperCase() || ''
    const episodes = parseInt(props.animeInfo.episodes)
    if (type === "TV" && episodes > 1) {
        type = "TV-сериал"
    }
    else if (type === "MOVIE" || episodes === 1) {
        type = "Фильм"
    }
    return (
        <Link className="actual-anime-elem hover-effect" to={getAnimeUrl(props.animeInfo.title, props.animeInfo.id)}>
            <img className="actual-poster-img" src={props.animeInfo.imgUrl} alt=""/>
            <div className="actual-description-container">
                <p className="actual-title elem-text-medium">{props.animeInfo.title}</p>
                <div className="elem-text-small actual-type">
                    <p className="format type-text">{type}</p>
                    {type === "TV-сериал" && (
                        <p className="serias type-text">{episodes} серий</p>
                    )}
                </div>
            </div>
        </Link>
    );
};

export default ActualAnime;