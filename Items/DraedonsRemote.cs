using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Items
{
	public class DraedonsRemote : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Draedon's Remote");
			Tooltip.SetDefault("Mayhem...");
		}
		
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 28;
			item.maxStack = 20;
			item.rare = 8;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.consumable = true;
		}
		
		public override bool CanUseItem(Player player)
		{
			return !Main.dayTime && !NPC.AnyNPCs(NPCID.TheDestroyer) && !NPC.AnyNPCs(NPCID.SkeletronPrime) && !NPC.AnyNPCs(NPCID.Spazmatism) && !NPC.AnyNPCs(NPCID.Retinazer);
		}
		
		public override bool UseItem(Player player)
		{
            CalamityGlobalNPC.DraedonMayhem = true;
            if (Main.netMode == 2)
            {
                NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
            }
            NPC.SpawnOnPlayer(player.whoAmI, NPCID.TheDestroyer);
            NPC.SpawnOnPlayer(player.whoAmI, NPCID.SkeletronPrime);
            NPC.SpawnOnPlayer(player.whoAmI, NPCID.Spazmatism);
            NPC.SpawnOnPlayer(player.whoAmI, NPCID.Retinazer);
            Main.PlaySound(SoundID.Roar, player.position, 0);
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.IronBar, 10);
            recipe.AddIngredient(ItemID.SoulofLight, 15);
            recipe.AddIngredient(ItemID.SoulofNight, 15);
			recipe.AddIngredient(null, "MeldBlob", 20);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LeadBar, 10);
            recipe.AddIngredient(ItemID.SoulofLight, 15);
            recipe.AddIngredient(ItemID.SoulofNight, 15);
            recipe.AddIngredient(null, "MeldBlob", 20);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}