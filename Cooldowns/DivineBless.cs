using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Cooldowns
{
    public class DivineBless : CooldownHandler
    {
        public DivineBless(CooldownInstance? c) : base(c) { }

        public override bool ShouldDisplay => true;
        public override string DisplayName => "Divine Bless Cooldown";
        public override string Texture => "CalamityMod/Cooldowns/DivineBless";
        public override Color OutlineColor =>  new Color(233, 192, 68);
        public override Color CooldownStartColor => new Color(177, 105, 33);
        public override Color CooldownEndColor => new Color(233, 192, 68);

        public override void OnCompleted()
        {
            if (instance.player.whoAmI == Main.myPlayer)
                Projectile.NewProjectile(instance.player.Center, Vector2.Zero, ProjectileType<AllianceTriangle>(), 0, 0f, instance.player.whoAmI);
        }
    }
}