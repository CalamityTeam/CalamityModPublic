using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class SilvaRevival : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Silva Invulnerability");
            Description.SetDefault("You are invulnerable");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }
    }
}
