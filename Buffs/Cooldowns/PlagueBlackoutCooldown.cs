using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class PlagueBlackoutCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Plague Blackout Cooldown");
            Description.SetDefault("Your plague blackout ability is recharging");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }
    }
}
