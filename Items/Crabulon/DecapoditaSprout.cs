using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Crabulon
{
	public class DecapoditaSprout : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Decapodita Sprout");
			Tooltip.SetDefault("Summons the giant mushroom crab");
		}
		
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 18;
			item.maxStack = 20;
			item.rare = 2;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.consumable = true;
		}
		
		public override bool CanUseItem(Player player)
		{
			return player.ZoneGlowshroom && !NPC.AnyNPCs(mod.NPCType("CrabulonIdle"));
		}
		
		public override bool UseItem(Player player)
		{
            if (Main.netMode != 1)
            {
                NPC.NewNPC((int)(player.position.X + (float)(Main.rand.Next(-50, 51))), (int)(player.position.Y - 50f), mod.NPCType("CrabulonIdle"), 0, 0f, 0f, 0f, 0f, 255);
                Main.PlaySound(SoundID.Roar, player.position, 0);
            }
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.GlowingMushroom, 25);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}