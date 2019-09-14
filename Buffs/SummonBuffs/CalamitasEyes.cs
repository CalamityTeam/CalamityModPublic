using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.SummonBuffs
{
	public class CalamitasEyes : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Blighted Eyes");
			Description.SetDefault("Calamitas and her brothers will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetCalamityPlayer();
			if (player.ownedProjectileCounts[mod.ProjectileType("Calamitamini")] > 0)
			{
				modPlayer.cEyes = true;
			}
			if (!modPlayer.cEyes)
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
