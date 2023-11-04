using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AstralachneaStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 19;
            Item.width = 52;
            Item.height = 52;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item46;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AstralachneaFang>();
            Item.shootSpeed = 13f;
        }
                
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            int j = Main.myPlayer;
            float fangSpeed = Item.shootSpeed;
            float mouseXDist = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
            float mouseYDist = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
            float mouseDistance = (float)Math.Sqrt((double)(mouseXDist * mouseXDist + mouseYDist * mouseYDist));
            int spikeAmount = 4;
            if (Main.rand.NextBool(3))
            {
                spikeAmount++;
            }
            if (Main.rand.NextBool(4))
            {
                spikeAmount++;
            }
            if (Main.rand.NextBool(5))
            {
                spikeAmount += 2;
            }
            for (int i = 0; i < spikeAmount; i++)
            {
                float fangSpawnX = mouseXDist;
                float fangSpawnY = mouseYDist;
                float offsetDampener = 0.05f * (float)i;
                fangSpawnX += (float)Main.rand.Next(-400, 400) * offsetDampener;
                fangSpawnY += (float)Main.rand.Next(-400, 400) * offsetDampener;
                float fangDistance = (float)Math.Sqrt((double)(fangSpawnX * fangSpawnX + fangSpawnY * fangSpawnY));
                fangDistance = fangSpeed / fangDistance;
                fangSpawnX *= fangDistance;
                fangSpawnY *= fangDistance;
                float x2 = realPlayerPos.X;
                float y2 = realPlayerPos.Y;
                Projectile.NewProjectile(source, x2, y2, fangSpawnX, fangSpawnY, ModContent.ProjectileType<AstralachneaFang>(), damage, knockback, i, 0f, 0f);
            }
            return false;
        }
    }
}
