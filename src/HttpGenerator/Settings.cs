using System.ComponentModel;
using Spectre.Console.Cli;

namespace HttpGenerator;

public class Settings : CommandSettings
{
    [Description("URL or file path to OpenAPI Specification file")]
    [CommandArgument(0, "[URL or input file]")]
    public string OpenApiPath { get; set; } = null!;

    [Description("Output directory")]
    [CommandOption("-o|--output <OUTPUT>")]
    [DefaultValue("./")]
    public string OutputFolder { get; set; } = "./";

    [Description("Don't log errors or collect telemetry")]
    [CommandOption("--no-logging")]
    [DefaultValue(false)]
    public bool NoLogging { get; set; }

    [Description("Skip validation of OpenAPI Specification file")]
    [CommandOption("--skip-validation")]
    [DefaultValue(false)]
    public bool SkipValidation { get; set; }
    
    [Description("Authorization header to use for all requests")]
    [CommandOption("--authorization-header <HEADER>")]
    public string? AuthorizationHeader { get; set; }
}