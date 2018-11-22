using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LORED
{
	public class NO : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("SYNTAX ERROR");
			Tooltip.SetDefault("asdhgjhrew8943923203lajflfjndg89w34ut8340ofjaur823u31r");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 10;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.consumable = false;
		}
		
		public override bool UseItem(Player player)
		{
            /*NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("THELORDE"));
            Main.PlaySound(SoundID.Roar, player.position, 0);*/
            Main.instance = null;
			return true;
		}
	}
}