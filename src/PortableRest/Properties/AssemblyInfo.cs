using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]

#if SIGNED
[assembly: InternalsVisibleTo("PortableRest.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c384ab1e009e83" +
                                                    "3f50689dfa1c441b3bf65f1d8086d0a7caa335fd03fc7a14f5c70e25d3c534e7a5453560ef8dce" +
                                                    "1b72b3340dfb5a408a09e300cfa50cdd55fc4ab6dbfaf6992ed0c33f5fdcaeb02bec36c87a0a10" +
                                                    "c88e22fa60aa6aae64c3ee2f00df2fb4103bd8be4c68df17fc9bacb116d14df7017b05208babce" +
                                                    "4044b6b4")]
#else
[assembly: InternalsVisibleTo("PortableRest.Tests")]
#endif
