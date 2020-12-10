using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class PlanetaryAnnihilation : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Planetary Annihilation");
            Tooltip.SetDefault("Fires a storm of arrows from the sky\n" +
                "Wooden arrows are converted to homing energy bolts");
        }

        public override void SetDefaults()
        {
            item.damage = 52;
            item.ranged = true;
            item.width = 58;
            item.height = 102;
            item.useTime = 23;
            item.useAnimation = 23;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5.5f;
			item.value = CalamityGlobalItem.Rarity11BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.UseSound = SoundID.Item75;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<PlanetaryAnnihilationProj>();
            item.shootSpeed = 12f;
            item.useAmmo = AmmoID.Arrow;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float num72 = item.shootSpeed;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = Main.mouseY + Main.screenPosition.Y - vector2.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + Main.screenHeight - Main.mouseY - vector2.Y;
            }
            float num80 = (float)Math.Sqrt(num78 * num78 + num79 * num79);
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = player.direction;
                num79 = 0f;
                num80 = num72;
            }
            else
            {
                num80 = num72 / num80;
            }

            vector2 = new Vector2(player.position.X + player.width * 0.5f + (Main.rand.Next(201) * -(float)player.direction) + (Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
            vector2.X = (vector2.X + player.Center.X) / 2f + Main.rand.Next(-200, 201);
            vector2.Y -= 100f;
            num78 = Main.mouseX + Main.screenPosition.X - vector2.X;
            num79 = Main.mouseY + Main.screenPosition.Y - vector2.Y;
            if (num79 < 0f)
            {
                num79 *= -1f;
            }
            if (num79 < 20f)
            {
                num79 = 20f;
            }
            num80 = (float)Math.Sqrt(num78 * num78 + num79 * num79);
            num80 = num72 / num80;
            num78 *= num80;
            num79 *= num80;
            if (type == ProjectileID.WoodenArrowFriendly)
            {
                for (int i = 0; i < 7; i++)
                {
                    float speedX4 = num78 + Main.rand.Next(-120, 121) * 0.02f;
                    float speedY5 = num79 + Main.rand.Next(-120, 121) * 0.02f;
                    Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, ModContent.ProjectileType<PlanetaryAnnihilationProj>(), damage, knockBack, player.whoAmI, 0f, i);
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    float speedX4 = num78 + Main.rand.Next(-120, 121) * 0.02f;
                    float speedY5 = num79 + Main.rand.Next(-120, 121) * 0.02f;
                    int num121 = Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, type, damage, knockBack, player.whoAmI, 0f, 0f);
                    Main.projectile[num121].noDropItem = true;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddIngredient(ModContent.ItemType<CosmicBolter>());
            recipe.AddIngredient(ItemID.DaedalusStormbow);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
