using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class CelestialReaper : RogueWeapon
    {
        public const int BaseDamage = 140;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestial Reaper");
            Tooltip.SetDefault("Throws a fast homing scythe\n" +
                               "The scythe will bounce after hitting an enemy up to six times\n" +
                               "Stealth strikes create damaging afterimages");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = BaseDamage;
            Item.width = 66;
            Item.height = 76;
            Item.useAnimation = 31;
            Item.useTime = 31;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.shoot = ModContent.ProjectileType<CelestialReaperProjectile>();
            Item.shootSpeed = 20f;
            Item.DamageType = RogueDamageClass.Instance;
        }

		public override float StealthDamageMultiplier => 0.9f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool usingStealth = player.Calamity().StealthStrikeAvailable();
            float strikeValue = usingStealth.ToInt(); //0 if false, 1 if true

            int p = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CelestialReaperProjectile>(), damage, knockback, player.whoAmI, strikeValue);
            if (player.Calamity().StealthStrikeAvailable() && p.WithinBounds(Main.maxProjectiles))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }
}
