using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Graphics.Primitives;

namespace CalamityMod.Projectiles.Boss
{
    public class DraedonSummonLaser : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public const float LaserLength = 3800f;

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private const int Lifetime = CalamityWorld.DraedonSummonCountdownMax - 60;

        public override void SetStaticDefaults() 
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.timeLeft = Lifetime;
        }

        public override bool? CanDamage() => Projectile.timeLeft < Lifetime - 30;

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(TeslaCannon.FireSound, Projectile.Center);
                for (int i = 0; i < 36; i++)
                {
                    Dust exoDust = Dust.NewDustPerfect(Projectile.BottomRight, 267);
                    exoDust.color = CalamityUtils.MulticolorLerp(i / 36f, CalamityUtils.ExoPalette);
                    exoDust.velocity = (MathHelper.TwoPi * i / 36f).ToRotationVector2() * new Vector2(3f, 1.45f) - Vector2.UnitY * 2f;
                    exoDust.scale = 2.8f;
                    exoDust.fadeIn = Main.rand.NextFloat(0.8f, 1.85f);
                    exoDust.noGravity = true;
                }

                for (int i = 0; i < 10; i++)
                {
                    Dust exoDust = Dust.NewDustPerfect(Projectile.BottomRight, 267);
                    exoDust.color = CalamityUtils.MulticolorLerp(Main.rand.NextFloat(), CalamityUtils.ExoPalette);
                    exoDust.velocity = Main.rand.NextVector2Circular(2f, 2f);
                    exoDust.scale = 4f;
                    exoDust.fadeIn = 2f;
                    exoDust.noGravity = true;
                }
                Projectile.localAI[0] = 1f;
            }

            Lighting.AddLight(Projectile.Center, Color.White.ToVector3());
            Projectile.scale = Utils.GetLerpValue(-1f, 15f, Projectile.timeLeft, true) * Utils.GetLerpValue(Lifetime + 1f, Lifetime - 15f, Projectile.timeLeft, true);
        }

        private float PrimitiveWidthFunction(float completionRatio)
        {
            return Utils.GetLerpValue(1f, 0.96f, completionRatio, true) * Utils.GetLerpValue(0f, 0.016f, completionRatio, true) * Projectile.scale * 20f;
        }

        private Color PrimitiveColorFunction(float completionRatio)
        {
            return CalamityUtils.MulticolorLerp((Main.GlobalTimeWrappedHourly * 0.67f - completionRatio * 3f) % 1f, CalamityUtils.ExoPalette) * 1.2f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:Flame"].UseImage1("Images/Misc/Perlin");

            Vector2[] basePoints = new Vector2[8];
            for (int i = 0; i < basePoints.Length; i++)
                basePoints[i] = Projectile.Center - Vector2.UnitY * i / basePoints.Length * LaserLength;

            Vector2 overallOffset = Projectile.Size * 0.5f;
            PrimitiveSet.Prepare(basePoints, new(PrimitiveWidthFunction, PrimitiveColorFunction, (_) => overallOffset, shader: GameShaders.Misc["CalamityMod:Flame"]), 92);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center - Vector2.UnitY * LaserLength);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
    }
}
