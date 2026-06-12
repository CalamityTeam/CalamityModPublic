using System.Collections.Generic;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class TriactisHammerExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public ref float Timer => ref Projectile.ai[0];
        public Player Owner => Main.player[Projectile.owner];

        public List<List<Vector2>> crackTrails = new List<List<Vector2>>();
        public static int crackCount = 15;
        public static int totalPoints = 12;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Timer == 0f)
            {
                float rotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                Particle blastR = new CustomPulse(Projectile.Center, Vector2.Zero, TriactisHammerFlare.GetColor(1f) * 0.7f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, rotation, 0f, 0.8f, 30);
                GeneralParticleHandler.SpawnParticle(blastR);
                Particle blastG = new CustomPulse(Projectile.Center, Vector2.Zero, TriactisHammerFlare.GetColor(2f) * 0.7f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, rotation + MathHelper.ToRadians(120f), 0f, 0.6f, 27);
                GeneralParticleHandler.SpawnParticle(blastG);
                Particle blastB = new CustomPulse(Projectile.Center, Vector2.Zero, TriactisHammerFlare.GetColor(3f) * 0.7f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, rotation + MathHelper.ToRadians(240f), 0f, 0.4f, 24);
                GeneralParticleHandler.SpawnParticle(blastB);
                Owner.SetScreenshake(15f);
            }

            if (Timer % 4f == 0f)
            {
                crackTrails.Clear();
                for (int i = 0; i < crackCount; i++)
                {
                    List<Vector2> points = new List<Vector2>();
                    for (int j = 0; j < totalPoints; j++)
                    {
                        float radians = MathHelper.TwoPi / crackCount;
                        if (j == 0)
                            points.Add(Projectile.Center + Main.rand.NextVector2Circular(30, 30));
                        else
                        {
                            Vector2 newPoint = new Vector2();
                            Vector2 jtolookfor = j > 1 ? points[j - 2] : Projectile.Center;
                            float baseDist = j == totalPoints - 1 ? 20 : Main.rand.Next(60, 120) * (1 + (20 - Projectile.timeLeft) / 15);
                            newPoint = points[j - 1] + (jtolookfor.DirectionTo(points[j - 1]) * baseDist).RotatedByRandom(MathHelper.PiOver2);
                            points.Add(newPoint);
                        }
                    }
                    crackTrails.Add(points);
                }
            }
            Timer++;

            for (int l = 0; l < 3; l++)
            {
                Particle spark = new GlowSparkParticle(Projectile.Center, Vector2.One.RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(18f, 60f), true, 15, Main.rand.NextFloat(0.02f, 0.05f), TriactisHammerFlare.GetColor(1f + Main.rand.Next(3)), new Vector2(3f, 1f), true);
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 640f, targetHitbox);

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ApplyScalingForcedCrit(Projectile);
            modifiers.HitDirectionOverride = (Owner.Center.X < target.Center.X).ToDirectionInt();
            modifiers.SourceDamage *= Utils.Remap(Projectile.numHits, 0, 10, 1f, 0.1f, true);
        }

        internal float WidthFunction(float completionRatio, Vector2 vertexPos) => MathHelper.Clamp(CalamityUtils.Convert01To010(completionRatio * 2), 0.2f, 1f) * 4f;
        internal Color ColorFunction(float completionRatio, Vector2 vertexPos) => Color.White;

        internal float BackgroundWidthFunction(float completionRatio, Vector2 vertexPos) => WidthFunction(completionRatio, vertexPos) * 2f;
        internal Color BackgroundColorFunction(float completionRatio, Vector2 vertexPos) => ColorFunction(completionRatio, vertexPos) * 0.5f;

        public override bool PreDraw(ref Color lightColor)
        {
            if (crackTrails.Count <= 0)
                return false;

            Main.spriteBatch.EnterShaderRegion(BlendState.Additive);
            GameShaders.Misc["CalamityMod:TeslaTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ZapTrail"));
            foreach (List<Vector2> points in crackTrails)
            {
                PrimitiveRenderer.RenderTrail(points, new(WidthFunction, ColorFunction, smoothen: false, shader: GameShaders.Misc["CalamityMod:TeslaTrail"]), 60);
                PrimitiveRenderer.RenderTrail(points, new(BackgroundWidthFunction, BackgroundColorFunction, smoothen: false, shader: GameShaders.Misc["CalamityMod:TeslaTrail"]), 60);
            }
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }
}
