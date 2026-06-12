using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class CosmicKunai : RogueWeapon
    {
        private bool stealthSalvo = false;

        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Laceration>(), ModContent.BuffType<WhisperingDeath>()];
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 48;
            Item.damage = 92;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTime = 2;
            Item.useAnimation = 10;
            Item.reuseDelay = 1;
            Item.useLimitPerAnimation = 5;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item109;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.shoot = ModContent.ProjectileType<CosmicKunaiProj>();
            Item.shootSpeed = 28f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile kunai = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable())
            {
                stealthSalvo = true;
                kunai.Calamity().stealthStrike = true;
                kunai.penetrate = 5;
                kunai.ai[2] = 1f; // Actually dictates the "travels farther" part
                SoundEngine.PlaySound(SoundID.Item73, player.Center);
                for (float i = 0; i < 9; i++)
                {
                    float angle = MathHelper.TwoPi / 9f * i;
                    Projectile.NewProjectile(source, player.Center, angle.ToRotationVector2() * 8f, ModContent.ProjectileType<CosmicScythe>(), damage * 3, knockback, player.whoAmI, angle);
                }
            }
            else if (stealthSalvo)
            {
                kunai.penetrate = 5;
                kunai.ai[2] = 1f;
            }

            if (player.ItemUsesThisAnimation == Item.useLimitPerAnimation)
                stealthSalvo = false;
            return false;
        }
    }
}
