using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Mapsui")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Geodan")]
[assembly: AssemblyProduct("Mapsui")]
[assembly: AssemblyCopyright("Copyright © Geodan 2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]

//[assembly: InternalsVisibleTo("Mapsui.Samples.iOS")]
//[assembly: InternalsVisibleTo("Mapsui.Samples.Silverlight")]
//[assembly: InternalsVisibleTo("Mapsui.Samples.Silverlight.Web")]
//[assembly: InternalsVisibleTo("Mapsui.Samples.Windows8")]
//[assembly: InternalsVisibleTo("Mapsui.Samples.WindowsPhone8")]
//[assembly: InternalsVisibleTo("Mapsui.Samples.WinForms")]
//[assembly: InternalsVisibleTo("Mapsui.Samples.Wpf")]
//[assembly: InternalsVisibleTo("Mapsui.Tests.Common")]
//[assembly: InternalsVisibleTo("Mapsui.Tests")]
//[assembly: InternalsVisibleTo("Mapsui.UI.Android")]
//[assembly: InternalsVisibleTo("Mapsui.UI.iOS")]
//[assembly: InternalsVisibleTo("Mapsui.UI.WinForms")]
//[assembly: InternalsVisibleTo("Mapsui.UI.Xaml")]
//[assembly: InternalsVisibleTo("Mapsui.UI.Xaml-SL")]
//[assembly: InternalsVisibleTo("Mapsui.UI.Xaml-UA")]
//[assembly: InternalsVisibleTo("Mapsui.UI.Xaml-W8")]
//[assembly: InternalsVisibleTo("Mapsui.UI.Xaml-WP8")]

[assembly: InternalsVisibleTo("Mapsui.Desktop, PublicKey=00240000048000009400000006020000002400005253413100040000010001000943df372fbe69b24af5f1f09cc47953a8f625c5417f7e705b7724a5232f64fd906bac23844b489026a73a55d50aab6ee69c1b2b4962dc421663aec74a336d7430c286fd42336b916b530a4490c67595d202b48c80f842ad6c027b3be45e58a950790be4dbb9a715f27423a867c59dd9901798f4ded6c7a3a71c8b883db2d8f4")]
[assembly: InternalsVisibleTo("Mapsui.Geometries, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d996abf3aceb6e822a1f5cc63baa0af8171cf8105aa45e70b8c452896c2967efb788efa0eed390f6d936833698444814c7bec09a8e6b2fdcd3c29de3cfc577f765dd7550693a4e19c80493205e6079fa5d34b284c3b37a95dce90d3188e1ac83b8d9a244bc518f1324bc78c6ed01253c3eeca1edb449f2e9c40228defe7189df")]
//[assembly: InternalsVisibleTo("Mapsui.Providers.Tests")]
//[assembly: InternalsVisibleTo("Mapsui.Rendering.Android")]
//[assembly: InternalsVisibleTo("Mapsui.Rendering.Android.Tests")]
[assembly: InternalsVisibleTo("Mapsui.Rendering.Gdi, PublicKey=00240000048000009400000006020000002400005253413100040000010001005b022276e96923bd5ccaa18abbe81d73f5bd4c114abf9668ea0a922ce79bc450094f3088760727d5fe8729f735a596ef53426345450472f1408a5cff4d6bba4c714b9e9bed6df5bf2321bb942b21f80e3f4545aec992d953e723d341471f9a3542ad019ab7b1288e3eddb463ce0672d3b9af9d311b275ab2b9e3e7bc62d058ae")]
//[assembly: InternalsVisibleTo("Mapsui.Rendering.Gdi.Tests")]
//[assembly: InternalsVisibleTo("Mapsui.Rendering.iOS")]
//[assembly: InternalsVisibleTo("Mapsui.Rendering.OpenTK.Android.Tests")]
[assembly: InternalsVisibleTo("Mapsui.Rendering.OpenTK, PublicKey=002400000480000094000000060200000024000052534131000400000100010007376b9b72c209d4028b1496326a9c4dfb76217a919c46cbc3d91f8a681f70a4059fc1e95674ef9d81c167fc1084ab9a0dc2c899b7e5ecaad62293d02dff94fc683f726e7ac26be20e3fd2ccd7e3fc084e957596e86a72012f131e76663e0ea89fc326e8dc9f8260f9aa09ac691d2e290769c6660fb76afa8964a10d71890296")]
//[assembly: InternalsVisibleTo("Mapsui.Rendering.OpenTK.iOS.Tests")]
//[assembly: InternalsVisibleTo("Mapsui.Rendering.OpenTK.Tests")]
//[assembly: InternalsVisibleTo("Mapsui.Rendering.OpenTK-A")]
//[assembly: InternalsVisibleTo("Mapsui.Rendering.OpenTK-iOS")]
[assembly: InternalsVisibleTo("Mapsui.Rendering.Xaml, PublicKey=00240000048000009400000006020000002400005253413100040000010001000105b9bb2a72b21c4e56a89620474e14735a3f137d1a25818aadfb3f2441266f796993cfcd054324517fe25e10553fb2d71d15a6b0605dce3a9e74a39cc659c0565fed30bd661fcb6b6133fbd71a0991fe83cfeaffdd3ef997d8ef0e5b90a7a0744405c86c02c66735e12f385924e910bf099abe7196c5537c8e943cbe81efd7")]
//[assembly: InternalsVisibleTo("Mapsui.Rendering.Xaml.Tests")]
//[assembly: InternalsVisibleTo("Mapsui.Rendering.Xaml-SL")]
//[assembly: InternalsVisibleTo("Mapsui.Rendering.Xaml-UA")]
//[assembly: InternalsVisibleTo("Mapsui.Rendering.Xaml-W8")]
//[assembly: InternalsVisibleTo("Mapsui.Rendering.Xaml-WP8")]
//[assembly: InternalsVisibleTo("Mapsui.Samples.Android")]
//[assembly: InternalsVisibleTo("Mapsui.Samples.Common")]
//[assembly: InternalsVisibleTo("Mapsui.Samples.Common.Desktop")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("0.8.0")]
[assembly: AssemblyVersion("0.8.0")]
[assembly: AssemblyFileVersion("0.8.0")]
