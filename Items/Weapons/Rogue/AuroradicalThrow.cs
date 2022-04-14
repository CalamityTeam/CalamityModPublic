using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.GameContent;

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
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 34;
            Item.height = 58;
            Item.damage = 32;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 30;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item117;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<AuroradicalSplitter>();
            Item.shootSpeed = 10f;
            Item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
                damage = (int)(damage * 1.2f);

            int star = Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);
            if (star.WithinBounds(Main.maxProjectiles))
                Main.projectile[star].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, TextureAssets.Item[Item.type].Value);
        }
    }
}
