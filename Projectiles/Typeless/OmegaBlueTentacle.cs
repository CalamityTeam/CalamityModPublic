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
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.timeLeft = 8;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool PreAI()
        {
            if (!initSegments)
            {
                initSegments = true;
                for (int i = 0; i < 6; i++)
                {
                    segment[i] = Projectile.Center;
                }
            }
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            bool hentai = player.Calamity().omegaBlueHentai;
            if (player.active && player.Calamity().omegaBlueSet)
                Projectile.timeLeft = 8;

            //tentacle head movement (homing)
            Vector2 playerVel = player.position - player.oldPosition;
            Projectile.position += playerVel;
            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 0f)
            {
                Vector2 home = player.Center + new Vector2(50, 0).RotatedBy(MathHelper.ToRadians(60) * Projectile.ai[1]);
                Vector2 distance = home - Projectile.Center;
                float range = distance.Length();
                distance.Normalize();
                if (Projectile.ai[0] == 0f)
                {
                    if (range > 13f)
                    {
                        Projectile.ai[0] = -1f; //if in fast mode, stay fast until back in range
                        if (range > 1300f)
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                    else
                    {
                        if (hentai)
                            Projectile.ai[0] = 120f;//45f + Main.rand.Next(45);
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 3f + Main.rand.NextFloat(3f);
                        Projectile.netUpdate = true;
                    }
                }
                else
                {
                    distance /= 8f;
                }
                if (range > 120f) //switch to fast return mode
                {
                    Projectile.ai[0] = -1f;
                    Projectile.netUpdate = true;
                }
                Projectile.velocity += distance;
                if (range > 30f)
                    Projectile.velocity *= 0.96f;

                if (Projectile.ai[0] > 120f) //attack nearby enemy
                {
                    Projectile.ai[0] = 10 + Main.rand.Next(10);
                    float maxDistance = hentai ? 900f : 600f;
                    int target = -1;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(Projectile))
                        {
                            float npcDistance = Projectile.Distance(npc.Center);
                            if (npcDistance < maxDistance)
                            {
                                maxDistance = npcDistance;
                                target = i;
                            }
                        }
                    }
                    if (target != -1)
                    {
                        Projectile.velocity = Main.npc[target].Center - Projectile.Center;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 13f;
                        Projectile.velocity += Main.npc[target].velocity / 2f;
                        Projectile.velocity -= playerVel / 2f;
                        Projectile.ai[0] *= -1f;
                    }
                    Projectile.netUpdate = true;
                }
            }

            //tentacle segment updates
            segment[0] = player.Center;
            for (int i = 1; i < 5; i++)
            {
                MoveSegment(segment[i - 1], ref segment[i], segment[i + 1]);
            }
            MoveSegment(segment[4], ref segment[5], Projectile.Center + Projectile.velocity);

            if (hentai)
            {
                if (Projectile.ai[0] != -1f)
                    Projectile.ai[0]++;
                Projectile.position += Projectile.velocity;
                //SMOOTH ASS DUST
                Vector2 dustPos = Projectile.position + Projectile.velocity;
                Vector2 tickVel = dustPos - Projectile.oldPosition; //playerVel + projectile.velocity * 2f;
                dustPos += new Vector2(Projectile.width / 2, 0).RotatedBy(Projectile.rotation);
                dustPos.X -= 4;
                dustPos.X += Projectile.width / 2;
                dustPos.Y -= 4;
                dustPos.Y += Projectile.height / 2;
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
            if (Main.player[Projectile.owner].Calamity().omegaBlueHentai)
                crit = true;
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            if (Main.player[Projectile.owner].Calamity().omegaBlueHentai)
                crit = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.owner == Main.myPlayer && Main.player[Projectile.owner].lifeSteal > 0f && !Main.player[Projectile.owner].moonLeech)
            {
                int healAmount = 10 * damage / Projectile.damage; //should always be around max, less if enemy has defense/DR
                if (healAmount > 0)
                {
                    Main.player[Projectile.owner].lifeSteal -= healAmount;
                    if (Main.player[Projectile.owner].Calamity().omegaBlueHentai) //hentai always crits, this makes it have same lifesteal delay
                        Main.player[Projectile.owner].lifeSteal += healAmount / 2;
                    /*Main.player[projectile.owner].statLife += healAmount;
                    if (Main.player[projectile.owner].statLife > Main.player[projectile.owner].statLifeMax2)
                        Main.player[projectile.owner].statLife = Main.player[projectile.owner].statLifeMax2;
                    Main.player[projectile.owner].HealEffect(healAmount, false);*/
                    Projectile.NewProjectile(Projectile.Center, Vector2.Zero, ProjectileID.SpiritHeal, 0, 0f, Projectile.owner, Projectile.owner, healAmount);
                }
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (Projectile.owner == Main.myPlayer && Main.player[Projectile.owner].lifeSteal > 0f && !Main.player[Projectile.owner].moonLeech)
            {
                int healAmount = 10 * damage / Projectile.damage; //should always be around max, less if enemy has defense/DR
                if (healAmount > 0)
                {
                    Main.player[Projectile.owner].lifeSteal -= healAmount;
                    if (Main.player[Projectile.owner].Calamity().omegaBlueHentai) //hentai always crits, this makes it have same lifesteal delay
                        Main.player[Projectile.owner].lifeSteal += healAmount / 2;
                    /*Main.player[projectile.owner].statLife += healAmount;
                    if (Main.player[projectile.owner].statLife > Main.player[projectile.owner].statLifeMax2)
                        Main.player[projectile.owner].statLife = Main.player[projectile.owner].statLifeMax2;
                    Main.player[projectile.owner].HealEffect(healAmount, false);*/
                    Projectile.NewProjectile(Projectile.Center, Vector2.Zero, ProjectileID.SpiritHeal, 0, 0f, Projectile.owner, Projectile.owner, healAmount);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            GameShaders.Armor.ApplySecondary(Main.player[Projectile.owner].cBody, Main.player[Projectile.owner], new DrawData?());
            Projectile.rotation = (Projectile.Center - segment[5]).ToRotation();
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D segmentSprite = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Chains/OmegaBlueTentacleSegment");
            for (int i = 0; i < 6; i++)
            {
                Main.spriteBatch.Draw(segmentSprite, segment[i] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), segmentSprite.Bounds, Projectile.GetAlpha(lightColor), 0f, segmentSprite.Bounds.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), texture2D13.Bounds, Projectile.GetAlpha(lightColor), Projectile.rotation, texture2D13.Bounds.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
