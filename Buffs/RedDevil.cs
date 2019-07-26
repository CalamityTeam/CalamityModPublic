using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
	public class RedDevil : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Red Devil");
			Description.SetDefault("The red devil will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.ownedProjectileCounts[mod.ProjectileType("RedDevil")] > 0)
			{
				modPlayer.rDevil = true;
			}
			if (!modPlayer.rDevil)
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
