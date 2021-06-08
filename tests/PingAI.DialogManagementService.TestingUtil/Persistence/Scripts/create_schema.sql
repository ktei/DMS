--
-- PostgreSQL database dump
--

-- Dumped from database version 12.3
-- Dumped by pg_dump version 12.4

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: chatbot; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA chatbot;


--
-- Name: enum_ChatHistories_session_status; Type: TYPE; Schema: chatbot; Owner: -
--

CREATE TYPE chatbot."enum_ChatHistories_session_status" AS ENUM (
    'CHATBOT',
    'HANDOVER'
);


--
-- Name: enum_Intents_type; Type: TYPE; Schema: chatbot; Owner: -
--

CREATE TYPE chatbot."enum_Intents_type" AS ENUM (
    'GENERIC',
    'STANDARD'
);


--
-- Name: enum_PhraseParts_type; Type: TYPE; Schema: chatbot; Owner: -
--

CREATE TYPE chatbot."enum_PhraseParts_type" AS ENUM (
    'CONSTANT_ENTITY',
    'ENTITY',
    'TEXT'
);


--
-- Name: enum_Response_type; Type: TYPE; Schema: chatbot; Owner: -
--

CREATE TYPE chatbot."enum_Response_type" AS ENUM (
    'RTE',
    'HANDOVER',
    'VIDEO',
    'SOCIAL',
    'WEBHOOK',
    'FORM',
    'QUICK_REPLY'
);


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: ChatHistories; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."ChatHistories" (
                                         id uuid NOT NULL,
                                         "projectId" uuid NOT NULL,
                                         "sessionId" uuid NOT NULL,
                                         input jsonb,
                                         output jsonb,
                                         "createdAt" timestamp with time zone NOT NULL,
                                         "updatedAt" timestamp with time zone NOT NULL,
                                         "requestId" uuid NOT NULL,
                                         "sessionStatus" chatbot."enum_ChatHistories_session_status" NOT NULL
);


--
-- Name: EntityNameMappings; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."EntityNameMappings" (
                                              id uuid NOT NULL,
                                              "entityNameId" uuid NOT NULL,
                                              "fromEntityNameId" uuid NOT NULL,
                                              "responseId" uuid NOT NULL,
                                              "intentId" uuid NOT NULL,
                                              "createdAt" timestamp with time zone NOT NULL,
                                              "updatedAt" timestamp with time zone NOT NULL
);


--
-- Name: EntityNames; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."EntityNames" (
                                       id uuid NOT NULL,
                                       name character varying(255) NOT NULL,
                                       "projectId" uuid NOT NULL,
                                       "canBeReferenced" boolean NOT NULL,
                                       "createdAt" timestamp with time zone NOT NULL,
                                       "updatedAt" timestamp with time zone NOT NULL
);


--
-- Name: EntityTypes; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."EntityTypes" (
                                       id uuid NOT NULL,
                                       name character varying(255) NOT NULL,
                                       "projectId" uuid NOT NULL,
                                       description character varying(255) NOT NULL,
                                       tags character varying[],
                                       "createdAt" timestamp with time zone NOT NULL,
                                       "updatedAt" timestamp with time zone NOT NULL
);


--
-- Name: EntityValues; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."EntityValues" (
                                        id uuid NOT NULL,
                                        value character varying(255) NOT NULL,
                                        synonyms character varying[],
                                        "entityTypeId" uuid NOT NULL,
                                        "createdAt" timestamp with time zone NOT NULL,
                                        "updatedAt" timestamp with time zone NOT NULL
);


--
-- Name: GreetingResponses; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."GreetingResponses" (
                                             id uuid NOT NULL,
                                             "projectId" uuid NOT NULL,
                                             "responseId" uuid NOT NULL,
                                             "createdAt" timestamp with time zone NOT NULL,
                                             "updatedAt" timestamp with time zone NOT NULL
);


--
-- Name: Intents; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."Intents" (
                                   id uuid NOT NULL,
                                   name character varying(255) NOT NULL,
                                   "iconName" character varying(255) NOT NULL,
                                   color character varying(255) NOT NULL,
                                   "projectId" uuid NOT NULL,
                                   type chatbot."enum_Intents_type" NOT NULL,
                                   "createdAt" timestamp with time zone NOT NULL,
                                   "updatedAt" timestamp with time zone NOT NULL
);


--
-- Name: OrganisationUsers; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."OrganisationUsers" (
                                             id uuid NOT NULL,
                                             "userId" uuid NOT NULL,
                                             "organisationId" uuid NOT NULL,
                                             "createdAt" timestamp with time zone NOT NULL,
                                             "updatedAt" timestamp with time zone NOT NULL
);


--
-- Name: Organisations; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."Organisations" (
                                         id uuid NOT NULL,
                                         name character varying(255) NOT NULL,
                                         description character varying(255) NOT NULL,
                                         tags character varying[],
                                         "createdAt" timestamp with time zone NOT NULL,
                                         "updatedAt" timestamp with time zone NOT NULL
);


--
-- Name: PhraseParts; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."PhraseParts" (
                                       id uuid NOT NULL,
                                       "position" integer,
                                       "phraseId" uuid NOT NULL,
                                       "entityNameId" uuid,
                                       text character varying(255),
                                       value character varying(255),
                                       type chatbot."enum_PhraseParts_type" NOT NULL,
                                       "intentId" uuid NOT NULL,
                                       "entityTypeId" uuid,
                                       "createdAt" timestamp with time zone NOT NULL,
                                       "updatedAt" timestamp with time zone NOT NULL,
                                       "displayOrder" integer DEFAULT 0 NOT NULL
);


--
-- Name: ProjectVersions; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."ProjectVersions" (
                                           id uuid NOT NULL,
                                           "projectId" uuid NOT NULL,
                                           "versionGroupId" uuid NOT NULL,
                                           version integer NOT NULL,
                                           "createdAt" timestamp with time zone NOT NULL,
                                           "updatedAt" timestamp with time zone NOT NULL,
                                           "organisationId" uuid NOT NULL
);


--
-- Name: Projects; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."Projects" (
                                    id uuid NOT NULL,
                                    name character varying(255) NOT NULL,
                                    "organisationId" uuid,
                                    "createdAt" timestamp with time zone NOT NULL,
                                    "updatedAt" timestamp with time zone NOT NULL,
                                    "widgetTitle" character varying(255),
                                    "widgetColor" character(7) DEFAULT '#ffffff'::bpchar NOT NULL,
                                    "widgetDescription" character varying(255),
                                    "fallbackMessage" character varying(255),
                                    "greetingMessage" character varying(255),
                                    enquiries character varying[],
                                    domains character varying[],
                                    "apiKey" character(32),
                                    "businessTimeStartUtc" timestamp with time zone,
                                    "businessTimeEndUtc" timestamp with time zone,
                                    "businessTimezone" character varying(255) DEFAULT 'Australia/Sydney'::character varying NOT NULL,
                                    "businessEmail" character varying(255)
);


--
-- Name: Queries; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."Queries" (
                                   id uuid NOT NULL,
                                   expressions jsonb NOT NULL,
                                   "projectId" uuid NOT NULL,
                                   name character varying(255) NOT NULL,
                                   description character varying(255) NOT NULL,
                                   tags character varying[],
                                   "displayOrder" integer NOT NULL,
                                   "createdAt" timestamp with time zone NOT NULL,
                                   "updatedAt" timestamp with time zone NOT NULL
);


--
-- Name: QueryIntents; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."QueryIntents" (
                                        id uuid NOT NULL,
                                        "queryId" uuid NOT NULL,
                                        "intentId" uuid NOT NULL,
                                        "createdAt" timestamp with time zone NOT NULL,
                                        "updatedAt" timestamp with time zone NOT NULL
);


--
-- Name: QueryResponses; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."QueryResponses" (
                                          id uuid NOT NULL,
                                          "queryId" uuid NOT NULL,
                                          "responseId" uuid NOT NULL,
                                          "createdAt" timestamp with time zone NOT NULL,
                                          "updatedAt" timestamp with time zone NOT NULL
);


--
-- Name: ResponseQueries; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."ResponseQueries" (
                                           id uuid NOT NULL,
                                           "queryId" uuid NOT NULL,
                                           "responseId" uuid NOT NULL,
                                           "createdAt" timestamp with time zone NOT NULL,
                                           "updatedAt" timestamp with time zone NOT NULL
);


--
-- Name: Responses; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."Responses" (
                                     id uuid NOT NULL,
                                     resolution jsonb NOT NULL,
                                     "projectId" uuid NOT NULL,
                                     "createdAt" timestamp with time zone NOT NULL,
                                     "updatedAt" timestamp with time zone NOT NULL,
                                     type chatbot."enum_Response_type" NOT NULL,
                                     "order" integer NOT NULL,
                                     "speechContexts" jsonb
);


--
-- Name: SlackChannels; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."SlackChannels" (
                                         id uuid NOT NULL,
                                         "sessionId" uuid NOT NULL,
                                         "workspaceId" uuid NOT NULL,
                                         "channelName" character varying(255) NOT NULL,
                                         "channelId" character varying(255) NOT NULL,
                                         "createdAt" timestamp with time zone NOT NULL,
                                         "updatedAt" timestamp with time zone NOT NULL
);


--
-- Name: SlackWorkspaces; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."SlackWorkspaces" (
                                           id uuid NOT NULL,
                                           "oauthAccessToken" character varying(255) NOT NULL,
                                           "webhookURL" character varying(255) NOT NULL,
                                           "projectId" uuid,
                                           "createdAt" timestamp with time zone NOT NULL,
                                           "updatedAt" timestamp with time zone NOT NULL,
                                           "teamId" character varying(255) NOT NULL
);


--
-- Name: Users; Type: TABLE; Schema: chatbot; Owner: -
--

CREATE TABLE chatbot."Users" (
                                 id uuid NOT NULL,
                                 name character varying(255) NOT NULL,
                                 "auth0Id" character varying(255) NOT NULL,
                                 "createdAt" timestamp with time zone NOT NULL,
                                 "updatedAt" timestamp with time zone NOT NULL
);


--
-- Name: ChatHistories ChatHistories_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."ChatHistories"
    ADD CONSTRAINT "ChatHistories_pkey" PRIMARY KEY (id);


--
-- Name: EntityNameMappings EntityNameMappings_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."EntityNameMappings"
    ADD CONSTRAINT "EntityNameMappings_pkey" PRIMARY KEY (id);


--
-- Name: EntityNames EntityNames_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."EntityNames"
    ADD CONSTRAINT "EntityNames_pkey" PRIMARY KEY (id);


--
-- Name: EntityTypes EntityTypes_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."EntityTypes"
    ADD CONSTRAINT "EntityTypes_pkey" PRIMARY KEY (id);


--
-- Name: EntityValues EntityValues_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."EntityValues"
    ADD CONSTRAINT "EntityValues_pkey" PRIMARY KEY (id);


--
-- Name: GreetingResponses GreetingResponses_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."GreetingResponses"
    ADD CONSTRAINT "GreetingResponses_pkey" PRIMARY KEY (id);


--
-- Name: Intents Intents_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."Intents"
    ADD CONSTRAINT "Intents_pkey" PRIMARY KEY (id);


--
-- Name: OrganisationUsers OrganisationUsers_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."OrganisationUsers"
    ADD CONSTRAINT "OrganisationUsers_pkey" PRIMARY KEY (id);


--
-- Name: Organisations Organisations_name_key; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."Organisations"
    ADD CONSTRAINT "Organisations_name_key" UNIQUE (name);


--
-- Name: Organisations Organisations_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."Organisations"
    ADD CONSTRAINT "Organisations_pkey" PRIMARY KEY (id);


--
-- Name: PhraseParts PhraseParts_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."PhraseParts"
    ADD CONSTRAINT "PhraseParts_pkey" PRIMARY KEY (id);


--
-- Name: ProjectVersions ProjectVersions_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."ProjectVersions"
    ADD CONSTRAINT "ProjectVersions_pkey" PRIMARY KEY (id);


--
-- Name: Projects Projects_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."Projects"
    ADD CONSTRAINT "Projects_pkey" PRIMARY KEY (id);


--
-- Name: Projects Projects_uniq_name_per_org; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."Projects"
    ADD CONSTRAINT "Projects_uniq_name_per_org" UNIQUE ("organisationId", name);


--
-- Name: Queries Queries_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."Queries"
    ADD CONSTRAINT "Queries_pkey" PRIMARY KEY (id);


--
-- Name: QueryIntents QueryIntents_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."QueryIntents"
    ADD CONSTRAINT "QueryIntents_pkey" PRIMARY KEY (id);


--
-- Name: QueryResponses QueryResponses_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."QueryResponses"
    ADD CONSTRAINT "QueryResponses_pkey" PRIMARY KEY (id);


--
-- Name: ResponseQueries ResponseQueries_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."ResponseQueries"
    ADD CONSTRAINT "ResponseQueries_pkey" PRIMARY KEY (id);


--
-- Name: Responses Responses_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."Responses"
    ADD CONSTRAINT "Responses_pkey" PRIMARY KEY (id);


--
-- Name: SlackChannels SlackChannels_channelId_workspaceId_key; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."SlackChannels"
    ADD CONSTRAINT "SlackChannels_channelId_workspaceId_key" UNIQUE ("channelId", "workspaceId");


--
-- Name: SlackChannels SlackChannels_channelName_workspaceId_key; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."SlackChannels"
    ADD CONSTRAINT "SlackChannels_channelName_workspaceId_key" UNIQUE ("channelName", "workspaceId");


--
-- Name: SlackChannels SlackChannels_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."SlackChannels"
    ADD CONSTRAINT "SlackChannels_pkey" PRIMARY KEY (id);


--
-- Name: SlackChannels SlackChannels_sessionId_key; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."SlackChannels"
    ADD CONSTRAINT "SlackChannels_sessionId_key" UNIQUE ("sessionId");


--
-- Name: SlackWorkspaces SlackWorkspaces_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."SlackWorkspaces"
    ADD CONSTRAINT "SlackWorkspaces_pkey" PRIMARY KEY (id);


--
-- Name: SlackWorkspaces SlackWorkspaces_projectId_key; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."SlackWorkspaces"
    ADD CONSTRAINT "SlackWorkspaces_projectId_key" UNIQUE ("projectId");


--
-- Name: Users Users_name_key; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."Users"
    ADD CONSTRAINT "Users_name_key" UNIQUE (name);


--
-- Name: Users Users_pkey; Type: CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."Users"
    ADD CONSTRAINT "Users_pkey" PRIMARY KEY (id);


--
-- Name: ChatHistory-createdAt-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "ChatHistory-createdAt-idx" ON chatbot."ChatHistories" USING btree ("createdAt");


--
-- Name: ChatHistory-projectId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "ChatHistory-projectId-idx" ON chatbot."ChatHistories" USING btree ("projectId");


--
-- Name: EntityNames-projectId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "EntityNames-projectId-idx" ON chatbot."EntityNames" USING btree ("projectId");


--
-- Name: EntityTypes-projectId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "EntityTypes-projectId-idx" ON chatbot."EntityTypes" USING btree ("projectId");


--
-- Name: EntityValues-entityTypeId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "EntityValues-entityTypeId-idx" ON chatbot."EntityValues" USING btree ("entityTypeId");


--
-- Name: OrganisationUsers-orgasnisationId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "OrganisationUsers-orgasnisationId-idx" ON chatbot."OrganisationUsers" USING btree ("organisationId");


--
-- Name: OrganisationUsers-projectId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "OrganisationUsers-projectId-idx" ON chatbot."Intents" USING btree ("projectId");


--
-- Name: PhraseParts-entityNameId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "PhraseParts-entityNameId-idx" ON chatbot."PhraseParts" USING btree ("entityNameId");


--
-- Name: PhraseParts-entityTypeId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "PhraseParts-entityTypeId-idx" ON chatbot."PhraseParts" USING btree ("entityTypeId");


--
-- Name: PhraseParts-intentId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "PhraseParts-intentId-idx" ON chatbot."PhraseParts" USING btree ("intentId");


--
-- Name: ProjectVersions-projectId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "ProjectVersions-projectId-idx" ON chatbot."ProjectVersions" USING btree ("projectId");


--
-- Name: Projects-orgasnisationId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "Projects-orgasnisationId-idx" ON chatbot."Projects" USING btree ("organisationId");


--
-- Name: Queries-projectId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "Queries-projectId-idx" ON chatbot."Queries" USING btree ("projectId");


--
-- Name: QueryIntents-intentId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "QueryIntents-intentId-idx" ON chatbot."QueryIntents" USING btree ("intentId");


--
-- Name: QueryIntents-queryId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "QueryIntents-queryId-idx" ON chatbot."QueryIntents" USING btree ("queryId");


--
-- Name: QueryResponses-queryId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "QueryResponses-queryId-idx" ON chatbot."QueryResponses" USING btree ("queryId");


--
-- Name: QueryResponses-responseId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "QueryResponses-responseId-idx" ON chatbot."QueryResponses" USING btree ("responseId");


--
-- Name: ResponseQueries-queryId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "ResponseQueries-queryId-idx" ON chatbot."ResponseQueries" USING btree ("queryId");


--
-- Name: ResponseQueries-responseId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "ResponseQueries-responseId-idx" ON chatbot."ResponseQueries" USING btree ("responseId");


--
-- Name: Responses-projectId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "Responses-projectId-idx" ON chatbot."Responses" USING btree ("projectId");


--
-- Name: SlackWorkspaces-projectId-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "SlackWorkspaces-projectId-idx" ON chatbot."SlackWorkspaces" USING btree ("projectId");


--
-- Name: apikey-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "apikey-idx" ON chatbot."Projects" USING btree ("apiKey");


--
-- Name: auth0Id_idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE UNIQUE INDEX "auth0Id_idx" ON chatbot."Users" USING btree ("auth0Id");


--
-- Name: entity-name-name-unique; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE UNIQUE INDEX "entity-name-name-unique" ON chatbot."EntityNames" USING btree (name, "projectId");


--
-- Name: entity-type-name-unique; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE UNIQUE INDEX "entity-type-name-unique" ON chatbot."EntityTypes" USING btree (name, "projectId");


--
-- Name: entity-value-value-unique; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE UNIQUE INDEX "entity-value-value-unique" ON chatbot."EntityValues" USING btree (value, "entityTypeId");


--
-- Name: followup-unique; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE UNIQUE INDEX "followup-unique" ON chatbot."ResponseQueries" USING btree ("queryId", "responseId");


--
-- Name: greeting-response-unique; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE UNIQUE INDEX "greeting-response-unique" ON chatbot."GreetingResponses" USING btree ("projectId", "responseId");


--
-- Name: intent-name-unique; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE UNIQUE INDEX "intent-name-unique" ON chatbot."Intents" USING btree (name, "projectId");


--
-- Name: mapping-unique; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE UNIQUE INDEX "mapping-unique" ON chatbot."EntityNameMappings" USING btree ("fromEntityNameId", "responseId", "intentId");


--
-- Name: organisation-user-unique; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE UNIQUE INDEX "organisation-user-unique" ON chatbot."OrganisationUsers" USING btree ("userId", "organisationId");


--
-- Name: project-version-unique; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE UNIQUE INDEX "project-version-unique" ON chatbot."ProjectVersions" USING btree ("versionGroupId", version);


--
-- Name: projectid-unique; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE UNIQUE INDEX "projectid-unique" ON chatbot."ProjectVersions" USING btree ("versionGroupId", "projectId");


--
-- Name: query-intent-unique; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE UNIQUE INDEX "query-intent-unique" ON chatbot."QueryIntents" USING btree ("queryId", "intentId");


--
-- Name: query-name-unique; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE UNIQUE INDEX "query-name-unique" ON chatbot."Queries" USING btree ("projectId", name);


--
-- Name: response-unique; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE UNIQUE INDEX "response-unique" ON chatbot."QueryResponses" USING btree ("queryId", "responseId");


--
-- Name: versiongroupid-idx; Type: INDEX; Schema: chatbot; Owner: -
--

CREATE INDEX "versiongroupid-idx" ON chatbot."ProjectVersions" USING btree ("versionGroupId");


--
-- Name: ChatHistories ChatHistories_projectId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."ChatHistories"
    ADD CONSTRAINT "ChatHistories_projectId_fkey" FOREIGN KEY ("projectId") REFERENCES chatbot."Projects"(id) ON UPDATE CASCADE;


--
-- Name: EntityNameMappings EntityNameMappings_entityNameId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."EntityNameMappings"
    ADD CONSTRAINT "EntityNameMappings_entityNameId_fkey" FOREIGN KEY ("entityNameId") REFERENCES chatbot."EntityNames"(id) ON UPDATE CASCADE;


--
-- Name: EntityNameMappings EntityNameMappings_fromEntityNameId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."EntityNameMappings"
    ADD CONSTRAINT "EntityNameMappings_fromEntityNameId_fkey" FOREIGN KEY ("fromEntityNameId") REFERENCES chatbot."EntityNames"(id) ON UPDATE CASCADE;


--
-- Name: EntityNameMappings EntityNameMappings_intentId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."EntityNameMappings"
    ADD CONSTRAINT "EntityNameMappings_intentId_fkey" FOREIGN KEY ("intentId") REFERENCES chatbot."Intents"(id) ON UPDATE CASCADE;


--
-- Name: EntityNameMappings EntityNameMappings_responseId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."EntityNameMappings"
    ADD CONSTRAINT "EntityNameMappings_responseId_fkey" FOREIGN KEY ("responseId") REFERENCES chatbot."Responses"(id) ON UPDATE CASCADE;


--
-- Name: EntityNames EntityNames_projectId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."EntityNames"
    ADD CONSTRAINT "EntityNames_projectId_fkey" FOREIGN KEY ("projectId") REFERENCES chatbot."Projects"(id) ON UPDATE CASCADE;


--
-- Name: EntityTypes EntityTypes_projectId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."EntityTypes"
    ADD CONSTRAINT "EntityTypes_projectId_fkey" FOREIGN KEY ("projectId") REFERENCES chatbot."Projects"(id) ON UPDATE CASCADE;


--
-- Name: EntityValues EntityValues_entityTypeId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."EntityValues"
    ADD CONSTRAINT "EntityValues_entityTypeId_fkey" FOREIGN KEY ("entityTypeId") REFERENCES chatbot."EntityTypes"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: GreetingResponses GreetingResponses_projectId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."GreetingResponses"
    ADD CONSTRAINT "GreetingResponses_projectId_fkey" FOREIGN KEY ("projectId") REFERENCES chatbot."Projects"(id);


--
-- Name: GreetingResponses GreetingResponses_responseId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."GreetingResponses"
    ADD CONSTRAINT "GreetingResponses_responseId_fkey" FOREIGN KEY ("responseId") REFERENCES chatbot."Responses"(id);


--
-- Name: Intents Intents_projectId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."Intents"
    ADD CONSTRAINT "Intents_projectId_fkey" FOREIGN KEY ("projectId") REFERENCES chatbot."Projects"(id) ON UPDATE CASCADE;


--
-- Name: OrganisationUsers OrganisationUsers_organisationId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."OrganisationUsers"
    ADD CONSTRAINT "OrganisationUsers_organisationId_fkey" FOREIGN KEY ("organisationId") REFERENCES chatbot."Organisations"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: OrganisationUsers OrganisationUsers_userId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."OrganisationUsers"
    ADD CONSTRAINT "OrganisationUsers_userId_fkey" FOREIGN KEY ("userId") REFERENCES chatbot."Users"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: PhraseParts PhraseParts_entityNameId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."PhraseParts"
    ADD CONSTRAINT "PhraseParts_entityNameId_fkey" FOREIGN KEY ("entityNameId") REFERENCES chatbot."EntityNames"(id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- Name: PhraseParts PhraseParts_entityTypeId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."PhraseParts"
    ADD CONSTRAINT "PhraseParts_entityTypeId_fkey" FOREIGN KEY ("entityTypeId") REFERENCES chatbot."EntityTypes"(id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- Name: PhraseParts PhraseParts_intentId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."PhraseParts"
    ADD CONSTRAINT "PhraseParts_intentId_fkey" FOREIGN KEY ("intentId") REFERENCES chatbot."Intents"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: ProjectVersions ProjectVersions_organisationId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."ProjectVersions"
    ADD CONSTRAINT "ProjectVersions_organisationId_fkey" FOREIGN KEY ("organisationId") REFERENCES chatbot."Organisations"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: ProjectVersions ProjectVersions_projectId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."ProjectVersions"
    ADD CONSTRAINT "ProjectVersions_projectId_fkey" FOREIGN KEY ("projectId") REFERENCES chatbot."Projects"(id);


--
-- Name: Projects Projects_organisationId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."Projects"
    ADD CONSTRAINT "Projects_organisationId_fkey" FOREIGN KEY ("organisationId") REFERENCES chatbot."Organisations"(id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- Name: Queries Queries_projectId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."Queries"
    ADD CONSTRAINT "Queries_projectId_fkey" FOREIGN KEY ("projectId") REFERENCES chatbot."Projects"(id) ON UPDATE CASCADE;


--
-- Name: QueryIntents QueryIntents_intentId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."QueryIntents"
    ADD CONSTRAINT "QueryIntents_intentId_fkey" FOREIGN KEY ("intentId") REFERENCES chatbot."Intents"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: QueryIntents QueryIntents_queryId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."QueryIntents"
    ADD CONSTRAINT "QueryIntents_queryId_fkey" FOREIGN KEY ("queryId") REFERENCES chatbot."Queries"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: QueryResponses QueryResponses_queryId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."QueryResponses"
    ADD CONSTRAINT "QueryResponses_queryId_fkey" FOREIGN KEY ("queryId") REFERENCES chatbot."Queries"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: QueryResponses QueryResponses_responseId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."QueryResponses"
    ADD CONSTRAINT "QueryResponses_responseId_fkey" FOREIGN KEY ("responseId") REFERENCES chatbot."Responses"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: ResponseQueries ResponseQueries_queryId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."ResponseQueries"
    ADD CONSTRAINT "ResponseQueries_queryId_fkey" FOREIGN KEY ("queryId") REFERENCES chatbot."Queries"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: ResponseQueries ResponseQueries_responseId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."ResponseQueries"
    ADD CONSTRAINT "ResponseQueries_responseId_fkey" FOREIGN KEY ("responseId") REFERENCES chatbot."Responses"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: Responses Responses_projectId_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."Responses"
    ADD CONSTRAINT "Responses_projectId_fkey" FOREIGN KEY ("projectId") REFERENCES chatbot."Projects"(id) ON UPDATE CASCADE;


--
-- Name: SlackChannels slackchannels_workspaceid_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."SlackChannels"
    ADD CONSTRAINT slackchannels_workspaceid_fkey FOREIGN KEY ("workspaceId") REFERENCES chatbot."SlackWorkspaces"(id);


--
-- Name: SlackWorkspaces slackworkspaces_projectid_fkey; Type: FK CONSTRAINT; Schema: chatbot; Owner: -
--

ALTER TABLE ONLY chatbot."SlackWorkspaces"
    ADD CONSTRAINT slackworkspaces_projectid_fkey FOREIGN KEY ("projectId") REFERENCES chatbot."Projects"(id);


--
-- PostgreSQL database dump complete
--

