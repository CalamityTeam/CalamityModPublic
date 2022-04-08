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
        }

        public override void SafeSetDefaults()
        {
            Item.width = 18;
            Item.height = 32;
            Item.damage = 98;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(gold: 80);
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;
            Item.shoot = ModContent.ProjectileType<CorpusAvertorProj>();
            Item.shootSpeed = 8.5f;
            Item.Calamity().rogue = true;
        }

        // Gains 10% of missing health as base damage.
        public override void SafeModifyWeaponDamage(Player player, ref StatModifier damage, ref float flat)
        {
            int lifeAmount = player.statLifeMax2 - player.statLife;
            flat += lifeAmount * 0.1f * player.RogueDamage();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int dagger = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CorpusAvertorStealth>(), (int)(damage * 3.5f), knockback * 2f, player.whoAmI);
                if (dagger.WithinBounds(Main.maxProjectiles))
                    Main.projectile[dagger].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}
