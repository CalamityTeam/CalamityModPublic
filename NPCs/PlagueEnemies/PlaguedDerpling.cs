using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.PlagueEnemies
{
    public class PlaguedDerpling : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Viruling");
            Main.npcFrameCount[NPC.type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 60;
            NPC.width = 58;
            NPC.height = 44;
            NPC.defense = 18;
            NPC.lifeMax = 400;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.HitSound = SoundID.NPCHit22;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<VirulingBanner>();
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead)
            {
                NPC.TargetClosest(true);
            }
            float num = 6f; //2
            float num2 = 0.05f;
            if (CalamityWorld.death)
            {
                num *= 1.5f;
                num2 *= 1.5f;
            }
            Vector2 vector = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float num4 = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2);
            float num5 = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2);
            num4 = (float)((int)(num4 / 8f) * 8);
            num5 = (float)((int)(num5 / 8f) * 8);
            vector.X = (float)((int)(vector.X / 8f) * 8);
            vector.Y = (float)((int)(vector.Y / 8f) * 8);
            num4 -= vector.X;
            num5 -= vector.Y;
            float num6 = (float)Math.Sqrt((double)(num4 * num4 + num5 * num5));
            if (num6 == 0f)
            {
                num4 = NPC.velocity.X;
                num5 = NPC.velocity.Y;
            }
            else
            {
                num6 = num / num6;
                num4 *= num6;
                num5 *= num6;
            }
            if (Main.player[NPC.target].dead)
            {
                num4 = (float)NPC.direction * num / 2f;
                num5 = -num / 2f;
            }
            if (NPC.velocity.X < num4)
            {
                NPC.velocity.X = NPC.velocity.X + num2;
            }
            else if (NPC.velocity.X > num4)
            {
                NPC.velocity.X = NPC.velocity.X - num2;
            }
            if (NPC.velocity.Y < num5)
            {
                NPC.velocity.Y = NPC.velocity.Y + num2;
            }
            else if (NPC.velocity.Y > num5)
            {
                NPC.velocity.Y = NPC.velocity.Y - num2;
            }
            if (num4 > 0f)
            {
                NPC.spriteDirection = 1;
                NPC.rotation = (float)Math.Atan2((double)num5, (double)num4);
            }
            else if (num4 < 0f)
            {
                NPC.spriteDirection = -1;
                NPC.rotation = (float)Math.Atan2((double)num5, (double)num4) + 3.14f;
            }
            float num12 = 0.7f;
            if (NPC.collideX)
            {
                NPC.netUpdate = true;
                NPC.velocity.X = NPC.oldVelocity.X * -num12;
                if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 2f)
                {
                    NPC.velocity.X = 2f;
                }
                if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -2f)
                {
                    NPC.velocity.X = -2f;
                }
            }
            if (NPC.collideY)
            {
                NPC.netUpdate = true;
                NPC.velocity.Y = NPC.oldVelocity.Y * -num12;
                if (NPC.velocity.Y > 0f && (double)NPC.velocity.Y < 1.5)
                {
                    NPC.velocity.Y = 2f;
                }
                if (NPC.velocity.Y < 0f && (double)NPC.velocity.Y > -1.5)
                {
                    NPC.velocity.Y = -2f;
                }
            }
            if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
            {
                NPC.netUpdate = true;
            }
            int num13 = Dust.NewDust(new Vector2(NPC.position.X - NPC.velocity.X, NPC.position.Y - NPC.velocity.Y), NPC.width, NPC.height, 46, NPC.velocity.X * 0.2f, NPC.velocity.Y * 0.2f, 100, default, 2f);
            Dust dust = Main.dust[num13];
            dust.noGravity = true;
            dust.velocity.X *= 0.3f;
            dust.velocity.Y *= 0.3f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !NPC.downedGolemBoss || spawnInfo.Player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.HardmodeJungle.Chance * 0.09f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Plague>(), 180, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Plague, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                SoundEngine.PlaySound(Mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/PlagueSounds/PlagueBoom" + Main.rand.Next(1, 5)), NPC.Center);
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Plague, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/Viruling"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/Viruling2"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/Viruling3"), 1f);
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(NPC, ModContent.ItemType<PlagueCellCluster>(), 1, 2);
        }
    }
}
