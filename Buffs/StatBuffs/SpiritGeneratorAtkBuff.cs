using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
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
            Main.buffNoTimeDisplay[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.minionDamage += 0.1f;
        }
    }
}
