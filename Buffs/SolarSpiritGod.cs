using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
	public class SolarSpiritGod : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Solar God Spirit");
			Description.SetDefault("The solar god spirit will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.ownedProjectileCounts[mod.ProjectileType("SolarGod")] > 0)
			{
				modPlayer.SPG = true;
			}
			if (!modPlayer.SPG)
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
			else
			{
				player.buffTime[buffIndex] = 18000;
			}
		}
	}
}
