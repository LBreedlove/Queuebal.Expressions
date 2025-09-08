using Queuebal.Json;
using Queuebal.Json.Data;

namespace Queuebal.UnitTests.Json.Data;


[TestClass]
public class TestVariableProvider
{
    [TestMethod]
    public void test_when_add_root_value_value_is_in_root_scope()
    {
        var variableProvider = new VariableProvider();
        variableProvider.AddRootValue("test_key", new JSONValue("Test Value"));
        var value = variableProvider.GetValue("test_key");
        Assert.IsNotNull(value);
        Assert.AreEqual(value.Value, "Test Value");
    }

    [TestMethod]
    public void test_when_pop_scope_and_no_scopes_does_not_pop_root_scope()
    {
        var variableProvider = new VariableProvider();
        variableProvider.AddRootValue("test_key", new JSONValue("Test Value"));
        variableProvider.PopScope();

        // get the value from the root scope, which should still exist even
        // though we called "PopScope" when no other scopes were registered.
        var value = variableProvider.GetValue("test_key");
        Assert.IsNotNull(value);
        Assert.AreEqual(value.Value, "Test Value");
    }

    [TestMethod]
    public void test_when_value_in_multiple_scopes_gets_top_value()
    {
        var variableProvider = new VariableProvider();
        variableProvider.AddRootValue("test_key", new JSONValue("Test Value - Root"));

        var scope1Values = new Dictionary<string, JSONValue>()
        {
            { "test_key", new JSONValue("Test Value - scope1") }
        };

        var scope2Values = new Dictionary<string, JSONValue>()
        {
            { "test_key", new JSONValue("Test Value - scope2") }
        };

        using (var scopeAccess1 = variableProvider.WithScope("scope1", scope1Values))
        {
            using (var scopeAccess2 = variableProvider.WithScope("scope2", scope2Values))
            {
                Assert.AreEqual(variableProvider.GetValue("test_key")?.Value, "Test Value - scope2");
            }

            // since scope 2 was popped, we should get the scope 1 value
            Assert.AreEqual(variableProvider.GetValue("test_key")?.Value, "Test Value - scope1");
        }

        Assert.AreEqual(variableProvider.GetValue("test_key")?.Value, "Test Value - Root");
    }

    [TestMethod]
    public void test_when_searching_by_scope_name_finds_correct_value()
    {
        var variableProvider = new VariableProvider();
        variableProvider.AddRootValue("test_key", new JSONValue("Test Value - Root"));

        var scope1Values = new Dictionary<string, JSONValue>()
        {
            { "test_key", new JSONValue("Test Value - scope1") }
        };

        var scope2Values = new Dictionary<string, JSONValue>()
        {
            { "test_key", new JSONValue("Test Value - scope2") }
        };

        variableProvider.PushScope(new VariableProviderScope("scope1", scope1Values));
        variableProvider.PushScope(new VariableProviderScope("scope2", scope2Values));

        Assert.AreEqual(variableProvider.GetValue("test_key", "scope1")?.Value, "Test Value - scope1");
        Assert.AreEqual(variableProvider.GetValue("test_key", "scope2")?.Value, "Test Value - scope2");
    }

    [TestMethod]
    public void test_when_scope_added_to_stack_without_using_accessor_pops_correct_scope()
    {
        var variableProvider = new VariableProvider();

        var scope1Values = new Dictionary<string, JSONValue>()
        {
            { "test_key", new JSONValue("Test Value - scope1") }
        };

        var scope2Values = new Dictionary<string, JSONValue>()
        {
            { "test_key", new JSONValue("Test Value - scope2") }
        };

        using (var scopeAccess1 = variableProvider.WithScope("scope1", scope1Values))
        {
            variableProvider.PushScope(new VariableProviderScope("scope2", scope2Values));
        }

        // we popped scope1, but scope2 should still exist, even though it was added
        // after scope1.
        Assert.AreEqual(variableProvider.GetValue("test_key")?.Value, "Test Value - scope2");
    }

    [TestMethod]
    public void test_when_add_to_parent_adds_to_correct_scope()
    {
        var variableProvider = new VariableProvider();

        var scope1Values = new Dictionary<string, JSONValue>()
        {
            { "test_key", new JSONValue("Test Value - scope1") }
        };

        using (var scopeAccess1 = variableProvider.WithScope("scope1", scope1Values))
        {
            variableProvider.AddParentValue("test_key", "Test Value - rootScope");
            var currentScopeValue = variableProvider.GetValue("test_key");
            Assert.AreEqual("Test Value - scope1", currentScopeValue);

            var rootScopeValue = variableProvider.GetValue("test_key", "__root");
            Assert.AreEqual("Test Value - rootScope", rootScopeValue);
        }
    }

    [TestMethod]
    public void test_when_add_to_parent_and_there_is_no_parent_adds_to_root_scope()
    {
        var variableProvider = new VariableProvider();
        variableProvider.AddParentValue("test_key", "test_value");

        var value = variableProvider.GetValue("test_key", scopeName: "__root");
        Assert.AreEqual("test_value", value);
    }

    [TestMethod]
    public void test_add_values_from_dict_adds_values_to_current_scope()
    {
        var variableProvider = new VariableProvider();
        var values = new Dictionary<string, JSONValue>
        {
            { "key1", new JSONValue("value1") },
            { "key2", new JSONValue("value2") }
        };

        variableProvider.AddValues(values);

        Assert.AreEqual("value1", variableProvider.GetValue("key1")?.Value);
        Assert.AreEqual("value2", variableProvider.GetValue("key2")?.Value);
    }

    [TestMethod]
    public void test_get_value_from_root_scope_returns_correct_value()
    {
        var variableProvider = new VariableProvider();
        variableProvider.AddRootValue("test_key", new JSONValue("Test Value - Root"));

        // add a duplicate key in a new scope to make sure we pull
        // from the root scope
        var scope1Values = new Dictionary<string, JSONValue>()
        {
            { "test_key", new JSONValue("Test Value - scope1") }
        };

        using (variableProvider.WithScope("scope1", scope1Values))
        {
            // Get value from root scope while in a child scope
            var value = variableProvider.GetRootValue("test_key");
            Assert.IsNotNull(value);
            Assert.AreEqual("Test Value - Root", value.Value);
        }
    }

    [TestMethod]
    public void test_get_value_when_key_not_found_returns_null()
    {
        var variableProvider = new VariableProvider();
        var value = variableProvider.GetValue("non_existent_key");
        Assert.IsNull(value);
    }

    [TestMethod]
    public void test_remove_scope_when_scope_from_different_data_provider_throws()
    {
        var variableProvider1 = new VariableProvider();
        var variableProvider2 = new VariableProvider();

        var scopeValues = new Dictionary<string, JSONValue>
        {
            { "test_key", new JSONValue("Test Value") }
        };

        using (var scopeAccessor = variableProvider1.WithScope("scope1", scopeValues))
        {
            // Attempt to remove a scope from a different data provider
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                variableProvider2.RemoveScope(scopeAccessor.Scope);
            });
        }
    }

    [TestMethod]
    public void test_pop_scope_when_multiple_scopes_exist_removes_top_scope()
    {
        var variableProvider = new VariableProvider();

        var scope1Values = new Dictionary<string, JSONValue>()
        {
            { "test_key", new JSONValue("Test Value - scope1") }
        };

        var scope2Values = new Dictionary<string, JSONValue>()
        {
            { "test_key", new JSONValue("Test Value - scope2") }
        };

        variableProvider.AddRootValue("test_key", new JSONValue("Test Value - Root"));
        variableProvider.WithScope("scope1", scope1Values);
        variableProvider.WithScope("scope2", scope2Values);

        // At this point, we are in scope2
        Assert.AreEqual(variableProvider.GetValue("test_key")?.Value, "Test Value - scope2");
        variableProvider.PopScope();

        // After popping scope2, we should be back in scope1
        Assert.AreEqual(variableProvider.GetValue("test_key")?.Value, "Test Value - scope1");
        variableProvider.PopScope();

        // After popping scope1, we should be back in the root scope
        Assert.AreEqual(variableProvider.GetValue("test_key")?.Value, "Test Value - Root");
        variableProvider.PopScope(); // We should not be able to pop the root scope, so this should not change anything.

        Assert.AreEqual(variableProvider.GetValue("test_key")?.Value, "Test Value - Root");
    }

    [TestMethod]
    public void test_add_value_adds_value_to_current_scope()
    {
        var variableProvider = new VariableProvider();
        variableProvider.AddValue("test_key", new JSONValue("Test Value"));

        var value = variableProvider.GetValue("test_key");
        Assert.IsNotNull(value);
        Assert.AreEqual("Test Value", value.Value);
    }

    [TestMethod]
    public void test_add_root_values_adds_values_to_root_scope()
    {
        var variableProvider = new VariableProvider();
        var scope2Values = new Dictionary<string, JSONValue>
        {
            { "key3", new JSONValue("value3") }
        };

        using (variableProvider.WithScope("scope2", scope2Values))
        {
            var values = new Dictionary<string, JSONValue>
            {
                { "key1", new JSONValue("value1") },
                { "key2", new JSONValue("value2") }
            };

            // Add values to root scope while in a child scope
            variableProvider.AddRootValues(values);

            Assert.AreEqual("value1", variableProvider.GetValue("key1", "__root")?.Value);
            Assert.AreEqual("value2", variableProvider.GetValue("key2", "__root")?.Value);
        }
    }

    [TestMethod]
    public void test_remove_root_scope_throws_invalid_operation_exception()
    {
        var variableProvider = new VariableProvider();

        var rootScopeField = typeof(VariableProvider).GetField("_rootScope", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var rootScope = (VariableProviderScope?)rootScopeField?.GetValue(variableProvider);

        var scopeAccessor = new VariableProviderScopeAccess(variableProvider, rootScope!);
        Assert.ThrowsExactly<InvalidOperationException>(() => scopeAccessor.Dispose());
    }

    [TestMethod]
    public void test_set_value_when_key_exists_updates_value()
    {
        var variableProvider = new VariableProvider();
        variableProvider.AddRootValue("test_key", new JSONValue("Initial Value"));

        // Set a new value for the existing key
        variableProvider.SetValue("test_key", new JSONValue("Updated Value"));

        var value = variableProvider.GetValue("test_key");
        Assert.IsNotNull(value);
        Assert.AreEqual("Updated Value", value.Value);
    }

    [TestMethod]
    public void test_set_value_when_key_does_not_exist_throws_exception()
    {
        var variableProvider = new VariableProvider();

        // Act & Assert
        Assert.ThrowsException<KeyNotFoundException>(() => variableProvider.SetValue("non_existent_key", new JSONValue("Some Value")));

        // Ensure that the key was not added
        var value = variableProvider.GetValue("non_existent_key");
        Assert.IsNull(value);
    }
}
