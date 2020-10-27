using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CommandsRegions
{
    public class Plugin : RocketPlugin<Config>
    {
        public override TranslationList DefaultTranslations => new TranslationList
        {
            { "blocked", "Вы не можете использовать эту команду здесь" }
        };

        protected override void Load()
        {
            R.Commands.OnExecuteCommand += OnExecuteCommand;
        }

        protected override void Unload()
        {
            R.Commands.OnExecuteCommand -= OnExecuteCommand;
        }

        private void OnExecuteCommand(IRocketPlayer player, IRocketCommand command, ref bool cancel)
        {
            var uplayer = player as UnturnedPlayer;
            if (uplayer == null)
                return;

            var perm = new List<string> { "commandsregions.bypass" };
            if (R.Permissions.GetPermissions(player, perm).Count != 0)
                return;

            var pos = uplayer.Position;
            var locations = Configuration.Instance.Locations;
            float dist = 0f;
            Location location = default;
            int i;
            string name = command.Name;
            for (i = 0; i < locations.Length; i++)
            {
                var loc = locations[i];
                float x = pos.x - loc.X;
                float z = pos.z - loc.Z;
                float dist2 = x * x + z * z;

                // Find the nearest location that can affect to this command
                if (dist2 <= (loc.Radius * loc.Radius) &&
                    (location.Commands == null || dist > dist2) &&
                    (loc.IsWhitelist || Contains(loc.Commands, name)))
                {
                    dist = dist2;
                    location = loc;
                }
            }

            if (location.Commands == null)
                return;
            // if contains and is blocklist or not contains and whitelist => block command
            // Contains XOR Whitelist
            if (Contains(location.Commands, name) != location.IsWhitelist)
            {
                cancel = true;
                UnturnedChat.Say(player, this.Translations.Instance["blocked"], Color.red);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Contains(string[] commands, string str)
        {
            for (int i = 0; i < commands.Length; i++)
            {
                if (string.Equals(commands[i], str, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
