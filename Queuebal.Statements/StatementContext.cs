using Queuebal.Json.Data;

namespace Queuebal.Statements;


public class StatementContext
{
    /// <summary>
    /// Gets or sets the data provider used to access data.
    /// </summary>
    public DataProvider DataProvider { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StatementContext"/> class.
    /// </summary>
    /// <param name="dataProvider">The data provider to use.</param>
    public StatementContext(DataProvider dataProvider)
    {
        DataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
    }
}
