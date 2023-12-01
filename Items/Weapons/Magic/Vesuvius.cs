using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Vesuvius : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.damage = 50;
            Item.mana = 7;
            Item.DamageType = DamageClass.Magic;
            Item.useAnimation = Item.useTime = 15;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item88;
            Item.autoReuse = true;
            Item.height = 62;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<AsteroidMolten>();

            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2)
                mult *= 1.5f;
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse != 2)
                return 1f;
            return 1.33f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                int meteorAmt = Main.rand.Next(2, 4);
                for (int i = 0; i < meteorAmt; ++i)
                {
                    float SpeedX = velocity.X + (float)Main.rand.Next(-30, 31) * 0.05f;
                    float SpeedY = velocity.Y + (float)Main.rand.Next(-30, 31) * 0.05f;
                    float ai0 = (float)Main.rand.Next(6);
                    Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI, ai0, 0.5f + (float)Main.rand.NextDouble() * 0.9f);
                }
                return false;
            }
            else
            {
                float meteorSpeed = Item.shootSpeed;
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
                    mouseDistance = meteorSpeed;
                }
                else
                {
                    mouseDistance = meteorSpeed / mouseDistance;
                }

                for (int i = 0; i < 4; i++)
                {
                    realPlayerPos = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                    realPlayerPos.X = (realPlayerPos.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                    realPlayerPos.Y -= (float)(100 * i);
                    mouseXDist = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X + (float)Main.rand.Next(-40, 41) * 0.03f;
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
                    mouseDistance = meteorSpeed / mouseDistance;
                    mouseXDist *= mouseDistance;
                    mouseYDist *= mouseDistance;
                    float meteorSpawnXOffset = mouseXDist;
                    float meteorSpawnYOffset = mouseYDist + (float)Main.rand.Next(-40, 41) * 0.02f;
                    float ai0 = (float)Main.rand.Next(6);
                    Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, meteorSpawnXOffset * 0.75f, meteorSpawnYOffset * 0.75f, type, damage, knockback, player.whoAmI, ai0, 0.5f + (float)Main.rand.NextDouble() * 0.9f); //0.3
                }
                return false;
            }
        }
    }
}
