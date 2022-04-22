using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Other
{
    public class LecherousOrb : ModNPC
    {
        public ref float Time => ref NPC.ai[0];
        public ref float Frame => ref NPC.localAI[0];
        public Player Owner
        {
            get
            {
                if (NPC.target >= 255 || NPC.target < 0)
                    NPC.TargetClosest();
                return Main.player[NPC.target];
            }
        }
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            DisplayName.SetDefault("Lecherous Orb");
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 28;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 181445;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = 0f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath7;
            NPC.knockBackResist = 0f;
            NPC.netAlways = true;
            NPC.aiStyle = -1;
            NPC.canGhostHeal = false;
            NPC.Calamity().DoesNotGenerateRage = true;
            NPC.Calamity().DoesNotDisappearInBossRush = true;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale) => NPC.lifeMax = 181445;

        public override void AI()
        {
            NPC.Opacity = Utils.GetLerpValue(0f, 15f, Time, true);
            NPC.velocity = Vector2.Zero;

            if (Main.myPlayer == NPC.target)
            {
                // Disappear if the player no longer has the enchanted weapon.
                if (!Owner.Calamity().lecherousOrbEnchant)
                {
                    NPC.active = false;
                    NPC.life = 0;
                    NPC.HitEffect();
                    NPC.checkDead();
                    NPC.netUpdate = true;
                }

                // Notify the owner that the orb has indeed spawned.
                Owner.Calamity().awaitingLecherousOrbSpawn = false;

                Vector2 destination = Vector2.Lerp(Owner.Center, Main.MouseWorld, 0.625f);
                NPC.Center = Vector2.Lerp(NPC.Center, destination, 0.035f).MoveTowards(destination, 8f);
                if (NPC.WithinRange(destination, 5f))
                    NPC.Center = destination;

                if (!NPC.WithinRange(destination, 2000f))
                    NPC.Center = Owner.Center;

                bool wasNotAtDestinationBefore = Vector2.Distance(NPC.position + NPC.Size * 0.5f, destination) < 0.1f && Vector2.Distance(NPC.oldPosition + NPC.Size * 0.5f, destination) > 0.1f;
                if (Vector2.Distance(NPC.position, NPC.oldPosition) > 30f || wasNotAtDestinationBefore)
                    NPC.SyncMotionToServer();
            }

            // Randomly start animations.
            if (Frame == 0f && Main.rand.NextBool(150))
                Frame = 1f;
            if (Frame == 0f && Main.rand.NextBool(300))
                Frame = 8f;

            // Face the owner.
            NPC.spriteDirection = (Owner.Center.X > NPC.Center.X).ToDirectionInt();

            Time++;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 3; i++)
            {
                Dust magic = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(12f, 12f), 264);
                magic.color = Color.Red;
                magic.velocity = Main.rand.NextVector2Circular(3f, 3f);
                magic.fadeIn = 0.9f;
                magic.scale = 1.3f;
                magic.noGravity = true;
            }

            if (NPC.life <= 0)
            {
                for (int i = 0; i < 15; i++)
                {
                    Dust magic = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(12f, 12f), 264);
                    magic.color = Color.Red;
                    magic.velocity = Main.rand.NextVector2Circular(6f, 6f);
                    magic.fadeIn = 1.25f;
                    magic.scale = Main.rand.NextFloat(1.2f, 1.56f);
                    magic.noGravity = true;
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 1; i <= 4; i++)
                        Gore.NewGoreDirect(NPC.Center, Main.rand.NextVector2Circular(3f, 3f), Mod.Find<ModGore>($"LecherousGore{i}").Type);
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (Frame != 0f)
                NPC.frameCounter++;
            else
                NPC.frameCounter = 0f;

            // Perform the blink animation.
            if (Frame >= 1f && Frame < 8f && NPC.frameCounter % 6f == 5f)
            {
                Frame++;
                if (Frame >= 8f)
                    Frame = 0f;
            }

            // Perform the turning inside-out animation.
            if (Frame >= 8f && Frame < 25f && NPC.frameCounter % 5f == 4f)
            {
                Frame++;
                if (Frame >= 25f)
                    Frame = 0f;
            }

            int verticalFrame = (int)Frame % 22;
            int horizontalFrame = (int)Frame / 22;
            NPC.frame.X = horizontalFrame * 64;
            NPC.frame.Y = verticalFrame * 90;
            NPC.frame.Width = 64;
            NPC.frame.Height = 90;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 drawPosition = NPC.Center - screenPos;
            SpriteEffects direction = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            // Draw a faded version of the heart behind the main one that pulses.
            float pulse = Main.GlobalTimeWrappedHourly * 1.9f % 1f;
            float pulseScale = NPC.scale * (1f + pulse * 0.33f);
            Color pulseColor = NPC.GetAlpha(Color.Red) * (1f - pulse) * 0.44f;
            spriteBatch.Draw(texture, drawPosition, NPC.frame, pulseColor, NPC.rotation, NPC.frame.Size() * 0.5f, pulseScale, direction, 0f);

            spriteBatch.Draw(texture, drawPosition, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, direction, 0f);
            return false;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (Main.myPlayer == NPC.target)
                NPC.SyncMotionToServer();

            return base.StrikeNPC(ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override void OnKill()
        {
            int heartsToGive = (int)MathHelper.Lerp(0f, 7f, Utils.GetLerpValue(45f, 540f, Time, true));
            for (int i = 0; i < heartsToGive; i++)
                Item.NewItem(NPC.GetSource_Loot(), NPC.Hitbox, ItemID.Heart);
        }

        public override Color? GetAlpha(Color drawColor) => Color.White * NPC.Opacity;
    }
}
