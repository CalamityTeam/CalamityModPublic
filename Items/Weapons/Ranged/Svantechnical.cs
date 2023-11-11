using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Svantechnical : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 150;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 60;
            Item.height = 26;            
            Item.useTime = 2;
            Item.useAnimation = 24;
            Item.reuseDelay = 10;
            Item.useLimitPerAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;

            Item.value = CalamityGlobalItem.Rarity16BuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;

            Item.UseSound = SoundID.Item31;
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (Main.rand.Next(0, 100) < 70)
                return false;
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int i = Main.myPlayer;
            float sSpeed = Item.shootSpeed;
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
            }
            else
            {
                mouseDistance = sSpeed / mouseDistance;
            }
            float randXOffset = mouseXDist;
            float randYOffset = mouseYDist;
            randXOffset += (float)Main.rand.Next(-1, 2) * 0.5f;
            randYOffset += (float)Main.rand.Next(-1, 2) * 0.5f;
            if (Collision.CanHitLine(player.Center, 0, 0, realPlayerPos + new Vector2(randXOffset, randYOffset) * 2f, 0, 0))
            {
                realPlayerPos += new Vector2(randXOffset, randYOffset);
            }
            Projectile.NewProjectile(source, position.X, position.Y - player.gravDir * 4f, randXOffset, randYOffset, type, damage, knockback, i, 0f, (float)Main.rand.Next(12) / 6f);
            int bulletAmt = Main.rand.Next(2, 4);
            for (int index = 0; index < bulletAmt; ++index)
            {
                float SpeedX = velocity.X + (float)Main.rand.Next(-60, 61) * 0.05f;
                float SpeedY = velocity.Y + (float)Main.rand.Next(-60, 61) * 0.05f;
                Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SDFMG>().
                AddIngredient(ItemID.SoulofMight, 10).
                AddIngredient(ItemID.SoulofSight, 10).
                AddIngredient(ItemID.SoulofFright, 10).
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
