using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using System.Linq;
using CalamityMod.Graphics.Primitives;

namespace CalamityMod.NPCs.AcidRain
{
    public class AcidEel : ModNPC
    {
        public Player Target => Main.player[NPC.target];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = 12;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                PortraitPositionXOverride = 0
            };
            value.Position.X += 15;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 20;

            NPC.damage = 20;
            NPC.lifeMax = 45;
            NPC.defense = 4;
            NPC.knockBackResist = 0.9f;

            if (DownedBossSystem.downedPolterghast)
            {
                NPC.DR_NERD(0.05f);
                NPC.damage = 100;
                NPC.lifeMax = 2000;
                NPC.defense = 20;
                NPC.knockBackResist = 0.7f;
            }
            else if (DownedBossSystem.downedAquaticScourge)
            {
                NPC.damage = 50;
                NPC.lifeMax = 180;
            }

            NPC.value = Item.buyPrice(0, 0, 3, 32);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.lavaImmune = false;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<AcidEelBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AcidRainBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.AcidEel")
            });
        }

        public override void AI()
        {
            NPC.TargetClosest(false);

            // Fall through platforms.
            NPC.Calamity().ShouldFallThroughPlatforms = true;

            // Play a slither sound from time to time.
            if (Main.rand.NextBool(480))
                SoundEngine.PlaySound(SoundID.Zombie32, NPC.Center);

            if (NPC.wet)
            {
                SwimTowardsTarget();
                return;
            }

            // Do nothing on land.
            NPC.rotation = NPC.rotation.AngleLerp(0f, 0.1f);
            NPC.velocity.X *= 0.95f;
            if (NPC.velocity.Y < 14f)
                NPC.velocity.Y += 0.15f;
            NPC.spriteDirection = NPC.direction;
        }

        public void SwimTowardsTarget()
        {
            float swimSpeed = 12f;
            if (DownedBossSystem.downedAquaticScourge)
                swimSpeed += 3f;
            if (DownedBossSystem.downedPolterghast)
                swimSpeed += 4f;

            // Swim upwards if sufficiently under water.
            bool airAbove = false;
            for (int dy = -160; dy < 0; dy += 8)
            {
                if (!Collision.WetCollision(NPC.position + Vector2.UnitY * dy, NPC.width, 16))
                {
                    airAbove = true;
                    break;
                }
            }

            if (!airAbove)
                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y - 0.25f, -14f, 14f);
            else
                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y + 0.4f, -4f, 8f);

            // Coast in a single direction until hitting something.
            if (NPC.direction == 0)
            {
                NPC.direction = Main.rand.NextBool().ToDirectionInt();
                NPC.netUpdate = true;
            }

            // Rebound on impending collision or when near the world edge.
            bool nearWorldEdge = NPC.Center.X < (Main.offLimitBorderTiles + 2f) * 16f || NPC.Center.X > (Main.maxTilesX - Main.offLimitBorderTiles - 2f) * 16f;
            if ((CalamityUtils.DistanceToTileCollisionHit(NPC.Center, Vector2.UnitX * NPC.direction, 20) ?? 20f) < 5f || nearWorldEdge)
            {
                NPC.direction *= -1;
                if (nearWorldEdge)
                    NPC.position.X += Math.Sign(Main.maxTilesX * 8f - NPC.position.X) * 12f;

                NPC.netUpdate = true;
            }

            NPC.velocity.X = (NPC.velocity.X * 24f + NPC.direction * swimSpeed) / 25f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<SulphuricScale>(), 2, 1, 3);
            LeadingConditionRule postAS = npcLoot.DefineConditionalDropSet(DropHelper.PostAS());
            postAS.Add(ModContent.ItemType<SlitheringEels>(), 20);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public float SegmentWidthFunction(float completionRatio) => NPC.width * NPC.scale * 0.5f;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 headDrawPosition = NPC.Center - screenPos;
            if (NPC.IsABestiaryIconDummy)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AcidRain/AcidEelBestiary").Value;
                Rectangle eelArea = NPC.frame with { Width = 74 };
                Main.EntitySpriteDraw(texture, headDrawPosition, eelArea, NPC.GetAlpha(Color.White), NPC.rotation, eelArea.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);
                return false;
            }

            Texture2D headTexture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D tailTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AcidRain/AcidEelTail").Value;
            Vector2[] segmentPositions = (Vector2[])NPC.oldPos.Clone();

            Vector2 segmentAreaTopLeft = Vector2.One * 999999f;
            Vector2 segmentAreaTopRight = Vector2.Zero;
            if (NPC.IsABestiaryIconDummy)
            {
                for (int i = 0; i < segmentPositions.Length; i++)
                    segmentPositions[i] = NPC.TopLeft + Vector2.UnitX * i * 5f;
            }
            segmentPositions = segmentPositions.Where(p => p != Vector2.Zero).ToArray();

            for (int i = 0; i < segmentPositions.Length; i++)
            {
                segmentPositions[i] += NPC.Size * 0.5f - NPC.rotation.ToRotationVector2() * Math.Sign(NPC.velocity.X) * 8f;
                if (segmentAreaTopLeft.X > segmentPositions[i].X)
                    segmentAreaTopLeft.X = segmentPositions[i].X;
                if (segmentAreaTopLeft.Y > segmentPositions[i].Y)
                    segmentAreaTopLeft.Y = segmentPositions[i].Y;

                if (segmentAreaTopRight.X < segmentPositions[i].X)
                    segmentAreaTopRight.X = segmentPositions[i].X;
                if (segmentAreaTopRight.Y < segmentPositions[i].Y)
                    segmentAreaTopRight.Y = segmentPositions[i].Y;
            }

            // Set shader parameters.
            float offsetAngle = (NPC.position - NPC.oldPos[1]).ToRotation();
            Vector2 primitiveArea = (segmentAreaTopRight - segmentAreaTopLeft).RotatedBy(offsetAngle);
            Rectangle tailArea = NPC.frame with { Width = 28 };
            GameShaders.Misc["CalamityMod:PrimitiveTexture"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/NPCs/AcidRain/AcidEelBody"));
            GameShaders.Misc["CalamityMod:PrimitiveTexture"].Shader.Parameters["uPrimitiveSize"].SetValue(primitiveArea);
            GameShaders.Misc["CalamityMod:PrimitiveTexture"].Shader.Parameters["flipVertically"].SetValue(NPC.velocity.X > 0f);

            SpriteEffects direction = NPC.velocity.X < 0f ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(headTexture, headDrawPosition, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, direction, 0);

            if (segmentPositions.Length >= 2)
            {
                float tailRotation = (segmentPositions[^2] - segmentPositions[^1]).ToRotation() + MathHelper.Pi;
                Vector2 tailDrawPosition = segmentPositions[^1] - tailRotation.ToRotationVector2() * 4f;
                SpriteEffects tailDirection = NPC.velocity.X < 0f ? SpriteEffects.None : SpriteEffects.FlipVertically;
                Main.EntitySpriteDraw(tailTexture, tailDrawPosition, tailArea, NPC.GetAlpha(Color.White), tailRotation, tailArea.Size() * new Vector2(0f, 0.5f), NPC.scale, tailDirection, 0);
                PrimitiveSet.Prepare(segmentPositions, new(SegmentWidthFunction, _ => NPC.GetAlpha(Color.White), pixelate: false, shader: GameShaders.Misc["CalamityMod:PrimitiveTexture"]), 36);
            }

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 8; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AcidEelGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AcidEelGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AcidEelGore3").Type, NPC.scale);
                }
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }
    }
}
