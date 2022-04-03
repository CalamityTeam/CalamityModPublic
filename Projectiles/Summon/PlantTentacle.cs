using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class PlantTentacle : ModProjectile
    {
        private bool initialized = false;
        private int counter = 0;
        private float desiredDistance = 150f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tentacle");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 24;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 12;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[1] < 0 || Projectile.ai[1] >= Main.maxProjectiles)
            {
                Projectile.Kill();
                return;
            }
            // If something has gone wrong with either the tentacle or the host plant, destroy the proj.
            Projectile hostPlant = Main.projectile[(int)Projectile.ai[1]];
            if (Projectile.type != ModContent.ProjectileType<PlantTentacle>() || !hostPlant.active || hostPlant.type != ModContent.ProjectileType<PlantSummon>())
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.frameCounter++ % 6 == 0)
            {
                Projectile.frame++;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            if (!initialized)
            {
                Projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = Projectile.damage;
                initialized = true;
            }
            if (player.MinionDamage() != Projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    Projectile.Calamity().spawnedPlayerMinionDamageValue * player.MinionDamage());
                Projectile.damage = trueDamage;
            }

            if (player.Calamity().plantera)
            {
                Projectile.timeLeft = 2;
            }

            Projectile.rotation = (Projectile.Center - hostPlant.Center).ToRotation() + MathHelper.Pi;

            Projectile.MinionAntiClump(1f);

            counter++;
            if (counter % 30 == 0) //changes every half second
            {
                desiredDistance = Main.rand.NextFloat(175f, 225f);
            }
            Vector2 targetPos = hostPlant.Center - Projectile.Center;
            float targetDist = targetPos.Length();
            targetPos.Normalize();
            if (targetDist > desiredDistance)
            {
                float speedMult = 10f;
                speedMult += (targetDist - desiredDistance) * 0.2f; //+0.2f for every 1f away from the desired distance
                targetPos *= speedMult;
                Projectile.velocity = (Projectile.velocity * 40f + targetPos) / 41f;
            }
            else
            {
                float reverseSpeedMult = 5f;
                targetPos *= -reverseSpeedMult;
                Projectile.velocity = (Projectile.velocity * 40f + targetPos) / 41f;
            }

            if (targetDist > 2000f)
            {
                Projectile.position.X = hostPlant.Center.X - (float)(Projectile.width / 2);
                Projectile.position.Y = hostPlant.Center.Y - (float)(Projectile.height / 2);
                Projectile.netUpdate = true;
            }

            if (Projectile.ai[0] >= 3f)
            {
                if (hostPlant.Center.X - Projectile.Center.X > 0)
                {
                    Projectile.velocity.X += 0.05f;
                }
            }
            else
            {
                if (hostPlant.Center.X - Projectile.Center.X < 0)
                {
                    Projectile.velocity.X -= 0.05f;
                }
            }

            if (Projectile.ai[0] % 2f == 0f)
            {
                if (hostPlant.Center.Y - Projectile.Center.Y > 0)
                {
                    Projectile.velocity.Y += 0.05f;
                }
            }
            else
            {
                if (hostPlant.Center.Y - Projectile.Center.Y < 0)
                {
                    Projectile.velocity.Y -= 0.05f;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 107);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (Projectile.ai[1] < 0 || Projectile.ai[1] >= Main.maxProjectiles)
            {
                return false;
            }

            // If something has gone wrong with either the tentacle or the host plant, return.
            Projectile hostPlant = Main.projectile[(int)Projectile.ai[1]];
            if (Projectile.type != ModContent.ProjectileType<PlantTentacle>() || !hostPlant.active || hostPlant.type != ModContent.ProjectileType<PlantSummon>())
            {
                return false;
            }

            Vector2 source = hostPlant.Center;
            Color transparent = Microsoft.Xna.Framework.Color.Transparent;
            Texture2D chain = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Chains/PlantationChain");
            Vector2 goal = Projectile.Center;
            Rectangle? sourceRectangle = null;
            float textureHeight = (float)chain.Height;
            Vector2 drawVector = source - goal;
            float rotation = (float)Math.Atan2((double)drawVector.Y, (double)drawVector.X) - MathHelper.PiOver2;
            bool shouldDraw = true;
            if (float.IsNaN(goal.X) && float.IsNaN(goal.Y))
            {
                shouldDraw = false;
            }
            if (float.IsNaN(drawVector.X) && float.IsNaN(drawVector.Y))
            {
                shouldDraw = false;
            }
            while (shouldDraw)
            {
                if (drawVector.Length() < textureHeight + 1f)
                {
                    shouldDraw = false;
                }
                else
                {
                    Vector2 value2 = drawVector;
                    value2.Normalize();
                    goal += value2 * textureHeight;
                    drawVector = source - goal;
                    Color color = Lighting.GetColor((int)goal.X / 16, (int)(goal.Y / 16f));
                    Main.spriteBatch.Draw(chain, goal - Main.screenPosition, sourceRectangle, color, rotation, chain.Size() / 2f, 1f, SpriteEffects.None, 0f);
                }
            }
            return true;
        }

        //It draws the host plant in here in order to have it draw over the tentacles
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Only 1 tentacle needs to draw this, the last one spawned because it's latest in the projectile array.
            if (Projectile.ai[0] < 5)
            {
                return;
            }

            if (Projectile.ai[1] < 0 || Projectile.ai[1] >= Main.maxProjectiles)
            {
                return;
            }

            // If something has gone wrong with either the tentacle or the host plant, return.
            Projectile hostPlant = Main.projectile[(int)Projectile.ai[1]];
            if (Projectile.type != ModContent.ProjectileType<PlantTentacle>() || !hostPlant.active || hostPlant.type != ModContent.ProjectileType<PlantSummon>())
            {
                return;
            }

            Texture2D texture = Main.projectileTexture[hostPlant.type];
            int height = texture.Height / Main.projFrames[hostPlant.type];
            int frameHeight = height * hostPlant.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (hostPlant.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Color color = Lighting.GetColor((int)hostPlant.Center.X / 16, (int)(hostPlant.Center.Y / 16f));

            Main.spriteBatch.Draw(texture, hostPlant.Center - Main.screenPosition + new Vector2(0f, hostPlant.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), color, hostPlant.rotation, new Vector2((float)texture.Width / 2f, (float)height / 2f), hostPlant.scale, spriteEffects, 0f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 120);
            target.AddBuff(BuffID.Venom, 60);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 120);
            target.AddBuff(BuffID.Venom, 60);
        }
    }
}
