using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class AndromedaCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Andromeda Special Attack Cooldown");
            Description.SetDefault("Andromeda's special attack is recharging");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }
    }
}
