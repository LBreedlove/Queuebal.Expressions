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
    /// <param name="variableProvider">The dataprovider used to resolve variable values when evaluating an expression.</param>
    public ExpressionContext(VariableProvider variableProvider)
    {
        VariableProvider = variableProvider;
    }

    /// <summary>
    /// The VariableProvider used when evaluating expressions.
    /// </summary>
    public VariableProvider VariableProvider { get; }
}
