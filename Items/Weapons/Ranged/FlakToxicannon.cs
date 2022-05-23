using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class FlakToxicannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flak Toxicannon");
            Tooltip.SetDefault("Fires angled shots in the direction of the cursor\n" +
                               "Can only be shot in a cone direction above the player\n" +
                               "High IQ required");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 88;
            Item.height = 28;
            Item.useTime = Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTurn = false;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item109;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ToxicannonShot>();
            Item.shootSpeed = 9f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float angle = velocity.ToRotation() + MathHelper.PiOver2;
            if (angle <= -MathHelper.PiOver4 || angle >= MathHelper.PiOver4)
                return false;
            angle -= MathHelper.PiOver2;



            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ToxicannonShot>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}
