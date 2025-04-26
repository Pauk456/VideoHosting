// https://4affaaed-6fc4-46c1-ad30-b085d8124025.mock.pstmn.io/test

function fetchRecommendations() {
    
    fetch('http://localhost:5006/api/title/all') // замените на реальный адрес API
        .then(response => response.json())
        .then(data => {
            console.log(data);
            const list1 = document.getElementsByClassName('recommend-anime-list');
            console.log(list1);
            const list = document.getElementsByClassName('recommend-anime-list')[0];
            console.log(list);
            list.innerHTML = ''; // очистка перед добавлением

            data.forEach(anime => {
                const li = document.createElement('li');
                li.className = 'recommend-anime-elem';

                li.innerHTML = `
                    <a href="html/video.html?id=${anime.seriesId}">
                        <img class="recommend-poster-img" src="http://localhost:5001/api/img/${anime.seriesId}" alt="">
                        <p class="recommend-anime-link anime-link">${anime.name}</p>
                    </a>
                `;

                list.appendChild(li);
            });
        })
        .catch(err => console.error("Ошибка при загрузке рекомендаций:", err));
}


// function fetchRecommendations() {
//     fetch('https://4affaaed-6fc4-46c1-ad30-b085d8124025.mock.pstmn.io/test')
//         .then(response => response.json())
//         .then(data => {
//             const list = document.getElementsByClassName('recommend-anime-list')[0];
//             list.innerHTML = ''; // очистка перед добавлением

//             data.forEach(anime => {
//                 const li = document.createElement('li');
//                 li.className = 'recommend-anime-elem';

//                 // Создаём элементы вручную, чтобы можно было навесить обработчик
//                 const a = document.createElement('a');
//                 a.href = './html/video.html'; // временная ссылка, будет заменена

//                 const img = document.createElement('img');
//                 img.className = 'recommend-poster-img';
//                 img.src = anime.image;
//                 img.alt = '';

//                 const p = document.createElement('p');
//                 p.className = 'recommend-anime-link anime-link';
//                 p.textContent = anime.title;

//                 a.appendChild(img);
//                 a.appendChild(p);

//                 // Обработчик клика
//                 a.addEventListener('click', function (event) {
//                     event.preventDefault(); // отменить стандартный переход
//                     const encodedTitle = encodeURIComponent(anime.title);
//                     window.location.href = `./html/video.html?text=${encodedTitle}`;
//                 });

//                 li.appendChild(a);
//                 list.appendChild(li);
//             });
//         })
//         .catch(err => console.error("Ошибка при загрузке рекомендаций:", err));
// }


function resizeRecommendContainer() {
    const poster = document.querySelector('.recommend-anime-elem');
    console.log(poster);
    const posterContainer = document.querySelector('.recommend-anime-list'); // Контейнер сетки
    console.log(posterContainer);

    const containerStyle = window.getComputedStyle(posterContainer);
    const posterStyle = window.getComputedStyle(poster);

// Получаем ширину контейнера и ширину постера
    const containerWidth = parseFloat(containerStyle.width); // Ширина контейнера
    const posterWidth = parseFloat(posterStyle.width); // Ширина одного постера
    const gap = parseFloat(containerStyle.gap); // Значение gap между элементами

    // console.log(containerWidth, posterWidth, gap);

    // Рассчитываем количество колонок, с учетом ширины постера и отступов
    console.log("COLS: "+ (containerWidth + gap) / (posterWidth + gap));
    const cols = Math.round((containerWidth + gap) / (posterWidth + gap));
        


    console.log(containerWidth, posterWidth);

    var items = document.querySelectorAll(".recommend-anime-elem");
    items.forEach((item, index) => {
        const rowIndex = Math.floor(index / cols); // Определяем на какой строке находится элемент

        if (rowIndex > 0) {
            console.log(index);
            item.style.display = 'none'; // Скрываем элемент, если он во второй строке или ниже
        }
        else {
            item.style.display = ''; // Показываем элемент, если он на первой строке
        }
    })
}


function resizeActualContainer() {
    const gridElem = document.querySelector('.actual-anime-elem');
    const actualGridContainer = document.querySelector('.actual-grid-container'); // Контейнер сетки
    const containerStyle = window.getComputedStyle(actualGridContainer);
    const elemStyle = window.getComputedStyle(gridElem);

// Получаем ширину контейнера и ширину постера
    const containerWidth = parseFloat(containerStyle.width); // Ширина контейнера
    const elemWidth = parseFloat(elemStyle.width); // Ширина одного постера
    const gap = parseFloat(containerStyle.columnGap); // Значение gap между элементами

    // console.log(containerWidth, posterWidth, gap);

    // Рассчитываем количество колонок, с учетом ширины постера и отступов
    //console.log("SMALL COLS: "+ containerWidth / elemWidth);
    const cols = Math.round((containerWidth + gap) / (elemWidth + gap));
        

    var items = document.querySelectorAll(".actual-anime-elem");
   // console.log("SMALL COLS: " + cols);
    items.forEach((item, index) => {
        if (index >= cols * 2) {
            item.style.display = 'none';
        } else {
            item.style.display = ''; // Показываем все остальные
        }
        // const rowIndex = Math.floor(index / cols); // Определяем на какой строке находится элемент

        // if (rowIndex > 0) {
        //     console.log(index);
        //     item.style.display = 'none'; // Скрываем элемент, если он во второй строке или ниже
        // }
        // else {
        //     item.style.display = ''; // Показываем элемент, если он на первой строке
        // }
    })
}

// Запуск при загрузке страницы
document.addEventListener('DOMContentLoaded', resizeRecommendContainer);

// И/или при изменении размера окна
window.addEventListener('resize', resizeRecommendContainer);

document.addEventListener('DOMContentLoaded', resizeActualContainer);

window.addEventListener('resize', resizeActualContainer);
document.addEventListener('DOMContentLoaded', fetchRecommendations);


// document.querySelector('.header-search-button').addEventListener('click', function() {
//     const searchInput = document.querySelector('.search-input');
//     console.log(searchInput)
//     if (searchInput.style.display === 'none' || !searchInput.style.display) {
//         searchInput.style.display = 'block'; // Показываем поле
//     } else {
//         searchInput.style.display = 'none'; // Скрываем поле
//     }

//     // searchInput.style.display = 'block';
// });
