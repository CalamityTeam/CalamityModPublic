using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Quasar : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Quasar");
            Tooltip.SetDefault("Succ\n" +
			"Stealth strikes spawn more explosions\n" +
			"Rare Item Variant");
        }

        public override void SafeSetDefaults()
        {
            item.width = 52;
            item.damage = 80; //50
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 12;
            item.knockBack = 0f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 48;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = ItemRarityID.Cyan;
            item.shoot = ModContent.ProjectileType<QuasarKnife>();
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;
            item.Calamity().customRarity = CalamityRarity.RareVariant;
        }

		// Terraria seems to really dislike high crit values in SetDefaults
		public override void GetWeaponCrit(Player player, ref int crit) => crit += 12;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int knife = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
			if (knife.WithinBounds(Main.maxProjectiles))
				Main.projectile[knife].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }
}
