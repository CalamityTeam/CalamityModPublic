using CalamityMod.NPCs.NormalNPCs.HorribleHog;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Enemy
{
    public class HorribleHogChargeHitbox : ModProjectile, ILocalizedModType
    {
        public ref float HogIndex => ref Projectile.ai[0];

        public new string LocalizationCategory => "Projectiles.Enemy";

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 28;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.hide = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanHitNPC(NPC target) => target.whoAmI != (int)HogIndex;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FinalDamage *= 5f;

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers) => modifiers.Knockback *= 0f;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.velocity.X += 8f * info.HitDirection;
            target.velocity.Y -= 3f;
        }

        public override void AI()
        {
            if (!Main.npc.IndexInRange((int)HogIndex) || !Main.npc[(int)HogIndex].active || Main.npc[(int)HogIndex].type != ModContent.NPCType<HorribleHog>())
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = Main.npc[(int)HogIndex].Center;
            Projectile.velocity = Main.npc[(int)HogIndex].velocity;
        }
    }
}
