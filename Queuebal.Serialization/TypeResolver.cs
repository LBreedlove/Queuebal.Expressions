using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using Queuebal.Services;

namespace Queuebal.Serialization;


/// <summary>
/// A JsonTypeInfoResolver that can support multiple base type registries.
/// </summary>
public class CompositeTypeResolver : DefaultJsonTypeInfoResolver
{
    private readonly object _syncLock = new();
    private readonly Dictionary<Type, ITypeRegistryService> _typeRegistries = new();

    public CompositeTypeResolver AddTypeRegistry(ITypeRegistryService typeRegistry)
    {
        lock (_syncLock)
        {
            _typeRegistries[typeRegistry.BaseType] = typeRegistry;
        }
        return this;
    }

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        if (!_typeRegistries.TryGetValue(type, out var typeMapService))
        {
            return jsonTypeInfo;
        }

        jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
        {
            TypeDiscriminatorPropertyName = typeMapService.DiscriminatorField,
            IgnoreUnrecognizedTypeDiscriminators = true,
            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor,
        };

        foreach (var typeMapping in typeMapService.TypeMap)
        {
            jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add
            (
                new JsonDerivedType(derivedType: typeMapping.Value, typeDiscriminator: typeMapping.Key)
            );
        }

        return jsonTypeInfo;
    }
}

public class TypeResolver<TBaseType> : DefaultJsonTypeInfoResolver
    where TBaseType : class
{
    private readonly ITypeRegistryService _typeMapService;

    public TypeResolver(ITypeRegistryService typeMapService)
    {
        _typeMapService = typeMapService;
    }

    /// <summary>
    /// Gets the type information for a given type, configuring polymorphism for Condition types.
    /// </summary>
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        Type baseType = typeof(TBaseType);
        if (baseType != type)
        {
            return jsonTypeInfo;
        }

        jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
        {
            TypeDiscriminatorPropertyName = _typeMapService.DiscriminatorField,
            IgnoreUnrecognizedTypeDiscriminators = true,
            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor,
        };

        foreach (var typeMapping in _typeMapService.TypeMap)
        {
            jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add
            (
                new JsonDerivedType(derivedType: typeMapping.Value, typeDiscriminator: typeMapping.Key)
            );
        }

        return jsonTypeInfo;
    }
}
