﻿using ACMESharp.POSH.Vault;
using System;
using System.IO;
using System.Management.Automation;
using ACMESharp.Util;

namespace ACMESharp.POSH
{
    [Cmdlet(VerbsData.Edit, "ProviderConfig", DefaultParameterSetName = PSET_LIST)]
    public class EditProviderConfig : Cmdlet
    {
        public const string PSET_LIST = "List";
        public const string PSET_EDIT = "Edit";

        [Parameter(ParameterSetName = PSET_LIST, Mandatory = true)]
        public SwitchParameter List
        { get; set; }

        [Parameter(ParameterSetName = PSET_EDIT, Mandatory = true)]
        public string Ref
        { get; set; }

        [Parameter]
        public string EditWith
        { get; set; }

        [Parameter]
        public string VaultProfile
        { get; set; }

        protected override void ProcessRecord()
        {
            using (var vp = InitializeVault.GetVaultProvider(VaultProfile))
            {
                vp.OpenStorage();
                var v = vp.LoadVault();

                if (v.ProviderConfigs == null || v.ProviderConfigs.Count < 1)
                    throw new InvalidOperationException("No provider configs found");

                if (List)
                {
                    foreach (var item in v.ProviderConfigs.Values)
                        WriteObject(item);
                }
                else
                {
                    var pc = v.ProviderConfigs.GetByRef(Ref);
                    if (pc == null)
                        throw new Exception("Unable to find Provider Config for the given reference");
                    var pcFilePath = Path.GetFullPath($"{pc.Id}.json");

                    // Copy out the asset to a temp file for editing
                    var pcAsset = vp.GetAsset(VaultAssetType.ProviderConfigInfo, $"{pc.Id}.json");
                    using (var temp = new TemporaryFile())
                    {
                        using (var s = vp.LoadAsset(pcAsset))
                        {
                            using (var fs = new FileStream(temp.FileName, FileMode.Create))
                            {
                                s.CopyTo(fs);
                            }
                        }
                        NewProviderConfig.EditFile(temp.FileName, EditWith);

                        using (Stream fs = new FileStream(temp.FileName, FileMode.Open),
                            assetStream = vp.SaveAsset(pcAsset))
                        {
                            fs.CopyTo(assetStream);
                        }
                    }
                }
            }
        }
    }
}
