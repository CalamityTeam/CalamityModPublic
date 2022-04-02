using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class HeavenlyGale : ModItem
    {
        public const float NormalArrowDamageMult = 1.25f;
        private static int[] ExoArrows;
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heavenly Gale");
            Tooltip.SetDefault("Converts wooden arrows into barrages of 5 random exo arrows\n" +
                "Green exo arrows erupt into swirling tornadoes\n" +
                "Blue exo arrows burst into barrages of following arrows\n" +
                "Orange exo arrows explode into flames\n" +
                "Teal exo arrows pierce infinitely and ignore immunity frames\n" +
                $"Any non-wooden arrows used will deal {NormalArrowDamageMult}x damage\n" +
                "66% chance to not consume ammo");

            ExoArrows = new int[]
            {
                ProjectileType<TealExoArrow>(),
                ProjectileType<OrangeExoArrow>(),
                ProjectileType<GreenExoArrow>(),
                ProjectileType<BlueExoArrow>()
            };
        }

        public override void SetDefaults()
        {
            item.damage = 198;
            item.ranged = true;
            item.width = 44;
            item.height = 58;
            item.useTime = 15;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ProjectileID.WoodenArrowFriendly;
            item.shootSpeed = 12f;
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 vel = new Vector2(speedX, speedY);
            float piOver10 = MathHelper.Pi * 0.1f;

            // This is not related to speed, but rather placing the arrows correctly along the large bow.
            Vector2 baseOffset = new Vector2(speedX, speedY);
            baseOffset.Normalize();
            baseOffset *= 40f;

            bool againstWall = !Collision.CanHit(source, 0, 0, source + baseOffset, 0, 0);

            int numArrows = 5;
            for (int i = 0; i < numArrows; i++)
            {
                float offsetAmt = i - (numArrows - 1f) / 2f;
                Vector2 offset = baseOffset.RotatedBy(piOver10 * offsetAmt);
                if (againstWall)
                    offset -= baseOffset;

                if (type == ProjectileID.WoodenArrowFriendly)
                {
                    int thisArrowType = Main.rand.Next(ExoArrows);
                    // Teal exo arrows deal less damage.
                    float dmgMult = thisArrowType == ProjectileType<TealExoArrow>() ? 0.66f : 1f;
                    int finalDamage = (int)(damage * dmgMult);
                    Projectile.NewProjectile(source + offset, vel, thisArrowType, finalDamage, knockBack, player.whoAmI);
                }
                else
                {
                    int normalArrowDamage = (int)(damage * NormalArrowDamageMult);
                    int proj = Projectile.NewProjectile(source + offset, vel, type, normalArrowDamage, knockBack, player.whoAmI);
                    Main.projectile[proj].noDropItem = true;
                }
            }
            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 66)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<PlanetaryAnnihilation>());
            recipe.AddIngredient(ItemType<Alluvion>());
            recipe.AddIngredient(ItemType<AstrealDefeat>());
            recipe.AddIngredient(ItemType<ClockworkBow>());
            recipe.AddIngredient(ItemType<Galeforce>());
            recipe.AddIngredient(ItemType<TheBallista>());
            recipe.AddIngredient(ItemType<MiracleMatter>());
            recipe.AddTile(TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
