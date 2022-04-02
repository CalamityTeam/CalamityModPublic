using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Infinity : ModItem
    {
        internal int rotation = 0;
        internal bool limit = true;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infinity");
            Tooltip.SetDefault("Fires a barrage of energy bolts that split and bounce\n" +
                "Right click to fire a barrage of normal bullets\n" +
                "They say infinity is neverending, yet you hold it in your hands");
        }

        public override void SetDefaults()
        {
            item.damage = 45;
            item.ranged = true;
            item.width = 56;
            item.height = 24;
            item.useTime = 2;
            item.reuseDelay = 6;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.UseSound = SoundID.Item31;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 12f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                //If you right click, shoots an helix of normal bullets
                Vector2 num7 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(rotation));
                Vector2 num8 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(-rotation));
                int shot1 = Projectile.NewProjectile(position.X, position.Y, num7.X, num7.Y, type, damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[shot1].timeLeft = 180;
                int shot2 = Projectile.NewProjectile(position.X, position.Y, num8.X, num8.Y, type, damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[shot2].timeLeft = 180;
                //Code to constantly make the shooting go side to side to make the helix
                if (limit)
                {
                    rotation += 2;
                }
                else
                {
                    rotation -= 2;
                }
                if (rotation >= 15)
                {
                    limit = false;
                }
                else if (rotation <= -15)
                {
                    limit = true;
                }
                return false;
            }
            else
            {
                //If left click, do the same as above but spawn Charged Blasts instead
                Vector2 num7 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(rotation));
                Vector2 num8 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(-rotation));
                int shot1 = Projectile.NewProjectile(position.X, position.Y, num7.X, num7.Y, ModContent.ProjectileType<ChargedBlast>(), damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[shot1].timeLeft = 180;
                int shot2 = Projectile.NewProjectile(position.X, position.Y, num8.X, num8.Y, ModContent.ProjectileType<ChargedBlast>(), damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[shot2].timeLeft = 180;
                if (limit)
                {
                    rotation += 2;
                }
                else
                {
                    rotation -= 2;
                }
                if (rotation >= 15)
                {
                    limit = false;
                }
                else if (rotation <= -15)
                {
                    limit = true;
                }
                return false;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Shredder>());
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
