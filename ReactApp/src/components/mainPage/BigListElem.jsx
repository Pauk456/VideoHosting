import React from 'react';
import {Link} from "react-router-dom";
import {getAnimeUrl} from "../../utils/UrlFormat.jsx";

const BigListElem = (props) => {
    return (
        <li className="big-list-elem">
            <Link to={getAnimeUrl(props.animeInfo.title, props.animeInfo.id)}>
                <div className="big-list-elem-container hover-effect">
                    <img className="big-list-poster-img" src={props.animeInfo.imgUrl} alt=""/>
                    <div className="anime-description-container">
                        <p className="anime-link elem-text-big">{props.animeInfo.title}</p>
                        <p className="elem-text-medium">{props.animeInfo.titleEng}</p>
                        <p className="story-description elem-text-small">{props.animeInfo.description}</p>
                    </div>

                </div>
            </Link>
        </li>
    );
};

export default BigListElem;