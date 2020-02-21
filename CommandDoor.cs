using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Effects;
using Rocket.Unturned.Player;
using SDG.Framework.Utilities;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ExtraConcentratedJuice.BreakAndEnter
{
    public class CommandDoor : IRocketCommand
    {
        #region Properties
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "door";

        public string Help => "Forces the door that you are looking at to open or close.";

        public string Syntax => "/door";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string> { "breakandenter.door" };
        #endregion

        public void Execute(IRocketPlayer caller, string[] args)
        {
            PlayerLook look = ((UnturnedPlayer)caller).Player.look;

            if (PhysicsUtility.raycast(new Ray(look.aim.position, look.aim.forward), out RaycastHit hit, Mathf.Infinity, RayMasks.BARRICADE))
            {
                InteractableDoorHinge hinge = hit.transform.GetComponent<InteractableDoorHinge>();

                if (hinge != null)
                {
                    InteractableDoor door = hinge.door;
                    bool open = !door.isOpen;

                    BarricadeManager.tryGetInfo(door.transform, out byte x, out byte y, out ushort plant, out ushort index, out BarricadeRegion region);

                    BarricadeManager manager = (BarricadeManager)typeof(BarricadeManager).GetField("manager", BindingFlags.NonPublic |
                         BindingFlags.Static).GetValue(null);

                    door.updateToggle(open);

                    manager.channel.send("tellToggleDoor", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
                    {
                        x,
                        y,
                        plant,
                        index,
                        open
                    });

                    UnturnedChat.Say(caller, Util.Translate("door_toggle", open ? "opened" : "closed"));
                }
                else
                {
                    UnturnedChat.Say(caller, Util.Translate("invalid_door"));
                }
            }
            else
            {
                UnturnedChat.Say(caller, Util.Translate("no_object"));
            }
        }
    }
}