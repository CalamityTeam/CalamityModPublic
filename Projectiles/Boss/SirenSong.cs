using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class SirenSong : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Musical Note");
		}

		public override void SetDefaults()
		{
			projectile.width = 26;
			projectile.height = 58;
			projectile.hostile = true;
			projectile.penetrate = 1;
			projectile.timeLeft = 1800;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[0]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
		}

		public override void AI()
		{
			projectile.velocity.X *= 0.985f;
			projectile.velocity.Y *= 0.985f;
			if (projectile.localAI[0] == 0f)
			{
				projectile.scale += 0.01f;
				if (projectile.scale >= 1.1f)
				{
					projectile.localAI[0] = 1f;
				}
			}
			else if (projectile.localAI[0] == 1f)
			{
				projectile.scale -= 0.01f;
				if (projectile.scale <= 0.9f)
				{
					projectile.localAI[0] = 0f;
				}
			}
			if (projectile.ai[1] == 0f)
			{
				projectile.ai[1] = 1f;
				float soundPitch = (Main.rand.NextFloat() - 0.5f) * 0.5f;
				Main.harpNote = soundPitch;
				Main.PlaySound(SoundID.Item26, projectile.position);
			}
			Lighting.AddLight(projectile.Center, 0.7f, 0.5f, 0f);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 0);
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(BuffID.Confused, 120);
		}
	}
}
