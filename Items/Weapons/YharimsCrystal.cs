using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class YharimsCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yharim's Crystal");
            Tooltip.SetDefault("Fires a beam of complete destruction\n" +
                "Only those that are worthy can use this item before Yharon is defeated");
        }

        public override void SetDefaults()
        {
            item.damage = 240;
            item.magic = true;
            item.mana = 100;
            item.width = 16;
            item.height = 16;
            item.useTime = 10;
            item.useAnimation = 10;
            item.reuseDelay = 5;
            item.useStyle = 5;
            item.UseSound = SoundID.Item13;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.knockBack = 0f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("YharimsCrystal");
            item.shootSpeed = 30f;
            item.Calamity().postMoonLordRarity = 17;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            bool playerName =
                player.name == "Fabsol" ||
                player.name == "Ziggums" ||
                player.name == "Poly" ||
                player.name == "Zach" ||
                player.name == "Grox the Great" ||
                player.name == "Jenosis" ||
                player.name == "DM DOKURO" ||
                player.name == "Uncle Danny" ||
                player.name == "Phoenix" ||
                player.name == "MineCat" ||
                player.name == "Khaelis" ||
                player.name == "Purple Necromancer" ||
                player.name == "gamagamer64" ||
                player.name == "Svante" ||
                player.name == "Puff" ||
                player.name == "Leviathan" ||
                player.name == "Testdude";
            bool yharon = CalamityWorld.downedYharon;
            if (playerName || yharon)
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("YharimsCrystal"), damage, knockBack, player.whoAmI, 0f, 0f);
                return false;
            }
            else
            {
                Projectile.NewProjectile(position.X, position.Y, 0f, 0f, 29, 0, 0f, player.whoAmI, 0f, 0f);
                return false;
            }
        }
    }
}
