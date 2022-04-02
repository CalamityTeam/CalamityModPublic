using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class FrostyFlare : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frosty Flare");
            Tooltip.SetDefault("Do not insert in flare gun\n" +
                "Sticks to enemies\n" +
                "Generates a localized hailstorm\n" +
                "Stealth strikes trail snowflakes and summon phantom copies instead of ice shards");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 32;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.width = 10;
            item.height = 22;
            item.useTime = 16;
            item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = false;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 0, 8, 0);
            item.rare = ItemRarityID.LightPurple;
            item.shoot = ModContent.ProjectileType<FrostyFlareProj>();
            item.shootSpeed = 22f;
            item.maxStack = 999;
            item.consumable = true;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int flare = Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<FrostyFlareStealth>(), (int)(damage * 0.9f), knockBack, player.whoAmI);
                if (flare.WithinBounds(Main.maxProjectiles))
                    Main.projectile[flare].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}
