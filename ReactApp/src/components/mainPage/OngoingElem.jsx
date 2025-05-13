import React from 'react';
import {getAnimeUrl} from "../../utils/UrlFormat.jsx";
import {Link} from "react-router-dom";

const OngoingElem = (props) => {
    return (
        <li>
            <Link to={getAnimeUrl(props.animeInfo.title, props.animeInfo.id)} >
                <div className="ongoing-elem-container hover-effect">
                    <img className="ongoing-poster-img" src={props.animeInfo.imgUrl} alt=""/>
                    <div className="ongoing-description-container">
                        <p className="ongoing-title elem-text-medium">{props.animeInfo.title}</p>
                        <p className="elem-text-medium series-number">{props.animeInfo.episodes} Серии</p>
                    </div>
                </div>
            </Link>
        </li>
    );
};

export default OngoingElem;