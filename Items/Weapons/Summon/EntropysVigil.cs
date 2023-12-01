using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    [LegacyName("BlightedEyeStaff")]
    public class EntropysVigil : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.mana = 10;
            Item.width = 52;
            Item.height = 52;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item82;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Calamitamini>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
            int p = Projectile.NewProjectile(source, realPlayerPos.X + spinningpoint.X, realPlayerPos.Y + spinningpoint.Y, spinningpoint.X, spinningpoint.Y, ModContent.ProjectileType<Calamitamini>(), damage, knockback, i, 0f, 1f);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            spinningpoint = spinningpoint.RotatedBy(-3.1415927410125732, default);
            p = Projectile.NewProjectile(source, realPlayerPos.X + spinningpoint.X, realPlayerPos.Y + spinningpoint.Y, spinningpoint.X, spinningpoint.Y, ModContent.ProjectileType<Catastromini>(), damage, knockback, i, 0f, 0f);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            spinningpoint = spinningpoint.RotatedBy(-5.1415927410125732, default);
            p = Projectile.NewProjectile(source, realPlayerPos.X + spinningpoint.X, realPlayerPos.Y + spinningpoint.Y, spinningpoint.X, spinningpoint.Y, ModContent.ProjectileType<Cataclymini>(), damage, knockback, i, 0f, 1f);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            return false;
        }
    }
}
