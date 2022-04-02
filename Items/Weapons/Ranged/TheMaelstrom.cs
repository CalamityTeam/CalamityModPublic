using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TheMaelstrom : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Maelstrom");
            Tooltip.SetDefault("Fires charged Reaper Sharks that explode into water");
        }

        public override void SetDefaults()
        {
            item.damage = 397;
            item.width = 20;
            item.height = 12;
            item.useTime = 45;
            item.useAnimation = 45;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 3f;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.ranged = true;
            item.channel = true;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MaelstromHoldout>();
            item.shootSpeed = 20f;
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY).SafeNormalize(Vector2.UnitX * player.direction), ModContent.ProjectileType<MaelstromHoldout>(), 0, 0f, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<TheStorm>());
            recipe.AddIngredient(ModContent.ItemType<ReaperTooth>(), 3);
            recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 20);
            recipe.AddIngredient(ModContent.ItemType<Voidstone>(), 50);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
