using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class ResurrectionButterfly : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Resurrection Butterfly");
            Tooltip.SetDefault("Remembering the melancholy of human existence\n" +
                "Even ghosts stray from the path of righteousness\n" +
                "Summons a pair of butterflies to fight for you");
        }

        public override void SetDefaults()
        {
            item.damage = 65;
            item.mana = 10;
            item.width = 46;
            item.height = 46;
            item.useTime = item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 1f;

            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.rare = ItemRarityID.Yellow;
            item.Calamity().donorItem = true;

            item.UseSound = SoundID.Item44;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<PinkButterfly>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int i = Main.myPlayer;
            float num72 = item.shootSpeed;
            float num74 = knockBack;
            num74 = player.GetWeaponKnockback(item, num74);
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
            spinningpoint = spinningpoint.RotatedBy(1.5707963705062866, default);
            Projectile.NewProjectile(vector2.X + spinningpoint.X, vector2.Y + spinningpoint.Y, spinningpoint.X, spinningpoint.Y, ModContent.ProjectileType<PinkButterfly>(), damage, num74, i, 0f, 0f);
            spinningpoint = spinningpoint.RotatedBy(-3.1415927410125732, default);
            Projectile.NewProjectile(vector2.X + spinningpoint.X, vector2.Y + spinningpoint.Y, spinningpoint.X, spinningpoint.Y, ModContent.ProjectileType<PurpleButterfly>(), damage, num74, i, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Silk, 40);
            r.AddIngredient(ItemID.Ectoplasm, 20);
            r.AddIngredient(ModContent.ItemType<BarofLife>(), 5);
            r.AddIngredient(ItemID.ButterflyDust, 2);
            r.AddTile(TileID.Loom);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
