using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless.FiniteUse
{
    public class GrenadeRound : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grenade Round");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 90;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/BazookaRocket"), (int)projectile.position.X, (int)projectile.position.Y);
            }
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 30f)
                projectile.velocity.Y = projectile.velocity.Y + 0.1f;
            if (projectile.velocity.Y > 16f)
                projectile.velocity.Y = 16f;
            if (Math.Abs(projectile.velocity.X) >= 8f || Math.Abs(projectile.velocity.Y) >= 8f)
            {
                for (int num246 = 0; num246 < 2; num246++)
                {
                    float num247 = 0f;
                    float num248 = 0f;
                    if (num246 == 1)
                    {
                        num247 = projectile.velocity.X * 0.5f;
                        num248 = projectile.velocity.Y * 0.5f;
                    }
                    int num249 = Dust.NewDust(new Vector2(projectile.position.X + 3f + num247, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 6, 0f, 0f, 100, default, 1f);
                    Main.dust[num249].scale *= 2f + Main.rand.Next(10) * 0.1f;
                    Main.dust[num249].velocity *= 0.2f;
                    Main.dust[num249].noGravity = true;
                    num249 = Dust.NewDust(new Vector2(projectile.position.X + 3f + num247, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 31, 0f, 0f, 100, default, 0.5f);
                    Main.dust[num249].fadeIn = 1f + Main.rand.Next(5) * 0.1f;
                    Main.dust[num249].velocity *= 0.05f;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.HitSound != SoundID.NPCHit1 && target.HitSound != SoundID.NPCHit6 && target.HitSound != SoundID.NPCHit7 &&
                target.HitSound != SoundID.NPCHit8 && target.HitSound != SoundID.NPCHit9 && target.HitSound != SoundID.NPCHit12 &&
                target.HitSound != SoundID.NPCHit13 && target.HitSound != SoundID.NPCHit14 && target.HitSound != SoundID.NPCHit18 &&
                target.HitSound != SoundID.NPCHit19 && target.HitSound != SoundID.NPCHit20 && target.HitSound != SoundID.NPCHit21 &&
                target.HitSound != SoundID.NPCHit22 && target.HitSound != SoundID.NPCHit23 && target.HitSound != SoundID.NPCHit24 &&
                target.HitSound != SoundID.NPCHit25 && target.HitSound != SoundID.NPCHit26 && target.HitSound != SoundID.NPCHit27 &&
                target.HitSound != SoundID.NPCHit28 && target.HitSound != SoundID.NPCHit29 && target.HitSound != SoundID.NPCHit31 &&
                target.HitSound != SoundID.NPCHit32 && target.HitSound != SoundID.NPCHit33 && target.HitSound != SoundID.NPCHit35 &&
                target.HitSound != SoundID.NPCHit37 && target.HitSound != SoundID.NPCHit38 && target.HitSound != SoundID.NPCHit40 &&
                target.HitSound != SoundID.NPCHit43 && target.HitSound != SoundID.NPCHit44 && target.HitSound != SoundID.NPCHit45 &&
                target.HitSound != SoundID.NPCHit46 && target.HitSound != SoundID.NPCHit47 && target.HitSound != SoundID.NPCHit48 &&
                target.HitSound != SoundID.NPCHit50 && target.HitSound != SoundID.NPCHit51 && target.HitSound != SoundID.NPCHit55 &&
                target.HitSound != SoundID.NPCHit56 && target.HitSound != SoundID.NPCHit57)
            {
                damage += target.lifeMax / 20; //500 + 200 = 700 + (100000 / 20 = 5000) = 5700 * 2 (explosion) = 11400 = 11.4% of boss HP
            }
            if (damage > target.lifeMax / 12 && CalamityPlayer.areThereAnyDamnBosses)
                damage = target.lifeMax / 12;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 62);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 96;
            projectile.position.X = projectile.position.X - projectile.width / 2;
            projectile.position.Y = projectile.position.Y - projectile.height / 2;
            for (int num193 = 0; num193 < 6; num193++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1.5f);
            }
            for (int num194 = 0; num194 < 60; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 0, default, 2.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1.5f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
            projectile.Damage();
        }
    }
}
