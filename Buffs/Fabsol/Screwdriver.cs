using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Fabsol
{
    public class Screwdriver : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Screwdriver");
			Description.SetDefault("Piercing projectile damage boosted, life regen reduced");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
            canBeCleared = false;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).screwdriver = true;
		}
	}
}
