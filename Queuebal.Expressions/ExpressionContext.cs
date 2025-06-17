using Queuebal.Json.Data;

namespace Queuebal.Expressions;


/// <summary>
/// Defines the context in which an expression is evaluated.
/// </summary>
public class ExpressionContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionContext"/> class.
    /// </summary>
    /// <param name="dataProvider">The dataprovider used to resolve variable values when evaluating an expression.</param>
    public ExpressionContext(DataProvider dataProvider)
    {
        DataProvider = dataProvider;
    }

    /// <summary>
    /// The DataProvider used when evaluating expressions.
    /// </summary>
    public DataProvider DataProvider { get; }
}
