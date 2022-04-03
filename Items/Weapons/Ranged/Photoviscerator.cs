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
            Item.damage = 230;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 84;
            Item.height = 30;
            Item.useTime = 2;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item34;
            Item.autoReuse = true;
            Item.shootSpeed = 18f;
            Item.useAmmo = AmmoID.Gel;

            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shoot = ModContent.ProjectileType<ExoFlareCluster>();
                Item.useTime = Item.useAnimation = 27;
            }
            else
            {
                Item.useTime = 2;
                Item.useAnimation = 10;
                Item.shoot = ModContent.ProjectileType<ExoFire>();
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
            CreateRecipe(1).AddIngredient(ModContent.ItemType<ElementalEruption>()).AddIngredient(ModContent.ItemType<CleansingBlaze>()).AddIngredient(ModContent.ItemType<HalleysInferno>()).AddIngredient(ModContent.ItemType<MiracleMatter>()).AddTile(ModContent.TileType<DraedonsForge>()).Register();
        }
    }
}
