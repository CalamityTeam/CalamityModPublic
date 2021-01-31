using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class DarkSpark : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Spark");
            Tooltip.SetDefault("And everything under the sun is in tune,\n" +
                "But the sun is eclipsed by the moon.");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 4));
        }

        public override void SetDefaults()
        {
            item.damage = 60;
            item.magic = true;
            item.mana = 10;
            item.width = 16;
            item.height = 16;
            item.useTime = 10;
            item.useAnimation = 10;
            item.reuseDelay = 5;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item13;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.knockBack = 0f;
            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 13;
            item.shoot = ModContent.ProjectileType<DarkSparkPrism>();
            item.shootSpeed = 30f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<DarkSparkPrism>(), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LastPrism);
            recipe.AddIngredient(ModContent.ItemType<DarkPlasma>(), 10);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 20);
            recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 30);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
