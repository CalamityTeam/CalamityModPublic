using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyBomb : ModProjectile, ILocalizedModType
    {
        float SquishAnimation = 1f;
        public int FireDamage = 0;

        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 80;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 250;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            CooldownSlot = ImmunityCooldownID.Bosses;
            Projectile.scale = 0;
        }

        public override void OnSpawn(IEntitySource source)
        {
            FireDamage = Providence.FireDamage.CalculateProvidenceDamage();

            if (source is EntitySource_Parent { Entity: NPC parent })
            {
                if (parent.type == ModContent.NPCType<ProfanedGuardianDefender>())
                    FireDamage = ProfanedGuardianDefender.FireDamage;
            }
        }
        public override void AI()
        {
            if (Projectile.scale < 1)
                Projectile.scale += 0.1f;
            ProvUtils.ApplyGFBDamage(Projectile, 120, 20);

            Lighting.AddLight(Projectile.Center, 0.45f, 0.35f, 0f);

            SquishAnimation += 0.06f;
            SquishAnimation = MathHelper.Clamp(SquishAnimation, 0f, 1f);

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] % 120f == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);


                for (int i = 0; i < 10; i++)
                {
                    Vector2 center = Projectile.Center + new Vector2(0, -15);

                    GeneralParticleHandler.SpawnParticle(new GlowOrbParticle(center, new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-4f, 2f)), false, 30, Main.rand.NextFloat(1f, 2.5f), ProvUtils.GetProjectileColor(255)));
                }

                float velocityY = -2f;

                SquishAnimation = 0f;

                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, velocityY, ModContent.ProjectileType<HolyFlare>(), FireDamage, Projectile.knockBack, Projectile.owner, 0f, 0f);
            }

            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            }

            Projectile.velocity *= 0.975f;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return ProvUtils.GetProjectileColor(lightColor);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float squish = CalamityUtils.SineBumpEasing(SquishAnimation, 1) * 0.25f;

            Texture2D texture = ProvUtils.StandardAI() ? Terraria.GameContent.TextureAssets.Projectile[Type].Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/HolyBombNight").Value;
            int framing = texture.Height / Main.projFrames[Type];
            int y6 = framing * Projectile.frame;
            Projectile.DrawBackglow(ProvUtils.GetProjectileColor(lightColor, true), 4f, new Vector2(Projectile.scale + squish, Projectile.scale - squish), texture, offset: new Vector2(0, (-22 * (1 - squish))));
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, framing)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, (framing / 2f) + 22), new Vector2(Projectile.scale + squish, Projectile.scale - squish), SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Color hiColor = ProvUtils.GetProjectileColor(255, false);
            Color loColor = ProvUtils.GetProjectileColor(0, true);

            for (int i = 0; i < 25; i++)
            {
                GeneralParticleHandler.SpawnParticle(new GlowOrbParticle(Projectile.Center, new Vector2(Main.rand.NextFloat(10), 0).RotatedByRandom(MathHelper.TwoPi), false, 10, Main.rand.NextFloat(0.8f, 1.2f), hiColor));
            }

            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", Vector2.One, 0f, 0.5f, 0.1f, 4));

            for (float i = 0; i < 1f; i += 0.25f)
            {
                GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, hiColor, "CalamityMod/Particles/SoftRoundExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0.02f * i, 0.075f * i, 24));
            }
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, hiColor, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0.02f, 0.045f, 16));

            SoundEngine.PlaySound(SoundID.Item20.WithPitchOffset(-0.8f), Projectile.Center);
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item100.WithPitchOffset(0.4f), Projectile.Center);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            // If the player is dodging, don't apply debuffs
            if (info.Damage <= 0 || target.creativeGodMode)
                return;

            ProvUtils.ApplyDebuffs(target, 120);
        }
    }
}
