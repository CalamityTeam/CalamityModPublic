using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class BloodfireArrowProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Items/Ammo/BloodfireArrow";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) - MathHelper.ToRadians(90);

            if (Projectile.timeLeft % 15 == 0)
            {
                for (int l = 0; l < 12; l++)
                {
                    Vector2 rotate = Vector2.UnitX * (float)-(float)Projectile.width / 2f;
                    rotate += -Vector2.UnitY.RotatedBy((double)((float)l * 3.14159274f / 6f), default) * new Vector2(8f, 16f);
                    rotate = rotate.RotatedBy((double)(Projectile.rotation - 1.57079637f), default);
                    int bloodDust = Dust.NewDust(Projectile.Center, 0, 0, (int)CalamityDusts.Brimstone, 0f, 0f, 160, default, 1f);
                    Main.dust[bloodDust].scale = 1.1f;
                    Main.dust[bloodDust].noGravity = true;
                    Main.dust[bloodDust].position = Projectile.Center + rotate;
                    Main.dust[bloodDust].velocity = Projectile.velocity * 0.1f;
                    Main.dust[bloodDust].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[bloodDust].position) * 1.25f;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int i = 0; i < 5; i++)
            {
                int brimDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                Main.dust[brimDust].velocity *= 0.5f;
                if (Main.rand.NextBool())
                {
                    Main.dust[brimDust].scale = 0.5f;
                    Main.dust[brimDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.canGhostHeal || Main.player[Projectile.owner].moonLeech)
                return;

            Player player = Main.player[Projectile.owner];

            int chance = 3;
            if (player.ActiveItem().type == ModContent.ItemType<TheStorm>())
                chance = 6;

            if (Main.rand.NextBool(chance))
            {
                player.statLife += 1;
                player.HealEffect(1);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}
