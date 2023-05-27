using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class ProfanedWeakness : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
    }
}
