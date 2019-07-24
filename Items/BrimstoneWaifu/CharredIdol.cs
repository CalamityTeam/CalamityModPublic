using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.BrimstoneWaifu
{
	public class CharredIdol : ModItem
	{
		public override void SetStaticDefaults()
 		{
 			DisplayName.SetDefault("Charred Idol");
 			Tooltip.SetDefault("Use in the Brimstone Crag at your own risk\n" +
                "Summons the Brimstone Elemental");
 		}
		
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 18;
			item.maxStack = 20;
			item.rare = 6;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.consumable = true;
		}
		
		public override bool CanUseItem(Player player)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			return modPlayer.ZoneCalamity && !NPC.AnyNPCs(mod.NPCType("BrimstoneElemental"));
		}
		
		public override bool UseItem(Player player)
		{
			NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("BrimstoneElemental"));
			Main.PlaySound(SoundID.Roar, player.position, 0);
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.SoulofNight, 5);
			recipe.AddIngredient(null, "EssenceofChaos", 5);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}