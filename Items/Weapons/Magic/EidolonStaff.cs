using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    [LegacyName("StardustStaff")]
    public class EidolonStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
                       Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 180;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 56;
            Item.height = 56;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Starblast>();
            Item.shootSpeed = 12f;
        }

        
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float projSpeed = Item.shootSpeed;
            float mouseXDist = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
            float mouseYDist = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
            float mouseDistance = (float)Math.Sqrt((double)(mouseXDist * mouseXDist + mouseYDist * mouseYDist));
            int stardustAmt = 5;
            float mouseXCopy = mouseXDist;
            float mouseYCopy = mouseYDist;
            mouseDistance = (float)Math.Sqrt((double)(mouseXCopy * mouseXCopy + mouseYCopy * mouseYCopy));
            mouseDistance = projSpeed / mouseDistance;
            mouseXCopy *= mouseDistance;
            mouseYCopy *= mouseDistance;
            float x2 = realPlayerPos.X;
            float y2 = realPlayerPos.Y;
            Projectile.NewProjectile(source, x2, y2, mouseXCopy, mouseYCopy, ModContent.ProjectileType<IceCluster>(), damage, knockback, player.whoAmI);
            for (int i = 0; i < stardustAmt; i++)
            {
                mouseXCopy = mouseXDist;
                mouseYCopy = mouseYDist;
                float randOffsetDampener = 0.05f * (float)i;
                mouseXCopy += (float)Main.rand.Next(-90, 91) * randOffsetDampener;
                mouseYCopy += (float)Main.rand.Next(-90, 91) * randOffsetDampener;
                mouseDistance = (float)Math.Sqrt((double)(mouseXCopy * mouseXCopy + mouseYCopy * mouseYCopy));
                mouseDistance = projSpeed / mouseDistance;
                mouseXCopy *= mouseDistance;
                mouseYCopy *= mouseDistance;
                x2 = realPlayerPos.X;
                y2 = realPlayerPos.Y;
                Projectile.NewProjectile(source, x2, y2, mouseXCopy, mouseYCopy, ModContent.ProjectileType<Starblast>(), damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
}
