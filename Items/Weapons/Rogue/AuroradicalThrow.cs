using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class AuroradicalThrow : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auroradical Throw");
            Tooltip.SetDefault("Launches a star that splits after a short period of time\n" +
                            "Split stars home in on nearby enemies after a few seconds\n" +
                            "Stealth strikes summon a meteor upon enemy impact");
        }

        public override void SafeSetDefaults()
        {
            item.width = 34;
            item.height = 58;
            item.damage = 32;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 30;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item117;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.shoot = ModContent.ProjectileType<AuroradicalSplitter>();
            item.shootSpeed = 10f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
                damage = (int)(damage * 1.2f);

            int star = Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            if (star.WithinBounds(Main.maxProjectiles))
                Main.projectile[star].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, Main.itemTexture[item.type]);
        }
    }
}
