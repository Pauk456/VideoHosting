import React, {useEffect, useRef, useState} from 'react';
import SearchListElem from "./SearchListElem.jsx";

const Header = () => {
    const searchInputRef = useRef(null);
    const [showDroplist, setShowDroplist] = useState(false)
    const [dropList, setDroplist] = useState([{title: "Lazarus", imgUrl: "/images/lazarus.jpg"}, {title: "Re:Zero", imgUrl: "/images/rezero.jpg"}])
    const [inputText, setInputText] = useState('')

    const handleSearchToggle = () => {
        if (searchInputRef.current) {
            if (searchInputRef.current.style.display === 'none') {
                searchInputRef.current.style.display = 'block';
                setShowDroplist(true);
            }
            else {
                searchInputRef.current.style.display = 'none';
                setShowDroplist(false);
            }
        }
    };


    useEffect(() => {
        // Запрос на поиск аниме
        // Обновление списка аниме
    }, [inputText]);

    return (
        <header>
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
                                        animeInfo={{ title: anime.title, imgUrl: anime.imgUrl }}
                                    />
                                ))}

                            </ul>
                        </div>
                    }
                    <button onClick={handleSearchToggle} className="header-search-button"></button>

                </div>


                <a className="header-auth-button">
                    <button className="auth-button-icon"></button>
                    <p className="auth-button-text container-title-text">Войти</p>
                </a>
            </div>
        </header>
    );
};

export default Header;