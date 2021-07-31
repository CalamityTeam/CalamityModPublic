using CalamityMod.NPCs.ExoMechs.Thanatos;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draedon");
            Main.npcFrameCount[npc.type] = 12;
        }

        public override void SetDefaults()
        {
            npc.damage = 0;
            npc.width = npc.height = 34;
            npc.defense = 9999;
            npc.lifeMax = 50;
            npc.friendly = true;
            npc.dontTakeDamage = true;
            npc.aiStyle = aiType = -1;
            npc.knockBackResist = 0f;
        }

        public override void AI()
        {
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

            npc.velocity = Vector2.UnitY * Utils.InverseLerp(0f, 50f, TalkTimer, true) * Utils.InverseLerp(65f, 50f, TalkTimer, true) * -4.5f;

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
            // Summon Thanatos underground.
            Vector2 thanatosSpawnPosition = PlayerToFollow.Center + Vector2.UnitY * 2100f;

            // Summon Ares in the sky, directly above the player.
            Vector2 aresSpawnPosition = PlayerToFollow.Center - Vector2.UnitY * 1400f;
            // CalamityUtils.SpawnBossBetter(thanatosSpawnPosition, ModContent.NPCType<Ares>());

            // Summon Apollo and Artemis above the player to their sides.
            Vector2 artemisSpawnPosition = PlayerToFollow.Center + new Vector2(1100f, -1600f);
            Vector2 apolloSpawnPosition = PlayerToFollow.Center + new Vector2(-1100f, -1600f);

            switch (MechToSummon)
            {
                case ExoMech.Destroyer:
                    NPC thanatos = CalamityUtils.SpawnBossBetter(thanatosSpawnPosition, ModContent.NPCType<ThanatosHead>());
                    if (thanatos != null)
                        thanatos.velocity = thanatos.SafeDirectionTo(PlayerToFollow.Center) * 40f;
                    break;
                case ExoMech.Prime:
                    //CalamityUtils.SpawnBossBetter(aresSpawnPosition, ModContent.NPCType<Ares>());
                    break;
                case ExoMech.Twins:
                    //CalamityUtils.SpawnBossBetter(artemisSpawnPosition, ModContent.NPCType<Artemis>());
                    //CalamityUtils.SpawnBossBetter(apolloSpawnPosition, ModContent.NPCType<Apollo>());
                    break;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter > 5)
            {
                npc.frameCounter = 0f;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= frameHeight * 6)
                    npc.frame.Y = 0;
            }
        }
    }
}
