using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Photoviscerator : ModItem
    {
        public const float AmmoNotConsumeChance = 0.9f;
        private const float AltFireShootSpeed = 17f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Photoviscerator");
            Tooltip.SetDefault("90% chance to not consume gel\n" +
                "Fires a stream of exo flames and light that explodes into homing sparks\n" +
                "Right click to fire homing flares which stick to enemies and incinerate them");
        }

        public override void SetDefaults()
        {
            item.damage = 230;
            item.ranged = true;
            item.width = 84;
            item.height = 30;
            item.useTime = 2;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.UseSound = SoundID.Item34;
            item.autoReuse = true;
            item.shootSpeed = 18f;
            item.useAmmo = AmmoID.Gel;

            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.value = CalamityGlobalItem.RarityVioletBuyPrice;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.shoot = ModContent.ProjectileType<ExoFlareCluster>();
                item.useTime = item.useAnimation = 27;
            }
            else
            {
                item.useTime = 2;
                item.useAnimation = 10;
                item.shoot = ModContent.ProjectileType<ExoFire>();
            }
            return base.CanUseItem(player);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // Alt fire: Shoot an Exo Flare Cluster.
            if (player.altFunctionUse == 2)
            {
                int projID = ModContent.ProjectileType<ExoFlareCluster>();
                Vector2 velocity = new Vector2(speedX, speedY);
                position += velocity.ToRotation().ToRotationVector2() * 80f;
                Projectile.NewProjectile(position, velocity.SafeNormalize(Vector2.Zero) * AltFireShootSpeed, projID, damage, knockBack, player.whoAmI);
                return false;
            }


            // Left click: Exo Fire, with a chance of Exo Light Bombs.
            for (int i = 0; i < 2; i++)
            {
                Vector2 velocity = new Vector2(speedX, speedY).RotatedByRandom(0.05f) * Main.rand.NextFloat(0.97f, 1.03f);
                Projectile.NewProjectile(position, velocity, type, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            if (player.itemAnimation == 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 velocity = new Vector2(speedX, speedY) * 0.7f;
                    position += velocity.ToRotation().ToRotationVector2() * 64f;
                    int yDirection = (i == 0).ToDirectionInt();
                    velocity = velocity.RotatedBy(0.2f * yDirection);
                    Projectile lightBomb = Projectile.NewProjectileDirect(position, velocity, ModContent.ProjectileType<ExoLight>(), damage, knockBack, player.whoAmI);

                    lightBomb.localAI[1] = yDirection;
                    lightBomb.netUpdate = true;
                }
            }
            return false;
        }

        public override bool ConsumeAmmo(Player player) => Main.rand.NextFloat() > AmmoNotConsumeChance;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ElementalEruption>());
            recipe.AddIngredient(ModContent.ItemType<CleansingBlaze>());
            recipe.AddIngredient(ModContent.ItemType<HalleysInferno>());
            recipe.AddIngredient(ModContent.ItemType<MiracleMatter>());
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
