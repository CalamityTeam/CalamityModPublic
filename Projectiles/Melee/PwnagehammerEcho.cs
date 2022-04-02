using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class PwnagehammerEcho : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pwnagehammer Echo");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.aiStyle = 0;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 248, 124, 255);
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            if (projectile.ai[0] < 42f)
            {
                projectile.velocity.Y *= 0.9575f;
                projectile.rotation += MathHelper.ToRadians(projectile.ai[0] * 0.5f) * projectile.localAI[0];
            }
            else if (projectile.ai[0] < 44f)
            {
                if (projectile.ai[1] < 0f)
                {
                    projectile.Kill();
                    return;
                }

                NPC target = Main.npc[(int)projectile.ai[1]];
                if (!target.CanBeChasedBy(projectile, false) || !target.active)
                    projectile.Kill();
                else
                {
                    float velConst = 3f;
                    projectile.velocity = new Vector2((target.Center.X - projectile.Center.X) / velConst, (target.Center.Y - projectile.Center.Y) / velConst);
                    projectile.rotation += MathHelper.ToRadians(48f) * projectile.localAI[0];
                }
            }

            if (Main.rand.NextBool(2))
            {
                Vector2 offset = new Vector2(7, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(3, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(projectile.position.X, projectile.position.Y) + offset, DustID.GoldFlame, new Vector2(projectile.velocity.X * 0.2f + velOffset.X, projectile.velocity.Y * 0.2f + velOffset.Y), 100, new Color(255, 245, 198), 2f);
                dust.noGravity = true;
            }

            if (Main.rand.NextBool(6))
            {
                Vector2 offset = new Vector2(7, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(3, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(projectile.position.X, projectile.position.Y) + offset, DustID.GoldFlame, new Vector2(projectile.velocity.X * 0.2f + velOffset.X, projectile.velocity.Y * 0.2f + velOffset.Y), 100, new Color(255, 245, 198), 2f);
                dust.noGravity = true;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (projectile.ai[0] <= 42f)
                return false;
            return null;
        }

        public override bool CanHitPvp(Player target) => projectile.ai[0] > 42f;

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.localAI[0] = target.whoAmI;
        }

        public override bool PreKill(int timeLeft)
        {
            Player player = Main.player[projectile.owner];

            float numberOfDusts = 32f;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(15f, 0).RotatedBy(rot);
                Vector2 velOffset = new Vector2(12.5f, 0).RotatedBy(rot);
                int dust = Dust.NewDust(projectile.position + offset, projectile.width, projectile.height, 269, velOffset.X, velOffset.Y);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity = velOffset;
                Main.dust[dust].scale = 2.5f;
            }

            float distance = 168f;

            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC npc = Main.npc[k];
                Vector2 vec = npc.Center - projectile.Center;
                float distanceTo = (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
                if (distanceTo < distance && npc.CanBeChasedBy(projectile, false) && k != projectile.localAI[0])
                {
                    float alldamage = projectile.damage * 0.5f;
                    double damage = npc.StrikeNPC((int)alldamage, projectile.knockBack, projectile.velocity.X > 0f ? 1 : -1, true);
                    player.addDPS((int)damage);
                }
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = sourceRectangle.Size() / 2f;

            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                float rot = MathHelper.ToRadians(22.5f) * Math.Sign(projectile.velocity.X);
                Vector2 drawPos = projectile.oldPos[i] - Main.screenPosition + origin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((projectile.oldPos.Length - i) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(texture, drawPos, new Rectangle?(), color, projectile.rotation - i * rot, origin, projectile.scale, spriteEffects, 0f);
            }
            return false;
        }
    }
}
