using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Magic
{
    public class CryoBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        private const float Spread = 0.15f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 35;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 90;
            Projectile.coldDamage = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.scale <= 2.5f)
            {
                Projectile.scale *= 1.02f;
                Projectile.ExpandHitboxBy((int)(35f * Projectile.scale));
            }
            else if (Projectile.ai[0] < 2f)
            {
                Projectile.ai[0] += 1f;

                if (Projectile.owner == Main.myPlayer)
                {
                    // Fire extra waves to the left and right
                    for (int i = 0; i < 2; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedBy(-Spread * (i + 1)), Projectile.type, Projectile.damage / 2, Projectile.knockBack, Projectile.owner, Projectile.ai[0], 0f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedBy(+Spread * (i + 1)), Projectile.type, Projectile.damage / 2, Projectile.knockBack, Projectile.owner, Projectile.ai[0], 0f);
                    }
                }

                Projectile.Kill();
            }

            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.5f);

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f || Projectile.ai[0] > 0f)
            {
                int ice = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 66, 0f, 0f, 100, default, Projectile.scale * 0.5f);
                Main.dust[ice].noGravity = true;
                Main.dust[ice].velocity *= 0f;
                int snow = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 185, 0f, 0f, 100, default, Projectile.scale * 0.5f);
                Main.dust[snow].noGravity = true;
                Main.dust[snow].velocity *= 0f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if ((Projectile.timeLeft > 596 && Projectile.ai[0] == 0f) || (Projectile.timeLeft > 599 && Projectile.ai[0] > 0f))
                return false;

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            int height = texture.Height / Main.projFrames[Projectile.type];
            int frameHeight = height * Projectile.frame;
            Rectangle rectangle = new Rectangle(0, frameHeight, texture.Width, height);
            Vector2 origin = new Vector2(texture.Width / 2f, height / 2f);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(rectangle), lightColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
            for (int index1 = 0; index1 < 15; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 88, 0f, 0f, 0, new Color(), 0.9f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 1.5f;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 shardPos = Projectile.oldPosition + 0.5f * Projectile.Size;
                for (int i = 0; i < 3; i++)
                {
                    Vector2 shardVel = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    while (shardVel.X == 0f && shardVel.Y == 0f)
                    {
                        shardVel = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    }
                    shardVel.Normalize();
                    shardVel *= Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), shardPos, shardVel, ProjectileID.Blizzard, Projectile.damage / 2, Projectile.knockBack * 0.5f, Projectile.owner);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
            target.AddBuff(BuffID.Frostburn2, 180);
        }
    }
}
