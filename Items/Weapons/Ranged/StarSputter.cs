using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class StarSputter : ModItem
    {
        private int counter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Sputter");
            Tooltip.SetDefault("Fires a chain of comets\n" +
            "Fires a bigger, more powerful comet every four rounds\n" +
            "Look to the stars for a galaxy far, far away");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 112;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 80;
            Item.height = 26;
            Item.useTime = 8;
            Item.reuseDelay = 15;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 15f;
            Item.value = Item.buyPrice(0, 95, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item92;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SputterComet>();
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.FallenStar;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool CanConsumeAmmo(Player player) //consume ammo only once per round
        {
            if (counter == 1 || counter == 2 || counter == 4 || counter == 5 || counter == 7 || counter == 8 || counter == 10 || counter == 11)
                return false;
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            counter++;
            if (counter == 10)
            {
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X * 0.8f, velocity.Y * 0.8f, ModContent.ProjectileType<SputterCometBig>(), (int)(damage * 1.5f), knockback, player.whoAmI, 0f, 0f);
            }
            if (counter >= 12)
                counter = 0;
            return true;
        }
    }
}
