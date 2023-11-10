using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class VileFeeder : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public static int BaseDamage = 14;

        public override void SetDefaults()
        {
            Item.damage = BaseDamage;
            Item.mana = 10;
            Item.width = 66;
            Item.height = 70;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 0.5f;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item2;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<VileFeederSummon>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                int i = Main.myPlayer;
                float projSpeed = Item.shootSpeed;
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
                    mouseDistance = projSpeed;
                }
                else
                {
                    mouseDistance = projSpeed / mouseDistance;
                }
                mouseXDist *= mouseDistance;
                mouseYDist *= mouseDistance;
                realPlayerPos.X = (float)Main.mouseX + Main.screenPosition.X;
                realPlayerPos.Y = (float)Main.mouseY + Main.screenPosition.Y;
                Vector2 spinningpoint = new Vector2(mouseXDist, mouseYDist);
                spinningpoint = spinningpoint.RotatedBy(MathHelper.PiOver2, default);
                int p = Projectile.NewProjectile(source, realPlayerPos.X + spinningpoint.X, realPlayerPos.Y + spinningpoint.Y, spinningpoint.X, spinningpoint.Y, type, damage, knockback, i, 0f, 0f);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DemoniteBar, 5).
                AddIngredient(ItemID.ShadowScale, 9).
                AddIngredient(ItemID.Ebonwood, 20).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
