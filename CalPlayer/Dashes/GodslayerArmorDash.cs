using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Enums;
using CalamityMod.Items.Armor.GodSlayer;
using CalamityMod.NPCs.DevourerofGods;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.Dashes
{
    public class GodslayerArmorDash : PlayerDashEffect
    {
        public static new string ID => "Godslayer Armor";

        public static int GodslayerCooldown = 45;

        public override DashCollisionType CollisionType => DashCollisionType.ShieldSlam;

        public override bool IsOmnidirectional => true;

        public override float CalculateDashSpeed(Player player) => 80f;

        public override void OnDashEffects(Player player)
        {
            SoundEngine.PlaySound(DevourerofGodsHead.AttackSound, player.Center);

            for (int d = 0; d < 60; d++)
            {
                Dust cosmiliteDust = Dust.NewDustDirect(player.position, player.width, player.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                cosmiliteDust.position += Main.rand.NextVector2Square(-5f, 5f);
                cosmiliteDust.velocity *= 0.2f;
                cosmiliteDust.scale *= Main.rand.NextFloat(1f, 1.2f);
                cosmiliteDust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                cosmiliteDust.noGravity = true;
                cosmiliteDust.fadeIn = 0.5f;
            }
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            // Fall way, way, faster than usual.
            player.maxFallSpeed = 50f;

            for (int m = 0; m < 24; m++)
            {
                Dust cosmiliteDust = Dust.NewDustDirect(new Vector2(player.position.X, player.position.Y + 4f), player.width, player.height - 8, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2.75f);
                cosmiliteDust.velocity *= 0.1f;
                cosmiliteDust.scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                cosmiliteDust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                cosmiliteDust.noGravity = true;
                if (Main.rand.NextBool(2))
                {
                    cosmiliteDust.fadeIn = 0.5f;
                }
            }

            // Dash at a much, much faster speed than the default value.
            dashSpeed = 40f;
            runSpeedDecelerationFactor = 0.8f;

            // Cooldown for God Slayer Armor dash.
            player.AddCooldown(Cooldowns.GodSlayerDash.ID, CalamityUtils.SecondsToFrames(GodslayerCooldown));
            player.Calamity().godSlayerDashHotKeyPressed = false;
        }

        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
        {
            SoundEngine.PlaySound(SoundID.Item67, player.Center);

            for (int j = 0; j < 30; j++)
            {
                Dust cosmiliteDust = Dust.NewDustDirect(player.position, player.width, player.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                cosmiliteDust.position += Main.rand.NextVector2Square(-20f, 20f);
                cosmiliteDust.velocity *= 0.9f;
                cosmiliteDust.scale *= Main.rand.NextFloat(1f, 1.4f);
                cosmiliteDust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                if (Main.rand.NextBool(2))
                    cosmiliteDust.scale *= Main.rand.NextFloat(1f, 1.4f);
            }

            float kbFactor = 15f;
            bool crit = Main.rand.Next(100) < player.GetCritChance<MeleeDamageClass>();
            if (player.kbGlove)
                kbFactor *= 2f;
            if (player.kbBuff)
                kbFactor *= 1.5f;

            int hitDirection = player.direction;
            if (player.velocity.X != 0f)
                hitDirection = Math.Sign(player.velocity.X);

            // Define hit context variables.
            hitContext.CriticalHit = crit;
            hitContext.HitDirection = hitDirection;
            hitContext.KnockbackFactor = kbFactor;

            // God Slayer Dash intentionally does not use the vanilla function for collision attack iframes.
            // This is because its immunity is meant to be completely consistent and not subject to vanilla anticheese.
            hitContext.PlayerImmunityFrames = GodSlayerChestplate.DashIFrames;
            hitContext.Damage = (int)player.GetBestClassDamage().ApplyTo(3000f);

            npc.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
        }
    }
}
