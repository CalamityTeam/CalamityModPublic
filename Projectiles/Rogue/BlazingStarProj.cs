using CalamityMod.Packets.Entities;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BlazingStarProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/BlazingStar";

        private static int Lifetime = 300;
        private int timeAlive => Lifetime - Projectile.timeLeft;
        private bool hasHit = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 3;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Lifetime = 300;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 6;
            Projectile.MaxUpdates = 3;
            Projectile.timeLeft = Lifetime;
            DrawOffsetX = -10;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.rotation += 0.175f * Projectile.direction;

            if (timeAlive == 0)
            {
                Projectile.localNPCHitCooldown = -1;
                if (Projectile.Calamity().stealthStrike)
                {
                    Projectile.penetrate = -1;
                    Projectile.MaxUpdates++;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        void Ricochet()
        {
            float maxDistance = 1600f;
            float npcDistCompare = 1600f;
            int index = -1;
            foreach (NPC n in Main.ActiveNPCs) // Find an NPC to ricochet to
            {
                if (!n.CanBeChasedBy(Projectile) || !Projectile.WithinRange(n.Center, maxDistance) || Projectile.localNPCImmunity[n.whoAmI] != 0)
                    continue;

                float currentNPCDist = Vector2.Distance(n.Center, Projectile.Center);
                if ((currentNPCDist < npcDistCompare) && (Collision.CanHit(Projectile.Center, 1, 1, n.Center, 1, 1)))
                {
                    npcDistCompare = currentNPCDist;
                    index = n.whoAmI;
                }
            }

            // If you find an NPC, ricochet in their direction and reset iframes for them
            if (index != -1)
            {
                Projectile.ai[1] = index;
                Projectile.velocity = CalamityUtils.CalculatePredictiveAimToTargetMaxUpdates(Projectile.Center, Main.npc[index], Projectile.velocity.Length(), Projectile.MaxUpdates);
                Projectile.netUpdate = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8f);
            if (Projectile.Calamity().stealthStrike)
            {
                if (!hasHit)
                {
                    target.Calamity().blazingStarShredTimer += 300;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), 0, 0, Projectile.owner, 0f, 0.1f);

                    // Shred must be synced, because OnHitNPC is only run for the client that hit the NPC
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        GlaiveShredPacket.Send(target);
                }
                hasHit = true;
            }
            Ricochet();
        }


        // Make it bounce on tiles.
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Impacts the terrain even though it bounces off.
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.ResetLocalNPCHitImmunity();
            Ricochet();
            Projectile.damage = (int)(Projectile.damage * 0.8f);
            if (Projectile.penetrate > 0)
                Projectile.penetrate--;
            return false;
        }
    }
}
