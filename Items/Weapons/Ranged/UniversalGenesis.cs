using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 192;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 26;
            Item.knockBack = 6.5f;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 20f;
            Item.autoReuse = true;
            Item.Calamity().canFirePointBlankShots = true;

            Item.width = 158;
            Item.height = 60;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item38;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.Calamity().donorItem = true;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-50f, -8f);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 shootVelocity = velocity;
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            Vector2 gunTip = position + shootDirection * Item.scale * 100f;
            gunTip.Y -= 10f;
            float tightness = 1f;
            if (type == ProjectileID.Bullet)
                type = ModContent.ProjectileType<UniversalGenesisStarcaller>();
            for (float i = -tightness * 5f; i <= tightness * 5f; i += tightness * 2f)
            {
                Vector2 perturbedSpeed = shootVelocity.RotatedBy(MathHelper.ToRadians(i));
                Projectile.NewProjectile(source, gunTip, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }

            // Stars from above
            float speed = Item.shootSpeed;
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
                int star = Projectile.NewProjectile(source, spawnPos.X, spawnPos.Y, xVel, yVel, ModContent.ProjectileType<UniversalGenesisStar>(), starDmg, knockback, player.whoAmI, i, 1f);
                Main.projectile[star].extraUpdates = 2;
                Main.projectile[star].localNPCHitCooldown = 30;
            }
            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextBool();

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Disseminator>().
                AddIngredient(ItemID.StarCloak, 3).
                AddIngredient<ArmoredShell>(2).
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<AscendantSpiritEssence>(5).
                AddTile<CosmicAnvil>().
                Register();

            CreateRecipe().
                AddIngredient<Disseminator>().
                AddIngredient(ItemID.StarCloak, 3).
                AddIngredient<ArmoredShell>(2).
                AddIngredient<NebulousCore>().
                AddIngredient<AscendantSpiritEssence>(5).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
