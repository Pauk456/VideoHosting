// src/components/videoPage/Video.jsx
import React, {useEffect, useRef, useState} from 'react';
import Hls from 'hls.js';

const Video = ({ host, seriesList}) => {
    const videoRef = useRef(null);
    const episodeKeys = Object.keys(seriesList);
    // По умолчанию первая серия
    const [selectedEpisode, setSelectedEpisode] = useState(episodeKeys[0]);

    const hlsData = seriesList[selectedEpisode]?.hls || {};
    const selectedPath =
        hlsData.fhd || hlsData.hd || hlsData.sd || '';

    // Полный URL
    const src = selectedPath
        ? `https://${host}${selectedPath}`
        : '';
    useEffect(() => {
        const video = videoRef.current;
        if (!video) return;

        // Если браузер нативно поддерживает HLS (Safari), просто ставим src
        if (video.canPlayType('application/vnd.apple.mpegurl')) {
            video.src = src;
        } else if (Hls.isSupported()) {
            // Для остальных создаём HLS-плеер
            const hls = new Hls();
            hls.loadSource(src);
            hls.attachMedia(video);
            // Очистка при анмаунте
            return () => {
                hls.destroy();
            };
        } else {
            console.error('HLS не поддерживается в этом браузере');
        }
    }, [src]);

    return (
        <div className="video-container container">
            <div className="list-container elem-text-big">
                <select id="season">
                    <option>Сезон</option>
                </select>
                <select id="episode"
                        value={selectedEpisode}
                        onChange={e => setSelectedEpisode(e.target.value)}>
                    {episodeKeys.map(key => (
                        <option key={key} value={key}>
                            Серия {key}
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
