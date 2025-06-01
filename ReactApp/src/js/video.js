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

    // fetch(`http://193.53.126.242:5006/api/title/getSeasonsAndEpisodes/${id}`)

    fetch(`http://193.53.126.242:5006/api/title/getSeasonsAndEpisodes/${id}`)
    .then(response => response.json())
    .then(data => {
      console.log(data);
      // Заполняем список сезонов
      data.forEach(season => {
        console.log(season);
        const option = document.createElement("option");
        // option.value = season.seasonNumber;
        option.textContent = season.seasonNumber;
        seasonSelect.appendChild(option);
      });

      // Добавляем обработчик изменения сезона
      seasonSelect.addEventListener("change", () => {
        const selectedSeason = data.find(season => season.seasonNumber == seasonSelect.value);
        episodeSelect.innerHTML = "";

        const defaultOption = document.createElement("option");
        defaultOption.textContent = "Серия";
        defaultOption.selected = true;
        episodeSelect.appendChild(defaultOption);
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
        console.log(episodeId);
        // Подставляем ID в ссылку на видео
        const videoUrl = `http://193.53.126.242:5001/api/video/${episodeId}`;
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


  document.querySelector('.comment-button').addEventListener('click', function() {
    const searchInput = document.querySelector('.input-comment');
    const button = this; // Используем this вместо повторного поиска
    if (button.textContent === 'Отправить') {
      const ul = document.querySelector('.comments-list-container')
      const li = document.createElement('li');
      const divComment = document.createElement('div');
      divComment.className = 'comment';

      const userAvatar = document.createElement('div');
      userAvatar.className = "user-avatar";
      const textCommentContainer = document.createElement('div')
     textCommentContainer.className ='text-comment-container';

      const userName = document.createElement('p');
      userName.className = 'elem-text-medium';
      const commentText = document.createElement('p');
      commentText.className = 'comment-text elem-text-extrasmall';
      commentText.textContent = searchInput.value;
      searchInput.value = "";
      userName.textContent = "Anonimous";
      textCommentContainer.appendChild(userName);
      textCommentContainer.appendChild(commentText);
      divComment.appendChild(userAvatar);
      divComment.appendChild(textCommentContainer);

      li.appendChild(divComment);
      console.log(li);
      ul.insertBefore(li, ul.firstChild);
      // ul.appendChild(li);
      searchInput.style.display = 'none'; // Скрываем поле
      button.textContent = "Написать комментарий";

    }
    else {
      button.textContent = "Отправить";
      console.log(searchInput)
      if (searchInput.style.display === 'none' || !searchInput.style.display) {
          searchInput.style.display = 'block'; // Показываем поле
      } else {
          searchInput.style.display = 'none'; // Скрываем поле
      }
  }

    // searchInput.style.display = 'block';
});