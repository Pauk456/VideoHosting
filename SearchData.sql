--
-- PostgreSQL database dump
--

-- Dumped from database version 17.5
-- Dumped by pg_dump version 17.5

-- Started on 2025-05-19 03:16:55

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
-- TOC entry 219 (class 1259 OID 80716)
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;

--
-- TOC entry 218 (class 1259 OID 80710)
-- Name: searchdata; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.searchdata (
    search_id integer NOT NULL,
    title_id integer NOT NULL,
    title_tag character(100) NOT NULL
);


ALTER TABLE public.searchdata OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 80709)
-- Name: searchdata_search_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.searchdata_search_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.searchdata_search_id_seq OWNER TO postgres;

--
-- TOC entry 4857 (class 0 OID 0)
-- Dependencies: 217
-- Name: searchdata_search_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.searchdata_search_id_seq OWNED BY public.searchdata.search_id;


--
-- TOC entry 4699 (class 2604 OID 80713)
-- Name: searchdata search_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.searchdata ALTER COLUMN search_id SET DEFAULT nextval('public.searchdata_search_id_seq'::regclass);


--
-- TOC entry 4851 (class 0 OID 80716)
-- Dependencies: 219
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 4850 (class 0 OID 80710)
-- Dependencies: 218
-- Data for Name: searchdata; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 4858 (class 0 OID 0)
-- Dependencies: 217
-- Name: searchdata_search_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.searchdata_search_id_seq', 1, true);


--
-- TOC entry 4703 (class 2606 OID 80720)
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- TOC entry 4701 (class 2606 OID 80715)
-- Name: searchdata searchdata_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.searchdata
    ADD CONSTRAINT searchdata_pkey PRIMARY KEY (search_id);


-- Completed on 2025-05-19 03:16:55

--
-- PostgreSQL database dump complete
--

