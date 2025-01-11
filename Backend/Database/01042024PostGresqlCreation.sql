---- DROP TABLES IF NEEDED, CARE, THIS WILL DELETE ALL DATA
--DROP TABLE IF EXISTS public."Registration" CASCADE;
--DROP TABLE IF EXISTS public."Profile" CASCADE;
--DROP TABLE IF EXISTS public."Person" CASCADE;
--DROP TABLE IF EXISTS public."CompetitionPerCategory" CASCADE;
--DROP TABLE IF EXISTS public."Competition" CASCADE;
--DROP TABLE IF EXISTS public."AgeCategory" CASCADE;
--DROP TABLE IF EXISTS public."School" CASCADE;
--DROP TABLE IF EXISTS public."Address" CASCADE;

--SET
--statement_timeout = 0;
--SET
--lock_timeout = 0;
--SET
--idle_in_transaction_session_timeout = 0;
--SET
--client_encoding = 'UTF8';
--SET
--standard_conforming_strings = on;
--SELECT pg_catalog.set_config('search_path', '', false);
--SET
--check_function_bodies = false;
--SET
--xmloption = content;
--SET
--client_min_messages = warning;
--SET
--row_security = off;
--/*CREATE SCHEMA public;
--*/
--SET
--search_path TO public;


--CREATE
--OR REPLACE FUNCTION get_competition_results()
--RETURNS TABLE (
--    competition_id int,
--    run_time text,
--    person_id int,
--    first_name varchar,
--    last_name varchar,
--    gender bpchar,
--    distance_name varchar,
--    age_category_name varchar
--) AS $$
--BEGIN
--RETURN QUERY
--SELECT c."Id"             as competition_id,
--       r."RunTime"        as run_time,
--       p."Id"             as person_id,
--       p."FirstName"      as first_name,
--       p."LastName"       as last_name,
--       cpc."Gender"       as gender,
--       cpc."DistanceName" as distance_name,
--       ac."Name"          as age_category_name
--FROM "Competition" c
--         INNER JOIN "Registration" r ON r."CompetitionId" = c."Id"
--         INNER JOIN "CompetitionPerCategory" cpc ON cpc."Id" = r."CompetitionPerCategoryId"
--         INNER JOIN "AgeCategory" ac ON cpc."AgeCategoryId" = ac."Id"
--         INNER JOIN "Person" p ON r."PersonId" = p."Id"
--WHERE r."RunNumber" IS NOT NULL
--  AND r."RunTime" IS NOT NULL
--  AND c."RankingActive" = true
--  AND cpc."DistanceName" IN ('kort', 'midden', 'lang')
--  AND p."Email" IS NOT NULL
--  AND ac."MinimumAge" >= 16
--ORDER BY r."RunTime";
--END;
--$$
--LANGUAGE plpgsql;
    
--CREATE
--OR REPLACE FUNCTION public.confirm_email(confirm_token character varying) RETURNS void
--    LANGUAGE plpgsql SECURITY DEFINER
--    AS $$
--BEGIN
--UPDATE auth.users AS u
--SET confirmation_token = '',
--    email_confirmed_at = NOW()
--WHERE u.confirmation_token = confirm_email.confirm_token;


--IF
--NOT FOUND THEN
--        RAISE EXCEPTION 'Invalid confirmation token';
--END IF;
--END;
--$$;

--CREATE
--OR REPLACE FUNCTION public.delete_from_person_details() RETURNS trigger
--    LANGUAGE plpgsql SECURITY DEFINER
--    AS $$
--BEGIN
--DELETE
--FROM auth.users
--WHERE id = OLD."UserId";
--RETURN OLD;
--END;

--$$;

--CREATE
--OR REPLACE FUNCTION public.find_user_email(email_address character varying) RETURNS auth.users
--    LANGUAGE plpgsql SECURITY DEFINER
--    AS $$DECLARE
--    _user auth.users%ROWTYPE;
--BEGIN
--SELECT *
--INTO _user
--FROM auth.users
--WHERE email = find_user_email.email_address;

--RETURN _user;
--END;$$;

--CREATE
--OR REPLACE FUNCTION public.get_competition_registrations(competitionid integer, searchvalue text) RETURNS TABLE(registration_id integer, run_number smallint, run_time text, competition_per_category_id integer, paid boolean, person_id integer, competition_id integer, last_name character varying, first_name character varying, birth_date date, iban_number character varying, school_id integer, address_id integer, gender character varying, user_id uuid, street character varying, house_number character varying, city character varying, zip_code character varying, distance_name character varying)
--    LANGUAGE plpgsql SECURITY DEFINER
--    AS $$BEGIN
--    RETURN QUERY
--SELECT r."Id"                       AS registration_id,
--       r."RunNumber"                AS run_number,
--       r."RunTime"                  AS run_time,
--       r."CompetitionPerCategoryId" AS competition_per_category_id,
--       r."Paid"                     AS paid,
--       r."PersonId"                 AS person_id,
--       r."CompetitionId"            AS competition_id,
--       p."LastName"                 AS last_name,
--       p."FirstName"                AS first_name,
--       p."BirthDate"                AS birth_date,
--       p."IBANNumber"               AS iban_number,
--       p."SchoolId"                 AS school_id,
--       p."AddressId"                AS address_id,
--       p."Gender"                   AS gender,
--       p."UserId"                   AS user_id,
--       a."Street"                   AS street,
--       a."HouseNumber"              AS house_number,
--       a."City"                     AS city,
--       a."ZipCode"                  AS zip_code,
--       cpc."DistanceName"           AS distance_name
--FROM "Registration" r
--         INNER JOIN personview p ON p."Id" = r."PersonId"
--         INNER JOIN "Address" a ON a."Id" = p."AddressId"
--         INNER JOIN "CompetitionPerCategory" cpc ON cpc."Id" = r."CompetitionPerCategoryId"
--WHERE r."CompetitionId" = get_competition_registrations.competitionId
--  AND p.fullname ILIKE '%' || get_competition_registrations.searchValue || '%';
--END;$$;

--CREATE
--OR REPLACE FUNCTION public.get_persons_by_search_value(search_value text) RETURNS TABLE(person_id integer, last_name character varying, first_name character varying, birth_date date, iban_number character varying, school_id integer, address_id integer, gender character varying, user_id uuid, email character varying, street character varying, house_number character varying, city character varying, zip_code character varying, user_role character varying)
--    LANGUAGE plpgsql SECURITY DEFINER
--    AS $$BEGIN
--    RETURN QUERY
--SELECT p."Id"          AS person_id,
--       p."LastName"    AS last_name,
--       p."FirstName"   AS first_name,
--       p."BirthDate"   AS birth_date,
--       p."IBANNumber"  AS iban_number,
--       p."SchoolId"    AS school_id,
--       p."AddressId"   AS address_id,
--       p."Gender"      AS gender,
--       p."UserId"      AS user_id,
--       p."Email"       AS email,
--       a."Street"      AS street,
--       a."HouseNumber" AS house_number,
--       a."City"        AS city,
--       a."ZipCode"     AS zip_code,
--       pr.role         AS user_role
--FROM personview p
--         INNER JOIN "Address" a ON a."Id" = p."AddressId"
--         LEFT JOIN "Profile" pr ON p."UserId" = pr.id
--WHERE p.fullname ILIKE '%' || search_value || '%';
--END;$$;

--CREATE
--OR REPLACE FUNCTION public.handle_new_user() RETURNS trigger
--    LANGUAGE plpgsql SECURITY DEFINER
--    AS $$
--BEGIN
--insert into public."Profile" (id, role)
--values (new.id, 'User');
--return new;
--END;
--$$;

--create
--or replace trigger on_auth_user_created
--    after insert on auth.users
--    for each row execute procedure public.handle_new_user();

--CREATE
--OR REPLACE FUNCTION public.remove_user_email(email_address character varying) RETURNS void
--    LANGUAGE plpgsql SECURITY DEFINER
--    AS $$
--BEGIN
--DELETE
--FROM auth.users
--WHERE email = email_address::text;
--END;
--$$;

--CREATE
--OR REPLACE FUNCTION public.set_email_confirm_token(confirm_token character varying, email character varying) RETURNS void
--    LANGUAGE plpgsql SECURITY DEFINER
--    SET search_path TO ''
--    AS $$
--BEGIN
--UPDATE auth.users AS u
--SET confirmation_token   = set_email_confirm_token.confirm_token,
--    email_confirmed_at   = null,
--    confirmation_sent_at = NOW()
--WHERE u.email = set_email_confirm_token.email;


--IF
--NOT FOUND THEN
--        RAISE EXCEPTION 'User not found with this email';
--END IF;
--END;
--$$;

--CREATE
--OR REPLACE FUNCTION public.set_password_recovery_token(recovery_token character varying, email character varying) RETURNS void
--    LANGUAGE plpgsql SECURITY DEFINER
--    AS $$
--BEGIN
--UPDATE auth.users AS u
--SET recovery_token   = set_password_recovery_token.recovery_token,
--    recovery_sent_at = NOW()
--WHERE u.email = set_password_recovery_token.email;


--IF
--NOT FOUND THEN
--        RAISE EXCEPTION 'User not found with this email';
--END IF;
--END;
--$$;

--CREATE
--OR REPLACE FUNCTION public.update_user_email(old_email_address character varying, new_email_address character varying) RETURNS void
--    LANGUAGE plpgsql SECURITY DEFINER
--    AS $_$BEGI    IF new_email_address !~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' THEN
--        RAISE EXCEPTION 'New email address % is not valid', new_email_address;
--END IF;
--UPDATE auth.users
--SET email = new_email_address
--WHERE email = old_email_address;
--IF
--NOT FOUND THEN
--        RAISE EXCEPTION 'Email address % does not exist', old_email_address;
--END IF;
--END;$_$;

--CREATE
--OR REPLACE FUNCTION public.update_user_password(user_id uuid, old_password character varying, new_password character varying) RETURNS void
--    LANGUAGE plpgsql SECURITY DEFINER
--    AS $$
--DECLARE
--_uid uuid;
--BEGIN
--SELECT id
--INTO _uid
--FROM auth.users
--WHERE id = update_user_password.user_id
--  AND encrypted_password =
--      crypt(update_user_password.old_password, auth.users.encrypted_password);

--IF
--NOT FOUND THEN
--      RAISE EXCEPTION 'Invalid old password';
--END IF;

--UPDATE auth.users
--SET encrypted_password = crypt(update_user_password.new_password, gen_salt('bf'))
--WHERE id = user_id;
--END;
--$$;

--CREATE
--OR REPLACE FUNCTION public.update_user_password_recovery_token(recovery_token character varying, new_password character varying) RETURNS void
--    LANGUAGE plpgsql SECURITY DEFINER
--    AS $$BEGIN
--UPDATE auth.users AS u
--SET encrypted_password = crypt(new_password, gen_salt('bf')),
--    recovery_token     = '',
--    confirmation_token = '',
--    email_confirmed_at = NOW()
--WHERE u.recovery_token = update_user_password_recovery_token.recovery_token;

--IF
--NOT FOUND THEN
--        RAISE EXCEPTION 'Recovery token not found';
--END IF;
--END;$$;


--SET
--default_tablespace = '';

--SET
--default_table_access_method = heap;
--CREATE TABLE public."Address"
--(
--    "Id"          integer                NOT NULL,
--    "Street"      character varying(100),
--    "City"        character varying(100) NOT NULL,
--    "HouseNumber" character varying,
--    "ZipCode"     character varying
--);

--ALTER TABLE public."Address" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
--    SEQUENCE NAME public."Address_Id_seq"
--    START WITH 1
--    INCREMENT BY 1
--    NO MINVALUE
--    NO MAXVALUE
--    CACHE 1
--);

--CREATE TABLE public."AgeCategory"
--(
--    "Id"         integer                NOT NULL,
--    "Name"       character varying(100) NOT NULL,
--    "MinimumAge" integer,
--    "MaximumAge" integer
--);

--ALTER TABLE public."AgeCategory" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
--    SEQUENCE NAME public."AgeCategory_Id_seq"
--    START WITH 1
--    INCREMENT BY 1
--    NO MINVALUE
--    NO MAXVALUE
--    CACHE 1
--);

--CREATE TABLE public."Competition"
--(
--    "Id"            integer                NOT NULL,
--    "Name"          character varying(100) NOT NULL,
--    "Date"          timestamp(0) without time zone,
--    "AddressId"     integer,
--    "Active"        boolean DEFAULT false,
--    img_url         text,
--    "Information"   text,
--    url             text,
--    "RankingActive" boolean DEFAULT false
--);

--CREATE TABLE public."CompetitionPerCategory"
--(
--    "Id"            integer NOT NULL,
--    "Gender"        character(1),
--    "DistanceName"  character varying(30),
--    "DistanceInKm"  real,
--    "AgeCategoryId" integer,
--    "CompetitionId" integer,
--    "GunTime"       timestamp(0) without time zone
--);

--ALTER TABLE public."CompetitionPerCategory" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
--    SEQUENCE NAME public."CompetitionPerCategory_Id_seq"
--    START WITH 1
--    INCREMENT BY 1
--    NO MINVALUE
--    NO MAXVALUE
--    CACHE 1
--);

--ALTER TABLE public."Competition" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
--    SEQUENCE NAME public."Competition_Id_seq"
--    START WITH 1
--    INCREMENT BY 1
--    NO MINVALUE
--    NO MAXVALUE
--    CACHE 1
--);

--CREATE TABLE public."Person"
--(
--    "Id"         integer               NOT NULL,
--    "LastName"   character varying(50) NOT NULL,
--    "FirstName"  character varying(50) NOT NULL,
--    "BirthDate"  date                  NOT NULL,
--    "IBANNumber" character varying(30),
--    "SchoolId"   integer,
--    "AddressId"  integer,
--    "UserId"     uuid,
--    "Gender"     character varying DEFAULT ''::character varying,
--    "Email"      character varying
--);

--ALTER TABLE public."Person" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
--    SEQUENCE NAME public."Person_Id_seq"
--    START WITH 1
--    INCREMENT BY 1
--    NO MINVALUE
--    NO MAXVALUE
--    CACHE 1
--);

--CREATE TABLE public."Profile"
--(
--    id   uuid NOT NULL,
--    role character varying(255)
--);

--CREATE TABLE public."Registration"
--(
--    "Id"                       integer               NOT NULL,
--    "RunNumber"                smallint,
--    "RunTime"                  text,
--    "CompetitionPerCategoryId" integer,
--    "Paid"                     boolean DEFAULT false NOT NULL,
--    "PersonId"                 integer,
--    "CompetitionId"            integer
--);

--ALTER TABLE public."Registration" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
--    SEQUENCE NAME public."Registration_Id_seq"
--    START WITH 1
--    INCREMENT BY 1
--    NO MINVALUE
--    NO MAXVALUE
--    CACHE 1
--);

--CREATE TABLE public."School"
--(
--    "Id"   integer                NOT NULL,
--    "Name" character varying(100) NOT NULL
--);

--ALTER TABLE public."School" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
--    SEQUENCE NAME public."School_Id_seq"
--    START WITH 1
--    INCREMENT BY 1
--    NO MINVALUE
--    NO MAXVALUE
--    CACHE 1
--);

--CREATE VIEW public.personview WITH (security_invoker='true') AS
--SELECT "Person"."Id",
--       "Person"."LastName",
--       "Person"."FirstName",
--       "Person"."BirthDate",
--       "Person"."IBANNumber",
--       "Person"."SchoolId",
--       "Person"."AddressId",
--       "Person"."UserId",
--       "Person"."Gender",
--       "Person"."Email",
--       ((("Person"."FirstName")::text || ' '::text) || ("Person"."LastName")::text) AS fullname
--FROM public."Person";

--ALTER TABLE ONLY public."Address"
--    ADD CONSTRAINT "Address_pkey" PRIMARY KEY ("Id");

--ALTER TABLE ONLY public."AgeCategory"
--    ADD CONSTRAINT "AgeCategory_pkey" PRIMARY KEY ("Id");

--ALTER TABLE ONLY public."CompetitionPerCategory"
--    ADD CONSTRAINT "CompetitionPerCategory_pkey" PRIMARY KEY ("Id");

--ALTER TABLE ONLY public."Competition"
--    ADD CONSTRAINT "Competition_pkey" PRIMARY KEY ("Id");

--ALTER TABLE ONLY public."Person"
--    ADD CONSTRAINT "Person_UserId_key" UNIQUE ("UserId");

--ALTER TABLE ONLY public."Person"
--    ADD CONSTRAINT "Person_pkey" PRIMARY KEY ("Id");

--ALTER TABLE ONLY public."Profile"
--    ADD CONSTRAINT "Profiles_pkey" PRIMARY KEY (id);

--ALTER TABLE ONLY public."Registration"
--    ADD CONSTRAINT "Registration_pkey" PRIMARY KEY ("Id");

--ALTER TABLE ONLY public."School"
--    ADD CONSTRAINT "School_pkey" PRIMARY KEY ("Id");

--ALTER TABLE ONLY public."Registration"
--    ADD CONSTRAINT unique_runnumber_competition UNIQUE ("RunNumber", "CompetitionId");

--CREATE INDEX person_firstname_idx ON public."Person" USING btree ("FirstName");

--CREATE INDEX person_lastname_idx ON public."Person" USING btree ("LastName");

--CREATE TRIGGER after_person_delete
--    AFTER DELETE
--    ON public."Person"
--    FOR EACH ROW EXECUTE FUNCTION public.delete_from_person_details();

--ALTER TABLE ONLY public."CompetitionPerCategory"
--    ADD CONSTRAINT "CompetitionPerCategory_AgeCategoryId_fkey" FOREIGN KEY ("AgeCategoryId") REFERENCES public."AgeCategory"("Id");

--ALTER TABLE ONLY public."CompetitionPerCategory"
--    ADD CONSTRAINT "CompetitionPerCategory_CompetitionId_fkey" FOREIGN KEY ("CompetitionId") REFERENCES public."Competition"("Id") ON
--UPDATE CASCADE
--ON
--DELETE
--CASCADE;

--ALTER TABLE ONLY public."Competition"
--    ADD CONSTRAINT "Competition_AddressId_fkey" FOREIGN KEY ("AddressId") REFERENCES public."Address"("Id");

--ALTER TABLE ONLY public."Person"
--    ADD CONSTRAINT "Person_AddressId_fkey" FOREIGN KEY ("AddressId") REFERENCES public."Address"("Id");

--ALTER TABLE ONLY public."Person"
--    ADD CONSTRAINT "Person_SchoolId_fkey" FOREIGN KEY ("SchoolId") REFERENCES public."School"("Id");

--ALTER TABLE ONLY public."Person"
--    ADD CONSTRAINT "Person_UserId_fkey" FOREIGN KEY ("UserId") REFERENCES auth.users(id) ON
--DELETE
--CASCADE;

--ALTER TABLE ONLY public."Person"
--    ADD CONSTRAINT "Person_UserId_fkey1" FOREIGN KEY ("UserId") REFERENCES public."Profile"(id);

--ALTER TABLE ONLY public."Profile"
--    ADD CONSTRAINT "Profile_id_fkey" FOREIGN KEY (id) REFERENCES auth.users(id) ON
--DELETE
--CASCADE;

--ALTER TABLE ONLY public."Registration"
--    ADD CONSTRAINT "Registration_CompetitionId_fkey" FOREIGN KEY ("CompetitionId") REFERENCES public."Competition"("Id") ON
--DELETE
--CASCADE;

--ALTER TABLE ONLY public."Registration"
--    ADD CONSTRAINT "Registration_CompetitionPerCategoryId_fkey" FOREIGN KEY ("CompetitionPerCategoryId") REFERENCES public."CompetitionPerCategory"("Id") ON
--DELETE
--CASCADE;

--ALTER TABLE ONLY public."Registration"
--    ADD CONSTRAINT "Registration_PersonId_fkey" FOREIGN KEY ("PersonId") REFERENCES public."Person"("Id") ON
--DELETE
--CASCADE;

--ALTER TABLE public."Address" ENABLE ROW LEVEL SECURITY;
--ALTER TABLE public."AgeCategory" ENABLE ROW LEVEL SECURITY;
--ALTER TABLE public."Competition" ENABLE ROW LEVEL SECURITY;
--ALTER TABLE public."CompetitionPerCategory" ENABLE ROW LEVEL SECURITY;
--CREATE
--POLICY "Competitions are viewable by everyone" ON public."Address" FOR
--SELECT TO authenticated, anon USING (true);

--CREATE
--POLICY "Competitions are viewable by everyone" ON public."AgeCategory" FOR
--SELECT TO authenticated, anon USING (true);

--CREATE
--POLICY "Competitions are viewable by everyone" ON public."Competition" FOR
--SELECT TO authenticated, anon USING (true);

--CREATE
--POLICY "Competitions are viewable by everyone" ON public."CompetitionPerCategory" FOR
--SELECT TO authenticated, anon USING (true);

--CREATE
--POLICY "Competitions are viewable by everyone" ON public."Person" FOR
--SELECT TO authenticated, anon USING (true);

--CREATE
--POLICY "Competitions are viewable by everyone" ON public."Profile" FOR
--SELECT TO authenticated, anon USING (true);

--CREATE
--POLICY "Competitions are viewable by everyone" ON public."Registration" FOR
--SELECT TO authenticated, anon USING (true);

--CREATE
--POLICY "Competitions are viewable by everyone" ON public."School" FOR
--SELECT TO authenticated, anon USING (true);

--CREATE
--POLICY "Competitions can be deleted by everyone" ON public."Address" FOR DELETE
--TO authenticated, anon USING (true);

--CREATE
--POLICY "Competitions can be deleted by everyone" ON public."AgeCategory" FOR DELETE
--TO authenticated, anon USING (true);

--CREATE
--POLICY "Competitions can be deleted by everyone" ON public."Competition" FOR DELETE
--TO authenticated, anon USING (true);

--CREATE
--POLICY "Competitions can be deleted by everyone" ON public."CompetitionPerCategory" FOR DELETE
--TO authenticated, anon USING (true);

--CREATE
--POLICY "Competitions can be deleted by everyone" ON public."Person" FOR DELETE
--TO authenticated, anon USING (true);

--CREATE
--POLICY "Competitions can be deleted by everyone" ON public."Profile" FOR DELETE
--TO authenticated, anon USING (true);

--CREATE
--POLICY "Competitions can be deleted by everyone" ON public."Registration" FOR DELETE
--TO authenticated, anon USING (true);

--CREATE
--POLICY "Competitions can be deleted by everyone" ON public."School" FOR DELETE
--TO authenticated, anon USING (true);

--CREATE
--POLICY "Competitions can be inserted by everyone" ON public."Address" FOR INSERT TO authenticated, anon WITH CHECK (true);

--CREATE
--POLICY "Competitions can be inserted by everyone" ON public."AgeCategory" FOR INSERT TO authenticated, anon WITH CHECK (true);

--CREATE
--POLICY "Competitions can be inserted by everyone" ON public."Competition" FOR INSERT TO authenticated, anon WITH CHECK (true);

--CREATE
--POLICY "Competitions can be inserted by everyone" ON public."CompetitionPerCategory" FOR INSERT TO authenticated, anon WITH CHECK (true);

--CREATE
--POLICY "Competitions can be inserted by everyone" ON public."Person" FOR INSERT TO authenticated, anon WITH CHECK (true);

--CREATE
--POLICY "Competitions can be inserted by everyone" ON public."Profile" FOR INSERT TO authenticated, anon WITH CHECK (true);

--CREATE
--POLICY "Competitions can be inserted by everyone" ON public."Registration" FOR INSERT TO authenticated, anon WITH CHECK (true);

--CREATE
--POLICY "Competitions can be inserted by everyone" ON public."School" FOR INSERT TO authenticated, anon WITH CHECK (true);

--CREATE
--POLICY "Competitions can be updated by everyone" ON public."Address" FOR
--UPDATE TO authenticated, anon USING (true)
--WITH CHECK (true);

--CREATE
--POLICY "Competitions can be updated by everyone" ON public."AgeCategory" FOR
--UPDATE TO authenticated, anon USING (true)
--WITH CHECK (true);

--CREATE
--POLICY "Competitions can be updated by everyone" ON public."Competition" FOR
--UPDATE TO authenticated, anon USING (true)
--WITH CHECK (true);

--CREATE
--POLICY "Competitions can be updated by everyone" ON public."CompetitionPerCategory" FOR
--UPDATE TO authenticated, anon USING (true)
--WITH CHECK (true);

--CREATE
--POLICY "Competitions can be updated by everyone" ON public."Person" FOR
--UPDATE TO authenticated, anon USING (true)
--WITH CHECK (true);

--CREATE
--POLICY "Competitions can be updated by everyone" ON public."Profile" FOR
--UPDATE TO authenticated, anon USING (true)
--WITH CHECK (true);

--CREATE
--POLICY "Competitions can be updated by everyone" ON public."Registration" FOR
--UPDATE TO authenticated, anon USING (true)
--WITH CHECK (true);

--CREATE
--POLICY "Competitions can be updated by everyone" ON public."School" FOR
--UPDATE TO authenticated, anon USING (true)
--WITH CHECK (true);

--ALTER TABLE public."Person" ENABLE ROW LEVEL SECURITY;
--ALTER TABLE public."Profile" ENABLE ROW LEVEL SECURITY;
--ALTER TABLE public."Registration" ENABLE ROW LEVEL SECURITY;
--ALTER TABLE public."School" ENABLE ROW LEVEL SECURITY;

--INSERT INTO public."AgeCategory" ("Name", "MinimumAge", "MaximumAge")
--VALUES ('-35', 16, 35),
--       ('-45', 36, 45),
--       ('-55', 46, 55),
--       ('55+', 56, 200),
--       ('-16', 0, 15);
