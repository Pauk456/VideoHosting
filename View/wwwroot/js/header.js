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