using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.Fabsol
{
    public class Trippy : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Trippy");
			Description.SetDefault("You see the world for what it truly is...and you also have a 50% increase to all damage");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).trippy = true;
		}
	}
}
