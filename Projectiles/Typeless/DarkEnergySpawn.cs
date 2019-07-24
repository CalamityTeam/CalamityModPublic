using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
	public class DarkEnergySpawn : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spawn");
		}
    	
		public override void SetDefaults()
		{
			projectile.width = 6;
			projectile.height = 6;
			projectile.aiStyle = 1;
			projectile.scale = 1f;
			projectile.penetrate = 1;
			projectile.timeLeft = 20;
			projectile.tileCollide = false;
			aiType = ProjectileID.Bullet;
		}
		
		public override void AI()
		{
			Player player = Main.player[projectile.owner];
			projectile.ai[1]++;
			
			if (projectile.ai[1] >= 0)
			{
				NPC.NewNPC((int)projectile.Center.X - 200, (int)projectile.Center.Y - 200, mod.NPCType("DarkEnergy"));
				NPC.NewNPC((int)projectile.Center.X + 200, (int)projectile.Center.Y - 200, mod.NPCType("DarkEnergy2"));
				NPC.NewNPC((int)projectile.Center.X, (int)projectile.Center.Y + 200, mod.NPCType("DarkEnergy3"));
				projectile.ai[1] = -30;
			}
		}
	}
}