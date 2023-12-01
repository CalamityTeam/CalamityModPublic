using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Avalanche : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 64;
            Item.scale = 1.5f;
            Item.damage = 100;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 35;
            Item.useTime = 35;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = player.GetSource_ItemUse(Item);
            int totalProjectiles = 4;
            float radians = MathHelper.TwoPi / totalProjectiles;
            int type = ModContent.ProjectileType<IceBombFriendly>();
            int bombDamage = player.CalcIntDamage<MeleeDamageClass>(Item.damage);
            float velocity = 4f;
            double angleA = radians * 0.5;
            double angleB = MathHelper.ToRadians(90f) - angleA;
            float velocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
            Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity) : new Vector2(-velocityX, -velocity);
            for (int k = 0; k < totalProjectiles; k++)
            {
                Vector2 projRotation = spinningPoint.RotatedBy(radians * k);
                Projectile.NewProjectile(source, target.Center, projRotation, type, bombDamage, hit.Knockback, Main.myPlayer);
            }
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            var source = player.GetSource_ItemUse(Item);
            int totalProjectiles = 4;
            float radians = MathHelper.TwoPi / totalProjectiles;
            int type = ModContent.ProjectileType<IceBombFriendly>();
            int bombDamage = player.CalcIntDamage<MeleeDamageClass>(Item.damage);
            float velocity = 4f;
            double angleA = radians * 0.5;
            double angleB = MathHelper.ToRadians(90f) - angleA;
            float velocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
            Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity) : new Vector2(-velocityX, -velocity);
            for (int k = 0; k < totalProjectiles; k++)
            {
                Vector2 projRotation = spinningPoint.RotatedBy(radians * k);
                Projectile.NewProjectile(source, target.Center, projRotation, type, bombDamage, 0f, Main.myPlayer);
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int iceDust = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 67, (float)(player.direction * 2), 0f, 150, default, 1.5f);
                Main.dust[iceDust].velocity *= 0.2f;
            }
        }
    }
}
