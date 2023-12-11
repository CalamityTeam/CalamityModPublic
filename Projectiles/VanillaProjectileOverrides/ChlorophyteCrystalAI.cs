using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using CalamityMod.Items.VanillaArmorChanges;

namespace CalamityMod.Projectiles.VanillaProjectileOverrides
{
    public static class ChlorophyteCrystalAI
    {
        public static bool DoChlorophyteCrystalAI(Projectile projectile)
        {
            Player owner = Main.player[projectile.owner];
            ref float timer = ref projectile.ai[1];

            timer++;
            projectile.aiStyle = -1;
            if (!owner.crystalLeaf)
            {
                projectile.Kill();
                return false;
            }

            // Stay above the player.
            projectile.Center = owner.Center + Vector2.UnitY * (owner.gfxOffY - 60f);
            if (Main.player[projectile.owner].gravDir == -1f)
            {
                projectile.position.Y += 120f;
                projectile.rotation = MathHelper.Pi;
            }
            else
                projectile.rotation = 0f;
            projectile.Center = projectile.Center.Floor();

            // Disable hardcoded spaghetti that fires crystal shots in vanilla code.
            owner.petalTimer = 2;

            // Have a periodically pulsing scale.
            projectile.scale = MathHelper.Lerp(0.9f, 1.15f, (float)Math.Sin(timer / 27f) * 0.5f + 0.5f);

            // Emit life pulses periodically.
            NPC potentialTarget = projectile.Center.ClosestNPCAt(560f, true, true);
            bool ownerNotFullHealth = owner.statLife < owner.statLifeMax2;
            bool willFirePulses = (potentialTarget is not null) || ownerNotFullHealth;
            if (timer % ChlorophyteArmorSetChange.PulseReleaseRate == ChlorophyteArmorSetChange.PulseReleaseRate - 1f && willFirePulses)
            {
                if (Main.myPlayer == projectile.owner)
                {
                    int pulseDamage = (int)owner.GetBestClassDamage().ApplyTo(ChlorophyteArmorSetChange.BaseDamageToEnemies);
                    pulseDamage = owner.ApplyArmorAccDamageBonusesTo(pulseDamage);

                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ModContent.ProjectileType<ChlorophyteLifePulse>(), pulseDamage, 0f, projectile.owner);
                }
            }

            return false;
        }

        public static bool DoChlorophyteCrystalDrawing(Projectile projectile)
        {
            // Why doesn't this work? How does one access the texture path of a vanilla projectile? Left bugged for someone else to figure out :)
            // It's me. I was that someone - Dominic
            // TextureAssets is the answer, along with Main.instance.LoadProjectile(projID), to ensure that the texture is loaded into memory.
            // In this instance, however, that should happen already on vanilla's end.
            Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * 0.5f;

            float backglowColorInterpolant = MathHelper.Lerp(0.44f, 0.88f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1.1f) * 0.5f + 0.5f);
            Color backglowColor = Color.Lerp(Color.Cyan, Color.Lime, backglowColorInterpolant) * 0.37f;
            backglowColor.A = 0;
            for (int i = 0; i < 8; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 3f;
                Main.EntitySpriteDraw(texture, drawPosition + drawOffset, null, projectile.GetAlpha(backglowColor), projectile.rotation, origin, projectile.scale, 0, 0);
            }

            Main.EntitySpriteDraw(texture, drawPosition, null, projectile.GetAlpha(Color.White * 0.75f), projectile.rotation, origin, projectile.scale, 0, 0);
            return false;
        }
    }
}
