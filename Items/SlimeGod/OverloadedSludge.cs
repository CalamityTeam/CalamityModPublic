using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SlimeGod
{
	public class OverloadedSludge : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Overloaded Sludge");
			Tooltip.SetDefault("It looks corrupted\n" +
                "Summons the Slime God");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.maxStack = 20;
			item.rare = 4;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.consumable = true;
		}
		
		public override bool CanUseItem(Player player)
		{
			return !NPC.AnyNPCs(mod.NPCType("SlimeGodCore")) && !NPC.AnyNPCs(mod.NPCType("SlimeGod")) && 
                !NPC.AnyNPCs(mod.NPCType("SlimeGodSplit")) && !NPC.AnyNPCs(mod.NPCType("SlimeGodRun")) && !NPC.AnyNPCs(mod.NPCType("SlimeGodRunSplit"));
		}
		
		public override bool UseItem(Player player)
		{
			NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("SlimeGod"));
			NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("SlimeGodRun"));
			NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("SlimeGodCore"));
			Main.PlaySound(SoundID.Roar, player.position, 0);
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "EbonianGel", 25);
			recipe.AddIngredient(ItemID.EbonstoneBlock, 25);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "EbonianGel", 25);
			recipe.AddIngredient(ItemID.CrimstoneBlock, 25);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "PurifiedGel", 25);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}