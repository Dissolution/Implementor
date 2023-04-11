namespace Jay.SourceGen.Extensions
{
    public static class MemberInfoExtensions
    {
        public static string GetFullName(this MemberInfo member)
        {
            if (member is Type type)
                return type.FullName;
            return $"{member.DeclaringType.Namespace}.{member.Name}";
        }

        public static Type GetReturnType(this MethodBase method)
        {
            if (method is MethodInfo methodInfo)
                return methodInfo.ReturnType;
            if (method is ConstructorInfo constructorInfo)
            {
                if (constructorInfo.IsStatic)
                    return typeof(void);
                return constructorInfo.DeclaringType!;
            }
            throw new ArgumentException();
        }
    }
}