using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SlimeGod
{
    public class CorruptSlimeSpawn2 : ModNPC
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.CorruptSlimeSpawn.DisplayName");
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            Main.npcFrameCount[NPC.type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = NPCAIStyleID.Slime;
            NPC.GetNPCDamage();
            NPC.width = 40;
            NPC.height = 30;
            if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                NPC.scale = 2f;

            NPC.defense = 4;
            NPC.lifeMax = BossRushEvent.BossRushActive ? 5000 : (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 180 : 90;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            AnimationType = NPCID.CorruptSlime;
            NPC.Opacity = 0.8f;
            NPC.lavaImmune = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            Color dustColor = Color.Lavender;
            dustColor.A = 150;
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hit.HitDirection, -1f, NPC.alpha, dustColor, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hit.HitDirection, -1f, NPC.alpha, dustColor, 1f);
                }
            }
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.rand.NextBool(8) && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart); 
            
            if (Main.zenithWorld && Main.rand.NextBool(5))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 valueBoom = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                    float spreadBoom = 15f * 0.0174f;
                    double startAngleBoom = Math.Atan2(NPC.velocity.X, NPC.velocity.Y) - spreadBoom / 2;
                    double deltaAngleBoom = spreadBoom / 8f;
                    double offsetAngleBoom;
                    int iBoom;
                    int damageBoom = 30;
                    for (iBoom = 0; iBoom < 5; iBoom++)
                    {
                        int projectileType = ModContent.ProjectileType<UnstableEbonianGlob>();
                        offsetAngleBoom = startAngleBoom + deltaAngleBoom * (iBoom + iBoom * iBoom) / 2f + 32f * iBoom;
                        float velocityfactor = 0.3f;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), valueBoom.X, valueBoom.Y, (float)(Math.Sin(offsetAngleBoom) * velocityfactor), (float)(Math.Cos(offsetAngleBoom) * velocityfactor), projectileType, damageBoom, 0f, Main.myPlayer, 0f, 0f);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), valueBoom.X, valueBoom.Y, (float)(-Math.Sin(offsetAngleBoom) * velocityfactor), (float)(-Math.Cos(offsetAngleBoom) * velocityfactor), projectileType, damageBoom, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ItemID.Vitamins, 50);

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Weak, 90, true);
        }
    }
}
