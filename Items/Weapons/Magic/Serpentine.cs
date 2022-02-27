using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Serpentine : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Serpentine");
            Tooltip.SetDefault("Casts a serpent that follows the mouse cursor");
        }

        public override void SetDefaults()
        {
            item.damage = 10;
            item.magic = true;
            item.mana = 20;
            item.width = 28;
            item.height = 32;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SerpentineHead>();
            item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int owner = player.whoAmI;
            float num72 = item.shootSpeed;
            player.itemTime = item.useTime;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 value = Vector2.UnitX.RotatedBy((double)player.fullRotation, default);
            Vector2 vector3 = Main.MouseWorld - vector2;
            float velX = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float velY = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            if (player.gravDir == -1f)
            {
                velY = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
            }
            float dist = (float)Math.Sqrt((double)(velX * velX + velY * velY));
            if ((float.IsNaN(velX) && float.IsNaN(velY)) || (velX == 0f && velY == 0f))
            {
                velX = (float)player.direction;
                velY = 0f;
                dist = num72;
            }
            else
            {
                dist = num72 / dist;
            }

            float num77 = Vector2.Dot(value, vector3);
            if (num77 > 0f)
            {
                player.ChangeDir(1);
            }
            else
            {
                player.ChangeDir(-1);
            }
            //velX = 0f;
            //velY = 0f;
            //vector2.X = (float)Main.mouseX + Main.screenPosition.X;
            //vector2.Y = (float)Main.mouseY + Main.screenPosition.Y;
            int curr = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<SerpentineHead>(), damage, knockBack, owner);

            int prev = curr;
            curr = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<SerpentineBody>(), damage, knockBack, owner, (float)prev);

            prev = curr;
            curr = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<SerpentineBody>(), damage, knockBack, owner, (float)prev);

            prev = curr;
            curr = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<SerpentineBody>(), damage, knockBack, owner, (float)prev);

            prev = curr;
            curr = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<SerpentineTail>(), damage, knockBack, owner, (float)prev);
            Main.projectile[prev].localAI[1] = (float)curr;
            Main.projectile[prev].netUpdate = true;
            return false;
        }
    }
}
