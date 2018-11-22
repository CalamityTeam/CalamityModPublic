using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
	public class BrimstoneWaifu : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Brimstone Waifu");
			Description.SetDefault("The brimstone elemental will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
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