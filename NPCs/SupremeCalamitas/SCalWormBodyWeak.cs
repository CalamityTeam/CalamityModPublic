using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.SupremeCalamitas
{
	public class SCalWormBodyWeak : ModNPC
	{
        public double damageTaken = 0.0;
        public int invinceTime = 360;

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimstone Heart");
		}
		
		public override void SetDefaults()
		{
			npc.damage = 0; //70
			npc.npcSlots = 5f;
			npc.width = 14; //324
			npc.height = 12; //216
			npc.defense = 0;
            npc.lifeMax = CalamityWorld.revenge ? 1200000 : 1000000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 2000000;
            }
            npc.aiStyle = 6; //new
            aiType = -1; //new
            animationType = 10; //new
			npc.knockBackResist = 0f;
			npc.scale = 1.2f;
			if (Main.expertMode)
			{
				npc.scale = 1.35f;
			}
			npc.alpha = 255;
            npc.chaseable = false;
            npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit13;
			npc.DeathSound = SoundID.NPCDeath13;
			npc.netAlways = true;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.dontCountMe = true;
		}

        public override void AI()
        {
            if (damageTaken >= (CalamityWorld.death ? 25000.0 : 10000.0))
            {
                npc.dontTakeDamage = true;
            }
            else
            {
                if (invinceTime > 0)
                {
                    invinceTime--;
                    npc.dontTakeDamage = true;
                }
                else
                {
                    npc.dontTakeDamage = false;
                }
            }
            if (Main.npc[(int)npc.ai[1]].alpha < 128)
            {
                npc.alpha -= 42;
                if (npc.alpha < 0)
                {
                    npc.alpha = 0;
                }
            }
            if (Main.netMode != 1)
            {
                npc.localAI[0] += 1f;
                if (npc.localAI[0] >= 900f)
                {
                    npc.localAI[0] = 0f;
                    int damage = Main.expertMode ? 150 : 200;
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 1f, 1f, mod.ProjectileType("BrimstoneBarrage"), damage, 0f, npc.target, 0f, 0f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, -1f, 1f, mod.ProjectileType("BrimstoneBarrage"), damage, 0f, npc.target, 0f, 0f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 1f, -1f, mod.ProjectileType("BrimstoneBarrage"), damage, 0f, npc.target, 0f, 0f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, -1f, -1f, mod.ProjectileType("BrimstoneBarrage"), damage, 0f, npc.target, 0f, 0f);
                    npc.netUpdate = true;
                }
            }
            if (!Main.npc[(int)npc.ai[1]].active)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.active = false;
            }
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (damage > npc.lifeMax / 25)
            {
                damage = 0;
                return false;
            }
            damageTaken += damage;
            if (crit)
            {
                damageTaken += damage;
            }
            return true;
        }

        public override bool CheckActive()
		{
			return false;
		}
		
		public override bool PreNPCLoot()
		{
			return false;
		}
	}
}