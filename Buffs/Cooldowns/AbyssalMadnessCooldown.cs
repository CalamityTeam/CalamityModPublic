using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class AbyssalMadnessCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Abyssal Madness Cooldown");
            Description.SetDefault("Your abyssal madness is recharging");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }
    }
}
