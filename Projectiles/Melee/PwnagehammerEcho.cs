using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class PwnagehammerEcho : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public static readonly SoundStyle BigSound = new("CalamityMod/Sounds/Item/PwnagehammerBigImpact") { Volume = 0.6f };
        public static readonly SoundStyle Kunk = new("CalamityMod/Sounds/Item/TF2PanHit") { Volume = 1.1f };
        public int Explodamage = 0;
        public float speed = 0f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 248, 124, 255);
        }

        public override void AI()
        {
            speed = Projectile.velocity.Length();
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 55f)
            {
                CalamityUtils.HomeInOnNPC(Projectile, Projectile.tileCollide, 2000f, speed, 12f);
            }
            if (Projectile.ai[0] < 42f)
            {
                Projectile.velocity.Y *= 0.9575f;
                Projectile.rotation += MathHelper.ToRadians(Projectile.ai[0] * 0.5f) * Projectile.localAI[0];
            }
            else if (Projectile.ai[0] < 44f)
            {
                Projectile.extraUpdates = 2;
                if (Projectile.ai[1] < 0f)
                {
                    Projectile.Kill();
                    return;
                }

                NPC target = Main.npc[(int)Projectile.ai[1]];
                if (!target.CanBeChasedBy(Projectile, false) || !target.active)
                    Projectile.Kill();
                else
                {
                    float velConst = 24f;
                    Projectile.velocity = new Vector2((target.Center.X - Projectile.Center.X) / velConst, (target.Center.Y - Projectile.Center.Y) / velConst);
                    Projectile.rotation += MathHelper.ToRadians(48f) * Projectile.localAI[0];
                }
            }

            if (Main.rand.NextBool(2))
            {
                Vector2 offset = new Vector2(7, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(3, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.position.X, Projectile.position.Y) + offset, DustID.GoldFlame, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, new Color(255, 245, 198), 2f);
                dust.noGravity = true;
            }

            if (Main.rand.NextBool(6))
            {
                Vector2 offset = new Vector2(7, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(3, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.position.X, Projectile.position.Y) + offset, DustID.GoldFlame, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, new Color(255, 245, 198), 2f);
                dust.noGravity = true;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] <= 42f)
                return false;
            return null;
        }

        public override bool CanHitPvp(Player target) => Projectile.ai[0] > 42f;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localAI[0] = target.whoAmI;
        }

        public override bool PreKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            float numberOfDusts = 32f;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(15f, 0).RotatedBy(rot);
                Vector2 velOffset = new Vector2(12.5f, 0).RotatedBy(rot);
                int dust = Dust.NewDust(Projectile.position + offset, Projectile.width, Projectile.height, 269, velOffset.X, velOffset.Y);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity = velOffset;
                Main.dust[dust].scale = 3f;
            }

            if (Main.zenithWorld)
                SoundEngine.PlaySound(Kunk, Projectile.Center);

            else
                SoundEngine.PlaySound(BigSound, Projectile.Center);

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0f, ModContent.ProjectileType<PwnagehammerExplosionBig>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 0f);

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = sourceRectangle.Size() / 2f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float rot = MathHelper.ToRadians(22.5f) * Math.Sign(Projectile.velocity.X);
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(), color, Projectile.rotation - i * rot, origin, Projectile.scale, spriteEffects, 0);
            }
            return false;
        }
    }
}
