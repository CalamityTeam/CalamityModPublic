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
        }

        public override void SetDefaults()
        {
            item.damage = 32;
            item.ranged = true;
            item.width = 32;
            item.height = 62;
            item.useTime = 22;
            item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.rare = ItemRarityID.LightRed;
            item.UseSound = SoundID.Item75;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<LunarBolt>();
            item.shootSpeed = 8f;
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
            float piOver10 = MathHelper.Pi * 0.1f;
            int projAmt = 2;
            Vector2 velocity = new Vector2(speedX, speedY);
            velocity.Normalize();
            velocity *= 15f;
            bool canHit = Collision.CanHit(source, 0, 0, source + velocity, 0, 0);
            for (int i = 0; i < projAmt; i++)
            {
                float offsetAmt = i - (projAmt - 1f) / 2f;
                Vector2 offset = velocity.RotatedBy(piOver10 * offsetAmt, default);
                if (!canHit)
                    offset -= velocity;

                if (type == ProjectileID.WoodenArrowFriendly)
                    Projectile.NewProjectile(source + offset, new Vector2(speedX, speedY), ModContent.ProjectileType<LunarBolt>(), damage, knockBack, player.whoAmI);
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
            recipe.AddIngredient(ItemID.DemonBow);
            recipe.AddIngredient(ItemID.BeesKnees);
            recipe.AddIngredient(ItemID.MoltenFury);
            recipe.AddIngredient(ModContent.ItemType<PurifiedGel>(), 10);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TendonBow);
            recipe.AddIngredient(ItemID.BeesKnees);
            recipe.AddIngredient(ItemID.MoltenFury);
            recipe.AddIngredient(ModContent.ItemType<PurifiedGel>(), 10);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
