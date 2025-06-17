using System.Reflection;

namespace Queuebal.Services;


public interface ITypeRegistryService
{
    /// <summary>
    /// Gets the field name used to discriminate between different types in the type map.
    /// </summary>
    string DiscriminatorField { get; }

    /// <summary>
    /// Gets the registered type mappings.
    /// </summary>
    Dictionary<string, Type> TypeMap { get; }

    /// <summary>
    /// Registers a type name with its corresponding type.
    /// </summary>
    ITypeRegistryService WithTypeMapping(Type implementationType);

    /// <summary>
    /// Registers a type name with its corresponding type.
    /// </summary>
    ITypeRegistryService WithTypeMapping<TImplementationType>();
}


public class TypeRegistryService<TBaseType> : ITypeRegistryService where TBaseType : class
{
    /// <summary>
    /// The object used to synchronize access to the type map.
    /// </summary>
    private readonly object _syncLock = new();

    /// <summary>
    /// A map of type names, as used in the discriminator field, to their type.
    /// </summary>
    private readonly Dictionary<string, Type> _typeMap = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// The field name used to discriminate between different types in the type map.
    /// </summary>
    private readonly string _discriminatorField;

    /// <summary>
    /// Gets the field name used to discriminate between different types in the type map.
    /// </summary>
    public string DiscriminatorField => _discriminatorField;

    /// <summary>
    /// Gets the registered type mappings.
    /// </summary>
    public Dictionary<string, Type> TypeMap
    {
        get
        {
            lock (_syncLock)
            {
                return _typeMap.ToDictionary
                (
                    kvp => kvp.Key,
                    kvp => kvp.Value,
                    StringComparer.OrdinalIgnoreCase
                );
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeRegistryService{TBaseType}"/> class.
    /// </summary>
    /// <param name="discriminatorField">The name of the property used as the discriminator value.</param>
    /// <exception cref="ArgumentException">Thrown if the discriminatorField is null or whitespace.</exception>
    public TypeRegistryService(string discriminatorField = "Type")
    {
        if (string.IsNullOrWhiteSpace(discriminatorField))
        {
            throw new ArgumentException("Discriminator field cannot be null or empty.", nameof(discriminatorField));
        }

        var baseType = typeof(TBaseType);
        var discriminatorProperty = baseType.GetProperty(discriminatorField, BindingFlags.Public | BindingFlags.Static);
        if (discriminatorProperty == null || discriminatorProperty.PropertyType != typeof(string))
        {
            throw new InvalidOperationException($"The base type '{baseType.FullName}' does not have a valid public static '{discriminatorField}' property.");
        }

        _discriminatorField = discriminatorField;
    }

    /// <summary>
    /// Registers a type name with its corresponding type.
    /// </summary>
    /// <param name="implementationType">The type used as the implementation.</param>
    /// <returns
    /// The <see cref="TypeRegistryService"/> instance for method chaining.</returns>
    /// </summary>
    public ITypeRegistryService WithTypeMapping<TImplementationType>()
    {
        RegisterTypeMapping(typeof(TImplementationType));
        return this;
    }

    /// <summary>
    /// Registers a type name with its corresponding type.
    /// </summary>
    /// <param name="implementationType">The type used as the implementation.</param>
    /// <returns
    /// The <see cref="TypeRegistryService"/> instance for method chaining.</returns>
    /// </summary>
    public ITypeRegistryService WithTypeMapping(Type implementationType)
    {
        RegisterTypeMapping(implementationType);
        return this;
    }

    /// <summary>
    /// Registers a type with its discriminator value.
    /// </summary>
    /// <param name="implementationType">The type of the implementing class.</param>
    public void RegisterTypeMapping(Type implementationType)
    {
        lock (_syncLock)
        {
            if (!typeof(TBaseType).IsAssignableFrom(implementationType))
            {
                throw new ArgumentException($"Type '{implementationType.FullName}' does not implement {typeof(TBaseType).FullName}.", nameof(implementationType));
            }

            var discriminatorTypeProperty = implementationType.GetProperty(_discriminatorField, BindingFlags.Public | BindingFlags.Static);
            if (discriminatorTypeProperty != null && discriminatorTypeProperty.PropertyType == typeof(string))
            {
                // Register the type with its discriminator value
                var discriminatorValue = (string)discriminatorTypeProperty.GetValue(null)!;
                if (string.IsNullOrWhiteSpace(discriminatorValue))
                {
                    throw new ArgumentException($"The discriminator value for type '{implementationType.FullName}' cannot be null or empty.", nameof(discriminatorValue));
                }

                if (_typeMap.ContainsKey(discriminatorValue))
                {
                    throw new InvalidOperationException($"A type with the name '{discriminatorValue}' is already registered.");
                }

                _typeMap[discriminatorValue] = implementationType;
                return;
            }

            throw new InvalidOperationException($"Type '{implementationType.FullName}' does not have a valid public static '{_discriminatorField}' property.");
        }
    }

    /// <summary>
    /// Builds an TypeRegistryService using all the loaded assemblies in the current AppDomain.
    /// </summary>
    /// <returns>
    /// A new TypeRegistryService with the TBaseType types in the assemblies in the current
    /// AppDomain registered.
    /// </returns>
    public static TypeRegistryService<TBaseType> BuildFromCurrentAppDomain(string discriminatorField)
    {
        // Get all types in the loaded assemblies in the current AppDomain
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        return BuildFromAssemblies(assemblies, discriminatorField);
    }

    /// <summary>
    /// Builds an TypeRegistryService using the provided assemblies.
    /// </summary>
    /// <returns>
    /// A new TypeRegistryService with the TBaseType types in the provided assemblies
    /// registered.
    /// </returns>
    public static TypeRegistryService<TBaseType> BuildFromAssemblies(IEnumerable<Assembly> assemblies, string discriminatorField)
    {
        var service = new TypeRegistryService<TBaseType>(discriminatorField);

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAssignableTo(typeof(TBaseType)) && !type.IsAbstract)
                {
                    service.RegisterTypeMapping(type);
                }
            }
        }

        return service;
    }
}
