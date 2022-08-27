using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ScourgeoftheSeas : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scourge of the Seas");
            Tooltip.SetDefault("Snaps apart into a venomous cloud upon striking an enemy\n" +
            "Stealth strikes are coated with vile toxins, afflicting enemies with a powerful debuff");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.knockBack = 3.5f;
            Item.useAnimation = Item.useTime = 20;
            Item.autoReuse = true;
            Item.DamageType = RogueDamageClass.Instance;
            Item.shootSpeed = 8f;
            Item.shoot = ModContent.ProjectileType<ScourgeoftheSeasProjectile>();

            Item.width = 64;
            Item.height = 66;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(0, 36, 0, 0);
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.RogueWeapon;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ScourgeoftheSeasProjectile>(), damage, knockback, player.whoAmI, 0f, 1f);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}
