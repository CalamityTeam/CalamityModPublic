using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.SummonBuffs
{
	public class BrimstoneWaifu : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Brimstone Elemental");
			Description.SetDefault("The brimstone elemental will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetCalamityPlayer();
			if (player.ownedProjectileCounts[mod.ProjectileType("BigBustyRose")] > 0)
			{
				modPlayer.bWaifu = true;
			}
			if (!modPlayer.bWaifu)
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
