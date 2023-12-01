using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class EtherealSubjugator : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetDefaults()
        {
            Item.damage = 200;
            Item.mana = 10;
            Item.width = 66;
            Item.height = 70;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.UseSound = SoundID.Item82;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PhantomGuy>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Summon/EtherealSubjugatorGlow").Value);
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
                spinningpoint = spinningpoint.RotatedBy(1.5707963705062866, default);
                int p = Projectile.NewProjectile(source, realPlayerPos.X + spinningpoint.X, realPlayerPos.Y + spinningpoint.Y, spinningpoint.X, spinningpoint.Y, type, damage, knockback, i, 0f, 1f);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }
    }
}
