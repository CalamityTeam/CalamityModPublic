using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 28;
            Item.height = 32;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SerpentineHead>();
            Item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int owner = player.whoAmI;
            float num72 = Item.shootSpeed;
            player.itemTime = Item.useTime;
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
            int curr = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SerpentineHead>(), damage, knockback, owner);

            int prev = curr;
            curr = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SerpentineBody>(), damage, knockback, owner, (float)prev);

            prev = curr;
            curr = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SerpentineBody>(), damage, knockback, owner, (float)prev);

            prev = curr;
            curr = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SerpentineBody>(), damage, knockback, owner, (float)prev);

            prev = curr;
            curr = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SerpentineTail>(), damage, knockback, owner, (float)prev);
            Main.projectile[prev].localAI[1] = (float)curr;
            Main.projectile[prev].netUpdate = true;
            return false;
        }
    }
}
