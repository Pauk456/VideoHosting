window.addEventListener("DOMContentLoaded", () => {
    const params = new URLSearchParams(window.location.search);
    const id = params.get("id");

    const seasonSelect = document.getElementById("season");
    const episodeSelect = document.getElementById("episode");
  
    // if (!slug) {
    //   // 🔁 Нет параметра — редирект на главную
    //   window.location.href = "../index.html";
    //   return;
    // }
    fetch(`http://localhost:5006/api/title/getSeasonsAndEpisodes/${id}`)
    .then(response => response.json())
    .then(data => {
      // Заполняем список сезонов
      data.forEach(season => {
        const option = document.createElement("option");
        option.value = season.seasonNumber;
        option.textContent = season.seasonNumber;
        seasonSelect.appendChild(option);
      });

      // Добавляем обработчик изменения сезона
      seasonSelect.addEventListener("change", () => {
        const selectedSeason = data.find(season => season.seasonNumber == seasonSelect.value);
        episodeSelect.innerHTML = "";

        if (selectedSeason) {
          selectedSeason.episodes.forEach(episode => {
            const option = document.createElement("option");
            option.value = episode.id; // id пригодится для ссылки
            option.textContent = episode.episodeNumber;
            episodeSelect.appendChild(option);
          });

          // Триггерим смену серии сразу после загрузки
          episodeSelect.dispatchEvent(new Event("change"));
        }
      });

      // Обработка смены серии
      episodeSelect.addEventListener("change", () => {
        const episodeId = episodeSelect.value;

        // Подставляем ID в ссылку на видео
        const videoUrl = `http://localhost:5001/api/video/${episodeId}`;
        const videoPlayer = document.getElementsByClassName("video-player")[0];

        videoPlayer.src = videoUrl;
        videoPlayer.load(); // пер
      });

      // Инициализация первой серии
      if (seasonSelect.options.length > 0) {
        seasonSelect.selectedIndex = 0;
        seasonSelect.dispatchEvent(new Event("change"));
      }
    });
    // Здесь можно подставить данные или загрузить из API
    // Например:
    // document.getElementById("app").innerHTML = `<h1>Аниме: ${slug}</h1>`;
  });