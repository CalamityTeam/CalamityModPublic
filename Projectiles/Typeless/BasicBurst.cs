using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class BasicBurst : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Vector2 pushVelocity;
        public float customKnockback = 0;

        public bool hasStongDisplacement = false; // If you set Knockback to anything below zero, the custom knockback will be able to effect enemies that normally ignore knockback.

        // Projectile.ai[0] is the size of the circular hitbox.
        // Projectile.ai[1] is the minimum multiplier on pierce damage.
        // Projectile.ai[2] is the number of hits to reach minimum pierce damage.

        // You can use the local ai variables 0 - 2 to inflict up to two debuffs, if you need more than two debuffs just make your own projectile
        // 0 and 2 are what debuff is being inflicted
        // 1 is the debuff duration
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            // Get custom knockback strength.
            // Takes the knockback applied to the projectile to determine strength, then sets regular knockback to zero
            // Negative knockback is Abs'd and turns on strong knockback that effects more kinds of enemies
            if (customKnockback == 0)
            {
                if (Projectile.knockBack < 0)
                    hasStongDisplacement = true;
                customKnockback = Math.Abs(Projectile.knockBack);
                Projectile.knockBack = 0;
            }
            if (Projectile.ai[0] == 0)
                Projectile.ai[0] = 50; // The base size for explosions is 50.
            if (Projectile.ai[1] == 0)
                Projectile.ai[1] = 0.1f; // The base minimum damage multiplier is 0.1f.
            if (Projectile.ai[2] == 0)
                Projectile.ai[2] = 5; // The base number of hit to reach minimum damage is 5.
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.localAI[0] != 0) // Debuff 1
            {
                target.AddBuff((int)(Projectile.localAI[0]), (int)(Projectile.localAI[1]));
            }
            if (Projectile.localAI[2] != 0) // Debuff 2
            {
                target.AddBuff((int)(Projectile.localAI[2]), (int)(Projectile.localAI[1]));
            }
            pushVelocity = Utils.DirectionTo(Projectile.Center, target.Center);
            float minMult = Projectile.ai[1];
            int hitsToMinMult = (int)Projectile.ai[2];
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;

            if (customKnockback != 0)
                target.MoveNPC(pushVelocity, customKnockback, hasStongDisplacement, Main.player[Projectile.owner]);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.ai[0], targetHitbox);
        }
        public override void SendExtraAI(BinaryWriter writer) // Sending extra ai for the debuff infliction
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
            writer.Write(Projectile.localAI[2]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
            Projectile.localAI[2] = reader.ReadSingle();
        }
    }
}
