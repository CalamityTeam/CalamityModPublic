using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AuguroftheElements : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Augur of the Elements");
            Tooltip.SetDefault("Casts a burst of elemental tentacles to spear your enemies");
        }

        public override void SetDefaults()
        {
            item.damage = 162;
            item.magic = true;
            item.mana = 6;
            item.width = 28;
            item.crit = 3;
            item.height = 30;
            item.useTime = 2;
            item.reuseDelay = 5;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5.5f;
			item.value = CalamityGlobalItem.Rarity11BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.UseSound = SoundID.Item103;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ElementTentacle>();
            item.shootSpeed = 30f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<EldritchTome>());
            recipe.AddIngredient(ModContent.ItemType<TomeofFates>());
            recipe.AddIngredient(ItemID.ShadowFlameHexDoll);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.Bookcases);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            Vector2 value2 = new Vector2(num78, num79);
            value2.Normalize();
            Vector2 value3 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
            value3.Normalize();
            value2 = value2 * 6f + value3;
            value2.Normalize();
            value2 *= item.shootSpeed;
            float num91 = (float)Main.rand.Next(10, 50) * 0.001f;
            if (Main.rand.NextBool(2))
            {
                num91 *= -1f;
            }
            float num92 = (float)Main.rand.Next(10, 50) * 0.001f;
            if (Main.rand.NextBool(2))
            {
                num92 *= -1f;
            }
            Projectile.NewProjectile(vector2.X, vector2.Y, value2.X, value2.Y, type, damage, knockBack, player.whoAmI, num92, num91);
            return false;
        }
    }
}
