import React, {useEffect, useRef, useState} from 'react';
import SearchListElem from "./SearchListElem.jsx";
import {data} from "react-router-dom";
import RegistrationForm from "./RegistrationForm.jsx";

const Header = () => {
    const searchInputRef = useRef(null);
    const [showDroplist, setShowDroplist] = useState(false)
    const [dropList, setDroplist] = useState([{title: "Lazarus", imgUrl: "/images/lazarus.jpg"}, {title: "Re:Zero", imgUrl: "/images/rezero.jpg"}])
    const [inputText, setInputText] = useState('')
    const [showRegistration, setShowRegistration] = useState(false);

    // http://localhost:5004/api/titleTag={inputText}&filter=TitleId
    const handleSearchToggle = () => {
        if (searchInputRef.current) {
            if (searchInputRef.current.style.display === 'none') {
                searchInputRef.current.style.display = 'block';
                // setShowDroplist(true);

            }
            else {
                searchInputRef.current.style.display = 'none';
                setShowDroplist(false);
                setInputText('');
                searchInputRef.current.value = '';
            }
        }
    };


    useEffect(() => {
        fetch(`http://localhost:5004/api/titleTag=${inputText}&filter=TitleId,TitleName`)
            .then(response => {
                if (!response.ok) {
                    setDroplist([]);
                    setShowDroplist(false);
                    throw new Error(`Ошибка HTTP: ${response.status} ${response.statusText}`);
                }
                return response.json();
            }).then(data => {
                setShowDroplist(true);
                setDroplist([{id: data.id, title: data.name, imgUrl: `http://localhost:5001/api/img/${data.id}`}])
            }
        )
        // Запрос на поиск аниме
        // Обновление списка аниме
    }, [inputText]);

    return (
        <header>
            {showRegistration && <div className="overlay" onClick={() => setShowRegistration(false)}></div>}
            {showRegistration && <RegistrationForm onClose={() => setShowRegistration(false)} />}
            <div className="header-container container">
                <div className="logo-container">
                    <a className="logo" href="/">AniWorld</a>
                </div>
                <div className="search-container">
                    <input ref={searchInputRef} type="text" className="search-input elem-text-medium" onChange={e => setInputText(e.target.value)} placeholder="Поиск..." style={{display:"none"}}/>
                    {
                        showDroplist && <div className="search-dropdown">
                            <ul className="search-dropdown-list">
                                {dropList.map((anime, idx) => (
                                    <SearchListElem
                                        key={idx}
                                        animeInfo={{id: anime.id, title: anime.title, imgUrl: anime.imgUrl }}
                                    />
                                ))}

                            </ul>
                        </div>
                    }
                    <button onClick={handleSearchToggle} className="header-search-button"></button>

                </div>


                <button className="header-auth-button"  onClick={() => setShowRegistration(true)}>
                    <div className="auth-button-icon"></div>
                    <p className="auth-button-text container-title-text">Войти</p>
                </button>
            </div>
        </header>
    );
};

export default Header;