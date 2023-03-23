// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel.AI.Embeddings;

namespace Microsoft.SemanticKernel.Memory;

/// <summary>
/// IMPORTANT: this is a storage schema. Changing the fields will invalidate existing metadata stored in persistent vector DBs.
/// </summary>
public class MemoryRecord : IEmbeddingWithMetadata<float>
{
    /// <summary>
    /// Whether the source data used to calculate embeddings are stored in the local
    /// storage provider or is available through and external service, such as web site, MS Graph, etc.
    /// </summary>
    public bool IsReference { get; private set; }

    /// <summary>
    /// A value used to understand which external service owns the data, to avoid storing the information
    /// inside the URI. E.g. this could be "MSTeams", "WebSite", "GitHub", etc.
    /// </summary>
    public string ExternalSourceName { get; private set; } = string.Empty;

    /// <summary>
    /// Unique identifier. The format of the value is domain specific, so it can be a URL, a GUID, etc.
    /// </summary>
    public string Id { get; private set; } = string.Empty;

    /// <summary>
    /// Optional title describing the content. Note: the title is not indexed.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Source text, available only when the memory is not an external source.
    /// </summary>
    public string Text { get; private set; } = string.Empty;

    public Dictionary<string, object> ValueData
    {
        get => new()
            {
                { nameof(this.IsReference), this.IsReference ? "T" : "F" },
                { nameof(this.ExternalSourceName), this.ExternalSourceName },
                { nameof(this.Id), this.Id },
                { nameof(this.Description), this.Description },
                { nameof(this.Text), this.Text }
            };

        set
        {
            if (value.TryGetValue(nameof(this.IsReference), out object retrievedIsReference))
            {
                this.IsReference = retrievedIsReference.ToString() switch
                {
                    "T" => true,
                    "F" => false,
                    _ => throw new InvalidDataException("Invalid IsReference value")
                };
            }

            if (value.TryGetValue(nameof(this.ExternalSourceName), out object retrievedName))
            {
                this.ExternalSourceName = retrievedName.ToString();
            }

            if (value.TryGetValue(nameof(this.Id), out object retrievedId))
            {
                this.Id = retrievedId.ToString();
            }

            if (value.TryGetValue(nameof(this.Description), out object retrievedDescription))
            {
                this.Description = retrievedDescription.ToString();
            }

            if (value.TryGetValue(nameof(this.Text), out object retrievedText))
            {
                this.Text = retrievedText.ToString();
            }
        }
    }

    /// <summary>
    /// Source content embeddings.
    /// </summary>
    public Embedding<float> Embedding { get; set; }

    /// <summary>
    /// Prepare an instance about a memory which source is stored externally.
    /// The universal resource identifies points to the URL (or equivalent) to find the original source.
    /// </summary>
    /// <param name="externalId">URL (or equivalent) to find the original source</param>
    /// <param name="sourceName">Name of the external service, e.g. "MSTeams", "GitHub", "WebSite", "Outlook IMAP", etc.</param>
    /// <param name="description">Optional description of the record. Note: the description is not indexed.</param>
    /// <param name="embedding">Source content embeddings</param>
    /// <returns>Memory record</returns>
    public static MemoryRecord ReferenceRecord(
        string externalId,
        string sourceName,
        string? description,
        Embedding<float> embedding)
    {
        return new MemoryRecord
        {
            IsReference = true,
            ExternalSourceName = sourceName,
            Id = externalId,
            Description = description ?? string.Empty,
            Embedding = embedding
        };
    }

    /// <summary>
    /// Prepare an instance for a memory stored in the internal storage provider.
    /// </summary>
    /// <param name="id">Resource identifier within the storage provider, e.g. record ID/GUID/incremental counter etc.</param>
    /// <param name="text">Full text used to generate the embeddings</param>
    /// <param name="description">Optional description of the record. Note: the description is not indexed.</param>
    /// <param name="embedding">Source content embeddings</param>
    /// <returns>Memory record</returns>
    public static MemoryRecord LocalRecord(
        string id,
        string text,
        string? description,
        Embedding<float> embedding)
    {
        return new MemoryRecord
        {
            IsReference = false,
            Id = id,
            Text = text,
            Description = description ?? string.Empty,
            Embedding = embedding
        };
    }

    /// <summary>
    /// Block constructor, use <see cref="ReferenceRecord"/> or <see cref="LocalRecord"/>
    /// </summary>
    [JsonConstructor]
    public MemoryRecord()
    {
    }
}
