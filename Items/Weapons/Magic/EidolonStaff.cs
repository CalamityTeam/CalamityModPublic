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
    public class EidolonStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eidolon Staff");
            Tooltip.SetDefault("The power of an ancient cultist resonates within this staff\n" +
                "Fires a spread of ancient light and has a chance to fire a spinning ice cluster");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
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
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Starblast>();
            Item.shootSpeed = 12f;
        }

        
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num72 = Item.shootSpeed;
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
            int stardustAmt = 5;
            float num132 = num78;
            float num133 = num79;
            num80 = (float)Math.Sqrt((double)(num132 * num132 + num133 * num133));
            num80 = num72 / num80;
            num132 *= num80;
            num133 *= num80;
            float x2 = vector2.X;
            float y2 = vector2.Y;
            Projectile.NewProjectile(source, x2, y2, num132, num133, ModContent.ProjectileType<IceCluster>(), damage, knockback, player.whoAmI);
            for (int i = 0; i < stardustAmt; i++)
            {
                num132 = num78;
                num133 = num79;
                float num134 = 0.05f * (float)i;
                num132 += (float)Main.rand.Next(-90, 91) * num134;
                num133 += (float)Main.rand.Next(-90, 91) * num134;
                num80 = (float)Math.Sqrt((double)(num132 * num132 + num133 * num133));
                num80 = num72 / num80;
                num132 *= num80;
                num133 *= num80;
                x2 = vector2.X;
                y2 = vector2.Y;
                Projectile.NewProjectile(source, x2, y2, num132, num133, ModContent.ProjectileType<Starblast>(), damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
}
