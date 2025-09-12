using Queuebal.Json;

namespace Queuebal.Json.Data;


/// <summary>Provides disposable access to a data provider scope.</summary>
public class VariableProviderScopeAccess : IDisposable
{
    /// <summary>Is the scope access disposed of yet.</summary>
    private int _disposed = 0;

    /// <summary>The VariableProvider the scope was added to.</summary>
    private readonly VariableProvider _variableProvider;

    /// <summary>The VariableProviderScope that was added to the VariableProvider.</summary>
    private readonly VariableProviderScope _scope;

    /// <summary>
    /// Initializes a new instance of the VariableProviderScopeAccess object, and pushes
    /// the scope onto the scope stack of the given VariableProvider.
    /// </summary>
    /// <param name="variableProvider">The VariableProvider to add the scope to.</param>
    /// <param name="scope">The scope to add to the VariableProvider.</param>
    public VariableProviderScopeAccess(VariableProvider variableProvider, VariableProviderScope scope)
    {
        _variableProvider = variableProvider;
        _scope = scope;

        _variableProvider.PushScope(scope);
    }

    ~VariableProviderScopeAccess()
    {
        Dispose(false);
    }

    /// <summary>
    /// Gets the scope that was added to the VariableProvider.
    /// </summary>
    public VariableProviderScope Scope => _scope;

    /// <summary>
    /// Disposes of the VariableProviderScopeAccess and removes the VariableProviderScope from the VariableProvider.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Disposes of the VariableProviderScopeAccess and removes the VariableProviderScope from the VariableProvider.
    /// </summary>
    private void Dispose(bool disposing)
    {
        if (Interlocked.CompareExchange(ref _disposed, 1, 0) != 0)
        {
            return;
        }

        if (disposing)
        {
            _variableProvider.RemoveScope(_scope);
        }
    }
}


/// <summary>
/// Defines a Scope for a VariableProvider to store values in.
/// </summary>
public class VariableProviderScope
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
    /// Initializes a new instance of a VariableProviderScope, with the specified name.
    /// </summary>
    /// <param name="scopeName">The name of the new scope.</param>
    public VariableProviderScope(string scopeName, Dictionary<string, JSONValue>? values = null)
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
    /// Gets the name of the VariableProviderScope.
    /// </summary>
    public string Name => _scopeName;

    /// <summary>
    /// Sets the value for the specified key in the scope.
    /// </summary>
    /// <param name="key">The name of the variable to set.</param>
    /// <param name="newValue">The new value of the key.</param>
    /// <returns>true if the key was found, otherwise false.</returns>
    public bool SetValue(string key, JSONValue newValue)
    {
        if (_values.ContainsKey(key))
        {
            _values[key] = newValue;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds a JSONValue to the scope.
    /// </summary>
    /// <param name="key">The key for the new value.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>The current scope.</returns>
    /// <remarks>If a value with the given key already exists, it will be overwritten.</remarks>
    public VariableProviderScope AddValue(string key, JSONValue value)
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
    public VariableProviderScope AddValues(Dictionary<string, JSONValue> values)
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
/// Provides access to data stored in memory, using a stack of VariableProviderScopes to
/// store and access values.
/// </summary>
public class VariableProvider
{
    private readonly VariableProviderScope _rootScope = new VariableProviderScope("__root");
    private readonly List<VariableProviderScope> _scopes = new();
    private readonly Dictionary<string, List<VariableProviderScope>> _scopesMap = new();
    private VariableProviderScope _currentScope;

    /// <summary>Initializes a new instance of the VariableProvider class.</summary>
    public VariableProvider()
    {
        _currentScope = _rootScope;
        _scopes.Add(_rootScope);
    }

    /// <summary>
    /// Sets the named variable in the first scope that contains the variable.
    /// If the variable is not found in any scope, a KeyNotFoundException is thrown.
    /// </summary>
    /// <param name="key">The name of the variable to set.</param>
    /// <param name="value">The value of the variable.</param>
    /// <returns>The new value of the variable.</returns>
    public JSONValue SetValue(string key, JSONValue newValue)
    {
        for (int idx = _scopes.Count - 1; idx >= 0; idx--)
        {
            var scope = _scopes[idx];
            if (scope.SetValue(key, newValue))
            {
                // If the value was set, return the new value.
                return newValue;
            }
        }

        throw new KeyNotFoundException($"The variable '{key}' was not found in any scope.");
    }

    /// <summary>
    /// Adds a value to the root scope.
    /// </summary>
    /// <param name="key">The key of the value to add.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>The current VariableProvider.</returns>
    /// <remarks>If a value with the given key already exists, it will be overwritten.</remarks>
    public VariableProvider AddRootValue(string key, JSONValue value)
    {
        _rootScope.AddValue(key, value);
        return this;
    }

    /// <summary>
    /// Adds a dictionary of values to the root scope.
    /// </summary>
    /// <param name="values">The values to add to the root scope.</param>
    /// <returns>The current VariableProvider.</returns>
    /// <remarks>If a value with the given key already exists, it will be overwritten.</remarks>
    public VariableProvider AddRootValues(Dictionary<string, JSONValue> values)
    {
        _rootScope.AddValues(values);
        return this;
    }

    /// <summary>Adds a value to the current scope.</summary>
    /// <remarks>If a value with the given key already exists, it will be overwritten.</remarks>
    public VariableProvider AddValue(string key, JSONValue value)
    {
        _currentScope.AddValue(key, value);
        return this;
    }

    /// <summary>
    /// Adds a dictionary of values to the current scope.
    /// </summary>
    /// <param name="values">The values to add to the current scope.</param>
    /// <returns>The current VariableProvider.</returns>
    /// <remarks>If a value with the given key already exists, it will be overwritten.</remarks>
    public VariableProvider AddValues(Dictionary<string, JSONValue> values)
    {
        _currentScope.AddValues(values);
        return this;
    }

    /// <summary>
    /// Adds a value to the scope that is the parent of the current scope.
    /// </summary>
    /// <param name="key">The name of the variable to add.</param>
    /// <param name="value">The value to assign to the variable.</param>
    /// <returns>The current VariableProvider.</returns>
    /// <remarks>If the currentScope is the rootScope, the key/value will be added to the currentScope.</remarks>
    /// <remarks>If a value with the given key already exists, it will be overwritten.</remarks>
    public VariableProvider AddParentValue(string key, JSONValue value)
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
    /// <returns>The current VariableProvider.</returns>
    public VariableProvider PushScope(VariableProviderScope scope)
    {
        _scopes.Add(scope);
        _currentScope = scope;
        return this;
    }

    /// <summary>
    /// Removes the last scope added to the scopes stack.
    /// </summary>
    /// <returns>The current VariableProvider.</returns>
    public VariableProvider PopScope()
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
    /// Removes the specified VariableProviderScope from the stack of scopes.
    /// </summary>
    /// <param name="scope">The scope to remove.</param>
    /// <returns>The current VariableProvider.</returns>
    public VariableProvider RemoveScope(VariableProviderScope scope)
    {
        // we expect the scope to be at the top of the stack, so check there first.
        if (scope == _rootScope)
        {
            throw new InvalidOperationException("Cannot remove the root scope from the VariableProvider.");
        }

        if (_scopes[^1] == scope)
        {
            _scopes.RemoveAt(_scopes.Count - 1);
            return this;
        }

        if (!_scopes.Remove(scope))
        {
            throw new InvalidOperationException("The specified scope does not belong to the current VariableProvider");
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
    public VariableProviderScopeAccess WithScope(string scopeName, Dictionary<string, JSONValue>? values = null)
    {
        var scope = new VariableProviderScope(scopeName, values);
        return new VariableProviderScopeAccess(this, scope);
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
    /// Attempts to get the value from the current scope, with the specified key.
    /// The current scope is the last scope added to the stack.
    /// </summary>
    /// <param name="key">The name of the value to search for.</param>
    /// <returns>A JSONValue containing the value, if the key is found, otherwise null.</returns>
    public JSONValue? GetValueInCurrentScope(string key)
    {
        return _currentScope.GetValue(key);
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
