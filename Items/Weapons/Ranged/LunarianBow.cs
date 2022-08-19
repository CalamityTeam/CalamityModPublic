using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class LunarianBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lunarian Bow");
            Tooltip.SetDefault("Fires two arrows at once\n" +
                "Converts wooden arrows into sliding energy bolts");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 19;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 32;
            Item.height = 62;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(0, 12, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item75;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LunarBolt>();
            Item.shootSpeed = 8f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo spawnSource, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
            float piOver10 = MathHelper.Pi * 0.1f;
            int projAmt = 2;

            velocity.Normalize();
            velocity *= 15f;
            bool canHit = Collision.CanHit(source, 0, 0, source + velocity, 0, 0);
            for (int i = 0; i < projAmt; i++)
            {
                float offsetAmt = i - (projAmt - 1f) / 2f;
                Vector2 offset = velocity.RotatedBy(piOver10 * offsetAmt, default);
                if (!canHit)
                    offset -= velocity;

                if (CalamityUtils.CheckWoodenAmmo(type, player))
                    Projectile.NewProjectile(spawnSource, source + offset, velocity, ModContent.ProjectileType<LunarBolt>(), damage, knockback, player.whoAmI);
                else
                {
                    int proj = Projectile.NewProjectile(spawnSource, source + offset, velocity, type, damage, knockback, player.whoAmI);
                    Main.projectile[proj].noDropItem = true;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DemonBow).
                AddIngredient(ItemID.BeesKnees).
                AddIngredient(ItemID.MoltenFury).
                AddIngredient<PurifiedGel>(10).
                AddTile(TileID.DemonAltar).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.TendonBow).
                AddIngredient(ItemID.BeesKnees).
                AddIngredient(ItemID.MoltenFury).
                AddIngredient<PurifiedGel>(10).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
