using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ValariBoomerang : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/FrostcrushValari";

        //This variable will be used for the stealth strike
        public float ReboundTime = 0f;
        public float timer = 0f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frostcrush Valari");
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 40;
            projectile.height = 40;
            projectile.penetrate = -1;
            projectile.timeLeft = 360;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 15;
            projectile.coldDamage = true;

            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            //Dust trail
            if (Main.rand.Next(5) == 0)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 67, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }

            //Constant rotation
            projectile.rotation += 0.2f;

            timer++;
            //Constant sound effects
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 15;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }
            //Slopes REEEEEEEEEEEE
            if (timer == 3f)
                projectile.tileCollide = true;
            //Decide the range of the boomerang depending on stealth
            if (projectile.Calamity().stealthStrike)
                ReboundTime = 27f;
            else
                ReboundTime = 55f;

            // ai[0] stores whether the boomerang is returning. If 0, it isn't. If 1, it is.
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[1] += 1f;
                if (projectile.ai[1] >= ReboundTime)
                {
                    projectile.ai[0] = 1f;
                    projectile.ai[1] = 0f;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                projectile.tileCollide = false;
                float returnSpeed = FrostcrushValari.Speed * 1.5f;
                float acceleration = 3.2f;
                Player owner = Main.player[projectile.owner];

                // Delete the boomerang if it's excessively far away.
                Vector2 playerCenter = owner.Center;
                float xDist = playerCenter.X - projectile.Center.X;
                float yDist = playerCenter.Y - projectile.Center.Y;
                float dist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                if (dist > 3000f)
                    projectile.Kill();

                dist = returnSpeed / dist;
                xDist *= dist;
                yDist *= dist;

                // Home back in on the player.
                if (projectile.velocity.X < xDist)
                {
                    projectile.velocity.X += acceleration;
                    if (projectile.velocity.X < 0f && xDist > 0f)
                        projectile.velocity.X += acceleration;
                }
                else if (projectile.velocity.X > xDist)
                {
                    projectile.velocity.X -= acceleration;
                    if (projectile.velocity.X > 0f && xDist < 0f)
                        projectile.velocity.X -= acceleration;
                }
                if (projectile.velocity.Y < yDist)
                {
                    projectile.velocity.Y += acceleration;
                    if (projectile.velocity.Y < 0f && yDist > 0f)
                        projectile.velocity.Y += acceleration;
                }
                else if (projectile.velocity.Y > yDist)
                {
                    projectile.velocity.Y -= acceleration;
                    if (projectile.velocity.Y > 0f && yDist < 0f)
                        projectile.velocity.Y -= acceleration;
                }


                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == projectile.owner)
                    if (projectile.Hitbox.Intersects(owner.Hitbox))
                        projectile.Kill();
            }
        }

        private void OnHitEffects()
        {
            //Start homing at player if you hit an enemy
            projectile.ai[0] = 1;

			int icicleAmt = Main.rand.Next(2, 4);
			if (projectile.owner == Main.myPlayer)
			{
				for (int i = 0; i < icicleAmt; i++)
				{
					Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
					int shard = Projectile.NewProjectile(projectile.Center, velocity, Main.rand.NextBool(2) ? ModContent.ProjectileType<Valaricicle>() : ModContent.ProjectileType<Valaricicle2>(), projectile.damage / 3, 0f, projectile.owner);
				}
			}
		}


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			OnHitEffects();
            target.AddBuff(BuffID.Frostburn, 120);
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);
			target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			OnHitEffects();
            target.AddBuff(BuffID.Frostburn, 120);
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);
			target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //Bounce off tiles and start homing on player if it hits a tile
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, projectile.position);
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            projectile.ai[0] = 1;
            return false;
        }
    }
}
