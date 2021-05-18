using CalamityMod.Events;
using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class BossRushEndEffectThing : ModProjectile
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
            projectile.timeLeft = BossRushEvent.EndVisualEffectTime;
            projectile.penetrate = -1;
        }

		public override void AI()
		{
            if (Time == 125f)
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/BossRushEnd"), Main.LocalPlayer.Center);

            projectile.Center = Owner.Center;
            BossRushEvent.SyncEndTimer((int)Time);

            float currentShakePower = MathHelper.Lerp(1f, 20f, Utils.InverseLerp(60f, 380f, Time, true) * Utils.InverseLerp(10f, 40f, projectile.timeLeft, true));
            Main.LocalPlayer.Calamity().ScreenShakePower = currentShakePower;

            MoonlordDeathDrama.RequestLight(Utils.InverseLerp(440f, 490f, Time, true) * Utils.InverseLerp(10f, 60f, projectile.timeLeft, true), Main.LocalPlayer.Center);

            Time++;
        }

        public override void Kill(int timeLeft)
		{
            BossRushEvent.End();
            DropHelper.DropItem(Main.LocalPlayer, ModContent.ItemType<Rock>());
            CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.BossRushTierFiveEndText", BossRushEvent.XerocTextColor);
        }
	}
}
