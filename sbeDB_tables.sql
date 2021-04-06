-- Table: public.users

-- DROP TABLE public.users;

CREATE TABLE public.users
(
    bio character varying(255) COLLATE pg_catalog."default",
    image character varying(255) COLLATE pg_catalog."default",
    name character varying(255) COLLATE pg_catalog."default",
    password character varying(255) COLLATE pg_catalog."default",
    username character varying(255) COLLATE pg_catalog."default" NOT NULL,
    authtoken character varying(255) COLLATE pg_catalog."default",
    totalcount character varying(255) COLLATE pg_catalog."default",
    elo character varying(255) COLLATE pg_catalog."default",
    CONSTRAINT "Users_pkey" PRIMARY KEY (username)
)

TABLESPACE pg_default;

ALTER TABLE public.users
    OWNER to postgres;



-- Table: public.tournament

-- DROP TABLE public.tournament;

CREATE TABLE public.tournament
(
    running character varying(255) COLLATE pg_catalog."default",
    starttime character varying(255) COLLATE pg_catalog."default",
    endtime character varying(255) COLLATE pg_catalog."default",
    tournid integer NOT NULL DEFAULT nextval('tournament_tournid_seq'::regclass),
    CONSTRAINT tournament_pkey PRIMARY KEY (tournid)
)

TABLESPACE pg_default;

ALTER TABLE public.tournament
    OWNER to postgres;





-- Table: public.tournplayers

-- DROP TABLE public.tournplayers;

CREATE TABLE public.tournplayers
(
    username character varying(255) COLLATE pg_catalog."default" NOT NULL,
    tournid integer NOT NULL,
    count character varying COLLATE pg_catalog."default",
    duration character varying COLLATE pg_catalog."default",
    CONSTRAINT tournplayers_pkey PRIMARY KEY (tournid, username),
    CONSTRAINT "tournFK" FOREIGN KEY (tournid)
        REFERENCES public.tournament (tournid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID,
    CONSTRAINT "userFK" FOREIGN KEY (username)
        REFERENCES public.users (username) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
)

TABLESPACE pg_default;

ALTER TABLE public.tournplayers
    OWNER to postgres;