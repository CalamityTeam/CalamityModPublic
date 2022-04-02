using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class YanmeisKnifeSlash : ModProjectile
    {
        // This is a rather weird thing, but it's what the patron asked for.
        public static readonly Func<NPC, bool> CanRecieveCoolEffectsFrom = (npc) =>
        {
            bool validBoss = npc.boss && npc.type != ModContent.NPCType<CeaselessVoid>()
                && npc.type != ModContent.NPCType<DevourerofGodsBody>()
                && npc.type != ModContent.NPCType<AstrumDeusBodySpectral>()
                && npc.type != ModContent.NPCType<ThanatosHead>()
                && npc.type != ModContent.NPCType<ThanatosBody1>()
                && npc.type != ModContent.NPCType<ThanatosBody2>()
                && npc.type != ModContent.NPCType<ThanatosTail>();
            if (validBoss)
                return true;
            bool bossMinion = CalamityLists.bossMinionList.Contains(npc.type);
            if (bossMinion)
                return true;
            bool miniboss = CalamityLists.minibossList.Contains(npc.type) || AcidRainEvent.AllMinibosses.Contains(npc.type);
            return miniboss;
        };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yanmei's Knife");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 180;
            projectile.height = 96;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ownerHitCheck = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            // Frames and crap
            projectile.frameCounter++;
            if (projectile.frameCounter % 7 == 0)
            {
                projectile.frame++;
                if (projectile.frame >= Main.projFrames[projectile.type])
                    projectile.Kill();
            }

            // Create idle light and dust.
            Vector2 origin = projectile.Center + projectile.velocity * 3f;
            Lighting.AddLight(origin, 0f, 1.5f, 0.1f);

            Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);

            // Rotation and directioning.
            float velocityAngle = projectile.velocity.ToRotation();
            projectile.rotation = velocityAngle + (projectile.spriteDirection == -1).ToInt() * MathHelper.Pi;
            projectile.direction = (Math.Cos(velocityAngle) > 0).ToDirectionInt();

            // Positioning close to the end of the player's arm.
            projectile.position = playerRotatedPoint - projectile.Size * 0.5f + velocityAngle.ToRotationVector2() * 80f;

            // Sprite and player directioning.
            projectile.spriteDirection = projectile.direction;
            player.ChangeDir(projectile.direction);

            // Prevents the projectile from dying
            projectile.timeLeft = 2;

            // Player item-based field manipulation.
            player.itemRotation = (projectile.velocity * projectile.direction).ToRotation();
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
        }

        public void HandleAttachmentMovement(Player player, Vector2 playerRotatedPoint)
        {
            float speed = 1f;
            if (player.ActiveItem().shoot == projectile.type)
            {
                speed = player.ActiveItem().shootSpeed * projectile.scale;
            }
            Vector2 newVelocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction) * speed;    

            // Sync if a velocity component changes.
            if (projectile.velocity.X != newVelocity.X || projectile.velocity.Y != newVelocity.Y)
            {
                projectile.netUpdate = true;
            }
            projectile.velocity = newVelocity;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!CanRecieveCoolEffectsFrom(target))
                return;
            target.AddBuff(ModContent.BuffType<KamiDebuff>(), 600);
            if (!Main.dedServ)
            {
                for (int i = 0; i < 60; i++)
                {
                    Dust dust = Dust.NewDustDirect(Main.player[projectile.owner].position, Main.player[projectile.owner].width, Main.player[projectile.owner].height, 267);
                    dust.position.X += Main.rand.NextFloat(-16f, 16f);
                    dust.color = Main.hslToRgb(Main.rand.NextFloat(0.26f, 0.37f), 1f, 0.75f);
                    dust.velocity = Main.rand.NextVector2Circular(24f, 24f);
                    dust.scale = Main.rand.NextFloat(1.4f, 1.8f);
                    dust.noGravity = true;
                }
            }
            if (projectile.ai[0] == 0f)
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/YanmeiKnifeHit"), (int)projectile.position.X, (int)projectile.position.Y);
                projectile.ai[0] = 1f;
            }
            Main.player[projectile.owner].AddBuff(ModContent.BuffType<KamiBuff>(), 600);
        }
        public override Color? GetAlpha(Color lightColor) => new Color(0, 215, 0, 0);
    }
}
