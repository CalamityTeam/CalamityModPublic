using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class BloodBath : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<BurningBlood>()];
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 50;
            Item.damage = 24;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.useTime = 15;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.75f;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item21;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BloodBeam>();
            Item.shootSpeed = 9f;
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float bloodSpeed = Item.shootSpeed;
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float mouseXPos = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
            float mouseYPos = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
            if (player.gravDir == -1f)
            {
                mouseYPos = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - realPlayerPos.Y;
            }
            float mouseDistance = (float)Math.Sqrt((double)(mouseXPos * mouseXPos + mouseYPos * mouseYPos));
            if ((float.IsNaN(mouseXPos) && float.IsNaN(mouseYPos)) || (mouseXPos == 0f && mouseYPos == 0f))
            {
                mouseXPos = (float)player.direction;
                mouseYPos = 0f;
                mouseDistance = bloodSpeed;
            }
            else
            {
                mouseDistance = bloodSpeed / mouseDistance;
            }

            int bloodAmt = 2;
            if (Main.rand.NextBool(3))
            {
                bloodAmt++;
            }
            for (int i = 0; i < bloodAmt; i++)
            {
                realPlayerPos = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                realPlayerPos.X = (realPlayerPos.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                realPlayerPos.Y -= (float)(100 * i);
                mouseXPos = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
                mouseYPos = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
                if (mouseYPos < 0f)
                {
                    mouseYPos *= -1f;
                }
                if (mouseYPos < 20f)
                {
                    mouseYPos = 20f;
                }
                mouseDistance = (float)Math.Sqrt((double)(mouseXPos * mouseXPos + mouseYPos * mouseYPos));
                mouseDistance = bloodSpeed / mouseDistance;
                mouseXPos *= mouseDistance;
                mouseYPos *= mouseDistance;
                float speedX4 = mouseXPos + (float)Main.rand.Next(-30, 31) * 0.02f;
                float speedY5 = mouseYPos + (float)Main.rand.Next(-30, 31) * 0.02f;
                Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, type, damage, knockback, player.whoAmI, 0f, (float)Main.rand.Next(15));
            }
            return false;
        }
    }
}
