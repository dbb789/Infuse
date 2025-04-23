using System;
using System.Linq;

namespace Infuse.Editor
{
    public static class InfuseEditorUtil
    {
        public static string GetReadableTypeName(Type type)
        {
            if (type == null)
            {
                return "null";
            }
            
            if (type.IsGenericType)
            {
                var fullName = type.GetGenericTypeDefinition().FullName;
                var genericName = fullName.Substring(0, fullName.IndexOf('`'));
                var types = string.Join(", ", type.GetGenericArguments().Select(GetReadableTypeName));
                
                return $"{genericName}<{types}>";
            }
            else
            {
                return type.FullName;
            }
        }
    }
}
