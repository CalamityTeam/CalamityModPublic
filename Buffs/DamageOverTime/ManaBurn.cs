using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class ManaBurn : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mana Burn");
            Description.SetDefault("The excess of mana sears your body and mind");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        // Mana Burn's actual effects are controlled by CalamityPlayer code.
        // However, every frame you have Mana Burn, it automatically scans for and removes Mana Sickness if you somehow have it simultaneously.
        // This would only occur if other mods added Mana Burn to the player via esoteric means, as AddBuff is IL edited by Calamity.
        // If found, it adds the remaining time of that Mana Sickness to Mana Burn, capping at its usual duration cap.
        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().ManaBurn = true;

            int manaSicknessIndex = player.FindBuffIndex(BuffID.ManaSickness);
            if (manaSicknessIndex != -1)
            {
                // Set the remaining duration of Mana Sickness to 1 frame so it disappears either this or the next frame.
                // If it would disappear the next frame because it was already iterated over, then add 1 less frame of Mana Burn.
                bool extraFrame = manaSicknessIndex < buffIndex;
                int timeToAdd = player.buffTime[manaSicknessIndex] - (extraFrame ? 1 : 0);
                player.buffTime[manaSicknessIndex] = 1;

                // Add that time to Mana Burn.
                int newBuffTime = player.buffTime[buffIndex] + timeToAdd;
                if (newBuffTime > Player.manaSickTimeMax)
                    newBuffTime = Player.manaSickTimeMax;
                player.buffTime[buffIndex] = newBuffTime;
            }
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            // Logic copy pasted from Mana Sickness
            player.buffTime[buffIndex] += time;
            if (player.buffTime[buffIndex] > Player.manaSickTimeMax)
                player.buffTime[buffIndex] = Player.manaSickTimeMax;

            // Skip vanilla re-apply logic
            return true;
        }
    }
}
