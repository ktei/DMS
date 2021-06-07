using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "chatbot");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:chatbot.enum_ChatHistories_session_status", "CHATBOT,HANDOVER")
                .Annotation("Npgsql:Enum:chatbot.enum_Intents_type", "STANDARD,GENERIC")
                .Annotation("Npgsql:Enum:chatbot.enum_PhraseParts_type", "CONSTANT_ENTITY,ENTITY,TEXT")
                .Annotation("Npgsql:Enum:chatbot.enum_Response_type", "RTE,HANDOVER,VIDEO,SOCIAL,WEBHOOK,FORM,QUICK_REPLY");

            migrationBuilder.CreateTable(
                name: "ChatHistories",
                schema: "chatbot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    projectId = table.Column<Guid>(type: "uuid", nullable: false),
                    sessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    requestId = table.Column<Guid>(type: "uuid", nullable: false),
                    input = table.Column<string>(type: "jsonb", nullable: true),
                    output = table.Column<string>(type: "jsonb", nullable: true),
                    sessionStatus = table.Column<SessionStatus>(type: "chatbot.\"enum_ChatHistories_session_status\"", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 876, DateTimeKind.Utc).AddTicks(8730)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 876, DateTimeKind.Utc).AddTicks(9080))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatHistories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Organisations",
                schema: "chatbot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    tags = table.Column<string[]>(type: "text[]", nullable: true),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 918, DateTimeKind.Utc).AddTicks(7350)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 918, DateTimeKind.Utc).AddTicks(7880))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "chatbot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    auth0Id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 953, DateTimeKind.Utc).AddTicks(7660)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 953, DateTimeKind.Utc).AddTicks(8070))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "chatbot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    organisationId = table.Column<Guid>(type: "uuid", nullable: false),
                    widgetTitle = table.Column<string>(type: "text", nullable: true),
                    widgetColor = table.Column<string>(type: "text", nullable: true),
                    widgetDescription = table.Column<string>(type: "text", nullable: true),
                    fallbackMessage = table.Column<string>(type: "text", nullable: true),
                    enquiries = table.Column<string[]>(type: "text[]", nullable: true),
                    domains = table.Column<string[]>(type: "text[]", nullable: true),
                    businessTimezone = table.Column<string>(type: "text", nullable: false),
                    businessTimeStartUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    businessTimeEndUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    businessEmail = table.Column<string>(type: "text", nullable: true),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 930, DateTimeKind.Utc).AddTicks(4630)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 930, DateTimeKind.Utc).AddTicks(6130))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.id);
                    table.ForeignKey(
                        name: "FK_Projects_Organisations_organisationId",
                        column: x => x.organisationId,
                        principalSchema: "chatbot",
                        principalTable: "Organisations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationUsers",
                schema: "chatbot",
                columns: table => new
                {
                    organisationId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 858, DateTimeKind.Utc).AddTicks(570)),
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("73b627ea-3d5b-435c-9e8d-4356d135a6ba")),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 858, DateTimeKind.Utc).AddTicks(840))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationUsers", x => new { x.organisationId, x.userId });
                    table.ForeignKey(
                        name: "OrganisationUsers_organisationId_fkey",
                        column: x => x.organisationId,
                        principalSchema: "chatbot",
                        principalTable: "Organisations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "OrganisationUsers_userId_fkey",
                        column: x => x.userId,
                        principalSchema: "chatbot",
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityNames",
                schema: "chatbot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    projectId = table.Column<Guid>(type: "uuid", nullable: false),
                    canBeReferenced = table.Column<bool>(type: "boolean", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 879, DateTimeKind.Utc).AddTicks(8550)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 879, DateTimeKind.Utc).AddTicks(9240))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityNames", x => x.id);
                    table.ForeignKey(
                        name: "FK_EntityNames_Projects_projectId",
                        column: x => x.projectId,
                        principalSchema: "chatbot",
                        principalTable: "Projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityTypes",
                schema: "chatbot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    projectId = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    tags = table.Column<string[]>(type: "text[]", nullable: true),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 884, DateTimeKind.Utc).AddTicks(9260)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 885, DateTimeKind.Utc).AddTicks(150))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTypes", x => x.id);
                    table.ForeignKey(
                        name: "FK_EntityTypes_Projects_projectId",
                        column: x => x.projectId,
                        principalSchema: "chatbot",
                        principalTable: "Projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Intents",
                schema: "chatbot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    projectId = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<IntentType>(type: "chatbot.\"enum_Intents_type\"", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 893, DateTimeKind.Utc).AddTicks(8120)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 893, DateTimeKind.Utc).AddTicks(8830)),
                    color = table.Column<string>(type: "text", nullable: false),
                    iconName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intents", x => x.id);
                    table.ForeignKey(
                        name: "FK_Intents_Projects_projectId",
                        column: x => x.projectId,
                        principalSchema: "chatbot",
                        principalTable: "Projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectVersions",
                schema: "chatbot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organisationId = table.Column<Guid>(type: "uuid", nullable: false),
                    projectId = table.Column<Guid>(type: "uuid", nullable: false),
                    versionGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 933, DateTimeKind.Utc).AddTicks(9270)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 933, DateTimeKind.Utc).AddTicks(9930))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectVersions", x => x.id);
                    table.ForeignKey(
                        name: "FK_ProjectVersions_Projects_projectId",
                        column: x => x.projectId,
                        principalSchema: "chatbot",
                        principalTable: "Projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Queries",
                schema: "chatbot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    projectId = table.Column<Guid>(type: "uuid", nullable: false),
                    expressions = table.Column<Expression[]>(type: "jsonb", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    tags = table.Column<string[]>(type: "text[]", nullable: true),
                    displayOrder = table.Column<int>(type: "integer", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 947, DateTimeKind.Utc).AddTicks(6080)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 947, DateTimeKind.Utc).AddTicks(6480))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Queries", x => x.id);
                    table.ForeignKey(
                        name: "FK_Queries_Projects_projectId",
                        column: x => x.projectId,
                        principalSchema: "chatbot",
                        principalTable: "Projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Responses",
                schema: "chatbot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    resolution = table.Column<string>(type: "jsonb", nullable: false),
                    projectId = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<ResponseType>(type: "chatbot.\"enum_Response_type\"", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    speechContexts = table.Column<string>(type: "jsonb", nullable: true),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 951, DateTimeKind.Utc).AddTicks(2240)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 951, DateTimeKind.Utc).AddTicks(2750))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responses", x => x.id);
                    table.ForeignKey(
                        name: "FK_Responses_Projects_projectId",
                        column: x => x.projectId,
                        principalSchema: "chatbot",
                        principalTable: "Projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SlackWorkspaces",
                schema: "chatbot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    projectId = table.Column<Guid>(type: "uuid", nullable: false),
                    oauthAccessToken = table.Column<string>(type: "text", nullable: false),
                    webhookURL = table.Column<string>(type: "text", nullable: false),
                    teamId = table.Column<string>(type: "text", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 952, DateTimeKind.Utc).AddTicks(8800)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 952, DateTimeKind.Utc).AddTicks(9250))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlackWorkspaces", x => x.id);
                    table.ForeignKey(
                        name: "FK_SlackWorkspaces_Projects_projectId",
                        column: x => x.projectId,
                        principalSchema: "chatbot",
                        principalTable: "Projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityValues",
                schema: "chatbot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    entityTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    synonyms = table.Column<string[]>(type: "text[]", nullable: true),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 886, DateTimeKind.Utc).AddTicks(5500)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 886, DateTimeKind.Utc).AddTicks(5970))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityValues", x => x.id);
                    table.ForeignKey(
                        name: "FK_EntityValues_EntityTypes_entityTypeId",
                        column: x => x.entityTypeId,
                        principalSchema: "chatbot",
                        principalTable: "EntityTypes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhraseParts",
                schema: "chatbot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    intentId = table.Column<Guid>(type: "uuid", nullable: false),
                    phraseId = table.Column<Guid>(type: "uuid", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: true),
                    text = table.Column<string>(type: "text", nullable: true),
                    value = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<PhrasePartType>(type: "chatbot.\"enum_PhraseParts_type\"", nullable: false),
                    entityNameId = table.Column<Guid>(type: "uuid", nullable: true),
                    entityTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    displayOrder = table.Column<int>(type: "integer", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 923, DateTimeKind.Utc).AddTicks(20)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 923, DateTimeKind.Utc).AddTicks(730))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhraseParts", x => x.id);
                    table.ForeignKey(
                        name: "FK_PhraseParts_EntityNames_entityNameId",
                        column: x => x.entityNameId,
                        principalSchema: "chatbot",
                        principalTable: "EntityNames",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhraseParts_EntityTypes_entityTypeId",
                        column: x => x.entityTypeId,
                        principalSchema: "chatbot",
                        principalTable: "EntityTypes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhraseParts_Intents_intentId",
                        column: x => x.intentId,
                        principalSchema: "chatbot",
                        principalTable: "Intents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QueryIntents",
                schema: "chatbot",
                columns: table => new
                {
                    intentId = table.Column<Guid>(type: "uuid", nullable: false),
                    queryId = table.Column<Guid>(type: "uuid", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 855, DateTimeKind.Utc).AddTicks(9320)),
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("5349f49a-9aec-405e-ad57-37e708e86f0b")),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 856, DateTimeKind.Utc).AddTicks(80))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryIntents", x => new { x.intentId, x.queryId });
                    table.ForeignKey(
                        name: "QueryIntents_intentId_fkey",
                        column: x => x.intentId,
                        principalSchema: "chatbot",
                        principalTable: "Intents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "QueryIntents_queryId_fkey",
                        column: x => x.queryId,
                        principalSchema: "chatbot",
                        principalTable: "Queries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GreetingResponses",
                schema: "chatbot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    projectId = table.Column<Guid>(type: "uuid", nullable: false),
                    responseId = table.Column<Guid>(type: "uuid", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 889, DateTimeKind.Utc).AddTicks(8290)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 889, DateTimeKind.Utc).AddTicks(8500))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GreetingResponses", x => x.id);
                    table.ForeignKey(
                        name: "FK_GreetingResponses_Projects_projectId",
                        column: x => x.projectId,
                        principalSchema: "chatbot",
                        principalTable: "Projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GreetingResponses_Responses_responseId",
                        column: x => x.responseId,
                        principalSchema: "chatbot",
                        principalTable: "Responses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QueryResponses",
                schema: "chatbot",
                columns: table => new
                {
                    queryId = table.Column<Guid>(type: "uuid", nullable: false),
                    responseId = table.Column<Guid>(type: "uuid", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 856, DateTimeKind.Utc).AddTicks(8380)),
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("672116f1-c07b-4709-ba9c-98a71e76df65")),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 7, 9, 57, 36, 856, DateTimeKind.Utc).AddTicks(8600))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryResponses", x => new { x.queryId, x.responseId });
                    table.ForeignKey(
                        name: "QueryResponses_queryId_fkey",
                        column: x => x.queryId,
                        principalSchema: "chatbot",
                        principalTable: "Queries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "QueryResponses_responseId_fkey",
                        column: x => x.responseId,
                        principalSchema: "chatbot",
                        principalTable: "Responses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityNames_projectId",
                schema: "chatbot",
                table: "EntityNames",
                column: "projectId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityTypes_projectId",
                schema: "chatbot",
                table: "EntityTypes",
                column: "projectId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityValues_entityTypeId",
                schema: "chatbot",
                table: "EntityValues",
                column: "entityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GreetingResponses_projectId",
                schema: "chatbot",
                table: "GreetingResponses",
                column: "projectId");

            migrationBuilder.CreateIndex(
                name: "IX_GreetingResponses_responseId",
                schema: "chatbot",
                table: "GreetingResponses",
                column: "responseId");

            migrationBuilder.CreateIndex(
                name: "IX_Intents_projectId",
                schema: "chatbot",
                table: "Intents",
                column: "projectId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationUsers_userId",
                schema: "chatbot",
                table: "OrganisationUsers",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_PhraseParts_entityNameId",
                schema: "chatbot",
                table: "PhraseParts",
                column: "entityNameId");

            migrationBuilder.CreateIndex(
                name: "IX_PhraseParts_entityTypeId",
                schema: "chatbot",
                table: "PhraseParts",
                column: "entityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PhraseParts_intentId",
                schema: "chatbot",
                table: "PhraseParts",
                column: "intentId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_organisationId",
                schema: "chatbot",
                table: "Projects",
                column: "organisationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectVersions_projectId",
                schema: "chatbot",
                table: "ProjectVersions",
                column: "projectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Queries_projectId",
                schema: "chatbot",
                table: "Queries",
                column: "projectId");

            migrationBuilder.CreateIndex(
                name: "IX_QueryIntents_queryId",
                schema: "chatbot",
                table: "QueryIntents",
                column: "queryId");

            migrationBuilder.CreateIndex(
                name: "IX_QueryResponses_responseId",
                schema: "chatbot",
                table: "QueryResponses",
                column: "responseId");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_projectId",
                schema: "chatbot",
                table: "Responses",
                column: "projectId");

            migrationBuilder.CreateIndex(
                name: "IX_SlackWorkspaces_projectId",
                schema: "chatbot",
                table: "SlackWorkspaces",
                column: "projectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatHistories",
                schema: "chatbot");

            migrationBuilder.DropTable(
                name: "EntityValues",
                schema: "chatbot");

            migrationBuilder.DropTable(
                name: "GreetingResponses",
                schema: "chatbot");

            migrationBuilder.DropTable(
                name: "OrganisationUsers",
                schema: "chatbot");

            migrationBuilder.DropTable(
                name: "PhraseParts",
                schema: "chatbot");

            migrationBuilder.DropTable(
                name: "ProjectVersions",
                schema: "chatbot");

            migrationBuilder.DropTable(
                name: "QueryIntents",
                schema: "chatbot");

            migrationBuilder.DropTable(
                name: "QueryResponses",
                schema: "chatbot");

            migrationBuilder.DropTable(
                name: "SlackWorkspaces",
                schema: "chatbot");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "chatbot");

            migrationBuilder.DropTable(
                name: "EntityNames",
                schema: "chatbot");

            migrationBuilder.DropTable(
                name: "EntityTypes",
                schema: "chatbot");

            migrationBuilder.DropTable(
                name: "Intents",
                schema: "chatbot");

            migrationBuilder.DropTable(
                name: "Queries",
                schema: "chatbot");

            migrationBuilder.DropTable(
                name: "Responses",
                schema: "chatbot");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "chatbot");

            migrationBuilder.DropTable(
                name: "Organisations",
                schema: "chatbot");
        }
    }
}
