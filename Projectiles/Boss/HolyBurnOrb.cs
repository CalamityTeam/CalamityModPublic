using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Events;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyBurnOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Orb");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 30;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
			projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 200;
        }

        public override void AI()
        {
			if (projectile.ai[0] < 240f)
			{
				projectile.ai[0] += 1f;

				if (projectile.timeLeft < 160)
					projectile.timeLeft = 160;
			}

			if (projectile.velocity.Length() < 16f)
				projectile.velocity *= 1.01f;

			int index = Player.FindClosest(projectile.Center, projectile.width, projectile.height);
			Player player = Main.player[index];
			if (player is null || player.Calamity().lol)
				return;

			float playerDist = Vector2.Distance(player.Center, projectile.Center);
            if (playerDist < 50f && !player.dead && projectile.position.X < player.position.X + player.width && projectile.position.X + projectile.width > player.position.X && projectile.position.Y < player.Bottom.Y && projectile.Bottom.Y > player.position.Y)
            {
                int dmgAmt = (int)projectile.ai[1];
                player.HealEffect(dmgAmt, false);
                player.statLife += dmgAmt;
                if (player.statLife > player.statLifeMax2)
                {
                    player.statLife = player.statLifeMax2;
                }
                if (player.statLife < 0 || CalamityWorld.armageddon)
                {
                    player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " burst into sinless ash."), 1000.0, 0, false);
                }
                NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, index, dmgAmt);
                projectile.Kill();
            }
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			float lerpMult = CalamityUtils.GetLerpValue(15f, 30f, projectile.timeLeft, clamped: true) * CalamityUtils.GetLerpValue(240f, 200f, projectile.timeLeft, clamped: true) * (1f + 0.2f * (float)Math.Cos(Main.GlobalTime % 30f / 0.5f * (MathHelper.Pi * 2f) * 3f)) * 0.8f;

			Texture2D texture = Main.projectileTexture[projectile.type];
			Vector2 drawPos = projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
			Color baseColor = Main.dayTime ? new Color(255, 200, 100, 255) : new Color(100, 200, 255, 255);
			baseColor *= 0.5f;
			baseColor.A = 0;
			Color colorA = baseColor;
			Color colorB = baseColor * 0.5f;
			colorA *= lerpMult;
			colorB *= lerpMult;
			Vector2 origin = texture.Size() / 2f;
			Vector2 scale = new Vector2(0.5f, 1.5f) * lerpMult;

			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(texture, drawPos, null, colorA, MathHelper.PiOver2, origin, scale, spriteEffects, 0);
			spriteBatch.Draw(texture, drawPos, null, colorA, 0f, origin, scale, spriteEffects, 0);
			spriteBatch.Draw(texture, drawPos, null, colorB, MathHelper.PiOver2, origin, scale * 0.6f, spriteEffects, 0);
			spriteBatch.Draw(texture, drawPos, null, colorB, 0f, origin, scale * 0.6f, spriteEffects, 0);

			spriteBatch.Draw(texture, drawPos, null, colorA, MathHelper.PiOver4, origin, scale * 0.6f, spriteEffects, 0);
			spriteBatch.Draw(texture, drawPos, null, colorA, MathHelper.PiOver4 * 3f, origin, scale * 0.6f, spriteEffects, 0);
			spriteBatch.Draw(texture, drawPos, null, colorB, MathHelper.PiOver4, origin, scale * 0.36f, spriteEffects, 0);
			spriteBatch.Draw(texture, drawPos, null, colorB, MathHelper.PiOver4 * 3f, origin, scale * 0.36f, spriteEffects, 0);

			return false;
		}

		public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
			CalamityGlobalProjectile.ExpandHitboxBy(projectile, 50);
			int dustType = Main.dayTime ? (int)CalamityDusts.ProfanedFire : (int)CalamityDusts.Nightwither;
			for (int d = 0; d < 10; d++)
            {
                int holy = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 2f);
                Main.dust[holy].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[holy].scale = 0.5f;
                    Main.dust[holy].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 15; d++)
            {
                int fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 3f);
                Main.dust[fire].noGravity = true;
                Main.dust[fire].velocity *= 5f;
                fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 2f);
                Main.dust[fire].velocity *= 2f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			int buffType = Main.dayTime ? ModContent.BuffType<HolyFlames>() : ModContent.BuffType<Nightwither>();
			target.AddBuff(buffType, 60);
		}
    }
}
