using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.Boss
{
    public class SCalBrimstoneGigablast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public static readonly SoundStyle ImpactSound = new("CalamityMod/Sounds/Custom/SCalSounds/BrimstoneGigablastImpact");
        public bool withinRange = false;
        public bool setLifetime = false;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.Opacity = 0f;
            Projectile.tileCollide = false;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 5)
                Projectile.frame = 0;

            if (!withinRange)
            {
                if (Projectile.ai[2] == 1f)
                    Projectile.Opacity = MathHelper.Clamp(Projectile.timeLeft / 60f, 0f, 1f);
                else
                    Projectile.Opacity = MathHelper.Clamp(1f - ((Projectile.timeLeft - 130) / 20f), 0f, 1f);
            }

            Lighting.AddLight(Projectile.Center, 0.9f * Projectile.Opacity, 0f, 0f);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            }

            int target = Player.FindClosest(Projectile.Center, 1, 1);

            if (!withinRange)
            {
                float projSpeed = Projectile.velocity.Length();
                Vector2 playerVec = Main.player[target].Center - Projectile.Center;
                playerVec.Normalize();
                playerVec *= projSpeed;
                Projectile.velocity = (Projectile.velocity * 24f + playerVec) / 25f;
                Projectile.velocity.Normalize();
                Projectile.velocity *= projSpeed;
            }

            float targetDist;
            if (target != -1 && !Main.player[target].dead && Main.player[target].active && Main.player[target] != null)
                targetDist = Vector2.Distance(Main.player[target].Center, Projectile.Center);
            else
                targetDist = 1000;

            if (Projectile.ai[1] == 2f && !withinRange && Main.rand.NextBool())
            {
                SparkParticle orb = new SparkParticle(Projectile.Center - Projectile.velocity + Main.rand.NextVector2Circular(30, 30), -Projectile.velocity * Main.rand.NextFloat(0.1f, 1f), false, 14, Main.rand.NextFloat(0.5f, 0.75f), (Main.rand.NextBool() ? Color.Lerp(Color.Red, Color.Magenta, 0.5f) : Color.Red) * Projectile.Opacity);
                GeneralParticleHandler.SpawnParticle(orb);
            }
            if ((Projectile.timeLeft == 1 && !withinRange) || (targetDist < 224 && Projectile.Opacity == 1f)) // When within 14 blocks of player or when it runs out of time
            {
                if (!setLifetime)
                {
                    Projectile.timeLeft = 60;
                    setLifetime = true;
                }
                withinRange = true;
            }
            if (withinRange && Projectile.ai[2] == 0f)
            {
                Projectile.velocity *= 0.9f;
                for (int i = 0; i < 2; i++)
                {
                    Dust failShotDust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 60 : 114);
                    failShotDust.noGravity = true;
                    failShotDust.velocity = new Vector2(4, 4).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1.3f);
                    failShotDust.scale = Main.rand.NextFloat(0.7f, 1.8f);
                }
                if (Projectile.timeLeft <= 40)
                {
                    if (Projectile.Opacity > 0)
                        Projectile.Opacity -= 0.05f;
                }
                if (Projectile.timeLeft == 30)
                {
                    Projectile.Opacity = 0;
                    Projectile.velocity *= 0;
                    for (int i = 0; i < 2; i++)
                    {
                        Particle bloom = new BloomParticle(Projectile.Center, Vector2.Zero, new Color(121, 21, 77), 0.1f, 0.85f, 30, false);
                        GeneralParticleHandler.SpawnParticle(bloom);
                        if (Projectile.ai[2] == 1f)
                            bloom.Lifetime = 0;
                    }
                }
                if (Projectile.timeLeft == 15)
                {
                    Particle bloom = new BloomParticle(Projectile.Center, Vector2.Zero, Color.Red, 0.1f, 0.8f, 15, false);
                    GeneralParticleHandler.SpawnParticle(bloom);
                }
                if (Projectile.timeLeft == 8)
                {
                    Particle bloom = new BloomParticle(Projectile.Center, Vector2.Zero, Color.White, 0.1f, 0.7f, 8, false);
                    GeneralParticleHandler.SpawnParticle(bloom);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int drawStart = frameHeight * Projectile.frame;
            lightColor.R = (byte)(255 * Projectile.Opacity);

            if (CalamityGlobalNPC.SCal != -1 && NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()) == true)
            {
                if (Main.npc[CalamityGlobalNPC.SCal].active)
                {
                    if (Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().permafrost)
                    {
                        lightColor.G = (byte)(255 * Projectile.Opacity);
                        lightColor.B = (byte)(255 * Projectile.Opacity);
                        lightColor.R = 0;
                    }
                }
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, drawStart, texture.Width, frameHeight)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool CanHitPlayer(Player target) => Projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0 || Projectile.Opacity != 1f)
                return;

            target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 300, true);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(ImpactSound, Projectile.Center);

            // Difficulty modes
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            if (Projectile.ai[2] == 0f)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    int totalProjectiles = bossRush ? 44 : death ? 36 : revenge ? 32 : expertMode ? 28 : 20;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    int type = ModContent.ProjectileType<BrimstoneBarrage>();
                    float velocity = 6.5f;
                    Vector2 spinningPoint = new Vector2(0f, -velocity);
                    for (int k = 0; k < totalProjectiles; k++)
                    {
                        Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity2, type, (int)Math.Round(Projectile.damage * 0.75), 0f, Projectile.owner, 0f, Projectile.ai[1], velocity * 1.5f);
                    }
                }

                if (Projectile.ai[1] == 2f)
                {
                    for (int i = 0; i < 25; i++)
                    {
                        Vector2 velocity = new Vector2(15, 15).RotatedByRandom(100);
                        PointParticle spark2 = new PointParticle(Projectile.Center + velocity, velocity * Main.rand.NextFloat(0.3f, 1f), false, 15, 1.25f, (Main.rand.NextBool() ? Color.Lerp(Color.Red, Color.Magenta, 0.5f) : Color.Red) * 0.6f);
                        GeneralParticleHandler.SpawnParticle(spark2);
                    }
                    for (int i = 0; i < 25; i++)
                    {
                        Dust failShotDust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 60 : 114);
                        failShotDust.noGravity = true;
                        failShotDust.velocity = new Vector2(20, 20).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1.3f);
                        failShotDust.scale = Main.rand.NextFloat(0.9f, 1.8f);
                    }
                }
            }
        }
    }
}
