using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class MoltenAmputator : RogueWeapon
    {
        public float speed = 16;
        public static int FlurryCount => 30;
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<HolyFlames>()];
        }
        public override void SetDefaults()
        {
            Item.width = 92;
            Item.height = 80;
            Item.damage = 200;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 25;
            Item.knockBack = 2f; // It needs to be low otherwise enemies dont stay in the slash zone
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.shoot = ModContent.ProjectileType<MoltenAmputatorProj>();
            Item.shootSpeed = speed;
            Item.DamageType = RogueDamageClass.Instance;
        }
        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;
            if (player.Calamity().mouseRight)
            {
                if (player.Calamity().StealthStrikeAvailable() && player.Calamity().focusFlurryAttackCount < FlurryCount)
                {
                    player.Calamity().focusFlurryAttackCount = FlurryCount;
                    player.Calamity().ConsumeStealthByAttacking();
                    SoundStyle buff = new("CalamityMod/Sounds/Custom/ProfanedGuardians/GuardianRay");
                    SoundEngine.PlaySound(buff with { Volume = 1f, Pitch = Main.rand.NextFloat(0.2f, 0.3f) }, player.Center);

                    for (int i = 0; i < 20; i++)
                    {
                        Dust c = Dust.NewDustPerfect(player.Center, ModContent.DustType<LightDust>());
                        c.velocity = (MathHelper.TwoPi * i / 20f).ToRotationVector2() * 15.5f * (i % 2 == 0 ? 0.88f : 1f);
                        c.scale = Main.rand.NextFloat(1.3f, 1.6f) * 0.8f * (i % 2 == 0 ? 2.2f : 1.8f);
                        c.noGravity = true;
                        c.color = Color.Goldenrod;
                        c.noLightEmittence = true;
                    }
                }
                for (int x = 0; x < Main.maxProjectiles; x++)
                {
                    Projectile projectile = Main.projectile[x];
                    if (projectile.active && projectile.type == ModContent.ProjectileType<MoltenAmputatorProj>() && projectile.ai[2] < 5 && projectile.timeLeft < 840)
                    {
                        projectile.ai[2] = 5;
                        SoundStyle pullback = new("CalamityMod/Sounds/Item/SwingMid");
                        SoundEngine.PlaySound(pullback with { Volume = 0.4f, Pitch = Main.rand.NextFloat(0.4f, 0.5f) }, player.Center);
                    }
                }
            }
        }
        public override float UseSpeedMultiplier(Player player)
        {
            return (player.Calamity().focusFlurryAttackCount > 0 ? 3f : 1);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool fastToss = player.Calamity().focusFlurryAttackCount > 0;

            SoundStyle fire = new("CalamityMod/Sounds/Item/SpearofDestiny");
            SoundEngine.PlaySound(fire with { Volume = 0.5f, Pitch = (player.Calamity().focusFlurryAttackCount > 0 ? -0.4f + (player.Calamity().focusFlurryAttackCount * 0.02f) : Main.rand.NextFloat(-0.4f, -0.65f)) }, position);
            // Since the positioning of the scythe is important, its velocity is based on your mouse position
            Vector2 staticSpeed = Utils.DirectionTo(position, position + velocity) * Utils.Distance(position, player.ClampedMouseWorld()) * 0.022f;
            // "fast toss" is the stealth, if you need to change stealth values, change those
            int fastTossDamage = (int)(damage * 0.65f);
            Projectile scythe = Projectile.NewProjectileDirect(source, position, staticSpeed.RotatedByRandom((player.Calamity().focusFlurryAttackCount > 0 ? 0.7f : 0)), type, fastToss ? fastTossDamage : damage, knockback, player.whoAmI, 0, 0, 0);
            if (fastToss)
            {
                scythe.extraUpdates = 6;
                scythe.Calamity().stealthStrike = true;
                player.Calamity().focusFlurryAttackCount--;
            }
            player.Calamity().ConsumeStealthByAttacking();

            return false;
        }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FlurryCount);
    }
}
