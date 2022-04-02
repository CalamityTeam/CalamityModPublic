using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class UniversalGenesis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Universal Genesis");
            Tooltip.SetDefault("Let the starry sky fall upon your enemies\n" +
                "Fires a spread of bullets from the gun and a flurry of stars to rain down on the cursor\n" +
                "Converts musket balls into starcaller shots that summon additional stars on enemy hits\n" +
                "50% chance to not consume ammo");
        }

        public override void SetDefaults()
        {
            item.damage = 192;
            item.ranged = true;
            item.useTime = item.useAnimation = 26;
            item.knockBack = 6.5f;
            item.useAmmo = AmmoID.Bullet;
            item.shoot = ProjectileID.Bullet;
            item.shootSpeed = 20f;
            item.autoReuse = true;
            item.Calamity().canFirePointBlankShots = true;

            item.width = 158;
            item.height = 60;
            item.noMelee = true;
            item.UseSound = SoundID.Item38;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.Calamity().donorItem = true;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-50f, -8f);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 shootVelocity = new Vector2(speedX, speedY);
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            Vector2 gunTip = position + shootDirection * item.scale * 100f;
            gunTip.Y -= 10f;
            float tightness = 1f;
            if (type == ProjectileID.Bullet)
                type = ModContent.ProjectileType<UniversalGenesisStarcaller>();
            for (float i = -tightness * 5f; i <= tightness * 5f; i += tightness * 2f)
            {
                Vector2 perturbedSpeed = shootVelocity.RotatedBy(MathHelper.ToRadians(i));
                Projectile.NewProjectile(gunTip, perturbedSpeed, type, damage, knockBack, player.whoAmI);
            }

            // Stars from above
            float speed = item.shootSpeed;
            Vector2 spawnPos = player.RotatedRelativePoint(player.MountedCenter, true);
            int starAmt = 6;
            int starDmg = (int)(damage * 0.4);
            for (int i = 0; i < starAmt; i++)
            {
                spawnPos = new Vector2(player.Center.X + (Main.rand.Next(201) * -(float)player.direction) + (Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                spawnPos.X = (spawnPos.X + player.Center.X) / 2f + Main.rand.Next(-200, 201);
                spawnPos.Y -= 100 + i;
                float xDist = Main.mouseX + Main.screenPosition.X - spawnPos.X;
                float yDist = Main.mouseY + Main.screenPosition.Y - spawnPos.Y;
                if (yDist < 0f)
                {
                    yDist *= -1f;
                }
                if (yDist < 20f)
                {
                    yDist = 20f;
                }
                float travelDist = (float)Math.Sqrt(xDist * xDist + yDist * yDist);
                travelDist = speed / travelDist;
                xDist *= travelDist;
                yDist *= travelDist;
                float xVel = xDist + Main.rand.NextFloat(-0.6f, 0.6f);
                float yVel = yDist + Main.rand.NextFloat(-0.6f, 0.6f);
                int star = Projectile.NewProjectile(spawnPos.X, spawnPos.Y, xVel, yVel, ModContent.ProjectileType<UniversalGenesisStar>(), starDmg, knockBack, player.whoAmI, i, 1f);
                Main.projectile[star].extraUpdates = 2;
                Main.projectile[star].localNPCHitCooldown = 30;
            }
            return false;
        }

        public override bool ConsumeAmmo(Player player) => Main.rand.NextBool();

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Interfacer>());
            recipe.AddIngredient(ItemID.StarCloak, 3); // and that's how you know it's a donor item
            recipe.AddIngredient(ModContent.ItemType<ArmoredShell>(), 2);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 5);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
            // but why
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Interfacer>());
            recipe.AddIngredient(ItemID.StarCloak, 3); // and that's how you know it's a donor item
            recipe.AddIngredient(ModContent.ItemType<ArmoredShell>(), 2);
            recipe.AddIngredient(ModContent.ItemType<NebulousCore>());
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 5);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
