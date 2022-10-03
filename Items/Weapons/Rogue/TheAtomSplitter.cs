using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class TheAtomSplitter : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Atom Splitter");
            Tooltip.SetDefault("Throws a quantum-superimposed javelin that strikes from numerous timelines at once\n" +
                "Stealth strikes perform far more simultaneous strikes");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 128;
            Item.damage = 320;
            Item.knockBack = 7f;
            Item.useAnimation = Item.useTime = 25;
            Item.DamageType = RogueDamageClass.Instance;
            Item.autoReuse = true;
            Item.shootSpeed = 24f;
            Item.shoot = ModContent.ProjectileType<TheAtomSplitterProjectile>();

            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
        }

		public override float StealthDamageMultiplier => 1.1f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int javelin = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, -1f);
            if (player.Calamity().StealthStrikeAvailable() && Main.projectile.IndexInRange(javelin))
			{
                Main.projectile[javelin].Calamity().stealthStrike = true;
            }
            return false;
        }
    }
}
