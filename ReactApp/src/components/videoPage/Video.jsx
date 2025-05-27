// src/components/videoPage/Video.jsx
import React, {useEffect, useRef, useState} from 'react';

const Video = ({seasons, timing}) => {
    const initialSeason = seasons?.[0]?.seasonNumber || null;

    const videoRef = useRef(null);
    // По умолчанию первая серия
    const [selectedSeason, setSelectedSeason] = useState(initialSeason);
    const [episodeList, setEpisodeList] = useState(
        seasons?.find(s => s.seasonNumber === initialSeason)?.episodes || []
    );

    const initialEpisode = episodeList?.[0]?.episodeNumber || null;
    const [selectedEpisode, setSelectedEpisode] = useState(initialEpisode);

    const selectedEpisodeObj = episodeList.find(e => e.episodeNumber === selectedEpisode);
    const selectedEpisodeId = selectedEpisodeObj?.id || null;

    // const [selectedEpisode, setSelectedEpisode] = useState(episodeKeys[0]);

    const [showSkip, setShowSkip] = useState(false);

    // … остальной ваш код (выбор сезона/серии)

    // когда видео загружается — сбросим кнопку
    useEffect(() => {
        setShowSkip(false);
    }, [selectedEpisodeId]);

    // слушаем прогресс воспроизведения
    useEffect(() => {
        const vid = videoRef.current;
        if (!vid) return;

        const onTimeUpdate = () => {
            if (vid.currentTime > timing.start && vid.currentTime < timing.end) {
                setShowSkip(true);
            }
            else {
                setShowSkip(false);
            }
        };

        vid.addEventListener('timeupdate', onTimeUpdate);
        return () => vid.removeEventListener('timeupdate', onTimeUpdate);
    }, [timing.start]);

    const skipOpening = () => {
        if (videoRef.current) {
            videoRef.current.currentTime = timing.end;
            videoRef.current.play();
            setShowSkip(false);
        }
    };

    useEffect(() => {
        const vid = videoRef.current;
        if (!vid) return;

        const onEnded = () => {
            // ищем индекс текущего эпизода в списке
            const cur = episodeList.find(e => e.episodeNumber === selectedEpisode);
            const next = episodeList.find(e => e.episodeNumber === cur.episodeNumber + 1);

            // если есть следующий эпизод в этом сезоне
            if (cur && next) {
                setSelectedEpisode(next.episodeNumber);
            } else {
                // опционально: переход на следующий сезон или остановка
                console.log('Серия последняя в сезоне');
            }
        };

        vid.addEventListener('ended', onEnded);
        return () => {
            vid.removeEventListener('ended', onEnded);
        };
    }, [selectedEpisode, episodeList]);

    useEffect(() => {
        const seasonData = seasons.find(s => s.seasonNumber === selectedSeason);
        const episodes = seasonData?.episodes || [];
        setEpisodeList(episodes);
        setSelectedEpisode(episodes[0]?.episodeNumber || null);
    }, [selectedSeason, seasons]);


    useEffect(() => {
        if (selectedEpisodeId && videoRef.current) {
            videoRef.current.src = `http://localhost:5001/api/video/${selectedEpisodeId}`;
            videoRef.current.load();
        }
    }, [selectedEpisodeId]);

    return (
        <div className="video-container container">
            <div className="list-container elem-text-big">
                <select id="season"
                    value={selectedSeason}
                    onChange={e => setSelectedSeason(parseInt(e.target.value, 10))}>
                    {seasons.map(s => (
                        <option key={s.seasonNumber} value={s.seasonNumber}>
                            Сезон {s.seasonNumber}
                        </option>
                    ))}
                </select>
                <select id="episode"
                        value={selectedEpisode}
                        onChange={e => setSelectedEpisode(parseInt(e.target.value, 10))}>
                    {episodeList.map(key => (
                        <option key={key.id} value={key.episodeNumber}>
                            Серия {key.episodeNumber}
                        </option>
                    ))}
                </select>
            </div>
            <div className="video-wrapper" style={{ position: 'relative', width: '100%' }}>

                <video
                    ref={videoRef}
                    className="video-player"
                    controls
                    style={{ width: '100%', height: 'auto' }}

                />
                {videoRef.current && showSkip && (
                    <button
                        onClick={skipOpening}
                        className="skip-button"
                        style={{
                            position: 'absolute',
                            top: '10px',
                            right: '10px',
                            padding: '8px 12px',
                            zIndex: 2,
                            background: 'rgba(0,0,0,0.5)',
                            color: '#fff',
                            border: 'none',
                            borderRadius: '4px',
                            cursor: 'pointer'
                        }}
                    >
                        Пропустить
                    </button>
                )}
            </div>
        </div>
    );
};

export default Video;
