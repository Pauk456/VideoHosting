import React from 'react';
import {getAnimeUrl} from "../../utils/UrlFormat.jsx";
import {Link} from "react-router-dom";

const SearchListElem = (props) => {
    return (
        <li className="search-item">
            <Link className="search-dropdown-item hover-effect" to={getAnimeUrl(props.animeInfo.title, props.animeInfo.id)}>
                <img className="search-icon" src={props.animeInfo.imgUrl} alt="" />
                <p className="search-text elem-text-medium">{props.animeInfo.title}</p>
            </Link>
        </li>
    );
};

export default SearchListElem;