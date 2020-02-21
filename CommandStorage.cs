using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Framework.Utilities;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ExtraConcentratedJuice.BreakAndEnter
{
    public class CommandStorage : IRocketCommand
    {
        #region Properties
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "storage";

        public string Help => "Lets you view the contents of the storage unit that you are looking at.";

        public string Syntax => "/storage";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string> { "breakandenter.storage" };
        #endregion

        public void Execute(IRocketPlayer caller, string[] args)
        {
            Player player = ((UnturnedPlayer)caller).Player;
            PlayerLook look = player.look;

            if (PhysicsUtility.raycast(new Ray(look.aim.position, look.aim.forward), out RaycastHit hit, Mathf.Infinity, RayMasks.BARRICADE))
            {
                InteractableStorage storage = hit.transform.GetComponent<InteractableStorage>();

                if (storage != null)
                {
                    storage.isOpen = true;
                    storage.opener = player;
                    player.inventory.isStoring = true;
                    player.inventory.isStorageTrunk = false;
                    player.inventory.storage = storage;
                    player.inventory.updateItems(PlayerInventory.STORAGE, storage.items);
                    player.inventory.sendStorage();

                    UnturnedChat.Say(caller, Util.Translate("storage_open"));
                }
                else
                {
                    UnturnedChat.Say(caller, Util.Translate("invalid_storage"));
                }
            }
            else
            {
                UnturnedChat.Say(caller, Util.Translate("no_object"));
            }
        }
    }
}