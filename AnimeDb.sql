--
-- PostgreSQL database dump
--

-- Dumped from database version 17.2
-- Dumped by pg_dump version 17.2

-- Started on 2025-05-08 18:37:25

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 218 (class 1259 OID 16635)
-- Name: anime_series; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.anime_series (
    id integer NOT NULL,
    preview_path character varying(255) NOT NULL,
    title character varying(100) NOT NULL
);


ALTER TABLE public.anime_series OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 16634)
-- Name: anime_series_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.anime_series_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.anime_series_id_seq OWNER TO postgres;

--
-- TOC entry 4872 (class 0 OID 0)
-- Dependencies: 217
-- Name: anime_series_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.anime_series_id_seq OWNED BY public.anime_series.id;


--
-- TOC entry 222 (class 1259 OID 16654)
-- Name: episodes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.episodes (
    id integer NOT NULL,
    season_id integer NOT NULL,
    file_path character varying(255) NOT NULL,
    episode_number integer NOT NULL
);


ALTER TABLE public.episodes OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 16653)
-- Name: episodes_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.episodes_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.episodes_id_seq OWNER TO postgres;

--
-- TOC entry 4873 (class 0 OID 0)
-- Dependencies: 221
-- Name: episodes_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.episodes_id_seq OWNED BY public.episodes.id;


--
-- TOC entry 220 (class 1259 OID 16642)
-- Name: seasons; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.seasons (
    id integer NOT NULL,
    series_id integer NOT NULL,
    season_number integer NOT NULL
);


ALTER TABLE public.seasons OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 16641)
-- Name: seasons_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.seasons_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.seasons_id_seq OWNER TO postgres;

--
-- TOC entry 4874 (class 0 OID 0)
-- Dependencies: 219
-- Name: seasons_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.seasons_id_seq OWNED BY public.seasons.id;


--
-- TOC entry 4705 (class 2604 OID 16638)
-- Name: anime_series id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.anime_series ALTER COLUMN id SET DEFAULT nextval('public.anime_series_id_seq'::regclass);


--
-- TOC entry 4707 (class 2604 OID 16657)
-- Name: episodes id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.episodes ALTER COLUMN id SET DEFAULT nextval('public.episodes_id_seq'::regclass);


--
-- TOC entry 4706 (class 2604 OID 16645)
-- Name: seasons id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.seasons ALTER COLUMN id SET DEFAULT nextval('public.seasons_id_seq'::regclass);


--
-- TOC entry 4862 (class 0 OID 16635)
-- Dependencies: 218
-- Data for Name: anime_series; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.anime_series (id, preview_path, title) VALUES (21, 'Videos\gg\naruto.jpg', 'gg');
INSERT INTO public.anime_series (id, preview_path, title) VALUES (1, 'Videos\naruto\naruto.jpg', 'naruto');
INSERT INTO public.anime_series (id, preview_path, title) VALUES (2, 'Videos\demon-slayer\demon-slayer.jpg', 'demon-slayer');
INSERT INTO public.anime_series (id, preview_path, title) VALUES (22, 'Videos\ggggg\naruto.jpg', 'ggggg');


--
-- TOC entry 4866 (class 0 OID 16654)
-- Dependencies: 222
-- Data for Name: episodes; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.episodes (id, season_id, file_path, episode_number) VALUES (4, 1, 'Videos\naruto\s1\ep1.mp4', 1);
INSERT INTO public.episodes (id, season_id, file_path, episode_number) VALUES (5, 1, 'Videos\naruto\s1\ep2.mp4', 2);
INSERT INTO public.episodes (id, season_id, file_path, episode_number) VALUES (8, 1, 'Videos\naruto\s1\ep3.mp4', 3);
INSERT INTO public.episodes (id, season_id, file_path, episode_number) VALUES (7, 1, 'Videos\naruto\s1\ep4.mp4', 4);


--
-- TOC entry 4864 (class 0 OID 16642)
-- Dependencies: 220
-- Data for Name: seasons; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.seasons (id, series_id, season_number) VALUES (1, 1, 1);
INSERT INTO public.seasons (id, series_id, season_number) VALUES (4, 1, 2);
INSERT INTO public.seasons (id, series_id, season_number) VALUES (5, 1, 3);
INSERT INTO public.seasons (id, series_id, season_number) VALUES (6, 2, 1);
INSERT INTO public.seasons (id, series_id, season_number) VALUES (7, 2, 2);
INSERT INTO public.seasons (id, series_id, season_number) VALUES (8, 2, 3);


--
-- TOC entry 4875 (class 0 OID 0)
-- Dependencies: 217
-- Name: anime_series_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.anime_series_id_seq', 22, true);


--
-- TOC entry 4876 (class 0 OID 0)
-- Dependencies: 221
-- Name: episodes_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.episodes_id_seq', 8, true);


--
-- TOC entry 4877 (class 0 OID 0)
-- Dependencies: 219
-- Name: seasons_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.seasons_id_seq', 8, true);


--
-- TOC entry 4709 (class 2606 OID 16640)
-- Name: anime_series anime_series_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.anime_series
    ADD CONSTRAINT anime_series_pkey PRIMARY KEY (id);


--
-- TOC entry 4713 (class 2606 OID 16659)
-- Name: episodes episodes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.episodes
    ADD CONSTRAINT episodes_pkey PRIMARY KEY (id);


--
-- TOC entry 4711 (class 2606 OID 16647)
-- Name: seasons seasons_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.seasons
    ADD CONSTRAINT seasons_pkey PRIMARY KEY (id);


--
-- TOC entry 4715 (class 2606 OID 16660)
-- Name: episodes episodes_season_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.episodes
    ADD CONSTRAINT episodes_season_id_fkey FOREIGN KEY (season_id) REFERENCES public.seasons(id) ON DELETE CASCADE;


--
-- TOC entry 4714 (class 2606 OID 16648)
-- Name: seasons seasons_series_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.seasons
    ADD CONSTRAINT seasons_series_id_fkey FOREIGN KEY (series_id) REFERENCES public.anime_series(id) ON DELETE CASCADE;


-- Completed on 2025-05-08 18:37:26

--
-- PostgreSQL database dump complete
--

