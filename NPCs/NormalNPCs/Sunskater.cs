using System;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class Sunskater : ModNPC
    {
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/Sunskater") { Volume = 0.9f};

        private bool hasBeenHit = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0);
        }

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.damage = 20;
            NPC.width = 58;
            NPC.height = 22;
            NPC.defense = 4;
            NPC.lifeMax = 100;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.HitSound = SoundID.NPCHit50;
            NPC.DeathSound = Main.zenithWorld ? AresGaussNuke.NukeExplosionSound : DeathSound;
            NPC.knockBackResist = 0.7f;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<SunskaterBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Sunskater")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hasBeenHit);
            writer.Write(NPC.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hasBeenHit = reader.ReadBoolean();
            NPC.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            NPC.spriteDirection = (NPC.direction > 0) ? 1 : -1;
            NPC.noGravity = true;
            if (NPC.direction == 0)
            {
                NPC.TargetClosest(true);
            }
            if (NPC.justHit)
            {
                hasBeenHit = true;
            }
            NPC.chaseable = hasBeenHit;
            if (!NPC.wet)
            {
                bool flag14 = hasBeenHit;
                NPC.TargetClosest(false);
                if ((Main.player[NPC.target].wet || Main.player[NPC.target].dead) && flag14)
                {
                    flag14 = false;
                }
                if (!flag14)
                {
                    if (NPC.collideX)
                    {
                        NPC.velocity.X = NPC.velocity.X * -1f;
                        NPC.direction *= -1;
                        NPC.netUpdate = true;
                    }
                    if (NPC.collideY)
                    {
                        NPC.netUpdate = true;
                        if (NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y = Math.Abs(NPC.velocity.Y) * -1f;
                            NPC.directionY = -1;
                            NPC.ai[0] = -1f;
                        }
                        else if (NPC.velocity.Y < 0f)
                        {
                            NPC.velocity.Y = Math.Abs(NPC.velocity.Y);
                            NPC.directionY = 1;
                            NPC.ai[0] = 1f;
                        }
                    }
                }
                if (flag14)
                {
                    NPC.TargetClosest(true);
                    NPC.velocity.X = NPC.velocity.X + (float)NPC.direction * 0.15f;
                    NPC.velocity.Y = NPC.velocity.Y + (float)NPC.directionY * 0.15f;
                    NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -10f, 10f);
                    NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -10f, 10f);
                }
                else
                {
                    NPC.velocity.X += (float)NPC.direction * 0.1f;
                    if (NPC.velocity.X < -2f || NPC.velocity.X > 2f)
                    {
                        NPC.velocity.X *= 0.95f;
                    }
                    if (NPC.ai[0] == -1f)
                    {
                        NPC.velocity.Y -= 0.01f;
                        if (NPC.velocity.Y < -0.3f)
                        {
                            NPC.ai[0] = 1f;
                        }
                    }
                    else
                    {
                        NPC.velocity.Y += 0.01f;
                        if (NPC.velocity.Y > 0.3f)
                        {
                            NPC.ai[0] = -1f;
                        }
                    }
                }
                int num258 = (int)(NPC.position.X + (float)(NPC.width / 2)) / 16;
                int num259 = (int)(NPC.position.Y + (float)(NPC.height / 2)) / 16;
                if (Main.tile[num258, num259 - 1].LiquidAmount < 128) //problem?
                {
                    if (Main.tile[num258, num259 + 1].HasTile)
                    {
                        NPC.ai[0] = -1f;
                    }
                    else if (Main.tile[num258, num259 + 2].HasTile)
                    {
                        NPC.ai[0] = -1f;
                    }
                }
                if (NPC.velocity.Y > 0.4f || NPC.velocity.Y < -0.4f)
                {
                    NPC.velocity.Y *= 0.95f;
                }
            }
            else
            {
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity.X *= 0.94f;
                    if (NPC.velocity.X > -0.2f && NPC.velocity.X < 0.2f)
                    {
                        NPC.velocity.X = 0f;
                    }
                }
                NPC.velocity.Y = NPC.velocity.Y + 0.3f;
                if (NPC.velocity.Y > 5f)
                {
                    NPC.velocity.Y = 5f;
                }
                NPC.ai[0] = 1f;
            }
            NPC.rotation = NPC.velocity.Y * (float)NPC.direction * 0.1f;
            if ((double)NPC.rotation < -0.1)
            {
                NPC.rotation = -0.1f;
            }
            if ((double)NPC.rotation > 0.1)
            {
                NPC.rotation = 0.1f;
                return;
            }
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return hasBeenHit;
            }
            return null;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += hasBeenHit ? 0.15f : 0.075f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe)
            {
                return 0f;
            }
            return SpawnCondition.Sky.Chance * 0.15f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 120, true);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.AddIf(() => Main.hardMode, ModContent.ItemType<EssenceofSunlight>(), 2);

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 64, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 64, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.zenithWorld)
                {
                    float screenShakePower = 16 * Utils.GetLerpValue(1300f, 0f, NPC.Distance(Main.LocalPlayer.Center), true);
                    if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < screenShakePower)
                        Main.LocalPlayer.Calamity().GeneralScreenShakePower = screenShakePower;
                }
            }
        }
    }
}
