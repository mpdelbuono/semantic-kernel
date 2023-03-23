// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;

namespace Microsoft.SemanticKernel.AI.Embeddings;

/// <summary>
/// Represents an object that has an <see cref="Embedding{TEmbedding}"/>.
/// </summary>
/// <typeparam name="TEmbedding">The embedding data type.</typeparam>
public interface IEmbeddingWithMetadata<TEmbedding>
    where TEmbedding : unmanaged
{
    /// <summary>
    /// Gets the <see cref="Embedding{TEmbedding}"/>.
    /// </summary>
    Embedding<TEmbedding> Embedding { get; }

    /// <summary>
    /// Additional information conveyed during serialization.
    /// </summary>
    Dictionary<string, object> ValueData { get; }
}
