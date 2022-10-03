using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class CorpusAvertor : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corpus Avertor");
            Tooltip.SetDefault("Seems like it has worn down over time\n" +
                "Attacks grant lifesteal based on damage dealt\n" +
                "The lower your HP the more damage this weapon does and heals the player on enemy hits\n" +
                "Stealth strikes throw a single rainbow outlined dagger\n" +
                "On enemy hits, this dagger boosts the damage and life regen of all members of your team\n" +
                "However, there is a small chance it will cut your health in half instead");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 44;
            Item.damage = 98;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;
            Item.shoot = ModContent.ProjectileType<CorpusAvertorProj>();
            Item.shootSpeed = 8.5f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        // Gains 10% of missing health as base damage.
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            int lifeAmount = player.statLifeMax2 - player.statLife;
            damage.Base += lifeAmount * 0.1f;
        }

        public override void ModifyStatsExtra(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			if (player.Calamity().StealthStrikeAvailable())
				type = ModContent.ProjectileType<CorpusAvertorStealth>();
        }

		public override float StealthDamageMultiplier => 3.5f;
        public override float StealthKnockbackMultiplier => 2f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int dagger = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (dagger.WithinBounds(Main.maxProjectiles))
                    Main.projectile[dagger].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}
