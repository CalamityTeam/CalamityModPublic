using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
	public class Herring : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Herring");
			Description.SetDefault("The herring will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.ownedProjectileCounts[mod.ProjectileType("Herring")] > 0)
			{
				modPlayer.herring = true;
			}
			if (!modPlayer.herring)
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
