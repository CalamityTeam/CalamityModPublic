using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class CometQuasherMeteor : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public ref float time => ref Projectile.ai[0];
        public Color mainColor = Color.DodgerBlue;
        public int fallTime = 60;
        public NPC chosenTarget;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 800;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            if (time % 5 == 0 && Projectile.extraUpdates < 12)
                Projectile.extraUpdates++;
            if (time == 0)
            {
                chosenTarget = Owner.ClampedMouseWorld().ClosestNPCAt(700);
                if (chosenTarget != null)
                    Projectile.velocity = (chosenTarget.Center - Projectile.Center + chosenTarget.velocity * 8).SafeNormalize(Vector2.UnitX) * 3;
                else
                {
                    // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                    Projectile.velocity = (Owner.Calamity().mouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX) * 3;
                }
            }
            if (Projectile.numHits < 1)
            {
                if (chosenTarget == null || chosenTarget.life <= 0)
                    chosenTarget = Owner.ClampedMouseWorld().ClosestNPCAt(700);
                CalamityUtils.HomeInOnSelectedNPC(Projectile, chosenTarget, true, 0.08f, 5, 0.99f, accelerate: true);
            }

            if (targetDist < 1400f)
            {
                if (time % 11 == 0)
                {
                    GlowSparkParticle orb = new(Projectile.Center + Main.rand.NextVector2Circular(20, 20) * Projectile.scale - Projectile.velocity * 2, -Projectile.velocity * 2, false, 11, 0.025f * Projectile.scale, mainColor * Main.rand.NextFloat(0.7f, 1), new Vector2(1f, 1f), true, false, 0.6f);
                    GeneralParticleHandler.SpawnParticle(orb);
                }
                if (time % 4 == 0)
                    GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(Projectile.Center, -Projectile.velocity * Main.rand.NextFloat(0.2f, 1.5f), Main.rand.NextBool(4) ? Color.AliceBlue : Color.DodgerBlue, 6, Main.rand.NextFloat(0.4f, 0.9f) * Projectile.scale, 0.65f, 0, true));
            }
            if (Main.rand.NextBool(13))
            {
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20, 20), DustID.FireworksRGB, -Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.2f, 3));
                dust2.scale = Main.rand.NextFloat(0.55f, 0.85f);
                dust2.noGravity = true;
                dust2.color = Main.rand.NextBool(3) ? Color.AliceBlue : Color.DodgerBlue;
            }
            
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            time++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex;
            switch (Projectile.ai[1])
            {
                case 0:
                default:
                    tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
                    break;
                case 1:
                    tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/CometQuasherMeteor2").Value;
                    break;
                case 2:
                    tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/CometQuasherMeteor3").Value;
                    break;
            }

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], Color.White, 1, tex);
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float minMult = 0.2f;
            int hitsToMinMult = 4;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;
            Projectile.netUpdate = true;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.netUpdate = true;
            Player Owner = Main.player[Projectile.owner];
            SoundEngine.PlaySound(SoundID.Item89, Projectile.position);

            if (Projectile.ai[2] > 0)
            {
                Vector2 spawnSpot = Owner.Center + new Vector2(Main.rand.NextFloat(-550, 550), Main.rand.NextFloat(-750, -950));
                Projectile meteor = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnSpot, Vector2.Zero, ModContent.ProjectileType<CometQuasherMeteor>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 0, Projectile.ai[2] - 1);
                meteor.scale = Projectile.scale;
            }

            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.damage = (int)(Projectile.damage * 0.7f);
                Projectile.ExpandHitboxBy((int)(128f * Projectile.scale));
                Projectile.penetrate = -1;
                Projectile.Damage();
            }
            for (int i = 0; i < 13; i++)
            {
                Particle spark3 = new GlowOrbParticle(Projectile.Center, Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(3.5f, 9), false, 20, Main.rand.NextFloat(0.5f, 1f) * Projectile.scale, Main.rand.NextBool(5) ? Color.AliceBlue : Color.DodgerBlue, true, false, false);
                GeneralParticleHandler.SpawnParticle(spark3);
            }
            if (!CalamityClientConfig.Instance.Photosensitivity)
            {
                Particle blastRing = new CustomPulse(Projectile.Center, Vector2.Zero, mainColor * 0.7f, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 2f * Projectile.scale, 1f * Projectile.scale, 25, true);
                GeneralParticleHandler.SpawnParticle(blastRing);
                Particle blastRing2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White * 0.7f, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 1f * Projectile.scale, 0.3f * Projectile.scale, 25, true);
                GeneralParticleHandler.SpawnParticle(blastRing2);
            }
        }
        public override bool? CanCutTiles() => false;
    }
}
