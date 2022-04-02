using CalamityMod.Events;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
	public class BossRushEffectThing : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public ref float Time => ref projectile.ai[0];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = BossRushEvent.StartEffectTotalTime;
            projectile.penetrate = -1;
        }

		public override void AI()
		{
            projectile.Center = Owner.Center;
            if (Time >= 70f)
                MoonlordDeathDrama.RequestLight(Utils.InverseLerp(70f, 85f, Time, true), Main.LocalPlayer.Center);

            if (Time % 10f == 9f)
                BossRushEvent.SyncStartTimer((int)Time);

            float currentShakePower = MathHelper.Lerp(8f, 12f, Utils.InverseLerp(BossRushEvent.StartEffectTotalTime * 0.6f, BossRushEvent.StartEffectTotalTime, Time, true));
            currentShakePower *= 1f - Utils.InverseLerp(1500f, 3700f, Main.LocalPlayer.Distance(projectile.Center), true);
            Main.LocalPlayer.Calamity().GeneralScreenShakePower = currentShakePower;

            Time++;
        }

        public override void Kill(int timeLeft)
		{
            BossRushEvent.SyncStartTimer(BossRushEvent.StartEffectTotalTime);
            for (int doom = 0; doom < 200; doom++)
            {
                NPC n = Main.npc[doom];
                if (!n.active)
                    continue;

                // will also correctly despawn EoW because none of his segments are boss flagged
                bool shouldDespawn = n.boss || n.type == NPCID.EaterofWorldsHead || n.type == NPCID.EaterofWorldsBody || n.type == NPCID.EaterofWorldsTail;
                if (shouldDespawn)
                {
                    n.active = false;
                    n.netUpdate = true;
                }
            }

            BossRushEvent.BossRushStage = 0;
            BossRushEvent.BossRushActive = true;
            string key = "Mods.CalamityMod.BossRushStartText";
            CalamityUtils.DisplayLocalizedText(key, BossRushEvent.XerocTextColor);

            CalamityNetcode.SyncWorld();
            if (Main.netMode == NetmodeID.Server)
            {
                var netMessage = mod.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.BossRushStage);
                netMessage.Write(BossRushEvent.BossRushStage);
                netMessage.Send();
            }
        }
	}
}
