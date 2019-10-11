using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.SummonBuffs
{
    public class SpiritGeneratorAtkBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Spirit Power");
            Description.SetDefault("Minion damage boosted by 10%");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().sPower = true;
        }
    }
}
