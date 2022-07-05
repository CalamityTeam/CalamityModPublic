using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class ReaperProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/TheReaper";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Reaper");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = Projectile.MaxUpdates * 90;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 32; // can't hit too fast, but can hit many many times
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 8;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.ai[1] += 1f;

                // If the Reaper lands a hit, switch to second behavior mode immediately.
                if (Projectile.ai[1] >= 60f || Projectile.numHits > 0)
                {
                    Projectile.localAI[0] = 1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }

                // Initial homing before landing a hit.
                else
                    CalamityUtils.HomeInOnNPC(Projectile, true, 250f, 12f, 14f);
            }

            // Homing after landing a hit. This homing repeatedly turns on and off.
            else
            {
                float homingRange = 700f;
                bool noHomingThisFrame = false;
                if (Projectile.ai[0] == 1f)
                {
                    Projectile.ai[1] += 1f;
                    if (Projectile.ai[1] > 40f)
                    {
                        Projectile.ai[1] = 1f;
                        Projectile.ai[0] = 0f;
                        Projectile.netUpdate = true;
                    }
                    else
                        noHomingThisFrame = true;
                }

                if (noHomingThisFrame)
                    return;

                Vector2 homingTarget = Projectile.Center;
                bool foundTarget = false;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC nPC2 = Main.npc[i];
                    if (nPC2.CanBeChasedBy(Projectile, false))
                    {
                        float npcDist = Vector2.Distance(nPC2.Center, Projectile.Center);
                        if (!foundTarget)
                        {
                            homingRange = npcDist;
                            homingTarget = nPC2.Center;
                            foundTarget = true;
                            break;
                        }
                    }
                }

                if (foundTarget && Projectile.ai[0] == 0f)
                {
                    Vector2 delta = homingTarget - Projectile.Center;
                    float distance = delta.Length();
                    delta /= distance;

                    if (distance > 200f)
                    {
                        float homingScalar = 11f;
                        delta *= homingScalar;
                        Projectile.velocity = (Projectile.velocity * 40f + delta) / 41f;
                    }
                    else
                    {
                        float homingScalar = 3.6f;
                        delta *= -homingScalar; // yes this is intentionally backwards
                        Projectile.velocity = (Projectile.velocity * 40f + delta) / 41f;
                    }
                }

                if (Projectile.ai[1] > 0f)
                {
                    Projectile.ai[1] += (float)Main.rand.Next(1, 4);
                }
                if (Projectile.ai[1] > 40f)
                {
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }
                if (Projectile.ai[0] == 0f)
                {
                    if (Projectile.ai[1] == 0f && foundTarget && homingRange < 500f)
                    {
                        Projectile.ai[1] += 1f;
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Projectile.ai[0] = 1f;
                            Vector2 value20 = homingTarget - Projectile.Center;
                            value20.Normalize();
                            Projectile.velocity = value20 * 8f;
                            Projectile.netUpdate = true;
                        }
                    }
                }
            }
            Projectile.rotation += 0.07f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item21, Projectile.position);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int num621 = 0; num621 < 5; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 8; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }
    }
}
