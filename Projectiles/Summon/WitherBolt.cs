using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class WitherBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wither Bolt");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 25;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.scale = 0.8f;
            projectile.width = projectile.height = (int)(72 * projectile.scale);
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 180;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = 2;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }
        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.DarkGreen.ToVector3() * 1.25f);
            float oldSpeed = projectile.velocity.Length();

            NPC potentialTarget = projectile.Center.MinionHoming(800f, Main.player[projectile.owner]);
            if (potentialTarget != null)
                projectile.velocity = (projectile.velocity * 3f + projectile.SafeDirectionTo(potentialTarget.Center) * oldSpeed) / 4f;
            projectile.velocity = projectile.velocity.SafeNormalize(Vector2.UnitY) * oldSpeed * 1.01f;

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
            projectile.Opacity = MathHelper.Lerp(projectile.Opacity, 0f, 0.25f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D boltTexture = Main.projectileTexture[projectile.type];
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                float completionRatio = i / (float)projectile.oldPos.Length;
                Color drawColor = Color.Lerp(lightColor, Color.Olive, 0.6f);
                drawColor = Color.Lerp(drawColor, Color.Black, completionRatio);
                drawColor = Color.Lerp(drawColor, Color.Transparent, completionRatio);

                Vector2 drawPosition = projectile.oldPos[i] + projectile.Size * 0.5f - Main.screenPosition;
                spriteBatch.Draw(boltTexture, drawPosition, null, projectile.GetAlpha(drawColor), projectile.oldRot[i], boltTexture.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (!Main.dedServ) 
            {
                for (int i = 0; i < projectile.oldPos.Length / 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        Dust plague = Dust.NewDustDirect(projectile.oldPos[i], projectile.width / 2, projectile.height / 2, 107);
                        plague.velocity = (projectile.oldRot[i] - MathHelper.PiOver2).ToRotationVector2() * 4.5f + Main.rand.NextVector2Circular(2f, 2f);
                        plague.color = Color.Olive;
                        plague.noGravity = true;
                    }
                }
            }

            if (Main.myPlayer == projectile.owner)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 spawnPosition = projectile.Center + Main.rand.NextVector2CircularEdge(8f, 8f);
                    int seeker = Projectile.NewProjectile(spawnPosition, Main.rand.NextVector2Circular(12f, 12f), ModContent.ProjectileType<PlagueSeeker>(), projectile.damage, projectile.knockBack, projectile.owner);
                    Main.projectile[seeker].Calamity().forceMinion = true;
                }
            }
        }
    }
}
