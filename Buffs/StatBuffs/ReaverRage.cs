using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class ReaverRage : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Rage");
            Description.SetDefault("You are angry");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().rRage = true;
        }
    }
}
