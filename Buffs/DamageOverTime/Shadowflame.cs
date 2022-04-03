using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class Shadowflame : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowflame");
            Description.SetDefault("Losing life");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().shadowflame = true;
        }
    }
}
