using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class AndromedaCripple : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Andromeda Cripple");
            Description.SetDefault("You're slow");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}
