using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
	public class Crimslime : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Crimslime");
			Description.SetDefault("The crimslime will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.ownedProjectileCounts[mod.ProjectileType("Crimslime")] > 0)
			{
				modPlayer.cSlime2 = true;
			}
			if (!modPlayer.cSlime2)
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