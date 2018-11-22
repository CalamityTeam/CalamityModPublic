using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
	public class ReaverOrb : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Reaver Orb");
			Description.SetDefault("The reaver orb will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.ownedProjectileCounts[mod.ProjectileType("ReaverOrb")] > 0)
			{
				modPlayer.rOrb = true;
			}
			if (!modPlayer.rOrb)
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