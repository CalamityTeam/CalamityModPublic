using System;
using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class MantisClawSlash : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Particles/SlashSmear";

        public float maxScale = 0;
        private const int TimerCap = 20;
        Color startColor;
        Color endColor;
        int dir = 1;

        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.timeLeft = TimerCap;
            Projectile.tileCollide = false;
            Projectile.width = 256;
            Projectile.height = 256;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = TrueMeleeDamageClass.Instance;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = 0f;

            Projectile.ai[2] = Main.rand.NextFloat(0.5f, 1.25f);

            if (Main.rand.NextBool(2)) dir = -1;

            // I cherry picked five specific colors to make a similar but different palette to the claws themselves
            // of those five, the projectile lerps between two, randomly chosen when the projectile is spawned
            List<Color> ColorList =
            [
                new Color(248, 197, 58),
                new Color(143, 208, 50),
                new Color(69, 114, 227),
                new Color(212, 128, 187),
                new Color(255, 140, 82),
            ];

            startColor = ColorList[Main.rand.Next(ColorList.Count)];
            endColor = ColorList[Main.rand.Next(ColorList.Count)];
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int size = 60;
            int scale = (int)(size * Math.Max(Projectile.ai[2], 1));
            Vector2 position = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * size / 2;
            return CalamityUtils.CircularHitboxCollision(position, scale, targetHitbox);
        }

        public override void AI()
        {
            if (maxScale == 0)
            {
                maxScale = Projectile.localAI[0];
                Projectile.ai[2] *= maxScale;
            }
            Projectile.velocity *= 0.9f;

            if (Projectile.timeLeft > (TimerCap / 2))
            {
                Projectile.scale = MathHelper.Lerp(Projectile.scale, Projectile.ai[2], 0.1f);
            }
            else
            {
                Projectile.scale = MathHelper.Lerp(Projectile.scale, Projectile.ai[2], -0.1f);
            }

            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(3f) * dir);

            Projectile.ai[1]++;
            Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], MathHelper.TwoPi * dir, 0.15f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HeavyBleeding>(), 180);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<HeavyBleeding>(), 180);

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);

            for (float i = 0; i < 1; i += 0.33f)
            {
                Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, tex.Frame(), Color.Lerp(startColor, endColor, Projectile.ai[1] / TimerCap).MultiplyRGBA(new Color(255, 255, 255, 0f)),
                    Projectile.rotation - (dir == -1 ? MathHelper.ToRadians(-135f) : MathHelper.ToRadians(180f)) + Projectile.ai[0], tex.Size() / 2, MathHelper.Lerp(0.6f, 1f, i) * Math.Min(Projectile.scale, maxScale), dir == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
            }
            return false;
        }
    }
}
