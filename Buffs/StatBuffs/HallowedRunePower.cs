using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class HallowedRunePower : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hallowed Power");
            Description.SetDefault("Minion damage boosted by 10%");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().hallowedPower = true;
            player.GetDamage<SummonDamageClass>() += 0.1f;
        }
    }
}
