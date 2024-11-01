using System;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    [LongDistanceNetSync(SyncWith = typeof(ArmoredDiggerHead))]
    public class ArmoredDiggerBody : ModNPC
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.ArmoredDiggerHead.DisplayName");
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.damage = 70;
            NPC.width = 38;
            NPC.height = 38;
            NPC.defense = 20;
            NPC.DR_NERD(0.2f);
            NPC.lifeMax = 20000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.netAlways = true;
            NPC.dontCountMe = true;
            Banner = ModContent.NPCType<ArmoredDiggerHead>();
            BannerItem = ModContent.ItemType<ArmoredDiggerBanner>();
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;

            // Scale stats in Expert and Master
            CalamityGlobalNPC.AdjustExpertModeStatScaling(NPC);
            CalamityGlobalNPC.AdjustMasterModeStatScaling(NPC);
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            if (NPC.ai[3] > 0f && Main.npc[(int)NPC.ai[3]].type == ModContent.NPCType<ArmoredDiggerHead>())
            {
                NPC.realLife = (int)NPC.ai[3];
            }
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest(true);
            }
            bool shouldDie = false;
            if (NPC.ai[1] <= 0f)
            {
                shouldDie = true;
            }
            else if (Main.npc[(int)NPC.ai[1]].life <= 0)
            {
                shouldDie = true;
            }
            if (shouldDie)
            {
                if (!Main.zenithWorld)
                {
                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.checkDead();
                }
                else
                {
                    // find a new npc to attatch to in gfb, it's not a bug, it's a feature:tm:
                    foreach (NPC host in Main.ActiveNPCs)
                    {
                        if (!host.friendly && !host.dontTakeDamage && host.type != ModContent.NPCType<ArmoredDiggerHead>() && host.type != ModContent.NPCType<ArmoredDiggerBody>() && host.type != ModContent.NPCType<ArmoredDiggerTail>())
                        {
                            NPC.ai[1] = host.whoAmI;
                            NPC.ai[3] = host.whoAmI;
                            break;
                        }
                    }
                    // if it goes through the loop without finding an npc to attatch to (aka, ai[1] never changes and the head is still dead) kill as normal
                    if (Main.npc[(int)NPC.ai[1]].life <= 0)
                    {
                        NPC.life = 0;
                        NPC.HitEffect(0, 10.0);
                        NPC.checkDead();
                    }
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] += (float)Main.rand.Next(4);
                if (NPC.localAI[0] >= (float)Main.rand.Next(1800, 26000))
                {
                    NPC.localAI[0] = 0f;
                    NPC.TargetClosest(true);
                    if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position,
                        Main.player[NPC.target].width, Main.player[NPC.target].height))
                    {
                        float speed = 7f;
                        Vector2 projFirePosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)(NPC.height / 2));
                        float projTargetX = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - projFirePosition.X + (float)Main.rand.Next(-20, 21);
                        float projTargetY = Main.player[NPC.target].position.Y + (float)Main.player[NPC.target].height * 0.5f - projFirePosition.Y + (float)Main.rand.Next(-20, 21);
                        float projTargetDist = (float)Math.Sqrt((double)(projTargetX * projTargetX + projTargetY * projTargetY));
                        projTargetDist = speed / projTargetDist;
                        projTargetX *= projTargetDist;
                        projTargetY *= projTargetDist;
                        int projType = Main.zenithWorld ? ModContent.ProjectileType<ProvidenceCrystalShard>() : ProjectileID.SaucerScrap;
                        projFirePosition.X += projTargetX * 5f;
                        projFirePosition.Y += projTargetY * 5f;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projFirePosition.X, projFirePosition.Y, projTargetX, projTargetY, projType, 30, 0f, Main.myPlayer, 0f, 0f);
                        NPC.netUpdate = true;
                    }
                }
            }
            Vector2 segmentPosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float targetXDist = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2);
            float targetYDist = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2);
            targetXDist = (float)((int)(targetXDist / 16f) * 16);
            targetYDist = (float)((int)(targetYDist / 16f) * 16);
            segmentPosition.X = (float)((int)(segmentPosition.X / 16f) * 16);
            segmentPosition.Y = (float)((int)(segmentPosition.Y / 16f) * 16);
            targetXDist -= segmentPosition.X;
            targetYDist -= segmentPosition.Y;
            float targetDistance = (float)Math.Sqrt((double)(targetXDist * targetXDist + targetYDist * targetYDist));
            if (NPC.ai[1] > 0f && NPC.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    segmentPosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                    targetXDist = Main.npc[(int)NPC.ai[1]].position.X + (float)(Main.npc[(int)NPC.ai[1]].width / 2) - segmentPosition.X;
                    targetYDist = Main.npc[(int)NPC.ai[1]].position.Y + (float)(Main.npc[(int)NPC.ai[1]].height / 2) - segmentPosition.Y;
                }
                catch
                {
                }
                NPC.rotation = (float)Math.Atan2((double)targetYDist, (double)targetXDist) + 1.57f;
                targetDistance = (float)Math.Sqrt((double)(targetXDist * targetXDist + targetYDist * targetYDist));
                int segmentWidth = (int)(44f * NPC.scale);
                targetDistance = (targetDistance - (float)segmentWidth) / targetDistance;
                targetXDist *= targetDistance;
                targetYDist *= targetDistance;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + targetXDist;
                NPC.position.Y = NPC.position.Y + targetYDist;
            }

            // Calculate contact damage based on velocity
            float maxChaseSpeed = CalamityWorld.death ? 13.5f : 10f;
            float minimalContactDamageVelocity = maxChaseSpeed * 0.25f;
            float minimalDamageVelocity = maxChaseSpeed * 0.5f;
            float bodyAndTailVelocity = (NPC.position - NPC.oldPosition).Length();
            if (bodyAndTailVelocity <= minimalContactDamageVelocity)
            {
                NPC.damage = 0;
            }
            else
            {
                float velocityDamageScalar = MathHelper.Clamp((bodyAndTailVelocity - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                NPC.damage = (int)MathHelper.Lerp(0f, NPC.defDamage, velocityDamageScalar);
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ArmoredDiggerBody").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ArmoredDiggerBody2").Type, 1f);
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void ModifyTypeName(ref string typeName)
        {
            if (Main.zenithWorld)
            {
                typeName = CalamityUtils.GetTextValue("NPCs.MechanizedSerpent");
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (Main.zenithWorld)
            {
                Color lightColor = Color.Orange * drawColor.A;
                return lightColor * NPC.Opacity;
            }
            else return null;
        }
    }
}
