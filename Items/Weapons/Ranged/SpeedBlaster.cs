using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("PaintballBlaster")]
    public class SpeedBlaster : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 34;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 54;
            Item.height = 26;
            Item.useTime = 4;
            Item.useAnimation = 24;
            Item.reuseDelay = 9;
            Item.useLimitPerAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
            Item.shoot = ProjectileID.PainterPaintball;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float num72 = Item.shootSpeed;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
            }
            float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = (float)player.direction;
                num79 = 0f;
            }
            else
            {
                num80 = num72 / num80;
            }
            float num208 = num78;
            float num209 = num79;
            num208 += (float)Main.rand.Next(-1, 2) * 0.5f;
            num209 += (float)Main.rand.Next(-1, 2) * 0.5f;
            if (Collision.CanHitLine(player.Center, 0, 0, vector2 + new Vector2(num208, num209) * 2f, 0, 0))
            {
                vector2 += new Vector2(num208, num209);
            }
            Projectile.NewProjectile(source, position.X, position.Y - player.gravDir * 4f, num208, num209, ProjectileID.PainterPaintball, damage, knockback, player.whoAmI, 0f, (float)Main.rand.Next(12) / 6f);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PainterPaintballGun).
                AddIngredient(ItemID.SoulofSight, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
