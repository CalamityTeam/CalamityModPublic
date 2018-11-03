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
	public class SCalWormHeart : ModNPC
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimstone Heart");
		}
		
		public override void SetDefaults()
		{
			npc.damage = 0; //70
			npc.width = 24; //324
			npc.height = 24; //216
			npc.defense = 0;
            npc.lifeMax = CalamityWorld.revenge ? 120000 : 100000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 150000;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 80000 : 60000;
            }
            npc.aiStyle = -1; //new
            aiType = -1; //new
			npc.knockBackResist = 0f;
			npc.alpha = 255;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit13;
			npc.DeathSound = SoundID.NPCDeath1;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
		}

        public override void AI()
        {
            if (!NPC.AnyNPCs(mod.NPCType("SupremeCalamitas")))
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }
            npc.alpha -= 42;
            if (npc.alpha < 0)
            {
                npc.alpha = 0;
            }
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.thrown && projectile.penetrate == -1)
            {
                damage = (int)((double)damage * 0.8);
            }
            else if (projectile.penetrate == -1) //not a minion and penetrate is infinite
            {
                damage = (int)((double)damage * 0.2);
            }
            else if (projectile.penetrate > 1) //not a minion, penetrate is not infinite, penetrate is greater than 3
            {
                damage = (int)((double)damage * 0.4);
            }
            else //not a minion, penetrate is not infinite, and penetrate is not greater than 1
            {
                projectile.penetrate = 1;
            }
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