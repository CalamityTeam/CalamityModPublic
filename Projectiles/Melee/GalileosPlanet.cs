using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using CalamityMod.Buffs.DamageOverTime;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class GalileosPlanet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        private float radius = 84f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 88;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
            for (float i = -5; i <= 0; i++)
            {
                if (Main.rand.NextFloat(1f) < 0.4f)
                {
                    // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                    Vector2 pos = Projectile.position + new Vector2((Projectile.width / 2) - 8, Projectile.height + 16);
                    Dust dust1 = Dust.NewDustPerfect(pos + new Vector2(-20 + i, -2), 180, new Vector2(i * 0.7f, -1.1f));
                    dust1.noGravity = true;
                }
            }
            for (float i = 0; i <= 5; i++)
            {
                if (Main.rand.NextFloat(1f) < 0.4f)
                {
                    // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                    Vector2 pos = Projectile.position + new Vector2((Projectile.width / 2) - 8, Projectile.height + 16);
                    Dust dust1 = Dust.NewDustPerfect(pos + new Vector2(20 + i, -2), 180, new Vector2(i * 0.7f, -1.1f));
                    dust1.noGravity = true;
                }
            }

            Dust dust;
            // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
            Vector2 pos2 = Projectile.Center - new Vector2(20, 20);
            dust = Main.dust[Dust.NewDust(pos2, 40, 40, 180, 0f, 0f, 0, new Color(255, 255, 255), 1f)];

            Lighting.AddLight(Projectile.Center, 0.3f, 0.3f, 0.7f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 32;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            radius = 100;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= 2;
            Projectile.Damage();
            for (int i = 0; i < 72; i++)
            {
                Dust dust;
                // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                Vector2 position = Projectile.Center - new Vector2(30, 30);
                dust = Main.dust[Terraria.Dust.NewDust(position, 60, 60, 187, Main.rand.Next(-5, 5), Main.rand.Next(-5, 5), 0, new Color(0, 167, 255), 5f)];
                dust.noGravity = true;
                dust.shader = GameShaders.Armor.GetSecondaryShader(47, Main.LocalPlayer);
            }

            SoundEngine.PlaySound(SoundID.NPCDeath43, Projectile.position);

            for (int i = 0; i < 155; i++)
            {
                Dust dust;
                // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                Vector2 position = Projectile.position;
                dust = Main.dust[Dust.NewDust(position, 94, 94, 180, Main.rand.Next(-5, 5), Main.rand.Next(-5, 5), 0, new Color(255, 255, 255), Main.rand.NextFloat(1.2f, 2.2f))];
                dust.noGravity = true;
            }

            for (int x = 1; x <= 3; x++)
            {
                int dustAmt = 36;
                for (int i = 0; i < dustAmt; i++)
                {
                    Vector2 spinningpoint = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f * 0.5f;
                    spinningpoint = spinningpoint.RotatedBy((double)((float)(i - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default(Vector2)) + Projectile.Center;
                    Vector2 vector = spinningpoint - Projectile.Center;
                    int stratus = Dust.NewDust(spinningpoint + vector, 0, 0, 180, vector.X * 2f, vector.Y * 2f, 0, new Color(255, 255, 255), 1f);
                    Main.dust[stratus].noGravity = true;
                    Main.dust[stratus].velocity = Vector2.Normalize(vector) * x;
                }
            }

            for (int x = 1; x <= 3; x++)
            {
                int dustAmt = 36;
                for (int i = 0; i < dustAmt; i++)
                {
                    Vector2 spinningpoint = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f * 0.5f;
                    spinningpoint = spinningpoint.RotatedBy((double)((float)(i - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default(Vector2)) + Projectile.Center;
                    Vector2 vector = spinningpoint - Projectile.Center;
                    int stratus = Dust.NewDust(spinningpoint + vector, 0, 0, 180, vector.X * 2f, vector.Y * 2f, 0, new Color(255, 255, 255), 1f);
                    Main.dust[stratus].noGravity = true;
                    Main.dust[stratus].velocity = Vector2.Normalize(vector) * x;
                    Main.dust[stratus].velocity *= 2;
                }
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 goreSource = Projectile.Center;
                int goreAmt = 3;
                Vector2 source = new Vector2(goreSource.X - 24f, goreSource.Y - 24f);
                for (int goreIndex = 0; goreIndex < goreAmt; goreIndex++)
                {
                    float velocityMult = 0.33f;
                    if (goreIndex < (goreAmt / 3))
                    {
                        velocityMult = 0.66f;
                    }
                    if (goreIndex >= (2 * goreAmt / 3))
                    {
                        velocityMult = 1f;
                    }
                    Mod mod = ModContent.GetInstance<CalamityMod>();
                    int type = Main.rand.Next(61, 64);
                    int smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    Gore gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y -= 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y -= 1f;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 240);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);
    }
}
