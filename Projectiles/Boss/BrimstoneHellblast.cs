using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.NPCs;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using CalamityMod.Particles;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneHellblast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 255;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 10)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;

            int target = Player.FindClosest(Projectile.Center, 1, 1);

            float targetDist;
            if (target != -1 && !Main.player[target].dead && Main.player[target].active && Main.player[target] != null)
                targetDist = Vector2.Distance(Main.player[target].Center, Projectile.Center);
            else
                targetDist = 1000;

            Lighting.AddLight(Projectile.Center, 0.9f * Projectile.Opacity, 0f, 0f);

            if (targetDist < 1400f && Projectile.ai[1] == 2f)
            {
                // Spawn in a helix-style pattern
                float sine = (float)Math.Sin(Projectile.timeLeft * 0.575f / MathHelper.Pi);

                Vector2 offset = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * sine * 16f;

                SparkParticle orb = new(Projectile.Center + offset, -Projectile.velocity * 0.05f, false, 8, 0.8f, Main.rand.NextBool() ? Color.Red : Color.Lerp(Color.Red, Color.Magenta, 0.5f));
                GeneralParticleHandler.SpawnParticle(orb);

                SparkParticle orb2 = new(Projectile.Center - offset, -Projectile.velocity * 0.05f, false, 8, 0.8f, Main.rand.NextBool() ? Color.Red : Color.Lerp(Color.Red, Color.Magenta, 0.5f));
                GeneralParticleHandler.SpawnParticle(orb2);
            }

            if (Projectile.timeLeft < 51)
                Projectile.Opacity -= 0.02f;

            if (Projectile.ai[2] == 0f)
            {
                Projectile.ai[2] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            }

            if (Projectile.velocity.Length() < 18f)
                Projectile.velocity *= 1.03f;

            if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = (float)Math.Atan2(-Projectile.velocity.Y, -Projectile.velocity.X);
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
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

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool CanHitPlayer(Player target) => Projectile.timeLeft >= 51;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0 || Projectile.timeLeft < 51)
                return;

            if (Projectile.ai[0] == 0f || Main.zenithWorld)
                target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 180);
            else
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            for (int dust = 0; dust <= 5; dust++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f);
            }
        }
    }
}
