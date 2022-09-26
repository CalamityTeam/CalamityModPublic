using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class SilvaRevival : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silva Immunity");
            Description.SetDefault("You are unkillable and immune to most debuffs");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}
