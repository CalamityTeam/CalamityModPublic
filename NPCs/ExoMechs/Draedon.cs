using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

using ApolloBoss = CalamityMod.NPCs.ExoMechs.Apollo.Apollo;
using ArtemisBoss = CalamityMod.NPCs.ExoMechs.Artemis.Artemis;

namespace CalamityMod.NPCs.ExoMechs
{
    public class Draedon : ModNPC
    {
        public enum ExoMech
        {
            Destroyer,
            Prime,
            Twins
        }
        public Player PlayerToFollow => Main.player[npc.target];
        public ref float TalkTimer => ref npc.ai[0];
        public ExoMech MechToSummon
        {
            get => (ExoMech)(int)npc.ai[1];
            set => npc.ai[1] = (int)value;
        }
        public static readonly Color TextColor = new Color(155, 255, 255);
        public const int TeleportFadeinTime = 45;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draedon");
            Main.npcFrameCount[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.damage = 0;
            npc.width = npc.height = 34;
            npc.defense = 9999;
            npc.lifeMax = 50;
            npc.friendly = true;
            npc.noGravity = true;
            npc.dontTakeDamage = true;
            npc.aiStyle = aiType = -1;
            npc.knockBackResist = 0f;
        }

        public override void AI()
        {
            npc.Opacity = Utils.InverseLerp(0f, 8f, TalkTimer, true);
            npc.spriteDirection = (PlayerToFollow.Center.X < npc.Center.X).ToDirectionInt();
            if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == 90f)
            {
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonIntroductionText1", TextColor);
                npc.TargetClosest(false);
                npc.netUpdate = true;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == 190f)
            {
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonIntroductionText2", TextColor);
                npc.netUpdate = true;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && TalkTimer == 270f)
            {
                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.DraedonIntroductionText3", TextColor);
                npc.netUpdate = true;
            }

            // Make the screen rumble and summon the exo mechs.
            if (TalkTimer < 375f)
            {
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = Utils.InverseLerp(4200f, 1400f, Main.LocalPlayer.Distance(PlayerToFollow.Center), true) * 18f;
                Main.LocalPlayer.Calamity().GeneralScreenShakePower *= Utils.InverseLerp(275f, 375f, TalkTimer, true);
            }

            if (TalkTimer == 370f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    SummonExoMech();

                if (Main.netMode != NetmodeID.Server)
                {
                    var sound = Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/FlareSound"), PlayerToFollow.Center);
                    sound.Volume = MathHelper.Clamp(sound.Volume * 1.55f, 0f, 1f);
                }
            }

            TalkTimer++;
        }

        public void SummonExoMech()
        {
            switch (MechToSummon)
            {
                // Summon Thanatos underground.
                case ExoMech.Destroyer:
                    Vector2 thanatosSpawnPosition = PlayerToFollow.Center + Vector2.UnitY * 2100f;
                    NPC thanatos = CalamityUtils.SpawnBossBetter(thanatosSpawnPosition, ModContent.NPCType<ThanatosHead>());
                    if (thanatos != null)
                        thanatos.velocity = thanatos.SafeDirectionTo(PlayerToFollow.Center) * 40f;
                    break;

                // Summon Ares in the sky, directly above the player.
                case ExoMech.Prime:
                    Vector2 aresSpawnPosition = PlayerToFollow.Center - Vector2.UnitY * 1400f;
                    CalamityUtils.SpawnBossBetter(aresSpawnPosition, ModContent.NPCType<AresBody>());
                    break;

                // Summon Apollo and Artemis above the player to their sides.
                case ExoMech.Twins:
                    Vector2 artemisSpawnPosition = PlayerToFollow.Center + new Vector2(-1100f, -1600f);
                    Vector2 apolloSpawnPosition = PlayerToFollow.Center + new Vector2(1100f, -1600f);
                    CalamityUtils.SpawnBossBetter(artemisSpawnPosition, ModContent.NPCType<ArtemisBoss>());
                    CalamityUtils.SpawnBossBetter(apolloSpawnPosition, ModContent.NPCType<ApolloBoss>());
                    break;
            }
        }

        // Draedon should not manually despawn.
        public override bool CheckActive() => false;

		public override Color? GetAlpha(Color drawColor)
		{
            float teleportFade = Utils.InverseLerp(0f, TeleportFadeinTime, TalkTimer, true);
            Color color = Color.Lerp(drawColor, Color.Cyan, 1f - (float)Math.Pow(teleportFade, 3D));
            color.A = (byte)(int)(teleportFade * 255f);

            return color * npc.Opacity;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
            if (TalkTimer > TeleportFadeinTime)
                return true;

            spriteBatch.EnterShaderRegion();

            Texture2D texture = Main.npcTexture[npc.type];
            Vector2 drawPosition = npc.Center - Main.screenPosition - Vector2.UnitY * 38f;
            Vector2 origin = npc.frame.Size() * 0.5f;
            Color color = npc.GetAlpha(drawColor);
            SpriteEffects direction = npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            GameShaders.Misc["CalamityMod:Glitch"].UseOpacity(1f - TalkTimer / TeleportFadeinTime);
            GameShaders.Misc["CalamityMod:Glitch"].UseSecondaryColor(color);
            GameShaders.Misc["CalamityMod:Glitch"].UseSaturation(color.A / 255f);
            GameShaders.Misc["CalamityMod:Glitch"].Apply();

            spriteBatch.Draw(texture, drawPosition, npc.frame, Color.White * npc.Opacity, npc.rotation, origin, npc.scale, direction, 0f);

            spriteBatch.ExitShaderRegion();

            return false;
		}
	}
}
