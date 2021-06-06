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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 933, DateTimeKind.Utc).AddTicks(5090)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 933, DateTimeKind.Utc).AddTicks(5320))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 956, DateTimeKind.Utc).AddTicks(6510)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 956, DateTimeKind.Utc).AddTicks(7520))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 46, 19, DateTimeKind.Utc).AddTicks(2700)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 46, 19, DateTimeKind.Utc).AddTicks(3240))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationUsers",
                schema: "chatbot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organisationId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 959, DateTimeKind.Utc).AddTicks(8960)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 959, DateTimeKind.Utc).AddTicks(9580))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationUsers", x => x.id);
                    table.ForeignKey(
                        name: "FK_OrganisationUsers_Organisations_organisationId",
                        column: x => x.organisationId,
                        principalSchema: "chatbot",
                        principalTable: "Organisations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganisationUsers_Users_userId",
                        column: x => x.userId,
                        principalSchema: "chatbot",
                        principalTable: "Users",
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 964, DateTimeKind.Utc).AddTicks(9310)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 965, DateTimeKind.Utc).AddTicks(200))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhraseParts", x => x.id);
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 944, DateTimeKind.Utc).AddTicks(3200)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 944, DateTimeKind.Utc).AddTicks(3650))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityValues", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "QueryIntents",
                schema: "chatbot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 913, DateTimeKind.Utc).AddTicks(2030)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 914, DateTimeKind.Utc).AddTicks(8750)),
                    intentId = table.Column<Guid>(type: "uuid", nullable: true),
                    queryId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryIntents", x => x.id);
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 982, DateTimeKind.Utc).AddTicks(6360)),
                    ResponseId = table.Column<Guid>(type: "uuid", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 982, DateTimeKind.Utc).AddTicks(8390))
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
                name: "EntityNames",
                schema: "chatbot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    projectId = table.Column<Guid>(type: "uuid", nullable: false),
                    canBeReferenced = table.Column<bool>(type: "boolean", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 937, DateTimeKind.Utc).AddTicks(640)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 937, DateTimeKind.Utc).AddTicks(1360))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 942, DateTimeKind.Utc).AddTicks(3750)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 942, DateTimeKind.Utc).AddTicks(4520))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 953, DateTimeKind.Utc).AddTicks(2760)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 953, DateTimeKind.Utc).AddTicks(3460)),
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 997, DateTimeKind.Utc).AddTicks(5740)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 997, DateTimeKind.Utc).AddTicks(6360))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectVersions", x => x.id);
                    table.ForeignKey(
                        name: "FK_ProjectVersions_Organisations_organisationId",
                        column: x => x.organisationId,
                        principalSchema: "chatbot",
                        principalTable: "Organisations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 46, 9, DateTimeKind.Utc).AddTicks(7290)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 46, 9, DateTimeKind.Utc).AddTicks(7810))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 46, 16, DateTimeKind.Utc).AddTicks(2830)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 46, 16, DateTimeKind.Utc).AddTicks(3800))
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
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 46, 18, DateTimeKind.Utc).AddTicks(2730)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 46, 18, DateTimeKind.Utc).AddTicks(3210))
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
                name: "GreetingResponses",
                schema: "chatbot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    projectId = table.Column<Guid>(type: "uuid", nullable: false),
                    responseId = table.Column<Guid>(type: "uuid", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 947, DateTimeKind.Utc).AddTicks(9820)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 948, DateTimeKind.Utc).AddTicks(80))
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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 915, DateTimeKind.Utc).AddTicks(8530)),
                    updatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2021, 6, 6, 7, 20, 45, 915, DateTimeKind.Utc).AddTicks(8770)),
                    queryId = table.Column<Guid>(type: "uuid", nullable: true),
                    responseId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryResponses", x => x.id);
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
                name: "IX_OrganisationUsers_organisationId",
                schema: "chatbot",
                table: "OrganisationUsers",
                column: "organisationId");

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
                name: "IX_Projects_ResponseId",
                schema: "chatbot",
                table: "Projects",
                column: "ResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectVersions_organisationId",
                schema: "chatbot",
                table: "ProjectVersions",
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
                name: "IX_QueryIntents_intentId",
                schema: "chatbot",
                table: "QueryIntents",
                column: "intentId");

            migrationBuilder.CreateIndex(
                name: "IX_QueryIntents_queryId",
                schema: "chatbot",
                table: "QueryIntents",
                column: "queryId");

            migrationBuilder.CreateIndex(
                name: "IX_QueryResponses_queryId",
                schema: "chatbot",
                table: "QueryResponses",
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

            migrationBuilder.AddForeignKey(
                name: "FK_PhraseParts_EntityNames_entityNameId",
                schema: "chatbot",
                table: "PhraseParts",
                column: "entityNameId",
                principalSchema: "chatbot",
                principalTable: "EntityNames",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhraseParts_EntityTypes_entityTypeId",
                schema: "chatbot",
                table: "PhraseParts",
                column: "entityTypeId",
                principalSchema: "chatbot",
                principalTable: "EntityTypes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhraseParts_Intents_intentId",
                schema: "chatbot",
                table: "PhraseParts",
                column: "intentId",
                principalSchema: "chatbot",
                principalTable: "Intents",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityValues_EntityTypes_entityTypeId",
                schema: "chatbot",
                table: "EntityValues",
                column: "entityTypeId",
                principalSchema: "chatbot",
                principalTable: "EntityTypes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "QueryIntents_intentId_fkey",
                schema: "chatbot",
                table: "QueryIntents",
                column: "intentId",
                principalSchema: "chatbot",
                principalTable: "Intents",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "QueryIntents_queryId_fkey",
                schema: "chatbot",
                table: "QueryIntents",
                column: "queryId",
                principalSchema: "chatbot",
                principalTable: "Queries",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Responses_ResponseId",
                schema: "chatbot",
                table: "Projects",
                column: "ResponseId",
                principalSchema: "chatbot",
                principalTable: "Responses",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Responses_Projects_projectId",
                schema: "chatbot",
                table: "Responses");

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
                name: "Projects",
                schema: "chatbot");

            migrationBuilder.DropTable(
                name: "Organisations",
                schema: "chatbot");

            migrationBuilder.DropTable(
                name: "Responses",
                schema: "chatbot");
        }
    }
}
