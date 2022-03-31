using Terraria.ModLoader;
using Terraria.ID;
using static CalamityMod.CalPlayer.CalamityPlayer;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class ArtemisMask : ModItem, IExtendedHat
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Artemis Mask");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 20;
            item.rare = ItemRarityID.Blue;
            item.vanity = true;
        }

        public override bool DrawHead() => false;

        public string ExtensionTexture() => "CalamityMod/Items/Armor/Vanity/ArtemisMask_Extra";
        public Vector2 ExtensionSpriteOffset(PlayerDrawInfo drawInfo) => new Vector2( drawInfo.drawPlayer.direction == 1f ? -16f : -10f, -10);
        public bool PreDrawExtension(PlayerDrawInfo drawInfo) => true;
    }
}
