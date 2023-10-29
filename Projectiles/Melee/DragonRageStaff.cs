using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class DragonRageStaff : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<DragonRage>();
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 408;
            Projectile.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90000;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float spinCycleTime = 50f;

            // If the player is dead, destroy the projectile
            if (player.dead || !player.channel)
            {
                Projectile.Kill();
                player.reuseDelay = 2;
                return;
            }

            int direction = Math.Sign(Projectile.velocity.X);
            Projectile.velocity = new Vector2(direction, 0f);

            // Initial Rotation
            if (Projectile.ai[0] == 0f)
            {
                Projectile.rotation = new Vector2(direction, -player.gravDir).ToRotation() + MathHelper.ToRadians(135f);
                if (Projectile.velocity.X < 0f)
                {
                    Projectile.rotation -= MathHelper.PiOver2;
                }
            }

            Projectile.ai[0] += 1f;
            Projectile.rotation += MathHelper.TwoPi * 2f / spinCycleTime * (float)direction;
            int expectedDirection = (player.SafeDirectionTo(Main.MouseWorld).X > 0f).ToDirectionInt();
            if (Projectile.ai[0] % spinCycleTime > spinCycleTime * 0.5f && expectedDirection != Projectile.velocity.X)
            {
                player.ChangeDir(expectedDirection);
                Projectile.velocity = Vector2.UnitX * expectedDirection;
                Projectile.rotation -= MathHelper.Pi;
                Projectile.netUpdate = true;
            }
            SpawnDust(player, direction);
            PositionAndRotation(player);
            VisibilityAndLight();
        }

        private void SpawnDust(Player player, int direction)
        {
            float rotateDirection = Projectile.rotation - MathHelper.PiOver4 * direction;
            Vector2 dustSpawn = Projectile.Center + (rotateDirection + ((direction == -1) ? MathHelper.Pi : 0f)).ToRotationVector2() * 30f;
            Vector2 staffTipDirection = rotateDirection.ToRotationVector2();
            Vector2 tipDustDirection = staffTipDirection.RotatedBy(MathHelper.PiOver2 * Projectile.spriteDirection);

            if (Main.rand.NextBool())
            {
                Dust dust = Dust.NewDustDirect(dustSpawn - new Vector2(5f), 10, 10, 244, player.velocity.X, player.velocity.Y, 150, default, 1f);
                dust.velocity = Projectile.SafeDirectionTo(dust.position) * 0.1f + dust.velocity * 0.1f;
            }
            for (int j = 0; j < 4; j++)
            {
                float dustVelMult = 1f;
                float dustVelMult2 = 1f;
                switch (j)
                {
                    case 1:
                        dustVelMult2 = -1f;
                        break;
                    case 2:
                        dustVelMult2 = 1.25f;
                        dustVelMult = 0.5f;
                        break;
                    case 3:
                        dustVelMult2 = -1.25f;
                        dustVelMult = 0.5f;
                        break;
                }
                if (Main.rand.Next(6) != 0)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, 0, 0, 244, 0f, 0f, 100, default, 1f);
                    dust.position = Projectile.Center + staffTipDirection * (60f + Main.rand.NextFloat() * 20f) * dustVelMult2;
                    dust.velocity = tipDustDirection * (4f + 4f * Main.rand.NextFloat()) * dustVelMult2 * dustVelMult;
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.scale = 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        dust.noGravity = false;
                    }
                }
            }
        }

        private void PositionAndRotation(Player player)
        {
            Vector2 plrCtr = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 offset = Vector2.Zero;
            Projectile.Center = plrCtr + offset;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;
            player.itemRotation = MathHelper.WrapAngle(Projectile.rotation);
        }

        private void VisibilityAndLight()
        {
            Lighting.AddLight(Projectile.Center, 1.45f, 1.22f, 0.58f);
            Projectile.alpha -= 128;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Dragonfire>(), 180);
            OnHitEffects(target.Center);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            OnHitEffects(target.Center);
        }

        private void OnHitEffects(Vector2 position)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                CalamityPlayer modPlayer = Main.player[Projectile.owner].Calamity();
                modPlayer.dragonRageHits++;
                if (modPlayer.dragonRageHits > 10 && modPlayer.dragonRageCooldown <= 0)
                {
                    SpawnFireballs();
                    modPlayer.dragonRageHits = 0;
                }

                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), Projectile.damage / 4, Projectile.knockBack, Projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                Main.projectile[proj].DamageType = DamageClass.Melee;
            }
        }

        private void SpawnFireballs()
        {
            // 10 to 15 fireballs
            int fireballAmt = Main.rand.Next(10, 16);
            for (int i = 0; i < fireballAmt; i++)
            {
                float angleStep = MathHelper.TwoPi / fireballAmt;
                float speed = 20f;
                Vector2 velocity = new Vector2(0f, speed);
                velocity = velocity.RotatedBy(angleStep * i * Main.rand.NextFloat(0.9f, 1.1f));

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<DragonRageFireball>(), Projectile.damage / 8, Projectile.knockBack / 3f, Projectile.owner);
            }
            Main.player[Projectile.owner].Calamity().dragonRageCooldown = 60;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Projectile.owner];
            Rectangle myRect = Projectile.Hitbox;
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    bool voodooDolls = Projectile.owner < Main.maxPlayers && (npc.type == NPCID.Guide && player.killGuide || npc.type == NPCID.Clothier && player.killClothier);
                    bool friendlyProjs = Projectile.friendly && (!npc.friendly || voodooDolls);
                    bool hostileProjs = Projectile.hostile && npc.friendly && !npc.dontTakeDamageFromHostiles;
                    if (npc.active && !npc.dontTakeDamage && (friendlyProjs || hostileProjs) && (Projectile.owner < 0 || npc.immune[Projectile.owner] == 0 || Projectile.maxPenetrate == 1))
                    {
                        if (npc.noTileCollide || !Projectile.ownerHitCheck || (ProjectileLoader.CanHitNPC(Projectile, npc) ?? false))
                        {
                            bool canHit;
                            Rectangle rect = npc.Hitbox;
                            if (npc.type == NPCID.SolarCrawltipedeTail)
                            {
                                int offset = 8;
                                rect.X -= offset;
                                rect.Y -= offset;
                                rect.Width += offset * 2;
                                rect.Height += offset * 2;
                                canHit = Projectile.Colliding(myRect, rect);
                            }
                            else
                            {
                                canHit = Projectile.Colliding(myRect, rect);
                            }
                            if (canHit)
                            {
                                modifiers.HitDirectionOverride = (player.Center.X < npc.Center.X) ? 1 : -1;
                            }
                        }
                    }
                }
            }
        }

        public override void CutTiles()
        {
            float staffRadius = 60f;
            float spinning = Projectile.rotation - MathHelper.PiOver4 * (float)Math.Sign(Projectile.velocity.X);
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(Projectile.Center + spinning.ToRotationVector2() * -staffRadius, Projectile.Center + spinning.ToRotationVector2() * staffRadius, (float)Projectile.width * Projectile.scale, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float spinning = Projectile.rotation - MathHelper.PiOver4 * (float)Math.Sign(Projectile.velocity.X);
            float staffRadiusHit = 110f;
            float useless = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + spinning.ToRotationVector2() * -staffRadiusHit, Projectile.Center + spinning.ToRotationVector2() * staffRadiusHit, 23f * Projectile.scale, ref useless))
            {
                return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Rectangle rectangle = new Rectangle(0, 0, tex.Width, tex.Height);
            Vector2 origin = tex.Size() / 2f;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(tex, drawPos, new Microsoft.Xna.Framework.Rectangle?(rectangle), lightColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
