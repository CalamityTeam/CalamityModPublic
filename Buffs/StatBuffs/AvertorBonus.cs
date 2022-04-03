using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class AvertorBonus : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Avertor Bonus");
            Description.SetDefault("Boosted damage and life regeneration");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().avertorBonus = true;
        }
    }
}
