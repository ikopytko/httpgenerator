﻿using System.Diagnostics;
using System.Text;
using System.Text.Json;
using NSwag;
using NSwag.CodeGeneration.CSharp;

namespace HttpGenerator.Core;

public static class HttpFileGenerator
{
    public static async Task<GeneratorResult> Generate(GeneratorSettings settings)
    {
        var document = await OpenApiDocumentFactory.CreateAsync(settings.OpenApiPath);
        var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings());
        generator.BaseSettings.OperationNameGenerator = new OperationNameGenerator(document);

        var baseUrl = document.Servers?.FirstOrDefault()?.Url ?? string.Empty;

        var files = new List<HttpFile>();
        foreach (var kv in document.Paths)
        {
            foreach (var operations in kv.Value)
            {
                var operation = operations.Value;
                var verb = operations.Key.CapitalizeFirstCharacter();
                var name = GenerateOperationName(kv.Key, verb, operation, document, generator);
                var filename = $"{name.CapitalizeFirstCharacter()}.http";

                var code = new StringBuilder();
                code.AppendLine($"### {verb.ToUpperInvariant()} {kv.Key} Request");
                code.AppendLine();
                code.AppendLine($"{verb.ToUpperInvariant()} {baseUrl}{kv.Key}");
                code.AppendLine("Content-Type: " + settings.ContentType);

                if (!string.IsNullOrWhiteSpace(settings.AuthorizationHeader))
                {
                    code.AppendLine($"Authorization: {settings.AuthorizationHeader}");
                }
                
                if (operation.RequestBody?.Content?.ContainsKey(settings.ContentType) == true)
                {
                    var requestBody = operation.RequestBody;
                    var requestBodySchema = requestBody.Content[settings.ContentType].Schema.ActualSchema;
                    var requestBodyJson = requestBodySchema.ToSampleJson().ToString();

                    if (requestBodySchema.Example != null)
                    {
                        requestBodyJson = JsonSerializer.Serialize(requestBodySchema.Example);
                    }

                    code.AppendLine();
                    code.AppendLine(requestBodyJson);
                }

                files.Add(new HttpFile(filename, code.ToString()));
            }
        }

        return new GeneratorResult(files);
    }

    private static string GenerateOperationName(
        string path,
        string verb,
        OpenApiOperation operation,
        OpenApiDocument document,
        CSharpClientGenerator generator,
        bool capitalizeFirstCharacter = false)
    {
        var operationName = generator
            .BaseSettings
            .OperationNameGenerator
            .GetOperationName(document, path, verb, operation);

        if (capitalizeFirstCharacter)
            operationName = operationName.CapitalizeFirstCharacter();

        return operationName;
    }
}