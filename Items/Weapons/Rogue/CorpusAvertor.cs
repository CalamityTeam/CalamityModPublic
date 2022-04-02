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
            item.width = 18;
            item.height = 32;
            item.damage = 98;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 15;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(gold: 80);
            item.rare = ItemRarityID.Yellow;
            item.Calamity().donorItem = true;
            item.shoot = ModContent.ProjectileType<CorpusAvertorProj>();
            item.shootSpeed = 8.5f;
            item.Calamity().rogue = true;
        }

        // Gains 10% of missing health as base damage.
        public override void SafeModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            int lifeAmount = player.statLifeMax2 - player.statLife;
            flat += lifeAmount * 0.1f * player.RogueDamage();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int dagger = Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<CorpusAvertorStealth>(), (int)(damage * 3.5f), knockBack * 2f, player.whoAmI);
                if (dagger.WithinBounds(Main.maxProjectiles))
                    Main.projectile[dagger].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}
