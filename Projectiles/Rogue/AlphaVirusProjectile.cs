using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.NPCs.StormWeaver;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Rogue
{
    public class AlphaVirusProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/AlphaVirus";

        public static int lifetime = 600;
        public static float finalVelocity = 2f;
        public static float decelerationRate = 0.07f;
        private const float radius = 100f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Alpha Virus");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 5;
            projectile.timeLeft = lifetime;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 15;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.rotation += 0.05f * projectile.direction;

            if (projectile.Calamity().stealthStrike)
            {
                if (projectile.ai[0] > finalVelocity)
                {
                    projectile.ai[0] -= decelerationRate;
                    if (projectile.ai[0] < finalVelocity)
                    {
                        projectile.ai[0] = finalVelocity;
                    }

                    projectile.velocity.Normalize();
                    projectile.velocity *= projectile.ai[0];
                }
                if (projectile.timeLeft < lifetime - 30 && projectile.timeLeft % 15 == 0 && projectile.ai[0] <= finalVelocity)
                {
                    int projID = ModContent.ProjectileType<AlphaSeeker>();
                    int damage = (int)(projectile.damage * 0.125f);
                    float kb = 1f;
                    float ai0 = 1f; // Stealth strike spawned seekers set ai[0] to 1, which makes them cling to the parent projectile.
                    Vector2 vel = Main.rand.NextVector2CircularEdge(5f, 5f);
                    Projectile.NewProjectile(projectile.Center, vel, projID, damage, kb, projectile.owner, ai0, projectile.identity);
                }
            }
            else
            {
                projectile.timeLeft--;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            float dist1 = Vector2.Distance(projectile.Center, target.Hitbox.TopLeft());
            float dist2 = Vector2.Distance(projectile.Center, target.Hitbox.TopRight());
            float dist3 = Vector2.Distance(projectile.Center, target.Hitbox.BottomLeft());
            float dist4 = Vector2.Distance(projectile.Center, target.Hitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            if (minDist <= projectile.width)
            {
                target.AddBuff(ModContent.BuffType<Plague>(), 120);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            float dist1 = Vector2.Distance(projectile.Center, target.Hitbox.TopLeft());
            float dist2 = Vector2.Distance(projectile.Center, target.Hitbox.TopRight());
            float dist3 = Vector2.Distance(projectile.Center, target.Hitbox.BottomLeft());
            float dist4 = Vector2.Distance(projectile.Center, target.Hitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            if (minDist <= projectile.width)
            {
                target.AddBuff(ModContent.BuffType<Plague>(), 120);
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.type == ModContent.NPCType<StormWeaverHead>() || target.type == ModContent.NPCType<StormWeaverBody>() || target.type == ModContent.NPCType<StormWeaverTail>())
            {
                damage /= 5;
            }

            float dist1 = Vector2.Distance(projectile.Center, target.Hitbox.TopLeft());
            float dist2 = Vector2.Distance(projectile.Center, target.Hitbox.TopRight());
            float dist3 = Vector2.Distance(projectile.Center, target.Hitbox.BottomLeft());
            float dist4 = Vector2.Distance(projectile.Center, target.Hitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            if (minDist > projectile.width)
            {
                damage /= 5;
                knockback = 0f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // Aura effect
            Texture2D aura = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/AlphaVirusAura");
            float scaleStep = 0.03f;
            float rotationOffset = 0.03f;
            float drawTransparency = 0.1f;

            if (projectile.timeLeft > lifetime - 10)
            {
                drawTransparency = (lifetime - projectile.timeLeft) * (drawTransparency / 10);
            }
            else if (projectile.timeLeft < 25)
            {
                drawTransparency = projectile.timeLeft * (drawTransparency / 25);
            }

            Color drawCol = Color.White;

            for (int i = 0; i < 10; i++)
            {
                spriteBatch.Draw(aura, projectile.Center - Main.screenPosition, null, drawCol * drawTransparency, -(projectile.rotation * 0.2f) + (rotationOffset * i * i), aura.Size() / 2f, projectile.scale - (i * scaleStep), SpriteEffects.None, 0f);
            }

            // Dust
            for (int i = 0; i < (lifetime - projectile.timeLeft) / 30; i++)
            {
                float min = projectile.width / 2;
                float max = radius;

                Vector2 pos = new Vector2(0f, -Main.rand.NextFloat(min, max));
                pos = pos.RotatedByRandom(MathHelper.TwoPi);
                Vector2 velocity = -pos * 0.02f;
                pos += projectile.Center;

                int dust = Dust.NewDust(pos, 1, 1, 89, 0f, 0f, 100, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity = velocity + projectile.velocity;
            }

            // Main sprite
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, projectile.position);
            projectile.Kill();
            return false;
        }

        public override void Kill(int timeLeft)
        {
            int numSeekers = 6;
            int damage = projectile.damage;
            float kb = 1f;
            float speed = 10f;
            for (int i = 0; i < numSeekers; i++)
            {
                Vector2 velocity = (MathHelper.Pi * i / 3f).ToRotationVector2() * speed;
                Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<AlphaSeeker>(), damage, kb, projectile.owner, 0f, 0f);
            }

            int numDust = 20;
            for (int i = 0; i < numDust; i++)
            {
                float min = projectile.width / 2;
                float max = radius;

                Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat(min, max));
                velocity = velocity.RotatedByRandom(MathHelper.TwoPi);
                velocity *= 0.1f;

                int dust = Dust.NewDust(projectile.Center, 1, 1, 89, 0f, 0f, 100, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity = velocity + projectile.velocity;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, radius, targetHitbox);
    }
}
