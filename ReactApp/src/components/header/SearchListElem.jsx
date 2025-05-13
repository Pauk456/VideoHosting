import React from 'react';

const SearchListElem = (props) => {
    return (
        <li className="search-item hover-effect">
            <a className="search-dropdown-item">
                <img className="search-icon" src={props.animeInfo.imgUrl} alt="" />
                <p className="search-text elem-text-medium">{props.animeInfo.title}</p>
            </a>
        </li>
    );
};

export default SearchListElem;