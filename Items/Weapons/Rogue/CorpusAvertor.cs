using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using CalamityMod.Systems.Collections;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class CorpusAvertor : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<HeavyBleeding>()];
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 44;
            Item.damage = 98;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;
            Item.shoot = ModContent.ProjectileType<CorpusAvertorProj>();
            Item.shootSpeed = 14f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override float StealthDamageMultiplier => 2.5f;
        public override float StealthKnockbackMultiplier => 2f;

        // Gains 10% of missing health as base damage.
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            int lifeAmount = player.statLifeMax2 - player.statLife;
            damage.Base += lifeAmount * 0.1f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int dagger = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CorpusAvertorStealth>(), damage, knockback, player.whoAmI);
                if (dagger.WithinBounds(Main.maxProjectiles))
                    Main.projectile[dagger].Calamity().stealthStrike = true;

                // Take 6 HP every time the stealth strike is used
                player.statLife -= 6;
                if (Main.myPlayer == player.whoAmI)
                    player.HealEffect(-6);
                if (player.statLife <= 0)
                    player.KillMe(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.CorpusAvertor").ToNetworkText(player.name)), 1000.0, 0, false);
                return false;
            }
            return true;
        }
    }
}
