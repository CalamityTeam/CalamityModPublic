using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class TyrantsFury : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tyrant's Fury");
            Description.SetDefault("30% increased melee damage and 10% increased melee crit chance");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().tFury = true;
        }
    }
}
