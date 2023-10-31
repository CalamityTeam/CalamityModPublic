using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AsteroidStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 145;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 18;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6.75f;
            Item.UseSound = SoundID.Item88;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Asteroid>();
            Item.shootSpeed = 20f;

            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float meteorSpeed = Item.shootSpeed;
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float meteorSpawnXPos = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
            float meteorSpawnYPos = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
            if (player.gravDir == -1f)
            {
                meteorSpawnYPos = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - realPlayerPos.Y;
            }
            float meteorSpawnDist = (float)Math.Sqrt((double)(meteorSpawnXPos * meteorSpawnXPos + meteorSpawnYPos * meteorSpawnYPos));
            if ((float.IsNaN(meteorSpawnXPos) && float.IsNaN(meteorSpawnYPos)) || (meteorSpawnXPos == 0f && meteorSpawnYPos == 0f))
            {
                meteorSpawnXPos = (float)player.direction;
                meteorSpawnYPos = 0f;
                meteorSpawnDist = meteorSpeed;
            }
            else
            {
                meteorSpawnDist = meteorSpeed / meteorSpawnDist;
            }

            int asteroidAmt = 3;
            for (int i = 0; i < asteroidAmt; i++)
            {
                realPlayerPos = new Vector2(player.Center.X + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                realPlayerPos.X = (realPlayerPos.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                realPlayerPos.Y -= (float)(100 * i);
                meteorSpawnXPos = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X + (float)Main.rand.Next(-40, 41) * 0.03f;
                meteorSpawnYPos = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
                if (meteorSpawnYPos < 0f)
                {
                    meteorSpawnYPos *= -1f;
                }
                if (meteorSpawnYPos < 20f)
                {
                    meteorSpawnYPos = 20f;
                }
                meteorSpawnDist = (float)Math.Sqrt((double)(meteorSpawnXPos * meteorSpawnXPos + meteorSpawnYPos * meteorSpawnYPos));
                meteorSpawnDist = meteorSpeed / meteorSpawnDist;
                meteorSpawnXPos *= meteorSpawnDist;
                meteorSpawnYPos *= meteorSpawnDist;
                float meteorSpawnXOffset = meteorSpawnXPos;
                float meteorSpawnYOffset = meteorSpawnYPos + (float)Main.rand.Next(-40, 41) * 0.02f;
                Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, meteorSpawnXOffset * 0.75f, meteorSpawnYOffset * 0.75f, type, damage, knockback, player.whoAmI, 0f, 0.5f + (float)Main.rand.NextDouble() * 0.3f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MeteorStaff).
                AddIngredient(ItemID.LunarBar, 5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
