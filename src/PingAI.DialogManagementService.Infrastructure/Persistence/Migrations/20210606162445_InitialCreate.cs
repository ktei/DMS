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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 44, 974, DateTimeKind.Utc).AddTicks(340)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 44, 974, DateTimeKind.Utc).AddTicks(710))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 45, 18, DateTimeKind.Utc).AddTicks(2350)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 45, 18, DateTimeKind.Utc).AddTicks(2940))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 45, 62, DateTimeKind.Utc).AddTicks(2840)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 45, 62, DateTimeKind.Utc).AddTicks(3590))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 45, 34, DateTimeKind.Utc).AddTicks(1770)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 45, 34, DateTimeKind.Utc).AddTicks(3100))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 44, 952, DateTimeKind.Utc).AddTicks(9780)),
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("b251c133-6311-426e-8906-3144180c1972")),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 44, 953, DateTimeKind.Utc).AddTicks(40))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 44, 977, DateTimeKind.Utc).AddTicks(7420)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 44, 977, DateTimeKind.Utc).AddTicks(8060))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 44, 982, DateTimeKind.Utc).AddTicks(7080)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 44, 982, DateTimeKind.Utc).AddTicks(7830))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 44, 992, DateTimeKind.Utc).AddTicks(570)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 44, 992, DateTimeKind.Utc).AddTicks(1330)),
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 45, 37, DateTimeKind.Utc).AddTicks(6380)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 45, 37, DateTimeKind.Utc).AddTicks(6950))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 45, 53, DateTimeKind.Utc).AddTicks(4850)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 45, 53, DateTimeKind.Utc).AddTicks(5250))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 45, 58, DateTimeKind.Utc).AddTicks(580)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 45, 58, DateTimeKind.Utc).AddTicks(1140))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 45, 60, DateTimeKind.Utc).AddTicks(1280)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 45, 60, DateTimeKind.Utc).AddTicks(2040))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 44, 984, DateTimeKind.Utc).AddTicks(6830)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 44, 984, DateTimeKind.Utc).AddTicks(7380))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 45, 22, DateTimeKind.Utc).AddTicks(6010)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 45, 22, DateTimeKind.Utc).AddTicks(6720))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 44, 949, DateTimeKind.Utc).AddTicks(1560)),
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("e3aa9477-778a-49bc-96e2-39402cae278a")),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 44, 949, DateTimeKind.Utc).AddTicks(3470))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 44, 988, DateTimeKind.Utc).AddTicks(250)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 44, 988, DateTimeKind.Utc).AddTicks(430))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 44, 951, DateTimeKind.Utc).AddTicks(1420)),
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("2d39708d-e99d-4183-b269-8cad2220601c")),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 16, 24, 44, 951, DateTimeKind.Utc).AddTicks(1650))
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
