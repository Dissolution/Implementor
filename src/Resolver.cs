// namespace Implementor;
//
// public static class Resolver
// {
//     static Resolver()
//     {
//         AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
//         {
//             AssemblyName name = new(args.Name);
//             Assembly loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().FullName == name.FullName);
//             if (loadedAssembly != null)
//             {
//                 return loadedAssembly;
//             }
//
//             string resourceName = $"Namespace.{name.Name}.dll";
//
//             using Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
//             if (resourceStream == null)
//             {
//                 return null;
//             }
//     
//             using MemoryStream memoryStream = new MemoryStream();
//             resourceStream.CopyTo(memoryStream);
//
//             return Assembly.Load(memoryStream.ToArray());
//         };
//     }
// }