using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
	public class SirenLure : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Siren");
			Description.SetDefault("The siren will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.ownedProjectileCounts[mod.ProjectileType("SirenLure")] > 0)
			{
				modPlayer.slWaifu = true;
			}
			if (!modPlayer.slWaifu)
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
