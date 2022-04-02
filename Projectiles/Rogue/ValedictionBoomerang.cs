using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ValedictionBoomerang : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Valediction";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Valediction");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 70;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 15;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 8;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[1] += 1f;
                if (projectile.ai[1] >= 60f)
                {
                    projectile.ai[0] = 1f;
                    projectile.ai[1] = 0f;
                    projectile.netUpdate = true;
                }
                else
                {
                    CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 200f, 12f, 20f);
                }
            }
            else
            {
                float returnSpeed = 30f;
                float acceleration = 5f;
                Vector2 projVector = player.Center - projectile.Center;
                float playerDist = projVector.Length();
                if (playerDist > 3000f)
                {
                    projectile.Kill();
                }
                playerDist = returnSpeed / playerDist;
                projVector.X *= playerDist;
                projVector.Y *= playerDist;
                if (projectile.velocity.X < projVector.X)
                {
                    projectile.velocity.X += acceleration;
                    if (projectile.velocity.X < 0f && projVector.X > 0f)
                    {
                        projectile.velocity.X += acceleration;
                    }
                }
                else if (projectile.velocity.X > projVector.X)
                {
                    projectile.velocity.X -= acceleration;
                    if (projectile.velocity.X > 0f && projVector.X < 0f)
                    {
                        projectile.velocity.X -= acceleration;
                    }
                }
                if (projectile.velocity.Y < projVector.Y)
                {
                    projectile.velocity.Y += acceleration;
                    if (projectile.velocity.Y < 0f && projVector.Y > 0f)
                    {
                        projectile.velocity.Y += acceleration;
                    }
                }
                else if (projectile.velocity.Y > projVector.Y)
                {
                    projectile.velocity.Y -= acceleration;
                    if (projectile.velocity.Y > 0f && projVector.Y < 0f)
                    {
                        projectile.velocity.Y -= acceleration;
                    }
                }
                if (Main.myPlayer == projectile.owner)
                {
                    Rectangle projHitbox = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
                    Rectangle playerHitbox = new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height);
                    if (projHitbox.Intersects(playerHitbox))
                    {
                        projectile.Kill();
                    }
                }
            }
            projectile.rotation += 0.5f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
            OnHitEffects();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
            OnHitEffects();
        }

        private void OnHitEffects()
        {
            int typhoonAmt = 3;
            int typhoonDamage = (int)(projectile.damage * 0.3f);
            if (projectile.owner == Main.myPlayer && projectile.numHits < 1 && projectile.Calamity().stealthStrike)
            {
                Main.PlaySound(SoundID.Item84, projectile.position);
                for (int typhoonCount = 0; typhoonCount < typhoonAmt; typhoonCount++)
                {
                    Vector2 velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (velocity.X == 0f && velocity.Y == 0f)
                    {
                        velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    velocity.Normalize();
                    velocity *= (float)Main.rand.Next(70, 101) * 0.1f;
                    Projectile typhoon = Projectile.NewProjectileDirect(projectile.Center, velocity, ModContent.ProjectileType<NuclearFuryProjectile>(), typhoonDamage, 0f, projectile.owner);
                    if (typhoon.whoAmI.WithinBounds(Main.maxProjectiles))
                    {
                        typhoon.Calamity().forceRogue = true;
                        typhoon.usesLocalNPCImmunity = true;
                        typhoon.localNPCHitCooldown = 10;
                    }
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item21, projectile.position);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 100;
            projectile.Center = projectile.position;
            for (int d = 0; d < 5; d++)
            {
                int water = Dust.NewDust(projectile.position, projectile.width, projectile.height, 33, 0f, 0f, 100, default, 2f);
                Main.dust[water].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[water].scale = 0.5f;
                    Main.dust[water].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 8; d++)
            {
                int water = Dust.NewDust(projectile.position, projectile.width, projectile.height, 33, 0f, 0f, 100, default, 3f);
                Main.dust[water].noGravity = true;
                Main.dust[water].velocity *= 5f;
                water = Dust.NewDust(projectile.position, projectile.width, projectile.height, 33, 0f, 0f, 100, default, 2f);
                Main.dust[water].velocity *= 2f;
            }
        }
    }
}
