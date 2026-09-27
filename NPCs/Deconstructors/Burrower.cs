using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Effects;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Sounds;
using CalamityMod.Systems;
using CalamityMod.Systems.Mechanic;
using CalamityMod.Tiles.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
namespace CalamityMod.NPCs.Deconstructors
{
    [LongDistanceNetSync(SyncWith = typeof(Burrower))]
    public class BurrowerHitbox : BaseWormHitboxNPC
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.Burrower.DisplayName");
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }
        public override void SetDefaults()
        {
            NPC.damage = 0;
            NPC.width = 88;
            NPC.height = 88;
            NPC.lifeMax = 100;
            NPC.value = 0;

            NPC.HitSound = ThanatosHead.ThanatosHitSoundClosed;
            NPC.DeathSound = CommonCalamitySounds.WulfrumNPCDeathSound;
            NPC.knockBackResist = 0f;
            NPC.behindTiles = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.SuperArmor = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToCold = false;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = false;
            NPC.chaseable = false;
            Banner = ModContent.NPCType<Burrower>();
            BannerItem = ModContent.ItemType<BurrowerBanner>();
            base.SetDefaults();
        }
        public override void AI()
        {
            base.AI();
            if (Main.npc.IndexInRange((int)NPC.ai[0]))
            {
                NPC headNPC = Main.npc[(int)NPC.ai[0]];
                NPC.Calamity().DR = headNPC.Calamity().DR;
                NPC.HitSound = headNPC.HitSound;
            }
            
            NPC.width = 38;
            NPC.height = 38;
        }
    }

    [LongDistanceNetSync]
    public class Burrower : BaseWormNPC
    {
        public override string Texture => "CalamityMod/NPCs/Deconstructors/DeconstructorMK1Head";

        public override int WormHitboxNpcType => ModContent.NPCType<BurrowerHitbox>();
        public override List<string> SegmentTextures => new()
        {
            "CalamityMod/NPCs/Deconstructors/DeconstructorMK1Body",
            "CalamityMod/NPCs/Deconstructors/DeconstructorMK1BodyAlt1",
            "CalamityMod/NPCs/Deconstructors/DeconstructorMK1BodyAlt2",
            "CalamityMod/NPCs/Deconstructors/DeconstructorMK1Tail"
        };

        public override List<string?> GlowTextures => new()
        {
            null,
            "CalamityMod/NPCs/Deconstructors/DeconstructorMK1BodyGlow",
            "CalamityMod/NPCs/Deconstructors/DeconstructorMK1BodyAlt1Glow",
            "CalamityMod/NPCs/Deconstructors/DeconstructorMK1BodyAlt2Glow"
        };
        public override int SegmentCount => 10;

        public override List<float> SegmentTypePositionOffsets => new()
        {
            32,
            32,
            32,
            32,
            32
        };

        public static HashSet<int> VulnerableDebuffs => [BuffID.Electrified, ModContent.BuffType<StaticDischarge>(), ModContent.BuffType<VermillionFlux>(), ModContent.BuffType<AuricRebuke>()];
        public override void SetStaticDefaults()
        {
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            foreach (var item in VulnerableDebuffs)
            {
                NPCID.Sets.SpecificDebuffImmunity[Type][item] = false;
            }
            NPCID.Sets.ImmuneToRegularBuffs[WormHitboxNpcType] = NPCID.Sets.ImmuneToRegularBuffs[Type];
            NPCID.Sets.SpecificDebuffImmunity[WormHitboxNpcType] = NPCID.Sets.SpecificDebuffImmunity[Type];
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            NPC.damage = 0;
            NPC.width = 38;
            NPC.height = 38;
            NPC.lifeMax = 500;
            NPC.value = Item.buyPrice(0, 0, 50, 0);
            NPC.rarity = 3;
            NPC.HitSound = ThanatosHead.ThanatosHitSoundClosed;
            NPC.DeathSound = SoundID.NPCDeath44;
            NPC.knockBackResist = 0f;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.chaseable = false;
            NPC.Calamity().DR = 0.9f;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToCold = false;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = false;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<BurrowerBanner>();

            for (var i = 0; i < SegmentCount - 1; i++)
            {
                Segments.Add(new BaseWormSegment(this, i % 3));
            }
            Segments.Add(new BaseWormSegment(this, 3));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(
            [
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Burrower")
            ]);
        }

        #region AI Variables

        public enum AttackState
        {
            Idle,
            Mining,
            GettingItem,
            Fleeing
        }
        public AttackState ActiveAttackState
        {
            get { return (AttackState)NPC.ai[1]; }
            set { NPC.ai[1] = (float)value; }
        }
        public float MainTimer
        {
            get { return NPC.ai[0]; }
            set { NPC.ai[0] = value; }
        }

        public float AttackSubstate
        {
            get { return NPC.ai[2]; }
            set { NPC.ai[2] = value; }
        }

        public Vector2 TargetVector = Vector2.Zero;
        public Vector2 SecondaryVector = Vector2.Zero;
        #endregion

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(TargetVector);
            writer.WriteVector2(SecondaryVector);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            TargetVector = reader.ReadVector2();
            SecondaryVector = reader.ReadVector2();
        }

        public void SwitchAttackState(AttackState State, float Substate = 0, bool resetVector = true)
        {
            ActiveAttackState = State;
            AttackSubstate = Substate;
            MainTimer = 0;
            if (resetVector)
                TargetVector = Vector2.Zero;
            NPC.ForceNetUpdate();
        }

        public override void AI()
        {
            HandleAIStates();
            MainTimer++;
            UpdateSegments();
        }
        public static List<List<Point>> FindOreVeins(Point wormTile)
        {
            List<List<Point>> oreVeins = new();
            HashSet<Point> visited = new();

            for (int x = -30; x <= 30; x++)
            {
                int tileX = wormTile.X + x;
                if (tileX < 0 || tileX >= Main.maxTilesX)
                    continue;

                for (int y = -30; y <= 30; y++)
                {
                    int tileY = wormTile.Y + y;
                    if (tileY < 0 || tileY >= Main.maxTilesY)
                        continue;

                    Point start = new(tileX, tileY);
                    if (visited.Contains(start))
                        continue;

                    var tile = Main.tile[start];

                    //This uses a flood fill to check for the ore vein
                    if (tile.HasTile && TileID.Sets.Ore[tile.TileType])
                    {
                        List<Point> vein = new();
                        Queue<Point> queue = new();
                        queue.Enqueue(start);
                        visited.Add(start);
                        while (queue.Count > 0)
                        {
                            Point p = queue.Dequeue();
                            vein.Add(p);
                            foreach (var offset in new[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) })
                            {
                                Point neighbor = p + offset;
                                if (neighbor.X < 0 || neighbor.X >= Main.maxTilesX || neighbor.Y < 0 || neighbor.Y >= Main.maxTilesY)
                                    continue;
                                if (visited.Contains(neighbor))
                                    continue;

                                var neighborTile = Main.tile[neighbor];
                                if (neighborTile.HasTile && TileID.Sets.Ore[neighborTile.TileType])
                                {
                                    queue.Enqueue(neighbor);
                                    visited.Add(neighbor);
                                }
                            }
                        }

                        oreVeins.Add(vein);
                    }
                }
            }
            return oreVeins;
        }

        public static (Point, Point)? FindTargetFromVein(List<Point> vein)
        {
            HashSet<Point> veinSet = new(vein);
            List<(Point, Point)> outerPoints = [];

            foreach (var p in vein)
            {
                foreach (var offset in new[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) })
                {
                    Point neighbor = p + offset;
                    if (!veinSet.Contains(neighbor))
                    {
                        if (neighbor.X >= 0 && neighbor.X < Main.maxTilesX && neighbor.Y >= 0 && neighbor.Y < Main.maxTilesY)
                        {
                            var tile = Main.tile[neighbor];
                            if (tile == null || !tile.HasTile || !tile.IsTileSolid())
                                outerPoints.Add((p, neighbor)); // Return the first found ore block adjacent to air, and the air block found.
                        }
                    }
                }
            }
            if (outerPoints.Count > 0)
                return outerPoints[Main.rand.Next(outerPoints.Count)];
            // If none are adjacent to air, return null
            return null;
        }

        private void LowerTargetToGround()
        {
            var pointToCheck = TargetVector.ToTileCoordinates();
            for (var i = 0; i < 50; i++)
            {
                if (pointToCheck.X < 0 || pointToCheck.X >= Main.maxTilesX || pointToCheck.Y < 0 || pointToCheck.Y >= Main.maxTilesY)
                    return;
                var targetTile = Main.tile[pointToCheck];
                if (targetTile == null || !targetTile.HasTile || !targetTile.IsTileSolidGround())
                    pointToCheck.Y += 1;
                else
                {
                    TargetVector = pointToCheck.ToWorldCoordinates() - new Vector2(0, 16);
                    return;
                }
            }
        }

        public void HandleAIStates()
        {
            if (!NPC.HasValidTarget)
            {
                NPC.CalamityTargeting(CalamityTargetingParameters.BossDefaults);
                return;
            }

            Player player = Main.player[NPC.target];
            SegmentMaxRotation = 0.65f;
            SegmentRigidity = 0.2f;
            if (NPC.life < NPC.lifeMax && ActiveAttackState != AttackState.Fleeing)
            {
                ActiveAttackState = AttackState.Fleeing;
                GeneralParticleHandler.SpawnParticle(new EmoteExpressionParticle(NPC.Top, -Vector2.UnitY * 5, 2, ArsenalEffects.ArsenalLaserColor, 60, EmoteExpressionParticle.EmoteType.DoubleExclamation));
            }

            NPC.FindClosestPlayer(out float distanceToPlayer);
            bool noGravity = distanceToPlayer > 800 || NPC.wet || Collision.SolidCollision(NPC.position, NPC.width, NPC.height, true);
            switch (ActiveAttackState)
            {
                case AttackState.Idle:
                    {
                        if (TargetVector == Vector2.Zero || MainTimer > 300 || NPC.Distance(TargetVector) < 32)
                        {
                            if (Main.rand.NextBool())
                            {
                                var veins = FindOreVeins(NPC.Center.ToTileCoordinates());
                                while (veins.Count > 0)
                                {
                                    var targetVein = veins[Main.rand.Next(veins.Count)];
                                    var foundTarget = FindTargetFromVein(targetVein);
                                    if (foundTarget is not null)
                                    {
                                        TargetVector = foundTarget.Value.Item1.ToWorldCoordinates();
                                        SecondaryVector = foundTarget.Value.Item2.ToWorldCoordinates();
                                        if (NPC.Distance(TargetVector) > 160)
                                            GeneralParticleHandler.SpawnParticle(new EmoteExpressionParticle(NPC.Top, -Vector2.UnitY * 5, 2, ArsenalEffects.ArsenalGaussColor, 60, EmoteExpressionParticle.EmoteType.Exclamation));
                                        SwitchAttackState(AttackState.Mining, resetVector: false);
                                        return;
                                    }
                                    else
                                        veins.Remove(targetVein);
                                }
                            }

                            TargetVector = player.Center + Main.rand.NextVector2Circular(800, 800);
                            LowerTargetToGround();
                            MainTimer = 0;
                            NPC.netUpdate = true;
                        }
                        if (AttackSubstate <= 0 && noGravity)
                        {
                            NPC.velocity += NPC.DirectionTo(TargetVector);
                            NPC.velocity *= 0.9f;
                        }
                        else
                        {
                            AttackSubstate--;
                            if (!noGravity)
                            {
                                AttackSubstate = 30;
                                NPC.velocity.Y += 1f;
                            }
                            else
                            {
                                if (NPC.velocity.Y > 8)
                                    NPC.velocity.Y *= 0.9f;
                                NPC.velocity.X *= 0.95f;
                            }
                        }
                        NPC.velocity = NPC.velocity.ClampMagnitude(0, 16);
                        NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;
                        break;
                    }
                case AttackState.Mining:
                    {
                        NPC.velocity += NPC.DirectionTo(SecondaryVector).SafeNormalize(Vector2.UnitY);
                        NPC.velocity *= 0.9f;
                        if (MainTimer > 600)
                            SwitchAttackState(AttackState.Idle);
                        if (NPC.Distance(SecondaryVector) < 4)
                        {
                            var dir = SecondaryVector.DirectionTo(TargetVector);

                            if (Main.tile[TargetVector.ToTileCoordinates()].TileType == ModContent.TileType<AuricOre>())
                            {
                                NPC.velocity = -NPC.DirectionTo(TargetVector) * 16;
                                NPC.Center = SecondaryVector + NPC.velocity;
                                NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
                                NPC.AddBuff(ModContent.BuffType<AuricRebuke>(), 600);
                                ActiveAttackState = AttackState.Fleeing;
                                AuricOre.Animate = true;
                                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/ExoMechs/TeslaShoot1"), NPC.Center);
                                return;
                            }
                            SegmentRigidity = 0f;
                            NPC.velocity = Vector2.Zero;
                            NPC.rotation = SecondaryVector.DirectionTo(TargetVector).ToRotation() + MathHelper.PiOver2;

                            if (Main.netMode != NetmodeID.Server && !(BurrowerPingTileEffect.Instance.Active))
                                TilePingerSystem.AddPing(BurrowerPingTileEffect.Instance, NPC.Center, player);
                            for (int i = 0; i < 1; i++)
                            {
                                int sparkLifetime = Main.rand.Next(10, 20);
                                float sparkScale = Main.rand.NextFloat(0.8f, 1f);
                                Color sparkColor = Color.Lerp(Color.Silver, Color.Gold, Main.rand.NextFloat(0.7f));
                                sparkColor = Color.Lerp(sparkColor, Color.Orange, Main.rand.NextFloat());

                                if (Main.rand.NextBool(10))
                                    sparkScale *= 2f;

                                Vector2 sparkVelocity = dir.RotatedByRandom(0.6f) * Main.rand.NextFloat(6f, 16f);
                                SparkParticle spark = new SparkParticle((TargetVector + SecondaryVector) * 0.5f, -sparkVelocity, true, sparkLifetime, sparkScale, sparkColor);
                                GeneralParticleHandler.SpawnParticle(spark);

                                if (MainTimer < 520)
                                    MainTimer = 520;
                            }
                            SoundEngine.PlaySound(SoundID.NPCHit18 with { Volume = 0.2f }, NPC.Center);
                        }
                        else
                            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;
                        break;
                    }
                case AttackState.GettingItem:
                    {
                        ActiveAttackState = AttackState.Idle;
                        break;
                    }
                case AttackState.Fleeing:
                    {
                        bool shocked = false;
                        foreach (var item in VulnerableDebuffs)
                        {
                            if (NPC.HasBuff(item) || Main.npc.Any(x => x.active && x.type == WormHitboxNpcType && x.HasBuff(item)))
                            {
                                shocked = true;
                                break;
                            }
                        }
                        if (noGravity)
                        {
                            if (shocked)
                            {
                                SegmentRigidity = 0;
                                NPC.velocity *= 0.75f;
                                foreach (var item in Segments)
                                {
                                    if (!Collision.SolidCollision(item.Center - new Vector2(19, 17), 38, 38, true))
                                        item.Center.Y += 2f;
                                }
                            }
                            else
                            {
                                NPC.velocity += NPC.DirectionFrom(Main.player[NPC.FindClosestPlayer()].Center);
                                NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;
                            }
                        }
                        else
                        {
                            NPC.velocity.Y += shocked ? 0.5f : 1;
                            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;
                        }
                        break;
                    }
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            // Gores are not loaded on dedicated servers, so Mod.Find would throw there and abort the kill before loot drops.
            if (NPC.life <= 0 && !Main.dedServ)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, Mod.Find<ModGore>("DeconstructorMK1_Head").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, Mod.Find<ModGore>("DeconstructorMK1_Head2").Type, NPC.scale);
                foreach (var item in Segments)
                {
                    switch (item.segmentType)
                    {
                        case 0:
                            Gore.NewGore(NPC.GetSource_Death(), item.Center, NPC.velocity, Mod.Find<ModGore>("DeconstructorMK1_Body").Type, NPC.scale);
                            Gore.NewGore(NPC.GetSource_Death(), item.Center, NPC.velocity, Mod.Find<ModGore>("DeconstructorMK1_Body2").Type, NPC.scale);
                            break;

                        case 1:
                            Gore.NewGore(NPC.GetSource_Death(), item.Center, NPC.velocity, Mod.Find<ModGore>("DeconstructorMK1_BodyAlt_1").Type, NPC.scale);
                            Gore.NewGore(NPC.GetSource_Death(), item.Center, NPC.velocity, Mod.Find<ModGore>("DeconstructorMK1_BodyAlt_2").Type, NPC.scale);
                            break;

                        case 2:
                            Gore.NewGore(NPC.GetSource_Death(), item.Center, NPC.velocity, Mod.Find<ModGore>("DeconstructorMK1_BodyAlt2_1").Type, NPC.scale);
                            Gore.NewGore(NPC.GetSource_Death(), item.Center, NPC.velocity, Mod.Find<ModGore>("DeconstructorMK1_BodyAlt2_2").Type, NPC.scale);
                            break;

                        case 3:
                            Gore.NewGore(NPC.GetSource_Death(), item.Center, NPC.velocity, Mod.Find<ModGore>("DeconstructorMK1_Tail").Type, NPC.scale);
                            break;
                    }
                }
            }
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<MysteriousCircuitry>(), 1, 4, 8);
            npcLoot.Add(ModContent.ItemType<DubiousPlating>(), 1, 4, 8);
            npcLoot.Add(ModContent.ItemType<BurrowerController>(), 10);
            npcLoot.Add(ItemID.CopperOre, 2, 6, 32);
            npcLoot.Add(ItemID.TinOre, 2, 6, 32);
            npcLoot.Add(ItemID.IronOre, 2, 6, 32);
            npcLoot.Add(ItemID.LeadOre, 2, 6, 32);
            npcLoot.Add(ItemID.SilverOre, 3, 6, 32);
            npcLoot.Add(ItemID.TungstenOre, 3, 6, 32);
            npcLoot.Add(ItemID.GoldOre, 4, 6, 32);
            npcLoot.Add(ItemID.PlatinumOre, 4, 6, 32);
            npcLoot.Add(ItemID.DemoniteOre, 5, 6, 32);
            npcLoot.Add(ItemID.CrimtaneOre, 5, 6, 32);

            var condition = npcLoot.DefineConditionalDropSet(DropHelper.PostPlant());
            condition.Add(ItemID.CobaltOre, 2, 6, 32);
            condition.Add(ItemID.PalladiumOre, 2, 6, 32);
            condition.Add(ItemID.MythrilOre, 3, 6, 32);
            condition.Add(ItemID.OrichalcumOre, 3, 6, 32);
            condition.Add(ItemID.AdamantiteOre, 4, 6, 32);
            condition.Add(ItemID.TitaniumOre, 4, 6, 32);
            condition.Add(ModContent.ItemType<Items.Placeables.Ores.HallowedOre>(), 5, 6, 32);
            condition.Add(ModContent.ItemType<Items.Placeables.Ores.PerennialOre>(), 5, 6, 32);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().InAnyCalamityBiome || Main.npc.Any(x => x.active && x.type == Type))
            {
                return 0f;
            }
            return SpawnCondition.Cavern.Chance * (Main.projectile.Any(x => x.active && x.type == ModContent.ProjectileType<WulfrumLureSignal>()) ? 5f : 0.01f);
        }
    }
}
