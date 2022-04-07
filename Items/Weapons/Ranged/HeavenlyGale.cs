using Terraria.DataStructures;
using Terraria.DataStructures;
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
            Item.damage = 198;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 44;
            Item.height = 58;
            Item.useTime = 15;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 vel = velocity;
            float piOver10 = MathHelper.Pi * 0.1f;

            // This is not related to speed, but rather placing the arrows correctly along the large bow.
            Vector2 baseOffset = velocity;
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
                    Projectile.NewProjectile(source + offset, vel, thisArrowType, finalDamage, knockback, player.whoAmI);
                }
                else
                {
                    int normalArrowDamage = (int)(damage * NormalArrowDamageMult);
                    int proj = Projectile.NewProjectile(source + offset, vel, type, normalArrowDamage, knockback, player.whoAmI);
                    Main.projectile[proj].noDropItem = true;
                }
            }
            return false;
        }

        public override bool CanConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 66)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemType<PlanetaryAnnihilation>()).AddIngredient(ItemType<Alluvion>()).AddIngredient(ItemType<AstrealDefeat>()).AddIngredient(ItemType<ClockworkBow>()).AddIngredient(ItemType<Galeforce>()).AddIngredient(ItemType<TheBallista>()).AddIngredient(ItemType<MiracleMatter>()).AddTile(TileType<DraedonsForge>()).Register();
        }
    }
}
