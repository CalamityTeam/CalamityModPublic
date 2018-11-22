using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
	public class CloudyWaifu : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Cloudy Waifu");
			Description.SetDefault("The cloud elemental will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.ownedProjectileCounts[mod.ProjectileType("CloudyWaifu")] > 0)
			{
				modPlayer.cWaifu = true;
			}
			if (!modPlayer.cWaifu)
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