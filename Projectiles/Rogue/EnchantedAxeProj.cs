using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class EnchantedAxeProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/EnchantedAxe";

        private bool recall = false;
        private bool summonAxe = true;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Projectile.Calamity().stealthStrike)
            {
                if (Projectile.timeLeft < 585)
                {
                    recall = true;
                    Projectile.tileCollide = false;
                }
            }
            else
            {
                if (Projectile.timeLeft < 590)
                {
                    recall = true;
                    Projectile.tileCollide = false;
                }
            }

            Projectile.rotation += 0.4f * Projectile.direction;

            if (recall)
            {
                Vector2 posDiff = Main.player[Projectile.owner].position - Projectile.position;
                if (posDiff.Length() > 30f)
                {
                    posDiff.Normalize();
                    Projectile.velocity = posDiff * 30f;
                }
                else
                {
                    Projectile.timeLeft = 0;
                    OnKill(Projectile.timeLeft);
                }

                if (summonAxe)
                {
                    float minDist = 999f;
                    int index = 0;
                    // Get the closest enemy to the axe
                    for (int i = 0; i < Main.npc.Length; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(Projectile, false))
                        {
                            float dist = (Projectile.Center - npc.Center).Length();
                            if (dist < minDist)
                            {
                                minDist = dist;
                                index = i;
                            }
                        }
                    }
                    Vector2 newAxeVelocity;
                    if (minDist < 999f)
                    {
                        newAxeVelocity = Main.npc[index].Center - Projectile.Center;
                    }
                    else
                    {
                        newAxeVelocity = -Projectile.velocity;
                    }
                    newAxeVelocity.Normalize();
                    newAxeVelocity *= 20f;
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, newAxeVelocity, ModContent.ProjectileType<EnchantedAxe2>(), (int)(Projectile.damage * 1.2f), 2, Projectile.owner, 0, 0);
                    Main.projectile[p].Calamity().stealthStrike = Projectile.Calamity().stealthStrike;
                    summonAxe = false;
                }
            }
            else
            {
                if (Projectile.timeLeft % 7 == 1 && Projectile.Calamity().stealthStrike)
                {
                    float axeSpeed = 15f;
                    int axeDamage = Projectile.damage / 2;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(1f, 0f) * axeSpeed, ModContent.ProjectileType<EnchantedAxe2>(), axeDamage, 2, Projectile.owner, 0, 0);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(0f, 1f) * axeSpeed, ModContent.ProjectileType<EnchantedAxe2>(), axeDamage, 2, Projectile.owner, 0, 0);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(-1f, 0f) * axeSpeed, ModContent.ProjectileType<EnchantedAxe2>(), axeDamage, 2, Projectile.owner, 0, 0);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(0f, -1f) * axeSpeed, ModContent.ProjectileType<EnchantedAxe2>(), axeDamage, 2, Projectile.owner, 0, 0);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Vector2.Normalize(new Vector2(1f, 1f)) * axeSpeed, ModContent.ProjectileType<EnchantedAxe2>(), axeDamage, 2, Projectile.owner, 0, 0);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Vector2.Normalize(new Vector2(1f, -1f)) * axeSpeed, ModContent.ProjectileType<EnchantedAxe2>(), axeDamage, 2, Projectile.owner, 0, 0);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Vector2.Normalize(new Vector2(-1f, -1f)) * axeSpeed, ModContent.ProjectileType<EnchantedAxe2>(), axeDamage, 2, Projectile.owner, 0, 0);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Vector2.Normalize(new Vector2(-1f, 1f)) * axeSpeed, ModContent.ProjectileType<EnchantedAxe2>(), axeDamage, 2, Projectile.owner, 0, 0);
                }
            }

            if (Projectile.position == Main.player[Projectile.owner].position)
            {
                OnKill(Projectile.timeLeft);
            }
            return;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (recall)
            {
                return false;
            }
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            recall = true;
            Projectile.tileCollide = false;
            return false;
        }
    }
}
