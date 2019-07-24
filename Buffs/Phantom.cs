using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
	public class Phantom : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Phantom");
			Description.SetDefault("The phantom will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.ownedProjectileCounts[mod.ProjectileType("PhantomGuy")] > 0)
			{
				modPlayer.pGuy = true;
			}
			if (!modPlayer.pGuy)
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