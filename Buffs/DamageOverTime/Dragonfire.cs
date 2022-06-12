using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class Dragonfire : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragonfire");
            Description.SetDefault("Losing life, reduced movement speed");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().dragonFire = true;
        }
    }
}
