--
-- PostgreSQL database dump
--

-- Dumped from database version 17.4
-- Dumped by pg_dump version 17.4

-- Started on 2025-05-19 18:54:05

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
-- TOC entry 218 (class 1259 OID 16399)
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 16394)
-- Name: titlerating; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.titlerating (
    idtitle integer NOT NULL,
    rating double precision,
    countreviews integer
);


ALTER TABLE public.titlerating OWNER TO postgres;

--
-- TOC entry 4794 (class 0 OID 16399)
-- Dependencies: 218
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 4793 (class 0 OID 16394)
-- Dependencies: 217
-- Data for Name: titlerating; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 4647 (class 2606 OID 16403)
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- TOC entry 4645 (class 2606 OID 16398)
-- Name: titlerating titlerating_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.titlerating
    ADD CONSTRAINT titlerating_pkey PRIMARY KEY (idtitle);


-- Completed on 2025-05-19 18:54:05

--
-- PostgreSQL database dump complete
--

