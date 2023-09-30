using CalamityMod.Events;
using CalamityMod.NPCs.ExoMechs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class BossRushEffectThing : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public Player Owner => Main.player[Projectile.owner];
        public ref float Time => ref Projectile.ai[0];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = BossRushEvent.StartEffectTotalTime;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.Center = Owner.Center;
            if (Time >= 70f)
                MoonlordDeathDrama.RequestLight(Utils.GetLerpValue(70f, 85f, Time, true), Main.LocalPlayer.Center);

            if (Time % 10f == 9f)
                BossRushEvent.SyncStartTimer((int)Time);

            float currentShakePower = MathHelper.Lerp(8f, 12f, Utils.GetLerpValue(BossRushEvent.StartEffectTotalTime * 0.6f, BossRushEvent.StartEffectTotalTime, Time, true));
            currentShakePower *= 1f - Utils.GetLerpValue(1500f, 3700f, Main.LocalPlayer.Distance(Projectile.Center), true);
            Main.LocalPlayer.Calamity().GeneralScreenShakePower = currentShakePower;

            Time++;
        }

        public override void OnKill(int timeLeft)
        {
            BossRushEvent.SyncStartTimer(BossRushEvent.StartEffectTotalTime);
            for (int doom = 0; doom < Main.maxNPCs; doom++)
            {
                NPC n = Main.npc[doom];
                if (!n.active)
                    continue;

                // will also correctly despawn EoW because none of his segments are boss flagged
                bool shouldDespawn = n.boss || n.type == NPCID.EaterofWorldsHead || n.type == NPCID.EaterofWorldsBody || n.type == NPCID.EaterofWorldsTail || n.type == ModContent.NPCType<Draedon>();
                if (shouldDespawn)
                {
                    n.active = false;
                    n.netUpdate = true;
                }
            }

            BossRushEvent.BossRushStage = 0;
            BossRushEvent.BossRushActive = true;
            string key = "Mods.CalamityMod.Events.BossRushStartText"; // "Hmm? Ah, another contender. Very well, may the ritual commence!"
            CalamityUtils.DisplayLocalizedText(key, BossRushEvent.XerocTextColor);

            CalamityNetcode.SyncWorld();
            if (Main.netMode == NetmodeID.Server)
            {
                var netMessage = Mod.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.BossRushStage);
                netMessage.Write(BossRushEvent.BossRushStage);
                netMessage.Send();
            }
        }
    }
}
