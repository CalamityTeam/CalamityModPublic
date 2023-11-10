using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Serpentine : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.damage = 14;
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
            float projSpeed = Item.shootSpeed;
            player.itemTime = Item.useTime;
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 projRotation = Vector2.UnitX.RotatedBy((double)player.fullRotation, default);
            Vector2 projSpawnPos = Main.MouseWorld - realPlayerPos;
            float velX = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
            float velY = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
            if (player.gravDir == -1f)
            {
                velY = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - realPlayerPos.Y;
            }
            float dist = (float)Math.Sqrt((double)(velX * velX + velY * velY));
            if ((float.IsNaN(velX) && float.IsNaN(velY)) || (velX == 0f && velY == 0f))
            {
                velX = (float)player.direction;
                velY = 0f;
                dist = projSpeed;
            }
            else
            {
                dist = projSpeed / dist;
            }

            float projDirection = Vector2.Dot(projRotation, projSpawnPos);
            if (projDirection > 0f)
            {
                player.ChangeDir(1);
            }
            else
            {
                player.ChangeDir(-1);
            }
            //velX = 0f;
            //velY = 0f;
            //realPlayerPos.X = (float)Main.mouseX + Main.screenPosition.X;
            //realPlayerPos.Y = (float)Main.mouseY + Main.screenPosition.Y;
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
