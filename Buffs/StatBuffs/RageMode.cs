using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class RageMode : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Rage Mode");
            Description.SetDefault("50% damage boost. 10% to 20% DR, increased based on how full the Rage Meter is.");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer mp = player.Calamity();
            mp.rageModeActive = true;
            float rageRatio = MathHelper.Clamp(mp.rage / mp.rageMax, 0f, 1f);
            player.endurance += MathHelper.Lerp(CalamityPlayer.MinRageDR, CalamityPlayer.MaxRageDR, rageRatio);
        }
    }
}
