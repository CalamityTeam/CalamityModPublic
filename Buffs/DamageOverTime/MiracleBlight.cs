using System.Collections.Generic;
using CalamityMod.DataStructures;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.Particles;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class MiracleBlight : ModBuff
    {
        public static DebuffData debuffData = new DebuffData()
        {
            EnemyLostRegen = 3000
        };

        public static Asset<Texture2D> ShaderTexture => field ??= ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Neurons");

        public static List<int> ExcludedNPCsForShader => new()
        {
            // TODO: Remove this list if possible

            // List the reason why the NPC(s) are excluded :)

            // The particle sets break with the visuals and this is the easiest way to fix this that isn't stupidly complex.
            ModContent.NPCType<AresBody>(),
            ModContent.NPCType<AresGaussNuke>(),
            ModContent.NPCType<AresLaserCannon>(),
            ModContent.NPCType<AresPlasmaFlamethrower>(),
            ModContent.NPCType<AresTeslaCannon>(),

            // Breaks with being behind tiles, and causes a funny interaction where his head goes behind his neck.
            NPCID.MoonLordCore,
            NPCID.MoonLordHand,
            NPCID.MoonLordHead
        };

        public override void Load()
        {
            On_Main.DrawNPC += DrawForNPC;
        }

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
            CalamityBuffSets.DebuffDataset[Type] = debuffData;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().miracleBlight = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().miracleBlight = true;
        }

        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;

            var sparkColor = Main.rand.Next(4) switch
            {
                0 => Color.Red,
                1 => Color.MediumTurquoise,
                2 => Color.Orange,
                _ => Color.LawnGreen,
            };

            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustPerfect(Player.Calamity().RandomDebuffVisualSpot, DustID.RainbowTorch, CalamityUtils.RandomVelocity(100f, 70f, 150f, 0.04f));
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.7f, 0.85f);
                dust.color = sparkColor;
            }

            float numberOfDusts = 1f;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 velOffset = CalamityUtils.RandomVelocity(100f, 70f, 150f, 0.04f);
                velOffset *= Main.rand.NextFloat(2, 13);
                Particle exoEnergy = new GlowSparkParticle(Player.Center + Player.velocity * 3 + velOffset * 1.5f, -velOffset * 0.25f, false, 4, 0.008f, sparkColor, new Vector2(0.6f, 1.2f));
                GeneralParticleHandler.SpawnParticle(exoEnergy);
            }
        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            Vector2 npcSize = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));
            var sparkColor = Main.rand.Next(4) switch
            {
                0 => Color.Red,
                1 => Color.MediumTurquoise,
                2 => Color.Orange,
                _ => Color.LawnGreen,
            };

            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustPerfect(npcSize, DustID.RainbowTorch, CalamityUtils.RandomVelocity(100f, 70f, 150f, 0.04f));
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.7f, 0.85f) + (0.0000007f * npc.width * npc.height);
                dust.color = sparkColor;
            }


            float rotFactor = 360f;
            if (Main.rand.NextBool(3))
            {
                float rot = MathHelper.ToRadians(rotFactor);
                Vector2 velOffset = CalamityUtils.RandomVelocity(100f, 70f, 150f, 0.04f);
                velOffset *= Main.rand.NextFloat(5, 9) + (0.0002f * npc.width * npc.height);

                Particle exoEnergy = new GlowSparkParticle(npc.Center + npc.velocity * 3 + velOffset * 1.5f, -velOffset * 0.25f, false, 4, 0.008f, sparkColor, new Vector2(0.6f, 1.2f));
                GeneralParticleHandler.SpawnParticle(exoEnergy);
            }
        }

        private static void DrawForNPC(On_Main.orig_DrawNPC orig, Main self, int iNPCIndex, bool behindTiles)
        {
            var npc = Main.npc[iNPCIndex];

            if (!CalamityDrawParameterNPC.DrawingMiracleBlight[npc.whoAmI])
            {
                orig(self, iNPCIndex, behindTiles);
                return;
            }

            if (ExcludedNPCsForShader.Contains(npc.type))
            {
                orig(self, iNPCIndex, behindTiles);
                return;
            }

            using (Main.spriteBatch.Scope())
            {
                using var lease = ScreenspaceTargetPool.Shared.Rent(Main.instance.GraphicsDevice);
                using (lease.Scope(clearColor: Color.Transparent))
                {
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
                    orig(self, iNPCIndex, behindTiles);
                    Main.spriteBatch.End();
                }

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

                var blightShader = GameShaders.Misc["CalamityMod:MiracleBlight"];
                blightShader.UseImage1(ShaderTexture);
                blightShader.UseOpacity(0.7f);
                blightShader.Apply();

                Main.spriteBatch.Draw(lease.Target, Vector2.Zero, Color.White);

                Main.spriteBatch.End();
            }
        }
    }
}
