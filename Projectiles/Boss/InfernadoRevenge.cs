using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class InfernadoRevenge : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public const int TornadoHeight = 8800;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 320;
            Projectile.height = 1020;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 360000;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (!CalamityPlayer.areThereAnyDamnBosses)
            {
                Projectile.active = false;
                Projectile.netUpdate = true;
                return;
            }
        }

        internal Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Yellow, Color.Yellow, completionRatio);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Bottom,
                Projectile.Bottom - Vector2.UnitY * TornadoHeight,
                72,
                ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:Bordernado"].UseSaturation(-0.2f);
            GameShaders.Misc["CalamityMod:Bordernado"].SetShaderTexture(ModContent.Request<Texture2D>("Terraria/Images/Misc/Perlin"));
            Vector2[] drawPoints = new Vector2[5];
            Vector2 upwardAscent = Vector2.UnitY * TornadoHeight;
            Vector2 downwardOffset = Vector2.UnitY * Projectile.height / (drawPoints.Length + 1);

            Vector2 bottom = Projectile.Bottom + downwardOffset;
            Vector2 top = bottom - upwardAscent;
            for (int i = 0; i < drawPoints.Length - 1; i++)
                drawPoints[i] = Vector2.Lerp(top, bottom, i / (float)(drawPoints.Length - 1));

            drawPoints[drawPoints.Length - 1] = bottom;
            PrimitiveSet.Prepare(drawPoints, new((_) => Projectile.width * 0.5f + 16f, ColorFunction, shader: GameShaders.Misc["CalamityMod:Bordernado"]), 85);

            Texture2D vortexTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/OldDukeVortex").Value;
            for (int i = 0; i < 110; i++)
            {
                float angle = MathHelper.TwoPi * i / 50f + Main.GlobalTimeWrappedHourly * MathHelper.TwoPi;
                Color drawColor = Color.White * 0.04f;
                drawColor.A = 0;
                Vector2 drawPosition = bottom + angle.ToRotationVector2() * 4f - Main.screenPosition;

                drawPosition += (angle + Main.GlobalTimeWrappedHourly * i / 16f).ToRotationVector2() * 6f;
                Main.EntitySpriteDraw(vortexTexture, drawPosition, null, drawColor, angle + MathHelper.PiOver2, vortexTexture.Size() * 0.5f, 0.9f, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<Dragonfire>(), 150);
        }
    }
}
