namespace LocalAI.Embeddings;

/// <summary>
/// Maths for comparing embedding vectors. Written out by hand so you can see exactly what happens.
/// (In production you would use TensorPrimitives.CosineSimilarity from System.Numerics.Tensors, or a vector database.)
/// </summary>
public static class VectorMath
{
    /// <summary>
    /// Measures how closely two vectors point in the same direction.
    /// 1.0 = same direction (same meaning), 0 = unrelated. Text embeddings rarely go below 0.
    /// </summary>
    public static double CosineSimilarity(float[] a, float[] b)
    {
        if (a.Length != b.Length)
        {
            throw new ArgumentException($"Vectors must have the same length. Got {a.Length} and {b.Length}.");
        }

        double dotProduct = 0;
        double lengthASquared = 0;
        double lengthBSquared = 0;

        for (int i = 0; i < a.Length; i++)
        {
            dotProduct += a[i] * b[i];
            lengthASquared += a[i] * a[i];
            lengthBSquared += b[i] * b[i];
        }

        if (lengthASquared == 0 || lengthBSquared == 0)
        {
            return 0;
        }

        return dotProduct / (Math.Sqrt(lengthASquared) * Math.Sqrt(lengthBSquared));
    }
}
