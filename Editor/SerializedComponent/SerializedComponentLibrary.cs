using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LTX.Tools.SerializedComponent;
using UnityEditor;

namespace LTX.Tools.Editor.SerializedComponent
{
    [InitializeOnLoad]
    public static class SerializedComponentLibrary
    {
        public struct TypeInfos
        {
            public string path;
            public Type type;
        }

        private static readonly List<TypeInfos> Types;

        static SerializedComponentLibrary()
        {
            Types = new();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var assemblyTypes = assembly.GetTypes();
                for (int i = 0; i < assemblyTypes.Length; i++)
                {
                    Type assemblyType = assemblyTypes[i];
                    if(assemblyType.IsSubclassOf(typeof(UnityEngine.Object)))
                        continue;
                    if(assemblyType.IsNestedPrivate)
                        continue;

                    if(assemblyType.GetConstructors().Any(ctx => ctx.GetParameters().Length > 0))
                        continue;

                    AddSerializedComponentMenuAttribute attribute = assemblyType.GetCustomAttribute<AddSerializedComponentMenuAttribute>();
                    var typeInfos = new TypeInfos()
                    {
                        type = assemblyType,
                        path = attribute == null ? $"Others/{assemblyType.Name}" : attribute.Path,
                    };

                    Types.Add(typeInfos);
                }
            }
        }


        public static IEnumerable<TypeInfos> GetTypes(string path = null, Type restrictType = null)
        {
            foreach (TypeInfos type in Types)
            {
                if (path != null && !type.path.StartsWith(path))
                    continue;

                if (restrictType != null && !IsSubClass(type.type, restrictType))
                    continue;

                yield return type;
            }
        }

        private static bool IsSubClass(Type type, Type restrictType)
        {
            if (type == restrictType)
                return true;

            if (restrictType.IsInterface)
            {
                Type[] interfaces = type.GetInterfaces();
                for (int j = 0; j < interfaces.Length; j++)
                {
                    if (interfaces[j] == restrictType)
                        return true;
                }
            }
            else
                return type.IsSubclassOf(restrictType);


            return false;
        }
    }
}