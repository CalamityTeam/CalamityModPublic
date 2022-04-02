using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class AstrealDefeat : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astreal Defeat");
            Tooltip.SetDefault("Ethereal bow of the tyrant king's mother\n" +
                "The mother strongly discouraged acts of violence throughout her life\n" +
                "Though she kept this bow close, to protect her family in times of great disaster\n" +
                "All arrows are converted to Astreal Arrows that emit flames as they travel");
        }

        public override void SetDefaults()
        {
            item.damage = 153;
            item.ranged = true;
            item.width = 40;
            item.height = 78;
            item.useTime = 4;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5.5f;
            item.value = CalamityGlobalItem.Rarity11BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item102;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<AstrealArrow>();
            item.shootSpeed = 4f;
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);
            float speed = velocity.Length();
            if (speed > 8f)
                velocity *= 8f / speed;

            // Always fires Astreal Arrows, regardless of ammo chosen.
            // Normally we like to allow bows to fire normal arrows but this weapon is incredibly overpowered when that is allowed.
            type = item.shoot;
            float aiVar = Main.rand.Next(4);

            Projectile.NewProjectile(position, velocity, type, damage, knockBack, player.whoAmI, aiVar);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpiritFlame);
            recipe.AddIngredient(ItemID.ShadowFlameBow);
            recipe.AddIngredient(ModContent.ItemType<GreatbowofTurmoil>());
            recipe.AddIngredient(ModContent.ItemType<BladedgeGreatbow>());
            recipe.AddIngredient(ModContent.ItemType<DarkechoGreatbow>());
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
