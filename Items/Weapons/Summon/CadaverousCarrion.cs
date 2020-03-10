using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CadaverousCarrion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cadaverous Carrion");
            Tooltip.SetDefault("Summons a gross Old Duke fishron head on the ground");
        }

        public override void SetDefaults()
        {
            item.damage = 400;
            item.mana = 32;
            item.summon = true;
            item.sentry = true;
            item.width = 54;
            item.height = 56;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = 1;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.UseSound = SoundID.NPCDeath13;
            item.shoot = ModContent.ProjectileType<OldDukeHeadCorpse>();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld;
                Point mouseTileCoords = position.ToTileCoordinates();
                if (WorldGen.SolidTile(mouseTileCoords.X, mouseTileCoords.Y))
                    return false;
                Projectile.NewProjectile(position, Vector2.Zero, type, damage, knockBack, player.whoAmI);
                player.UpdateMaxTurrets();
            }
            return false;
        }
    }
}
