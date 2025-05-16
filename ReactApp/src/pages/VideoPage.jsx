import React, {useEffect, useRef, useState} from 'react';
import Header from "../components/header/Header.jsx";

import '../css/video.css'
import DescriptionLi from "../components/videoPage/DescriptionLi.jsx";
import Comment from "../components/videoPage/Comment.jsx";
import Video from "../components/videoPage/Video.jsx";
import {data, useParams} from "react-router-dom";


const VideoPage = () => {
    const { title, id } = useParams();
    const [animeData, setAnimeData] = useState(null);
    const [showBigDescription, setShowBigDescription] = useState(false);


    const [comments, setComments] = useState([
        { username: "Гигачад", text: "ГОЙДА!" },
        { username: "ТраВоМаН", text: "Мои стримы лучше" },
        { username: "Мисаки-чан", text: "Lorem ipsum dolor sit amet consectetur adipiscing elit. Quisque faucibus ex sapien vitae pellentesque sem placerat. In id cursus mi pretium tellus duis convallis. Tempus leo eu aenean sed diam urna tempor. Pulvinar vivamus fringilla lacus nec metus bibendum egestas. Iaculis massa nisl malesuada lacinia integer nunc posuere. Ut hendrerit semper vel class aptent taciti sociosqu. Ad litora torquent per conubia nostra inceptos himenaeos.                                   Lorem ipsum dolor sit amet consectetur adipiscing elit. Quisque faucibus ex sapien vitae pellentesque sem placerat. In id cursus mi pretium tellus duis convallis. Tempus leo eu aenean sed diam urna tempor. Pulvinar vivamus fringilla lacus nec metus bibendum egestas. Iaculis massa nisl malesuada lacinia integer nunc posuere. Ut hendrerit semper vel class aptent taciti sociosqu. Ad litora torquent per conubia nostra inceptos himenaeos\n" },
    ]);

    const [newText, setNewText] = useState('');
    const [isInputVisible, setInputVisible] = useState(false);

    const buttonLabel = isInputVisible ? 'Отправить' : 'Написать комментарий';

    useEffect(() => {
        async function fetchAnimeData() {
            const nameRes = await fetch(`http://localhost:5006/api/title/getAnimeName/${id}`);
            const nameJson = await nameRes.json();
            const seasonsRes = await fetch(`http://localhost:5006/api/title/getSeasonsAndEpisodes/${id}`);
            const seasonsJson = await seasonsRes.json();
            setAnimeData({
                name: nameJson.name,
                seasons: seasonsJson,
            });
        }
        fetchAnimeData();
        // fetch(`http://localhost:5006/api/title/getAnimeName/${id}`)
        //     .then(res => res.json())
        //     .then(data => {
        //         const name = data.name;
        //         return Promise.all(
        //             fetch(`http://localhost:5006/api/title/getSeasonsAndEpisodes/${id}`)
        //                 .then(res => res.json())
        //                 .then(data => ({
        //                     name: name,
        //                     seasons: data
        //                 }))
        //         )
        //
        //     })
        //     .then(data => setAnimeData(data))
    }, [id]);

    useEffect(() => {
        const handleResize = () => {
            // при каждом изменении размера проверяем условие и обновляем состояние
            setShowBigDescription(window.innerWidth < 900);
        };

        // сразу выставим нужное значение на монтировании
        handleResize();

        // вешаем слушатель
        window.addEventListener('resize', handleResize);
        return () => {
            // не забываем убирать слушатель
            window.removeEventListener('resize', handleResize);
        };
    }, [])

    const handleButtonClick = () => {
        if (!isInputVisible) {
            // Показываем поле ввода
            setInputVisible(true);
        } else {
            // Если поле уже показано — отправляем комментарий
            if (newText.trim()) {
                setComments([
                    { username: "Anonimous", text: newText.trim() },
                    ...comments,
                ]);
                setNewText('');
            }
            // Скрываем поле ввода
            setInputVisible(false);
        }
    };

    if (!animeData) {
        return (
            <div>
                <Header />
                <main className="container">
                    <p>Загрузка данных…</p>
                </main>
            </div>
        );
    }
    // const eps1 = listObj["1"];           // найдёт свойство "1"
    // const url = eps1.hls.hd;            // достанет нужный URL
    return (
        <div >
           < Header />
            <main>
                <div className="description-container container">
                    <img className="poster-img" src={`http://localhost:5001/api/img/${id}`} alt=""/>
                    <div className="description">
                        <p className="container-title-text" id="title">{animeData.name}</p>

                        <ul className="description-list">
                            < DescriptionLi name = "Студия" value = "2x2"/>
                            < DescriptionLi name = "Год выпуска" value = "2025"/>
                            < DescriptionLi name = "Жанры" value = "Детектив, Историческое, Романтика"/>
                            {!showBigDescription && < DescriptionLi name = "Описание" value = "В процессе"/>}
                        </ul>
                    </div>
                </div>
                {showBigDescription && <div className={"second-description-container container"}>
                    <p className="elem-text-medium big-description-container-title">Описание</p>
                    <p className={"elem-text-small big-description-text"}>"В процессе"</p>
                </div>}

                <Video seasons={animeData.seasons}/>
                <div className="comments-container container">
                    <p className="comments-container-title container-title-text">Комментарии</p>
                    {isInputVisible && <textarea className="input-comment elem-text-small regular-font-weight"
                                                 onChange={e => setNewText(e.target.value)}></textarea>}
                    <button onClick={handleButtonClick} className="comment-button elem-text-medium">{buttonLabel}</button>
                    <ul className="comments-list-container">
                        {comments.map((c, idx) => (
                            <Comment
                                key={idx}
                                username={c.username}
                                commentText={c.text}
                            />
                        ))}

                    </ul>
                </div>
            </main>
        </div>
    );
};

export default VideoPage;