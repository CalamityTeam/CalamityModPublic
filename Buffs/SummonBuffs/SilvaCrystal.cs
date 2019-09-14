using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.SummonBuffs
{
	public class SilvaCrystal : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Silva Crystal");
			Description.SetDefault("The crystal will protect you");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			CalamityPlayer modPlayer = player.GetCalamityPlayer();
			if (player.ownedProjectileCounts[mod.ProjectileType("SilvaCrystal")] > 0)
			{
				modPlayer.sCrystal = true;
			}
			if (!modPlayer.sCrystal)
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
