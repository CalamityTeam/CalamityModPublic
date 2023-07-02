using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Seadragon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        private int shotType = 1;
        private bool rocket = false;
        private bool blast = false; //Melee Explosion every 9th shot out of 18

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 90;
            Item.height = 38;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.5f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float SpeedX = velocity.X + (float)Main.rand.Next(-10, 11) * 0.05f;
            float SpeedY = velocity.Y + (float)Main.rand.Next(-10, 11) * 0.05f;

            if (shotType > 18)
            {
                shotType = 1;
                rocket = true;
            }

            if (shotType > 8) //didn''t let me do "= 9"
            {
                if (shotType < 10)
                {
                blast = true;
                }
            }
            if (blast)
                Projectile.NewProjectile(source,
                position,
                velocity.RotatedByRandom(MathHelper.ToRadians(4.5f)) * 0,
                ModContent.ProjectileType<SeaDragonFlameburst>(),
                (int)(damage * 1.2f),
                knockback * 7f,
                player.whoAmI);
                blast = false;
            if (!rocket)
            {
                if (shotType % 2 == 1)
                    Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI, 0.0f, 0.0f);
                else
                    Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<ArcherfishShot>(), damage, knockback, player.whoAmI, 0f, 0f);
                
                if (shotType < 19)
                Projectile.NewProjectile(source,
                position,
                velocity.RotatedByRandom(MathHelper.ToRadians(5.5f)) * Main.rand.NextFloat(0.45f, 0.65f),
                ModContent.ProjectileType<ArcherfishRing>(),
                (int)(damage * 0.5f),
                knockback * 4f,
                player.whoAmI);

                shotType++;
            }
            else
            {
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<SeaDragonRocket>(), (int)(damage * 5), knockback, player.whoAmI, 0f, 0f);
                rocket = false;
            }

            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (shotType % 2 == 0)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Megalodon>().
                AddIngredient<Phantoplasm>(9).
                AddIngredient<ArmoredShell>(3).
                AddIngredient<SeaPrism>(10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
