using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class ConcoctionCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Concoction Cooldown");
            Description.SetDefault("Revive is recharging");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }
    }
}
