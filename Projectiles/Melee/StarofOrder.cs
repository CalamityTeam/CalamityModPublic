using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class StarofOrder : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public ref float time => ref Projectile.ai[0];
        public Color mainColor;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 4;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 7;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float hitboxSize = Projectile.width * Projectile.scale;
            return CalamityUtils.CircularHitboxCollision(Projectile.Center, hitboxSize, targetHitbox);
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            mainColor = player.Calamity().lightRGB;
            if (time == 0 && Projectile.numHits == 0)
            {
                Projectile.scale *= Main.rand.NextFloat(0.65f, 0.8f);
            }
            float sine = (float)Math.Sin(time * 0.575f * Projectile.scale / MathHelper.Pi);

            Projectile.rotation = 0.25f * sine;
            if (time < 20)
            {
                Projectile.extraUpdates = 1;
                Projectile.velocity = Projectile.velocity.RotatedBy(-Projectile.ai[1] * 0.05f);
            }
            else
            {
                Projectile.extraUpdates = 0;
                CalamityUtils.HomeInOnNPC(Projectile, true, 900f, 25, MathHelper.Clamp(30 - time, 15, 30));
            }
            if (time % 2 == 0 && Projectile.numHits < 1)
            {
                Particle spark = new CustomSpark(Projectile.Center, Projectile.velocity * 0.2f, "CalamityMod/Particles/BloomCircle", false, 13, 0.35f, mainColor * 0.75f, new Vector2(0.8f, 1.35f) * Projectile.scale, true, false, shrinkSpeed: 0.3f);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            
            time++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.Lerp(mainColor, Color.White, 0.2f) with { A = 0 };
            float drawRotation = Projectile.rotation;
            Vector2 rotationPoint = texture.Size() * 0.5f;
            //CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], drawColor * 0.5f, 1, texture, true, true);
            bool roted = true;
            for (int i = 0; i < 5; i++)
            {
                Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, (roted ? 0 : MathHelper.PiOver2) + Projectile.rotation, rotationPoint, new Vector2(1 - 0.12f * i, 1 + 0.75f * i) * Projectile.scale * 0.2f * Main.rand.NextFloat(0.8f, 1.1f), SpriteEffects.None);
                if (roted && i == 4)
                {
                    i = -1;
                    roted = false;
                }
            }
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int buffType = ModContent.BuffType<ElementalMix>();
            target.AddBuff(buffType, 60);

            if (Projectile.ai[2] == 0)
            {
                Projectile.timeLeft = 180;
                time = 0;
                Projectile.velocity *= 1.2f;
            }
        }
        public override void OnKill(int timeLeft)
        {
            int points = 4;
            float radians = MathHelper.TwoPi / points;
            Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f).RotatedBy(Projectile.rotation));
            for (int k = 0; k < points; k++)
            {
                Vector2 velocity = spinningPoint.RotatedBy(radians * k).RotatedBy(-0.45f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + velocity * 2, ModContent.DustType<SquashDust>(), velocity * 8);
                dust.scale = 3.5f * Projectile.scale;
                dust.color = mainColor;
                dust.noGravity = true;
                dust.fadeIn = 6f * Projectile.scale;
            }
        }
    }
}
