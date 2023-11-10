using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.NPCs.PlagueEnemies
{
    public class Viruling : ModNPC
    {
        public override void SetStaticDefaults()
        {
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

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundJungle,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Viruling")
            });
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
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead)
            {
                NPC.TargetClosest(true);
            }
            float maxSpeed = 6f;
            float acceleration = 0.05f;
            if (CalamityWorld.revenge)
            {
                maxSpeed *= 1.25f;
                acceleration *= 1.25f;
            }
            if (CalamityWorld.death)
            {
                maxSpeed *= 1.25f;
                acceleration *= 1.25f;
            }
            Vector2 vector = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float targetXDist = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2);
            float targetYDist = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2);
            targetXDist = (float)((int)(targetXDist / 8f) * 8);
            targetYDist = (float)((int)(targetYDist / 8f) * 8);
            vector.X = (float)((int)(vector.X / 8f) * 8);
            vector.Y = (float)((int)(vector.Y / 8f) * 8);
            targetXDist -= vector.X;
            targetYDist -= vector.Y;
            float targetDistance = (float)Math.Sqrt((double)(targetXDist * targetXDist + targetYDist * targetYDist));
            if (targetDistance == 0f)
            {
                targetXDist = NPC.velocity.X;
                targetYDist = NPC.velocity.Y;
            }
            else
            {
                targetDistance = maxSpeed / targetDistance;
                targetXDist *= targetDistance;
                targetYDist *= targetDistance;
            }
            if (Main.player[NPC.target].dead)
            {
                targetXDist = (float)NPC.direction * maxSpeed / 2f;
                targetYDist = -maxSpeed / 2f;
            }
            if (NPC.velocity.X < targetXDist)
            {
                NPC.velocity.X = NPC.velocity.X + acceleration;
            }
            else if (NPC.velocity.X > targetXDist)
            {
                NPC.velocity.X = NPC.velocity.X - acceleration;
            }
            if (NPC.velocity.Y < targetYDist)
            {
                NPC.velocity.Y = NPC.velocity.Y + acceleration;
            }
            else if (NPC.velocity.Y > targetYDist)
            {
                NPC.velocity.Y = NPC.velocity.Y - acceleration;
            }
            if (targetXDist > 0f)
            {
                NPC.spriteDirection = 1;
                NPC.rotation = (float)Math.Atan2((double)targetYDist, (double)targetXDist);
            }
            else if (targetXDist < 0f)
            {
                NPC.spriteDirection = -1;
                NPC.rotation = (float)Math.Atan2((double)targetYDist, (double)targetXDist) + 3.14f;
            }
            float recoilSpeed = 0.7f;
            if (NPC.collideX)
            {
                NPC.netUpdate = true;
                NPC.velocity.X = NPC.oldVelocity.X * -recoilSpeed;
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
                NPC.velocity.Y = NPC.oldVelocity.Y * -recoilSpeed;
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
            int idleDust = Dust.NewDust(new Vector2(NPC.position.X - NPC.velocity.X, NPC.position.Y - NPC.velocity.Y), NPC.width, NPC.height, 46, NPC.velocity.X * 0.2f, NPC.velocity.Y * 0.2f, 100, default, 2f);
            Dust dust = Main.dust[idleDust];
            dust.noGravity = true;
            dust.velocity.X *= 0.3f;
            dust.velocity.Y *= 0.3f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !NPC.downedGolemBoss || spawnInfo.Player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.HardmodeJungle.Chance * 0.09f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Plague>(), 90, true);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Plague, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.PlagueBoomSound, NPC.Center);
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Plague, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Viruling").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Viruling2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Viruling3").Type, 1f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ModContent.ItemType<PlagueCellCanister>(), 1, 1, 2);
    }
}
