using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.AquaticScourge
{
	public class Seafood : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Seafood");
			Tooltip.SetDefault("The sulphuric sand stirs...");
		}
		
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 28;
			item.maxStack = 20;
			item.rare = 5;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.consumable = true;
		}
		
		public override bool CanUseItem(Player player)
		{
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            return modPlayer.ZoneSulphur && !NPC.AnyNPCs(mod.NPCType("AquaticScourgeHead"));
		}
		
		public override bool UseItem(Player player)
		{
			NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("AquaticScourgeHead"));
			Main.PlaySound(SoundID.Roar, player.position, 0);
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "SulphurousSand", 10);
            recipe.AddIngredient(ItemID.Starfish, 5);
            recipe.AddIngredient(ItemID.SharkFin, 3);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}