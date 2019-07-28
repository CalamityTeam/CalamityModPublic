using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
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
			player.GetModPlayer<CalamityPlayer>(mod).ceaselessHunger = true;
		}
	}
}
