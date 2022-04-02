using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class InfernadoRevenge : ModProjectile
    {
        internal PrimitiveTrail TornadoDrawer;
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public const int TornadoHeight = 8800;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infernado");
        }

        public override void SetDefaults()
        {
            projectile.width = 320;
            projectile.height = 1020;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.timeLeft = 360000;
            cooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (!CalamityPlayer.areThereAnyDamnBosses)
            {
                projectile.active = false;
                projectile.netUpdate = true;
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
                projectile.Bottom,
                projectile.Bottom - Vector2.UnitY * TornadoHeight,
                72,
                ref _);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (TornadoDrawer is null)
                TornadoDrawer = new PrimitiveTrail(_ => projectile.width * 0.5f + 16f, ColorFunction, specialShader: GameShaders.Misc["CalamityMod:Bordernado"]);

            GameShaders.Misc["CalamityMod:Bordernado"].UseSaturation(-0.2f);
            GameShaders.Misc["CalamityMod:Bordernado"].SetShaderTexture(ModContent.GetTexture("Terraria/Misc/Perlin"));
            Vector2[] drawPoints = new Vector2[5];
            Vector2 upwardAscent = Vector2.UnitY * TornadoHeight;
            Vector2 downwardOffset = Vector2.UnitY * projectile.height / (drawPoints.Length + 1);

            Vector2 bottom = projectile.Bottom + downwardOffset;
            Vector2 top = bottom - upwardAscent;
            for (int i = 0; i < drawPoints.Length - 1; i++)
                drawPoints[i] = Vector2.Lerp(top, bottom, i / (float)(drawPoints.Length - 1));

            drawPoints[drawPoints.Length - 1] = bottom;
            TornadoDrawer.Draw(drawPoints, -Main.screenPosition, 85);

            Texture2D vortexTexture = ModContent.GetTexture("CalamityMod/Projectiles/Boss/OldDukeVortex");
            for (int i = 0; i < 110; i++)
            {
                float angle = MathHelper.TwoPi * i / 50f + Main.GlobalTime * MathHelper.TwoPi;
                Color drawColor = Color.White * 0.04f;
                drawColor.A = 0;
                Vector2 drawPosition = bottom + angle.ToRotationVector2() * 4f - Main.screenPosition;

                drawPosition += (angle + Main.GlobalTime * i / 16f).ToRotationVector2() * 6f;
                spriteBatch.Draw(vortexTexture, drawPosition, null, drawColor, angle + MathHelper.PiOver2, vortexTexture.Size() * 0.5f, 0.9f, SpriteEffects.None, 0f);
            }
            
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<LethalLavaBurn>(), 420);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)    
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
