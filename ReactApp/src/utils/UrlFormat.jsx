import slugifyLib from 'slugify';

export function getAnimeUrl(title, id) {
  const slug = slugifyLib(title, {
    lower: true,
    locale: 'ru',
    strict: true,
  });
  return `/anime/${slug}/${id}`;
}