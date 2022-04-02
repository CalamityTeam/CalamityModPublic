using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class CosmicBolter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Bolter");
            Tooltip.SetDefault("Fires three arrows at once\n" +
                "Converts wooden arrows into sliding energy bolts");
        }

        public override void SetDefaults()
        {
            item.damage = 39;
            item.ranged = true;
            item.width = 40;
            item.height = 76;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2.75f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.UseSound = SoundID.Item75;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<LunarBolt2>();
            item.shootSpeed = 10f;
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
            float piOver10 = MathHelper.Pi * 0.1f;
            int projAmt = 3;
            Vector2 velocity = new Vector2(speedX, speedY);
            velocity.Normalize();
            velocity *= 30f;
            bool canHit = Collision.CanHit(source, 0, 0, source + velocity, 0, 0);
            for (int i = 0; i < projAmt; i++)
            {
                float offsetAmt = i - (projAmt - 1f) / 2f;
                Vector2 offset = velocity.RotatedBy(piOver10 * offsetAmt, default);
                if (!canHit)
                    offset -= velocity;

                if (type == ProjectileID.WoodenArrowFriendly)
                    Projectile.NewProjectile(source + offset, new Vector2(speedX, speedY), ModContent.ProjectileType<LunarBolt2>(), damage, knockBack, player.whoAmI);
                else
                {
                    int proj = Projectile.NewProjectile(source + offset, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                    Main.projectile[proj].noDropItem = true;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<LunarianBow>());
            recipe.AddIngredient(ModContent.ItemType<LivingShard>(), 5);
            recipe.AddIngredient(ItemID.HallowedBar, 5);
            recipe.AddIngredient(ItemID.SoulofSight, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
