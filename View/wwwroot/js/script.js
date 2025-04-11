function resizeSliderContainer() {
    const poster = document.querySelector('.poster-elem');
    const posterContainer = document.querySelector('.poster-list'); // Контейнер сетки
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

    var items = document.querySelectorAll(".poster-elem");
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
    const gridElem = document.querySelector('.anime-elem');
    const actualGridContainer = document.querySelector('.actual-grid-container'); // Контейнер сетки
    const containerStyle = window.getComputedStyle(actualGridContainer);
    const elemStyle = window.getComputedStyle(gridElem);

// Получаем ширину контейнера и ширину постера
    const containerWidth = parseFloat(containerStyle.width); // Ширина контейнера
    const elemWidth = parseFloat(elemStyle.width); // Ширина одного постера
    const gap = parseFloat(containerStyle.columnGap); // Значение gap между элементами

    // console.log(containerWidth, posterWidth, gap);

    // Рассчитываем количество колонок, с учетом ширины постера и отступов
    console.log("SMALL COLS: "+ containerWidth / elemWidth);
    const cols = Math.round((containerWidth + gap) / (elemWidth + gap));
        

    var items = document.querySelectorAll(".anime-elem");
    console.log("SMALL COLS: " + cols);
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
document.addEventListener('DOMContentLoaded', resizeSliderContainer);

// И/или при изменении размера окна
window.addEventListener('resize', resizeSliderContainer);

document.addEventListener('DOMContentLoaded', resizeActualContainer);

window.addEventListener('resize', resizeActualContainer);

document.querySelector('.header-search-button').addEventListener('click', function() {
    const searchInput = document.querySelector('.search-input');
    console.log(searchInput)
    if (searchInput.style.display === 'none' || !searchInput.style.display) {
        searchInput.style.display = 'block'; // Показываем поле
    } else {
        searchInput.style.display = 'none'; // Скрываем поле
    }

    // searchInput.style.display = 'block';
});
