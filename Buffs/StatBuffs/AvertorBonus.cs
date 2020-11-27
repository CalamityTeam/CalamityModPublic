using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class AvertorBonus : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Avertor Bonus");
            Description.SetDefault("Boosted damage and life regeneration");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			player.Calamity().avertorBonus = true;
		}
	}
}
