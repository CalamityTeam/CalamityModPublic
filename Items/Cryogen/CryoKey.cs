using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Cryogen
{
	public class CryoKey : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cryo Key");
			Tooltip.SetDefault("Summons the magic of the ancient ice castle");
		}
		
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 18;
			item.maxStack = 20;
			item.rare = 5;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.consumable = true;
		}
		
		public override bool CanUseItem(Player player)
		{
			return player.ZoneSnow && !NPC.AnyNPCs(mod.NPCType("Cryogen"));
		}
		
		public override bool UseItem(Player player)
		{
			NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("Cryogen"));
			Main.PlaySound(SoundID.Roar, player.position, 0);
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.IceBlock, 50);
			recipe.AddIngredient(ItemID.SoulofNight, 3);
			recipe.AddIngredient(ItemID.SoulofLight, 3);
			recipe.AddIngredient(null, "EssenceofEleum", 5);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}