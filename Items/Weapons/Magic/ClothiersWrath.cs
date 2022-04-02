using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ClothiersWrath : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clothier's Wrath");
            Tooltip.SetDefault("Shoots 3 shadowflame skulls");
        }

        public override void SetDefaults()
        {
            item.damage = 38;
            item.magic = true;
            item.mana = 15;
            item.width = 28;
            item.height = 30;
            item.useTime = 22;
            item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.UseSound = SoundID.Item8;
            item.autoReuse = true;
            item.shoot = ProjectileID.ClothiersCurse;
            item.shootSpeed = 6f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int numProj = 2;
            float rotation = MathHelper.ToRadians(2);
            for (int i = 0; i < numProj + 1; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                int proj = Projectile.NewProjectile(position, perturbedSpeed, type, damage, knockBack, player.whoAmI);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].Calamity().forceMagic = true;
            }
            return false;
        }
    }
}
