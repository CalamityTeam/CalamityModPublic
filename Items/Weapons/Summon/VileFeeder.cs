using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class VileFeeder : ModItem
    {
        public static int BaseDamage = 9;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vile Feeder");
            Tooltip.SetDefault("Summons a baby eater of souls to protect you");
        }

        public override void SetDefaults()
        {
            item.damage = BaseDamage;
            item.mana = 10;
            item.width = 66;
            item.height = 70;
            item.useTime = item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 0.5f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item2;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<VileFeederSummon>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                int i = Main.myPlayer;
                float num72 = item.shootSpeed;
                float knockback = knockBack;
                knockback = player.GetWeaponKnockback(item, knockback);
                player.itemTime = item.useTime;
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
                    num80 = num72;
                }
                else
                {
                    num80 = num72 / num80;
                }
                num78 *= num80;
                num79 *= num80;
                vector2.X = (float)Main.mouseX + Main.screenPosition.X;
                vector2.Y = (float)Main.mouseY + Main.screenPosition.Y;
                Vector2 spinningpoint = new Vector2(num78, num79);
                spinningpoint = spinningpoint.RotatedBy(MathHelper.PiOver2, default);
                Projectile.NewProjectile(vector2.X + spinningpoint.X, vector2.Y + spinningpoint.Y, spinningpoint.X, spinningpoint.Y, type, damage, knockback, i, 0f, 0f);
            }
            return false;
        }
        
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DemoniteBar, 5);
            recipe.AddIngredient(ItemID.ShadowScale, 9);
            recipe.AddIngredient(ItemID.Ebonwood, 20);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
