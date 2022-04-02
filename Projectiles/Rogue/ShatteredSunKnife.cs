using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ShatteredSunKnife : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ShatteredSun";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shattered Sun");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 56;
            projectile.height = 56;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 4;
            projectile.timeLeft = 300;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 15;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);
            projectile.ai[1] += 1f;
            if (projectile.ai[1] < 5f)
            {
                projectile.alpha -= 50;
            }
            if (projectile.ai[1] == 5f)
            {
                projectile.alpha = 0;
                projectile.tileCollide = false;
            }

            if (projectile.ai[1] == 20f)
            {
                int numProj = 5;
                if (projectile.owner == Main.myPlayer)
                {
                    int spread = 6;
                    int projID = ModContent.ProjectileType<ShatteredSunScorchedBlade>();
                    int splitDamage = (int)(0.75f * projectile.damage);
                    float splitKB = 1f;
                    for (int i = 0; i < numProj; i++)
                    {
                        Vector2 perturbedspeed = new Vector2(projectile.velocity.X, projectile.velocity.Y + Main.rand.Next(-3, 4)).RotatedBy(MathHelper.ToRadians(spread));
                        int proj = Projectile.NewProjectile(projectile.Center, perturbedspeed * 0.2f, projID, splitDamage, splitKB, projectile.owner, 0f, 0f);
                        Main.projectile[proj].Calamity().stealthStrike = projectile.Calamity().stealthStrike;
                        spread -= Main.rand.Next(2, 6);
                    }
                    Main.PlaySound(SoundID.Item27, projectile.position);
                    projectile.active = false;
                    for (int num621 = 0; num621 < 8; num621++)
                    {
                        int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 127, 0f, 0f, 100, default, 2f);
                        Main.dust[num622].velocity *= 3f;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int num623 = 0; num623 < 16; num623++)
                    {
                        int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 127, 0f, 0f, 100, default, 3f);
                        Main.dust[num624].noGravity = true;
                        Main.dust[num624].velocity *= 5f;
                        num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 127, 0f, 0f, 100, default, 2f);
                        Main.dust[num624].velocity *= 2f;
                    }
                }
            }

            if (projectile.Calamity().stealthStrike)
            {
                float num472 = projectile.Center.X;
                float num473 = projectile.Center.Y;
                float num474 = 600f;
                for (int num475 = 0; num475 < Main.maxNPCs; num475++)
                {
                    if (Main.npc[num475].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1) && !CalamityPlayer.areThereAnyDamnBosses)
                    {
                        float npcCenterX = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
                        float npcCenterY = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
                        float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - npcCenterX) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - npcCenterY);
                        if (num478 < num474)
                        {
                            if (Main.npc[num475].position.X < num472)
                            {
                                Main.npc[num475].velocity.X += 0.25f;
                            }
                            else
                            {
                                Main.npc[num475].velocity.X -= 0.25f;
                            }
                            if (Main.npc[num475].position.Y < num473)
                            {
                                Main.npc[num475].velocity.Y += 0.25f;
                            }
                            else
                            {
                                Main.npc[num475].velocity.Y -= 0.25f;
                            }
                        }
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        private void ShatteredExplosion()
        {
            int projID = ModContent.ProjectileType<ShatteredExplosion>();
            int explosionDamage = (int)(projectile.damage * 0.45f);
            float explosionKB = 3f;
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, projID, explosionDamage, explosionKB, projectile.owner, 0f, 0f);
            Main.PlaySound(SoundID.Item14, projectile.position);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => ShatteredExplosion();

        public override void OnHitPvp(Player target, int damage, bool crit) => ShatteredExplosion();

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            ShatteredExplosion();
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.position);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 246, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 0, default, 1f);
            }
        }
    }
}
