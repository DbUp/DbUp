﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using Assent;
using Xunit;

namespace DbUp.Tests.Common;

public abstract class NoPublicApiChangesBase
{
    private readonly Assembly assembly;
    private readonly bool differByFramework;
    private readonly string? callerFilePath;

    public NoPublicApiChangesBase(Assembly assembly, bool differByFramework = false, [CallerFilePath] string? callerFilePath = null)
    {
        this.assembly = assembly;
        this.differByFramework = differByFramework;
        this.callerFilePath = callerFilePath;
    }

    [Fact]
    public void Run()
    {
        var result = GetPublicApi(assembly);

#if NETFRAMEWORK
            const string framework = "netfx";
#else
        const string framework = "netcore"; // "Core" is no longer a thing but maintain the identifier for compatibility.
#endif
        var approvalPostfix = differByFramework ? $".{framework}" : "";

        var config = new Configuration()
            .UsingExtension("cs")
            .UsingNamer(m => Path.Combine(Path.GetDirectoryName(m.FilePath), "ApprovalFiles",
                assembly.GetName().Name + approvalPostfix));

        // Automatically approve the change, make sure to check the result before committing
        // config = config.UsingReporter((received, approved) => File.Copy(received, approved, true));

        this.Assent(result, config, "NoPublicApiChanges", callerFilePath);
    }

    static string GetPublicApi(Assembly assembly)
    {
        var sb = new StringBuilder();

        var customAttributes = assembly.GetCustomAttributesData();

        AppendAttributes(sb, 0, customAttributes, true);

        sb.AppendLine();

        var namespaces = from t in assembly.GetTypes()
            where t.IsPublic
            group t by t.Namespace;

        foreach (var ns in namespaces.OrderBy(n => n.Key, StringComparer.InvariantCulture))
        {
            if (ns.Key == null)
            {
                AppendTypes(sb, 0, ns);
            }
            else
            {
                sb.AppendLine($"namespace {ns.Key}");
                sb.AppendLine("{");
                AppendTypes(sb, 4, ns);
                sb.AppendLine("}");
            }
        }

        return sb.ToString();
    }

    static void AppendAttributes(StringBuilder sb, int indent, IList<CustomAttributeData> customAttributes,
        bool isAssembly)
    {
        var c = customAttributes.Where(a => a.AttributeType.Namespace != "System.Reflection")
            .Where(a => a.AttributeType.Namespace != "System.Diagnostics")
            .Where(a => a.AttributeType.Namespace != "System.Runtime.CompilerServices")
            .Where(a => a.AttributeType != typeof(TargetFrameworkAttribute))
            .OrderBy(a => a.AttributeType.FullName, StringComparer.InvariantCulture);

        foreach (var attribute in c)
        {
            sb
                .Append(' ', indent)
                .Append("[");

            if (isAssembly)
                sb.Append("assembly: ");

            sb.Append(GetTypeName(attribute.AttributeType));

            AppendAttributeArguments(sb, attribute);

            sb.AppendLine("]");
        }
    }

    static void AppendAttributeArguments(StringBuilder sb, CustomAttributeData attribute)
    {
        var isFirst = true;
        sb.Append("(");

        void Append(object argumentValue, string? argumentName = null)
        {
            if (!isFirst)
                sb.Append(", ");
            isFirst = false;

            if (argumentName != null)
                sb.Append(argumentName).Append(" = ");

            sb.Append(FormatValue(argumentValue));
        }

        foreach (var argument in attribute.ConstructorArguments)
            Append(argument.Value);

        foreach (var argument in attribute.NamedArguments)
            Append(argument.TypedValue, argument.MemberName);

        sb.Append(")");
    }

    static void AppendTypes(StringBuilder sb, int indent, IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            AppendAttributes(sb, indent, type.GetCustomAttributesData(), false);

            if (type.IsEnum)
                AppendEnum(sb, indent, type);
            else
                AppendClassOrInterface(sb, indent, type);
        }
    }

    static void AppendEnum(StringBuilder sb, int indent, Type type)
    {
        sb.Append(' ', indent)
            .Append(type.IsNestedFamily ? "protected" : "public")
            .Append(" enum ")
            .Append(type.Name)
            .Append(" : ")
            .AppendLine(GetTypeName(type.GetEnumUnderlyingType()));
        sb.Append(' ', indent)
            .AppendLine("{");

        foreach (var value in Enum.GetValues(type))
        {
            sb.Append(' ', indent + 4)
                .Append(Enum.GetName(type, value))
                .Append(" = ")
                .AppendLine(Convert.ChangeType(value, type.GetEnumUnderlyingType())?.ToString());
        }

        sb.Append(' ', indent)
            .AppendLine("}");
    }

    static void AppendClassOrInterface(StringBuilder sb, int indent, Type type)
    {
        sb.Append(' ', indent)
            .Append(type.IsNestedFamily ? "protected " : "public ");

        if (type.IsClass)
        {
            if (type.IsSealed && type.IsAbstract)
                sb.Append("static ");
            else if (type.IsSealed)
                sb.Append("sealed ");
            else if (type.IsAbstract)
                sb.Append("abstract ");
        }

        if (type.IsClass)
            sb.Append("class");
        else if (type.IsInterface)
            sb.Append("interface");
        else
            throw new ArgumentException("Could not determine type for " + type);

        sb.Append(" ")
            .Append(type.Name);

        if (type.IsGenericType)
            AppendGenericArguments(sb, type.GetGenericArguments());

        var baseAndInterfaces = type.GetInterfaces().ToList();
        if (type.BaseType != null && type.BaseType != typeof(object))
            baseAndInterfaces.Insert(0, type.BaseType);

        if (baseAndInterfaces.Count > 0)
        {
            sb.Append(" : ")
                .Append(GetTypeList(baseAndInterfaces));
        }

        sb.AppendLine();

        sb.Append(' ', indent).AppendLine("{");
        AppendMembers(sb, type, indent + 4);
        sb.Append(' ', indent).AppendLine("}");
    }

    static void AppendMembers(StringBuilder sb, Type type, int indent)
    {
        var bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
                           BindingFlags.DeclaredOnly;

        var fields = type.GetFields(bindingFlags).Where(m => m.IsPublic || m.IsFamily)
            .OrderBy(p => p.Name, StringComparer.InvariantCulture);
        foreach (var field in fields)
            AppendField(sb, indent, field);

        var ctors = type.GetConstructors(bindingFlags).Where(m => m.IsPublic || m.IsFamily)
            .OrderBy(ctor => ctor.GetParameters().Length);
        foreach (var ctor in ctors)
            AppendConstructors(sb, indent, ctor);

        foreach (var evt in type.GetEvents(bindingFlags).OrderBy(p => p.Name, StringComparer.InvariantCulture))
            AppendEvent(sb, indent, evt);

        foreach (var property in type.GetProperties(bindingFlags).OrderBy(p => p.Name, StringComparer.InvariantCulture))
            AppendProperty(sb, type, indent, property);

        var methods = type.GetMethods(bindingFlags)
            .Where(m => m.IsPublic || m.IsFamily)
            .Where(m => !m.IsSpecialName)
            .OrderBy(p => p.Name, StringComparer.InvariantCulture)
            .ThenBy(p => GetTypeName(p.ReturnParameter.ParameterType), StringComparer.InvariantCulture)
            .ThenBy(p => p.GetParameters().Length);
        foreach (var method in methods)
            AppendMethod(sb, indent, method);

        var nested = type.GetNestedTypes(bindingFlags).Where(m => m.IsPublic || m.IsNestedPublic || m.IsNestedFamily)
            .OrderBy(p => p.Name, StringComparer.InvariantCulture);
        AppendTypes(sb, indent, nested);
    }

    static void AppendConstructors(StringBuilder sb, int indent, ConstructorInfo ctor)
    {
        AppendAttributes(sb, indent, ctor.GetCustomAttributesData(), false);

        sb.Append(' ', indent)
            .Append(ctor.IsPublic ? "public " : "protected ")
            .Append(ctor.DeclaringType.Name);

        AppendParameters(sb, null, ctor.GetParameters());

        sb.AppendLine(" { }");
    }

    static void AppendField(StringBuilder sb, int indent, FieldInfo field)
    {
        AppendAttributes(sb, indent, field.GetCustomAttributesData(), false);

        sb.Append(' ', indent)
            .Append(field.IsPublic ? "public " : "protected ");

        var isConst = field.IsLiteral && !field.IsInitOnly;

        if (isConst)
            sb.Append("const ");
        else if (field.IsStatic)
            sb.Append("static ");

        if (field.IsInitOnly)
            sb.Append("readonly ");

        sb.Append(GetTypeName(field.FieldType))
            .Append(" ")
            .Append(field.Name);

        if (isConst)
        {
            sb.Append(" = ")
                .Append(FormatValue(field.GetValue(null)));
        }

        sb.AppendLine(";");
    }

    static void AppendEvent(StringBuilder sb, int indent, EventInfo evt)
    {
        AppendAttributes(sb, indent, evt.GetCustomAttributesData(), false);

        var isClass = evt.DeclaringType.IsClass;

        var add = evt.GetAddMethod(true);
        if (!add.IsPublic && !add.IsFamily)
            return;

        sb.Append(' ', indent);

        if (isClass)
            sb.Append(add.IsPublic ? "public " : "protected ");

        if (add.IsStatic)
            sb.Append("static ");

        sb.Append("event ")
            .Append(GetTypeName(evt.EventHandlerType))
            .Append(" ")
            .Append(evt.Name)
            .AppendLine(";");
    }

    static void AppendProperty(StringBuilder sb, Type type, int indent, PropertyInfo property)
    {
        AppendAttributes(sb, indent, property.GetCustomAttributesData(), false);

        var isClass = property.DeclaringType.IsClass;
        var get = property.GetGetMethod(true);
        var set = property.GetSetMethod(true);
        var showGet = get != null && (get.IsPublic || get.IsFamily);
        var showSet = set != null && (set.IsPublic || set.IsFamily);

        var getIsPublic = get?.IsPublic ?? false;
        var setIsPublic = set?.IsPublic ?? false;

        if (showGet || showSet)
        {
            sb.Append(' ', indent);

            if (isClass)
                sb.Append(getIsPublic || setIsPublic ? "public " : "protected ");

            if (!type.IsInterface)
                AppendModifiers(sb, get!);

            sb.Append(GetTypeName(property.PropertyType))
                .Append(" ")
                .Append(property.Name)
                .Append(" { ");
            if (showGet)
            {
                if (setIsPublic && !getIsPublic)
                    sb.Append("protected ");
                sb.Append("get; ");
            }

            if (showSet)
            {
                if (getIsPublic && !setIsPublic)
                    sb.Append("protected ");
                sb.Append("set; ");
            }

            sb.AppendLine("}");
        }
    }

    static void AppendMethod(StringBuilder sb, int indent, MethodInfo method)
    {
        AppendAttributes(sb, indent, method.GetCustomAttributesData(), false);

        var isClass = method.DeclaringType.IsClass;

        sb.Append(' ', indent);

        if (isClass)
            sb.Append(method.IsPublic ? "public " : "protected ");

        if (isClass)
            AppendModifiers(sb, method);

        sb.Append(GetTypeName(method.ReturnType))
            .Append(" ")
            .Append(method.Name);

        if (method.IsGenericMethod)
            AppendGenericArguments(sb, method.GetGenericArguments());

        AppendParameters(sb, method, method.GetParameters());

        if (!method.IsAbstract)
            sb.AppendLine(" { }");
        else
            sb.AppendLine(";");
    }

    static void AppendGenericArguments(StringBuilder sb, Type[] genericArguments)
    {
        sb.Append("<")
            .Append(GetTypeList(genericArguments))
            .Append(">");
    }

    static string GetTypeList(IReadOnlyList<Type> genericArguments) =>
        string.Join(", ", genericArguments.Select(GetTypeName));

    static void AppendModifiers(StringBuilder sb, MethodInfo method)
    {
        if (method.IsStatic)
            sb.Append("static ");

        if (method.IsAbstract)
        {
            sb.Append("abstract ");
        }
        else if (method.IsVirtual && !method.IsFinal)
        {
            if (method.Attributes.HasFlag(MethodAttributes.VtableLayoutMask))
                sb.Append("virtual ");
            else
                sb.Append("override ");
        }
    }

    static void AppendParameters(StringBuilder sb, MethodInfo? method, ParameterInfo[] parameters)
    {
        sb.Append("(");
        if (method?.GetCustomAttribute<ExtensionAttribute>() != null)
            sb.Append("this ");

        var isFirst = true;
        foreach (var parameter in parameters)
        {
            if (!isFirst)
                sb.Append(", ");
            isFirst = false;

            if (parameter.IsOut)
                sb.Append("out ");
            if (parameter.IsIn)
                sb.Append("in ");
            if (parameter.IsDefined(typeof (ParamArrayAttribute), false))
                sb.Append("params ");

            sb.Append(GetTypeName(parameter.ParameterType))
                .Append(" ")
                .Append(parameter.Name);

            if (parameter.IsOptional)
            {
                sb.Append(" = ")
                    .Append(FormatValue(parameter.RawDefaultValue));
            }
        }

        sb.Append(")");
    }

    static string FormatValue(object value)
    {
        if (value == null)
            return "null";

        if (value is bool b)
            return b ? "true" : "false";

        var asString = value is string ||
                       value is Guid;

        return asString ? $"\"{value}\"" : "" + value;
    }

    static string GetTypeName(Type type)
    {
        if (type.IsArray)
            return GetTypeName(type.GetElementType()) + "[]";

        if (type.IsGenericParameter)
            return type.Name;

        if (type.IsByRef)
            return GetTypeName(type.GetElementType());

        if (type == typeof(string))
            return "string";
        if (type == typeof(object))
            return "object";
        if (type == typeof(int))
            return "int";
        if (type == typeof(bool))
            return "bool";
        if (type == typeof(char))
            return "char";
        if (type == typeof(void))
            return "void";

        var name = (type.FullName ?? type.Name).Replace('+', '.');

        if (!type.IsGenericType)
            return name;

        var garguments = string.Join(", ", type.GetGenericArguments().Select(GetTypeName));
        var nongenericName = name.Substring(0, name.IndexOf('`'));
        return $"{nongenericName}<{garguments}>";
    }
}
