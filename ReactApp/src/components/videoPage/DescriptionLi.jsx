import React from 'react';

const DescriptionLi = (props) => {
    return (
        <li className="description-elem">
            <p className="info-label elem-text-medium"> {props.name}</p>
            <p className="info-arg  elem-text-small">{props.value}</p>
        </li>
    );
};

export default DescriptionLi;