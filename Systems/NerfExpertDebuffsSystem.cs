using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class NerfExpertDebuffsSystem : ModSystem
    {
        public override void PostUpdateTime()
        {
            // Reduce the expert+ debuff time multiplier to the normal mode multiplier
            if (CalamityConfig.Instance.NerfExpertDebuffs)
            {
                var copy = Main.RegisteredGameModes[GameModeID.Expert];
                copy.DebuffTimeMultiplier = 1f;
                Main.RegisteredGameModes[GameModeID.Expert] = copy;

                copy = Main.RegisteredGameModes[GameModeID.Master];
                copy.DebuffTimeMultiplier = 1f;
                Main.RegisteredGameModes[GameModeID.Master] = copy;

                // NOTE -- While this may seem at a glance to be redundant and nonsensical, the underlying setter for this property is what causes the game mode properties to
                // be refreshed and copied from RegisteredGameModes. Without this, the above behavior is not reflected ingame, as GameModeData is a value type, not a reference type.
                Main.GameMode = Main.GameMode;
            }
            // If people want to suffer, let them suffer
			else
            {
                var copy = Main.RegisteredGameModes[GameModeID.Expert];
                copy.DebuffTimeMultiplier = 2f;
                Main.RegisteredGameModes[GameModeID.Expert] = copy;

                copy = Main.RegisteredGameModes[GameModeID.Master];
                copy.DebuffTimeMultiplier = 2.5f;
                Main.RegisteredGameModes[GameModeID.Master] = copy;

                Main.GameMode = Main.GameMode;
            }
        }
    }
}
