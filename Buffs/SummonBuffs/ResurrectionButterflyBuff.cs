using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.SummonBuffs
{
	public class ResurrectionButterflyBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Resurrection Butterfly");
			Description.SetDefault("Sleep beneath the Cherry Blossoms, Red-White Butterfly");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.ownedProjectileCounts[mod.ProjectileType("PinkButterfly")] > 0 || player.ownedProjectileCounts[mod.ProjectileType("PurpleButterfly")] > 0)
			{
				modPlayer.resButterfly = true;
			}
			if (!modPlayer.resButterfly)
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
