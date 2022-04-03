using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class ElysianGuard : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elysian Guard");
            Description.SetDefault("Movement speed reduced, other stats buffed");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}
