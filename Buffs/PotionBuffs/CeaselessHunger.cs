using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.PotionBuffs
{
    public class CeaselessHunger : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Ceaseless Hunger");
			Description.SetDefault("You are sucking up all the items");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetCalamityPlayer().ceaselessHunger = true;
		}
	}
}
