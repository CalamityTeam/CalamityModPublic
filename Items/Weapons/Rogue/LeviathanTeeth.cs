using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class LeviathanTeeth : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Leviathan Teeth");
            Tooltip.SetDefault("Rapidly throws a variety of poisonous fangs that stick to enemies\n" +
                "Stealth strikes cause 6 very fast teeth to be thrown, ignoring gravity and inflicting extreme knockback");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 38;
            Item.damage = 66;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<LeviathanTooth>();
            Item.shootSpeed = 12f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool stealthStrike = false;
            int teethCount;
            if (player.Calamity().StealthStrikeAvailable())
            {
                teethCount = 6;
                stealthStrike = true;
            }
            else
            {
                teethCount = Main.rand.Next(1, 3);
            }

            float spreadAngle = MathHelper.ToRadians(10);

            for (int i = 0; i < teethCount; i++)
            {
                float offsetSpeedX = velocity.X + Main.rand.NextFloat(-4f, 4f);
                float offsetSpeedY = velocity.Y + Main.rand.NextFloat(-4f, 4f);

                if (stealthStrike)
                {
                    int tooth = Projectile.NewProjectile(source, position.X, position.Y, offsetSpeedX * 1.5f, offsetSpeedY * 1.5f, type, damage, knockback * 10f, player.whoAmI);
                    if (tooth.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[tooth].Calamity().stealthStrike = true;
                        Main.projectile[tooth].Calamity().lineColor = Main.rand.Next(3);
                    }
                }
                else
                {
                    int tooth = Projectile.NewProjectile(source, position.X, position.Y, offsetSpeedX, offsetSpeedY, type, damage, knockback, player.whoAmI);
                    if (tooth.WithinBounds(Main.maxProjectiles))
                        Main.projectile[tooth].Calamity().lineColor = Main.rand.Next(3);
                }
            }
            return false;
        }
    }
}
