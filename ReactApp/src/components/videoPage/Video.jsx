// src/components/videoPage/Video.jsx
import React, {useEffect, useRef, useState} from 'react';

const Video = ({seasons}) => {
    const initialSeason = seasons?.[0]?.seasonNumber || null;

    const videoRef = useRef(null);
    // По умолчанию первая серия
    const [selectedSeason, setSelectedSeason] = useState(initialSeason);
    const [episodeList, setEpisodeList] = useState(
        seasons?.find(s => s.seasonNumber === initialSeason)?.episodes || []
    );

    const initialEpisode = episodeList?.[0]?.episodeNumber || null;
    const [selectedEpisode, setSelectedEpisode] = useState(initialEpisode);
    // const [selectedEpisode, setSelectedEpisode] = useState(episodeKeys[0]);

    useEffect(() => {
        const seasonData = seasons.find(s => s.seasonNumber === selectedSeason);
        const episodes = seasonData?.episodes || [];
        setEpisodeList(episodes);
        setSelectedEpisode(episodes[0]?.episodeNumber || null);
    }, [selectedSeason, seasons]);

    const selectedEpisodeObj = episodeList.find(e => e.episodeNumber === selectedEpisode);
    const selectedEpisodeId = selectedEpisodeObj?.id || null;

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
                    {seasons.map(key => (
                        <option key={key.id} value={key.episodeNumber}>
                            Серия {key.episodeNumber}
                        </option>
                    ))}
                </select>
            </div>
            <video
                ref={videoRef}
                className="video-player"
                controls
                style={{ width: '100%', height: 'auto' }}
            />
        </div>
    );
};

export default Video;
