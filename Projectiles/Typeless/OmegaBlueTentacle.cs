using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class OmegaBlueTentacle : ModProjectile
    {
        public bool initSegments = false;
        public Vector2[] segment = new Vector2[6];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omega Blue Tentacle");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.timeLeft = 8;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override bool PreAI()
        {
            if (!initSegments)
            {
                initSegments = true;
                for (int i = 0; i < 6; i++)
                {
                    segment[i] = projectile.Center;
                }
            }
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            bool hentai = player.Calamity().omegaBlueHentai;
            if (player.active && player.Calamity().omegaBlueSet)
                projectile.timeLeft = 8;

            //tentacle head movement (homing)
            Vector2 playerVel = player.position - player.oldPosition;
            projectile.position += playerVel;
            projectile.ai[0]++;
            if (projectile.ai[0] >= 0f)
            {
                Vector2 home = player.Center + new Vector2(50, 0).RotatedBy(MathHelper.ToRadians(60) * projectile.ai[1]);
                Vector2 distance = home - projectile.Center;
                float range = distance.Length();
                distance.Normalize();
                if (projectile.ai[0] == 0f)
                {
                    if (range > 13f)
                    {
                        projectile.ai[0] = -1f; //if in fast mode, stay fast until back in range
                        if (range > 1300f)
                        {
                            projectile.Kill();
                            return;
                        }
                    }
                    else
                    {
                        if (hentai)
                            projectile.ai[0] = 120f;//45f + Main.rand.Next(45);
                        projectile.velocity.Normalize();
                        projectile.velocity *= 3f + Main.rand.NextFloat(3f);
                        projectile.netUpdate = true;
                    }
                }
                else
                {
                    distance /= 8f;
                }
                if (range > 120f) //switch to fast return mode
                {
                    projectile.ai[0] = -1f;
                    projectile.netUpdate = true;
                }
                projectile.velocity += distance;
                if (range > 30f)
                    projectile.velocity *= 0.96f;

                if (projectile.ai[0] > 120f) //attack nearby enemy
                {
                    projectile.ai[0] = 10 + Main.rand.Next(10);
                    float maxDistance = hentai ? 900f : 600f;
                    int target = -1;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(projectile))
                        {
                            float npcDistance = projectile.Distance(npc.Center);
                            if (npcDistance < maxDistance)
                            {
                                maxDistance = npcDistance;
                                target = i;
                            }
                        }
                    }
                    if (target != -1)
                    {
                        projectile.velocity = Main.npc[target].Center - projectile.Center;
                        projectile.velocity.Normalize();
                        projectile.velocity *= 13f;
                        projectile.velocity += Main.npc[target].velocity / 2f;
                        projectile.velocity -= playerVel / 2f;
                        projectile.ai[0] *= -1f;
                    }
                    projectile.netUpdate = true;
                }
            }

            //tentacle segment updates
            segment[0] = player.Center;
            for (int i = 1; i < 5; i++)
            {
                MoveSegment(segment[i - 1], ref segment[i], segment[i + 1]);
            }
            MoveSegment(segment[4], ref segment[5], projectile.Center + projectile.velocity);

            if (hentai)
            {
                if (projectile.ai[0] != -1f)
                    projectile.ai[0]++;
                projectile.position += projectile.velocity;
                //SMOOTH ASS DUST
                Vector2 dustPos = projectile.position + projectile.velocity;
                Vector2 tickVel = dustPos - projectile.oldPosition; //playerVel + projectile.velocity * 2f;
                dustPos += new Vector2(projectile.width / 2, 0).RotatedBy(projectile.rotation);
                dustPos.X -= 4;
                dustPos.X += projectile.width / 2;
                dustPos.Y -= 4;
                dustPos.Y += projectile.height / 2;
                const float factor = 3f;
                int limit = (int)(tickVel.Length() / factor);
                if (limit == 0)
                {
                    int d = Dust.NewDust(dustPos, 0, 0, 20, 0, 0, 100, Color.Transparent, 0.9f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].noLight = true;
                    Main.dust[d].fadeIn = 1f;
                    Main.dust[d].velocity = Vector2.Zero;
                }
                else
                {
                    tickVel.Normalize();
                    tickVel *= factor;
                    for (int i = 0; i <= limit; i++)
                    {
                        int d = Dust.NewDust(dustPos, 0, 0, 20, 0, 0, 100, Color.Transparent, 0.9f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].fadeIn = 1f;
                        Main.dust[d].position -= tickVel * i;
                        Main.dust[d].velocity = Vector2.Zero;
                    }
                }
            }
        }

        private static void MoveSegment(Vector2 previous, ref Vector2 current, Vector2 next)
        {
            current = previous + next;
            current /= 2;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Main.player[projectile.owner].Calamity().omegaBlueHentai)
                crit = true;
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (Main.player[projectile.owner].Calamity().omegaBlueHentai)
                crit = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].lifeSteal > 0f && !Main.player[projectile.owner].moonLeech)
            {
                int healAmount = 10 * damage / projectile.damage; //should always be around max, less if enemy has defense/DR
                if (healAmount > 0)
                {
                    Main.player[projectile.owner].lifeSteal -= healAmount;
                    if (Main.player[projectile.owner].Calamity().omegaBlueHentai) //hentai always crits, this makes it have same lifesteal delay
                        Main.player[projectile.owner].lifeSteal += healAmount / 2;
                    /*Main.player[projectile.owner].statLife += healAmount;
                    if (Main.player[projectile.owner].statLife > Main.player[projectile.owner].statLifeMax2)
                        Main.player[projectile.owner].statLife = Main.player[projectile.owner].statLifeMax2;
                    Main.player[projectile.owner].HealEffect(healAmount, false);*/
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileID.SpiritHeal, 0, 0f, projectile.owner, projectile.owner, healAmount);
                }
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].lifeSteal > 0f && !Main.player[projectile.owner].moonLeech)
            {
                int healAmount = 10 * damage / projectile.damage; //should always be around max, less if enemy has defense/DR
                if (healAmount > 0)
                {
                    Main.player[projectile.owner].lifeSteal -= healAmount;
                    if (Main.player[projectile.owner].Calamity().omegaBlueHentai) //hentai always crits, this makes it have same lifesteal delay
                        Main.player[projectile.owner].lifeSteal += healAmount / 2;
                    /*Main.player[projectile.owner].statLife += healAmount;
                    if (Main.player[projectile.owner].statLife > Main.player[projectile.owner].statLifeMax2)
                        Main.player[projectile.owner].statLife = Main.player[projectile.owner].statLifeMax2;
                    Main.player[projectile.owner].HealEffect(healAmount, false);*/
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileID.SpiritHeal, 0, 0f, projectile.owner, projectile.owner, healAmount);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            GameShaders.Armor.ApplySecondary(Main.player[projectile.owner].cBody, Main.player[projectile.owner], new DrawData?());
            projectile.rotation = (projectile.Center - segment[5]).ToRotation();
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            Texture2D segmentSprite = ModContent.GetTexture("CalamityMod/ExtraTextures/Chains/OmegaBlueTentacleSegment");
            for (int i = 0; i < 6; i++)
            {
                Main.spriteBatch.Draw(segmentSprite, segment[i] - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), segmentSprite.Bounds, projectile.GetAlpha(lightColor), 0f, segmentSprite.Bounds.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), texture2D13.Bounds, projectile.GetAlpha(lightColor), projectile.rotation, texture2D13.Bounds.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
