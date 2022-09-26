using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MechwormTail : ModProjectile
    {
        private int playerMinionSlots = 0;
        private bool runCheck = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mechworm");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.netImportant = true;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.maxMinions > playerMinionSlots)
                playerMinionSlots = owner.maxMinions;

            if (runCheck)
            {
                runCheck = false;
                playerMinionSlots = owner.maxMinions;
            }

            Projectile.localAI[0] = 0f;

            MechwormBody.SegmentAI(Projectile, 16, ref playerMinionSlots);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(tex, drawPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool? CanDamage() => Projectile.alpha == 0;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override bool ShouldUpdatePosition() => false;
    }
}
