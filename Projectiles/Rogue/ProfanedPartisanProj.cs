using System.Linq;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ProfanedPartisanProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ProfanedPartisan";


        public static int Lifetime => 600;
        public int ExplodeTime => 60 * Projectile.MaxUpdates;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 24;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 5;
            Projectile.timeLeft = Lifetime * Projectile.MaxUpdates;
            Projectile.ignoreWater = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        public float StuckEnemyID
        {
            get { return Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }
        public float StuckEnemyDistance
        {
            get { return Projectile.ai[1]; }
            set { Projectile.ai[1] = value; }
        }
        public float StuckEnemyRotation
        {
            get { return Projectile.ai[2]; }
            set { Projectile.ai[2] = value; }
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1f, 0.8f, 0.2f);
            if (StuckEnemyID > 0)
            {
                Projectile.tileCollide = false;
                if (!Main.npc[(int)StuckEnemyID - 1].active)
                {
                    StuckEnemyID = 0;
                    Projectile.tileCollide = true;
                    Projectile.timeLeft = ExplodeTime;
                    return;
                }
                Projectile.Center = Main.npc[(int)StuckEnemyID - 1].Center + Vector2.UnitX.RotatedBy(StuckEnemyRotation) * StuckEnemyDistance;
                return;
            }
            else if (Projectile.damage == 0) //after running out of penetrate hits, if not impaled or dead
            {
                Projectile.velocity *= 0.975f;
                float intensity = 1 - Projectile.timeLeft / (float)ExplodeTime;
                Projectile.position += Main.rand.NextVector2CircularEdge(intensity, intensity);

                if (Projectile.timeLeft == 1)
                {

                    Color hiColor = new Color(255, 155, 25, 255);
                    Color loColor = new Color(255, 0, 0, 0);

                    for (int i = 0; i < 25; i++)
                    {
                        GeneralParticleHandler.SpawnParticle(new GlowOrbParticle(Projectile.Center, new Vector2(Main.rand.NextFloat(10), 0).RotatedByRandom(MathHelper.TwoPi), false, 10, Main.rand.NextFloat(0.8f, 1.2f), hiColor));
                    }

                    GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", Vector2.One, 0f, 0.5f, 0.1f, 4));

                    for (float i = 0; i < 1; i += 0.25f)
                    {
                        GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, hiColor, "CalamityMod/Particles/SoftRoundExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0.02f * i, 0.075f * i, 24));
                    }
                    GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, hiColor, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0.02f, 0.045f, 16));

                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact.WithPitchOffset(0.6f), Projectile.Center);
                    SoundEngine.PlaySound(SoundID.Item100.WithPitchOffset(0.4f), Projectile.Center);
                    if (Main.myPlayer == Projectile.owner)
                    {
                        for (var i = 0; i < 3; i++)
                        {
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2().RotatedBy((i - 1) * 0.25f + Main.rand.NextFloat(-0.1f, 0.1f)) * 24f, ModContent.ProjectileType<ProfanedPartisanFlare>(), 0, Projectile.knockBack, Projectile.owner, 0f, 0f);
                        }
                    }
                }
                return;
            }


            Projectile.velocity *= 0.998f;
            if (Projectile.timeLeft < (Lifetime - 45)*Projectile.MaxUpdates)
                Projectile.velocity.Y += 0.02f;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (Main.rand.NextBool(3))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.ProfanedFire, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1.1f);
                Main.dust[d].position = Projectile.Center;
                Main.dust[d].velocity *= 0.3f;
                Main.dust[d].velocity += Projectile.velocity * 0.85f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.ProfanedFire, 0f, 0f, 50, default, 2.6f);
            }
            SoundEngine.PlaySound(SoundID.Item45, Projectile.position);
        }

        public override bool PreDraw(ref Color lightColor)
        {

            for (var i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i += 3)
            {
                Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition, null, lightColor * Projectile.Opacity * (i > 0 ? (1 - i / (float)ProjectileID.Sets.TrailCacheLength[Type]) * 0.25f : 1), Projectile.rotation, TextureAssets.Projectile[Type].Size() * 0.5f, Projectile.scale, 0);
            }

            float glowIntensity = Projectile.damage == 0 && StuckEnemyID == 0 ? 4f * (1-(Projectile.timeLeft/(float)ExplodeTime)) : 0f;
            Projectile.DrawProjectileWithBackglow(Color.Yellow, lightColor, glowIntensity);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.tileCollide = false;
            if (Projectile.Calamity().stealthStrike)
            {
                StuckEnemyID = target.whoAmI + 1;
                StuckEnemyDistance = Projectile.Distance(target.Center);
                StuckEnemyRotation = Projectile.DirectionFrom(target.Center).ToRotation();
                Projectile.timeLeft = Lifetime * Projectile.MaxUpdates;
            }
            else
            {
                Projectile.timeLeft = ExplodeTime;
            } 
            bool strongSplit = Main.projectile.Any(x => x.active && x.owner == Projectile.owner && x.type == Projectile.type && x.ai[0] == (target.whoAmI + 1));
            if (!strongSplit)
                for (var i = 0; i < 3; i++)
                {
                    var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2().RotatedBy((i-1)*0.25f + Main.rand.NextFloat(-0.1f,0.1f)) * 10, ModContent.ProjectileType<ProfanedPartisanSpear>(), 0, Projectile.knockBack, Projectile.owner, 0f, 0f);
                    p.localNPCImmunity[target.whoAmI] = 60;
                }
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
            if (!strongSplit)
            {
                Projectile.Kill();
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
    }
}
