using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;

namespace CalamityMod.Items.Weapons.Rogue
{
	public class LuminousStriker : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Luminous Striker");
			Tooltip.SetDefault("Send the stars back to where they belong\n"
							  +"Throws a stardust javelin trailed by rising stardust shards\n"
                              +"Explodes into additional stardust shards upon hitting enemies\n"
							  +"Stealth Strikes cause the stardust shards to fly alongside the javelin instead of rising");
        }

        public override void SafeSetDefaults()
        {
            item.width = 86;
			item.height = 102;
            item.damage = 149;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 30;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(1, 0, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<LuminousStrikerProj>();
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<SpearofPaleolith>());
			recipe.AddIngredient(ModContent.ItemType<ScourgeoftheSeas>());
			recipe.AddIngredient(ModContent.ItemType<Turbulance>());
            recipe.AddIngredient(ModContent.ItemType<MeldiateBar>(), 10);
			recipe.AddIngredient(ItemID.FragmentStardust, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
