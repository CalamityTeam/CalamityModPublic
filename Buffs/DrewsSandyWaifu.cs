using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
	public class DrewsSandyWaifu : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Sandy Waifu");
			Description.SetDefault("The sand elemental will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.ownedProjectileCounts[mod.ProjectileType("DrewsSandyWaifu")] > 0)
			{
				modPlayer.dWaifu = true;
			}
			if (!modPlayer.dWaifu)
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