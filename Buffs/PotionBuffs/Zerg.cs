using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.PotionBuffs
{
    public class Zerg : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Zerg");
			Description.SetDefault("Spawn rates are boosted");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetCalamityPlayer().zerg = true;
		}
	}
}
