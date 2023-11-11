using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Vortexpopper : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 22;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.25f;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item95;
            Item.autoReuse = true;
            Item.shootSpeed = 50f;
            Item.shoot = ProjectileID.Xenopopper;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float bulletSpeed = Item.shootSpeed;
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float mouseXDist = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
            float mouseYDist = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
            if (player.gravDir == -1f)
            {
                mouseYDist = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - realPlayerPos.Y;
            }
            float mouseDistance = (float)Math.Sqrt((double)(mouseXDist * mouseXDist + mouseYDist * mouseYDist));
            if ((float.IsNaN(mouseXDist) && float.IsNaN(mouseYDist)) || (mouseXDist == 0f && mouseYDist == 0f))
            {
                mouseXDist = (float)player.direction;
                mouseYDist = 0f;
                mouseDistance = bulletSpeed;
            }
            else
            {
                mouseDistance = bulletSpeed / mouseDistance;
            }
            mouseXDist *= mouseDistance;
            mouseYDist *= mouseDistance;
            Vector2 bulletVel = Vector2.Normalize(new Vector2(mouseXDist, mouseYDist)) * 40f * Item.scale;
            if (Collision.CanHit(realPlayerPos, 0, 0, realPlayerPos + bulletVel, 0, 0))
            {
            }
            float ai = new Vector2(mouseXDist, mouseYDist).ToRotation();
            float twoThirdsPi = 2.09439516f;
            for (int i = 0; i < 6; i++)
            {
                float randVelMult = (float)Main.rand.NextDouble() * 0.2f + 0.05f;
                Vector2 bulletSpawnVelocity = new Vector2(mouseXDist, mouseYDist).RotatedBy((double)(twoThirdsPi * (float)Main.rand.NextDouble() - twoThirdsPi / 2f), default) * randVelMult;
                int initialBullet = Projectile.NewProjectile(source, position.X, position.Y, bulletSpawnVelocity.X, bulletSpawnVelocity.Y, ProjectileID.Xenopopper, damage, knockback, player.whoAmI, ai, 0f);
                Main.projectile[initialBullet].localAI[0] = (float)type;
                Main.projectile[initialBullet].localAI[1] = 12f;
            }
            for (int j = 0; j < 6; j++)
            {
                realPlayerPos = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                realPlayerPos.X = (realPlayerPos.X + player.Center.X) / 2f + (float)Main.rand.Next(-800, 801);
                realPlayerPos.Y -= (float)(100 * j);
                mouseXDist = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
                mouseYDist = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
                if (mouseYDist < 0f)
                {
                    mouseYDist *= -1f;
                }
                if (mouseYDist < 20f)
                {
                    mouseYDist = 20f;
                }
                mouseDistance = (float)Math.Sqrt((double)(mouseXDist * mouseXDist + mouseYDist * mouseYDist));
                mouseDistance = bulletSpeed / mouseDistance;
                mouseXDist *= mouseDistance;
                mouseYDist *= mouseDistance;
                float speedX4 = mouseXDist + (float)Main.rand.Next(-1000, 1001) * 0.02f;
                float speedY5 = mouseYDist + (float)Main.rand.Next(-1000, 1001) * 0.02f;
                int extraBullet = Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, ProjectileID.Xenopopper, damage, knockback, player.whoAmI, ai, 0f);
                Main.projectile[extraBullet].localAI[0] = (float)type;
                Main.projectile[extraBullet].localAI[1] = 12f;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Xenopopper).
                AddIngredient(ItemID.FragmentVortex, 12).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
