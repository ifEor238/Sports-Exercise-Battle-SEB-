PGDMP                         y           sbeDB    13.2    13.2     ?           0    0    ENCODING    ENCODING     #   SET client_encoding = 'SQL_ASCII';
                      false            ?           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            ?           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            ?           1262    16416    sbeDB    DATABASE     j   CREATE DATABASE "sbeDB" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE = 'English_United States.932';
    DROP DATABASE "sbeDB";
                postgres    false            ?            1259    16425 
   tournament    TABLE     ?   CREATE TABLE public.tournament (
    running character varying(255),
    starttime character varying(255),
    endtime character varying(255),
    tournid integer NOT NULL
);
    DROP TABLE public.tournament;
       public         heap    postgres    false            ?            1259    16451    tournament_tournid_seq    SEQUENCE     ?   CREATE SEQUENCE public.tournament_tournid_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 -   DROP SEQUENCE public.tournament_tournid_seq;
       public          postgres    false    201            ?           0    0    tournament_tournid_seq    SEQUENCE OWNED BY     Q   ALTER SEQUENCE public.tournament_tournid_seq OWNED BY public.tournament.tournid;
          public          postgres    false    203            ?            1259    16433    tournplayers    TABLE     ?   CREATE TABLE public.tournplayers (
    username character varying(255) NOT NULL,
    tournid integer NOT NULL,
    count character varying,
    duration character varying
);
     DROP TABLE public.tournplayers;
       public         heap    postgres    false            ?            1259    16417    users    TABLE     A  CREATE TABLE public.users (
    bio character varying(255),
    image character varying(255),
    name character varying(255),
    password character varying(255),
    username character varying(255) NOT NULL,
    authtoken character varying(255),
    totalcount character varying(255),
    elo character varying(255)
);
    DROP TABLE public.users;
       public         heap    postgres    false            -           2604    16453    tournament tournid    DEFAULT     x   ALTER TABLE ONLY public.tournament ALTER COLUMN tournid SET DEFAULT nextval('public.tournament_tournid_seq'::regclass);
 A   ALTER TABLE public.tournament ALTER COLUMN tournid DROP DEFAULT;
       public          postgres    false    203    201            ?          0    16425 
   tournament 
   TABLE DATA           J   COPY public.tournament (running, starttime, endtime, tournid) FROM stdin;
    public          postgres    false    201          ?          0    16433    tournplayers 
   TABLE DATA           J   COPY public.tournplayers (username, tournid, count, duration) FROM stdin;
    public          postgres    false    202   ?       ?          0    16417    users 
   TABLE DATA           a   COPY public.users (bio, image, name, password, username, authtoken, totalcount, elo) FROM stdin;
    public          postgres    false    200   ?       ?           0    0    tournament_tournid_seq    SEQUENCE SET     E   SELECT pg_catalog.setval('public.tournament_tournid_seq', 72, true);
          public          postgres    false    203            /           2606    16424    users Users_pkey 
   CONSTRAINT     V   ALTER TABLE ONLY public.users
    ADD CONSTRAINT "Users_pkey" PRIMARY KEY (username);
 <   ALTER TABLE ONLY public.users DROP CONSTRAINT "Users_pkey";
       public            postgres    false    200            1           2606    16468    tournament tournament_pkey 
   CONSTRAINT     ]   ALTER TABLE ONLY public.tournament
    ADD CONSTRAINT tournament_pkey PRIMARY KEY (tournid);
 D   ALTER TABLE ONLY public.tournament DROP CONSTRAINT tournament_pkey;
       public            postgres    false    201            3           2606    16461    tournplayers tournplayers_pkey 
   CONSTRAINT     k   ALTER TABLE ONLY public.tournplayers
    ADD CONSTRAINT tournplayers_pkey PRIMARY KEY (tournid, username);
 H   ALTER TABLE ONLY public.tournplayers DROP CONSTRAINT tournplayers_pkey;
       public            postgres    false    202    202            5           2606    16474    tournplayers tournFK    FK CONSTRAINT     ?   ALTER TABLE ONLY public.tournplayers
    ADD CONSTRAINT "tournFK" FOREIGN KEY (tournid) REFERENCES public.tournament(tournid) NOT VALID;
 @   ALTER TABLE ONLY public.tournplayers DROP CONSTRAINT "tournFK";
       public          postgres    false    202    2865    201            4           2606    16469    tournplayers userFK    FK CONSTRAINT     ?   ALTER TABLE ONLY public.tournplayers
    ADD CONSTRAINT "userFK" FOREIGN KEY (username) REFERENCES public.users(username) NOT VALID;
 ?   ALTER TABLE ONLY public.tournplayers DROP CONSTRAINT "userFK";
       public          postgres    false    202    200    2863            ?      x?????? ? ?      ?      x?????? ? ?      ?      x?????? ? ?     