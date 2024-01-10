using Developist.Core.ArgumentValidation;

namespace Developist.Core.Persistence;

/// <summary>
/// Provides extension methods for configuring sorting options on instances of <see cref="PaginationCriteriaConfigurator{T}"/>.
/// </summary>
public static class PaginationCriteriaConfiguratorExtensions
{
    /// <summary>
    /// Configures sorting criteria for a <see cref="PaginationCriteriaConfigurator{T}"/> using a comma-separated string of sorting directives.
    /// </summary>
    /// <remarks>
    /// The <paramref name="sortString"/> parameter contains one or more sorting directives, separated by commas.
    /// Each sorting directive specifies the name of a property to sort by and, optionally, the sort direction (either "ASC" or "DESC").
    /// If no sort direction is specified, the default direction is ascending ("ASC").
    /// </remarks>
    /// <example>
    /// 1. Sort by FamilyName (ASC), then by Age (DESC):
    /// <code>configurator.SortByString("FamilyName ASC, Age DESC");</code>   
    /// 2. Sort by FamilyName (ASC), then Age (DESC) using alternative syntax:
    /// <code>configurator.SortByString("FamilyName, Age DESC");</code>
    /// </example>
    /// <typeparam name="T">The type of data items to be sorted.</typeparam>
    /// <param name="configurator">The <see cref="PaginationCriteriaConfigurator{T}"/> to configure sorting for.</param>
    /// <param name="sortString">The comma-separated string of sorting directives.</param>
    /// <returns>The <see cref="PaginationCriteriaConfigurator{T}"/> with the specified sorting criteria.</returns>
    /// <exception cref="FormatException"/>
    public static PaginationCriteriaConfigurator<T> SortByString<T>(this PaginationCriteriaConfigurator<T> configurator, string sortString)
    {
        Ensure.Argument.NotNullOrWhiteSpace(sortString);
        
        foreach (var (propertyName, direction) in ParseSortingDirectives(sortString))
        {
            try
            {
                configurator.SortBy(propertyName, direction);
            }
            catch (ArgumentException exception)
            {
                throw new FormatException(
                    "Failed to parse a sorting directive from the input string. See the inner exception for details.",
                    innerException: exception);
            }
        }

        return configurator;
    }

    private static IEnumerable<(string, SortDirection)> ParseSortingDirectives(string sortString)
    {
        var directives = sortString.Split(',').Select(s => s.Trim());
        if (directives.Any(string.IsNullOrEmpty))
        {
            throw new FormatException("Invalid sorting string format: The string contains an empty sorting directive.");
        }

        return directives.Select(ParseSortingDirective);
    }

    private static (string, SortDirection) ParseSortingDirective(string directive)
    {
        var parts = directive.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);

        var propertyName = parts[0];
        var direction = parts.Length switch
        {
            1 => SortDirection.Ascending,
            2 => ParseSortDirection(parts[1]),
            _ => throw new FormatException($"Invalid format for sorting directive: '{directive}'. Expected format is 'PropertyName [ASC|DESC]'.")
        };

        return (propertyName, direction);
    }

    private static SortDirection ParseSortDirection(string direction)
    {
        return direction.ToUpperInvariant() switch
        {
            "ASC" => SortDirection.Ascending,
            "DESC" => SortDirection.Descending,
            _ => throw new FormatException($"Invalid sort direction in sorting directive: '{direction}'. Must be either 'ASC' or 'DESC'.")
        };
    }
}
