namespace Developist.Core.Persistence.EntityFrameworkCore.Tests.Fixture;

public static class QueryableExtensions
{
    public static bool IsIncludable<T>(this IQueryable<T> query)
    {
        var detector = new QueryableMethodDetector("Include", "ThenInclude");
        detector.Visit(query.Expression);

        return detector.IsMethodDetected;
    }

    public static bool IsSplitQuery<T>(this IQueryable<T>query)
    {
        var detector = new QueryableMethodDetector("AsSplitQuery");
        detector.Visit(query.Expression);

        return detector.IsMethodDetected;
    }

    public static bool IsNoTracking<T>(this IQueryable<T> query)
    {
        var detector = new QueryableMethodDetector("AsNoTracking");
        detector.Visit(query.Expression);

        return detector.IsMethodDetected;
    }

    private class QueryableMethodDetector : ExpressionVisitor
    {
        private readonly ICollection<string> _methodNames;

        public QueryableMethodDetector(params string[] methodNames)
        {
            _methodNames = new HashSet<string>(methodNames);
        }

        public bool IsMethodDetected { get; private set; }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (_methodNames.Contains(node.Method.Name))
            {
                IsMethodDetected = true;
            }

            return base.VisitMethodCall(node);
        }
    }
}
