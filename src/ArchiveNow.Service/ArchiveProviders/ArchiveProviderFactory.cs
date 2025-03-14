using System;
using System.Collections.Generic;
using ArchiveNow.Providers.Core;
using ArchiveNow.Providers.Core.EntryTransforms;
using ArchiveNow.Providers.Core.PasswordProviders;
using ArchiveNow.Providers.Listing;
using ArchiveNow.Providers.Lz4;
using ArchiveNow.Providers.MsiPackage;
//using ArchiveNow.Providers.MsiPackage;
//using ArchiveNow.Providers.RoboCopy;

namespace ArchiveNow.Service.ArchiveProviders
{
    public class ArchiveProviderFactory
    {
        private static readonly IDictionary<string, Func<IArchiveFilePathBuilder, IArchiveEntryTransform, IPasswordProvider, IArchiveProvider>> ProviderMap =
                new Dictionary<string, Func<IArchiveFilePathBuilder, IArchiveEntryTransform, IPasswordProvider, IArchiveProvider>>(StringComparer.OrdinalIgnoreCase)
                    {
                        {
                            "SevenZip",
                            (pathBuilder, entryTransform, passwordProvider) =>
                                new SevenZipArchiveProvider(pathBuilder, passwordProvider)
                        },
                        {
                            "SharpZip",
                            (pathBuilder, entryTransform, passwordProvider) =>
                                new SharpZipArchiveProvider(pathBuilder, entryTransform, passwordProvider)
                        },
                        {
                            "SystemZip",
                            (pathBuilder, entryTransform, passwordProvider) => 
                                new SystemZipArchiveProvider(pathBuilder, entryTransform, passwordProvider)
                        },
                        {
                            "LiteDb",
                            (pathBuilder, entryTransform, passwordProvider) =>
                                new LiteDbArchiveProvider(pathBuilder, entryTransform, passwordProvider)
                        },
                        {
                            "LZ4",
                            (pathBuilder, entryTransform, passwordProvider) =>
                                new Lz4ArchiveProvider(pathBuilder, passwordProvider)
                        },
                        {
                            "Msi",
                            (pathBuilder, entryTransform, passwordProvider) =>
                                new MsiArchiveProvider(pathBuilder, entryTransform, passwordProvider)
                        },
                        //{
                        //    "RoboCopy",
                        //    (pathBuilder, entryTransform, passwordProvider) => 
                        //        new RoboCopyArchiveProvider(pathBuilder)
                        //},
                        {
                            "Listing",
                            (pathBuilder, entryTransform, passwordProvider) =>
                                new ListingProvider(pathBuilder)
                        },
                    };

        private static readonly Func<IArchiveFilePathBuilder, IArchiveEntryTransform, IPasswordProvider, IArchiveProvider> Default;

        static ArchiveProviderFactory()
        {
            Default = ProviderMap["SevenZip"];
            ProviderMap.Add("Default", Default);
        }

        public static IArchiveProvider Build(
            string name,
            IArchiveFilePathBuilder pathBuilder,
            IArchiveEntryTransform entryTransform = null,
            IPasswordProvider passwordProvider = null)
        {
            var creator = ProviderMap.ContainsKey(name) ? ProviderMap[name] : Default;

            return creator(pathBuilder, entryTransform, passwordProvider); 
        }
    }

    public class ArchiveNowContext
    {

    }
}