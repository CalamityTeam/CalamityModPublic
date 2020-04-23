using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.HiveMind
{
    public class DankCreeper : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dank Creeper");
        }

        public override void SetDefaults()
        {
            npc.damage = 25;
            npc.width = 70;
            npc.height = 70;
            npc.defense = 6;
            npc.lifeMax = 45;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 10000;
            }
            npc.aiStyle = -1;
            aiType = -1;
            animationType = 10;
            npc.knockBackResist = CalamityWorld.bossRushActive ? 0f : 0.3f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
        }

        public override void AI()
        {
            bool revenge = CalamityWorld.revenge;
            float speed = revenge ? 12f : 11f;
            if (CalamityWorld.bossRushActive)
                speed = 18f;
            CalamityAI.CryocoreAI(npc, mod, speed);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<MarkedforDeath>(), 120);
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 13, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 13, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/HiveMindGores/DankCreeperGore"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/HiveMindGores/DankCreeperGore2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/HiveMindGores/DankCreeperGore3"), 1f);
            }
        }

        public override void NPCLoot()
        {
            if (Main.expertMode && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, 0f, ModContent.ProjectileType<ShadeNimbusHostile>(), 14, 0f, Main.myPlayer, 0f, 0f);
            }
        }
    }
}
