namespace Server.Domain.ValueObjects.Security;

public class PasswordOptions
{
    public const string SectionName = "Security:Password";

    /// <summary>
    ///     Degree of parallelism (threads to use)
    ///     Recommended: Number of CPU cores
    /// </summary>
    public int DegreeOfParallelism { get; set; } = Environment.ProcessorCount;

    /// <summary>
    ///     Memory size in KB
    ///     Recommended: 65536 KB (64 MB) for high security
    ///     Minimum: 4096 KB (4 MB) for basic security
    /// </summary>
    public int MemorySize { get; set; } = 65536; // 64 MB

    /// <summary>
    ///     Number of iterations
    ///     Recommended: 3-4 for high security
    ///     Minimum: 2 for basic security
    /// </summary>
    public int Iterations { get; set; } = 3;

    /// <summary>
    ///     Salt size in bytes
    ///     Recommended: 16-32 bytes
    /// </summary>
    public int SaltSize { get; set; } = 32;

    /// <summary>
    ///     Hash length in bytes
    ///     Recommended: 32 bytes (256 bits)
    /// </summary>
    public int HashLength { get; set; } = 32;
}
