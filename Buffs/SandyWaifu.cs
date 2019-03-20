using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
	public class SandyWaifu : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Sand Elemental");
			Description.SetDefault("The sand elemental will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.ownedProjectileCounts[mod.ProjectileType("SandyWaifu")] > 0)
			{
				modPlayer.sWaifu = true;
			}
			if (!modPlayer.sWaifu)
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