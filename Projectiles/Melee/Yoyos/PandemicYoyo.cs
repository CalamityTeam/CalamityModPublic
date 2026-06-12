using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class PandemicYoyo : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Pandemic>();

        List<NPC> hitNPCs = [];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = Pandemic.Lifetime * 2f;
            ProjectileID.Sets.YoyosMaximumRange[Type] = Pandemic.Reach;
            ProjectileID.Sets.YoyosTopSpeed[Type] = Pandemic.Speed / 2f;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.MaxUpdates = 2;
        }

        public override void AI()
        {
            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f) //200 blocks
                Projectile.Kill();
            Projectile.scale = Pandemic.SizeMultiplier(hitNPCs.Count);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= Pandemic.DamageMultiplier(hitNPCs.Count);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.HasBuff(ModContent.BuffType<Plague>()))
            {
                target.AddBuff(ModContent.BuffType<Plague>(), 300); //Pandemic hits refresh Plague to 5 seconds
                if (!hitNPCs.Contains(target))
                {
                    hitNPCs.Add(target);
                    Projectile.localAI[0] = 0;

                    DirectionalPulseRing pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Main.rand.NextBool(3) ? Color.LimeGreen : Color.Green, Vector2.One, 0, 0f, 0.5f, 30);
                    GeneralParticleHandler.SpawnParticle(pulse);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
