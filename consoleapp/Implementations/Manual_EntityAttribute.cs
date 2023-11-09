// #nullable enable
//
// namespace ConsoleApp.Implementations;
//
// [AttributeUsage(AttributeTargets.Interface)]
// public sealed class EntityAttribute : Attribute
// {
//     internal string[]? KeyNames { get; }
//
//     public EntityAttribute()
//     {
//         this.KeyNames = null;
//     }
//
//     public EntityAttribute(string keyName)
//     {
//         this.KeyNames = new string[1] { keyName };
//     }
//
//     public EntityAttribute(params string[]? keyNames)
//     {
//         if (keyNames is null || keyNames.Length == 0)
//         {
//             this.KeyNames = null;
//         }
//         else
//         {
//             this.KeyNames = keyNames;
//         }
//     }
// }