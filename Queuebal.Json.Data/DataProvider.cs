using Queuebal.Json;

namespace Queuebal.Json.Data;


/// <summary>Provides disposable access to a data provider scope.</summary>
public class DataProviderScopeAccess : IDisposable
{
    /// <summary>Is the scope access disposed of yet.</summary>
    private int _disposed = 0;

    /// <summary>The DataProvider the scope was added to.</summary>
    private readonly DataProvider _dataProvider;

    /// <summary>The DataProviderScope that was added to the DataProvider.</summary>
    private readonly DataProviderScope _scope;

    /// <summary>
    /// Initializes a new instance of the DataProviderScopeAccess object, and pushes
    /// the scope onto the scope stack of the given DataProvider.
    /// </summary>
    /// <param name="dataProvider">The DataProvider to add the scope to.</param>
    /// <param name="scope">The scope to add to the DataProvider.</param>
    public DataProviderScopeAccess(DataProvider dataProvider, DataProviderScope scope)
    {
        _dataProvider = dataProvider;
        _scope = scope;

        _dataProvider.PushScope(scope);
    }

    ~DataProviderScopeAccess()
    {
        Dispose(false);
    }

    /// <summary>
    /// Gets the scope that was added to the DataProvider.
    /// </summary>
    public DataProviderScope Scope => _scope;

    /// <summary>
    /// Disposes of the DataProviderScopeAccess and removes the DataProviderScope from the DataProvider.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Disposes of the DataProviderScopeAccess and removes the DataProviderScope from the DataProvider.
    /// </summary>
    private void Dispose(bool disposing)
    {
        if (Interlocked.CompareExchange(ref _disposed, 1, 0) != 0)
        {
            return;
        }

        if (disposing)
        {
            _dataProvider.RemoveScope(_scope);
        }
    }
}


/// <summary>
/// Defines a Scope for a DataProvider to store values in.
/// </summary>
public class DataProviderScope
{
    /// <summary>
    /// The name of the scope.
    /// </summary>
    private readonly string _scopeName;

    /// <summary>
    /// A map of values contained in the scope.
    /// </summary>
    private readonly Dictionary<string, JSONValue> _values = new();

    /// <summary>
    /// Initializes a new instance of a DataProviderScope, with the specified name.
    /// </summary>
    /// <param name="scopeName">The name of the new scope.</param>
    public DataProviderScope(string scopeName, Dictionary<string, JSONValue>? values = null)
    {
        _scopeName = scopeName;
        if (values != null)
        {
            foreach (var item in values)
            {
                _values.Add(item.Key, item.Value);
            }
        }
    }

    /// <summary>
    /// Gets the name of the DataProviderScope.
    /// </summary>
    public string Name => _scopeName;

    /// <summary>
    /// Adds a JSONValue to the scope.
    /// </summary>
    /// <param name="key">The key for the new value.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>The current scope.</returns>
    /// <remarks>If a value with the given key already exists, it will be overwritten.</remarks>
    public DataProviderScope AddValue(string key, JSONValue value)
    {
        _values[key] = value;
        return this;
    }

    /// <summary>
    /// Adds a collection of JSONValue's to the scope.
    /// </summary>
    /// <param name="values">The values to add to the scope.</param>
    /// <returns>The current scope.</returns>
    /// <remarks>If a value with the given key already exists, it will be overwritten.</remarks>
    public DataProviderScope AddValues(Dictionary<string, JSONValue> values)
    {
        foreach (var keyValue in values)
        {
            AddValue(keyValue.Key, keyValue.Value);
        }

        return this;
    }

    /// <summary>
    /// Attempts to get the value with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to lookup.</param>
    /// <returns>The value stored in the key, if the value is found, otherwise null.</returns>
    public JSONValue? GetValue(string key) =>
        _values.TryGetValue(key, out var value) ? value : null;
}


/// <summary>
/// Provides access to data stored in memory, using a stack of DataProviderScopes to
/// store and access values.
/// </summary>
public class DataProvider
{
    private readonly DataProviderScope _rootScope = new DataProviderScope("__root");
    private readonly List<DataProviderScope> _scopes = new();
    private readonly Dictionary<string, List<DataProviderScope>> _scopesMap = new();
    private DataProviderScope _currentScope;

    /// <summary>Initializes a new instance of the DataProvider class.</summary>
    public DataProvider()
    {
        _currentScope = _rootScope;
        _scopes.Add(_rootScope);
    }

    /// <summary>
    /// Adds a value to the root scope.
    /// </summary>
    /// <param name="key">The key of the value to add.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>The current DataProvider.</returns>
    /// <remarks>If a value with the given key already exists, it will be overwritten.</remarks>
    public DataProvider AddRootValue(string key, JSONValue value)
    {
        _rootScope.AddValue(key, value);
        return this;
    }

    /// <summary>
    /// Adds a dictionary of values to the root scope.
    /// </summary>
    /// <param name="values">The values to add to the root scope.</param>
    /// <returns>The current DataProvider.</returns>
    /// <remarks>If a value with the given key already exists, it will be overwritten.</remarks>
    public DataProvider AddRootValues(Dictionary<string, JSONValue> values)
    {
        _rootScope.AddValues(values);
        return this;
    }

    /// <summary>Adds a value to the current scope.</summary>
    /// <remarks>If a value with the given key already exists, it will be overwritten.</remarks>
    public DataProvider AddValue(string key, JSONValue value)
    {
        _currentScope.AddValue(key, value);
        return this;
    }

    /// <summary>
    /// Adds a dictionary of values to the current scope.
    /// </summary>
    /// <param name="values">The values to add to the current scope.</param>
    /// <returns>The current DataProvider.</returns>
    /// <remarks>If a value with the given key already exists, it will be overwritten.</remarks>
    public DataProvider AddValues(Dictionary<string, JSONValue> values)
    {
        _currentScope.AddValues(values);
        return this;
    }

    /// <summary>
    /// Adds a value to the scope that is the parent of the current scope.
    /// </summary>
    /// <param name="key">The name of the variable to add.</param>
    /// <param name="value">The value to assign to the variable.</param>
    /// <returns>The current DataProvider.</returns>
    /// <remarks>If the currentScope is the rootScope, the key/value will be added to the currentScope.</remarks>
    /// <remarks>If a value with the given key already exists, it will be overwritten.</remarks>
    public DataProvider AddParentValue(string key, JSONValue value)
    {
        if (_scopes.Count == 1)
        {
            // if we're at the rootScope, add the value here.
            _rootScope.AddValue(key, value);
            return this;
        }

        // get the second from the last element - the last element is currentScope, the second to the last is our parentScope.
        var scope = _scopes[^2];
        scope.AddValue(key, value);
        return this;
    }

    /// <summary>
    /// Adds a scope to the top of the scope stack.
    /// </summary>
    /// <param name="scope">The scope to add.</param>
    /// <returns>The current DataProvider.</returns>
    public DataProvider PushScope(DataProviderScope scope)
    {
        _scopes.Add(scope);
        _currentScope = scope;
        return this;
    }

    /// <summary>
    /// Removes the last scope added to the scopes stack.
    /// </summary>
    /// <returns>The current DataProvider.</returns>
    public DataProvider PopScope()
    {
        // we don't want to pop the root scope, so make sure there's
        // more than one scope registered.
        if (_scopes.Count > 1)
        {
            _scopes.RemoveAt(_scopes.Count - 1);
        }

        _currentScope = _scopes[^1];
        return this;
    }

    /// <summary>
    /// Removes the specified DataProviderScope from the stack of scopes.
    /// </summary>
    /// <param name="scope">The scope to remove.</param>
    /// <returns>The current DataProvider.</returns>
    public DataProvider RemoveScope(DataProviderScope scope)
    {
        // we expect the scope to be at the top of the stack, so check there first.
        if (scope == _rootScope)
        {
            throw new InvalidOperationException("Cannot remove the root scope from the DataProvider.");
        }

        if (_scopes[^1] == scope)
        {
            _scopes.RemoveAt(_scopes.Count - 1);
            return this;
        }

        if (!_scopes.Remove(scope))
        {
            throw new InvalidOperationException("The specified scope does not belong to the current DataProvider");
        }

        return this;
    }

    /// <summary>
    /// Adds a new scope to the scopes stack and returns an IDisposable
    /// to pop the scope off the stack when the caller is done using it.
    /// </summary>
    /// <param name="scopeName">The scope to add to the </param>
    /// <param name="values"></param>
    /// <returns></returns>
    public DataProviderScopeAccess WithScope(string scopeName, Dictionary<string, JSONValue>? values = null)
    {
        var scope = new DataProviderScope(scopeName, values);
        return new DataProviderScopeAccess(this, scope);
    }

    /// <summary>
    /// Attempts to get the value from the rootScope, with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <returns>A JSONValue containing the value, if the key is found, otherwise null.</returns>
    public JSONValue? GetRootValue(string key)
    {
        return _rootScope.GetValue(key);
    }

    /// <summary>
    /// Attempts to find the value with the specified key. The search
    /// is performed from the top of the scope stack to the root. If
    /// scopeName is provided, only scopes with the matching name will
    /// be searched.
    /// </summary>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="scopeName">The name of the scopes to include in the search.</param>
    /// <returns>A JSONValue if the value is found, otherwise null.</returns>
    public JSONValue? GetValue(string key, string? scopeName = null)
    {
        for (int idx = _scopes.Count - 1; idx >= 0; idx--)
        {
            var scope = _scopes[idx];
            if (scopeName == null || scopeName == scope.Name)
            {
                var value = scope.GetValue(key);
                if (value != null)
                {
                    return value;
                }
            }
        }

        return null;
    }
}
