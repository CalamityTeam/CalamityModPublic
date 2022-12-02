using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AdultEidolonWyrm
{
    public class AdultEidolonWyrmHead : ModNPC
    {
        public Vector2 PatrolSpot = Vector2.Zero;

        public bool TailSpawned = false;

        public bool DetectsPlayer = false;

        public float speed = 15f;

        public float turnSpeed = 0.4f;

        public Player Target => Main.player[NPC.target];

        public const int MinLength = 40;

        public const int MaxLength = 41;

        public static readonly SoundStyle SpawnSound = new("CalamityMod/Sounds/Custom/Scare");

        public static readonly SoundStyle RoarSound = new("CalamityMod/Sounds/Custom/EidolonWyrmRoarClose");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Adult Eidolon Wyrm");
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.50f,
                PortraitScale = 0.6f,
                PortraitPositionXOverride = 40,
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/AdultEidolonWyrm_Bestiary"
            };
            value.Position.X += 55;
            value.Position.Y += 5;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 24f;
            NPC.damage = 1500;
            NPC.width = 254;
            NPC.height = 138;
            NPC.defense = 700;
            CalamityGlobalNPC global = NPC.Calamity();
            global.DR = 0.95f;
            global.unbreakableDR = true;
            NPC.lifeMax = 1000000;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(10, 0, 0, 0);
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.netAlways = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                // AAAAAAAAAAAAH Scary abyss superboss guy so he gets pitch black bg and no biome source.
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(),

                // Will move to localization whenever that is cleaned up.
                new FlavorTextBestiaryInfoElement("Traces of them appear even in records going back to before the Golden Age of Dragons… They may very well be a glimpse into the full potential of nature.")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(PatrolSpot);
            writer.Write(DetectsPlayer);
            writer.Write(NPC.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            PatrolSpot = reader.ReadVector2();
            DetectsPlayer = reader.ReadBoolean();
            NPC.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Target.dead)
            {
                NPC.TargetClosest(true);
            }
            if (NPC.justHit || DetectsPlayer || Target.chaosState)
            {
                if (!DetectsPlayer)
                {
                    if (Main.netMode != NetmodeID.Server)
                        SoundEngine.PlaySound(SpawnSound, Target.Center);
                    DetectsPlayer = true;
                }
                NPC.damage = 1500;
            }
            else
            {
                NPC.damage = 0;
            }
            NPC.chaseable = DetectsPlayer;
            if (DetectsPlayer)
            {
                if (NPC.soundDelay <= 0 && Main.netMode != NetmodeID.Server)
                {
                    NPC.soundDelay = 420;
                    SoundEngine.PlaySound(RoarSound with { Volume = 2.5f }, NPC.Center);
                }
            }
            else if (Main.rand.NextBool(900) && Main.netMode != NetmodeID.Server)
                SoundEngine.PlaySound(RoarSound with { Volume = 2.5f }, NPC.Center);
            
            if (NPC.ai[2] > 0f)
            {
                NPC.realLife = (int)NPC.ai[2];
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!TailSpawned && NPC.ai[0] == 0f)
                {
                    int Previous = NPC.whoAmI;
                    for (int i = 0; i < MaxLength; i++)
                    {
                        int lol;
                        if (i >= 0 && i < MinLength)
                        {
                            if (i % 2 == 0)
                                lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<AdultEidolonWyrmBody>(), NPC.whoAmI);
                            else
                                lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<AdultEidolonWyrmBodyAlt>(), NPC.whoAmI);
                        }
                        else
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<AdultEidolonWyrmTail>(), NPC.whoAmI);

                        Main.npc[lol].realLife = NPC.whoAmI;
                        Main.npc[lol].ai[2] = (float)NPC.whoAmI;
                        Main.npc[lol].ai[1] = (float)Previous;
                        Main.npc[Previous].ai[0] = (float)lol;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
                        Previous = lol;
                    }
                    TailSpawned = true;
                }
                if (DetectsPlayer)
                {
                    NPC.localAI[0] += 1f;
                    if (NPC.localAI[0] >= 300f)
                    {
                        NPC.localAI[0] = 0f;
                        NPC.TargetClosest(true);
                        NPC.netUpdate = true;
                        
                        int damage = Main.expertMode ? 300 : 400;
                        float xPos = Main.rand.NextBool() ? NPC.position.X + 200f : NPC.position.X - 200f;
                        Vector2 projectileSpawnPosition = new Vector2(xPos, NPC.position.Y + Main.rand.Next(-200, 201));
                        int random = Main.rand.Next(2);
                        if (random == 0)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnPosition, Vector2.Zero, ProjectileID.CultistBossLightningOrb, damage, 0f, Main.myPlayer, 0f, 0f);
                        else
                        {
                            Vector2 mistVelocity = NPC.SafeDirectionTo(Target.Center + Target.velocity * 20f) * 4f;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnPosition, mistVelocity.RotatedBy(-0.6f), ProjectileID.CultistBossIceMist, damage, 0f, Main.myPlayer, 0f, 1f);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnPosition, mistVelocity.RotatedBy(0.6f), ProjectileID.CultistBossIceMist, damage, 0f, Main.myPlayer, 0f, 1f);
                        }
                    }
                }
            }

            // Look in the current movement direction.
            NPC.spriteDirection = NPC.velocity.X.DirectionalSign();

            // Fuck off if the target is dead.
            if (Target.dead)
            {
                NPC.TargetClosest(false);

                NPC.velocity.Y += 3f;
                if (NPC.position.Y > Main.worldSurface * 16.0)
                    NPC.velocity.Y += 3f;

                if (NPC.position.Y > (Main.maxTilesY - 200) * 16.0)
                {
                    for (int a = 0; a < Main.maxNPCs; a++)
                    {
                        if (Main.npc[a].type == NPC.type || Main.npc[a].type == ModContent.NPCType<AdultEidolonWyrmBody>() || Main.npc[a].type == ModContent.NPCType<AdultEidolonWyrmBodyAlt>() || Main.npc[a].type == ModContent.NPCType<AdultEidolonWyrmTail>())
                            Main.npc[a].active = false;
                    }
                }
            }

            // Fade in.
            NPC.Opacity = MathHelper.Clamp(NPC.Opacity + 0.2f, 0f, 1f);

            // Disappear if the target is far away or there's no tail for some reason.
            if (Vector2.Distance(Target.Center, NPC.Center) > 6400f || !NPC.AnyNPCs(ModContent.NPCType<AdultEidolonWyrmTail>()))
                NPC.active = false;
            
            float swimSpeed = speed;
            float swimAcceleration = turnSpeed;

            if (PatrolSpot == Vector2.Zero)
                PatrolSpot = Target.Center;

            Vector2 hoverDestination = DetectsPlayer ? Target.Center : PatrolSpot;

            if (!DetectsPlayer)
            {
                hoverDestination.Y += 800;
                if (MathHelper.Distance(NPC.Center.X, hoverDestination.X) < 400f)
                    hoverDestination.X += NPC.velocity.X.DirectionalSign() * 500f;
            }
            else if (!Target.wet)
            {
                swimSpeed *= 2f;
                swimAcceleration *= 2f;
            }

            // Ensure that speed stays within a specific range.
            NPC.velocity = NPC.velocity.ClampMagnitude(swimSpeed * 0.2f, swimSpeed * 1.3f);
            Vector2 idealVelocity = NPC.SafeDirectionTo(hoverDestination) * swimSpeed;

            if ((NPC.velocity.X > 0f && idealVelocity.X > 0f) || (NPC.velocity.X < 0f && idealVelocity.X < 0f) || (NPC.velocity.Y > 0f && idealVelocity.Y > 0f) || (NPC.velocity.Y < 0f && idealVelocity.Y < 0f))
            {
                // Accelerate towards the ideal velocity.
                NPC.velocity.X += (NPC.velocity.X < idealVelocity.X).ToDirectionInt() * swimAcceleration;
                NPC.velocity.Y += (NPC.velocity.Y < idealVelocity.Y).ToDirectionInt() * swimAcceleration;

                // Swim more quickly towards the ideal velocity if there isn't much speed currently or if the velocity goes against the ideal velocity.
                if (Math.Abs(idealVelocity.Y) < swimSpeed * 0.2 && ((NPC.velocity.X > 0f && idealVelocity.X < 0f) || (NPC.velocity.X < 0f && idealVelocity.X > 0f)))
                    NPC.velocity.Y += NPC.velocity.Y.DirectionalSign() * swimAcceleration * 2f;

                if (Math.Abs(idealVelocity.X) < swimSpeed * 0.2 && ((NPC.velocity.Y > 0f && idealVelocity.Y < 0f) || (NPC.velocity.Y < 0f && idealVelocity.Y > 0f)))
                    NPC.velocity.X += NPC.velocity.X.DirectionalSign() * swimAcceleration * 2f;
            }

            // Choose whichever axis the Wyrm is closest to it's destination on and emphasize moving in that direction.
            else if (MathHelper.Distance(hoverDestination.X, NPC.Center.X) > MathHelper.Distance(hoverDestination.Y, NPC.Center.Y))
            {
                NPC.velocity.X += (NPC.velocity.X < idealVelocity.X).ToDirectionInt() * swimAcceleration * 1.1f;
                if (NPC.velocity.ManhattanDistance(Vector2.Zero) < swimSpeed * 0.5)
                    NPC.velocity.Y += NPC.velocity.Y.DirectionalSign() * swimAcceleration;
            }
            else
            {
                NPC.velocity.Y += (NPC.velocity.Y < idealVelocity.Y).ToDirectionInt() * swimAcceleration * 1.1f;
                if (NPC.velocity.ManhattanDistance(Vector2.Zero) < swimSpeed * 0.5)
                    NPC.velocity.X += NPC.velocity.X.DirectionalSign() * swimAcceleration;
            }
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return DetectsPlayer;
            }
            return null;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.Opacity = 1f;
                return;
            }

            Vector2 center = NPC.Center;
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AdultEidolonWyrm/AdultEidolonWyrmHeadGlow").Value;
            SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.Draw(texture, center - screenPos, NPC.frame, Color.White, NPC.rotation, texture.Size() * 0.5f, NPC.scale, spriteEffects, 0f);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<EidolicWail>());
            npcLoot.Add(ModContent.ItemType<SoulEdge>());
            npcLoot.Add(ModContent.ItemType<HalibutCannon>());
            npcLoot.Add(ModContent.ItemType<Voidstone>(), 1, 80, 100);

            var postClone = npcLoot.DefineConditionalDropSet(() => DownedBossSystem.downedCalamitas);
            postClone.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<Lumenyl>(), 1, 50, 108, 65, 135));
            postClone.Add(ItemID.Ectoplasm, 1, 21, 32);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            // Create gore and dust hit effects.
            if (!DetectsPlayer || Main.netMode == NetmodeID.Server)
                return;

            for (int k = 0; k < 15; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("WyrmAdult").Type, 1f);
            }
        }

        public override bool CheckActive()
        {
            if (DetectsPlayer && !Target.dead)
                return false;

            // Delete all segments when ready to fuck off.
            if (NPC.timeLeft <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int k = (int)NPC.ai[0]; k > 0; k = (int)Main.npc[k].ai[0])
                {
                    if (Main.npc[k].active)
                    {
                        Main.npc[k].active = false;
                        if (Main.netMode == NetmodeID.Server)
                        {
                            Main.npc[k].life = 0;
                            Main.npc[k].netSkip = -1;
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, k, 0f, 0f, 0f, 0, 0, 0);
                        }
                    }
                }
            }
            return true;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<CrushDepth>(), 1200, true);
        }
    }
}
