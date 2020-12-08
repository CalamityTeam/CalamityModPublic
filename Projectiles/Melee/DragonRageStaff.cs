using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class DragonRageStaff : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Rage");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 158;
			projectile.scale = 2.2f;
            projectile.melee = true;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.hide = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            float lifeSpan = 50f;

			// If the player is dead, destroy the projectile
            if (player.dead)
            {
                projectile.Kill();
                return;
            }

            int direction = Math.Sign(projectile.velocity.X);
            projectile.velocity = new Vector2(direction, 0f);

			// Initial Rotation
            if (projectile.ai[0] == 0f)
            {
                projectile.rotation = new Vector2(direction, -player.gravDir).ToRotation() + MathHelper.ToRadians(135f);
                if (projectile.velocity.X < 0f)
                {
                    projectile.rotation -= MathHelper.PiOver2;
                }
            }
			
            projectile.ai[0] += 1f;
            projectile.rotation += MathHelper.TwoPi * 2f / lifeSpan * (float)direction;
            bool halfWay = projectile.ai[0] == lifeSpan / 2f;
            if (projectile.ai[0] >= lifeSpan || (halfWay && !player.controlUseItem))
            {
                projectile.Kill();
                player.reuseDelay = 2;
            }
            else if (halfWay)
            {
                int direction2 = (player.DirectionTo(Main.MouseWorld).X > 0f) ? 1 : -1;
                if ((float)direction2 != projectile.velocity.X)
                {
                    player.ChangeDir(direction2);
                    projectile.velocity = new Vector2(direction2, 0f);
                    projectile.netUpdate = true;
                    projectile.rotation -= MathHelper.Pi;
                }
            }
            if ((projectile.ai[0] == 1f || (halfWay && projectile.active)) && projectile.owner == Main.myPlayer)
            {
                Vector2 mouse = player.DirectionTo(Main.MouseWorld) * 0f;
                player.DirectionTo(mouse);
            }

			SpawnDust(player, direction);
			PositionAndRotation(player);
			VisibilityAndLight();
        }

		private void SpawnDust(Player player, int direction)
		{
            float num12 = projectile.rotation - MathHelper.PiOver4 * (float)direction;
            Vector2 value3 = projectile.Center + (num12 + ((direction == -1) ? MathHelper.Pi : 0f)).ToRotationVector2() * 30f;
            Vector2 vector2 = num12.ToRotationVector2();
            Vector2 value4 = vector2.RotatedBy((double)(MathHelper.PiOver2 * projectile.spriteDirection), default);
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(value3 - new Vector2(5f), 10, 10, 244, player.velocity.X, player.velocity.Y, 150, default, 1f);
                dust.velocity = projectile.DirectionTo(dust.position) * 0.1f + dust.velocity * 0.1f;
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
                    Dust dust = Dust.NewDustDirect(projectile.position, 0, 0, 244, 0f, 0f, 100, default, 1f);
                    dust.position = projectile.Center + vector2 * (60f + Main.rand.NextFloat() * 20f) * dustVelMult2;
                    dust.velocity = value4 * (4f + 4f * Main.rand.NextFloat()) * dustVelMult2 * dustVelMult;
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
            projectile.Center = plrCtr + offset;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
            player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;
            player.itemRotation = MathHelper.WrapAngle(projectile.rotation);
		}

		private void VisibilityAndLight()
		{
            Lighting.AddLight(projectile.Center, 1.45f, 1.22f, 0.58f);
            projectile.alpha -= 128;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
		}

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
			Player player = Main.player[projectile.owner];
            Rectangle myRect = projectile.Hitbox;
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
					NPC npc = Main.npc[i];
					bool voodooDolls = projectile.owner < Main.maxPlayers && (npc.type == NPCID.Guide && player.killGuide || npc.type == NPCID.Clothier && player.killClothier);
					bool friendlyProjs = projectile.friendly && (!npc.friendly || voodooDolls);
					bool hostileProjs = projectile.hostile && npc.friendly && !npc.dontTakeDamageFromHostiles;
                    if (npc.active && !npc.dontTakeDamage && (friendlyProjs || hostileProjs) && (projectile.owner < 0 || npc.immune[projectile.owner] == 0 || projectile.maxPenetrate == 1))
                    {
                        if (npc.noTileCollide || !projectile.ownerHitCheck || projectile.CanHit(npc))
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
                                canHit = projectile.Colliding(myRect, rect);
                            }
                            else
                            {
                                canHit = projectile.Colliding(myRect, rect);
                            }
                            if (canHit)
                            {
                                hitDirection = (player.Center.X < npc.Center.X) ? 1 : -1;
                            }
                        }
                    }
                }
            }
        }

        public override void CutTiles()
        {
            float num5 = 60f;
            float f = projectile.rotation - MathHelper.PiOver4 * (float)Math.Sign(projectile.velocity.X);
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(projectile.Center + f.ToRotationVector2() * -num5, projectile.Center + f.ToRotationVector2() * num5, (float)projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CutTiles));
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float f = projectile.rotation - MathHelper.PiOver4 * (float)Math.Sign(projectile.velocity.X);
            float num2 = 0f;
            float num3 = 110f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center + f.ToRotationVector2() * -num3, projectile.Center + f.ToRotationVector2() * num3, 23f * projectile.scale, ref num2))
            {
                return true;
            }
            return false;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D tex = Main.projectileTexture[projectile.type];
			Vector2 drawPos = projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
			Rectangle rectangle = new Rectangle(0, 0, tex.Width, tex.Height);
			Vector2 origin = tex.Size() / 2f;
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(tex, drawPos, new Microsoft.Xna.Framework.Rectangle?(rectangle), lightColor, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);
			return false;
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Daybreak, 360);
			OnHitEffects(target.Center);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			OnHitEffects(target.Center);
        }

		private void OnHitEffects(Vector2 position)
		{
			if (projectile.owner == Main.myPlayer)
			{
				int proj = Projectile.NewProjectile(position, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
				Main.projectile[proj].Calamity().forceMelee = true;

				Vector2 velocity = position - projectile.Center;
				velocity.Normalize();
				velocity *= 6f;
				velocity.X += Main.rand.NextFloat(-1.75f, 1.75f);
				velocity.Y += Main.rand.NextFloat(-1.75f, 1.75f);
				Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<DragonRageProj>(), (int)(projectile.damage * 0.5), projectile.knockBack * 0.5f, projectile.owner);
			}
		}
    }
}
