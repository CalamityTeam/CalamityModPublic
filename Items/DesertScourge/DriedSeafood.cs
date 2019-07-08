using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Items.DesertScourge
{
	public class DriedSeafood : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Desert Medallion");
			Tooltip.SetDefault("The desert sand stirs...\n" +
                "Summons the Desert Scourge");
		}
		
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 28;
			item.maxStack = 20;
			item.rare = 2;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.consumable = true;
		}
		
		public override bool CanUseItem(Player player)
		{
			return player.ZoneDesert && !NPC.AnyNPCs(mod.NPCType("DesertScourgeHead"));
		}
		
		public override bool UseItem(Player player)
		{
			NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("DesertScourgeHead"));
			if (CalamityWorld.revenge)
			{
				NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("DesertScourgeHeadSmall"));
				NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("DesertScourgeHeadSmall"));
			}
			Main.PlaySound(SoundID.Roar, player.position, 0);
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.SandBlock, 15);
			recipe.AddIngredient(ItemID.AntlionMandible, 3);
			recipe.AddIngredient(ItemID.Cactus, 10);
			recipe.AddIngredient(null, "StormlionMandible");
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}