using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class FinalDawnProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public const float MaxChargeTime = 20f;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            //projectile.extraUpdates = 2;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player is null || player.dead)
                Projectile.Kill();

            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.velocity = Main.MouseWorld - player.Center;
                Projectile.velocity.Normalize();
            }
            player.direction = Projectile.direction;
            player.heldProj = Projectile.whoAmI;
            Projectile.Center = player.Center;
            Projectile.position.Y -= 44;
            Projectile.position.X -= 46 * player.direction;
            player.bodyFrame.Y = player.bodyFrame.Height;

            if (Projectile.ai[0] < MaxChargeTime)
            {
                // Charging dust
                Vector2 dustCenter = new Vector2(Projectile.Center.X, Projectile.Center.Y - 40);
                int flame = Dust.NewDust(dustCenter, Projectile.width, Projectile.height, ModContent.DustType<FinalFlame>(), 0, 0, 100, default, 2f);
                Main.dust[flame].noGravity = true;
                Main.dust[flame].fadeIn = 1.5f;
                Main.dust[flame].scale = 1.4f;
                Vector2 offsetVector = Utils.NextVector2CircularEdge(Main.rand, 100f, 100f);
                Main.dust[flame].position = dustCenter - offsetVector;
                Vector2 newVelocity = dustCenter - Main.dust[flame].position;
                Main.dust[flame].velocity = newVelocity * 0.1f;
            }
            if (Projectile.ai[0] == MaxChargeTime)
            {
                int dustCount = 36;
                for (int i = 0; i < dustCount; i++)
                {
                    Vector2 startingPosition = Projectile.Center + new Vector2(0, -40);
                    Vector2 offset = Vector2.UnitX * Projectile.width * 0.1875f;
                    offset = offset.RotatedBy((i - (dustCount / 2 - 1)) * MathHelper.TwoPi / dustCount);
                    int dustIdx = Dust.NewDust(startingPosition + offset, 0, 0, ModContent.DustType<FinalFlame>(), offset.X * 2f, offset.Y * 2f, 100, default, 3.4f);
                    Main.dust[dustIdx].noGravity = true;
                    Main.dust[dustIdx].noLight = true;
                    Main.dust[dustIdx].velocity = Vector2.Normalize(offset) * 5f;
                }
                Projectile.frame = 1;
            }
            Projectile.ai[0]++;

            Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();

            if (Main.myPlayer == Projectile.owner)
            {
                if (!player.channel || player.noItems || player.CCed)
                {
                    AttemptExecuteAttacks(player);
                    Projectile.Kill();
                }
            }
        }

        public void AttemptExecuteAttacks(Player player)
        {
            //projectile.ai[1] == 1f if spawned via Venerated Locket
            if (Projectile.ai[0] >= MaxChargeTime && !player.noItems && !player.CCed)
            {
                // Far range attack
                if (player.controlUp)
                {
                    // Stealth Strike
                    if (player.Calamity().StealthStrikeAvailable() && Projectile.ai[1] != 1f)
                    {
                        int stealth = Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center,
                                                 player.SafeDirectionTo(Main.MouseWorld) * 28f,
                                                 ModContent.ProjectileType<FinalDawnThrow2>(),
                                                 (int)(Projectile.damage * 1f),
                                                 Projectile.knockBack,
                                                 Projectile.owner);
                        Main.projectile[stealth].Calamity().stealthStrike = true;
                        player.Calamity().ConsumeStealthByAttacking();
                        Main.player[Projectile.owner].immuneNoBlink = true;
                        Main.player[Projectile.owner].immuneTime += 20; //Adding iframes in case they get hit before the dash so those iframes are not wasted
                    }
                    else
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                                 Projectile.SafeDirectionTo(Main.MouseWorld) * 38f,
                                                 ModContent.ProjectileType<FinalDawnThrow>(), Projectile.damage,
                                                 Projectile.knockBack, Projectile.owner);
                        if (Projectile.ai[1] != 1f)
                            player.Calamity().ConsumeStealthByAttacking();
                    }
                }
                // Close range attack
                else
                {
                    // Stealth
                    if (player.Calamity().StealthStrikeAvailable() && Projectile.ai[1] != 1f)
                    {
                        int stealth = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                                 Projectile.velocity,
                                                 ModContent.ProjectileType<FinalDawnHorizontalSlash>(),
                                                 (int)(Projectile.damage * 1.275f),
                                                 Projectile.knockBack,
                                                 Projectile.owner);
                        Main.projectile[stealth].Calamity().stealthStrike = true;
                        player.Calamity().ConsumeStealthByAttacking();
                    }
                    else
                    {
                        //This one doesn't consume stealth since it replenishes stealth on hits
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                                 Projectile.velocity,
                                                 ModContent.ProjectileType<FinalDawnFireSlash>(),
                                                 Projectile.damage,
                                                 Projectile.knockBack,
                                                 Projectile.owner);
                        if (p.WithinBounds(Main.maxProjectiles) && Projectile.Calamity().LocketClone)
                            Main.projectile[p].Calamity().LocketClone = true; // NO STEALTH GEN OK?
                    }
                }
            }
            else
                player.Calamity().ConsumeStealthByAttacking();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D scytheTexture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/FinalDawnProjectile_Glow").Value;
            int height = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int yStart = height * Projectile.frame;
            Main.spriteBatch.Draw(scytheTexture,
                                  Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY,
                                  new Rectangle?(new Rectangle(0, yStart, scytheTexture.Width, height)),
                                  Projectile.GetAlpha(lightColor),
                                  Projectile.rotation,
                                  new Vector2(scytheTexture.Width / 2f, height / 2f),
                                  Projectile.scale,
                                  Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                  0f);
            Main.spriteBatch.Draw(glowmask,
                                  Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY,
                                  new Rectangle?(new Rectangle(0, yStart, scytheTexture.Width, height)),
                                  Projectile.GetAlpha(Color.White),
                                  Projectile.rotation,
                                  new Vector2(scytheTexture.Width / 2f, height / 2f),
                                  Projectile.scale,
                                  Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                  0f);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Dragonfire>(), 300);
        }
    }
}
