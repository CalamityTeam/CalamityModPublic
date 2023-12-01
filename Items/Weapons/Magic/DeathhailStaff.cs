using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class DeathhailStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
                       Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 328;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 16;
            Item.width = 80;
            Item.height = 84;
            Item.useTime = 11;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.UseSound = SoundID.Item12;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DeathhailBeam>();
            Item.shootSpeed = 18f;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Magic/DeathhailStaffGlow").Value);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int j = Main.myPlayer;
            float beamSpeed = Item.shootSpeed;
            float beamKnockback = knockback;
            beamKnockback = player.GetWeaponKnockback(Item, beamKnockback);
            player.itemTime = Item.useTime;
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
                mouseDistance = beamSpeed;
            }
            else
            {
                mouseDistance = beamSpeed / mouseDistance;
            }

            int numLasers = 2;
            for (int i = 0; i < numLasers; i++)
            {
                realPlayerPos = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(91) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                realPlayerPos.X = (realPlayerPos.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                realPlayerPos.Y -= (float)(100 * i);
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
                mouseDistance = beamSpeed / mouseDistance;
                mouseXDist *= mouseDistance;
                mouseYDist *= mouseDistance;
                float speedX4 = mouseXDist + (float)Main.rand.Next(-50, 51) * 0.02f;
                float speedY5 = mouseYDist + (float)Main.rand.Next(-50, 51) * 0.02f;
                Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, type, damage, beamKnockback, i, 0f, 0f);
            }
            return false;
        }
    }
}
