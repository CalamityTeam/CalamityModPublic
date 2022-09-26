using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class ProfanedWeakness : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Weakness");
            Description.SetDefault("You do less damage");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
    }
}
