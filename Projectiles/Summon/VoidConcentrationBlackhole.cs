using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    class VoidConcentrationBlackhole : ModProjectile
    {
        private int damage = 0;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(damage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            damage = reader.ReadInt32();
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Black Hole");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 80; //32 base, 2.5x the normal (for hitbox purposes)
            projectile.height = 85; //34 base, 2.5x the normal (for hitbox purposes)
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 1800;
            projectile.penetrate = -1;

            projectile.tileCollide = false;
            projectile.minion = true;
            projectile.scale = 0.01f;
        }

        private void ApplySucc(NPC npc)
        {
            float succStrength = 4f / projectile.scale;
            succStrength *= projectile.scale;
            Vector2 velocity = projectile.Center - npc.Center;
            velocity *= 2f;
            velocity.SafeNormalize(Vector2.Zero);
            float num550 = 5f * projectile.scale;
            Vector2 vector43 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num551 = projectile.Center.X - vector43.X;
            float num552 = projectile.Center.Y - vector43.Y;
            float num553 = (float)Math.Sqrt((double)(num551 * num551 + num552 * num552));
            if (num553 < 100f)
            {
                num550 = 28f; //14
            }
            num553 = num550 / num553;
            num551 *= num553;
            num552 *= num553;
            npc.velocity.X = (velocity.X * 15f + num551) / 16f;
            npc.velocity.Y = (velocity.Y * 15f + num552) / 16f;
            npc.velocity = (velocity / succStrength);
        }
        
        private void Death()
        {
            if (projectile.scale >= 2.5f) //it's boom o' clock
            {
                projectile.height *= 2;
                projectile.width *= 2;
                projectile.maxPenetrate = -1;
                projectile.penetrate = -1;
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = 10;
                projectile.damage = damage;
                projectile.Damage();
                projectile.friendly = false;
                projectile.Damage();
                Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 14);
                projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int d = 0; d < 6; d++)
            {
                int shadow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 27, 0f, 0f, 100, new Color(0, 0, 0), 4f);
                Main.dust[shadow].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[shadow].scale = 0.5f;
                    Main.dust[shadow].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 18; d++)
            {
                int shadow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 27, 0f, 0f, 100, new Color(0, 0, 0), 4f);
                Main.dust[shadow].noGravity = true;
                Main.dust[shadow].velocity *= 5f;
                shadow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 27, 0f, 0f, 100, new Color(0, 0, 0), 3f);
                Main.dust[shadow].velocity *= 2f;
            }
        }

        public override bool PreAI()
        {   
            if (damage == 0)
            {
                damage = projectile.damage;
                projectile.damage = 0;
                projectile.position = Main.player[projectile.owner].position;
            }
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 drawPos = projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
            int height = texture.Height / Main.projFrames[projectile.type];
            int frameHeight = height * projectile.frame;
            Rectangle rectangle = new Rectangle(0, frameHeight, texture.Width, height);
            Vector2 origin = new Vector2(texture.Width / 2f, height / 2f);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(rectangle), lightColor, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override void AI()
        {

            projectile.scale += 0.01f;
            if (projectile.scale % 0.02f == 0)
            {
                projectile.scale += 0.01f; //speeds up the projectile's growth to prevent it being too long
            }
            int radius = (int)(projectile.scale * 100f); //0.01f; to a max of 2.5f; so radius of 1 to 250 by default
            if (projectile.scale >= 2f)
                radius *= 2; 

            int baseWidth = 32;
            int baseHeight = 34;
            int newWidth = (int)(baseWidth * projectile.scale);
            int newHeight = (int)(baseHeight * projectile.scale);
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, newWidth, newHeight);

            

            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type] - 1)
            {
                projectile.frame = 0;
            }
            projectile.frameCounter++;

                for (int i = 0; i < Main.npc.Length; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].friendly /*&& CalamityGlobalNPC.ShouldAffectNPC(Main.npc[i])*/) //TODO - REMOVE COMMENT BEFORE MERGE
                    {
                        if (Vector2.Distance(projectile.Center, Main.npc[i].Center) <= radius)
                            ApplySucc(Main.npc[i]);
                    }
                }
            

            if (projectile.scale >= 2.5f) //it's boom o' clock
            {
                Death();
            }
        }
    }
}
