using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            npc.damage = 0;
            npc.npcSlots = 5f;
            npc.width = 14;
            npc.height = 12;
            npc.lifeMax = CalamityWorld.revenge ? 1200000 : 1000000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 2000000;
            }
            npc.aiStyle = 6;
            aiType = -1;
            npc.knockBackResist = 0f;
            CalamityGlobalNPC global = npc.Calamity();
            global.DR = 0.999999f;
            global.unbreakableDR = true;
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
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit13;
            npc.DeathSound = SoundID.NPCDeath13;
            npc.netAlways = true;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.dontCountMe = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (Main.npc[(int)npc.ai[1]].alpha < 128)
            {
                npc.alpha -= 42;
                if (npc.alpha < 0)
                {
                    npc.alpha = 0;
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] += 1f;
                if (npc.localAI[0] >= 900f)
                {
                    npc.localAI[0] = 0f;
                    int damage = Main.expertMode ? 150 : 200;
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 1f, 1f, ModContent.ProjectileType<BrimstoneBarrage>(), damage, 0f, npc.target, 0f, 0f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, -1f, 1f, ModContent.ProjectileType<BrimstoneBarrage>(), damage, 0f, npc.target, 0f, 0f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 1f, -1f, ModContent.ProjectileType<BrimstoneBarrage>(), damage, 0f, npc.target, 0f, 0f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, -1f, -1f, ModContent.ProjectileType<BrimstoneBarrage>(), damage, 0f, npc.target, 0f, 0f);
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

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
			if (CalamityMod.projectileDestroyExceptionList.TrueForAll(x => projectile.type != x))
			{
				if (projectile.penetrate == -1 && !projectile.minion)
				{
					projectile.penetrate = 1;
				}
				else if (projectile.penetrate >= 1)
				{
					projectile.penetrate = 1;
				}
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
