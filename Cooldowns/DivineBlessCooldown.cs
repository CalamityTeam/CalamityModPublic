using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Cooldowns
{
    public class DivineBlessCooldown : Cooldown
    {
        public override string SyncID => "DivineBless";
        public override bool ShouldDisplay => true;
        public override string DisplayName => "Divine Bless Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/DivineRecharge";
        public override Color OutlineColor =>  new Color(233, 192, 68);
        public override Color CooldownStartColor => new Color(177, 105, 33);
        public override Color CooldownEndColor => new Color(233, 192, 68);


        public DivineBlessCooldown(int duration, Player player) : base(duration, player)
        {
        }

        public override void OnCompleted()
        {
            if (AfflictedPlayer.whoAmI == Main.myPlayer)
                Projectile.NewProjectile(AfflictedPlayer.Center, Vector2.Zero, ProjectileType<AllianceTriangle>(), 0, 0f, AfflictedPlayer.whoAmI);
        }
    }
}