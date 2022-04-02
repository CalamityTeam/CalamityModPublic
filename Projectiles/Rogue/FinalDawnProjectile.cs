using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class FinalDawnProjectile : ModProjectile
    {
        public const float MaxChargeTime = 20f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Final Dawn");
            Main.projFrames[projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = false;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            //projectile.extraUpdates = 2;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            if (player is null || player.dead)
                projectile.Kill();

            if (Main.myPlayer == player.whoAmI)
            {
                projectile.velocity = Main.MouseWorld - player.Center;
                projectile.velocity.Normalize();
            }
            player.direction = projectile.direction;
            player.heldProj = projectile.whoAmI;
            projectile.Center = player.Center;
            projectile.position.Y -= 44;
            projectile.position.X -= 46 * player.direction;
            player.bodyFrame.Y = player.bodyFrame.Height;

            if (projectile.ai[0] < MaxChargeTime)
            {
                // Charging dust
                Vector2 dustCenter = new Vector2(projectile.Center.X, projectile.Center.Y - 40);
                int flame = Dust.NewDust(dustCenter, projectile.width, projectile.height, ModContent.DustType<FinalFlame>(), 0, 0, 100, default, 2f);
                Main.dust[flame].noGravity = true;
                Main.dust[flame].fadeIn = 1.5f;
                Main.dust[flame].scale = 1.4f;
                Vector2 offsetVector = Utils.NextVector2CircularEdge(Main.rand, 100f, 100f);
                Main.dust[flame].position = dustCenter - offsetVector;
                Vector2 newVelocity = dustCenter - Main.dust[flame].position;
                Main.dust[flame].velocity = newVelocity * 0.1f;
            }
            if (projectile.ai[0] == MaxChargeTime)
            {
                int dustCount = 36;
                for (int i = 0; i < dustCount; i++)
                {
                    Vector2 startingPosition = projectile.Center + new Vector2(0, -40);
                    Vector2 offset = Vector2.UnitX * projectile.width * 0.1875f;
                    offset = offset.RotatedBy((i - (dustCount / 2 - 1)) * MathHelper.TwoPi / dustCount);
                    int dustIdx = Dust.NewDust(startingPosition + offset, 0, 0, ModContent.DustType<FinalFlame>(), offset.X * 2f, offset.Y * 2f, 100, default, 3.4f);
                    Main.dust[dustIdx].noGravity = true;
                    Main.dust[dustIdx].noLight = true;
                    Main.dust[dustIdx].velocity = Vector2.Normalize(offset) * 5f;
                }
                projectile.frame = 1;
            }
            projectile.ai[0]++;

            projectile.spriteDirection = (projectile.velocity.X > 0).ToDirectionInt();

            if (Main.myPlayer == projectile.owner)
            {
                if (!player.channel || player.noItems || player.CCed)
                {
                    AttemptExecuteAttacks(player);
                    projectile.Kill();
                }
            }
        }

        public void AttemptExecuteAttacks(Player player)
        {
            //projectile.ai[1] == 1f if spawned via Venerated Locket
            if (projectile.ai[0] >= MaxChargeTime && !player.noItems && !player.CCed)
            {
                // Far range attack
                if (player.controlUp)
                {
                    // Stealth Strike
                    if (player.Calamity().StealthStrikeAvailable() && projectile.ai[1] != 1f)
                    {
                        int stealth = Projectile.NewProjectile(player.Center,
                                                 player.SafeDirectionTo(Main.MouseWorld) * 38f,
                                                 ModContent.ProjectileType<FinalDawnThrow2>(),
                                                 (int)(projectile.damage * 1.05f),
                                                 projectile.knockBack,
                                                 projectile.owner);
                        Main.projectile[stealth].Calamity().stealthStrike = true;
                        player.Calamity().ConsumeStealthByAttacking();
                    }
                    else
                    {
                        Projectile.NewProjectile(projectile.Center,
                                                 projectile.SafeDirectionTo(Main.MouseWorld) * 38f,
                                                 ModContent.ProjectileType<FinalDawnThrow>(), projectile.damage,
                                                 projectile.knockBack, projectile.owner);
                        if (projectile.ai[1] != 1f)
                            player.Calamity().ConsumeStealthByAttacking();
                    }
                }
                // Close range attack
                else
                {
                    // Stealth
                    if (player.Calamity().StealthStrikeAvailable() && projectile.ai[1] != 1f)
                    {
                        int stealth = Projectile.NewProjectile(projectile.Center,
                                                 projectile.velocity,
                                                 ModContent.ProjectileType<FinalDawnHorizontalSlash>(),
                                                 (int)(projectile.damage * 1.275f),
                                                 projectile.knockBack,
                                                 projectile.owner);
                        Main.projectile[stealth].Calamity().stealthStrike = true;
                        player.Calamity().ConsumeStealthByAttacking();
                    }
                    else
                    {
                        //This one doesn't consume stealth since it replenishes stealth on hits
                        Projectile.NewProjectile(projectile.Center,
                                                 projectile.velocity,
                                                 ModContent.ProjectileType<FinalDawnFireSlash>(),
                                                 projectile.damage,
                                                 projectile.knockBack,
                                                 projectile.owner);
                    }
                }
            }
            else
                player.Calamity().ConsumeStealthByAttacking();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D scytheTexture = Main.projectileTexture[projectile.type];
            Texture2D glowmask = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/FinalDawnProjectile_Glow");
            int height = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int yStart = height * projectile.frame;
            Main.spriteBatch.Draw(scytheTexture,
                                  projectile.Center - Main.screenPosition + Vector2.UnitY * projectile.gfxOffY,
                                  new Rectangle?(new Rectangle(0, yStart, scytheTexture.Width, height)),
                                  projectile.GetAlpha(lightColor),
                                  projectile.rotation,
                                  new Vector2(scytheTexture.Width / 2f, height / 2f),
                                  projectile.scale,
                                  projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                  0f);
            Main.spriteBatch.Draw(glowmask,
                                  projectile.Center - Main.screenPosition + Vector2.UnitY * projectile.gfxOffY,
                                  new Rectangle?(new Rectangle(0, yStart, scytheTexture.Width, height)),
                                  projectile.GetAlpha(Color.White),
                                  projectile.rotation,
                                  new Vector2(scytheTexture.Width / 2f, height / 2f),
                                  projectile.scale,
                                  projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                  0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
        }
    }
}