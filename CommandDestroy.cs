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
    public class CommandDestroy : IRocketCommand
    {
        #region Properties
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "destroy";

        public string Help => "Destroys the barricade or structure that you are looking at.";

        public string Syntax => "/destroy";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string> { "breakandenter.destroy" };
        #endregion

        public void Execute(IRocketPlayer caller, string[] args)
        {
            Player player = ((UnturnedPlayer)caller).Player;
            PlayerLook look = player.look;

            if (PhysicsUtility.raycast(new Ray(look.aim.position, look.aim.forward), out RaycastHit hit, Mathf.Infinity, RayMasks.BARRICADE | RayMasks.STRUCTURE))
            {
                Interactable2SalvageBarricade barri = hit.transform.GetComponent<Interactable2SalvageBarricade>();
                Interactable2SalvageStructure struc = hit.transform.GetComponent<Interactable2SalvageStructure>();

                if (barri != null)
                {
                    BarricadeManager.tryGetInfo(barri.root, out byte x, out byte y, out ushort plant, out ushort index, out BarricadeRegion region);

                    region.barricades.RemoveAt(index);

                    BarricadeManager manager = (BarricadeManager)typeof(BarricadeManager).GetField("manager", BindingFlags.NonPublic |
                         BindingFlags.Static).GetValue(null);

                    manager.channel.send("tellTakeBarricade", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
                    {
                        x,
                        y,
                        plant,
                        index
                    });

                    UnturnedChat.Say(caller, Util.Translate("barricade_removed"));
                }
                else if (struc != null)
                {
                    StructureManager.tryGetInfo(struc.transform, out byte x, out byte y, out ushort index, out StructureRegion region);

                    region.structures.RemoveAt(index);

                    StructureManager manager = (StructureManager)typeof(StructureManager).GetField("manager", BindingFlags.NonPublic |
                         BindingFlags.Static).GetValue(null);

                    manager.channel.send("tellTakeStructure", ESteamCall.ALL, x, y, StructureManager.STRUCTURE_REGIONS, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
                    {
                        x,
                        y,
                        index,
                        (region.drops[index].model.position - player.transform.position).normalized * 100f
                    });

                    UnturnedChat.Say(caller, Util.Translate("structure_removed"));
                }
                else
                {
                    UnturnedChat.Say(caller, Util.Translate("invalid_destroy"));
                }
            }
            else
            {
                UnturnedChat.Say(caller, Util.Translate("no_object"));
            }
        }
    }
}